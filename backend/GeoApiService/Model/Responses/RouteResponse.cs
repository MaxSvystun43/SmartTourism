using System.Text.Json.Serialization;

namespace GeoApiService.Model.Responses;

public class SourceToTarget
{
    public int? Distance { get; set; }
    public int? Time { get; set; }
    [JsonPropertyName("source_index")]
    public int SourceIndex { get; set; }
    [JsonPropertyName("target_index")]
    public int TargetIndex { get; set; }
}

public class RouteResponse
{
    [JsonPropertyName("sources_to_targets")]
    public List<List<SourceToTarget>> SourcesToTargets { get; set; }
    public string Mode { get; set; }
    public string Type { get; set; }
    public string Traffic { get; set; }
}