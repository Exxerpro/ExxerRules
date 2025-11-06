using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Implementation of knowledge graph service using Neo4j.
/// </summary>
public class Neo4jKnowledgeGraphService : IKnowledgeGraphServicePort
{
    private readonly IGraphDatabasePort _graphDatabasePort;
    private readonly Neo4jOptions _options;
    private readonly ILogger<Neo4jKnowledgeGraphService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jKnowledgeGraphService"/> class.
    /// </summary>
    /// <param name="graphDatabasePort">The graph database port instance.</param>
    /// <param name="options">The Neo4j configuration options.</param>
    /// <param name="logger">Logger instance.</param>
    public Neo4jKnowledgeGraphService(
        IGraphDatabasePort graphDatabasePort,
        IOptions<Neo4jOptions> options,
        ILogger<Neo4jKnowledgeGraphService> logger)
    {
        _graphDatabasePort = graphDatabasePort ?? throw new ArgumentNullException(nameof(graphDatabasePort));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<GraphQueryResult> QueryAsync(
        GraphQuery query, 
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation("Executing graph query: {Cypher}", query.Query);
        
        try
        {
            // Handle timeout from query.TimeoutMs
            using var timeoutCts = query.TimeoutMs > 0 
                ? new CancellationTokenSource(TimeSpan.FromMilliseconds(query.TimeoutMs))
                : null;
            
            var linkedCts = timeoutCts != null
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token)
                : null;
            
            var effectiveToken = linkedCts?.Token ?? cancellationToken;
            
            // Execute the query using the graph database port
            var queryResult = await _graphDatabasePort.ExecuteReadAsync(
                query.Query,
                query.Parameters != null ? new Dictionary<string, object>(query.Parameters) : null,
                _options.Database,
                effectiveToken);
            
            stopwatch.Stop();
            
            // Handle query failure
            if (queryResult.IsFailure)
            {
                _logger.LogError("Graph query failed: {Error}", queryResult.Error);
                return new GraphQueryResult(
                    Records: [],
                    ExecutionTimeMs: stopwatch.ElapsedMilliseconds,
                    RecordsAffected: 0,
                    Success: false,
                    ErrorMessage: queryResult.Error ?? "Query execution failed");
            }
            
            // Map CypherRecord to GraphRecord
            var records = new List<GraphRecord>();
            if (queryResult.Value != null)
            {
                foreach (var cypherRecord in queryResult.Value)
                {
                    // Convert Dictionary<string, object> Values to IReadOnlyList<object>
                    // Order by Keys to maintain consistency
                    var values = new List<object>();
                    foreach (var key in cypherRecord.Keys)
                    {
                        values.Add(cypherRecord.Values.GetValueOrDefault(key) ?? (object)string.Empty);
                    }
                    
                    records.Add(new GraphRecord(
                        Values: values,
                        Keys: cypherRecord.Keys));
                }
            }
            
            _logger.LogInformation(
                "Graph query executed successfully in {ExecutionTimeMs}ms, {RecordsAffected} records returned",
                stopwatch.ElapsedMilliseconds,
                records.Count);
            
            return new GraphQueryResult(
                Records: records,
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds,
                RecordsAffected: records.Count,
                Success: true,
                ErrorMessage: null);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogWarning("Graph query was cancelled or timed out");
            return new GraphQueryResult(
                Records: [],
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds,
                RecordsAffected: 0,
                Success: false,
                ErrorMessage: ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error executing graph query: {Query}", query.Query);
            return new GraphQueryResult(
                Records: [],
                ExecutionTimeMs: stopwatch.ElapsedMilliseconds,
                RecordsAffected: 0,
                Success: false,
                ErrorMessage: ex.Message);
        }
    }

    // Note: AddNodeAsync, UpdateNodeAsync, DeleteNodeAsync, CreateRelationshipAsync, DeleteRelationshipAsync, AddCodeNodeAsync
    // are legacy methods not part of IKnowledgeGraphServicePort. They remain for backward compatibility.
    // TODO: 2025-01-27 - [IMPLEMENT] Determine if these methods should be removed or moved to a different interface
    
    /// <inheritdoc />
    public async Task<GraphNode> AddNodeAsync(
        GraphNode node, 
        CancellationToken cancellationToken = default)
    {
        if (node.Equals(default(GraphNode)))
            throw new ArgumentException("Node cannot be default", nameof(node));
        
        if (string.IsNullOrWhiteSpace(node.Id))
            throw new ArgumentException("Node ID cannot be null or empty", nameof(node));
        
        // TODO: 2025-01-27 - [IMPLEMENT] Fix GraphNode type - check if it should use Labels (plural) instead of Label
        // Temporarily commented out to fix compilation
        // if (string.IsNullOrWhiteSpace(node.Label))
        //     throw new ArgumentException("Node label cannot be null or empty", nameof(node));

        _logger.LogInformation("Adding node: {NodeId}", node.Id);
        
        // TODO: 2025-01-27 - [IMPLEMENT] Implement Neo4j node creation logic using Neo4j driver
        await Task.Delay(100, cancellationToken); // Placeholder
        
        return node;
    }

    /// <inheritdoc />
    public async Task<GraphNode> UpdateNodeAsync(
        string nodeId, 
        GraphNode node, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            throw new ArgumentException("Node ID cannot be null or empty", nameof(nodeId));
        
        if (node.Equals(default(GraphNode)))
            throw new ArgumentException("Node cannot be default", nameof(node));

        _logger.LogInformation("Updating node: {NodeId}", nodeId);
        
        // TODO: 2025-01-27 - [IMPLEMENT] Implement Neo4j node update logic using Cypher MATCH and SET
        await Task.Delay(100, cancellationToken); // Placeholder
        
        return node;
    }

    /// <inheritdoc />
    public async Task DeleteNodeAsync(
        string nodeId, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            throw new ArgumentException("Node ID cannot be null or empty", nameof(nodeId));

        _logger.LogInformation("Deleting node: {NodeId}", nodeId);
        
        // TODO: 2025-01-27 - [IMPLEMENT] Implement Neo4j node deletion logic using Cypher DETACH DELETE
        await Task.Delay(100, cancellationToken); // Placeholder
    }

    /// <inheritdoc />
    public async Task<GraphRelationship> CreateRelationshipAsync(
        GraphRelationship relationship, 
        CancellationToken cancellationToken = default)
    {
        if (relationship.Equals(default(GraphRelationship)))
            throw new ArgumentException("Relationship cannot be default", nameof(relationship));
        
        if (string.IsNullOrWhiteSpace(relationship.Id))
            throw new ArgumentException("Relationship ID cannot be null or empty", nameof(relationship));
        
        if (string.IsNullOrWhiteSpace(relationship.StartNodeId))
            throw new ArgumentException("Start node ID cannot be null or empty", nameof(relationship));
        
        if (string.IsNullOrWhiteSpace(relationship.EndNodeId))
            throw new ArgumentException("End node ID cannot be null or empty", nameof(relationship));
        
        if (relationship.StartNodeId == relationship.EndNodeId)
            throw new ArgumentException("Self-referencing relationships are not allowed", nameof(relationship));

        _logger.LogInformation("Creating relationship: {RelationshipId}", relationship.Id);
        
        // TODO: 2025-01-27 - [IMPLEMENT] Implement Neo4j relationship creation logic using Cypher MATCH and CREATE
        await Task.Delay(100, cancellationToken); // Placeholder
        
        return relationship;
    }

    /// <inheritdoc />
    public async Task<Result> DeleteRelationshipAsync(
        string relationshipId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(relationshipId))
                return Result.WithFailure($"Relationship ID cannot be null or empty: {nameof(relationshipId)}");

            _logger.LogInformation("Deleting relationship: {RelationshipId}", relationshipId);

            var cypher = @"
                MATCH ()-[r {id: $id}]-()
                DELETE r";

            var parameters = new Dictionary<string, object> { ["id"] = relationshipId };
            var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, parameters, _options.Database, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogError("Failed to delete relationship: {Error}", result.Error);
                return result;
            }
            
