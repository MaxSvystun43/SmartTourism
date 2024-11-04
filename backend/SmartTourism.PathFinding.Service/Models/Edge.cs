namespace SmartTourism.PathFinding.Service.Models;

public class Edge
{
    public Location From { get; set; }
    public Location To { get; set; }
    public double Distance { get; set; }
    public double RoadTime { get; set; } 
    
    public Edge(Location from, Location to, double roadTime)
    {
        From = from;
        To = to;
        RoadTime = roadTime;
    }
}