namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a semantic pattern for code analysis.
/// </summary>
/// <param name="Id">Unique identifier for the pattern.</param>
/// <param name="Name">Name of the pattern.</param>
/// <param name="Description">Description of what the pattern matches.</param>
/// <param name="Pattern">The pattern definition (regex, AST pattern, etc.).</param>
/// <param name="Category">Category of the pattern.</param>
/// <param name="Confidence">Confidence level for matches (0.0 to 1.0).</param>
/// <param name="Metadata">Additional metadata about the pattern.</param>
public readonly record struct SemanticPattern(
    string Id,
    string Name,
    string Description,
    string Pattern,
    string Category,
    float Confidence = 0.8f,
    IReadOnlyDictionary<string, object>? Metadata = null)
{
    /// <summary>
    /// Validates the semantic pattern.
    /// </summary>
    /// <returns>A Result indicating whether the pattern is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Name))
            return Result.WithFailure("Pattern name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Description))
            return Result.WithFailure("Pattern description cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Pattern))
            return Result.WithFailure("Pattern definition cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Category))
            return Result.WithFailure("Pattern category cannot be null or empty");

        if (Confidence < 0.0f || Confidence > 1.0f)
            return Result.WithFailure("Pattern confidence must be between 0.0 and 1.0");

        return Result.Success();
    }
}