            _logger.LogInformation("Successfully deleted relationship: {RelationshipId}", relationshipId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete relationship: {RelationshipId}", relationshipId);
            return Result.WithFailure($"Failed to delete relationship: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<GraphQueryResult> GetContextAsync(
        string query, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be null or empty", nameof(query));

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation("Getting context for query: {Query}", query);
        
        // TODO: 2025-01-27 - [IMPLEMENT] Implement Neo4j context retrieval logic using Cypher queries based on query string
        await Task.Delay(100, cancellationToken); // Placeholder
        
        stopwatch.Stop();
        
        return new GraphQueryResult
        {
            Records = [],
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
            RecordsAffected = 0,
            Success = true
        };
    }

    /// <inheritdoc />
    public async Task<CodeNode> AddCodeNodeAsync(
        CodeNode codeNode, 
        CancellationToken cancellationToken = default)
    {
        if (codeNode.Equals(default(CodeNode)))
            throw new ArgumentException("Code node cannot be default", nameof(codeNode));
        
        if (string.IsNullOrWhiteSpace(codeNode.Id))
            throw new ArgumentException("Code node ID cannot be null or empty", nameof(codeNode));

        _logger.LogInformation("Adding code node: {NodeId}", codeNode.Id);
        
        // TODO: 2025-01-27 - [IMPLEMENT] Implement Neo4j code node creation logic using Neo4j driver
        await Task.Delay(100, cancellationToken); // Placeholder
        
        return codeNode;
    }

    /// <inheritdoc />
    public async Task<Result> CreateEntityAsync(KnowledgeEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating entity: {EntityId} of type: {EntityType}", entity.Id, entity.Type);

            var cypher = @"
                MERGE (n:KnowledgeEntity {id: $id})
                SET n.name = $name,
                    n.type = $type,
                    n.description = $description,
                    n.properties = $properties,
                    n.confidence = $confidence,
                    n.createdAt = $createdAt
                RETURN n";

            var parameters = new Dictionary<string, object>
            {
                ["id"] = entity.Id,
                ["name"] = entity.Name,
                ["type"] = entity.Type,
                ["description"] = entity.Description ?? string.Empty,
                ["properties"] = entity.Properties ?? [],
                ["confidence"] = entity.Confidence,
                ["createdAt"] = entity.CreatedAt
            };

            var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to create entity: {Error}", result.Error);
                return result;
            }
            
            _logger.LogInformation("Successfully created entity: {EntityId}", entity.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create entity: {EntityId}", entity.Id);
            return Result.WithFailure($"Failed to create entity: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> CreateEntitiesAsync(IReadOnlyList<KnowledgeEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating {Count} entities", entities.Count);

            if (entities.Count == 0)
            {
                return Result.Success();
            }

            var cypher = @"
                UNWIND $entities AS entity
                MERGE (n:KnowledgeEntity {id: entity.id})
                SET n.name = entity.name,
                    n.type = entity.type,
                    n.description = entity.description,
                    n.properties = entity.properties,
                    n.confidence = entity.confidence,
                    n.createdAt = entity.createdAt";

            var parameters = new Dictionary<string, object>
            {
                ["entities"] = entities.Select(e => new Dictionary<string, object>
                {
                    ["id"] = e.Id,
                    ["name"] = e.Name,
                    ["type"] = e.Type,
                    ["description"] = e.Description ?? string.Empty,
                    ["properties"] = e.Properties ?? [],
                    ["confidence"] = e.Confidence,
                    ["createdAt"] = e.CreatedAt
                }).ToList()
            };

            var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to create entities: {Error}", result.Error);
                return result;
            }
            
            _logger.LogInformation("Successfully created {Count} entities", entities.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create {Count} entities", entities.Count);
            return Result.WithFailure($"Failed to create entities: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> CreateRelationshipAsync(KnowledgeRelationship relationship, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating relationship: {RelationshipId} from {FromNodeId} to {ToNodeId}", 
                relationship.Id, relationship.FromNodeId, relationship.ToNodeId);

            var validation = relationship.Validate();
            if (validation.IsFailure)
            {
                _logger.LogWarning("Relationship validation failed: {Error}", validation.Error);
                return validation;
            }

            var cypher = @"
                MATCH (from:KnowledgeEntity {id: $fromNodeId})
                MATCH (to:KnowledgeEntity {id: $toNodeId})
                MERGE (from)-[r:" + relationship.RelationshipType + @" {id: $id}]->(to)
                SET r.properties = $properties,
                    r.createdAt = $createdAt
                RETURN r";

            var parameters = new Dictionary<string, object>
            {
                ["id"] = relationship.Id,
                ["fromNodeId"] = relationship.FromNodeId,
                ["toNodeId"] = relationship.ToNodeId,
                ["properties"] = relationship.Properties ?? new Dictionary<string, object>(),
                ["createdAt"] = relationship.CreatedAt
            };

            var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to create relationship: {Error}", result.Error);
                return result;
            }
            
            _logger.LogInformation("Successfully created relationship: {RelationshipId}", relationship.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create relationship: {RelationshipId}", relationship.Id);
            return Result.WithFailure($"Failed to create relationship: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> CreateRelationshipsAsync(IReadOnlyList<KnowledgeRelationship> relationships, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating {Count} relationships", relationships.Count);

            if (relationships.Count == 0)
            {
                return Result.Success();
            }

            // Validate all relationships first
            foreach (var relationship in relationships)
            {
                var validation = relationship.Validate();
                if (validation.IsFailure)
                {
                    _logger.LogWarning("Relationship validation failed for {RelationshipId}: {Error}", 
                        relationship.Id, validation.Error);
                    return validation;
                }
            }

            // Note: Neo4j doesn't support parameterized relationship types directly
            // Group by relationship type for batch efficiency
            var groupedByType = relationships.GroupBy(r => r.RelationshipType);
            
            foreach (var group in groupedByType)
            {
                var relationshipType = group.Key;
                // Validate relationship type contains only safe characters
                if (!System.Text.RegularExpressions.Regex.IsMatch(relationshipType, @"^[A-Z_][A-Z0-9_]*$"))
                {
                    _logger.LogWarning("Invalid relationship type format: {RelationshipType}", relationshipType);
                    return Result.WithFailure($"Invalid relationship type format: {relationshipType}");
                }

                var cypher = $@"
                    UNWIND $relationships AS rel
                    MATCH (from:KnowledgeEntity {{id: rel.fromNodeId}})
                    MATCH (to:KnowledgeEntity {{id: rel.toNodeId}})
                    MERGE (from)-[r:{relationshipType} {{id: rel.id}}]->(to)
                    SET r.properties = rel.properties,
                        r.createdAt = rel.createdAt";

                var parameters = new Dictionary<string, object>
                {
                    ["relationships"] = group.Select(r => new Dictionary<string, object>
                    {
                        ["id"] = r.Id,
                        ["fromNodeId"] = r.FromNodeId,
                        ["toNodeId"] = r.ToNodeId,
                        ["properties"] = r.Properties ?? new Dictionary<string, object>(),
                        ["createdAt"] = r.CreatedAt
                    }).ToList()
                };

                var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, parameters, _options.Database, cancellationToken);
                if (result.IsFailure)
                {
                    _logger.LogError("Failed to create relationships: {Error}", result.Error);
                    return result;
                }
            }
            
            _logger.LogInformation("Successfully created {Count} relationships", relationships.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create {Count} relationships", relationships.Count);
            return Result.WithFailure($"Failed to create relationships: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<KnowledgeEntity>> GetEntityAsync(string entityId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting entity: {EntityId}", entityId);

            var cypher = @"
                MATCH (n:KnowledgeEntity {id: $id})
                RETURN n.id AS id, n.name AS name, n.type AS type, n.description AS description,
                       n.properties AS properties, n.confidence AS confidence, n.createdAt AS createdAt";

            var parameters = new Dictionary<string, object> { ["id"] = entityId };
            var result = await _graphDatabasePort.ExecuteReadSingleAsync(cypher, parameters, _options.Database, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get entity: {Error}", result.Error);
                return Result<KnowledgeEntity>.WithFailure(result.Error!, default);
            }

            var cypherRecord = result.Value;
            if (cypherRecord == null)
            {
                _logger.LogWarning("Entity not found: {EntityId}", entityId);
                return Result<KnowledgeEntity>.WithFailure($"Entity not found: {entityId}", default);
            }

            var entity = new KnowledgeEntity(
                Id: cypherRecord.Values["id"].ToString() ?? string.Empty,
                Name: cypherRecord.Values["name"].ToString() ?? string.Empty,
                Type: cypherRecord.Values["type"].ToString() ?? string.Empty,
                Description: cypherRecord.Values["description"].ToString() ?? string.Empty,
                Properties: cypherRecord.Values["properties"] as Dictionary<string, object> ?? [],
                Confidence: Convert.ToDouble(cypherRecord.Values["confidence"]),
                CreatedAt: (DateTime)cypherRecord.Values["createdAt"]
            );

            _logger.LogInformation("Successfully retrieved entity: {EntityId}", entityId);
            return Result<KnowledgeEntity>.Success(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity: {EntityId}", entityId);
            return Result<KnowledgeEntity>.WithFailure($"Failed to get entity: {ex.Message}", default);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeEntity>>> GetEntitiesAsync(IReadOnlyList<string> entityIds, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting {Count} entities", entityIds.Count);

            if (entityIds.Count == 0)
            {
                return Result<IReadOnlyList<KnowledgeEntity>>.Success(Array.Empty<KnowledgeEntity>());
            }

            var cypher = @"
                MATCH (n:KnowledgeEntity)
                WHERE n.id IN $ids
                RETURN n.id AS id, n.name AS name, n.type AS type, n.description AS description,
                       n.properties AS properties, n.confidence AS confidence, n.createdAt AS createdAt";

            var parameters = new Dictionary<string, object> { ["ids"] = entityIds.ToList() };
            var result = await _graphDatabasePort.ExecuteReadAsync(cypher, parameters, _options.Database, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get entities: {Error}", result.Error);
                return Result<IReadOnlyList<KnowledgeEntity>>.WithFailure(result.Error!, Array.Empty<KnowledgeEntity>());
            }

            var entities = result.Value?.Select(cypherRecord => new KnowledgeEntity(
                Id: cypherRecord.Values["id"].ToString() ?? string.Empty,
                Name: cypherRecord.Values["name"].ToString() ?? string.Empty,
                Type: cypherRecord.Values["type"].ToString() ?? string.Empty,
                Description: cypherRecord.Values["description"].ToString() ?? string.Empty,
                Properties: cypherRecord.Values["properties"] as Dictionary<string, object> ?? [],
                Confidence: Convert.ToDouble(cypherRecord.Values["confidence"]),
                CreatedAt: (DateTime)cypherRecord.Values["createdAt"]
            )).ToList() ?? [];

            _logger.LogInformation("Successfully retrieved {Count} entities", entities.Count);
            return Result<IReadOnlyList<KnowledgeEntity>>.Success(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get {Count} entities", entityIds.Count);
            return Result<IReadOnlyList<KnowledgeEntity>>.WithFailure($"Failed to get entities: {ex.Message}", Array.Empty<KnowledgeEntity>());
        }
    }

    /// <inheritdoc />
    public async Task<Result<KnowledgeRelationship>> GetRelationshipAsync(string relationshipId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting relationship: {RelationshipId}", relationshipId);

            var cypher = @"
                MATCH ()-[r {id: $id}]-()
                RETURN r.id AS id, startNode(r).id AS fromNodeId, endNode(r).id AS toNodeId,
                       type(r) AS relationshipType, r.properties AS properties, r.createdAt AS createdAt";

            var parameters = new Dictionary<string, object> { ["id"] = relationshipId };
            var result = await _graphDatabasePort.ExecuteReadSingleAsync(cypher, parameters, _options.Database, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogError("Failed to get relationship: {Error}", result.Error);
                return Result<KnowledgeRelationship>.WithFailure(result.Error ?? "Failed to get relationship", default);
            }

            if (result.Value == null)
            {
                _logger.LogWarning("Relationship not found: {RelationshipId}", relationshipId);
                return Result<KnowledgeRelationship>.WithFailure($"Relationship not found: {relationshipId}", default);
            }

            var record = result.Value;
            var relationship = new KnowledgeRelationship(
                Id: record.Values["id"].As<string>(),
                FromNodeId: record.Values["fromNodeId"].As<string>(),
                ToNodeId: record.Values["toNodeId"].As<string>(),
                RelationshipType: record.Values["relationshipType"].As<string>(),
                Properties: record.Values["properties"] is IReadOnlyDictionary<string, object> props ? props : new Dictionary<string, object>(),
                CreatedAt: record.Values["createdAt"] is DateTimeOffset dt ? dt : DateTimeOffset.MinValue
            );

            _logger.LogInformation("Successfully retrieved relationship: {RelationshipId}", relationshipId);
            return Result<KnowledgeRelationship>.Success(relationship);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get relationship: {RelationshipId}", relationshipId);
            return Result<KnowledgeRelationship>.WithFailure($"Failed to get relationship: {ex.Message}", default);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeRelationship>>> GetRelationshipsAsync(IReadOnlyList<string> relationshipIds, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting {Count} relationships", relationshipIds.Count);

            if (relationshipIds.Count == 0)
            {
                return Result<IReadOnlyList<KnowledgeRelationship>>.Success(Array.Empty<KnowledgeRelationship>());
            }

            var cypher = @"
                MATCH ()-[r]-()
                WHERE r.id IN $ids
                RETURN r.id AS id, startNode(r).id AS fromNodeId, endNode(r).id AS toNodeId,
                       type(r) AS relationshipType, r.properties AS properties, r.createdAt AS createdAt";

            var parameters = new Dictionary<string, object> { ["ids"] = relationshipIds.ToList() };
            var result = await _graphDatabasePort.ExecuteReadAsync(cypher, parameters, _options.Database, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get relationships: {Error}", result.Error);
                return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure(result.Error!, Array.Empty<KnowledgeRelationship>());
            }

            var relationships = result.Value?.Select(cypherRecord => new KnowledgeRelationship(
                Id: cypherRecord.Values["id"].ToString() ?? string.Empty,
                FromNodeId: cypherRecord.Values["fromNodeId"].ToString() ?? string.Empty,
                ToNodeId: cypherRecord.Values["toNodeId"].ToString() ?? string.Empty,
                RelationshipType: cypherRecord.Values["relationshipType"].ToString() ?? string.Empty,
                Properties: cypherRecord.Values["properties"] as Dictionary<string, object> ?? [],
                CreatedAt: (DateTimeOffset)cypherRecord.Values["createdAt"]
            )).ToList() ?? [];

            _logger.LogInformation("Successfully retrieved {Count} relationships", relationships.Count);
            return Result<IReadOnlyList<KnowledgeRelationship>>.Success(relationships);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get {Count} relationships", relationshipIds.Count);
            return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure($"Failed to get relationships: {ex.Message}", Array.Empty<KnowledgeRelationship>());
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeEntity>>> SearchEntitiesAsync(string entityType, IReadOnlyDictionary<string, object>? properties = null, int limit = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching entities of type: {EntityType} with limit: {Limit}", entityType, limit);

            var whereConditions = new List<string> { "n.type = $entityType" };
            var parameters = new Dictionary<string, object> 
            { 
                ["entityType"] = entityType,
                ["limit"] = limit
            };

            // Add property filters if provided
            if (properties != null && properties.Count > 0)
            {
                var propIndex = 0;
                foreach (var prop in properties)
                {
                    var paramKeyName = $"propKey{propIndex}";
                    var paramValueName = $"propValue{propIndex}";
                    whereConditions.Add($"n.properties[$paramKeyName] = ${paramValueName}");
                    parameters[paramKeyName] = prop.Key;
                    parameters[paramValueName] = prop.Value;
                    propIndex++;
                }
            }

            var whereClause = string.Join(" AND ", whereConditions);
            var cypher = $@"
                MATCH (n:KnowledgeEntity)
                WHERE {whereClause}
                RETURN n.id AS id, n.name AS name, n.type AS type, n.description AS description,
                       n.properties AS properties, n.confidence AS confidence, n.createdAt AS createdAt
                LIMIT $limit";

            var result = await _graphDatabasePort.ExecuteReadAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to search entities: {Error}", result.Error);
                return Result<IReadOnlyList<KnowledgeEntity>>.WithFailure(result.Error ?? "Failed to search entities", Array.Empty<KnowledgeEntity>());
            }

            if (result.Value == null)
            {
                return Result<IReadOnlyList<KnowledgeEntity>>.WithFailure("No results returned", Array.Empty<KnowledgeEntity>());
            }

            var entities = result.Value.Select(record => new KnowledgeEntity(
                Id: record.Values["id"]?.ToString() ?? string.Empty,
                Name: record.Values["name"]?.ToString() ?? string.Empty,
                Type: record.Values["type"]?.ToString() ?? string.Empty,
                Description: record.Values.GetValueOrDefault("description")?.ToString() ?? string.Empty,
                Properties: record.Values.GetValueOrDefault("properties") is Dictionary<string, object> props ? props : [],
                Confidence: record.Values.GetValueOrDefault("confidence") is double conf ? (float)conf : 0.0f,
                CreatedAt: record.Values.GetValueOrDefault("createdAt") is DateTimeOffset dt ? dt.DateTime : DateTime.MinValue
            )).ToList();

            _logger.LogInformation("Found {Count} entities of type: {EntityType}", entities.Count, entityType);
            return Result<IReadOnlyList<KnowledgeEntity>>.Success(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search entities of type: {EntityType}", entityType);
            return Result<IReadOnlyList<KnowledgeEntity>>.WithFailure($"Failed to search entities: {ex.Message}", Array.Empty<KnowledgeEntity>());
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeRelationship>>> SearchRelationshipsAsync(string relationshipType, IReadOnlyDictionary<string, object>? properties = null, int limit = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching relationships of type: {RelationshipType} with limit: {Limit}", relationshipType, limit);

            var whereConditions = new List<string> { "type(r) = $relationshipType" };
            var parameters = new Dictionary<string, object> 
            { 
                ["relationshipType"] = relationshipType,
                ["limit"] = limit
            };

            // Add property filters if provided
            if (properties != null && properties.Count > 0)
            {
                var propIndex = 0;
                foreach (var prop in properties)
                {
                    var paramKeyName = $"propKey{propIndex}";
                    var paramValueName = $"propValue{propIndex}";
                    whereConditions.Add($"r.properties[$paramKeyName] = ${paramValueName}");
                    parameters[paramKeyName] = prop.Key;
                    parameters[paramValueName] = prop.Value;
                    propIndex++;
                }
            }

            var whereClause = string.Join(" AND ", whereConditions);
            var cypher = $@"
                MATCH ()-[r]-()
                WHERE {whereClause}
                RETURN r.id AS id, startNode(r).id AS fromNodeId, endNode(r).id AS toNodeId,
                       type(r) AS relationshipType, r.properties AS properties, r.createdAt AS createdAt
                LIMIT $limit";

            var result = await _graphDatabasePort.ExecuteReadAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to search relationships: {Error}", result.Error);
                return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure(result.Error ?? "Failed to search relationships", Array.Empty<KnowledgeRelationship>());
            }

            if (result.Value == null)
            {
                return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure("No results returned", Array.Empty<KnowledgeRelationship>());
            }

            var relationships = result.Value.Select(record => new KnowledgeRelationship(
                Id: record.Values["id"]?.ToString() ?? string.Empty,
                FromNodeId: record.Values["fromNodeId"]?.ToString() ?? string.Empty,
                ToNodeId: record.Values["toNodeId"]?.ToString() ?? string.Empty,
                RelationshipType: record.Values["relationshipType"]?.ToString() ?? string.Empty,
                Properties: record.Values.GetValueOrDefault("properties") is IReadOnlyDictionary<string, object> props ? props : new Dictionary<string, object>(),
                CreatedAt: record.Values.GetValueOrDefault("createdAt") is DateTimeOffset dt ? dt.DateTime : DateTime.MinValue
            )).ToList();

            _logger.LogInformation("Found {Count} relationships of type: {RelationshipType}", relationships.Count, relationshipType);
            return Result<IReadOnlyList<KnowledgeRelationship>>.Success(relationships);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search relationships of type: {RelationshipType}", relationshipType);
            return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure($"Failed to search relationships: {ex.Message}", Array.Empty<KnowledgeRelationship>());
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeEntity>>> FindConnectedEntitiesAsync(string entityId, IReadOnlyList<string>? relationshipTypes = null, int maxDepth = 2, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Finding connected entities for: {EntityId} with max depth: {MaxDepth}", entityId, maxDepth);

            var depthRange = maxDepth == 1 ? "" : $"*1..{maxDepth}";
            var relationshipFilter = relationshipTypes != null && relationshipTypes.Count > 0
                ? ":" + string.Join("|", relationshipTypes.Select(rt => $"`{rt}`"))
                : "";

            var cypher = $@"
                MATCH (start:KnowledgeEntity {{id: $entityId}})
                MATCH (start)-[r{relationshipFilter}{depthRange}]->(connected:KnowledgeEntity)
                RETURN DISTINCT connected.id AS id, connected.name AS name, connected.type AS type, 
                       connected.description AS description, connected.properties AS properties, 
                       connected.confidence AS confidence, connected.createdAt AS createdAt";

            var parameters = new Dictionary<string, object> { ["entityId"] = entityId };
            var result = await _graphDatabasePort.ExecuteReadAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to find connected entities: {Error}", result.Error);
                return Result<IReadOnlyList<KnowledgeEntity>>.WithFailure(result.Error ?? "Failed to find connected entities", Array.Empty<KnowledgeEntity>());
            }

            if (result.Value == null)
            {
                return Result<IReadOnlyList<KnowledgeEntity>>.WithFailure("No results returned", Array.Empty<KnowledgeEntity>());
            }

            var entities = result.Value.Select(record => new KnowledgeEntity(
                Id: record.Values["id"]?.ToString() ?? string.Empty,
                Name: record.Values["name"]?.ToString() ?? string.Empty,
                Type: record.Values["type"]?.ToString() ?? string.Empty,
                Description: record.Values.GetValueOrDefault("description")?.ToString() ?? string.Empty,
                Properties: record.Values.GetValueOrDefault("properties") is Dictionary<string, object> props ? props : [],
                Confidence: record.Values.GetValueOrDefault("confidence") is double conf ? (float)conf : 0.0f,
                CreatedAt: record.Values.GetValueOrDefault("createdAt") is DateTimeOffset dt ? dt.DateTime : DateTime.MinValue
            )).ToList();

            _logger.LogInformation("Found {Count} connected entities for: {EntityId}", entities.Count, entityId);
            return Result<IReadOnlyList<KnowledgeEntity>>.Success(entities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to find connected entities for: {EntityId}", entityId);
            return Result<IReadOnlyList<KnowledgeEntity>>.WithFailure($"Failed to find connected entities: {ex.Message}", Array.Empty<KnowledgeEntity>());
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<GraphPath>>> FindPathsAsync(string fromEntityId, string toEntityId, int maxPathLength = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Finding paths from {FromEntityId} to {ToEntityId} with max length: {MaxPathLength}", 
                fromEntityId, toEntityId, maxPathLength);

            var cypher = $@"
                MATCH (from:KnowledgeEntity {{id: $fromEntityId}})
                MATCH (to:KnowledgeEntity {{id: $toEntityId}})
                MATCH path = allShortestPaths((from)-[*1..{maxPathLength}]->(to))
                RETURN path";

            var parameters = new Dictionary<string, object> 
            { 
                ["fromEntityId"] = fromEntityId,
                ["toEntityId"] = toEntityId
            };

            var result = await _graphDatabasePort.ExecuteReadAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to find paths: {Error}", result.Error);
                return Result<IReadOnlyList<GraphPath>>.WithFailure(result.Error ?? "Failed to find paths", Array.Empty<GraphPath>());
            }

            var paths = new List<GraphPath>();
            foreach (var record in result.Value ?? Array.Empty<CypherRecord>())
            {
                var pathValue = record.Values.GetValueOrDefault("path");
                if (pathValue is not IPath path)
                {
                    _logger.LogWarning("Path value is not IPath type");
                    continue;
                }
                var nodes = path.Nodes.Select(n => new GraphNode(
                    Id: n.Properties["id"].As<string>(),
                    Type: n.Properties.ContainsKey("type") ? n.Properties["type"].As<string>() : "KnowledgeEntity",
                    Properties: n.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    Labels: n.Labels.ToList()
                )).ToList();

                var relationships = path.Relationships.Select(r => new GraphRelationship(
                    Id: r.Properties.ContainsKey("id") ? r.Properties["id"].As<string>() : r.ElementId,
                    Type: r.Type,
                    StartNodeId: r.StartNodeElementId,
                    EndNodeId: r.EndNodeElementId,
                    Properties: r.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                )).ToList();

                paths.Add(new GraphPath(
                    Nodes: nodes,
                    Relationships: relationships,
                    Length: relationships.Count
                ));
            }

            _logger.LogInformation("Found {Count} paths from {FromEntityId} to {ToEntityId}", paths.Count, fromEntityId, toEntityId);
            return Result<IReadOnlyList<GraphPath>>.Success(paths);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to find paths from {FromEntityId} to {ToEntityId}", fromEntityId, toEntityId);
            return Result<IReadOnlyList<GraphPath>>.WithFailure($"Failed to find paths: {ex.Message}", Array.Empty<GraphPath>());
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateEntityAsync(KnowledgeEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating entity: {EntityId}", entity.Id);

            var cypher = @"
                MATCH (n:KnowledgeEntity {id: $id})
                SET n.name = $name,
                    n.type = $type,
                    n.description = $description,
                    n.properties = $properties,
                    n.confidence = $confidence,
                    n.createdAt = $createdAt
                RETURN n";

            var parameters = new Dictionary<string, object>
            {
                ["id"] = entity.Id,
                ["name"] = entity.Name,
                ["type"] = entity.Type,
                ["description"] = entity.Description ?? string.Empty,
                ["properties"] = entity.Properties ?? [],
                ["confidence"] = entity.Confidence,
                ["createdAt"] = entity.CreatedAt
            };

            var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to update entity: {Error}", result.Error);
                return result;
            }
            
            _logger.LogInformation("Successfully updated entity: {EntityId}", entity.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity: {EntityId}", entity.Id);
            return Result.WithFailure($"Failed to update entity: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateRelationshipAsync(KnowledgeRelationship relationship, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating relationship: {RelationshipId}", relationship.Id);

            var cypher = @"
                MATCH ()-[r {id: $id}]-()
                SET r.properties = $properties,
                    r.createdAt = $createdAt
                RETURN r";

            var parameters = new Dictionary<string, object>
            {
                ["id"] = relationship.Id,
                ["properties"] = relationship.Properties ?? new Dictionary<string, object>(),
                ["createdAt"] = relationship.CreatedAt
            };

            var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to update relationship: {Error}", result.Error);
                return result;
            }
            
            _logger.LogInformation("Successfully updated relationship: {RelationshipId}", relationship.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update relationship: {RelationshipId}", relationship.Id);
            return Result.WithFailure($"Failed to update relationship: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteEntityAsync(string entityId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting entity: {EntityId}", entityId);

            var cypher = @"
                MATCH (n:KnowledgeEntity {id: $id})
                DETACH DELETE n";

            var parameters = new Dictionary<string, object> { ["id"] = entityId };
            var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, parameters, _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to delete entity: {Error}", result.Error);
                return result;
            }
            
            _logger.LogInformation("Successfully deleted entity: {EntityId}", entityId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete entity: {EntityId}", entityId);
            return Result.WithFailure($"Failed to delete entity: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<KnowledgeGraphStatistics>> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting graph statistics");

            // Get total nodes and relationships
            var countCypher = @"
                MATCH (n:KnowledgeEntity)
                WITH count(n) AS totalNodes
                MATCH ()-[r]-()
                WITH totalNodes, count(DISTINCT r) AS totalRelationships
                RETURN totalNodes, totalRelationships";

            var countResult = await _graphDatabasePort.ExecuteReadSingleAsync(countCypher, null, _options.Database, cancellationToken);
            if (countResult.IsFailure)
            {
                _logger.LogError("Failed to get graph statistics: {Error}", countResult.Error);
                return Result<KnowledgeGraphStatistics>.WithFailure(countResult.Error ?? "Failed to get graph statistics", default);
            }
            
            var countRecord = countResult.Value;
            var totalNodes = countRecord?.Values.GetValueOrDefault("totalNodes") is long tn ? tn : 0;
            var totalRelationships = countRecord?.Values.GetValueOrDefault("totalRelationships") is long tr ? tr : 0;

            // Get node types distribution
            var nodeTypesCypher = @"
                MATCH (n:KnowledgeEntity)
                RETURN n.type AS type, count(n) AS count";

            var nodeTypesResult = await _graphDatabasePort.ExecuteReadAsync(nodeTypesCypher, null, _options.Database, cancellationToken);
            if (nodeTypesResult.IsFailure)
            {
                _logger.LogError("Failed to get node types: {Error}", nodeTypesResult.Error);
                return Result<KnowledgeGraphStatistics>.WithFailure(nodeTypesResult.Error ?? "Failed to get node types", default);
            }
            
            if (nodeTypesResult.Value == null)
            {
                return Result<KnowledgeGraphStatistics>.WithFailure("No node types returned", default);
            }
            
            var nodeTypes = nodeTypesResult.Value.ToDictionary(
                r => r.Values["type"]?.ToString() ?? string.Empty,
                r => r.Values.GetValueOrDefault("count") is long count ? count : 0);

            // Get relationship types distribution
            var relTypesCypher = @"
                MATCH ()-[r]-()
                RETURN type(r) AS type, count(DISTINCT r) AS count";

            var relTypesResult = await _graphDatabasePort.ExecuteReadAsync(relTypesCypher, null, _options.Database, cancellationToken);
            if (relTypesResult.IsFailure)
            {
                _logger.LogError("Failed to get relationship types: {Error}", relTypesResult.Error);
                return Result<KnowledgeGraphStatistics>.WithFailure(relTypesResult.Error ?? "Failed to get relationship types", default);
            }
            
            if (relTypesResult.Value == null)
            {
                return Result<KnowledgeGraphStatistics>.WithFailure("No relationship types returned", default);
            }
            
            var relationshipTypes = relTypesResult.Value.ToDictionary(
                r => r.Values["type"]?.ToString() ?? string.Empty,
                r => r.Values.GetValueOrDefault("count") is long count ? count : 0);

            var stats = new KnowledgeGraphStatistics(
                TotalNodes: totalNodes,
                TotalRelationships: totalRelationships,
                NodeTypes: nodeTypes,
                RelationshipTypes: relationshipTypes,
                LastUpdated: DateTime.UtcNow
            );

            _logger.LogInformation("Graph statistics: {TotalNodes} nodes, {TotalRelationships} relationships", totalNodes, totalRelationships);
            return Result<KnowledgeGraphStatistics>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get graph statistics");
            return Result<KnowledgeGraphStatistics>.WithFailure($"Failed to get statistics: {ex.Message}", default);
        }
    }

    /// <inheritdoc />
    public async Task<Result> ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("Clearing entire graph - this will delete all nodes and relationships");

            var cypher = @"
                MATCH (n:KnowledgeEntity)
                DETACH DELETE n";

            var result = await _graphDatabasePort.ExecuteWriteVoidAsync(cypher, [], _options.Database, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogError("Failed to clear graph: {Error}", result.Error);
                return result;
            }
            
            _logger.LogInformation("Successfully cleared graph");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear graph");
            return Result.WithFailure($"Failed to clear graph: {ex.Message}");
        }
    }
}
