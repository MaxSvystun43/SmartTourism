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
}