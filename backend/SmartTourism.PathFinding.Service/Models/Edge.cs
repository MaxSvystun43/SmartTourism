namespace SmartTourism.PathFinding.Service.Models;

public class Edge
{
    public required Location From { get; set; }
    public required Location To { get; set; }
    public double Distance { get; set; }
}