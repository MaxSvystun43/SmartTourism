namespace SmartTourism.Database.Models;

public class Setting
{
    public int Id { get; set; } // Primary Key
    public bool VisitRestaurant { get; set; }
    public double LowerLimit { get; set; }
    public double UpperLimit { get; set; }
}
