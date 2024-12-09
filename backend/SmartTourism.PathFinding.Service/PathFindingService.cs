using SmartTourism.PathFinding.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartTourism.PathFinding.Service
{
    public class PathFindingService
    {
        private readonly List<Location> _locations;
        private readonly List<UpdatedEdge> _edges;

        public PathFindingService(List<Location> locations, List<UpdatedEdge> edges)
        {
            _locations = locations;
            _edges = edges;
        }

        /// <summary>
        /// Performs clustering on the given locations.
        /// </summary>
        public Dictionary<int, List<Location>> PerformClustering(int numberOfClusters)
        {
            var random = new Random();
            var centroids = _locations.OrderBy(_ => random.Next()).Take(numberOfClusters).ToList();
            var clusters = new Dictionary<int, List<Location>>();

            bool changed;
            do
            {
                clusters = centroids
                    .Select((c, i) => (i, new List<Location>()))
                    .ToDictionary(x => x.i, x => x.Item2);

                foreach (var location in _locations)
                {
                    var nearestCentroid = centroids
                        .Select((c, i) => (i, CalculateDistance(location, c)))
                        .OrderBy(x => x.Item2)
                        .First().i;

                    clusters[nearestCentroid].Add(location);
                }

                changed = false;
                for (int i = 0; i < centroids.Count; i++)
                {
                    var clusterLocations = clusters[i];
                    if (clusterLocations.Count > 0)
                    {
                        var newCentroid = new Location
                        {
                            Longitude = clusterLocations.Average(l => l.Longitude),
                            Latitude = clusterLocations.Average(l => l.Latitude),
                            Id = centroids[i].Id,
                            Name = centroids[i].Name
                        };

                        if (CalculateDistance(centroids[i], newCentroid) > 1e-6)
                        {
                            centroids[i] = newCentroid;
                            changed = true;
                        }
                    }
                }
            } while (changed);

            return clusters;
        }

        /// <summary>
        /// Finds the optimal path using A* algorithm with a learned heuristic.
        /// </summary>
        public List<Location> FindOptimalPath(Guid startId, Guid endId, double alpha, double beta, double targetWeight)
        {
            var graph = _edges.GroupBy(e => e.From.Id)
                .ToDictionary(g => g.Key, g => g.Select(e => (e.To, e.Distance)).ToList());

            var openSet = new SortedSet<(double FScore, Guid Node)>(Comparer<(double, Guid)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));
            var gScore = _locations.ToDictionary(l => l.Id, _ => double.PositiveInfinity);
            var cameFrom = new Dictionary<Guid, Guid>();
            var fScore = _locations.ToDictionary(l => l.Id, _ => double.PositiveInfinity);

            gScore[startId] = 0;
            fScore[startId] = alpha * Math.Abs(targetWeight - gScore[startId]) +
                              beta * CalculateDistance(GetLocationById(startId), GetLocationById(endId));
            openSet.Add((fScore[startId], startId));

            List<Guid> bestPath = null;
            double bestScore = double.PositiveInfinity;

            while (openSet.Any())
            {
                var current = openSet.First().Node;
                openSet.Remove(openSet.First());

                if (current == endId)
                {
                    double currentScore = Math.Abs(targetWeight - gScore[current]);
                    if (currentScore < bestScore)
                    {
                        bestScore = currentScore;
                        bestPath = ReconstructPath(cameFrom, current).Select(loc => loc.Id).ToList();

                        // If an exact match is found, return immediately
                        if (bestScore == 0)
                        {
                            return ReconstructPath(cameFrom, current);
                        }
                    }
                    continue;
                }

                if (!graph.ContainsKey(current)) continue;

                foreach (var (neighbor, weight) in graph[current])
                {
                    var tentativeGScore = gScore[current] + weight;
                    var tentativeFScore = alpha * Math.Abs(targetWeight - tentativeGScore) +
                                          beta * CalculateDistance(GetLocationById(neighbor.Id), GetLocationById(endId));

                    if (tentativeGScore < gScore[neighbor.Id])
                    {
                        cameFrom[neighbor.Id] = current;
                        gScore[neighbor.Id] = tentativeGScore;
                        fScore[neighbor.Id] = tentativeFScore;

                        openSet.Add((fScore[neighbor.Id], neighbor.Id));
                    }
                }
            }

            if (bestPath != null)
            {
                // Return the best path found
                return bestPath.Select(id => GetLocationById(id)).ToList();
            }

            return new List<Location>(); // No path found
        }


        /// <summary>
        /// Creates a sparse graph using k-nearest neighbors.
        /// </summary>
        public List<UpdatedEdge> CreateSparseGraph(int k)
        {
            var edges = new List<UpdatedEdge>();

            foreach (var location in _locations)
            {
                var neighbors = _locations
                    .Where(l => l.Id != location.Id)
                    .Select(l => new UpdatedEdge
                    {
                        From = location,
                        To = l,
                        Distance = CalculateDistance(location, l),
                        Duration = CalculateDistance(location, l) / 50, // Assume avg speed = 50
                        TimeSpend = 0 // Placeholder
                    })
                    .OrderBy(e => e.Distance)
                    .Take(k);

                edges.AddRange(neighbors);
            }

            return edges;
        }

        private Location GetLocationById(Guid id)
        {
            return _locations.First(l => l.Id == id);
        }

        private List<Location> ReconstructPath(Dictionary<Guid, Guid> cameFrom, Guid current)
        {
            var path = new List<Location>();
            while (cameFrom.ContainsKey(current))
            {
                path.Insert(0, GetLocationById(current));
                current = cameFrom[current];
            }

            path.Insert(0, GetLocationById(current));
            return path;
        }

        private static double CalculateDistance(Location p1, Location p2)
        {
            return CalculateHaversineDistance(p1, p2);
        }
        
        public static double CalculateHaversineDistance(Location p1, Location p2)
        {
            const double EarthRadiusKm = 6371; // Earth's radius in kilometers

            double lat1Rad = DegreesToRadians(p1.Latitude);
            double lon1Rad = DegreesToRadians(p1.Longitude);
            double lat2Rad = DegreesToRadians(p2.Latitude);
            double lon2Rad = DegreesToRadians(p2.Longitude);

            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c; // Distance in kilometers
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}

