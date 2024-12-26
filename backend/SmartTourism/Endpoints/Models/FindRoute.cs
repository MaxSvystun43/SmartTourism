namespace SmartTourism.Endpoints.Models;

public class FindRoute
{
    public required Location Start { get; set; }
    public required Location End { get; set; }
    public List<Location> Waypoints { get; set; } = []; 
}