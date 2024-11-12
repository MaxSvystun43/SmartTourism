namespace SmartTourism.PathFinding.Service.Models;

public class Graph
{
    public List<Location> Locations { get; set; } = [];
    public List<Edge> Edges { get; set; } = [];
    public Dictionary<Location, List<Location>> GraphData { get; set; } = [];
    
    private const double EarthRadiusKm = 6371;

    public void AddPosition(Location position)
    {
        Locations.Add(position);
    }
    public void BuildNearestNeighborsGraph(int neighborsCount)
    {
        var visitedPairs = new HashSet<(Location, Location)>();
        var usedPositions = new HashSet<Location>();

        foreach (var position in Locations)
        {
            // Skip this position if it has already been used in another edge
            if (usedPositions.Contains(position))
                continue;

            // Calculate distance from the current position to all other positions
            var nearestPositions = Locations
                .Where(p => p != position && !usedPositions.Contains(p)) // Exclude already used positions
                .Select(p => new { Position = p, Distance = HaversineDistance(position, p) })
                .OrderBy(p => p.Distance)
                .Take(neighborsCount) // Choose the closest 'neighborsCount' positions
                .ToList();

            // Create edges to the nearest neighbors
            foreach (var nearest in nearestPositions)
            {
                var fromToPair = (position, nearest.Position);
                var toFromPair = (nearest.Position, position);

                // Only add the edge if neither direction has been added before
                if (!visitedPairs.Contains(fromToPair) && !visitedPairs.Contains(toFromPair))
                {
                    Edges.Add(new Edge
                    {
                        From = position,
                        To = nearest.Position,
                        Distance = nearest.Distance
                    });

                    // Mark both directions as visited
                    visitedPairs.Add(fromToPair);
                    visitedPairs.Add(toFromPair);

                    // Mark both positions as used
                    usedPositions.Add(position);
                    usedPositions.Add(nearest.Position);
                }
            }

            GraphData.Add(position, nearestPositions.Select(x => x.Position).ToList());
        }
    }


    private double HaversineDistance(Location pos1, Location pos2)
    {
        double dLat = DegreesToRadians(pos2.Latitude - pos1.Latitude);
        double dLon = DegreesToRadians(pos2.Longitude - pos1.Longitude);

        double lat1 = DegreesToRadians(pos1.Latitude);
        double lat2 = DegreesToRadians(pos2.Latitude);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c; // Distance in kilometers
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
    
    private bool EdgeExists(Location from, Location to)
    {
        // Check if an edge from 'to' to 'from' already exists
        return Edges.Any(edge => 
            (edge.From == to && edge.To == from) || 
            (edge.From == from && edge.To == to)
        );
    }
}