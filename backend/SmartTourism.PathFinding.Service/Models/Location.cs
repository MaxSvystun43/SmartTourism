namespace SmartTourism.PathFinding.Service.Models;

public class Location
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required double Longitude { get; set; }
    public required double Latitude { get; set; }
}