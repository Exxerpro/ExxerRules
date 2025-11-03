using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.Tests.Unit.Helpers;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Implementations;

/// <summary>
/// TDD tests for Neo4jGraphDatabaseAdapter implementation behavior.
/// These tests validate the concrete implementation behavior, not the interface contract.
/// Tests use real adapter instances with mocked underlying dependencies.
/// </summary>
public class Neo4jGraphDatabaseAdapterTests : BaseTDDTest<Neo4jGraphDatabaseAdapter>
{
	private IDriver _driver = null!;
	private IOptions<Neo4jOptions> _options = null!;
	private ILogger<Neo4jGraphDatabaseAdapter> _logger = null!;

	protected override Neo4jGraphDatabaseAdapter CreateImplementation()
	{
		// ✅ TDD: Mock underlying Neo4j driver to test adapter behavior
		_driver = Substitute.For<IDriver>();
		_logger = NullLogger<Neo4jGraphDatabaseAdapter>.Instance;
		
		var neo4jOptions = new Neo4jOptions
		{
			Uri = "bolt://localhost:7687",
			Username = "neo4j",
			Password = "test",
			Database = "neo4j"
		};
		_options = Options.Create(neo4jOptions);
		
		return new Neo4jGraphDatabaseAdapter(_driver, _options, _logger);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteReadAsync_WithNullCypher_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior - adapter validates before calling Neo4j
		var result = await Implementation.ExecuteReadAsync(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteReadAsync_WithEmptyCypher_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.ExecuteReadAsync(string.Empty);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteWriteAsync_WithNullCypher_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.ExecuteWriteAsync(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteWriteAsync_WithEmptyCypher_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.ExecuteWriteAsync(string.Empty);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteReadSingleAsync_WithNullCypher_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.ExecuteReadSingleAsync(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteWriteSingleAsync_WithNullCypher_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.ExecuteWriteSingleAsync(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteReadVoidAsync_WithNullCypher_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.ExecuteReadVoidAsync(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteWriteVoidAsync_WithNullCypher_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.ExecuteWriteVoidAsync(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly with error code
		AssertResultFailure(result, ErrorCodes.ParameterNullOrWhitespace);
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteReadAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.ExecuteReadAsync("MATCH (n) RETURN n", cancellationToken: cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteWriteAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.ExecuteWriteAsync("CREATE (n:Test)", cancellationToken: cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteReadSingleAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.ExecuteReadSingleAsync("MATCH (n) RETURN n LIMIT 1", cancellationToken: cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteWriteSingleAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.ExecuteWriteSingleAsync("CREATE (n:Test) RETURN n", cancellationToken: cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteReadVoidAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.ExecuteReadVoidAsync("MATCH (n) RETURN n", cancellationToken: cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}

	[Fact(Timeout = 5000)]
	public async Task ExecuteWriteVoidAsync_WithCancellation_ShouldReturnCancelled()
	{
		// ✅ TDD: Test cancellation handling
		var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		
		var result = await Implementation.ExecuteWriteVoidAsync("CREATE (n:Test)", cancellationToken: cancellationTokenSource.Token);
		
		// ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
		result.ShouldBeCancelled();
	}
}

