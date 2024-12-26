namespace SmartTourism.RuleBase.Service.Models;

public class Point
{
    public string Name { get; set; }
    
    public Guid Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public required string Categories { get; set; }
    public double VisitTime { get; set; }
}
