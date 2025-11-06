using System.Text.Json.Serialization;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the status of a processing operation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProcessingStatus
{
    /// <summary>
    /// Processing is pending.
    /// </summary>
    Pending,
    /// <summary>
    /// Processing is in progress.
    /// </summary>
    InProgress,
    /// <summary>
    /// Processing completed successfully.
    /// </summary>
    Success,
    /// <summary>
    /// Processing failed.
    /// </summary>
    Failed,
    /// <summary>
    /// Processing was cancelled.
    /// </summary>
    Cancelled
}