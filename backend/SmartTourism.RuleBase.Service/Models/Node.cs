namespace SmartTourism.RuleBase.Service.Models;

public class Node
{
    public string Name { get; set; }
    public double GScore { get; set; } // Cost from start to current node
    public double HScore { get; set; } // Heuristic estimate to target
    public double FScore => GScore + HScore; // Total estimated cost
    public Node Parent { get; set; } // Parent for path reconstruction
}
