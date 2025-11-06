namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Represents a record returned from a Cypher query.
/// </summary>
/// <param name="Keys">Keys in the result record.</param>
/// <param name="Values">Values in the result record, indexed by key.</param>
public record CypherRecord(
    IReadOnlyList<string> Keys,
    Dictionary<string, object> Values);