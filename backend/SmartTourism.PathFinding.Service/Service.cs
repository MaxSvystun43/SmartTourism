using GeoApiService.Model.Responses;

public class DijkstraSorver
{
    private readonly Dictionary<int, List<SourceToTarget>> _adjacencyList = new();
    
    public DijkstraSorver(RouteResponse results)
    {
        foreach (var edgeList in results.SourcesToTargets)
        {
            foreach (var edge in edgeList)
            {
                AddRoute(edge.SourceIndex, edge.TargetIndex, edge.Distance, edge.Time);
            }
        }
    }

    public void AddRoute(int source, int target, int distance, int time)
    {
        if (!_adjacencyList.ContainsKey(source))
            _adjacencyList[source] = new List<SourceToTarget>();

        _adjacencyList[source].Add(new SourceToTarget { SourceIndex = source, TargetIndex = target, Distance = distance, Time = time });
    }

    public Dictionary<int, int> Dijkstra(int start)
    {
        var distances = new Dictionary<int, int>();
        var priorityQueue = new SortedSet<(int Distance, int Node)>();

        foreach (var node in _adjacencyList.Keys)
        {
            distances[node] = int.MaxValue;
        }
        distances[start] = 0;
        priorityQueue.Add((0, start));

        while (priorityQueue.Count > 0)
        {
            var (currentDistance, currentNode) = priorityQueue.Min;
            priorityQueue.Remove(priorityQueue.Min);

            if (currentDistance > distances[currentNode]) continue;

            foreach (var edge in _adjacencyList[currentNode])
            {
                int newDistance = currentDistance + edge.Distance;

                if (newDistance < distances[edge.TargetIndex])
                {
                    distances[edge.TargetIndex] = newDistance;
                    priorityQueue.Add((newDistance, edge.TargetIndex));
                }
            }
        }

        return distances;
    }

    public List<int> FindShortestRoute(int start, int end, List<int> waypoints)
    {
        List<int> bestRoute = new List<int>();
        int minDistance = int.MaxValue;

        // Add start and end to waypoints
        waypoints.Insert(0, start);
        waypoints.Add(end);

        var allDistances = new Dictionary<(int, int), int>();

        // Get distances between all waypoints
        for (int i = 0; i < waypoints.Count; i++)
        {
            for (int j = 0; j < waypoints.Count; j++)
            {
                if (i != j)
                {
                    var distancesFromWaypoint = Dijkstra(waypoints[i]);
                    allDistances[(waypoints[i], waypoints[j])] = distancesFromWaypoint[waypoints[j]];
                }
            }
        }

        // Use permutations to find the shortest path visiting all waypoints
        var permutations = GetPermutations(waypoints.Skip(1).ToList(), waypoints.Count - 2);

        foreach (var perm in permutations)
        {
            List<int> currentRoute = new List<int> { start };
            currentRoute.AddRange(perm);
            currentRoute.Add(end);

            int currentDistance = 0;
            for (int i = 0; i < currentRoute.Count - 1; i++)
            {
                currentDistance += allDistances[(currentRoute[i], currentRoute[i + 1])];
            }

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                bestRoute = new List<int>(currentRoute);
            }
        }

        return bestRoute;
    }

    private IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1)
            return list.Select(t => new T[] { t });

        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(e => !t.Contains(e)),
                        (t1, t2) => t1.Concat(new T[] { t2 }));
    }
}

