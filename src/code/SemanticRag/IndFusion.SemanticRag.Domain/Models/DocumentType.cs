using System.Text.Json.Serialization;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents different document types that can be processed.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentType
{
    /// <summary>
    /// Unknown document type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Plain text document.
    /// </summary>
    Text,

    /// <summary>
    /// Markdown document.
    /// </summary>
    Markdown,

    /// <summary>
    /// PDF document.
    /// </summary>
    Pdf,

    /// <summary>
    /// Microsoft Word document.
    /// </summary>
    Word,

    /// <summary>
    /// Microsoft Excel spreadsheet.
    /// </summary>
    Excel,

    /// <summary>
    /// PowerPoint presentation.
    /// </summary>
    PowerPoint,

    /// <summary>
    /// Image file (PNG, JPG, etc.).
    /// </summary>
    Image,

    /// <summary>
    /// C# code file.
    /// </summary>
    CSharpCode,

    /// <summary>
    /// TypeScript/JavaScript code file.
    /// </summary>
    TypeScriptCode,

    /// <summary>
    /// Python code file.
    /// </summary>
    PythonCode,

    /// <summary>
    /// HTML document.
    /// </summary>
    Html,

    /// <summary>
    /// XML document.
    /// </summary>
    Xml,

    /// <summary>
    /// JSON document.
    /// </summary>
    Json,

    /// <summary>
    /// CSV file.
    /// </summary>
    Csv
}