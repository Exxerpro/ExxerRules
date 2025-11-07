using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
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
/// Neo4j implementation of the knowledge graph port following hexagonal architecture.
/// </summary>
public class Neo4jKnowledgeGraphAdapter : IKnowledgeGraphPort
{
    private readonly IDriver _driver;
    private readonly ILogger<Neo4jKnowledgeGraphAdapter> _logger;
    private readonly Neo4jOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jKnowledgeGraphAdapter"/> class.
    /// </summary>
    /// <param name="driver">The Neo4j driver instance.</param>
    /// <param name="options">The Neo4j configuration options.</param>
    /// <param name="logger">The logger instance.</param>
    public Neo4jKnowledgeGraphAdapter(
        IDriver driver,
        IOptions<Neo4jOptions> options,
        ILogger<Neo4jKnowledgeGraphAdapter> logger)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result> CreateNodeAsync(KnowledgeNode node, CancellationToken cancellationToken = default)
    {
        return await StoreNodeAsync(node, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> StoreNodeAsync(KnowledgeNode node, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }

        try
        {
            var validation = node.Validate();
            if (validation.IsFailure)
            {
                return Result.WithFailure(validation.Error ?? ErrorCodes.KnowledgeNodeIdRequired);
            }

            return await Task.FromResult(Result<KnowledgeNode>.Success(node))
                .ThenAsync<KnowledgeNode, Result>(
                    async n =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Storing node: {NodeId} with label: {Label}", n.Id, n.Label);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            MERGE (n:KnowledgeNode {id: $id})
                            SET n.label = $label,
                                n.properties = $properties,
                                n.createdAt = $createdAt,
                                n.updatedAt = $updatedAt
                            RETURN n";

                        var parameters = new Dictionary<string, object>
                        {
                            ["id"] = n.Id,
                            ["label"] = n.Label,
                            ["properties"] = n.Properties,
                            ["createdAt"] = n.CreatedAt,
                            ["updatedAt"] = n.UpdatedAt
                        };

                        await session.RunAsync(cypher, parameters);

                        _logger.LogInformation("Successfully stored node: {NodeId}", n.Id);
                        return Result.Success();
                    });
        }
        catch (OperationCanceledException)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store node: {NodeId}", node.Id);
            return Result.WithFailure(ErrorCodes.GraphDatabaseError);
        }
    }

    /// <inheritdoc />
    public async Task<Result> CreateNodesAsync(IReadOnlyList<KnowledgeNode> nodes, CancellationToken cancellationToken = default)
    {
        return await StoreNodesAsync(nodes, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> StoreNodesAsync(IReadOnlyList<KnowledgeNode> nodes, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }

        try
        {
            if (nodes == null)
            {
                return Result.WithFailure(ErrorCodes.ParameterNull);
            }

            // Allow empty list - it's a valid operation (no-op)
            if (!nodes.Any())
            {
                _logger.LogInformation("Empty nodes list provided - returning success");
                return Result.Success();
            }

            return await Task.FromResult(Result<IReadOnlyList<KnowledgeNode>>.Success(nodes))
                .ThenAsync<IReadOnlyList<KnowledgeNode>, Result>(
                    async nodesList =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Storing {Count} nodes", nodesList.Count);

                        // Validate all nodes first
                        foreach (var node in nodesList)
                        {
                            var validation = node.Validate();
                            if (validation.IsFailure)
                            {
                                _logger.LogWarning("Node validation failed for {NodeId}: {Error}", node.Id, validation.Error);
                                return Result.WithFailure(validation.Error ?? ErrorCodes.KnowledgeNodeIdRequired);
                            }
                        }

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            UNWIND $nodes AS node
                            MERGE (n:KnowledgeNode {id: node.id})
                            SET n.label = node.label,
                                n.properties = node.properties,
                                n.createdAt = node.createdAt,
                                n.updatedAt = node.updatedAt";

                        var parameters = new Dictionary<string, object>
                        {
                            ["nodes"] = nodesList.Select(n => new Dictionary<string, object>
                            {
                                ["id"] = n.Id,
                                ["label"] = n.Label,
                                ["properties"] = n.Properties,
                                ["createdAt"] = n.CreatedAt,
                                ["updatedAt"] = n.UpdatedAt
                            }).ToList()
                        };

                        await session.RunAsync(cypher, parameters);

                        _logger.LogInformation("Successfully stored {Count} nodes", nodesList.Count);
                        return Result.Success();
                    });
        }
        catch (OperationCanceledException)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store {Count} nodes", nodes.Count);
            return Result.WithFailure(ErrorCodes.GraphDatabaseError);
        }
    }

    /// <inheritdoc />
    public async Task<Result> CreateRelationshipAsync(KnowledgeRelationship relationship, CancellationToken cancellationToken = default)
    {
        return await StoreRelationshipAsync(relationship, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> StoreRelationshipAsync(KnowledgeRelationship relationship, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }

        try
        {
            var validation = relationship.Validate();
            if (validation.IsFailure)
            {
                return Result.WithFailure(validation.Error ?? ErrorCodes.KnowledgeRelationshipIdRequired);
            }

            return await Task.FromResult(Result<KnowledgeRelationship>.Success(relationship))
                .ThenAsync<KnowledgeRelationship, Result>(
                    async r =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Storing relationship: {RelationshipId} from {SourceId} to {TargetId}",
                            r.Id, r.FromNodeId, r.ToNodeId);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            MATCH (source:KnowledgeNode {id: $sourceId})
                            MATCH (target:KnowledgeNode {id: $targetId})
                            MERGE (source)-[r:RELATIONSHIP {id: $id}]->(target)
                            SET r.type = $type,
                                r.properties = $properties,
                                r.createdAt = $createdAt
                            RETURN r";

                        var parameters = new Dictionary<string, object>
                        {
                            ["id"] = r.Id,
                            ["sourceId"] = r.FromNodeId,
                            ["targetId"] = r.ToNodeId,
                            ["type"] = r.RelationshipType,
                            ["properties"] = r.Properties,
                            ["createdAt"] = r.CreatedAt
                        };

                        await session.RunAsync(cypher, parameters);

                        _logger.LogInformation("Successfully stored relationship: {RelationshipId}", r.Id);
                        return Result.Success();
                    });
        }
        catch (OperationCanceledException)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store relationship: {RelationshipId}", relationship.Id);
            return Result.WithFailure(ErrorCodes.GraphDatabaseError);
        }
    }

    /// <inheritdoc />
    public async Task<Result> CreateRelationshipsAsync(IReadOnlyList<KnowledgeRelationship> relationships, CancellationToken cancellationToken = default)
    {
        return await StoreRelationshipsAsync(relationships, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> StoreRelationshipsAsync(IReadOnlyList<KnowledgeRelationship> relationships, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<IReadOnlyList<KnowledgeRelationship>>.Success(relationships)
                .Ensure(
                    rs => rs != null && rs.Any(),
                    ErrorCodes.CollectionEmpty))
                .ThenAsync<IReadOnlyList<KnowledgeRelationship>, Result>(
                    async relationshipsList =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Storing {Count} relationships", relationshipsList.Count);

                        // Validate all relationships first
                        foreach (var relationship in relationshipsList)
                        {
                            var validation = relationship.Validate();
                            if (validation.IsFailure)
                            {
                                _logger.LogWarning("Relationship validation failed for {RelationshipId}: {Error}",
                                    relationship.Id, validation.Error);
                                return validation;
                            }
                        }

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            UNWIND $relationships AS rel
                            MATCH (source:KnowledgeNode {id: rel.sourceId})
                            MATCH (target:KnowledgeNode {id: rel.targetId})
                            MERGE (source)-[r:RELATIONSHIP {id: rel.id}]->(target)
                            SET r.type = rel.type,
                                r.properties = rel.properties,
                                r.createdAt = rel.createdAt";

                        var parameters = new Dictionary<string, object>
                        {
                            ["relationships"] = relationshipsList.Select(r => new Dictionary<string, object>
                            {
                                ["id"] = r.Id,
                                ["sourceId"] = r.FromNodeId,
                                ["targetId"] = r.ToNodeId,
                                ["type"] = r.RelationshipType,
                                ["properties"] = r.Properties,
                                ["createdAt"] = r.CreatedAt
                            }).ToList()
                        };

                        await session.RunAsync(cypher, parameters);

                        _logger.LogInformation("Successfully stored {Count} relationships", relationshipsList.Count);
                        return Result.Success();
                    });
        }
        catch (OperationCanceledException)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store {Count} relationships", relationships.Count);
            return Result.WithFailure(ErrorCodes.GraphDatabaseError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<KnowledgeNode?>> GetNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        var result = await GetNodeByIdAsync(nodeId, cancellationToken);
        return result.IsSuccess
            ? Result<KnowledgeNode?>.Success(result.Value)
            : Result<KnowledgeNode?>.WithFailure(result.Error!);
    }

    /// <inheritdoc />
    public async Task<Result<KnowledgeNode>> GetNodeByIdAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<KnowledgeNode>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(nodeId)
                .Ensure(
                    id => !string.IsNullOrWhiteSpace(id),
                    ErrorCodes.ParameterNullOrWhitespace))
                .ThenAsync(
                    async id =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<KnowledgeNode>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Retrieving node: {NodeId}", id);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            MATCH (n:KnowledgeNode {id: $id})
                            RETURN n.id AS id, n.label AS label, n.properties AS properties,
                                   n.createdAt AS createdAt, n.updatedAt AS updatedAt";

                        var parameters = new Dictionary<string, object> { ["id"] = id };
                        IRecord? record;
                        try
                        {
                            var result = await session.RunAsync(cypher, parameters);
                            record = await result.SingleOrDefaultAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception while retrieving node: {NodeId}", id);
                            return Result<KnowledgeNode>.WithFailure(ErrorCodes.CypherQueryFailed);
                        }

                        if (record == null)
                        {
                            _logger.LogWarning("Node not found: {NodeId}", id);
                            return Result<KnowledgeNode>.WithFailure(ErrorCodes.EntityNotFound);
                        }

                        var node = new KnowledgeNode(
                            record["id"].As<string>(),
                            record["label"].As<string>(),
                            record["properties"].As<Dictionary<string, object>>(),
                            record["createdAt"].As<DateTimeOffset>(),
                            record["updatedAt"].As<DateTimeOffset>()
                        );

                        _logger.LogInformation("Successfully retrieved node: {NodeId}", id);
                        return Result<KnowledgeNode>.Success(node);
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<KnowledgeNode>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve node: {NodeId}", nodeId);
            return WithFailure<KnowledgeNode>(ErrorCodes.CypherQueryFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeNode>>> GetNodesByLabelAsync(
        string label,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<IReadOnlyList<KnowledgeNode>>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(label)
                .Ensure(
                    l => !string.IsNullOrWhiteSpace(l),
                    ErrorCodes.ParameterNullOrWhitespace))
                .ThenAsync(
                    async labelValue =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<IReadOnlyList<KnowledgeNode>>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Retrieving nodes by label: {Label} with limit: {Limit}", labelValue, limit);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            MATCH (n:KnowledgeNode {label: $label})
                            RETURN n.id AS id, n.label AS label, n.properties AS properties,
                                   n.createdAt AS createdAt, n.updatedAt AS updatedAt
                            LIMIT $limit";

                        var parameters = new Dictionary<string, object>
                        {
                            ["label"] = labelValue,
                            ["limit"] = limit
                        };

                        var result = await session.RunAsync(cypher, parameters);
                        var records = await result.ToListAsync(cancellationToken);

                        var nodes = records.Select(record => new KnowledgeNode(
                            record["id"].As<string>(),
                            record["label"].As<string>(),
                            record["properties"].As<Dictionary<string, object>>(),
                            record["createdAt"].As<DateTimeOffset>(),
                            record["updatedAt"].As<DateTimeOffset>()
                        )).ToList();

                        _logger.LogInformation("Successfully retrieved {Count} nodes with label: {Label}", nodes.Count, labelValue);
                        return Result<IReadOnlyList<KnowledgeNode>>.Success(nodes);
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<IReadOnlyList<KnowledgeNode>>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve nodes by label: {Label}", label);
            return WithFailure<IReadOnlyList<KnowledgeNode>>(ErrorCodes.CypherQueryFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeRelationship>>> GetNodeRelationshipsAsync(
        string nodeId,
        string? relationshipType = null,
        RelationshipDirection direction = RelationshipDirection.Both,
        CancellationToken cancellationToken = default)
    {
        return await GetRelationshipsForNodeAsync(nodeId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeRelationship>>> GetRelationshipsForNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<IReadOnlyList<KnowledgeRelationship>>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(nodeId)
                .Ensure(
                    id => !string.IsNullOrWhiteSpace(id),
                    ErrorCodes.ParameterNullOrWhitespace))
                .ThenAsync(
                    async id =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Retrieving relationships for node: {NodeId}", id);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            MATCH (n:KnowledgeNode {id: $id})-[r:RELATIONSHIP]-(related)
                            RETURN r.id AS id, r.type AS type, r.properties AS properties, r.createdAt AS createdAt,
                                   startNode(r).id AS sourceId, endNode(r).id AS targetId";

                        var parameters = new Dictionary<string, object> { ["id"] = id };
                        var result = await session.RunAsync(cypher, parameters);
                        var records = await result.ToListAsync(cancellationToken);

                        var relationships = records.Select(record => new KnowledgeRelationship(
                            record["id"].As<string>(),
                            record["sourceId"].As<string>(),
                            record["targetId"].As<string>(),
                            record["type"].As<string>(),
                            record["properties"].As<Dictionary<string, object>>(),
                            record["createdAt"].As<DateTimeOffset>()
                        )).ToList();

                        _logger.LogInformation("Successfully retrieved {Count} relationships for node: {NodeId}",
                            relationships.Count, id);
                        return Result<IReadOnlyList<KnowledgeRelationship>>.Success(relationships);
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<IReadOnlyList<KnowledgeRelationship>>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve relationships for node: {NodeId}", nodeId);
            return WithFailure<IReadOnlyList<KnowledgeRelationship>>(ErrorCodes.CypherQueryFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeNode>>> QueryNodesAsync(
        string cypherQuery,
        IReadOnlyDictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<IReadOnlyList<KnowledgeNode>>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(cypherQuery)
                .Ensure(
                    query => !string.IsNullOrWhiteSpace(query),
                    ErrorCodes.ParameterNullOrWhitespace))
                .ThenAsync(
                    async query =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<IReadOnlyList<KnowledgeNode>>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Executing node query: {Query}", query);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var result = await session.RunAsync(query, new Dictionary<string, object>(parameters ?? new Dictionary<string, object>()));
                        var records = await result.ToListAsync(cancellationToken);

                        var nodes = records.Select(record => new KnowledgeNode(
                            record["id"].As<string>(),
                            record["label"].As<string>(),
                            record["properties"].As<Dictionary<string, object>>(),
                            record["createdAt"].As<DateTimeOffset>(),
                            record["updatedAt"].As<DateTimeOffset>()
                        )).ToList();

                        _logger.LogInformation("Successfully executed node query, returned {Count} nodes", nodes.Count);
                        return Result<IReadOnlyList<KnowledgeNode>>.Success(nodes);
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<IReadOnlyList<KnowledgeNode>>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute node query: {Query}", cypherQuery);
            return WithFailure<IReadOnlyList<KnowledgeNode>>(ErrorCodes.CypherQueryFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeRelationship>>> QueryRelationshipsAsync(
        string cypherQuery,
        IReadOnlyDictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<IReadOnlyList<KnowledgeRelationship>>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(cypherQuery)
                .Ensure(
                    query => !string.IsNullOrWhiteSpace(query),
                    ErrorCodes.ParameterNullOrWhitespace))
                .ThenAsync(
                    async query =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Executing relationship query: {Query}", query);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var result = await session.RunAsync(query, new Dictionary<string, object>(parameters ?? new Dictionary<string, object>()));
                        var records = await result.ToListAsync(cancellationToken);

                        var relationships = records.Select(record => new KnowledgeRelationship(
                            record["id"].As<string>(),
                            record["sourceId"].As<string>(),
                            record["targetId"].As<string>(),
                            record["type"].As<string>(),
                            record["properties"].As<Dictionary<string, object>>(),
                            record["createdAt"].As<DateTimeOffset>()
                        )).ToList();

                        _logger.LogInformation("Successfully executed relationship query, returned {Count} relationships", relationships.Count);
                        return Result<IReadOnlyList<KnowledgeRelationship>>.Success(relationships);
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<IReadOnlyList<KnowledgeRelationship>>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute relationship query: {Query}", cypherQuery);
            return WithFailure<IReadOnlyList<KnowledgeRelationship>>(ErrorCodes.CypherQueryFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateNodeAsync(KnowledgeNode node, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }

        try
        {
            var validation = node.Validate();
            if (validation.IsFailure)
            {
                return Result.WithFailure(validation.Error ?? ErrorCodes.KnowledgeNodeIdRequired);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return Result.WithFailure(ErrorCodes.OperationCancelled);
            }

            _logger.LogInformation("Updating node: {NodeId}", node.Id);

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = @"
                MATCH (n:KnowledgeNode {id: $id})
                SET n.label = $label,
                    n.properties = $properties,
                    n.updatedAt = $updatedAt
                RETURN n";

            var parameters = new Dictionary<string, object>
            {
                ["id"] = node.Id,
                ["label"] = node.Label,
                ["properties"] = node.Properties,
                ["updatedAt"] = node.UpdatedAt
            };

            await session.RunAsync(cypher, parameters);

            _logger.LogInformation("Successfully updated node: {NodeId}", node.Id);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update node: {NodeId}", node.Id);
            return Result.WithFailure(ErrorCodes.GraphDatabaseError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<int>> GetNodeCountAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<int>(ErrorCodes.OperationCancelled);
        }

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result<int>.WithFailure(ErrorCodes.OperationCancelled);
            }

            _logger.LogInformation("Getting node count");

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = "MATCH (n:KnowledgeNode) RETURN count(n) AS count";
            var result = await session.RunAsync(cypher, new Dictionary<string, object>());
            var record = await result.SingleAsync(cancellationToken);

            var count = record["count"].As<int>();
            _logger.LogInformation("Node count: {Count}", count);
            return Result<int>.Success(count);
        }
        catch (OperationCanceledException)
        {
            return Cancelled<int>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get node count");
            return WithFailure<int>(ErrorCodes.CypherQueryFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<int>> GetRelationshipCountAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<int>(ErrorCodes.OperationCancelled);
        }

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result<int>.WithFailure(ErrorCodes.OperationCancelled);
            }

            _logger.LogInformation("Getting relationship count");

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = "MATCH ()-[r:RELATIONSHIP]-() RETURN count(r) AS count";
            var result = await session.RunAsync(cypher, new Dictionary<string, object>());
            var record = await result.SingleAsync(cancellationToken);

            var count = record["count"].As<int>();
            _logger.LogInformation("Relationship count: {Count}", count);
            return Result<int>.Success(count);
        }
        catch (OperationCanceledException)
        {
            return Cancelled<int>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get relationship count");
            return WithFailure<int>(ErrorCodes.CypherQueryFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result> ClearAllAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.WithFailure(ErrorCodes.OperationCancelled);
            }

            _logger.LogInformation("Clearing all nodes and relationships");

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = "MATCH (n) DETACH DELETE n";
            await session.RunAsync(cypher, new Dictionary<string, object>());

            _logger.LogInformation("Successfully cleared all nodes and relationships");
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear all nodes and relationships");
            return Result.WithFailure(ErrorCodes.GraphDatabaseError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IReadOnlyDictionary<string, object>>>> ExecuteGraphQueryAsync(

        string query,
        IReadOnlyDictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<IReadOnlyList<IReadOnlyDictionary<string, object>>>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(query)
                .Ensure(
                    q => !string.IsNullOrWhiteSpace(q),
                    ErrorCodes.ParameterNullOrWhitespace))
                .ThenAsync(
                    async q =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<IReadOnlyList<IReadOnlyDictionary<string, object>>>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Executing graph query: {Query}", q);

                        try
                        {
                            using var session = _driver.AsyncSession(ConfigureSession);
                            var result = await session.RunAsync(q, new Dictionary<string, object>(parameters ?? new Dictionary<string, object>()));
                            var records = await result.ToListAsync(cancellationToken);

                            var queryResults = records.Select(record =>
                                record.Values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) as IReadOnlyDictionary<string, object>
                            ).ToList();

                            _logger.LogInformation("Successfully executed graph query, returned {Count} results", queryResults.Count);
                            return Result<IReadOnlyList<IReadOnlyDictionary<string, object>>>.Success(queryResults);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception while executing graph query: {Query}", q);
                            return Result<IReadOnlyList<IReadOnlyDictionary<string, object>>>.WithFailure(ErrorCodes.CypherQueryFailed);
                        }
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<IReadOnlyList<IReadOnlyDictionary<string, object>>>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute graph query: {Query}", query);
            return WithFailure<IReadOnlyList<IReadOnlyDictionary<string, object>>>(ErrorCodes.CypherQueryFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(nodeId)
                .Ensure(
                    id => !string.IsNullOrWhiteSpace(id),
                    ErrorCodes.ParameterNullOrWhitespace))
                .ThenAsync<string, Result>(
                    async id =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Deleting node: {NodeId}", id);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            MATCH (n:KnowledgeNode {id: $id})
                            DETACH DELETE n";

                        var parameters = new Dictionary<string, object> { ["id"] = id };
                        await session.RunAsync(cypher, parameters);

                        _logger.LogInformation("Successfully deleted node: {NodeId}", id);
                        return Result.Success();
                    });
        }
        catch (OperationCanceledException)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete node: {NodeId}", nodeId);
            return Result.WithFailure(ErrorCodes.GraphDatabaseError);
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteRelationshipAsync(string relationshipId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(relationshipId)
                .Ensure(
                    id => !string.IsNullOrWhiteSpace(id),
                    ErrorCodes.ParameterNullOrWhitespace))
                .ThenAsync<string, Result>(
                    async id =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Deleting relationship: {RelationshipId}", id);

                        using var session = _driver.AsyncSession(ConfigureSession);
                        var cypher = @"
                            MATCH ()-[r:RELATIONSHIP {id: $id}]-()
                            DELETE r";

                        var parameters = new Dictionary<string, object> { ["id"] = id };
                        await session.RunAsync(cypher, parameters);

                        _logger.LogInformation("Successfully deleted relationship: {RelationshipId}", id);
                        return Result.Success();
                    });
        }
        catch (OperationCanceledException)
        {
            return Result.WithFailure(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete relationship: {RelationshipId}", relationshipId);
            return Result.WithFailure(ErrorCodes.GraphDatabaseError);
        }
    }

    /// <summary>
    /// Configures the Neo4j session with database and other options.
    /// </summary>
    /// <param name="config">The session configuration builder.</param>
    private void ConfigureSession(SessionConfigBuilder config)
    {
        config.WithDatabase(_options.Database);
    }

    /// <summary>
    /// Disposes the Neo4j driver.
    /// </summary>
    public void Dispose()
    {
        _driver?.Dispose();
    }
}