namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// A search hit from vector search.
/// </summary>
/// <param name="PointId">ID of the matched point.</param>
/// <param name="Score">Similarity score.</param>
/// <param name="Payload">Metadata payload of the point.</param>
/// <param name="Vector">Vector embedding (if included in results).</param>
public record VectorSearchHit(
    ulong PointId,
    float Score,
    Dictionary<string, object>? Payload = null,
    float[]? Vector = null);