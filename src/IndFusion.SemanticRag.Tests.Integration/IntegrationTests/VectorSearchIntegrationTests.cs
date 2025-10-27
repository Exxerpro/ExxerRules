using IndFusion.SemanticRag.Application.Commands.VectorSearch;
using IndFusion.SemanticRag.Application.Queries.VectorSearch;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Infrastructure;
using IndFusion.SemanticRag.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;
using IndQuestResults;

namespace IndFusion.SemanticRag.Tests.Integration.IntegrationTests;

/// <summary>
/// Integration tests for vector search functionality.
/// </summary>
public class VectorSearchIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the VectorSearchIntegrationTests class.
    /// </summary>
    /// <param name="fixture">The integration test fixture.</param>
    public VectorSearchIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _mediator = _fixture.ServiceProvider.GetRequiredService<IMediator>();
    }

    /// <summary>
    /// Sets up the test by clearing the repository.
    /// </summary>
    private async Task SetupTestAsync()
    {
        await _fixture.ClearRepositoryAsync();
    }

    /// <summary>
    /// Should store and retrieve vectors successfully.
    /// </summary>
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
        var storeResult = await _mediator.Send(storeCommand, TestContext.Current.CancellationToken);

        // Assert
        storeResult.IsSuccess.ShouldBeTrue();
        storeResult.Error.ShouldBeNull();
    }

    /// <summary>
    /// Should search for similar vectors successfully.
    /// </summary>
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
        
        await _mediator.Send(storeCommand1, TestContext.Current.CancellationToken);
        await _mediator.Send(storeCommand2, TestContext.Current.CancellationToken);

        var searchQuery = new SearchSimilarVectorsQuery(query);
        var searchResult = await _mediator.SendQuery<SearchSimilarVectorsQuery, IReadOnlyList<VectorSearchResult>>(searchQuery, TestContext.Current.CancellationToken);

        // Assert
        searchResult.IsSuccess.ShouldBeTrue();
        searchResult.Value.ShouldNotBeNull();
        searchResult.Value.Count.ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Should handle invalid vector data gracefully.
    /// </summary>
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
        var storeResult = await _mediator.Send(storeCommand, TestContext.Current.CancellationToken);

        // Assert
        storeResult.IsFailure.ShouldBeTrue();
        storeResult.Error.ShouldNotBeNull();
        storeResult.Error.ShouldContain("Vector ID cannot be null or empty");
    }

    /// <summary>
    /// Should handle invalid search query gracefully.
    /// </summary>
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
        var searchResult = await _mediator.Send(searchQuery, TestContext.Current.CancellationToken);

        // Assert
        searchResult.IsFailure.ShouldBeTrue();
        searchResult.Error.ShouldNotBeNull();
        searchResult.Error.ShouldContain("Search query cannot be null or empty");
    }

    /// <summary>
    /// Should handle empty repository gracefully.
    /// </summary>
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
        var searchResult = await _mediator.SendQuery<SearchSimilarVectorsQuery, IReadOnlyList<VectorSearchResult>>(searchQuery, TestContext.Current.CancellationToken);

        // Assert
        searchResult.IsSuccess.ShouldBeTrue();
        searchResult.Value.ShouldNotBeNull();
        searchResult.Value.Count.ShouldBe(0);
    }
}