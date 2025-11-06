namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a pattern definition for code analysis.
/// </summary>
/// <param name="Id">Unique identifier for the pattern.</param>
/// <param name="Name">Name of the pattern.</param>
/// <param name="Description">Description of what the pattern checks for.</param>
/// <param name="Category">Category of the pattern (e.g., "Performance", "Security", "Maintainability").</param>
/// <param name="Severity">Default severity level for violations of this pattern.</param>
/// <param name="Pattern">The pattern definition (regex, AST pattern, etc.).</param>
/// <param name="Tags">Tags associated with the pattern.</param>
/// <param name="IsEnabled">Whether the pattern is currently enabled.</param>
/// <param name="CreatedAt">When the pattern was created.</param>
/// <param name="UpdatedAt">When the pattern was last updated.</param>
public readonly record struct PatternDefinition(
    string Id,
    string Name,
    string Description,
    string Category,
    PatternSeverity Severity,
    string Pattern,
    IReadOnlyList<string> Tags,
    bool IsEnabled = true,
    DateTimeOffset? CreatedAt = null,
    DateTimeOffset? UpdatedAt = null)
{
    /// <summary>
    /// Validates the pattern definition.
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

        if (string.IsNullOrWhiteSpace(Category))
            return Result.WithFailure("Pattern category cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Pattern))
            return Result.WithFailure("Pattern definition cannot be null or empty");

        if (Tags is null)
            return Result.WithFailure("Pattern tags cannot be null");

        return Result.Success();
    }

    /// <summary>
    /// Checks if the pattern has a specific tag.
    /// </summary>
    /// <param name="tag">The tag to check for.</param>
    /// <returns>True if the pattern has the tag, otherwise false.</returns>
    public bool HasTag(string tag) => Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
}