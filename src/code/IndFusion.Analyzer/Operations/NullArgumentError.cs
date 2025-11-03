namespace IndFusion.Analyzers.Operations;

/// <summary>
/// Represents a single null-argument validation failure discovered during guard evaluation.
/// </summary>
/// <param name="parameterName">The name of the parameter that was <c>null</c>.</param>
/// <param name="message">A custom error message to associate with the null parameter.</param>
public class NullArgumentError(string parameterName, string? message = null)
{
    /// <summary>
    /// Gets the name of the parameter that was detected as <c>null</c>.
    /// </summary>
    public string ParameterName { get; } = parameterName;

    /// <summary>
    /// Gets the descriptive message associated with the null-argument error.
    /// </summary>
    public string? Message { get; } = message ?? $"Parameter '{parameterName}' cannot be null";

    /// <summary>
    /// Returns a string representation of the null-argument error.
    /// </summary>
    /// <returns>A human-readable description of the null parameter failure.</returns>
    public override string ToString() => Message ?? $"Parameter '{ParameterName}' cannot be null";
}
