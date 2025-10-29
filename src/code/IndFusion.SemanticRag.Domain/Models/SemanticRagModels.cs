namespace IndFusion.SemanticRag.Domain.Models;

// Note: GraphPath is now defined in TraversalModels.cs
// This file is kept for backward compatibility but the duplicate GraphPath definition has been removed

/// <summary>
/// Statistics about the knowledge graph.
/// </summary>
/// <param name="TotalNodes">Total number of nodes.</param>
/// <param name="TotalRelationships">Total number of relationships.</param>
/// <param name="NodeTypes">Count of nodes by type.</param>
/// <param name="RelationshipTypes">Count of relationships by type.</param>
/// <param name="LastUpdated">When statistics were last updated.</param>
public record KnowledgeGraphStatistics(
    long TotalNodes,
    long TotalRelationships,
    Dictionary<string, long> NodeTypes,
    Dictionary<string, long> RelationshipTypes,
    DateTime LastUpdated
);

/// <summary>
/// Represents a semantic document with extracted content and metadata.
/// </summary>
/// <param name="Id">Unique identifier for the document.</param>
/// <param name="Title">Document title.</param>
/// <param name="Content">Extracted text content.</param>
/// <param name="Metadata">Document metadata including source, author, etc.</param>
/// <param name="CreatedAt">When the document was processed.</param>
/// <param name="UpdatedAt">When the document was last updated.</param>
public record SemanticDocument(
    string Id,
    string Title,
    string Content,
    Dictionary<string, object> Metadata,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Represents a knowledge entity extracted from documents.
/// </summary>
/// <param name="Id">Unique identifier for the entity.</param>
/// <param name="Name">Entity name.</param>
/// <param name="Type">Entity type (person, organization, concept, etc.).</param>
/// <param name="Description">Entity description.</param>
/// <param name="Properties">Additional entity properties.</param>
/// <param name="Confidence">Confidence score for the extraction.</param>
/// <param name="CreatedAt">When the entity was extracted.</param>
public record KnowledgeEntity(
    string Id,
    string Name,
    string Type,
    string Description,
    Dictionary<string, object> Properties,
    double Confidence,
    DateTime CreatedAt
);

/// <summary>
/// Result of a knowledge graph query operation.
/// </summary>
/// <param name="Nodes">Nodes returned by the query.</param>
/// <param name="Relationships">Relationships returned by the query.</param>
/// <param name="TotalCount">Total number of results available.</param>
/// <param name="QueryTime">Time taken to execute the query.</param>
public record KnowledgeGraphResult(
    IEnumerable<GraphNode> Nodes,
    IEnumerable<GraphRelationship> Relationships,
    long TotalCount,
    TimeSpan QueryTime
);