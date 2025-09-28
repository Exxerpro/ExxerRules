namespace IndFusion.Analyzers.Operations;

/// <summary>
/// Represents a null argument validation error
/// </summary>
/// <remarks>
/// Initializes a new instance of NullArgumentError
/// </remarks>
/// <param name="parameterName">Name of the null parameter</param>
/// <param name="message">Optional error message</param>
public class NullArgumentError(string parameterName, string? message = null)
{
    /// <summary>
    /// Name of the parameter that was null
    /// </summary>
    public string ParameterName { get; } = parameterName;

    /// <summary>
    /// Optional message describing the error
    /// </summary>
    public string? Message { get; } = message ?? $"Parameter '{parameterName}' cannot be null";

    /// <summary>
    /// Returns string representation of the error
    /// </summary>
    public override string ToString() => Message ?? $"Parameter '{ParameterName}' cannot be null";
}
