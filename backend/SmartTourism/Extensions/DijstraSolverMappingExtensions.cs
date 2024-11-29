using GeoApiService.Model.Responses;
using SmartTourism.DijstraSolver.Service.Models;
using SmartTourism.Endpoints.Models;

namespace SmartTourism.Extensions;

internal static class DijstraSolverMappingExtensions
{
    public static IEnumerable<SourceTarget> ToDijstraModel(this List<List<SourceToTarget>> sourceToTargets,
        List<Location> locations)
    {
        return sourceToTargets.SelectMany(x => x).Select(location => new SourceTarget()
        {
            Distance = location.Distance,
            Time = location.Time,
            SourceId = locations[location.SourceIndex].Id,
            TargetId = locations[location.TargetIndex].Id
        });
    }
}