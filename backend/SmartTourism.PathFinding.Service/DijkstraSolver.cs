using SmartTourism.PathFinding.Service.Models;

public class DijkstraSolver
{
    private readonly Dictionary<int, List<UpdatedEdge>> _adjacencyList = new();
    private readonly Dictionary<int, string> _categories;  // Map location index to category

    public DijkstraSolver(List<UpdatedEdge> updatedEdges, Dictionary<int, string> categories)
    {
        _categories = categories;
        
        foreach (var edge in updatedEdges)
        {
            int sourceIndex = edge.From.Id.GetHashCode();
            int targetIndex = edge.To.Id.GetHashCode();
            double timeSpend = GetAverageVisitTime(_categories.ContainsKey(targetIndex) ? _categories[targetIndex] : "Default");

            AddRoute(sourceIndex, targetIndex, edge.Distance, edge.Duration, timeSpend, edge.From, edge.To);
        }
    }

    public void AddRoute(int source, int target, double distance, double travelTime, double timeSpend, Location from, Location to)
    {
        if (!_adjacencyList.ContainsKey(source))
            _adjacencyList[source] = new List<UpdatedEdge>();

        _adjacencyList[source].Add(new UpdatedEdge
        {
            From = from, // Placeholder data for From
            To = to,   // Placeholder data for To
            Distance = distance,
            Duration = travelTime,
            TimeSpend = timeSpend
        });
    }

    public List<int> FindBestRoute(int start, int end, int totalAvailableTime)
    {
        var bestRoute = new List<int>();          // To store the best route found
        int closestTimeDifference = int.MaxValue;  // Track the smallest difference from totalAvailableTime

        // A queue to manage paths during exploration
        var pathQueue = new Queue<(List<int> Route, int TotalTime)>();
        pathQueue.Enqueue((new List<int> { start }, 0));  // Start with initial node

        // Explore all paths
        while (pathQueue.Count > 0)
        {
            var (currentRoute, currentTime) = pathQueue.Dequeue();
            int currentNode = currentRoute.Last();

            // If we reached the end node, evaluate this route's total time
            if (currentNode == end)
            {
                int timeDifference = Math.Abs(totalAvailableTime - currentTime);
                if (timeDifference < closestTimeDifference)
                {
                    closestTimeDifference = timeDifference;
                    bestRoute = new List<int>(currentRoute);
                }
                continue;
            }

            // Get all neighbors for the current node
            if (!_adjacencyList.ContainsKey(currentNode)) continue;
            var neighbors = _adjacencyList[currentNode];

            foreach (var edge in neighbors)
            {
                int nextNode = edge.To.Id.GetHashCode();
                int routeTime = currentTime + (int)edge.Duration + (int)edge.TimeSpend;

                // Skip revisiting nodes in the current route to avoid cycles
                if (currentRoute.Contains(nextNode)) continue;

                // Evaluate and enqueue the new path
                var newRoute = new List<int>(currentRoute) { nextNode };
                pathQueue.Enqueue((newRoute, routeTime));

                // Check if this new route is closer to `totalAvailableTime`
                int newTimeDifference = Math.Abs(totalAvailableTime - routeTime);
                if (newTimeDifference < closestTimeDifference)
                {
                    closestTimeDifference = newTimeDifference;
                    bestRoute = new List<int>(newRoute);
                }
            }
        }

        return bestRoute;
    }





    private double GetAverageVisitTime(string category)
    {
        var categoryVisitTimes = new Dictionary<string, double>
        {
            { "Museum", 60 },        // 60 minutes
            { "Park", 30 },          // 30 minutes
            { "Restaurant", 90 },    // 90 minutes
            { "Shopping", 45 }       // 45 minutes
        };

        return categoryVisitTimes.ContainsKey(category) ? categoryVisitTimes[category] : 20; // Default to 20 if not specified
    }
}

