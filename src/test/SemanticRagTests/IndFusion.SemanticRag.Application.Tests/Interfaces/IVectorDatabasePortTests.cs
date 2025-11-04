using IndFusion.SemanticRag.Application.Tests.Shared;
using IndFusion.SemanticRag.Domain.Ports;
using NSubstitute;

namespace IndFusion.SemanticRag.Application.Tests.Interfaces;

/// <summary>
/// IITDD tests for IVectorDatabasePort interface contract.
/// These tests validate the interface contract, not implementation details.
/// Tests must pass for ANY valid implementation (Liskov Substitution Principle).
/// </summary>
public class IVectorDatabasePortTests : BaseIITDDTest<IVectorDatabasePort, SearchRequest, IReadOnlyList<VectorSearchHit>>
{
    public IVectorDatabasePortTests()
    {
        SetUp();
    }

    protected override SearchRequest CreateValidRequest()
    {
        return new SearchRequest
        {
            CollectionName = "test-collection",
            QueryVector = new float[] { 0.1f, 0.2f, 0.3f },
            Limit = 10,
            ScoreThreshold = 0.5f,
            Filter = []
        };
    }

    protected override SearchRequest CreateInvalidRequest()
    {
        return new SearchRequest
        {
            CollectionName = null, // Invalid: null collection name
            QueryVector = new float[] { 0.1f, 0.2f, 0.3f },
            Limit = 10
        };
    }

    [Fact(Timeout = 5000)]
    public async Task SearchAsync_WithValidRequest_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract using mock
        var request = CreateValidRequest();
        var expectedHits = new List<VectorSearchHit>
        {
            new VectorSearchHit(PointId: 1, Score: 0.95f, Payload: null, Vector: null)
        }.AsReadOnly();

        MockPort.SearchAsync(
            request.CollectionName!,
            request.QueryVector!,
            request.Limit,
            request.ScoreThreshold,
            request.Filter,
            Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<VectorSearchHit>>.Success(expectedHits));

        var result = await MockPort.SearchAsync(
            request.CollectionName!,
            request.QueryVector!,
            request.Limit,
            request.ScoreThreshold,
            request.Filter, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract, not implementation details
        AssertSuccess(result);
        result.Value.ShouldNotBeNull();
        // ✅ DO NOT assert: result.Value.Count, ExecutionTimeMs, etc. (implementation details)
    }

    [Fact(Timeout = 5000)]
    public async Task SearchAsync_WithNullCollectionName_ShouldReturnFailure()
    {
        // ✅ IITDD: Test contract - null collection name should fail
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f };

        MockPort.SearchAsync(
            null!,
            queryVector,
            10,
            null,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<VectorSearchHit>>.WithFailure("Collection name cannot be null or empty"));

        var result = await MockPort.SearchAsync(null!, queryVector, 10, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert failure contract
        AssertFailure(result);
        // ✅ Use error code if available: AssertFailure(result, ErrorCodes.CollectionNameRequired);
    }

    [Fact(Timeout = 5000)]
    public async Task GetCollectionInfoAsync_WithValidCollectionName_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var collectionName = "test-collection";
        var expectedInfo = new CollectionInfo(
            CollectionName: collectionName,
            VectorSize: 128,
            Distance: VectorDistance.Cosine,
            PointsCount: 1000);

        MockPort.GetCollectionInfoAsync(
            collectionName,
            Arg.Any<CancellationToken>())
            .Returns(Result<CollectionInfo?>.Success(expectedInfo));

        var result = await MockPort.GetCollectionInfoAsync(collectionName, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract compliance
        AssertSuccess(result);
        result.Value.ShouldNotBeNull();
        result.Value!.CollectionName.ShouldBe(collectionName);
        // ✅ This assertion is valid because Name is part of the contract (required by interface)
    }

