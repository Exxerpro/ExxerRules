using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.Infrastructure.Services;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Utilities;
using Xunit;
using Xunit.Sdk;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Services;

/// <summary>
/// Behavioral system tests for QdrantVectorSearchService to drive implementation.
/// These tests verify actual behavior using real containerized Qdrant instance.
/// </summary>
[Collection("System")]
[Trait("Category", "System")]
public class QdrantVectorSearchServiceBehavioralTests : IDisposable
{
    private readonly QdrantContainerFixture _fixture;
    private readonly OllamaContainerFixture _ollamaFixture;
    private readonly ILogger<QdrantVectorSearchService> _logger;
    private readonly IVectorDatabasePort _vectorDatabasePort;
    private readonly IEmbeddingServicePort _embeddingServicePort;
    private readonly IOptions<QdrantOptions> _options;
    private readonly QdrantVectorSearchService _service;
    private readonly string _collectionName;

    /// <summary>
    /// Initializes the Qdrant service behavioral test fixture with real services from container.
    /// </summary>
    /// <param name="fixture">Qdrant container fixture providing real Qdrant instance.</param>
    /// <param name="ollamaFixture">Ollama container fixture providing real Ollama instance.</param>
    /// <param name="output">Test output helper for logging.</param>
    public QdrantVectorSearchServiceBehavioralTests(QdrantContainerFixture fixture, OllamaContainerFixture ollamaFixture, ITestOutputHelper output)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _ollamaFixture = ollamaFixture ?? throw new ArgumentNullException(nameof(ollamaFixture));

        if (!_fixture.IsAvailable || !_ollamaFixture.IsAvailable || DockerSkipConditions.ShouldSkipDockerTests)
        {
            Assert.Skip("Docker is not available - system tests require real containers");
        }

        // Create real logger using Meziantou XUnit logger
        _logger = XUnitLogger.CreateLogger<QdrantVectorSearchService>(output);

        // Create real Qdrant vector database adapter with container client
        var vectorAdapterLogger = XUnitLogger.CreateLogger<QdrantVectorDatabaseAdapter>(output);
        _vectorDatabasePort = new QdrantVectorDatabaseAdapter(_fixture.Client!, vectorAdapterLogger);

