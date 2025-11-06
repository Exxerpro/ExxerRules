namespace IndFusion.SemanticRag.Application.Tests.Interfaces;

public class CypherQuery
{
    public string Cypher { get; set; } = null!;
    public Dictionary<string, object>? Parameters { get; set; }
    public string? Database { get; set; }
}