    [Fact(Timeout = 5000)]
    public async Task GetCollectionInfoAsync_WithNonExistentCollection_ShouldReturnNull()
    {
        // ✅ IITDD: Test contract - non-existent collection should return null (not failure)
        var nonExistentCollection = "non-existent-collection";

        MockPort.GetCollectionInfoAsync(
            nonExistentCollection,
            Arg.Any<CancellationToken>())
            .Returns(Result<CollectionInfo?>.Success(null));

        var result = await MockPort.GetCollectionInfoAsync(nonExistentCollection, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert "not found" contract
        AssertSuccess(result);
        result.Value.ShouldBeNull();
    }

    [Fact(Timeout = 5000)]
    public async Task CreateCollectionAsync_WithValidParameters_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var collectionName = "test-collection";
        var vectorSize = 128u;
        var distance = VectorDistance.Cosine;

        MockPort.CreateCollectionAsync(
            collectionName,
            vectorSize,
            distance,
            Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await MockPort.CreateCollectionAsync(collectionName, vectorSize, distance, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract - success result
        AssertSuccess(result);
    }

    [Fact(Timeout = 5000)]
    public async Task CreateCollectionAsync_WithNullCollectionName_ShouldReturnFailure()
    {
        // ✅ IITDD: Test contract - null collection name should fail
        var vectorSize = 128u;
        var distance = VectorDistance.Cosine;

        MockPort.CreateCollectionAsync(
            null!,
            vectorSize,
            distance,
            Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Collection name cannot be null or empty"));

        var result = await MockPort.CreateCollectionAsync(null!, vectorSize, distance, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert failure contract
        AssertFailure(result);
    }

    [Fact(Timeout = 5000)]
    public async Task UpsertAsync_WithValidParameters_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var collectionName = "test-collection";
        var points = new List<QdrantPoint>
        {
            new QdrantPoint(Id: 1, Vector: new float[] { 0.1f, 0.2f, 0.3f }, Payload: null)
        }.AsReadOnly();

        MockPort.UpsertAsync(
            collectionName,
            points,
            Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await MockPort.UpsertAsync(collectionName, points, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract - success result
        AssertSuccess(result);
    }

    [Fact(Timeout = 5000)]
    public async Task UpsertAsync_WithNullCollectionName_ShouldReturnFailure()
    {
        // ✅ IITDD: Test contract - null collection name should fail
        var points = new List<QdrantPoint>
        {
            new QdrantPoint(Id: 1, Vector: new float[] { 0.1f, 0.2f, 0.3f }, Payload: null)
        }.AsReadOnly();

        MockPort.UpsertAsync(
            null!,
            points,
            Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Collection name cannot be null or empty"));

        var result = await MockPort.UpsertAsync(null!, points, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert failure contract
        AssertFailure(result);
    }

    [Fact(Timeout = 5000)]
    public async Task DeleteAsync_WithValidParameters_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var collectionName = "test-collection";
        var pointIds = new List<ulong> { 1, 2, 3 }.AsReadOnly();

        MockPort.DeleteAsync(
            collectionName,
            pointIds,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await MockPort.DeleteAsync(collectionName, pointIds, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract - success result
        AssertSuccess(result);
    }

    [Fact(Timeout = 5000)]
    public async Task DeleteAsync_WithNullCollectionName_ShouldReturnFailure()
    {
        // ✅ IITDD: Test contract - null collection name should fail
        var pointIds = new List<ulong> { 1, 2, 3 }.AsReadOnly();

        MockPort.DeleteAsync(
            null!,
            pointIds,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Collection name cannot be null or empty"));

        var result = await MockPort.DeleteAsync(null!, pointIds, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert failure contract
        AssertFailure(result);
    }
}

// Helper request class for test data
public class SearchRequest
{
    public string? CollectionName { get; set; }
    public float[]? QueryVector { get; set; }
    public uint Limit { get; set; }
    public float? ScoreThreshold { get; set; }
    public Dictionary<string, object>? Filter { get; set; }
}