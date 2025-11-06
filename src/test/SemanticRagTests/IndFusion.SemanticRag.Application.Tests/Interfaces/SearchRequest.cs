namespace IndFusion.SemanticRag.Application.Tests.Interfaces;

public class SearchRequest
{
    public string? CollectionName { get; set; }
    public float[]? QueryVector { get; set; }
    public uint Limit { get; set; }
    public float? ScoreThreshold { get; set; }
    public Dictionary<string, object>? Filter { get; set; }
}