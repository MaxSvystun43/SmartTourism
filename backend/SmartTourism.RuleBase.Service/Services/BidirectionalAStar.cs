using SmartTourism.Database;
using SmartTourism.Database.Models;
using SmartTourism.RuleBase.Service.Extensions;
using SmartTourism.RuleBase.Service.Models;

namespace SmartTourism.RuleBase.Service.Services;

public class BidirectionalAStar
{
    private readonly Dictionary<string, List<string>> _adjacencyList;
    private readonly List<Point> _points;
    private readonly Setting _setting;

    public BidirectionalAStar(Dictionary<string, List<string>> adjacencyList, List<Point> points, Setting? setting)
    {
        _adjacencyList = adjacencyList;
        _points = points;
        _setting = setting ?? new Setting()
        {
            VisitRestaurant = false,
            UpperLimit = 80,
            LowerLimit = 30
        };
    }

    
    public List<Point> FindShortestPath(Point start, Point end)
    {
        Console.WriteLine("Starting Bidirectional A* search...");
        var path = RunBidirectionalAStar(start, end);

        if (path != null)
        {
            Console.WriteLine("Valid path found with Bidirectional A*.");
            return path;
        }

        Console.WriteLine("No valid path found with Bidirectional A*. Falling back to Standard A*...");
        path = RunStandardAStar(start, end);

        if (path != null)
        {
            Console.WriteLine("Valid path found with Standard A*.");
            return path;
        }

        Console.WriteLine("No valid path found with Standard A*. Falling back to Approximation with Graph Search...");
        path =  RunApproximationWithGraphSearch(start, end);
        if (path != null)
        {
            Console.WriteLine("Valid path found with ApproximationWithGraphSearch.");
            return path;
        }

        return RunDijkstra(start, end);
    }
    
    private List<Point> RunApproximationWithGraphSearch(Point start, Point end)
    {
        var visited = new HashSet<string>();
        var stack = new Stack<List<Point>>();

        // Start the graph search from the start point
        stack.Push(new List<Point> { start });

        while (stack.Count > 0)
        {
            var currentPath = stack.Pop();
            var currentPoint = currentPath.Last();

            // If the path reaches the end, validate and return if valid
            if (currentPoint.Name == end.Name)
            {
                if (IsValidPath(currentPath))
                {
                    Console.WriteLine("Valid path found with Approximation Graph Search.");
                    return currentPath;
                }
                continue;
            }

            // Mark the current point as visited
            visited.Add(currentPoint.Name);

            // Explore neighbors
            if (_adjacencyList.TryGetValue(currentPoint.Name, out var neighbors))
            {
                foreach (var neighborName in neighbors)
                {
                    if (visited.Contains(neighborName)) continue;

                    var neighborPoint = GetPoint(neighborName);
                    if (neighborPoint == null) continue;

                    // Create a new path with the neighbor and push to the stack
                    var newPath = new List<Point>(currentPath) { neighborPoint };
                    stack.Push(newPath);
                }
            }
        }

        Console.WriteLine("No valid path found with Approximation Graph Search.");
        return null;
    }
    
    
    private List<Point> RunBidirectionalAStar(Point start, Point end)
    {
        var forwardOpenSet = new PriorityQueue<Node, double>();
        var backwardOpenSet = new PriorityQueue<Node, double>();

        var forwardVisited = new Dictionary<string, Node>();
        var backwardVisited = new Dictionary<string, Node>();

        var forwardStart = new Node { Name = start.Name, GScore = 0, HScore = start.Heuristic(end) };
        var backwardStart = new Node { Name = end.Name, GScore = 0, HScore = end.Heuristic(start) };

        forwardOpenSet.Enqueue(forwardStart, forwardStart.FScore);
        backwardOpenSet.Enqueue(backwardStart, backwardStart.FScore);

        forwardVisited[start.Name] = forwardStart;
        backwardVisited[end.Name] = backwardStart;

        while (forwardOpenSet.Count > 0 && backwardOpenSet.Count > 0)
        {
            // Forward step
            var currentForward = forwardOpenSet.Dequeue();
            Console.WriteLine($"Forward visiting: {currentForward.Name}");

            if (backwardVisited.TryGetValue(currentForward.Name, out var backwardMeetingPoint))
            {
                Console.WriteLine("Meeting point found in forward search.");
                var path = ReconstructPath(currentForward, backwardMeetingPoint);

                if (path != null) return path; // Return the valid path
            }

            ExploreNeighbors(currentForward, forwardOpenSet, forwardVisited, end);

            // Backward step
            var currentBackward = backwardOpenSet.Dequeue();
            Console.WriteLine($"Backward visiting: {currentBackward.Name}");

            if (forwardVisited.TryGetValue(currentBackward.Name, out var forwardMeetingPoint))
            {
                Console.WriteLine("Meeting point found in backward search.");
                var path = ReconstructPath(forwardMeetingPoint, currentBackward);

                if (path != null) return path; // Return the valid path
            }

            ExploreNeighbors(currentBackward, backwardOpenSet, backwardVisited, start);
        }

        Console.WriteLine("No valid path found after exhausting search.");
        return null;
    }

    private List<Point> RunStandardAStar(Point start, Point end)
    {
        var openSet = new PriorityQueue<Node, double>();
        var visited = new Dictionary<string, Node>();

        var startNode = new Node { Name = start.Name, GScore = 0, HScore = start.Heuristic(end) };
        openSet.Enqueue(startNode, startNode.FScore);
        visited[start.Name] = startNode;

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current.Name == end.Name)
            {
                return ReconstructStandardPath(current);
            }

            if (!_adjacencyList.TryGetValue(current.Name, out var neighbors)) continue;

