using Calculator.Core.Models;
using Microsoft.Extensions.Logging;

namespace Calculator.Core.Services;

/// <summary>
/// Implementation of calculator service providing mathematical operations.
/// </summary>
public class CalculatorService : ICalculatorService
{
    private readonly ILogger<CalculatorService> _logger;
    private decimal? _memory;
    private readonly List<string> _history = [];

    /// <summary>
    /// Initializes a new instance of the CalculatorService class.
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public CalculatorService(ILogger<CalculatorService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public CalculationResult Calculate(decimal left, decimal right, OperationType operation)
    {
        try
        {
            _logger.LogInformation("Performing {Operation} on {Left} and {Right}", operation, left, right);

            var result = operation switch
            {
                OperationType.Add => left + right,
                OperationType.Subtract => left - right,
                OperationType.Multiply => left * right,
                OperationType.Divide => Divide(left, right),
                OperationType.Power => Power(left, right),
                OperationType.Percentage => left * (right / 100),
                _ => throw new ArgumentException($"Unsupported binary operation: {operation}", nameof(operation))
            };

            var calculationResult = CalculationResult.Success(result);
            AddToHistory($"{left} {GetOperationSymbol(operation)} {right} = {result}");
            
            _logger.LogInformation("Calculation result: {Result}", result);
            return calculationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing calculation: {Left} {Operation} {Right}", left, operation, right);
            return CalculationResult.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public CalculationResult Calculate(decimal value, OperationType operation)
    {
        try
        {
            _logger.LogInformation("Performing unary {Operation} on {Value}", operation, value);

            var result = operation switch
            {
                OperationType.SquareRoot => SquareRoot(value),
                _ => throw new ArgumentException($"Unsupported unary operation: {operation}", nameof(operation))
            };

            var calculationResult = CalculationResult.Success(result);
            AddToHistory($"{GetOperationSymbol(operation)}({value}) = {result}");
            
            _logger.LogInformation("Unary calculation result: {Result}", result);
            return calculationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing unary calculation: {Operation} {Value}", operation, value);
            return CalculationResult.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public void StoreMemory(decimal value)
    {
        _memory = value;
        _logger.LogInformation("Stored {Value} in memory", value);
    }

    /// <inheritdoc />
    public decimal? RecallMemory()
    {
        _logger.LogInformation("Recalling memory: {Value}", _memory);
        return _memory;
    }

    /// <inheritdoc />
    public void ClearMemory()
    {
        _memory = null;
        _logger.LogInformation("Memory cleared");
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetHistory()
    {
        return _history.AsReadOnly();
    }

    /// <inheritdoc />
    public void ClearHistory()
    {
        _history.Clear();
        _logger.LogInformation("History cleared");
    }

    private static decimal Divide(decimal left, decimal right)
    {
        if (right == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero");
        }
        return left / right;
    }

    private static decimal Power(decimal left, decimal right)
    {
        if (right < 0)
        {
            throw new ArgumentException("Power operation does not support negative exponents", nameof(right));
        }
        
        if (right == 0)
        {
            return 1;
        }

        var result = left;
        for (var i = 1; i < right; i++)
        {
            result *= left;
        }
        return result;
    }

    private static decimal SquareRoot(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Cannot calculate square root of negative number", nameof(value));
        }

        if (value == 0)
        {
            return 0;
        }

        // Simple Newton-Raphson method for square root
        var guess = value / 2;
        for (var i = 0; i < 10; i++) // 10 iterations should be sufficient for most cases
        {
            var newGuess = (guess + value / guess) / 2;
            if (Math.Abs(newGuess - guess) < 0.0000001m)
            {
                return newGuess;
            }
            guess = newGuess;
        }
        return guess;
    }

    private static string GetOperationSymbol(OperationType operation) => operation switch
    {
        OperationType.Add => "+",
        OperationType.Subtract => "-",
        OperationType.Multiply => "*",
        OperationType.Divide => "/",
        OperationType.Power => "^",
        OperationType.SquareRoot => "√",
        OperationType.Percentage => "%",
        _ => "?"
    };

    private void AddToHistory(string entry)
    {
        _history.Add($"{DateTime.Now:HH:mm:ss} - {entry}");
        
        // Keep only the last 100 entries
        if (_history.Count > 100)
        {
            _history.RemoveAt(0);
        }
    }
}
