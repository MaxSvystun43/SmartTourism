
using Serilog;
using SmartTourism.RuleBase.Service.Extensions;
using SmartTourism.RuleBase.Service.Models;

namespace SmartTourism.RuleBase.Service.Services;

public class BacktrackService
{
    private readonly Dictionary<string, List<string>> _adjacencyList;
    private readonly List<Point> _points;
    private readonly Point _start;
    private readonly Point _end;

    public BacktrackService(List<Point> points, Point start, Point end, Dictionary<string, List<string>> adjacencyList)
    {
        _points = points;
        _start = start;
        _end = end;
        _adjacencyList = adjacencyList;
    }

    public List<Point> FindBestRoute()
    {
        var bestRoute = new List<Point>();
        var bestScore = double.MinValue;

        var visited = new HashSet<Point>();
        var currentPath = new List<Point>();

        Backtrack(_start, visited, currentPath, 0, ref bestRoute, ref bestScore);

        if (!bestRoute.Any())
        {
            Log.Information("No valid route found.");
            return new List<Point>();
        }

        return bestRoute;
    }

    private void Backtrack(Point current, HashSet<Point> visited, List<Point> currentPath, double currentDistance, ref List<Point> bestRoute, ref double bestScore)
    {
        Log.Information($"Visiting: {current.Name}, Current Path: {string.Join(" -> ", currentPath.Select(p => p.Name))}");

        currentPath.Add(current);
        visited.Add(current);

        if (current == _end)
        {
            Log.Information($"Reached end: {string.Join(" -> ", currentPath.Select(p => p.Name))}");
            var currentScore = EvaluateRoute(currentPath, currentDistance);
            Log.Information($"Route Score: {currentScore}");
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                bestRoute = new List<Point>(currentPath);
            }
        }
        else
        {
            if (_adjacencyList.ContainsKey(current.Name))
            {
                foreach (var neighborName in _adjacencyList[current.Name])
                {
                    var neighbor = GetPoint(neighborName);
                    if (!visited.Contains(neighbor))
                    {
                        var distanceToNeighbor = PointExtensions.CalculateDistance(
                            current.Latitude, current.Longitude,
                            neighbor.Latitude, neighbor.Longitude);

                        Backtrack(neighbor, visited, currentPath, currentDistance + distanceToNeighbor, ref bestRoute, ref bestScore);
                    }
                }
            }
        }

        currentPath.RemoveAt(currentPath.Count - 1);
        visited.Remove(current);
    }

    // private double EvaluateRoute(List<Point> route, double totalDistance)
    // {
    //     return 1000 - totalDistance; // Basic scoring for testing
    // }
    
    private double EvaluateRoute(List<Point> route, double totalDistance)
    {
        // Base score
        var score = 1000 - totalDistance / 100; // Penalize distance more heavily

        // Reward diversity of categories
        var categories = route.Select(p => p.Categories).Distinct().Count();
        score += categories * 50;

        // Reward restaurant placement
        var restaurantIndex = route.FindIndex(p => p.Categories == "Catering");
        if (restaurantIndex >= 0)
        {
            var restaurantPosition = (double)restaurantIndex / route.Count;
            if (restaurantPosition >= 0.4 && restaurantPosition <= 0.8)
                score += 100; // Reward well-placed restaurants
        }

        // Ensure exactly one restaurant is included
        if (route.Count(p => p.Categories == "Catering") != 1)
            return double.MinValue;

        // Add penalty for overly diverse or overly long routes
        if (categories > 4 || route.Count > 6) // Arbitrary limits for this case
            score -= 200;

        return score;
    }


    private Point GetPoint(string name) => _points.First(p => p.Name == name);
}