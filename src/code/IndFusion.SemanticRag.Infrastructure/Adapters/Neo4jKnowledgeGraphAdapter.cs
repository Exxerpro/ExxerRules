using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using IndQuestResults;
using IndFusion.SemanticRag.Infrastructure.Configuration;

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
        try
        {
            _logger.LogInformation("Storing node: {NodeId} with label: {Label}", node.Id, node.Label);

            var validation = node.Validate();
            if (validation.IsFailure)
            {
                _logger.LogWarning("Node validation failed: {Error}", validation.Error);
                return validation;
            }

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
                ["id"] = node.Id,
                ["label"] = node.Label,
                ["properties"] = node.Properties,
                ["createdAt"] = node.CreatedAt,
                ["updatedAt"] = node.UpdatedAt
            };

            await session.RunAsync(cypher, parameters);
            
            _logger.LogInformation("Successfully stored node: {NodeId}", node.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store node: {NodeId}", node.Id);
            return Result.WithFailure($"Failed to store node: {ex.Message}");
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
        try
        {
            _logger.LogInformation("Storing {Count} nodes", nodes.Count);

            if (!nodes.Any())
            {
                _logger.LogWarning("No nodes provided for storage");
                return Result.Success();
            }

            // Validate all nodes first
            foreach (var node in nodes)
            {
                var validation = node.Validate();
                if (validation.IsFailure)
                {
                    _logger.LogWarning("Node validation failed for {NodeId}: {Error}", node.Id, validation.Error);
                    return validation;
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
                ["nodes"] = nodes.Select(n => new Dictionary<string, object>
                {
                    ["id"] = n.Id,
                    ["label"] = n.Label,
                    ["properties"] = n.Properties,
                    ["createdAt"] = n.CreatedAt,
                    ["updatedAt"] = n.UpdatedAt
                }).ToList()
            };

            await session.RunAsync(cypher, parameters);
            
            _logger.LogInformation("Successfully stored {Count} nodes", nodes.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store {Count} nodes", nodes.Count);
            return Result.WithFailure($"Failed to store nodes: {ex.Message}");
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
        try
        {
            _logger.LogInformation("Storing relationship: {RelationshipId} from {SourceId} to {TargetId}", 
                relationship.Id, relationship.FromNodeId, relationship.ToNodeId);

            var validation = relationship.Validate();
            if (validation.IsFailure)
            {
                _logger.LogWarning("Relationship validation failed: {Error}", validation.Error);
                return validation;
            }

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
                ["id"] = relationship.Id,
                ["sourceId"] = relationship.FromNodeId,
                ["targetId"] = relationship.ToNodeId,
                ["type"] = relationship.RelationshipType,
                ["properties"] = relationship.Properties,
                ["createdAt"] = relationship.CreatedAt
            };

            await session.RunAsync(cypher, parameters);
            
            _logger.LogInformation("Successfully stored relationship: {RelationshipId}", relationship.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store relationship: {RelationshipId}", relationship.Id);
            return Result.WithFailure($"Failed to store relationship: {ex.Message}");
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
        try
        {
            _logger.LogInformation("Storing {Count} relationships", relationships.Count);

            if (!relationships.Any())
            {
                _logger.LogWarning("No relationships provided for storage");
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
                ["relationships"] = relationships.Select(r => new Dictionary<string, object>
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
            
            _logger.LogInformation("Successfully stored {Count} relationships", relationships.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store {Count} relationships", relationships.Count);
            return Result.WithFailure($"Failed to store relationships: {ex.Message}");
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
        try
        {
            _logger.LogInformation("Retrieving node: {NodeId}", nodeId);

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = @"
                MATCH (n:KnowledgeNode {id: $id})
                RETURN n.id AS id, n.label AS label, n.properties AS properties, 
                       n.createdAt AS createdAt, n.updatedAt AS updatedAt";

            var parameters = new Dictionary<string, object> { ["id"] = nodeId };
            var result = await session.RunAsync(cypher, parameters);
            var record = await result.SingleOrDefaultAsync(cancellationToken);

            if (record == null)
            {
                _logger.LogWarning("Node not found: {NodeId}", nodeId);
                return Result<KnowledgeNode>.WithFailure($"Node not found: {nodeId}");
            }

            var node = new KnowledgeNode(
                record["id"].As<string>(),
                record["label"].As<string>(),
                record["properties"].As<Dictionary<string, object>>(),
                record["createdAt"].As<DateTimeOffset>(),
                record["updatedAt"].As<DateTimeOffset>()
            );

            _logger.LogInformation("Successfully retrieved node: {NodeId}", nodeId);
            return Result<KnowledgeNode>.Success(node);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve node: {NodeId}", nodeId);
            return Result<KnowledgeNode>.WithFailure($"Failed to retrieve node: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeNode>>> GetNodesByLabelAsync(
        string label, 
        int limit = 100, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving nodes by label: {Label} with limit: {Limit}", label, limit);

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = @"
                MATCH (n:KnowledgeNode {label: $label})
                RETURN n.id AS id, n.label AS label, n.properties AS properties, 
                       n.createdAt AS createdAt, n.updatedAt AS updatedAt
                LIMIT $limit";

            var parameters = new Dictionary<string, object> 
            { 
                ["label"] = label,
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

            _logger.LogInformation("Successfully retrieved {Count} nodes with label: {Label}", nodes.Count, label);
            return Result<IReadOnlyList<KnowledgeNode>>.Success(nodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve nodes by label: {Label}", label);
            return Result<IReadOnlyList<KnowledgeNode>>.WithFailure($"Failed to retrieve nodes by label: {ex.Message}");
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
        try
        {
            _logger.LogInformation("Retrieving relationships for node: {NodeId}", nodeId);

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = @"
                MATCH (n:KnowledgeNode {id: $id})-[r:RELATIONSHIP]-(related)
                RETURN r.id AS id, r.type AS type, r.properties AS properties, r.createdAt AS createdAt,
                       startNode(r).id AS sourceId, endNode(r).id AS targetId";

            var parameters = new Dictionary<string, object> { ["id"] = nodeId };
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
                relationships.Count, nodeId);
            return Result<IReadOnlyList<KnowledgeRelationship>>.Success(relationships);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve relationships for node: {NodeId}", nodeId);
            return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure($"Failed to retrieve relationships: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeNode>>> QueryNodesAsync(
        string cypherQuery,
        IReadOnlyDictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Executing node query: {Query}", cypherQuery);

            using var session = _driver.AsyncSession(ConfigureSession);
            var result = await session.RunAsync(cypherQuery, new Dictionary<string, object>(parameters ?? new Dictionary<string, object>()));
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute node query: {Query}", cypherQuery);
            return Result<IReadOnlyList<KnowledgeNode>>.WithFailure($"Failed to execute node query: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<KnowledgeRelationship>>> QueryRelationshipsAsync(
        string cypherQuery,
        IReadOnlyDictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Executing relationship query: {Query}", cypherQuery);

            using var session = _driver.AsyncSession(ConfigureSession);
            var result = await session.RunAsync(cypherQuery, new Dictionary<string, object>(parameters ?? new Dictionary<string, object>()));
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute relationship query: {Query}", cypherQuery);
            return Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure($"Failed to execute relationship query: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateNodeAsync(KnowledgeNode node, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating node: {NodeId}", node.Id);

            var validation = node.Validate();
            if (validation.IsFailure)
            {
                _logger.LogWarning("Node validation failed: {Error}", validation.Error);
                return validation;
            }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update node: {NodeId}", node.Id);
            return Result.WithFailure($"Failed to update node: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<int>> GetNodeCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting node count");

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = "MATCH (n:KnowledgeNode) RETURN count(n) AS count";
            var result = await session.RunAsync(cypher, new Dictionary<string, object>());
            var record = await result.SingleAsync(cancellationToken);

            var count = record["count"].As<int>();
            _logger.LogInformation("Node count: {Count}", count);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get node count");
            return Result<int>.WithFailure($"Failed to get node count: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<int>> GetRelationshipCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting relationship count");

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = "MATCH ()-[r:RELATIONSHIP]-() RETURN count(r) AS count";
            var result = await session.RunAsync(cypher, new Dictionary<string, object>());
            var record = await result.SingleAsync(cancellationToken);

            var count = record["count"].As<int>();
            _logger.LogInformation("Relationship count: {Count}", count);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get relationship count");
            return Result<int>.WithFailure($"Failed to get relationship count: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> ClearAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Clearing all nodes and relationships");

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = "MATCH (n) DETACH DELETE n";
            await session.RunAsync(cypher, new Dictionary<string, object>());
            
            _logger.LogInformation("Successfully cleared all nodes and relationships");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear all nodes and relationships");
            return Result.WithFailure($"Failed to clear all: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<IReadOnlyDictionary<string, object>>>> ExecuteGraphQueryAsync(
        string query,
        IReadOnlyDictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Executing graph query: {Query}", query);

            using var session = _driver.AsyncSession(ConfigureSession);
            var result = await session.RunAsync(query, new Dictionary<string, object>(parameters ?? new Dictionary<string, object>()));
            var records = await result.ToListAsync(cancellationToken);

            var queryResults = records.Select(record => 
                record.Values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) as IReadOnlyDictionary<string, object>
            ).ToList();

            _logger.LogInformation("Successfully executed graph query, returned {Count} results", queryResults.Count);
            return Result<IReadOnlyList<IReadOnlyDictionary<string, object>>>.Success(queryResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute graph query: {Query}", query);
            return Result<IReadOnlyList<IReadOnlyDictionary<string, object>>>.WithFailure($"Failed to execute graph query: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting node: {NodeId}", nodeId);

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = @"
                MATCH (n:KnowledgeNode {id: $id})
                DETACH DELETE n";

            var parameters = new Dictionary<string, object> { ["id"] = nodeId };
            await session.RunAsync(cypher, parameters);
            
            _logger.LogInformation("Successfully deleted node: {NodeId}", nodeId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete node: {NodeId}", nodeId);
            return Result.WithFailure($"Failed to delete node: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteRelationshipAsync(string relationshipId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting relationship: {RelationshipId}", relationshipId);

            using var session = _driver.AsyncSession(ConfigureSession);
            var cypher = @"
                MATCH ()-[r:RELATIONSHIP {id: $id}]-()
                DELETE r";

            var parameters = new Dictionary<string, object> { ["id"] = relationshipId };
            await session.RunAsync(cypher, parameters);
            
            _logger.LogInformation("Successfully deleted relationship: {RelationshipId}", relationshipId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete relationship: {RelationshipId}", relationshipId);
            return Result.WithFailure($"Failed to delete relationship: {ex.Message}");
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