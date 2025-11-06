using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Queries.VectorSearch;

/// <summary>
/// Query to search for similar vectors using vector similarity search.
/// </summary>
/// <param name="Query">The vector search query containing search parameters.</param>
public record SearchSimilarVectorsQuery(VectorSearchQuery Query);