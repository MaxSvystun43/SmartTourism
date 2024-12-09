using GeoApiService.Model.Enums;

namespace GeoApiService.Model;

public class PlacesToVisit
{
    /// <summary>
    /// The name of the geographical feature.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// The longitude coordinate of the geographical feature.
    /// </summary>
    public double Lon { get; set; }

    /// <summary>
    /// The latitude coordinate of the geographical feature.
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// The categories of the geographical feature.
    /// </summary>
    public IReadOnlyList<Category> Categories { get; set; } = [];// need to make enum 

    /// <summary>
    /// The details of the geographical feature.
    /// </summary>
    public IReadOnlyList<string> Details { get; set; } = []; // need to make enum 
    
    /// <summary>
    /// The opening hours of the geographical feature.
    /// </summary>
    public string OpeningHours { get; set; }
}