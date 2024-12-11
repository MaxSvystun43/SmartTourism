using GeoApiService.Model;
using SmartTourism.PathFinding.Service.Models;
using SmartTourism.RuleBase.Service.Models;

namespace SmartTourism.Extensions;

public static class PathFindingMappingExtensions
{
    public static Location ToLocationModel(this Endpoints.Models.Location location, string name)
    {
        ArgumentNullException.ThrowIfNull(location);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        return new Location()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Latitude = location.Location[0],
            Longitude = location.Location[1]
        };
    }
    
    
    public static Point ToPoint(this Endpoints.Models.Location location, string name)
    {
        ArgumentNullException.ThrowIfNull(location);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        return new Point()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Latitude = location.Location[0],
            Longitude = location.Location[1],
            Categories = name
        };
    }
    
    
    public static Point ToPoint(this PlacesToVisit location)
    {
        ArgumentNullException.ThrowIfNull(location);
        
        return new Point()
        {
            Id = Guid.NewGuid(),
            Name = location.Name!,
            Latitude = location.Lat,
            Longitude = location.Lon,
            Categories = location.Categories
                .Select(category => category.ToString()).Aggregate((a, b) => $"{a} {b}")
        };
    }
}