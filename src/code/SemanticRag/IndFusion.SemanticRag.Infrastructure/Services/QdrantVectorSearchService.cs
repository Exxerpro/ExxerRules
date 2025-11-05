using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Domain.ValueObjects;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Implementation of vector search service using Qdrant.
/// </summary>
public class QdrantVectorSearchService : IVectorSearchService
{
    private readonly IVectorDatabasePort _vectorDatabasePort;
    private readonly IEmbeddingServicePort _embeddingServicePort;
    private readonly ILogger<QdrantVectorSearchService> _logger;
    private readonly QdrantOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantVectorSearchService"/> class.
    /// </summary>
    /// <param name="vectorDatabasePort">Vector database port.</param>
    /// <param name="embeddingServicePort">Embedding service port.</param>
    /// <param name="options">Qdrant configuration options.</param>
    /// <param name="logger">Logger instance.</param>
    public QdrantVectorSearchService(
        IVectorDatabasePort vectorDatabasePort,
        IEmbeddingServicePort embeddingServicePort,
        IOptions<QdrantOptions> options,
        ILogger<QdrantVectorSearchService> logger)
    {
        _vectorDatabasePort = vectorDatabasePort ?? throw new ArgumentNullException(nameof(vectorDatabasePort));
        _embeddingServicePort = embeddingServicePort ?? throw new ArgumentNullException(nameof(embeddingServicePort));
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Ensures the collection exists and is properly configured.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the operation.</returns>
    private async Task<Result> EnsureCollectionExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var collectionInfoResult = await _vectorDatabasePort.GetCollectionInfoAsync(_options.CollectionName, cancellationToken);
            
            // If collection exists, return success
            if (collectionInfoResult.IsSuccess && collectionInfoResult.Value != null)
            {
                _logger.LogDebug("Collection already exists: {CollectionName}", _options.CollectionName);
                return Result.Success();
            }
            
            // If collection doesn't exist (NotFound error), create it
            if (collectionInfoResult.IsFailure)
            {
                // Result.Error might be a collection, so join all errors into a single string
                var errorMessage = collectionInfoResult.Error ?? string.Empty;
                // Normalize error message by removing newlines and extra whitespace for easier matching
                var normalizedErrorMessage = errorMessage.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
                
                _logger.LogInformation("GetCollectionInfoAsync returned failure: {Error}", errorMessage);
                
                // Check if the error is "NotFound" - this means collection doesn't exist and we should create it
                // Error format is typically: "VE005: Status(StatusCode="NotFound", Detail="...")"
                // The error message contains both the error code (VE005) and the exception details
                // Normalize the error message to handle newlines and special characters
                var isNotFoundError = !string.IsNullOrEmpty(normalizedErrorMessage) &&
                                     (normalizedErrorMessage.Contains("NotFound", StringComparison.OrdinalIgnoreCase) ||
                                     normalizedErrorMessage.Contains("doesn't exist", StringComparison.OrdinalIgnoreCase) ||
                                     normalizedErrorMessage.Contains("VE005", StringComparison.OrdinalIgnoreCase) ||
                                     normalizedErrorMessage.Contains("StatusCode=\"NotFound\"", StringComparison.OrdinalIgnoreCase) ||
                                     normalizedErrorMessage.Contains("StatusCode=NotFound", StringComparison.OrdinalIgnoreCase) ||
                                     normalizedErrorMessage.Contains("StatusCode='NotFound'", StringComparison.OrdinalIgnoreCase));
                
                if (isNotFoundError)
                {
                    _logger.LogInformation("Collection does not exist (NotFound detected), creating: {CollectionName}", _options.CollectionName);
                    
                    var createResult = await _vectorDatabasePort.CreateCollectionAsync(
                        _options.CollectionName,
                        (uint)_options.VectorSize,
                        VectorDistance.Cosine,
                        cancellationToken);
                    
                    if (createResult.IsFailure)
                    {
                        _logger.LogError("Failed to create collection: {Error}", createResult.Error);
                        return createResult;
                    }
                    
                    _logger.LogInformation("Collection created successfully: {CollectionName}", _options.CollectionName);
                    return Result.Success();
                }
                
                // If it's a different error, return it
                _logger.LogError("Failed to get collection info (non-NotFound error): {Error}", collectionInfoResult.Error);
                return Result.WithFailure(collectionInfoResult.Error ?? ErrorCodes.VectorDatabaseError);
            }

            // If result is success but value is null, collection doesn't exist - create it
            if (collectionInfoResult.Value == null)
            {
                _logger.LogInformation("Collection does not exist, creating: {CollectionName}", _options.CollectionName);
                
                var createResult = await _vectorDatabasePort.CreateCollectionAsync(
                    _options.CollectionName,
                    (uint)_options.VectorSize,
                    VectorDistance.Cosine,
                    cancellationToken);
                
                if (createResult.IsFailure)
                {
                    _logger.LogError("Failed to create collection: {Error}", createResult.Error);
                    return createResult;
                }
                
                _logger.LogInformation("Collection created successfully: {CollectionName}", _options.CollectionName);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring collection exists: {CollectionName}", _options.CollectionName);
            return Result.WithFailure($"Error ensuring collection exists: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<VectorSearchResponse> SearchSimilarAsync(
        string query, 
        VectorSearchOptions options, 
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        try
        {
            _logger.LogInformation("Starting vector search for query: {Query}", query);

            // Ensure collection exists
            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                _logger.LogError("Failed to ensure collection exists: {Error}", ensureResult.Error);
                return new VectorSearchResponse(
                    Results: [],
                    TotalCount: 0,
                    Query: query,
                    ProcessingTimeMs: (long)(DateTime.UtcNow - startTime).TotalMilliseconds,
                    SearchOptions: options,
                    Success: false,
                    ErrorMessage: $"Failed to ensure collection exists: {ensureResult.Error}");
            }

            // Generate embeddings for the query
            var embedding = await GenerateEmbeddingAsync(query, cancellationToken);

            // Convert EmbeddingVector to float[] for search
            var queryVector = embedding.Values.ToArray();

            // Build filter from options
            Dictionary<string, object>? domainFilter = null;
            if (options.Filters != null && options.Filters.Count > 0)
            {
                domainFilter = new Dictionary<string, object>(options.Filters);
            }

            // Execute vector search using the vector database port
            var searchResult = await _vectorDatabasePort.SearchAsync(
                collectionName: _options.CollectionName,
                queryVector: queryVector,
                limit: (uint)options.Limit,
                scoreThreshold: options.Threshold > 0 ? options.Threshold : null,
                filter: domainFilter,
                cancellationToken: cancellationToken);

            var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;

            // Handle search failure
            if (searchResult.IsFailure)
            {
                _logger.LogError("Vector search failed: {Error}", searchResult.Error);
                return new VectorSearchResponse(
                    Results: [],
                    TotalCount: 0,
                    Query: query,
                    ProcessingTimeMs: (long)elapsedMs,
                    SearchOptions: options,
                    Success: false,
                    ErrorMessage: searchResult.Error ?? "Vector search failed");
            }

            // Map VectorSearchHit to VectorSearchResult
            var results = new List<VectorSearchResult>();
            int rank = 1;
            
            if (searchResult.Value != null)
            {
                foreach (var hit in searchResult.Value)
                {
                    var score = hit.Score;
                    
                    // Apply threshold filter
                    if (score < options.Threshold)
                        continue;

                    // Extract metadata from payload
                    var metadata = new Dictionary<string, object>();
                    if (options.IncludeMetadata && hit.Payload != null)
                    {
                        foreach (var payloadKvp in hit.Payload)
                        {
                            metadata[payloadKvp.Key] = payloadKvp.Value;
                        }
                    }

                    // Extract content from payload or use default
                    var content = metadata.GetValueOrDefault("content")?.ToString() ?? string.Empty;

                    // Extract embedding vector if requested
                    var embeddingVector = options.IncludeEmbedding && hit.Vector != null
                        ? hit.Vector.ToArray()
                        : Array.Empty<float>();

                    var vectorEmbedding = new VectorEmbedding(
                        Id: hit.PointId.ToString(),
                        Content: content,
                        Embedding: embeddingVector,
                        Metadata: metadata as IReadOnlyDictionary<string, object> ?? new Dictionary<string, object>(metadata),
                        CreatedAt: DateTimeOffset.UtcNow
                    );

                    results.Add(new VectorSearchResult(
                        Vector: vectorEmbedding,
                        Similarity: score,
                        Rank: rank++));
                }
            }

            _logger.LogInformation(
                "Vector search completed. Found {Count} results in {ElapsedMs}ms", 
                results.Count, 
                elapsedMs);

            return new VectorSearchResponse(
                Results: results,
                TotalCount: results.Count,
                Query: query,
                ProcessingTimeMs: (long)elapsedMs,
                SearchOptions: options,
                Success: true,
                ErrorMessage: null);
        }
        catch (Exception ex)
        {
            var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogError(ex, "Error during vector search for query: {Query}", query);
            return new VectorSearchResponse(
                Results: [],
                TotalCount: 0,
                Query: query,
                ProcessingTimeMs: (long)elapsedMs,
                SearchOptions: options,
                Success: false,
                ErrorMessage: ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<EmbeddingVector> GenerateEmbeddingAsync(
        string text, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty", nameof(text));

        try
        {
            _logger.LogDebug("Generating embedding for text of length: {Length}", text.Length);

            var embeddingResult = await _embeddingServicePort.GenerateEmbeddingAsync(text, cancellationToken);
            if (embeddingResult.IsFailure)
            {
                _logger.LogError("Failed to generate embedding: {Error}", embeddingResult.Error);
                throw new InvalidOperationException($"Failed to generate embedding: {embeddingResult.Error}");
            }

            return new EmbeddingVector(embeddingResult.Value ?? Array.Empty<float>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding for text");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task StoreDocumentAsync(
        string id, 
        string content, 
        Dictionary<string, object> metadata, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be null or empty", nameof(content));
        
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        try
        {
            _logger.LogInformation("Storing document with ID: {Id}", id);

            // Ensure collection exists
            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                _logger.LogError("Failed to ensure collection exists: {Error}", ensureResult.Error);
                throw new InvalidOperationException($"Failed to ensure collection exists: {ensureResult.Error}");
            }

            // Generate embedding for document content
            var embedding = await GenerateEmbeddingAsync(content, cancellationToken);

            // Convert document ID to ulong (use hash if not numeric)
            if (!ulong.TryParse(id, out var pointId))
            {
                pointId = (ulong)id.GetHashCode();
            }

            // Create payload with metadata and content
            var payload = new Dictionary<string, object>(metadata);
            payload["content"] = content;
            payload["id"] = id;

            // Create QdrantPoint with embedding and metadata
            var point = new QdrantPoint(
                Id: pointId,
                Vector: embedding.Values.ToArray(),
                Payload: payload);

            // Upsert the point into the collection
            var upsertResult = await _vectorDatabasePort.UpsertAsync(
                _options.CollectionName,
                new[] { point },
                cancellationToken);

            if (upsertResult.IsFailure)
            {
                _logger.LogError("Failed to upsert document: {Error}", upsertResult.Error);
                throw new InvalidOperationException($"Failed to store document: {upsertResult.Error}");
            }

            _logger.LogInformation("Document stored successfully with ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing document with ID: {Id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteDocumentAsync(
        string id, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        try
        {
            _logger.LogInformation("Deleting document with ID: {Id}", id);

            // Ensure collection exists
            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                _logger.LogError("Failed to ensure collection exists: {Error}", ensureResult.Error);
                throw new InvalidOperationException($"Failed to ensure collection exists: {ensureResult.Error}");
            }

            // Convert document ID to ulong (use hash if not numeric)
            if (!ulong.TryParse(id, out var pointId))
            {
                pointId = (ulong)id.GetHashCode();
            }

            // Delete the point from the collection
            var deleteResult = await _vectorDatabasePort.DeleteAsync(
                _options.CollectionName,
                pointIds: new[] { pointId },
                filter: null,
                cancellationToken);

            if (deleteResult.IsFailure)
            {
                _logger.LogError("Failed to delete document: {Error}", deleteResult.Error);
                throw new InvalidOperationException($"Failed to delete document: {deleteResult.Error}");
            }

            _logger.LogInformation("Document deleted successfully with ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document with ID: {Id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateDocumentAsync(
        string id, 
        string content, 
        Dictionary<string, object> metadata, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be null or empty", nameof(content));
        
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        try
        {
            _logger.LogInformation("Updating document with ID: {Id}", id);

            // Ensure collection exists
            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                _logger.LogError("Failed to ensure collection exists: {Error}", ensureResult.Error);
                throw new InvalidOperationException($"Failed to ensure collection exists: {ensureResult.Error}");
            }

            // Generate new embedding for updated content
            var embedding = await GenerateEmbeddingAsync(content, cancellationToken);

            // Convert document ID to ulong (use hash if not numeric)
            if (!ulong.TryParse(id, out var pointId))
            {
                pointId = (ulong)id.GetHashCode();
            }

            // Create payload with updated metadata and content
            var payload = new Dictionary<string, object>(metadata);
            payload["content"] = content;
            payload["id"] = id;

            // Create QdrantPoint with same ID but new vector and metadata
            var point = new QdrantPoint(
                Id: pointId,
                Vector: embedding.Values.ToArray(),
                Payload: payload);

            // Upsert the point (upsert updates existing points with same ID)
            var upsertResult = await _vectorDatabasePort.UpsertAsync(
                _options.CollectionName,
                new[] { point },
                cancellationToken);

            if (upsertResult.IsFailure)
            {
                _logger.LogError("Failed to upsert document: {Error}", upsertResult.Error);
                throw new InvalidOperationException($"Failed to update document: {upsertResult.Error}");
            }

            _logger.LogInformation("Document updated successfully with ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document with ID: {Id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Result<VectorSearchResult>> SearchAsync(float[] queryVector, VectorSearchOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching with vector of dimension: {Dimension}", queryVector.Length);

            await EnsureCollectionExistsAsync(cancellationToken);

            // Build filter from options
            Filter? filter = null;
            if (options.Filters != null && options.Filters.Count > 0)
            {
                var conditions = new List<Condition>();
                foreach (var kvp in options.Filters)
                {
                    Condition condition;
                    if (kvp.Value is bool boolValue)
                    {
                        condition = Match(kvp.Key, boolValue);
                    }
                    else if (kvp.Value is string stringValue)
                    {
                        condition = MatchKeyword(kvp.Key, stringValue);
                    }
                    else if (kvp.Value is int || kvp.Value is long)
                    {
                        condition = new Condition
                        {
                            Field = new FieldCondition
                            {
                                Key = kvp.Key,
                                Match = new Qdrant.Client.Grpc.Match { Integer = Convert.ToInt64(kvp.Value) }
                            }
                        };
                    }
                    else if (kvp.Value is float || kvp.Value is double)
                    {
                        condition = new Condition
                        {
                            Field = new FieldCondition
                            {
                                Key = kvp.Key,
                                Match = new Qdrant.Client.Grpc.Match { Integer = Convert.ToInt64(kvp.Value) }
                            }
                        };
                    }
                    else
                    {
                        // Fallback to keyword match with string representation
                        condition = MatchKeyword(kvp.Key, kvp.Value?.ToString() ?? string.Empty);
                    }
                    conditions.Add(condition);
                }
                if (conditions.Count == 1)
                {
                    filter = new Filter { Must = { conditions[0] } };
                }
                else if (conditions.Count > 1)
                {
                    filter = new Filter { Must = { conditions } };
                }
            }

            // Convert Qdrant filter to domain filter dictionary if needed
            Dictionary<string, object>? domainFilter = null;
            if (filter != null)
            {
                // Note: Filter conversion would be complex, for now pass null and rely on scoreThreshold
                // This is a limitation - ideally we'd convert Qdrant Filter to domain Dictionary
                domainFilter = null;
            }

            var searchResult = await _vectorDatabasePort.SearchAsync(
                collectionName: _options.CollectionName,
                queryVector: queryVector,
                limit: (uint)options.Limit,
                scoreThreshold: options.Threshold > 0 ? options.Threshold : null,
                filter: domainFilter,
                cancellationToken: cancellationToken);

            if (searchResult.IsFailure)
            {
                _logger.LogError("Failed to search: {Error}", searchResult.Error);
                return Result<VectorSearchResult>.WithFailure(searchResult.Error!, default);
            }

            var results = new List<VectorSearchResult>();
            int rank = 1;
            foreach (var hit in searchResult.Value ?? Array.Empty<VectorSearchHit>())
            {
                var score = hit.Score;
                if (score < options.Threshold)
                    continue;

                // Extract metadata from payload
                var metadata = new Dictionary<string, object>();
                if (options.IncludeMetadata && hit.Payload != null)
                {
                    foreach (var payloadKvp in hit.Payload)
                    {
                        metadata[payloadKvp.Key] = payloadKvp.Value;
                    }
                }

                // Extract content from payload or use default
                var content = metadata.GetValueOrDefault("content")?.ToString() ?? string.Empty;

                var embedding = new VectorEmbedding(
                    Id: hit.PointId.ToString(),
                    Content: content,
                    Embedding: options.IncludeEmbedding && hit.Vector != null 
                        ? hit.Vector
                        : Array.Empty<float>(),
                    Metadata: metadata,
                    CreatedAt: DateTimeOffset.UtcNow
                );

                results.Add(new VectorSearchResult(embedding, score, rank++));
            }

            // Return the first result or empty result
            var firstResult = results.Count > 0 ? results[0] : new VectorSearchResult(
                new VectorEmbedding(
                    Id: string.Empty,
                    Content: string.Empty,
                    Embedding: Array.Empty<float>(),
                    Metadata: new Dictionary<string, object>(),
                    CreatedAt: DateTimeOffset.UtcNow
                ),
                0.0f,
                0
            );

            _logger.LogInformation("Search completed: {Count} results found", results.Count);
            return Result<VectorSearchResult>.Success(firstResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search with vector");
            return Result<VectorSearchResult>.WithFailure($"Failed to search: {ex.Message}", default);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<VectorSearchResult>>> SearchBatchAsync(IReadOnlyList<float[]> queryVectors, VectorSearchOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Batch searching with {Count} vectors", queryVectors.Count);

            if (queryVectors.Count == 0)
            {
                return Result<IReadOnlyList<VectorSearchResult>>.Success(Array.Empty<VectorSearchResult>());
            }

            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                _logger.LogError("Failed to ensure collection exists: {Error}", ensureResult.Error);
                return Result<IReadOnlyList<VectorSearchResult>>.WithFailure(
                    ensureResult.Error ?? "Failed to ensure collection exists",
                    Array.Empty<VectorSearchResult>());
            }

            // Build filter from options
            Filter? filter = null;
            Dictionary<string, object>? domainFilter = null;
            if (options.Filters != null && options.Filters.Count > 0)
            {
                var conditions = new List<Condition>();
                foreach (var kvp in options.Filters)
                {
                    Condition condition;
                    if (kvp.Value is bool boolValue)
                    {
                        condition = Match(kvp.Key, boolValue);
                    }
                    else if (kvp.Value is string stringValue)
                    {
                        condition = MatchKeyword(kvp.Key, stringValue);
                    }
                    else if (kvp.Value is int || kvp.Value is long)
                    {
                        condition = new Condition
                        {
                            Field = new FieldCondition
                            {
                                Key = kvp.Key,
                                Match = new Qdrant.Client.Grpc.Match { Integer = Convert.ToInt64(kvp.Value) }
                            }
                        };
                    }
                    else if (kvp.Value is float || kvp.Value is double)
                    {
                        condition = new Condition
                        {
                            Field = new FieldCondition
                            {
                                Key = kvp.Key,
                                Match = new Qdrant.Client.Grpc.Match { Integer = Convert.ToInt64(kvp.Value) }
                            }
                        };
                    }
                    else
                    {
                        // Fallback to keyword match with string representation
                        condition = MatchKeyword(kvp.Key, kvp.Value?.ToString() ?? string.Empty);
                    }
                    conditions.Add(condition);
                }
                if (conditions.Count == 1)
                {
                    filter = new Filter { Must = { conditions[0] } };
                }
                else if (conditions.Count > 1)
                {
                    filter = new Filter { Must = { conditions } };
                }
                // Convert Qdrant filter conditions to domain filter dictionary
                // Note: This is a simplified conversion - complex filters may not map perfectly
                domainFilter = [];
                foreach (var kvp in options.Filters ?? new Dictionary<string, object>())
                {
                    domainFilter[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                domainFilter = null;
            }

            // Execute searches in parallel
            var searchTasks = queryVectors.Select(async queryVector =>
            {
                var searchResult = await _vectorDatabasePort.SearchAsync(
                    collectionName: _options.CollectionName,
                    queryVector: queryVector,
                    limit: (uint)options.Limit,
                    scoreThreshold: options.Threshold > 0 ? options.Threshold : null,
                    filter: domainFilter,
                    cancellationToken: cancellationToken);

                if (searchResult.IsFailure)
                {
                    _logger.LogWarning("Failed to search with vector: {Error}", searchResult.Error);
                    return [];
                }

                var results = new List<VectorSearchResult>();
                int rank = 1;
                foreach (var hit in searchResult.Value ?? Array.Empty<VectorSearchHit>())
                {
                    var score = hit.Score;
                    if (score < options.Threshold)
                        continue;

                    var metadata = new Dictionary<string, object>();
                    if (options.IncludeMetadata && hit.Payload != null)
                    {
                        foreach (var payloadKvp in hit.Payload)
                        {
                            metadata[payloadKvp.Key] = payloadKvp.Value;
                        }
                    }

                    var content = metadata.GetValueOrDefault("content")?.ToString() ?? string.Empty;

                    var embedding = new VectorEmbedding(
                        Id: hit.PointId.ToString(),
                        Content: content,
                        Embedding: options.IncludeEmbedding && hit.Vector != null 
                            ? hit.Vector
                            : Array.Empty<float>(),
                        Metadata: metadata,
                        CreatedAt: DateTimeOffset.UtcNow
                    );

                    results.Add(new VectorSearchResult(embedding, score, rank++));
                }

                return results;
            });

            var allResults = await Task.WhenAll(searchTasks);
            var flatResults = allResults.SelectMany(r => r).ToList();

            _logger.LogInformation("Batch search completed: {Count} total results", flatResults.Count);
            return Result<IReadOnlyList<VectorSearchResult>>.Success(flatResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to batch search with {Count} vectors", queryVectors.Count);
            return Result<IReadOnlyList<VectorSearchResult>>.WithFailure($"Failed to batch search: {ex.Message}", Array.Empty<VectorSearchResult>());
        }
    }

    /// <inheritdoc />
    public async Task<Result> IndexAsync(VectorEmbedding embedding, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Indexing embedding: {EmbeddingId}", embedding.Id);

            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                return ensureResult;
            }

            // Parse ID - Qdrant supports uint64 IDs, so we'll use a hash or numeric ID
            if (!ulong.TryParse(embedding.Id, out var pointId))
            {
                // Use hash of ID if not numeric
                pointId = (ulong)embedding.Id.GetHashCode();
            }

            // Convert to domain QdrantPoint
            var payload = new Dictionary<string, object>();
            if (embedding.Metadata != null)
            {
                foreach (var kvp in embedding.Metadata)
                {
                    payload[kvp.Key] = kvp.Value;
                }
            }
            // Always include content in payload
            payload["content"] = embedding.Content;

            var point = new QdrantPoint(
                Id: pointId,
                Vector: embedding.Embedding.ToArray(),
                Payload: payload.Count > 0 ? payload : null);

            var upsertResult = await _vectorDatabasePort.UpsertAsync(_options.CollectionName, new[] { point }, cancellationToken);
            if (upsertResult.IsFailure)
            {
                _logger.LogError("Failed to upsert embedding: {Error}", upsertResult.Error);
                return upsertResult;
            }

            _logger.LogInformation("Successfully indexed embedding: {EmbeddingId}", embedding.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to index embedding: {EmbeddingId}", embedding.Id);
            return Result.WithFailure($"Failed to index embedding: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> IndexBatchAsync(IReadOnlyList<VectorEmbedding> embeddings, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Batch indexing {Count} embeddings", embeddings.Count);

            if (embeddings.Count == 0)
            {
                return Result.Success();
            }

            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                return ensureResult;
            }

            var points = new List<QdrantPoint>();
            foreach (var embedding in embeddings)
            {
                if (!ulong.TryParse(embedding.Id, out var pointId))
                {
                    pointId = (ulong)embedding.Id.GetHashCode();
                }

                // Convert to domain QdrantPoint
                var payload = new Dictionary<string, object>();
                if (embedding.Metadata != null)
                {
                    foreach (var kvp in embedding.Metadata)
                    {
                        payload[kvp.Key] = kvp.Value;
                    }
                }
                payload["content"] = embedding.Content;

                var point = new QdrantPoint(
                    Id: pointId,
                    Vector: embedding.Embedding.ToArray(),
                    Payload: payload.Count > 0 ? payload : null);
                points.Add(point);
            }

            var upsertResult = await _vectorDatabasePort.UpsertAsync(_options.CollectionName, points.AsReadOnly(), cancellationToken);
            if (upsertResult.IsFailure)
            {
                _logger.LogError("Failed to batch upsert embeddings: {Error}", upsertResult.Error);
                return upsertResult;
            }

            _logger.LogInformation("Successfully batch indexed {Count} embeddings", embeddings.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to batch index {Count} embeddings", embeddings.Count);
            return Result.WithFailure($"Failed to batch index embeddings: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateAsync(VectorEmbedding embedding, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating embedding: {EmbeddingId}", embedding.Id);

            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                return ensureResult;
            }

            if (!ulong.TryParse(embedding.Id, out var pointId))
            {
                pointId = (ulong)embedding.Id.GetHashCode();
            }

            // Use UpsertAsync to update both vector and payload - convert to domain QdrantPoint
            var payload = new Dictionary<string, object>();
            if (embedding.Metadata != null)
            {
                foreach (var kvp in embedding.Metadata)
                {
                    payload[kvp.Key] = kvp.Value;
                }
            }
            payload["content"] = embedding.Content;

            var point = new QdrantPoint(
                Id: pointId,
                Vector: embedding.Embedding.ToArray(),
                Payload: payload.Count > 0 ? payload : null);

            var upsertResult = await _vectorDatabasePort.UpsertAsync(_options.CollectionName, new[] { point }, cancellationToken);
            if (upsertResult.IsFailure)
            {
                _logger.LogError("Failed to upsert embedding: {Error}", upsertResult.Error);
                return upsertResult;
            }

            _logger.LogInformation("Successfully updated embedding: {EmbeddingId}", embedding.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update embedding: {EmbeddingId}", embedding.Id);
            return Result.WithFailure($"Failed to update embedding: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(string embeddingId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting embedding: {EmbeddingId}", embeddingId);

            var ensureResult = await EnsureCollectionExistsAsync(cancellationToken);
            if (ensureResult.IsFailure)
            {
                return ensureResult;
            }

            if (!ulong.TryParse(embeddingId, out var pointId))
            {
                pointId = (ulong)embeddingId.GetHashCode();
            }

            var deleteResult = await _vectorDatabasePort.DeleteAsync(_options.CollectionName, pointIds: new[] { pointId }, filter: null, cancellationToken);
            if (deleteResult.IsFailure)
            {
                _logger.LogError("Failed to delete embedding: {Error}", deleteResult.Error);
                return deleteResult;
            }

            _logger.LogInformation("Successfully deleted embedding: {EmbeddingId}", embeddingId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete embedding: {EmbeddingId}", embeddingId);
            return Result.WithFailure($"Failed to delete embedding: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<VectorIndexStatistics>> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting vector index statistics");

            var collectionInfoResult = await _vectorDatabasePort.GetCollectionInfoAsync(_options.CollectionName, cancellationToken);
            if (collectionInfoResult.IsFailure)
            {
                _logger.LogError("Failed to get collection info: {Error}", collectionInfoResult.Error);
                return Result<VectorIndexStatistics>.WithFailure(collectionInfoResult.Error!, default);
            }

            var collectionInfo = collectionInfoResult.Value;
            if (collectionInfo == null)
            {
                _logger.LogWarning("Collection not found: {CollectionName}", _options.CollectionName);
                var emptyStats = new VectorIndexStatistics(0, 0, DateTimeOffset.UtcNow, 0);
                return Result<VectorIndexStatistics>.Success(emptyStats);
            }

            var totalVectors = collectionInfo.PointsCount;
            var indexSize = totalVectors; // CollectionInfo doesn't have IndexedVectorsCount, use PointsCount as approximation
            var averageDimension = (int)collectionInfo.VectorSize;

            var stats = new VectorIndexStatistics(
                TotalVectors: totalVectors,
                IndexSize: indexSize,
                LastUpdated: DateTimeOffset.UtcNow,
                AverageVectorDimension: averageDimension
            );

            _logger.LogInformation("Statistics: {TotalVectors} vectors, size: {IndexSize}", totalVectors, indexSize);
            return Result<VectorIndexStatistics>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get statistics");
            return Result<VectorIndexStatistics>.WithFailure($"Failed to get statistics: {ex.Message}", default);
        }
    }

    /// <inheritdoc />
    public async Task<Result> ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("Clearing vector index - this will delete all vectors in collection: {CollectionName}", _options.CollectionName);

            var collectionInfoResult = await _vectorDatabasePort.GetCollectionInfoAsync(_options.CollectionName, cancellationToken);
            if (collectionInfoResult.IsFailure)
            {
                _logger.LogError("Failed to get collection info: {Error}", collectionInfoResult.Error);
                return Result.WithFailure(collectionInfoResult.Error ?? ErrorCodes.VectorDatabaseError);
            }

            if (collectionInfoResult.Value == null)
            {
                _logger.LogInformation("Collection does not exist, nothing to clear");
                return Result.Success();
            }

            // Delete all points using an empty filter dictionary (note: port interface expects Dictionary, not Qdrant Filter)
            // Since we can't delete all with empty filter easily, we'll pass null filter and pointIds
            // This is a limitation - ideally the port interface would support deleting all points
            var deleteResult = await _vectorDatabasePort.DeleteAsync(_options.CollectionName, pointIds: null, filter: [], cancellationToken);
            if (deleteResult.IsFailure)
            {
                _logger.LogError("Failed to delete all points: {Error}", deleteResult.Error);
                return deleteResult;
            }

            _logger.LogInformation("Successfully cleared vector index");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear vector index");
            return Result.WithFailure($"Failed to clear vector index: {ex.Message}");
        }
    }

}
