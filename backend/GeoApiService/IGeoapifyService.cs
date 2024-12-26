using GeoApiService.Model;
using GeoApiService.Model.Requests;
using GeoApiService.Model.Responses;

namespace GeoApiService;

public interface IGeoapifyService
{
    Task<List<PlacesToVisit>> GetPlacesAsync(GeoApiRequest request);

    Task<RouteResponse> GetPlaceRoutesAsync(IEnumerable<double[]> locations);
    Task<RouteResponse> GetPlaceRoutesAsync(LocationModel startLocation, List<LocationModel> endLocation);
}