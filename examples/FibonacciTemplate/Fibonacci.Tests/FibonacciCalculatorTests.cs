using Fibonacci.Core;

namespace Fibonacci.Tests;

/// <summary>
/// Unit tests for the FibonacciCalculator class.
/// </summary>
public class FibonacciCalculatorTests
{
    private readonly FibonacciCalculator _calculator;

    public FibonacciCalculatorTests()
    {
        _calculator = new FibonacciCalculator();
    }

    [Fact]
    public void CalculateSequence_ShouldReturnCorrectSequence_WhenGivenValidTerms()
    {
        // Arrange
        const int terms = 10;
        var expected = new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 };

        // Act
        var result = _calculator.CalculateSequence(terms).ToArray();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void CalculateSequence_ShouldReturnSingleTerm_WhenGivenOne()
    {
        // Arrange
        const int terms = 1;
        var expected = new long[] { 0 };

        // Act
        var result = _calculator.CalculateSequence(terms).ToArray();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void CalculateSequence_ShouldReturnTwoTerms_WhenGivenTwo()
    {
        // Arrange
        const int terms = 2;
        var expected = new long[] { 0, 1 };

        // Act
        var result = _calculator.CalculateSequence(terms).ToArray();

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void CalculateSequence_ShouldThrowArgumentOutOfRangeException_WhenGivenZero()
    {
        // Arrange
        const int terms = 0;

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateSequence(terms))
            .ParamName.ShouldBe("terms");
    }

    [Fact]
    public void CalculateSequence_ShouldThrowArgumentOutOfRangeException_WhenGivenNegativeTerms()
    {
        // Arrange
        const int terms = -5;

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateSequence(terms))
            .ParamName.ShouldBe("terms");
    }

    [Fact]
    public void CalculateSequence_ShouldThrowArgumentOutOfRangeException_WhenExceedingMaxSafeTerms()
    {
        // Arrange
        const int terms = 100; // Exceeds MaxSafeTermCount (92)

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateSequence(terms))
            .ParamName.ShouldBe("terms");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 3)]
    [InlineData(5, 5)]
    [InlineData(10, 55)]
    public void CalculateNth_ShouldReturnCorrectValue_ForVariousPositions(int position, long expected)
    {
        // Act
        var result = _calculator.CalculateNth(position);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 3)]
    [InlineData(5, 5)]
    [InlineData(10, 55)]
    public void CalculateNthIterative_ShouldReturnCorrectValue_ForVariousPositions(int position, long expected)
    {
        // Act
        var result = _calculator.CalculateNthIterative(position);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void CalculateNth_ShouldThrowArgumentOutOfRangeException_WhenGivenNegativePosition()
    {
        // Arrange
        const int position = -1;

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateNth(position))
            .ParamName.ShouldBe("n");
    }

    [Fact]
    public void CalculateNthIterative_ShouldThrowArgumentOutOfRangeException_WhenGivenNegativePosition()
    {
        // Arrange
        const int position = -1;

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateNthIterative(position))
            .ParamName.ShouldBe("n");
    }

    [Fact]
    public void CalculateNth_ShouldThrowArgumentOutOfRangeException_WhenExceedingMaxSafePosition()
    {
        // Arrange
        const int position = 100; // Exceeds MaxSafeTermCount (92)

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateNth(position))
            .ParamName.ShouldBe("n");
    }

    [Fact]
    public void CalculateNthIterative_ShouldThrowArgumentOutOfRangeException_WhenExceedingMaxSafePosition()
    {
        // Arrange
        const int position = 100; // Exceeds MaxSafeTermCount (92)

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateNthIterative(position))
            .ParamName.ShouldBe("n");
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(10, true)]
    [InlineData(92, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(93, false)]
    [InlineData(100, false)]
    public void IsValidTermCount_ShouldReturnCorrectResult_ForVariousInputs(int terms, bool expected)
    {
        // Act
        var result = _calculator.IsValidTermCount(terms);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void GetMaxSafeTermCount_ShouldReturnCorrectValue()
    {
        // Act
        var result = _calculator.GetMaxSafeTermCount();

        // Assert
        result.ShouldBe(92);
    }

    [Fact]
    public void CalculateNth_And_CalculateNthIterative_ShouldReturnSameResults_ForSameInputs()
    {
        // Arrange
        var positions = new[] { 0, 1, 2, 3, 4, 5, 10, 15, 20 };

        foreach (var position in positions)
        {
            // Act
            var recursiveResult = _calculator.CalculateNth(position);
            var iterativeResult = _calculator.CalculateNthIterative(position);

            // Assert
            recursiveResult.ShouldBe(iterativeResult, 
                $"Results should match for position {position}");
        }
    }

    [Fact]
    public void CalculateSequence_ShouldReturnCorrectLength_ForVariousTermCounts()
    {
        // Arrange
        var termCounts = new[] { 1, 5, 10, 20, 50 };

        foreach (var termCount in termCounts)
        {
            // Act
            var result = _calculator.CalculateSequence(termCount).ToArray();

            // Assert
            result.Length.ShouldBe(termCount, 
                $"Sequence length should match term count for {termCount} terms");
        }
    }
}
