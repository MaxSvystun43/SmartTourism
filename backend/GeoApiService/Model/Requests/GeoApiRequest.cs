using GeoApiService.Model.Enums;
using GeoApiService.Model.Extensions;

namespace GeoApiService.Model.Requests;

public class GeoApiRequest
{
    public List<Category> Categories { get; set; }
    
    public Filter Filter { get; set; }
    
    public Bias Bias { get; set; }
    
    public int Limit { get; set; }
}