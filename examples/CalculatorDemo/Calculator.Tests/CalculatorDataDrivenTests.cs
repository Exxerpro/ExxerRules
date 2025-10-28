using Calculator;

namespace Calculator.Tests;

/// <summary>
/// Data-driven tests demonstrating XUnit v3 Theory and InlineData capabilities.
/// </summary>
public class CalculatorDataDrivenTests
{
    private readonly BasicCalculator _calculator = new();

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 2)]
    [InlineData(-1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, -1, -2)]
    [InlineData(1.5, 2.5, 4.0)]
    [InlineData(-3.7, 2.3, -1.4)]
    [InlineData(0.1, 0.2, 0.3)]
    public void Add_ShouldReturnCorrectSum_ForVariousInputs(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Add(a, b);

        // Assert
        result.ShouldBe(expected, 0.0000001); // Allow for floating point precision
    }

    [Theory]
    [InlineData(10, 2, 5)]
    [InlineData(15, 3, 5)]
    [InlineData(100, 4, 25)]
    [InlineData(7.5, 2.5, 3)]
    [InlineData(1, 1, 1)]
    [InlineData(0, 1, 0)]
    public void Divide_ShouldReturnCorrectQuotient_ForValidInputs(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Divide(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(4, 2)]
    [InlineData(9, 3)]
    [InlineData(25, 5)]
    [InlineData(144, 12)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    public void SquareRoot_ShouldReturnCorrectResult_ForPerfectSquares(double number, double expected)
    {
        // Act
        var result = _calculator.SquareRoot(number);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(2, 3, 8)]
    [InlineData(3, 2, 9)]
    [InlineData(5, 2, 25)]
    [InlineData(2, 10, 1024)]
    [InlineData(10, 0, 1)]
    [InlineData(2, -1, 0.5)]
    public void Power_ShouldReturnCorrectResult_ForVariousExponents(double baseNumber, double exponent, double expected)
    {
        // Act
        var result = _calculator.Power(baseNumber, exponent);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(0, 1, -1)]
    [InlineData(10, 5, 5)]
    [InlineData(5, 10, -5)]
    [InlineData(-5, -3, -2)]
    public void Subtract_ShouldReturnCorrectDifference_ForVariousInputs(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Subtract(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(5, 4, 20)]
    [InlineData(-3, 4, -12)]
    [InlineData(-2, -3, 6)]
    [InlineData(2.5, 4, 10)]
    public void Multiply_ShouldReturnCorrectProduct_ForVariousInputs(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Multiply(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    // Test cases for edge cases and error conditions
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(double.MinValue)]
    public void SquareRoot_ShouldThrowArgumentException_ForNegativeNumbers(double number)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => _calculator.SquareRoot(number))
            .ParamName.ShouldBe("number");
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(5, 0)]
    [InlineData(-10, 0)]
    [InlineData(0, 0)]
    public void Divide_ShouldThrowDivideByZeroException_WhenDivisorIsZero(double a, double b)
    {
        // Act & Assert
        Should.Throw<DivideByZeroException>(() => _calculator.Divide(a, b));
    }
}
