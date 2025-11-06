using System.Text.Json.Serialization;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents different chunking strategies for documents.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChunkingStrategy
{
    /// <summary>
    /// Splits text by a fixed size.
    /// </summary>
    FixedSize,
    /// <summary>
    /// Splits text by sentences.
    /// </summary>
    Sentence,
    /// <summary>
    /// Splits text by paragraphs.
    /// </summary>
    Paragraph,
    /// <summary>
    /// Recursively splits text based on delimiters.
    /// </summary>
    Recursive,
    /// <summary>
    /// Splits text semantically based on content structure.
    /// </summary>
    Semantic,
    /// <summary>
    /// Splits text by lines (useful for code).
    /// </summary>
    LineBased,
    /// <summary>
    /// Splits text by paragraphs.
    /// </summary>
    ParagraphBased
}