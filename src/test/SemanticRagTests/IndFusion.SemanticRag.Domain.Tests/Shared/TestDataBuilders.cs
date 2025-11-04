using IndFusion.SemanticRag.Domain.Builders;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Tests.Shared;

/// <summary>
/// Helper methods for creating test data using factory builders.
/// Provides convenient methods for creating valid and invalid test objects following the railway pattern.
/// </summary>
public static class TestDataBuilders
{
    /// <summary>
    /// Creates a valid <see cref="KnowledgeEntity"/> using the factory builder.
    /// </summary>
    /// <param name="id">Optional entity ID. Defaults to "entity-1".</param>
    /// <param name="name">Optional entity name. Defaults to "John Doe".</param>
    /// <param name="type">Optional entity type. Defaults to "Person".</param>
    /// <returns>A Result containing a valid KnowledgeEntity.</returns>
    public static Result<KnowledgeEntity> CreateValidKnowledgeEntity(
        string id = "entity-1",
        string name = "John Doe",
        string type = "Person")
    {
        return KnowledgeEntityBuilder.Build(
            id: id,
            name: name,
            type: type,
            description: "Test entity description",
            properties: new Dictionary<string, object> { ["key"] = "value" },
            confidence: 0.9,
            createdAt: DateTime.UtcNow);
    }

    /// <summary>
    /// Creates an invalid <see cref="KnowledgeEntity"/> using the factory builder (null ID).
    /// </summary>
    /// <returns>A Result containing a failure for KnowledgeEntity creation.</returns>
    public static Result<KnowledgeEntity> CreateInvalidKnowledgeEntity()
    {
        return KnowledgeEntityBuilder.Build(
            id: null!, // Invalid: null ID
            name: "John Doe",
            type: "Person",
            description: "Test entity description",
            properties: [],
            confidence: 0.9,
            createdAt: DateTime.UtcNow);
    }

