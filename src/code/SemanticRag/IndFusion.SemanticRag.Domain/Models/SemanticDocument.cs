namespace IndFusion.SemanticRag.Domain.Models;

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