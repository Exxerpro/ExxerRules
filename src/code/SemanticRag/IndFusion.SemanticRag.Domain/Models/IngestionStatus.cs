using System.Text.Json.Serialization;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Status of document ingestion operations.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IngestionStatus
{
    /// <summary>
    /// Ingestion is pending.
    /// </summary>
    Pending,

    /// <summary>
    /// Ingestion is in progress.
    /// </summary>
    Processing,

    /// <summary>
    /// Ingestion completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Ingestion failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Ingestion was cancelled.
    /// </summary>
    Cancelled
}