    /// <summary>
    /// Creates a valid <see cref="KnowledgeRelationship"/> using the factory builder.
    /// </summary>
    /// <param name="id">Optional relationship ID. Defaults to "rel-1".</param>
    /// <param name="fromNodeId">Optional source node ID. Defaults to "entity-1".</param>
    /// <param name="toNodeId">Optional target node ID. Defaults to "entity-2".</param>
    /// <returns>A Result containing a valid KnowledgeRelationship.</returns>
    public static Result<KnowledgeRelationship> CreateValidKnowledgeRelationship(
        string id = "rel-1",
        string fromNodeId = "entity-1",
        string toNodeId = "entity-2")
    {
        return KnowledgeRelationshipBuilder.Build(
            id: id,
            fromNodeId: fromNodeId,
            toNodeId: toNodeId,
            relationshipType: "KNOWS",
            properties: new Dictionary<string, object> { ["since"] = DateTime.UtcNow },
            createdAt: DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Creates an invalid <see cref="KnowledgeRelationship"/> using the factory builder (null ID).
    /// </summary>
    /// <returns>A Result containing a failure for KnowledgeRelationship creation.</returns>
    public static Result<KnowledgeRelationship> CreateInvalidKnowledgeRelationship()
    {
        return KnowledgeRelationshipBuilder.Build(
            id: null!, // Invalid: null ID
            fromNodeId: "entity-1",
            toNodeId: "entity-2",
            relationshipType: "KNOWS",
            properties: new Dictionary<string, object>(),
            createdAt: DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Creates a valid <see cref="VectorEmbedding"/> using the factory builder.
    /// </summary>
    /// <param name="id">Optional embedding ID. Defaults to "emb-1".</param>
    /// <param name="content">Optional content. Defaults to "Test content".</param>
    /// <param name="embeddingSize">Optional embedding vector size. Defaults to 4.</param>
    /// <returns>A Result containing a valid VectorEmbedding.</returns>
    public static Result<VectorEmbedding> CreateValidVectorEmbedding(
        string id = "emb-1",
        string content = "Test content",
        int embeddingSize = 4)
    {
        var embedding = new float[embeddingSize];
        for (int i = 0; i < embeddingSize; i++)
        {
            embedding[i] = 0.1f * (i + 1);
        }

        return VectorEmbeddingBuilder.Build(
            id: id,
            content: content,
            embedding: embedding,
            metadata: new Dictionary<string, object> { ["source"] = "test" },
            createdAt: DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Creates an invalid <see cref="VectorEmbedding"/> using the factory builder (null ID).
    /// </summary>
    /// <returns>A Result containing a failure for VectorEmbedding creation.</returns>
    public static Result<VectorEmbedding> CreateInvalidVectorEmbedding()
    {
        return VectorEmbeddingBuilder.Build(
            id: null!, // Invalid: null ID
            content: "Test content",
            embedding: new float[] { 0.1f, 0.2f, 0.3f, 0.4f },
            metadata: new Dictionary<string, object>(),
            createdAt: DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Creates a valid <see cref="Document"/> using the factory builder.
    /// </summary>
    /// <param name="id">Optional document ID. Defaults to "doc-1".</param>
    /// <param name="content">Optional content. Defaults to "Test document content".</param>
    /// <returns>A Result containing a valid Document.</returns>
    public static Result<Document> CreateValidDocument(
        string id = "doc-1",
        string content = "Test document content")
    {
        return DocumentBuilder.Build(
            id: id,
            content: content,
            sourcePath: "/path/to/document.md",
            repository: "test-repo",
            commitHash: "abc123def456",
            documentType: DocumentType.Markdown,
            metadata: new Dictionary<string, object> { ["author"] = "Test Author" },
            createdAt: DateTimeOffset.UtcNow,
            updatedAt: DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Creates an invalid <see cref="Document"/> using the factory builder (null ID).
    /// </summary>
    /// <returns>A Result containing a failure for Document creation.</returns>
    public static Result<Document> CreateInvalidDocument()
    {
        return DocumentBuilder.Build(
            id: null!, // Invalid: null ID
            content: "Test document content",
            sourcePath: "/path/to/document.md",
            repository: "test-repo",
            commitHash: "abc123def456",
            documentType: DocumentType.Markdown,
            metadata: new Dictionary<string, object>(),
            createdAt: DateTimeOffset.UtcNow,
            updatedAt: DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Creates a valid <see cref="SemanticDocument"/> using the factory builder.
    /// </summary>
    /// <param name="id">Optional document ID. Defaults to "semantic-doc-1".</param>
    /// <param name="title">Optional title. Defaults to "Test Document".</param>
    /// <param name="content">Optional content. Defaults to "Test semantic document content".</param>
    /// <returns>A Result containing a valid SemanticDocument.</returns>
    public static Result<SemanticDocument> CreateValidSemanticDocument(
        string id = "semantic-doc-1",
        string title = "Test Document",
        string content = "Test semantic document content")
    {
        return SemanticDocumentBuilder.Build(
            id: id,
            title: title,
            content: content,
            metadata: new Dictionary<string, object> { ["source"] = "test" },
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow);
    }

    /// <summary>
    /// Creates an invalid <see cref="SemanticDocument"/> using the factory builder (null ID).
    /// </summary>
    /// <returns>A Result containing a failure for SemanticDocument creation.</returns>
    public static Result<SemanticDocument> CreateInvalidSemanticDocument()
    {
        return SemanticDocumentBuilder.Build(
            id: null!, // Invalid: null ID
            title: "Test Document",
            content: "Test semantic document content",
            metadata: [],
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow);
    }

    /// <summary>
    /// Asserts that a builder result is successful and extracts the value.
    /// This helper ensures the railway pattern is followed correctly.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result from a factory builder.</param>
    /// <returns>The value from the successful result.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is a failure.</exception>
    public static T GetValueOrThrow<T>(Result<T> result)
    {
        result.IsSuccess.ShouldBeTrue($"Expected successful result, but got error: {result.Error}");
        
        // Only check for null if T is a reference type (value types can't be null)
        if (!typeof(T).IsValueType)
        {
            ((object?)result.Value).ShouldNotBeNull("Result value should not be null when successful");
        }
        
        return result.Value!;
    }
}
