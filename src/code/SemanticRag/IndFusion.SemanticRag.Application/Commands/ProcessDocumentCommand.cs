using IndQuestResults;

namespace IndFusion.SemanticRag.Application.Commands;

/// <summary>
/// Command to process a document.
/// </summary>
public class ProcessDocumentCommand
{
    /// <summary>
    /// Gets or sets the document content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document type.
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];
}