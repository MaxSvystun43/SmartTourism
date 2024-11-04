using GeoApiService.Model;
using GeoApiService.Model.Requests;
using GeoApiService.Model.Responses;
using Refit;

namespace GeoApiService;

public interface IGeoapifyApi
{
    [Get("/v2/places")]
    public Task<GeoApiResponse> GetPlacesAsync(
        [Query("categories")] string categories,
        [Query("filter")] string filter,
        [Query("bias")] string bias,
        [Query("limit")] int limit,
        [Query("apiKey")] string apiKey);

    [Post("/v1/routematrix")]
    public Task<RouteResponse> GetRoadsAsync(
        [Query("apiKey")] string apiKey, 
        [Body] RouteRequest request);
}