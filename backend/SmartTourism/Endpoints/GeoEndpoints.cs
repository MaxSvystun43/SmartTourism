using GeoApiService;
using GeoApiService.Model.Requests;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartTourism.Endpoints.Models;

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
            var results = await service.GetPlacesAsync(request);
            return Results.Ok(results.Features);
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

        return app;
    }
}