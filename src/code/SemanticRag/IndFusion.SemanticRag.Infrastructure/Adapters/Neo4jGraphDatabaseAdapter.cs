using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndQuestResults;
using IndQuestResults.Operations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using static IndFusion.SemanticRag.Domain.Errors.ResultExtensionsWithErrorCodes;

namespace IndFusion.SemanticRag.Infrastructure.Adapters;

/// <summary>
/// Neo4j-based implementation of the graph database port.
/// </summary>
public class Neo4jGraphDatabaseAdapter : IGraphDatabasePort
{
	private readonly IDriver _driver;
	private readonly ILogger<Neo4jGraphDatabaseAdapter> _logger;
	private readonly Neo4jOptions _options;

	/// <summary>
	/// Initializes a new instance of the <see cref="Neo4jGraphDatabaseAdapter"/> class.
	/// </summary>
	/// <param name="driver">The Neo4j driver instance.</param>
	/// <param name="options">The Neo4j configuration options.</param>
	/// <param name="logger">The logger instance.</param>
	public Neo4jGraphDatabaseAdapter(
		IDriver driver,
		IOptions<Neo4jOptions> options,
		ILogger<Neo4jGraphDatabaseAdapter> logger)
	{
		_driver = driver ?? throw new ArgumentNullException(nameof(driver));
		_options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <inheritdoc />
	public async Task<Result<IReadOnlyList<CypherRecord>>> ExecuteReadAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(cypher))
		{
			return Result<IReadOnlyList<CypherRecord>>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Cancelled<IReadOnlyList<CypherRecord>>(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(cypher)
				.Ensure(
					query => !string.IsNullOrWhiteSpace(query),
					ErrorCodes.ParameterNullOrWhitespace))
				.ThenAsync(
					async query =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result<IReadOnlyList<CypherRecord>>.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Executing read query: {Cypher}", query);

						using var session = _driver.AsyncSession(ConfigureSession(database));
						var result = await session.RunAsync(query, parameters ?? new Dictionary<string, object>());
						var records = await result.ToListAsync(cancellationToken);

						var cypherRecords = records.Select(MapToCypherRecord).ToList();

						_logger.LogInformation("Read query completed: {Count} records returned", cypherRecords.Count);
						return Result<IReadOnlyList<CypherRecord>>.Success(cypherRecords.AsReadOnly());
					});
		}
		catch (OperationCanceledException)
		{
			return Cancelled<IReadOnlyList<CypherRecord>>(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to execute read query: {Cypher}", cypher);
			return WithFailure<IReadOnlyList<CypherRecord>>(ErrorCodes.CypherQueryFailed, ex);
		}
	}

	/// <inheritdoc />
	public async Task<Result<IReadOnlyList<CypherRecord>>> ExecuteWriteAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(cypher))
		{
			return Result<IReadOnlyList<CypherRecord>>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Cancelled<IReadOnlyList<CypherRecord>>(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(cypher)
				.Ensure(
					query => !string.IsNullOrWhiteSpace(query),
					ErrorCodes.ParameterNullOrWhitespace))
				.ThenAsync(
					async query =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result<IReadOnlyList<CypherRecord>>.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Executing write query: {Cypher}", query);

						using var session = _driver.AsyncSession(ConfigureSession(database));
						var result = await session.RunAsync(query, parameters ?? new Dictionary<string, object>());
						var records = await result.ToListAsync(cancellationToken);

						var cypherRecords = records.Select(MapToCypherRecord).ToList();

						_logger.LogInformation("Write query completed: {Count} records returned", cypherRecords.Count);
						return Result<IReadOnlyList<CypherRecord>>.Success(cypherRecords.AsReadOnly());
					});
		}
		catch (OperationCanceledException)
		{
			return Cancelled<IReadOnlyList<CypherRecord>>(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to execute write query: {Cypher}", cypher);
			return WithFailure<IReadOnlyList<CypherRecord>>(ErrorCodes.CypherQueryFailed, ex);
		}
	}

	/// <inheritdoc />
	public async Task<Result<CypherRecord?>> ExecuteReadSingleAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(cypher))
		{
			return Result<CypherRecord?>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Cancelled<CypherRecord?>(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(cypher)
				.Ensure(
					query => !string.IsNullOrWhiteSpace(query),
					ErrorCodes.ParameterNullOrWhitespace))
				.ThenAsync(
					async query =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result<CypherRecord?>.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Executing read single query: {Cypher}", query);

						using var session = _driver.AsyncSession(ConfigureSession(database));
						var result = await session.RunAsync(query, parameters ?? new Dictionary<string, object>());
						var record = await result.SingleOrDefaultAsync(cancellationToken);

						if (record == null)
						{
							_logger.LogInformation("Read single query returned no results");
							return Result<CypherRecord?>.Success(null);
						}

						var cypherRecord = MapToCypherRecord(record);

						_logger.LogInformation("Read single query completed: 1 record returned");
						return Result<CypherRecord?>.Success(cypherRecord);
					});
		}
		catch (OperationCanceledException)
		{
			return Cancelled<CypherRecord?>(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to execute read single query: {Cypher}", cypher);
			return WithFailure<CypherRecord?>(ErrorCodes.CypherQueryFailed, ex);
		}
	}

	/// <inheritdoc />
	public async Task<Result<CypherRecord?>> ExecuteWriteSingleAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(cypher))
		{
			return Result<CypherRecord?>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Cancelled<CypherRecord?>(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(cypher)
				.Ensure(
					query => !string.IsNullOrWhiteSpace(query),
					ErrorCodes.ParameterNullOrWhitespace))
				.ThenAsync(
					async query =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result<CypherRecord?>.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Executing write single query: {Cypher}", query);

						using var session = _driver.AsyncSession(ConfigureSession(database));
						var result = await session.RunAsync(query, parameters ?? new Dictionary<string, object>());
						var record = await result.SingleOrDefaultAsync(cancellationToken);

						if (record == null)
						{
							_logger.LogInformation("Write single query returned no results");
							return Result<CypherRecord?>.Success(null);
						}

						var cypherRecord = MapToCypherRecord(record);

						_logger.LogInformation("Write single query completed: 1 record returned");
						return Result<CypherRecord?>.Success(cypherRecord);
					});
		}
		catch (OperationCanceledException)
		{
			return Cancelled<CypherRecord?>(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to execute write single query: {Cypher}", cypher);
			return WithFailure<CypherRecord?>(ErrorCodes.CypherQueryFailed, ex);
		}
	}

	/// <inheritdoc />
	public async Task<Result> ExecuteReadVoidAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(cypher))
		{
			return Result.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(cypher)
				.Ensure(
					query => !string.IsNullOrWhiteSpace(query),
					ErrorCodes.ParameterNullOrWhitespace))
				.ThenAsync<string, Result>(
					async query =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Executing read void query: {Cypher}", query);

						using var session = _driver.AsyncSession(ConfigureSession(database));
						await session.RunAsync(query, parameters ?? new Dictionary<string, object>());

						_logger.LogInformation("Read void query completed successfully");
						return Result.Success();
					});
		}
		catch (OperationCanceledException)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to execute read void query: {Cypher}", cypher);
			return Result.WithFailure(ErrorCodes.CypherQueryFailed);
		}
	}

	/// <inheritdoc />
	public async Task<Result> ExecuteWriteVoidAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(cypher))
		{
			return Result.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(cypher)
				.Ensure(
					query => !string.IsNullOrWhiteSpace(query),
					ErrorCodes.ParameterNullOrWhitespace))
				.ThenAsync<string, Result>(
					async query =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Executing write void query: {Cypher}", query);

						using var session = _driver.AsyncSession(ConfigureSession(database));
						await session.RunAsync(query, parameters ?? new Dictionary<string, object>());

						_logger.LogInformation("Write void query completed successfully");
						return Result.Success();
					});
		}
		catch (OperationCanceledException)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to execute write void query: {Cypher}", cypher);
			return Result.WithFailure(ErrorCodes.CypherQueryFailed);
		}
	}

	/// <summary>
	/// Maps a Neo4j IRecord to a domain CypherRecord.
	/// </summary>
	private static CypherRecord MapToCypherRecord(IRecord record)
	{
		var keys = record.Keys.ToList().AsReadOnly();
		var values = new Dictionary<string, object>();

		foreach (var key in keys)
		{
			values[key] = record[key];
		}

		return new CypherRecord(Keys: keys, Values: values);
	}

	/// <summary>
	/// Configures the Neo4j session with database and other options.
	/// </summary>
	private Action<SessionConfigBuilder> ConfigureSession(string? database)
	{
		return config =>
		{
			config.WithDatabase(database ?? _options.Database);
		};
	}
}

