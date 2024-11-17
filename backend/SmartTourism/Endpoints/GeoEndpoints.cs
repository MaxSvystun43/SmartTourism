using GeoApiService;
using GeoApiService.Model.Requests;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartTourism.Endpoints.Models;
using SmartTourism.PathFinding.Service.Models;
using SmartTourism.Services;

namespace SmartTourism.Endpoints;

internal static class GeoEndpoints
{
    public static WebApplication AddGeoEndpoints(this WebApplication app)
    {
        app.MapPost("/api/geo/get-places", async (
            [FromBody] GeoApiRequest request,
            [FromServices] IGeoapifyService service
        ) =>
        {
            Log.Information("GeoEndpoints request {@Request}", request);
            var results = await service.GetPlacesAsync(request);
            return Results.Ok(results);
        });
        
        app.MapPost("/api/geo/get-routes", async (
            [FromServices] IGeoapifyService service,
            [FromBody] FindRoute request
        ) =>
        {
            Log.Information("GeoEndpoints request {@Request}", request);
            var results = await service.GetPlaceRoutesAsync(request.Locations);
            
            var routeFinder = new DijkstraSorver(results);
            
            int startNode = 0; // Starting position (ID)
            int endNode = request.Locations.Count - 1;   // Ending position (ID)
            var waypoints =  Enumerable.Range(1, request.Locations.Count - 1).ToList(); // Other waypoints to visit
            var shortestPath = routeFinder.FindShortestRoute(startNode, endNode, waypoints);

            var resultRoute = new List<LocationModel>();
            
            foreach (var node in shortestPath[..^1])
            {
                resultRoute.Add(request.Locations[node]);
            }
            
            Log.Information("Result path shortest {@Result}", shortestPath);
            Log.Debug("Result shortest route {@Result}", resultRoute);
            
            return Results.Ok(resultRoute);
        });


        app.MapGet("/api/geo/get-neighbors", (
        ) =>
        {
            var graph = new Graph();
            graph.AddPosition(new Location { Id = Guid.NewGuid(), Latitude = 40.7128, Longitude = -74.0060, Name = "New York" }); // New York
            graph.AddPosition(new Location { Id = Guid.NewGuid(), Latitude = 34.0522, Longitude = -118.2437, Name = "Los Angeles" }); // Los Angeles
            graph.AddPosition(new Location { Id = Guid.NewGuid(), Latitude = 41.8781, Longitude = -87.6298, Name = "Chicago" }); // Chicago
            graph.AddPosition(new Location { Id = Guid.NewGuid(), Latitude = 29.7604, Longitude = -95.3698, Name = "Houston" }); // Houston
            graph.AddPosition(new Location { Id = Guid.NewGuid(), Latitude = 33.4484, Longitude = -112.0740, Name = "Phoenix" }); // Phoenix
            
            graph.BuildNearestNeighborsGraph(3);
            
            foreach (var edge in graph.Edges)
            {
                Log.Information($"Edge from Position {edge.From.Name} to Position {edge.To.Name} with distance {edge.Distance:F2} km");
            }
            
            return Results.Ok(graph.Edges);
        });
        
        
        app.MapPost("/api/geo/get-neighbors-edges-new", async (
            [FromServices] SmartTourismService service,
            [FromBody] GeoApiRequest request
        ) =>
        {
            //var data = await service.GetSmartTourismSuggestionsAsync(request);
            
            service.TestSolver();
            return Results.Ok();
        });

        return app;
    }
}