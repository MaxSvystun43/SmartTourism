namespace GeoApiService.Model.Requests;

public class Bias
{
    public required double Longitude { get; set; }
    public required double Latitude { get; set; }

    public override string ToString()
    {
        return $"proximity:{Longitude},{Latitude}";
    }
}