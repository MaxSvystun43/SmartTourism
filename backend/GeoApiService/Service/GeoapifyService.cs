using GeoApiService.Extensions;
using GeoApiService.Model;
using GeoApiService.Model.Enums;
using GeoApiService.Model.Extensions;
using GeoApiService.Model.Requests;
using GeoApiService.Model.Responses;
using Refit;
using Serilog;

namespace GeoApiService.Service;

public class GeoapifyService : IGeoapifyService
{
    private readonly IGeoapifyApi _geoapifyApi;
    private const string ApiKey = "d14bbbf1347b41b288e27f6d9432932e"; 
    
    public GeoapifyService(IGeoapifyApi geoapifyApi)
    {
        _geoapifyApi = geoapifyApi;
    }
    
    public async Task<List<PlacesToVisit>> GetPlacesAsync(GeoApiRequest request)
    {
        var resultPlaces = new List<PlacesToVisit>();
        try
        {
            if (request.Categories.Contains(Category.Catering))
            {
                request.Categories.Remove(Category.Catering);
                
                resultPlaces.AddRange(await GetCafeteriaPlaces(request));
            }
            
            Log.Information("Getting all places to visit");
            var newCategories = request.Categories.ToSnakeString();
            var newFilter = request.Filter.ToString();
            var newBias = request.Bias.ToString();
            var response = await _geoapifyApi.GetPlacesAsync(newCategories, newFilter, newBias, request.Limit * 4, ApiKey);
            
            Log.Information("Response data from geoapify : {@Response}", response);

            resultPlaces.AddRange(response.Features.Select(p => p.ToPlacesToVisit()));
            
            return resultPlaces.Where(x => x.Name != null).OrderBy(x => x.Name).TakeLast(request.Limit).ToList();
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


    public async Task<RouteResponse> GetPlaceRoutesAsync(IEnumerable<double[]> locations)
    {
        var sources = locations.Select(location => new LocationModel() { Location = location}).ToList();
        try
        {
            var routeData = new RouteRequest()
            {
                Mode = "drive",
                Sources = sources,
                Targets = sources,
                Type = "short",
                Traffic = "approximated"
            };
            
            var response = await _geoapifyApi.GetRoadsAsync(ApiKey, routeData);
            
            Log.Debug("Response data from geoapify : {@Response}", response);

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
    public async Task<RouteResponse> GetPlaceRoutesAsync(LocationModel startLocation, List<LocationModel> endLocation)
    {
        try
        {
            var routeData = new RouteRequest()
            {
                Mode = "drive",
                Sources = [startLocation],
                Targets = endLocation,
                Type = "short",
                Traffic = "approximated"
            };
            
            Log.Information("Route data {@Data}", routeData);
            
            var response = await _geoapifyApi.GetRoadsAsync(ApiKey, routeData);
            
            Log.Debug("Response data from geoapify : {@Response}", response);

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

    private async Task<IReadOnlyList<PlacesToVisit>> GetCafeteriaPlaces(GeoApiRequest request)
    {
        Log.Information("Getting data about caterings");
        var cateringCategory = "catering";
        var filter = request.Filter.ToString();
        var bias = request.Bias.ToString();

        var response = await _geoapifyApi.GetPlacesAsync(cateringCategory, filter, bias, request.Limit/4, ApiKey);
        
        Log.Debug("Got data about catering {@Response}", response);
        
        return response.Features.Select(p => p.ToPlacesToVisit()).ToList();
    }

}