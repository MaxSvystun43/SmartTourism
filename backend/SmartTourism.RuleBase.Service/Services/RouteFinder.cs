using SmartTourism.RuleBase.Service.Extensions;
using SmartTourism.RuleBase.Service.Models;
using ILogger = Serilog.ILogger;

namespace SmartTourism.RuleBase.Service.Services;

public class RouteFinder
{
    private readonly List<Point> _points;
    private readonly Point _start;
    private readonly Point _end;
    private readonly BacktrackService _backtrack;
    private readonly BidirectionalAStar _bidirectual;

    public RouteFinder(List<Point> points, Point start, Point end)
    {
        _points = points;
        _start = start;
        _end = end;
        _backtrack = new BacktrackService(_points, _start, _end, CreateAdjacencyListUsingKNN(_points, (int)(_points.Count * 0.15))); // Example usage with KNN
        
        _bidirectual = new BidirectionalAStar(CreateBidirectionalAdjacencyListUsingKNN(_points, (int)(_points.Count * 0.15)), _points); // Example usage with KNN
    }
    
    public RouteFinder(List<Point> points, Point start, Point end, int k)
    {
        _points = points;
        _start = start;
        _end = end;
        _backtrack = new BacktrackService(_points, _start, _end, CreateAdjacencyListUsingKNN(_points, k)); // Example usage with KNN
        _bidirectual = new BidirectionalAStar(CreateBidirectionalAdjacencyListUsingKNN(_points, k), _points); // Example usage with KNN
    }
    
    public static Dictionary<string, List<string>> CreateAdjacencyListUsingKNN(List<Point> points, int k)
    {
        var adjacencyList = new Dictionary<string, List<string>>();

        foreach (var point in points)
        {
            // Calculate distances to all other points
            var distances = points
                .Where(p => p != point) // Exclude the current point
                .Select(p => new { Name = p.Name, Distance = PointExtensions.CalculateDistance(point.Latitude, point.Longitude, p.Latitude, p.Longitude) })
                .OrderBy(x => x.Distance) // Sort by distance
                .Take(k) // Take the k nearest neighbors
                .ToList();

            // Add the k nearest neighbors to the adjacency list
            adjacencyList[point.Name] = distances.Select(d => d.Name).ToList();
        }

        return adjacencyList;
    }
    
    
    public static Dictionary<string, List<string>> CreateBidirectionalAdjacencyListUsingKNN(List<Point> points, int k)
    {
        var adjacencyList = new Dictionary<string, List<string>>();

        foreach (var point in points)
        {
            if (!adjacencyList.ContainsKey(point.Name))
            {
                adjacencyList[point.Name] = new List<string>();
            }

            // Calculate distances to all other points
            var distances = points
                .Where(p => p != point) // Exclude the current point
                .Select(p => new { Name = p.Name, Distance = PointExtensions.CalculateDistance(point.Latitude, point.Longitude, p.Latitude, p.Longitude) })
                .OrderBy(x => x.Distance) // Sort by distance
                .Take(k) // Take the k nearest neighbors
                .ToList();

            foreach (var neighbor in distances)
            {
                // Add neighbor to the current point's adjacency list
                adjacencyList[point.Name].Add(neighbor.Name);

                // Ensure bidirectional connectivity
                if (!adjacencyList.ContainsKey(neighbor.Name))
                {
                    adjacencyList[neighbor.Name] = new List<string>();
                }
                if (!adjacencyList[neighbor.Name].Contains(point.Name))
                {
                    adjacencyList[neighbor.Name].Add(point.Name);
                }
            }
        }

        return adjacencyList;
    }


    public List<Point> FindRouteUsingBachtrack()
    {
        var route = _backtrack.FindBestRoute();
        Console.WriteLine("Best route using backtracking:");
        Console.WriteLine(string.Join(" -> ", route.Select(p => p.Name)));
        return route;
    }
    
    public List<Point>? FindRouteUsingBidirectualAStar()
    {
        var route = _bidirectual.FindShortestPath(_start, _end);
        Console.WriteLine("Best route using bidirectual:");
        Console.WriteLine(string.Join(" -> ", route?.Select(p => p.Name) ?? Array.Empty<string>()));
        return route;
    }
}