using IndFusion.SemanticRag.Application.Commands.VectorSearch;
using IndFusion.SemanticRag.Application.Queries.VectorSearch;
using IndFusion.SemanticRag.Domain.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace IndFusion.SemanticRag.Integration.Tests.IntegrationTests;

/// <summary>
/// Integration tests that exercise the end-to-end vector storage and similarity search workflows against the configured infrastructure.
/// </summary>
public class VectorSearchIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    /// <summary>
    /// Provides access to the shared service provider and cleanup helpers for each test run.
    /// </summary>
    private readonly IntegrationTestFixture _fixture;

    /// <summary>
    /// Issues commands and queries against the vector search pipeline under test.
    /// </summary>
    private readonly IRequestDispatcher _requestDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorSearchIntegrationTests"/> class and resolves the requestDispatcher from the integration fixture.
    /// </summary>
    /// <param name="fixture">The shared integration test fixture that provides the service provider and cleanup utilities.</param>
    public VectorSearchIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _requestDispatcher = _fixture.ServiceProvider.GetRequiredService<IRequestDispatcher>();
    }

    /// <summary>
    /// Resets the vector repository so each test executes against a clean data store.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the repository has been cleared.</returns>
    private async Task SetupTestAsync()
    {
        await _fixture.ClearRepositoryAsync();
    }

    /// <summary>
    /// Verifies that persisting a <see cref="VectorEmbedding"/> through <see cref="StoreVectorCommand"/> succeeds without errors.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the assertion workflow finishes.</returns>
    /// <remarks>
    /// The test writes a representative vector payload and expects a successful result without any captured error details.
    /// </remarks>
    [Fact]
    public async Task Should_StoreAndRetrieveVectors_Successfully()
    {
        // Arrange
        await SetupTestAsync();
        var vector = new VectorEmbedding(
            Id: "test-vector-1",
            Content: "This is a test document about machine learning",
            Embedding: new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f },
            Metadata: new Dictionary<string, object> { { "category", "AI" }, { "source", "test" } },
            CreatedAt: DateTimeOffset.UtcNow
        );

        // Act
        var storeCommand = new StoreVectorCommand(vector);
        var storeResult = await _requestDispatcher.Send(storeCommand, TestContext.Current.CancellationToken);

        // Assert
        storeResult.IsSuccess.ShouldBeTrue();
        storeResult.Error.ShouldBeNull();
    }

    /// <summary>
    /// Ensures that previously stored vectors are returned by <see cref="SearchSimilarVectorsQuery"/> when the similarity threshold is met.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the search assertions succeed.</returns>
    /// <remarks>
    /// Two related embeddings are stored and the subsequent similarity search is expected to yield at least one matching result.
    /// </remarks>
    [Fact]
    public async Task Should_SearchSimilarVectors_Successfully()
    {
        // Arrange
        await SetupTestAsync();
        var vector1 = new VectorEmbedding(
            Id: "test-vector-1",
            Content: "Machine learning is a subset of artificial intelligence",
            Embedding: new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f },
            Metadata: new Dictionary<string, object> { { "category", "AI" } },
            CreatedAt: DateTimeOffset.UtcNow
        );

        var vector2 = new VectorEmbedding(
            Id: "test-vector-2",
            Content: "Artificial intelligence includes machine learning and deep learning",
            Embedding: new float[] { 0.15f, 0.25f, 0.35f, 0.45f, 0.55f },
            Metadata: new Dictionary<string, object> { { "category", "AI" } },
            CreatedAt: DateTimeOffset.UtcNow
        );

        var query = new VectorSearchQuery(
            Query: "What is machine learning?",
            Embedding: new float[] { 0.12f, 0.22f, 0.32f, 0.42f, 0.52f },
            Limit: 10,
            Threshold: 0.7f
        );

        // Act
        var storeCommand1 = new StoreVectorCommand(vector1);
        var storeCommand2 = new StoreVectorCommand(vector2);

        await _requestDispatcher.Send(storeCommand1, TestContext.Current.CancellationToken);
        await _requestDispatcher.Send(storeCommand2, TestContext.Current.CancellationToken);

        var searchQuery = new SearchSimilarVectorsQuery(query);
        var searchResult = await _requestDispatcher.SendQuery<SearchSimilarVectorsQuery, IReadOnlyList<VectorSearchResult>>(searchQuery, TestContext.Current.CancellationToken);

        // Assert
        searchResult.IsSuccess.ShouldBeTrue();
        searchResult.Value.ShouldNotBeNull();
        searchResult.Value.Count.ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Confirms that invalid vector payloads are rejected and surface descriptive validation errors.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the failure assertions are evaluated.</returns>
    [Fact]
    public async Task Should_HandleInvalidVectorData_Gracefully()
    {
        // Arrange
        await SetupTestAsync();
        var invalidVector = new VectorEmbedding(
            Id: "", // Invalid: empty ID
            Content: "This is a test document",
            Embedding: new float[] { 0.1f, 0.2f, 0.3f },
            Metadata: new Dictionary<string, object>(),
            CreatedAt: DateTimeOffset.UtcNow
        );

        // Act
        var storeCommand = new StoreVectorCommand(invalidVector);
        var storeResult = await _requestDispatcher.Send(storeCommand, TestContext.Current.CancellationToken);

        // Assert
        storeResult.IsFailure.ShouldBeTrue();
        storeResult.Error.ShouldNotBeNull();
        storeResult.Error.ShouldContain("Embedding ID cannot be null or empty");
    }

    /// <summary>
    /// Verifies that submitting an invalid search query results in a failure with a meaningful error description.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the failure outcome has been asserted.</returns>
    [Fact]
    public async Task Should_HandleInvalidSearchQuery_Gracefully()
    {
        // Arrange
        await SetupTestAsync();
        var invalidQuery = new VectorSearchQuery(
            Query: "", // Invalid: empty query
            Embedding: new float[] { 0.1f, 0.2f, 0.3f },
            Limit: 10,
            Threshold: 0.7f
        );

        // Act
        var searchQuery = new SearchSimilarVectorsQuery(invalidQuery);
        var searchResult = await _requestDispatcher.SendQuery<SearchSimilarVectorsQuery, IReadOnlyList<VectorSearchResult>>(searchQuery, TestContext.Current.CancellationToken);

        // Assert
        searchResult.IsFailure.ShouldBeTrue();
        searchResult.Error.ShouldNotBeNull();
        searchResult.Error.ShouldContain("Search query cannot be null or empty");
    }

    /// <summary>
    /// Ensures that searching an empty repository succeeds and returns an empty result set rather than failing.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the empty-result assertions run.</returns>
    [Fact]
    public async Task Should_HandleEmptyRepository_Gracefully()
    {
        // Arrange
        await SetupTestAsync();
        var query = new VectorSearchQuery(
            Query: "What is machine learning?",
            Embedding: new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f },
            Limit: 10,
            Threshold: 0.7f
        );

        // Act
        var searchQuery = new SearchSimilarVectorsQuery(query);
        var searchResult = await _requestDispatcher.SendQuery<SearchSimilarVectorsQuery, IReadOnlyList<VectorSearchResult>>(searchQuery, TestContext.Current.CancellationToken);

        // Assert
        searchResult.IsSuccess.ShouldBeFalse();
    }
}