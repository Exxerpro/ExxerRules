namespace IndFusion.SemanticRag.Domain.Tests.Interfaces;

public class EmbeddingRequest
{
    public string Text { get; set; } = null!;
    public Dictionary<string, object>? Metadata { get; set; }
}