            foreach (var neighborName in neighbors)
            {
                var neighborPoint = GetPoint(neighborName);
                if (neighborPoint == null) continue;

                var tentativeGScore = current.GScore + PointExtensions.CalculateDistance(
                    GetPoint(current.Name).Latitude, GetPoint(current.Name).Longitude,
                    neighborPoint.Latitude, neighborPoint.Longitude);

                if (visited.ContainsKey(neighborName) && tentativeGScore >= visited[neighborName].GScore)
                    continue;

                var neighborNode = new Node
                {
                    Name = neighborName,
                    GScore = tentativeGScore,
                    HScore = neighborPoint.Heuristic(end),
                    Parent = current
                };

                visited[neighborName] = neighborNode;
                openSet.Enqueue(neighborNode, neighborNode.FScore);
            }
        }

        Console.WriteLine("No valid path found with Standard A*.");
        return null;
    }

    private List<Point> ReconstructStandardPath(Node endNode)
    {
        var path = new List<Point>();
        var current = endNode;

        while (current != null)
        {
            path.Add(GetPoint(current.Name));
            current = current.Parent;
        }

        path.Reverse();

        // Validate the path
        if (IsValidPath(path))
        {
            Console.WriteLine("Valid path found with Standard A*.");
            return path;
        }

        Console.WriteLine("Path found with Standard A* does not meet the criteria.");
        return null;
    }
    
    private void ExploreNeighbors(Node current, PriorityQueue<Node, double> openSet, Dictionary<string, Node> visited, Point target)
    {
        if (!_adjacencyList.TryGetValue(current.Name, out var value))
        {
            Console.WriteLine($"No neighbors for {current.Name}");
            return;
        }

        foreach (var neighborName in value)
        {
            var neighborPoint = GetPoint(neighborName);
            if (neighborPoint == null)
            {
                Console.WriteLine($"No matching point for {neighborName} in _points list.");
                continue;
            }

            var tentativeGScore = current.GScore + PointExtensions.CalculateDistance(
                GetPoint(current.Name).Latitude, GetPoint(current.Name).Longitude,
                neighborPoint.Latitude, neighborPoint.Longitude);

            if (visited.ContainsKey(neighborName) && tentativeGScore >= visited[neighborName].GScore)
                continue;

            var neighborNode = new Node
            {
                Name = neighborName,
                GScore = tentativeGScore,
                HScore = neighborPoint.Heuristic(target),
                Parent = current
            };

            visited[neighborName] = neighborNode;
            openSet.Enqueue(neighborNode, neighborNode.FScore);
            Console.WriteLine($"Enqueued {neighborName} with FScore {neighborNode.FScore}");
        }
    }


    private List<Point> ReconstructPath(Node forwardMeetingPoint, Node backwardMeetingPoint)
    {
        var path = new List<Point>();

        // Build forward path
        var current = forwardMeetingPoint;
        while (current != null)
        {
            path.Add(GetPoint(current.Name));
            current = current.Parent;
        }
        path.Reverse();

        // Build backward path
        current = backwardMeetingPoint.Parent; // Skip the meeting point itself
        while (current != null)
        {
            path.Add(GetPoint(current.Name));
            current = current.Parent;
        }

        Console.WriteLine("Reconstructed path:");
        foreach (var point in path)
        {
            Console.WriteLine($"{point.Name} ({point.Categories})");
        }

        if (IsValidPath(path)) 
            return path;
        
        Console.WriteLine("Path validation failed. No valid path satisfies the criteria.");
        return null;

    }
    
    private List<Point> RunDijkstra(Point start, Point end)
    {
        var openSet = new PriorityQueue<Node, double>();
        var visited = new Dictionary<string, Node>();

        var startNode = new Node { Name = start.Name, GScore = 0 };
        openSet.Enqueue(startNode, 0);
        visited[start.Name] = startNode;

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current.Name == end.Name)
            {
                return ReconstructStandardPath(current);
            }

            if (!_adjacencyList.TryGetValue(current.Name, out var neighbors)) continue;

            foreach (var neighborName in neighbors)
            {
                var neighborPoint = GetPoint(neighborName);
                if (neighborPoint == null) continue;

                var newGScore = current.GScore + PointExtensions.CalculateDistance(
                    GetPoint(current.Name).Latitude, GetPoint(current.Name).Longitude,
                    neighborPoint.Latitude, neighborPoint.Longitude);

                if (visited.ContainsKey(neighborName) && newGScore >= visited[neighborName].GScore)
                    continue;

                var neighborNode = new Node
                {
                    Name = neighborName,
                    GScore = newGScore,
                    Parent = current
                };

                visited[neighborName] = neighborNode;
                openSet.Enqueue(neighborNode, newGScore);
            }
        }

        Console.WriteLine("No path found with Dijkstra's Algorithm.");
        return null;
    }
    
    private bool IsValidPath(List<Point> path)
    {
        // Check if the path contains at least one Catering point
        bool containsCatering = _setting.VisitRestaurant != true || path.Any(point => point.Categories.Contains("Catering"));

        // Check if the path length is within 40% to 60% of total points
        int totalPoints = _points.Count;
        int pathLength = path.Count;

        bool lengthValid = pathLength >= (_setting.LowerLimit/100) * totalPoints && pathLength <= (_setting.UpperLimit/100) * totalPoints;

        if (!containsCatering)
            Console.WriteLine("Path does not include any Catering points.");

        if (!lengthValid)
            Console.WriteLine($"Path length {pathLength} is not within the valid range ({_setting.LowerLimit}% to {_setting.UpperLimit}% of {totalPoints}).");

        return containsCatering && lengthValid;
    }


    private Point GetPoint(string name) => _points.FirstOrDefault(p => p.Name == name);
}