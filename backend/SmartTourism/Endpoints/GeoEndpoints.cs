using GeoApiService;
using GeoApiService.Model.Requests;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartTourism.DijstraSolver.Service;
using SmartTourism.Endpoints.Models;
using SmartTourism.Extensions;
using SmartTourism.PathFinding.Service.Models;
using SmartTourism.Services;
using Location = SmartTourism.PathFinding.Service.Models.Location;

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
        
        app.MapPost("/api/geo/dijstra/get-routes", async (
            [FromServices] IGeoapifyService service,
            [FromBody] FindRoute request
        ) =>
        {
            Log.Information("GeoEndpoints request {@Request}", request);
            var locations = request.Waypoints;
            locations.AddRange([request.Start, request.End]);
            
            var results = await service.GetPlaceRoutesAsync(locations.Select(x => x.Location));
            
            var routeFinder = new DijkstraTestSolver(results.SourcesToTargets.ToDijstraModel(locations));
            
            int startNode = 0; // Starting position (ID)
            int endNode = request.Waypoints.Count - 1;   // Ending position (ID)
            var waypoints =  Enumerable.Range(1, request.Waypoints.Count - 1).ToList(); // Other waypoints to visit
            var shortestPath = routeFinder.FindShortestRoute(startNode, endNode, waypoints);

            var resultRoute = new List<LocationModel>();
            
            foreach (var node in shortestPath[..^1])
            {
                resultRoute.Add(request.Waypoints[node]);
            }
            
            Log.Information("Result path shortest {@Result}", shortestPath);
            Log.Debug("Result shortest route {@Result}", resultRoute);
            
            return Results.Ok(resultRoute);
        });


        app.MapPost("/api/geo/pathfindin/get-routes", async (
            [FromServices] SmartTourismService service,
            [FromBody] PathFindingRequest request
        ) =>
        {
            var data = await service.Test(request);
            
            return Results.Ok(data);
        });
        
        // app.MapGet("/api/geo/pathfindin/test", (
        //     [FromServices] SmartTourismService service
        // ) =>
        // {
        //     var data = service.Test();
        //     
        //     return Results.Ok(data);
        // });



        return app;
    }
}