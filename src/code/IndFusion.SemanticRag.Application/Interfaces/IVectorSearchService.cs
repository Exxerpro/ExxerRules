using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.ValueObjects;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for vector search operations using embeddings.
/// </summary>
public interface IVectorSearchService
{
    /// <summary>
    /// Searches for similar content using vector embeddings.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="options">Search options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search results.</returns>
    Task<VectorSearchResponse> SearchSimilarAsync(
        string query, 
        VectorSearchOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for the given text.
    /// </summary>
    /// <param name="text">The text to embed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Embedding vector.</returns>
    Task<EmbeddingVector> GenerateEmbeddingAsync(
        string text, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a document with its embeddings in the vector database.
    /// </summary>
    /// <param name="id">Unique identifier for the document.</param>
    /// <param name="content">Document content.</param>
    /// <param name="metadata">Document metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task StoreDocumentAsync(
        string id, 
        string content, 
        Dictionary<string, object> metadata, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document from the vector database.
    /// </summary>
    /// <param name="id">Document identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task DeleteDocumentAsync(
        string id, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a document in the vector database.
    /// </summary>
    /// <param name="id">Document identifier.</param>
    /// <param name="content">Updated content.</param>
    /// <param name="metadata">Updated metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    Task UpdateDocumentAsync(
        string id, 
        string content, 
        Dictionary<string, object> metadata, 
        CancellationToken cancellationToken = default);
}
