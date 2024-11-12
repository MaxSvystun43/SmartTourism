using GeoApiService.Model;
using GeoApiService.Model.Responses;

namespace GeoApiService.Extensions;

public static class PlacesToVisitMapperExtensions
{
    public static PlacesToVisit ToPlacesToVisit(this Feature feature)
    {
        return new PlacesToVisit()
        {
            Name = feature.Properties.Name,
            Lon = feature.Properties.Lon,
            Lat = feature.Properties.Lat,
            Categories = feature.Properties.Categories.GetCategories(),
            Details = feature.Properties.Details
        };
    }
}