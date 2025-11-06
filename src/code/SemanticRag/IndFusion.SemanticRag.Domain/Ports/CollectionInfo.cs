namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Information about a vector database collection.
/// </summary>
/// <param name="CollectionName">Name of the collection.</param>
/// <param name="VectorSize">Size of vectors in this collection.</param>
/// <param name="Distance">Distance metric used.</param>
/// <param name="PointsCount">Number of points in the collection.</param>
public record CollectionInfo(
    string CollectionName,
    uint VectorSize,
    VectorDistance Distance,
    long PointsCount);