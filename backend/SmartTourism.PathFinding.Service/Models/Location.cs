namespace SmartTourism.PathFinding.Service.Models;

public class Location
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required double Longitude { get; set; }
    public required double Latitude { get; set; }
}