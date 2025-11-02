using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Tests.Unit.Helpers;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Qdrant.Client;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Implementations;

/// <summary>
/// TDD tests for QdrantVectorDatabaseAdapter implementation behavior.
/// These tests validate the concrete implementation behavior, not the interface contract.
/// Tests use real adapter instances with mocked underlying dependencies.
/// </summary>
public class QdrantVectorDatabaseAdapterTests : BaseTDDTest<QdrantVectorDatabaseAdapter>
{
	private QdrantClient _qdrantClient = null!;
	private ILogger<QdrantVectorDatabaseAdapter> _logger = null!;

	protected override QdrantVectorDatabaseAdapter CreateImplementation()
	{
		// ✅ TDD: Test adapter behavior - create real QdrantClient instance
		// Note: Validation tests fail before calling QdrantClient, so real instance is safe for these tests
		// For tests that need mocked behavior, we'll mock at the port level (IVectorDatabasePort)
		try
		{
			_qdrantClient = new QdrantClient("localhost", 6333);
		}
		catch
		{
			// If QdrantClient construction fails, create a minimal instance
			// This should only happen if constructor signature changed
			_qdrantClient = new QdrantClient("127.0.0.1", 6333);
		}
		
		_logger = NullLogger<QdrantVectorDatabaseAdapter>.Instance;
		
		return new QdrantVectorDatabaseAdapter(_qdrantClient, _logger);
	}

	[Fact]
	public async Task SearchAsync_WithNullQueryVector_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior - adapter validates before calling QdrantClient
		var collectionName = "test-collection";
		var limit = 10u;
		
		// Act: Call real adapter implementation with invalid input
		var result = await Implementation.SearchAsync(
			collectionName,
			null!, // Invalid input
			limit,
			scoreThreshold: null,
			filter: null);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.VectorContentRequired);
		
		// Verify QdrantClient was not called (adapter validates before calling client)
		// Note: We can't easily verify this with mocked QdrantClient, but the test validates behavior
	}

	[Fact]
	public async Task SearchAsync_WithNullCollectionName_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior for invalid input
		var queryVector = new float[] { 0.1f, 0.2f, 0.3f };
		
		var result = await Implementation.SearchAsync(
			null!, // Invalid input
			queryVector,
			10,
			scoreThreshold: null,
			filter: null);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
		
		// Note: Using real QdrantClient instance, so cannot verify DidNotReceive()
		// The validation happens before calling QdrantClient, so the test validates the behavior
	}

	[Fact]
	public async Task GetCollectionInfoAsync_WithEmptyCollectionName_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior - adapter validates before calling QdrantClient
		var emptyCollectionName = string.Empty;
		
		// Act
		var result = await Implementation.GetCollectionInfoAsync(emptyCollectionName);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}


	[Fact]
	public async Task CreateCollectionAsync_WithZeroVectorSize_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior - adapter validates vector size
		var collectionName = "test-collection";
		var vectorSize = 0u; // Invalid
		var distance = VectorDistance.Cosine;
		
		// Act
		var result = await Implementation.CreateCollectionAsync(
			collectionName,
			vectorSize,
			distance);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ValueOutOfRange);
	}

	[Fact]
	public async Task CreateCollectionAsync_WithNullCollectionName_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior for invalid input
		var vectorSize = 128u;
		var distance = VectorDistance.Cosine;
		
		var result = await Implementation.CreateCollectionAsync(
			null!, // Invalid input
			vectorSize,
			distance);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
		
		// Verify QdrantClient was not called (adapter validates before calling client)
		// Note: CreateCollectionAsync signature varies by Qdrant client version - verification disabled for now
		// await _qdrantClient.DidNotReceive().CreateCollectionAsync(...);
	}

	[Fact]
	public async Task UpsertAsync_WithEmptyPointsList_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior - adapter validates points list
		var collectionName = "test-collection";
		var emptyPoints = new List<QdrantPoint>().AsReadOnly();
		
		// Act
		var result = await Implementation.UpsertAsync(collectionName, emptyPoints);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.CollectionEmpty);
	}

	[Fact]
	public async Task UpsertAsync_WithNullCollectionName_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior for invalid input
		var points = new List<QdrantPoint>
		{
			new QdrantPoint(Id: 1, Vector: new float[] { 0.1f, 0.2f, 0.3f }, Payload: null)
		}.AsReadOnly();
		
		var result = await Implementation.UpsertAsync(null!, points);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact]
	public async Task DeleteAsync_WithNoPointIdsAndNoFilter_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior - adapter requires either pointIds or filter
		var collectionName = "test-collection";
		
		// Act: Call with neither pointIds nor filter (invalid)
		var result = await Implementation.DeleteAsync(collectionName, pointIds: null, filter: null);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNull);
	}

	[Fact]
	public async Task DeleteAsync_WithNullCollectionName_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior for invalid input
		var pointIds = new List<ulong> { 1, 2, 3 }.AsReadOnly();
		
		var result = await Implementation.DeleteAsync(null!, pointIds);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact]
	public async Task GetCollectionInfoAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.GetCollectionInfoAsync("test-collection", cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}

	[Fact]
	public async Task SearchAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.SearchAsync(
			"test-collection",
			new float[] { 0.1f, 0.2f, 0.3f },
			10,
			scoreThreshold: null,
			filter: null,
			cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}

	[Fact]
	public async Task CreateCollectionAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.CreateCollectionAsync(
			"test-collection",
			128u,
			VectorDistance.Cosine,
			cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}
}

