using GeoApiService.Model;
using GeoApiService.Model.Requests;
using GeoApiService.Model.Responses;

namespace GeoApiService;

public interface IGeoapifyService
{
    Task<List<PlacesToVisit>> GetPlacesAsync(GeoApiRequest request);

    Task<RouteResponse> GetPlaceRoutesAsync(List<LocationModel> locations);
    Task<RouteResponse> GetPlaceRoutesAsync(LocationModel startLocation, List<LocationModel> endLocation);
}