using Calculator;

namespace Calculator.Tests;

/// <summary>
/// Integration tests for calculator operations using real dependencies.
/// </summary>
public class CalculatorIntegrationTests
{
    private readonly ICalculator _calculator;

    public CalculatorIntegrationTests()
    {
        _calculator = new BasicCalculator();
    }

    [Fact]
    public void Calculator_ShouldImplementICalculatorInterface()
    {
        // Assert
        _calculator.ShouldBeAssignableTo<ICalculator>();
    }

    [Fact]
    public async Task Calculator_ShouldHandleConcurrentOperations_WhenMultipleThreadsAccessIt()
    {
        // Arrange
        const int numberOfOperations = 100;
        var tasks = new List<Task<double>>();

        // Act
        for (int i = 0; i < numberOfOperations; i++)
        {
            var task = Task.Run(() => _calculator.Add(i, i * 2));
            tasks.Add(task);
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        results.ShouldNotBeEmpty();
        results.Length.ShouldBe(numberOfOperations);
        
        // Verify each result is correct
        for (int i = 0; i < numberOfOperations; i++)
        {
            results[i].ShouldBe(i + (i * 2));
        }
    }

    [Fact]
    public void Calculator_ShouldMaintainPrecision_WhenPerformingFloatingPointOperations()
    {
        // Arrange
        var a = 0.1;
        var b = 0.2;
        var expected = 0.3;

        // Act
        var result = _calculator.Add(a, b);

        // Assert
        result.ShouldBe(expected, 0.0000001); // Allow for floating point precision
    }

    [Fact]
    public void Calculator_ShouldHandleEdgeCases_WhenPerformingDivision()
    {
        // Arrange
        var verySmallNumber = double.Epsilon;
        var largeNumber = double.MaxValue / 2;

        // Act
        var result = _calculator.Divide(largeNumber, verySmallNumber);

        // Assert
        result.ShouldNotBe(double.NaN);
        result.ShouldNotBe(double.PositiveInfinity);
        result.ShouldNotBe(double.NegativeInfinity);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(0, 1, 1)]
    [InlineData(-1, 1, 0)]
    [InlineData(1, -1, 0)]
    public void Calculator_ShouldHandleZeroValues_WhenPerformingAddition(double a, double b, double expected)
    {
        // Act
        var result = _calculator.Add(a, b);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Calculator_ShouldHandleLargeNumbers_WhenPerformingPowerOperation()
    {
        // Arrange
        var baseNumber = 2.0;
        var exponent = 10.0;
        var expected = 1024.0;

        // Act
        var result = _calculator.Power(baseNumber, exponent);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void Calculator_ShouldHandleNegativeExponents_WhenPerformingPowerOperation()
    {
        // Arrange
        var baseNumber = 2.0;
        var exponent = -2.0;
        var expected = 0.25;

        // Act
        var result = _calculator.Power(baseNumber, exponent);

        // Assert
        result.ShouldBe(expected);
    }
}
