namespace GeoApiService.Configuration;

public class HttpConfiguration
{
    public Uri Uri { get; set; } = new("https://api.geoapify.com");
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}