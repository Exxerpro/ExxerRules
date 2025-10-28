using Calculator.Core.Models;

namespace Calculator.Core.Services;

/// <summary>
/// Defines the contract for calculator operations.
/// </summary>
public interface ICalculatorService
{
    /// <summary>
    /// Performs a binary operation on two numbers.
    /// </summary>
    /// <param name="left">The left operand</param>
    /// <param name="right">The right operand</param>
    /// <param name="operation">The operation to perform</param>
    /// <returns>The result of the calculation</returns>
    CalculationResult Calculate(decimal left, decimal right, OperationType operation);

    /// <summary>
    /// Performs a unary operation on a single number.
    /// </summary>
    /// <param name="value">The operand</param>
    /// <param name="operation">The operation to perform</param>
    /// <returns>The result of the calculation</returns>
    CalculationResult Calculate(decimal value, OperationType operation);

    /// <summary>
    /// Stores a value in memory.
    /// </summary>
    /// <param name="value">The value to store</param>
    void StoreMemory(decimal value);

    /// <summary>
    /// Recalls the value from memory.
    /// </summary>
    /// <returns>The stored value, or null if no value is stored</returns>
    decimal? RecallMemory();

    /// <summary>
    /// Clears the memory.
    /// </summary>
    void ClearMemory();

    /// <summary>
    /// Gets the calculation history.
    /// </summary>
    /// <returns>A read-only list of calculation history entries</returns>
    IReadOnlyList<string> GetHistory();

    /// <summary>
    /// Clears the calculation history.
    /// </summary>
    void ClearHistory();
}
