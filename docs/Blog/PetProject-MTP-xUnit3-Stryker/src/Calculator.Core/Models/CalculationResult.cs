namespace Calculator.Core.Models;

/// <summary>
/// Represents the result of a calculation operation.
/// </summary>
/// <param name="Value">The calculated value</param>
/// <param name="IsSuccess">Indicates if the calculation was successful</param>
/// <param name="ErrorMessage">Error message if calculation failed</param>
public readonly record struct CalculationResult(
    decimal Value,
    bool IsSuccess,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Creates a successful calculation result.
    /// </summary>
    /// <param name="value">The calculated value</param>
    /// <returns>A successful calculation result</returns>
    public static CalculationResult Success(decimal value) => new(value, true);

    /// <summary>
    /// Creates a failed calculation result.
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    /// <returns>A failed calculation result</returns>
    public static CalculationResult Failure(string errorMessage) => new(0, false, errorMessage);

    /// <summary>
    /// Gets a value indicating whether the calculation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;
}
