namespace IndFusion.SemanticRag.WebAPI.Controllers;

/// <summary>
/// Request model for code analysis.
/// </summary>
public record CodeAnalysisRequest
{
    /// <summary>
    /// The code to analyze.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Programming language of the code.
    /// </summary>
    public required string Language { get; init; }

    /// <summary>
    /// Optional context for analysis.
    /// </summary>
    public string? Context { get; init; }
}