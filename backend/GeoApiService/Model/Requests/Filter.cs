namespace GeoApiService.Model.Requests;

public sealed class Filter
{
    public FilterType FilterType { get; set; } = FilterType.Circle;
    public required double Longitude { get; set; }
    public required double Latitude { get; set; }
    public required int RadiusInMeters { get; set; }

    public override string ToString()
    {
        return FilterType switch
        {
            FilterType.Circle => $"circle:{Longitude},{Latitude},{RadiusInMeters}",
            _ => throw new ArgumentOutOfRangeException(nameof(FilterType), FilterType, null)
        };
    }
}

public enum FilterType
{
    Circle
}