using GeoApiService.Model.Requests;

namespace SmartTourism.Endpoints.Models;

public class PathFindingRequest
{
    public GeoApiRequest GeoApiRequest { get; set; }
    public required Location Start { get; set; }
    public required Location End { get; set; }
}