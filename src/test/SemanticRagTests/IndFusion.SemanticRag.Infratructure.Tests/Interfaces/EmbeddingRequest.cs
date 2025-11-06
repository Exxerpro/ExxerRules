namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Interfaces;

public class EmbeddingRequest
{
    public string Text { get; set; } = null!;
    public Dictionary<string, object>? Metadata { get; set; }
}