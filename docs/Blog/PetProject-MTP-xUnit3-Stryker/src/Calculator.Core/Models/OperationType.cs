namespace Calculator.Core.Models;

/// <summary>
/// Represents the type of mathematical operation.
/// </summary>
public enum OperationType
{
    /// <summary>
    /// Addition operation
    /// </summary>
    Add,

    /// <summary>
    /// Subtraction operation
    /// </summary>
    Subtract,

    /// <summary>
    /// Multiplication operation
    /// </summary>
    Multiply,

    /// <summary>
    /// Division operation
    /// </summary>
    Divide,

    /// <summary>
    /// Power operation (exponentiation)
    /// </summary>
    Power,

    /// <summary>
    /// Square root operation
    /// </summary>
    SquareRoot,

    /// <summary>
    /// Percentage calculation
    /// </summary>
    Percentage
}
