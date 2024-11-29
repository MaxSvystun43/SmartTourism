namespace SmartTourism.DijstraSolver.Service.Models;

public record SourceTarget
{
    public int SourceId { get; set; }
    public int TargetId { get; set; }
    public int? Distance { get; set; } = Int32.MaxValue;
    public int? Time { get; set; } = Int32.MaxValue;
}