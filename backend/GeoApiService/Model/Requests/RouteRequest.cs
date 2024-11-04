namespace GeoApiService.Model.Requests;

// Define the main data model class
public record RouteRequest
{
    public string Mode { get; set; } = "drive";
    public List<LocationModel> Sources { get; set; }
    public List<LocationModel> Targets { get; set; }
    public string Type { get; set; } = "short";
    public string Traffic { get; set; } = "approximated";
}

public record LocationModel
{
    public double[] Location { get; set; } // Array to store latitude and longitude
}