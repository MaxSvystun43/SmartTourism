using GeoApiService.Model.Requests;

namespace SmartTourism.Endpoints.Models;

public record FindRoute
{
    public List<LocationModel> Locations { get; set; }
}
