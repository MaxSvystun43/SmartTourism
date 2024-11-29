using GeoApiService.Model.Requests;

namespace SmartTourism.Endpoints.Models;

public record Location : LocationModel
{
    public int Id { get; set; }
}