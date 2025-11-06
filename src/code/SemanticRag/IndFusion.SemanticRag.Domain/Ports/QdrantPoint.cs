namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// A point to be upserted into a vector database.
/// </summary>
/// <param name="Id">Unique identifier for the point.</param>
/// <param name="Vector">Vector embedding.</param>
/// <param name="Payload">Metadata payload.</param>
public record QdrantPoint(
    ulong Id,
    float[] Vector,
    Dictionary<string, object>? Payload = null);