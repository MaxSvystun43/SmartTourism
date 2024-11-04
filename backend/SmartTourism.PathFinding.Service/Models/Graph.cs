namespace SmartTourism.PathFinding.Service.Models;

public class Graph
{
    public Dictionary<Location, List<Edge>> LocationData { get; set; }

    public Graph()
    {
        LocationData = new Dictionary<Location, List<Edge>>();
    }
    
    public void AddPlace(Location place)
    {
        if (!LocationData.ContainsKey(place))
        {
            LocationData[place] = new List<Edge>();
        }
    }
    
    public void AddEdge(Location from, Location to, double roadTime)
    {
        AddPlace(from);
        AddPlace(to);
        LocationData[from].Add(new Edge(from, to, roadTime));
    }
}