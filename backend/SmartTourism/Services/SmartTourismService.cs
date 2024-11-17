using GeoApiService;
using GeoApiService.Model;
using GeoApiService.Model.Requests;
using Serilog;
using SmartTourism.PathFinding.Service.Models;

namespace SmartTourism.Services;

public class SmartTourismService
{
    private readonly IGeoapifyService _geoapifyService;
    private List<UpdatedEdge> _updatedEdges = new();

    public SmartTourismService(IGeoapifyService geoapifyService)
    {
        _geoapifyService = geoapifyService ?? throw new ArgumentNullException(nameof(geoapifyService));
    }

    public async Task<List<PlacesToVisit>> GetSmartTourismSuggestionsAsync(GeoApiRequest request)
    {
        var results = await _geoapifyService.GetPlacesAsync(request);
        var graph = new Graph();

        foreach (var result in results)
        {
            graph.AddPosition(new Location
            {
                Id = Guid.NewGuid(),
                Name = result.Name,
                Latitude = result.Lat,
                Longitude = result.Lon
            });
        }
        
        graph.BuildNearestNeighborsGraph(4);
        
        Log.Information("Old edges to get this {@Edges}", graph.Edges);

        foreach (var location in graph.Locations)
        {
            var neighbors = graph.Edges.Where(x => x.From.Id == location.Id).ToList();

            var endPoints = neighbors.Select(x => new LocationModel()
            {
                Location = new[] { x.To.Latitude, x.To.Longitude },
            }).ToList();
            if (endPoints.Count == 0 )
                continue;
            var positions =
                await _geoapifyService.GetPlaceRoutesAsync(new LocationModel() { Location = new[] { location.Latitude, location.Longitude }}, endPoints);

            foreach (var position in positions.SourcesToTargets[0])
            {
                var neighbor = neighbors[position.TargetIndex].To;
               
                _updatedEdges.Add(new UpdatedEdge
                {
                    From = location,
                    To = neighbor,
                    Duration = position.Time ?? 0,
                    Distance = position.Distance ?? 0
                });
            }

            Log.Information("New data {@Data}", _updatedEdges);
        }
        
        Log.Information("Updated edges to get this {@UpdatedEdges}", _updatedEdges);

        return results;
    }



    public void TestSolver()
    {
        // Sample locations
        var locationA = new Location { Id = Guid.NewGuid(), Name = "A", Latitude = 0, Longitude = 0 };
        var locationB = new Location { Id = Guid.NewGuid(), Name = "B", Latitude = 1, Longitude = 1 };
        var locationC = new Location { Id = Guid.NewGuid(), Name = "C", Latitude = 2, Longitude = 2 };
        var locationD = new Location { Id = Guid.NewGuid(), Name = "D", Latitude = 3, Longitude = 3 };
        var locationE = new Location { Id = Guid.NewGuid(), Name = "E", Latitude = 4, Longitude = 4 };

        // Map each location's hash to its category
        var categories = new Dictionary<int, string>
        {
            { locationA.Id.GetHashCode(), "Default" },
            { locationB.Id.GetHashCode(), "Park" },
            { locationC.Id.GetHashCode(), "Museum" },
            { locationD.Id.GetHashCode(), "Cafe" },
            { locationE.Id.GetHashCode(), "Shopping" }
        };

        // Define edges between locations
        var updatedEdges = new List<UpdatedEdge>
        {
            new UpdatedEdge { From = locationA, To = locationB, Duration = 20, Distance = 5, TimeSpend = 15 },
            new UpdatedEdge { From = locationA, To = locationC, Duration = 30, Distance = 8, TimeSpend = 40 },
            new UpdatedEdge { From = locationB, To = locationC, Duration = 10, Distance = 3, TimeSpend = 40 },
            new UpdatedEdge { From = locationB, To = locationD, Duration = 25, Distance = 4, TimeSpend = 20 },
            new UpdatedEdge { From = locationC, To = locationE, Duration = 30, Distance = 7, TimeSpend = 45 },
            new UpdatedEdge { From = locationD, To = locationE, Duration = 15, Distance = 6, TimeSpend = 30 },
            new UpdatedEdge { From = locationA, To = locationE, Duration = 50, Distance = 15, TimeSpend = 0 } // Direct path with no stop
        };

        // Initialize the DijkstraSolver with the edges and categories
        var solver = new DijkstraSolver(updatedEdges, categories);

        // Find the best route from A to E with total available time of 120 minutes
        int start = locationA.Id.GetHashCode();
        int end = locationE.Id.GetHashCode();
        int totalAvailableTime = 120;

        var bestRoute = solver.FindBestRoute(start, end, totalAvailableTime);

        // Output the best route
        Console.WriteLine("Best Route:");
        foreach (var locationId in bestRoute)
        {
            Log.Warning(locationId == start ? "A" : locationId == locationB.Id.GetHashCode() ? "B" : 
                              locationId == locationC.Id.GetHashCode() ? "C" : 
                              locationId == locationD.Id.GetHashCode() ? "D" : "E");
        }
    }
}