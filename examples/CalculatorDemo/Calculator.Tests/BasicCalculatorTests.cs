using Calculator;

namespace Calculator.Tests;

/// <summary>
/// Unit tests for the BasicCalculator class.
/// </summary>
public class BasicCalculatorTests
{
    private readonly BasicCalculator _calculator;

    public BasicCalculatorTests()
    {
        _calculator = new BasicCalculator();
    }

    [Fact]
    public void Add_ShouldReturnCorrectSum_WhenGivenTwoNumbers()
    {
        // Arrange
        var a = 5.0;
        var b = 3.0;
        var expected = 8.0;

        // Act
        var result = _calculator.Add(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Subtract_ShouldReturnCorrectDifference_WhenGivenTwoNumbers()
    {
        // Arrange
        var a = 10.0;
        var b = 4.0;
        var expected = 6.0;

        // Act
        var result = _calculator.Subtract(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Multiply_ShouldReturnCorrectProduct_WhenGivenTwoNumbers()
    {
        // Arrange
        var a = 6.0;
        var b = 7.0;
        var expected = 42.0;

        // Act
        var result = _calculator.Multiply(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Divide_ShouldReturnCorrectQuotient_WhenGivenTwoNumbers()
    {
        // Arrange
        var a = 15.0;
        var b = 3.0;
        var expected = 5.0;

        // Act
        var result = _calculator.Divide(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Divide_ShouldThrowDivideByZeroException_WhenDivisorIsZero()
    {
        // Arrange
        var a = 10.0;
        var b = 0.0;

        // Act & Assert
        Should.Throw<DivideByZeroException>(() => _calculator.Divide(a, b))
            .Message.ShouldBe("Cannot divide by zero.");
    }

    [Fact]
    public void Power_ShouldReturnCorrectResult_WhenGivenBaseAndExponent()
    {
        // Arrange
        var baseNumber = 2.0;
        var exponent = 3.0;
        var expected = 8.0;

        // Act
        var result = _calculator.Power(baseNumber, exponent);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void SquareRoot_ShouldReturnCorrectResult_WhenGivenPositiveNumber()
    {
        // Arrange
        var number = 16.0;
        var expected = 4.0;

        // Act
        var result = _calculator.SquareRoot(number);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void SquareRoot_ShouldThrowArgumentException_WhenGivenNegativeNumber()
    {
        // Arrange
        var number = -4.0;

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => _calculator.SquareRoot(number));
        exception.ParamName.ShouldBe("number");
        exception.Message.ShouldContain("Cannot calculate square root of negative number");
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 2)]
    [InlineData(-1, 1, 0)]
    [InlineData(1.5, 2.5, 4.0)]
    [InlineData(-3.7, 2.3, -1.4)]
    public void Add_ShouldReturnCorrectSum_WhenGivenVariousNumbers(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Add(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(10, 2, 5)]
    [InlineData(15, 3, 5)]
    [InlineData(100, 4, 25)]
    [InlineData(7.5, 2.5, 3)]
    public void Divide_ShouldReturnCorrectQuotient_WhenGivenVariousNumbers(double a, double b, double expected)
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
    public void SquareRoot_ShouldReturnCorrectResult_WhenGivenPerfectSquares(double number, double expected)
    {
        // Act
        var result = _calculator.SquareRoot(number);

        // Assert
        result.ShouldBe(expected);
    }
}
