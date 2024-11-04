using GeoApiService.Model;
using GeoApiService.Model.Requests;
using GeoApiService.Model.Responses;

namespace GeoApiService;

public interface IGeoapifyService
{
    Task<GeoApiResponse> GetPlacesAsync(GeoApiRequest request);

    Task<RouteResponse> GetPlaceRoutesAsync(List<LocationModel> locations);
}