using GeoApiService.Model;
using GeoApiService.Model.Extensions;
using GeoApiService.Model.Requests;
using GeoApiService.Model.Responses;
using Refit;
using Serilog;

namespace GeoApiService.Service;

public class GeoapifyService : IGeoapifyService
{
    private readonly IGeoapifyApi _geoapifyApi;
    const string ApiKey = "d14bbbf1347b41b288e27f6d9432932e"; 
    
    public GeoapifyService(IGeoapifyApi geoapifyApi)
    {
        _geoapifyApi = geoapifyApi;
    }
    
    public async Task<GeoApiResponse> GetPlacesAsync(GeoApiRequest request)
    {
        try
        {
            var categories = "commercial,entertainment,national_park,tourism,catering";
            var filter = "circle:26.240300448673793,50.6225296,5000";
            var bias = "proximity:26.240300448673793,50.6225296";
            var limit = 10;

            var newCategories = request.Categories.ToSnakeString();
            var newFilter = request.Filter.ToString();
            var newBias = request.Bias.ToString();

            var response = await _geoapifyApi.GetPlacesAsync(newCategories, newFilter, newBias, request.Limit, ApiKey);

            // Process the response
            foreach (var place in response.Features)
            {
                Log.Information("Name: {@place}", place);
            }
            return response;
        }
        catch (ApiException apiEx)
        {
            Log.Error($"API error: {apiEx.Message}");
            Log.Error($"Status code: {apiEx.StatusCode}");
            throw;
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred: {ex.Message}");
            throw;
        }
    }



    public async Task<RouteResponse> GetPlaceRoutesAsync(List<LocationModel> locations)
    {
        try
        {
            var routeData = new RouteRequest()
            {
                Mode = "drive",
                Sources = locations,
                Targets = locations,
                Type = "short",
                Traffic = "approximated"
            };
            
            var response = await _geoapifyApi.GetRoadsAsync(ApiKey, routeData);
            
            Log.Information("Response data from geoapify : {@Response}", response);

            return response;

        }
        catch (ApiException apiEx)
        {
            Log.Error($"API error: {apiEx.Message}");
            Log.Error($"Status code: {apiEx.StatusCode}");
            throw;
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred: {ex.Message}");
            throw;
        }
    }
}