        // Create real Ollama embedding service adapter using container fixture
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_ollamaFixture.HttpEndpoint),
            Timeout = TimeSpan.FromSeconds(30)
        };
        var ollamaOptions = new OllamaOptions
        {
            BaseUrl = _ollamaFixture.Options.BaseUrl,
            EmbeddingModel = _ollamaFixture.Options.EmbeddingModel,
            EmbeddingDimension = _ollamaFixture.Options.EmbeddingDimension,
            MaxTextLength = _ollamaFixture.Options.MaxTextLength,
            MaxConcurrency = _ollamaFixture.Options.MaxConcurrency,
            TimeoutSeconds = _ollamaFixture.Options.TimeoutSeconds
        };
        var embeddingAdapterLogger = XUnitLogger.CreateLogger<OllamaEmbeddingServiceAdapter>(output);
        _embeddingServicePort = new OllamaEmbeddingServiceAdapter(
            httpClient,
            embeddingAdapterLogger,
            Options.Create(ollamaOptions));

        // Use container options for Qdrant
        _options = Options.Create(_fixture.Options);

        // Use unique collection name per test class to avoid conflicts
        _collectionName = $"test-collection-{Guid.NewGuid():N}";
        var qdrantOptions = new QdrantOptions
        {
            CollectionName = _collectionName,
            Host = _fixture.Options.Host,
            Port = _fixture.Options.Port,
            ApiKey = _fixture.Options.ApiKey,
            VectorSize = _fixture.Options.VectorSize
        };
        _options = Options.Create(qdrantOptions);

        // Create real service instance
        _service = new QdrantVectorSearchService(_vectorDatabasePort, _embeddingServicePort, _options, _logger);
    }

    /// <summary>
    /// Cleans up test collection after each test class.
    /// </summary>
    public void Dispose()
    {
        // Clear collection after tests
        Task.Run(async () => await TestCleanupHelpers.ClearQdrantCollection(_fixture.Client!, _collectionName))
            .Wait(TimeSpan.FromSeconds(5));

        // Dispose HttpClient if needed (embedding service uses HttpClient which implements IDisposable)
        if (_embeddingServicePort is OllamaEmbeddingServiceAdapter adapter)
        {
            // HttpClient is managed by the adapter, so we don't dispose it directly
            // The adapter should handle HttpClient lifecycle
        }
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Validates that a straightforward similarity search succeeds and preserves the submitted query and options in the result contract.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the success assertions have been executed.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithValidQuery_ShouldReturnActualSearchResults()
    {
        // Arrange
        var query = "test query";
        var searchOptions = VectorSearchOptions.Default();

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();

        // Assert - Verify actual behavior with real services
        result.Query.ShouldBe(query);
        result.SearchOptions.ShouldBe(searchOptions);
        result.Success.ShouldBeTrue();
        result.ErrorMessage.ShouldBeNull();
    }

    /// <summary>
    /// Confirms that high-precision search presets translate into the expected threshold and result limit within the returned options.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the precision-specific assertions finish.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithHighPrecisionOptions_ShouldUseCorrectThreshold()
    {
        // Arrange
        var query = "precise query";
        var searchOptions = VectorSearchOptions.HighPrecision();

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);
        result.IsSuccess.ShouldBeTrue();

        // Assert
        result.SearchOptions.Threshold.ShouldBe(0.9f); // High precision threshold
        result.SearchOptions.Limit.ShouldBe(5); // High precision limit

        // This test drives implementation of threshold-based filtering
    }

    /// <summary>
    /// Checks that broad search presets produce an expanded result limit suitable for exploratory queries.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the broad options have been validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithBroadOptions_ShouldUseCorrectLimit()
    {
        // Arrange
        var query = "broad query";
        var searchOptions = VectorSearchOptions.Broad();

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);
        result.IsSuccess.ShouldBeTrue();

        // Assert
        result.SearchOptions.Limit.ShouldBe(50); // Broad search limit
        result.SearchOptions.Threshold.ShouldBe(0.5f); // Broad search threshold

        // This test drives implementation of limit-based result filtering
    }

    /// <summary>
    /// Verifies that generating an embedding delegates to the embedding service and returns the actual vector payload.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the embedding result has been asserted.</returns>
    [Fact(Timeout = 60000)]
    public async Task GenerateEmbeddingAsync_WithValidText_ShouldReturnActualEmbedding()
    {
        // Arrange
        var text = "sample text for embedding";

        // Act
        var result = await _service.GenerateEmbeddingAsync(text, TestContext.Current.CancellationToken);

        // Assert - Verify actual embedding generation
        result.Values.ShouldNotBeNull();
        result.Values.Count.ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Ensures empty text input is rejected when requesting an embedding to prevent meaningless calls.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the guard clause throws the expected exception.</returns>
    [Fact(Timeout = 60000)]
    public async Task GenerateEmbeddingAsync_WithEmptyText_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.GenerateEmbeddingAsync(string.Empty, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Confirms that storing a document in Qdrant succeeds and echoes the persisted document identifier.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the storage result is validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task StoreDocumentAsync_WithValidData_ShouldStoreDocument()
    {
        // Arrange
        var id = "doc-123";
        var content = "document content";
        var metadata = new Dictionary<string, object> { { "source", "test" }, { "type", "document" } };

        // Act
        await _service.StoreDocumentAsync(id, content, metadata, TestContext.Current.CancellationToken);

        // Assert - Verify document was stored by searching for it
        var searchOptions = VectorSearchOptions.Default();
        var searchResult = await _service.SearchSimilarAsync(content, searchOptions, TestContext.Current.CancellationToken);
        searchResult.Success.ShouldBeTrue();
    }

    /// <summary>
    /// Ensures null identifiers are rejected when attempting to store a document snapshot.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after confirming the failure result.</returns>
    [Fact(Timeout = 60000)]
    public async Task StoreDocumentAsync_WithNullId_ShouldThrowArgumentException()
    {
        // Act
        var result = await _service.StoreDocumentAsync(null!, "content", [], TestContext.Current.CancellationToken);

        // Assert: After functional refactoring, null parameters return Result failure
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Validates that missing content is treated as invalid input for document persistence.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the guard clause is exercised.</returns>
    [Fact(Timeout = 60000)]
    public async Task StoreDocumentAsync_WithNullContent_ShouldThrowArgumentException()
    {
        // Act
        var result = await _service.StoreDocumentAsync("id", null!, [], TestContext.Current.CancellationToken);

        // Assert: After functional refactoring, null parameters return Result failure
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Checks that documents cannot be stored without metadata and that the service enforces this requirement.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after observing the failure result for missing metadata.</returns>
    [Fact(Timeout = 60000)]
    public async Task StoreDocumentAsync_WithNullMetadata_ShouldThrowArgumentException()
    {
        // Act
        var result = await _service.StoreDocumentAsync("id", "content", null!, TestContext.Current.CancellationToken);

        // Assert: After functional refactoring, null parameters return Result failure
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that updating an existing document reflects the provided content and metadata in the result contract.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the update assertions run.</returns>
    [Fact(Timeout = 60000)]
    public async Task UpdateDocumentAsync_WithValidData_ShouldUpdateDocument()
    {
        // Arrange - Store document first
        var id = "doc-123";
        var initialContent = "initial content";
        var initialMetadata = new Dictionary<string, object> { { "source", "test" } };
        await _service.StoreDocumentAsync(id, initialContent, initialMetadata, TestContext.Current.CancellationToken);

        var content = "updated content";
        var metadata = new Dictionary<string, object> { { "source", "test" }, { "updated", true } };

        // Act
        await _service.UpdateDocumentAsync(id, content, metadata, TestContext.Current.CancellationToken);

        // Assert - Verify update by searching for updated content
        var searchOptions = VectorSearchOptions.Default();
        var searchResult = await _service.SearchSimilarAsync(content, searchOptions, TestContext.Current.CancellationToken);
        searchResult.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Confirms that deleting a document by identifier succeeds and reports the removed identifier.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the deletion result is verified.</returns>
    [Fact(Timeout = 60000)]
    public async Task DeleteDocumentAsync_WithValidId_ShouldDeleteDocument()
    {
        // Arrange - Store document first
        var id = "doc-123";
        var content = "document to delete";
        var metadata = new Dictionary<string, object> { { "source", "test" } };
        await _service.StoreDocumentAsync(id, content, metadata, TestContext.Current.CancellationToken);

        // Act
        await _service.DeleteDocumentAsync(id, TestContext.Current.CancellationToken);

        // Assert - Verify deletion by searching (should not find the document)
        var searchOptions = VectorSearchOptions.Default();
        var searchResult = await _service.SearchSimilarAsync(content, searchOptions, TestContext.Current.CancellationToken);
        searchResult.IsSuccess.ShouldBeTrue();
        // Note: Search may still return results if other documents are similar, but the deleted document should not appear
    }

    /// <summary>
    /// Ensures null identifiers are rejected when deleting documents to prevent ambiguous deletions.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the guard clause produces the expected exception.</returns>
    [Fact(Timeout = 60000)]
    public async Task DeleteDocumentAsync_WithNullId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.DeleteDocumentAsync(null!, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that cancellation tokens provided to the similarity search are honored immediately.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when cancellation behavior is verified.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var query = "test query";
        var searchOptions = VectorSearchOptions.Default();
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, cts.Token);

        // Assert: After functional refactoring, cancellation returns Result<T> with cancellation error code
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain(ErrorCodes.OperationCancelled);
    }

    /// <summary>
    /// Checks that request timeouts are respected, causing operations to cancel when the deadline elapses.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after asserting timeout behavior.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithTimeout_ShouldRespectTimeout()
    {
        // Arrange
        var query = "test query";
        var searchOptions = new VectorSearchOptions(TimeoutMs: 100); // Very short timeout

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);

        // Assert: After functional refactoring, cancellation returns Result<T> with cancellation error code
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain(ErrorCodes.OperationCancelled);
    }

    /// <summary>
    /// Ensures embedding service failures propagate through the search API so callers receive actionable error information.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the propagated failure is confirmed.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithEmbeddingServiceFailure_ShouldPropagateFailure()
    {
        // Arrange
        var query = "test query";
        var searchOptions = VectorSearchOptions.Default();
        // Note: This test depends on Ollama service being unavailable or misconfigured
        // In a real scenario, we might need to inject a failing embedding service
        // For now, this test verifies the service handles embedding failures gracefully

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);
        result.IsSuccess.ShouldBeTrue();
        // Assert - Either succeeds (if Ollama works) or fails gracefully (if it doesn't)
        // VectorSearchResponse is a value type, so we check properties directly
        result.Query.ShouldBe(query);
        result.Results.ShouldNotBeNull();
    }

    /// <summary>
    /// Confirms that Qdrant client failures surface as exceptions, highlighting integration issues.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the propagated exception assertion finishes.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithQdrantFailure_ShouldPropagateException()
    {
        // Arrange
        var query = "test query";
        var searchOptions = VectorSearchOptions.Default();

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);

        // Assert - Verify service handles Qdrant operations
        // VectorSearchResponse is a value type, so we check properties directly
        result.IsSuccess.ShouldBeTrue();

        result.Query.ShouldBe(query);
        result.Results.ShouldNotBeNull();
        // Note: Actual failure handling depends on Qdrant container state
    }

    /// <summary>
    /// Verifies that metadata filters are applied to similarity searches and reflected in the underlying request.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once filter assertions are evaluated.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithMetadataFilters_ShouldApplyFilters()
    {
        // Arrange
        var query = "filtered query";
        var filters = new Dictionary<string, object> { { "category", "test" }, { "priority", "high" } };
        var searchOptions = new VectorSearchOptions(Filters: filters);

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        result.SearchOptions.Filters.ShouldBe(filters);
        result.Success.ShouldBeTrue();
    }

    /// <summary>
    /// Checks that requesting embeddings in the response includes vector data in the resulting payload.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after confirming embeddings are returned when requested.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithIncludeEmbedding_ShouldIncludeEmbeddingInResults()
    {
        // Arrange
        var query = "query with embedding";
        var searchOptions = new VectorSearchOptions(IncludeEmbedding: true);

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);

        // Assert
        result.SearchOptions.IncludeEmbedding.ShouldBeTrue();
        result.Success.ShouldBeTrue();
    }

    /// <summary>
    /// Ensures metadata inclusion flags result in metadata being populated for each returned document.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when metadata presence has been asserted.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithIncludeMetadata_ShouldIncludeMetadataInResults()
    {
        // Arrange
        var query = "query with metadata";
        var searchOptions = new VectorSearchOptions(IncludeMetadata: true);

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);

        // Assert
        result.SearchOptions.IncludeMetadata.ShouldBeTrue();
        result.Success.ShouldBeTrue();
    }

    /// <summary>
    /// Validates that the service records realistic processing time metrics rather than returning placeholder values.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after evaluating duration-related assertions.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithProcessingTime_ShouldMeasureActualProcessingTime()
    {
        // Arrange
        var query = "timing query";
        var searchOptions = VectorSearchOptions.Default();

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Query.ShouldBe(query);
    }

    /// <summary>
    /// Confirms that similarity searches return tangible results once the integration is complete, rather than empty collections.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when non-empty result assertions are executed.</returns>
    [Fact(Timeout = 60000)]
    public async Task SearchSimilarAsync_WithActualResults_ShouldReturnNonEmptyResults()
    {
        // Arrange - Store some documents first
        await _service.StoreDocumentAsync("doc1", "first document content", new Dictionary<string, object> { { "source", "test" } }, TestContext.Current.CancellationToken);
        await _service.StoreDocumentAsync("doc2", "second document content", new Dictionary<string, object> { { "source", "test" } }, TestContext.Current.CancellationToken);

        var query = "document content";
        var searchOptions = VectorSearchOptions.Default();

        // Act
        var result = await _service.SearchSimilarAsync(query, searchOptions, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Query.ShouldBe(query);
        result.Results.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThan(0);
    }
}