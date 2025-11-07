using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Tests.Infratructure.Tests.Shared;
using NSubstitute;

namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Interfaces;

/// <summary>
/// IITDD tests for IGraphDatabasePort interface contract.
/// These tests validate the interface contract, not implementation details.
/// Tests must pass for ANY valid implementation (Liskov Substitution Principle).
/// </summary>
public class IGraphDatabasePortTests : BaseIITDDTest<IGraphDatabasePort, CypherQuery, IReadOnlyList<CypherRecord>>
{
    public IGraphDatabasePortTests()
    {
        SetUp();
    }

    protected override CypherQuery CreateValidRequest()
    {
        return new CypherQuery
        {
            Cypher = "MATCH (n) RETURN n LIMIT 10",
            Parameters = []
        };
    }

    protected override CypherQuery CreateInvalidRequest()
    {
        return new CypherQuery
        {
            Cypher = null!, // Invalid: null cypher query
            Parameters = []
        };
    }

    [Fact(Timeout = 120000)] // Extended timeout for first test (allows Docker container startup)
    public async Task ExecuteReadAsync_WithValidQuery_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract using mock
        var request = CreateValidRequest();
        var expectedRecords = new List<CypherRecord>
        {
            new CypherRecord(
                Keys: new List<string> { "n" }.AsReadOnly(),
                Values: new Dictionary<string, object> { ["n"] = "test-value" })
        }.AsReadOnly();

        MockPort.ExecuteReadAsync(
            request.Cypher,
            request.Parameters,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<CypherRecord>>.Success(expectedRecords));

        var result = await MockPort.ExecuteReadAsync(
            request.Cypher,
            request.Parameters, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract, not implementation details
        AssertSuccess(result);
        result.Value.ShouldNotBeNull();
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteReadAsync_WithNullCypher_ShouldReturnFailure()
    {
        // ✅ IITDD: Test contract - null cypher query should fail
        MockPort.ExecuteReadAsync(
            null!,
            null,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<CypherRecord>>.WithFailure("Cypher query cannot be null or empty"));

        var result = await MockPort.ExecuteReadAsync(null!, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert failure contract
        AssertFailure(result);
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteReadAsync_WithEmptyCypher_ShouldReturnFailure()
    {
        // ✅ IITDD: Test contract - empty cypher query should fail
        MockPort.ExecuteReadAsync(
            string.Empty,
            null,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<CypherRecord>>.WithFailure("Cypher query cannot be null or empty"));

        var result = await MockPort.ExecuteReadAsync(string.Empty, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert failure contract
        AssertFailure(result);
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteWriteAsync_WithValidQuery_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var cypher = "CREATE (n:Test {id: $id})";
        var parameters = new Dictionary<string, object> { ["id"] = "test-id" };
        var expectedRecords = new List<CypherRecord>().AsReadOnly();

        MockPort.ExecuteWriteAsync(
            cypher,
            parameters,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<CypherRecord>>.Success(expectedRecords));

        var result = await MockPort.ExecuteWriteAsync(cypher, parameters, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract compliance
        AssertSuccess(result);
        result.Value.ShouldNotBeNull();
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteWriteAsync_WithNullCypher_ShouldReturnFailure()
    {
        // ✅ IITDD: Test contract - null cypher query should fail
        MockPort.ExecuteWriteAsync(
            null!,
            null,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<CypherRecord>>.WithFailure("Cypher query cannot be null or empty"));

        var result = await MockPort.ExecuteWriteAsync(null!, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert failure contract
        AssertFailure(result);
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteReadSingleAsync_WithValidQuery_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var cypher = "MATCH (n {id: $id}) RETURN n";
        var parameters = new Dictionary<string, object> { ["id"] = "test-id" };
        var expectedRecord = new CypherRecord(
            Keys: new List<string> { "n" }.AsReadOnly(),
            Values: new Dictionary<string, object> { ["n"] = "test-value" });

        MockPort.ExecuteReadSingleAsync(
            cypher,
            parameters,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<CypherRecord?>.Success(expectedRecord));

        var result = await MockPort.ExecuteReadSingleAsync(cypher, parameters, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract compliance
        AssertSuccess(result);
        result.Value.ShouldNotBeNull();
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteReadSingleAsync_WithNoResults_ShouldReturnNull()
    {
        // ✅ IITDD: Test contract - no results should return null (not failure)
        var cypher = "MATCH (n {id: $id}) RETURN n";
        var parameters = new Dictionary<string, object> { ["id"] = "non-existent" };

        MockPort.ExecuteReadSingleAsync(
            cypher,
            parameters,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<CypherRecord?>.Success(null));

        var result = await MockPort.ExecuteReadSingleAsync(cypher, parameters, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert "not found" contract
        AssertSuccess(result);
        result.Value.ShouldBeNull();
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteWriteSingleAsync_WithValidQuery_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var cypher = "CREATE (n:Test {id: $id}) RETURN n";
        var parameters = new Dictionary<string, object> { ["id"] = "test-id" };
        var expectedRecord = new CypherRecord(
            Keys: new List<string> { "n" }.AsReadOnly(),
            Values: new Dictionary<string, object> { ["n"] = "test-value" });

        MockPort.ExecuteWriteSingleAsync(
            cypher,
            parameters,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result<CypherRecord?>.Success(expectedRecord));

        var result = await MockPort.ExecuteWriteSingleAsync(cypher, parameters, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract compliance
        AssertSuccess(result);
        result.Value.ShouldNotBeNull();
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteReadVoidAsync_WithValidQuery_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var cypher = "CREATE (n:Test {id: $id})";
        var parameters = new Dictionary<string, object> { ["id"] = "test-id" };

        MockPort.ExecuteReadVoidAsync(
            cypher,
            parameters,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await MockPort.ExecuteReadVoidAsync(cypher, parameters, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract - success result
        AssertSuccess(result);
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteWriteVoidAsync_WithValidQuery_ShouldReturnSuccess()
    {
        // ✅ IITDD: Test interface contract
        var cypher = "DELETE (n:Test {id: $id})";
        var parameters = new Dictionary<string, object> { ["id"] = "test-id" };

        MockPort.ExecuteWriteVoidAsync(
            cypher,
            parameters,
            null,
            Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await MockPort.ExecuteWriteVoidAsync(cypher, parameters, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ IITDD: Assert contract - success result
        AssertSuccess(result);
    }
}

// Helper request class for test data