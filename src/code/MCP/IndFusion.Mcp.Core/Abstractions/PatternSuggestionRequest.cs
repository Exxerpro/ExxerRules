namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for pattern suggestion operations.
/// </summary>
/// <param name="ViolationId">ID of the violation to suggest patterns for.</param>
/// <param name="RuleId">Rule ID that generated the violation.</param>
/// <param name="CodeSnippet">Code snippet containing the violation.</param>
/// <param name="FilePath">Path to the file containing the violation.</param>
/// <param name="Context">Additional context about the violation.</param>
/// <param name="MaxSuggestions">Maximum number of suggestions to return.</param>
/// <param name="ConfidenceThreshold">Minimum confidence threshold for suggestions.</param>
public record PatternSuggestionRequest(
    string ViolationId,
    string RuleId,
    string CodeSnippet,
    string FilePath,
    Dictionary<string, object>? Context = null,
    int MaxSuggestions = 5,
    double ConfidenceThreshold = 0.7
);