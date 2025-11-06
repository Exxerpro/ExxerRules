namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Statistics about the vector index.
/// </summary>
/// <param name="TotalVectors">Total number of vectors in the index.</param>
/// <param name="IndexSize">Size of the index in bytes.</param>
/// <param name="LastUpdated">When the index was last updated.</param>
/// <param name="AverageVectorDimension">Average dimension of vectors in the index.</param>
public record VectorIndexStatistics(
    long TotalVectors,
    long IndexSize,
    DateTimeOffset LastUpdated,
    int AverageVectorDimension
);