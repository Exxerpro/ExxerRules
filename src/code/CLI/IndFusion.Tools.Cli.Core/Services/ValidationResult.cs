namespace IndFusion.Tools.Cli.Core.Services;

/// <summary>
/// Represents the result of request validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets whether the validation passed
    /// </summary>
    public bool IsValid { get; private set; }

    /// <summary>
    /// Gets the error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Creates a valid validation result
    /// </summary>
    /// <returns>Valid validation result</returns>
    public static ValidationResult Valid()
    {
        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Creates an invalid validation result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Invalid validation result</returns>
    public static ValidationResult Invalid(string errorMessage)
    {
        return new ValidationResult { IsValid = false, ErrorMessage = errorMessage };
    }
}