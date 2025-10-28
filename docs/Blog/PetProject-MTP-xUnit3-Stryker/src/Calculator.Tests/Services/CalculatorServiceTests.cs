using Calculator.Core.Models;
using Calculator.Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Calculator.Tests.Services;

/// <summary>
/// Unit tests for CalculatorService using xUnit v3 and MTP 2.0.
/// </summary>
public class CalculatorServiceTests
{
    private readonly ILogger<CalculatorService> _logger;
    private readonly CalculatorService _calculator;

    public CalculatorServiceTests()
    {
        _logger = Substitute.For<ILogger<CalculatorService>>();
        _calculator = new CalculatorService(_logger);
    }

    [Fact]
    public void Calculate_Add_ShouldReturnCorrectResult()
    {
        // Arrange
        const decimal left = 5.5m;
        const decimal right = 3.2m;
        const decimal expected = 8.7m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Add);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
        result.ErrorMessage.ShouldBeNull();
    }

    [Theory]
    [InlineData(3.0, 3.0, 6.0)]
    [InlineData(3, 3, 6)]
    public void Calculate_Add_ShouldReturnCorrectResult_WithSeveralValues(decimal left, decimal right, decimal expected)
    {
        // Act
        var result = _calculator.Calculate(left, right, OperationType.Add);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void Calculate_Subtract_ShouldReturnCorrectResult()
    {
        // Arrange
        const decimal left = 10.0m;
        const decimal right = 3.5m;
        const decimal expected = 6.5m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Subtract);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void Calculate_Multiply_ShouldReturnCorrectResult()
    {
        // Arrange
        const decimal left = 4.0m;
        const decimal right = 2.5m;
        const decimal expected = 10.0m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Multiply);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void Calculate_Divide_ShouldReturnCorrectResult()
    {
        // Arrange
        const decimal left = 15.0m;
        const decimal right = 3.0m;
        const decimal expected = 5.0m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Divide);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void Calculate_DivideByZero_ShouldReturnFailure()
    {
        // Arrange
        const decimal left = 10.0m;
        const decimal right = 0.0m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Divide);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("divide by zero");
    }

    [Fact]
    public void Calculate_Power_ShouldReturnCorrectResult()
    {
        // Arrange
        const decimal left = 2.0m;
        const decimal right = 3.0m;
        const decimal expected = 8.0m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Power);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void Calculate_PowerWithZeroExponent_ShouldReturnOne()
    {
        // Arrange
        const decimal left = 5.0m;
        const decimal right = 0.0m;
        const decimal expected = 1.0m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Power);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void Calculate_PowerWithNegativeExponent_ShouldReturnFailure()
    {
        // Arrange
        const decimal left = 2.0m;
        const decimal right = -1.0m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Power);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("negative exponents");
    }

    [Fact]
    public void Calculate_SquareRoot_ShouldReturnCorrectResult()
    {
        // Arrange
        const decimal value = 16.0m;
        const decimal expected = 4.0m;

        // Act
        var result = _calculator.Calculate(value, OperationType.SquareRoot);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected, 0.0001m); // Allow for small precision differences
    }

    [Fact]
    public void Calculate_SquareRootOfZero_ShouldReturnZero()
    {
        // Arrange
        const decimal value = 0.0m;
        const decimal expected = 0.0m;

        // Act
        var result = _calculator.Calculate(value, OperationType.SquareRoot);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public void Calculate_SquareRootOfNegativeNumber_ShouldReturnFailure()
    {
        // Arrange
        const decimal value = -4.0m;

        // Act
        var result = _calculator.Calculate(value, OperationType.SquareRoot);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("negative number");
    }

    [Fact]
    public void Calculate_Percentage_ShouldReturnCorrectResult()
    {
        // Arrange
        const decimal left = 20.0m;
        const decimal right = 100.0m;
        const decimal expected = 20.0m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.Percentage);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData(5.0, 3.0, 8.0)]
    [InlineData(-2.0, 7.0, 5.0)]
    [InlineData(0.0, 0.0, 0.0)]
    [InlineData(1.5, 2.5, 4.0)]
    public void Calculate_Add_WithVariousInputs_ShouldReturnCorrectResults(decimal left, decimal right, decimal expected)
    {
        // Act
        var result = _calculator.Calculate(left, right, OperationType.Add);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData(4.0, 2.0)]
    [InlineData(9.0, 3.0)]
    [InlineData(25.0, 5.0)]
    [InlineData(100.0, 10.0)]
    public void Calculate_SquareRoot_WithPerfectSquares_ShouldReturnCorrectResults(decimal value, decimal expected)
    {
        // Act
        var result = _calculator.Calculate(value, OperationType.SquareRoot);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected, 0.0001m);
    }

    [Fact]
    public void StoreMemory_ShouldStoreValue()
    {
        // Arrange
        const decimal value = 42.5m;

        // Act
        _calculator.StoreMemory(value);

        // Assert
        var recalled = _calculator.RecallMemory();
        recalled.ShouldBe(value);
    }

    [Fact]
    public void RecallMemory_WhenEmpty_ShouldReturnNull()
    {
        // Act
        var result = _calculator.RecallMemory();

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void ClearMemory_ShouldClearStoredValue()
    {
        // Arrange
        _calculator.StoreMemory(10.0m);

        // Act
        _calculator.ClearMemory();

        // Assert
        var result = _calculator.RecallMemory();
        result.ShouldBeNull();
    }

    [Fact]
    public void GetHistory_ShouldReturnCalculationHistory()
    {
        // Arrange
        _calculator.Calculate(5.0m, 3.0m, OperationType.Add);
        _calculator.Calculate(10.0m, 2.0m, OperationType.Multiply);

        // Act
        var history = _calculator.GetHistory();

        // Assert
        history.ShouldNotBeEmpty();
        history.Count.ShouldBe(2);
        history[0].ShouldContain("5.0 + 3.0 = 8.0");
        history[1].ShouldContain("10.0 * 2.0 = 20.0");
    }

    [Fact]
    public void ClearHistory_ShouldClearAllHistory()
    {
        // Arrange
        _calculator.Calculate(5.0m, 3.0m, OperationType.Add);
        _calculator.Calculate(10.0m, 2.0m, OperationType.Multiply);

        // Act
        _calculator.ClearHistory();

        // Assert
        var history = _calculator.GetHistory();
        history.ShouldBeEmpty();
    }

    [Fact]
    public void Calculate_UnsupportedBinaryOperation_ShouldReturnFailure()
    {
        // Arrange
        const decimal left = 5.0m;
        const decimal right = 3.0m;

        // Act
        var result = _calculator.Calculate(left, right, OperationType.SquareRoot);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("Unsupported binary operation");
    }

    [Fact]
    public void Calculate_UnsupportedUnaryOperation_ShouldReturnFailure()
    {
        // Arrange
        const decimal value = 5.0m;

        // Act
        var result = _calculator.Calculate(value, OperationType.Add);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("Unsupported unary operation");
    }
}