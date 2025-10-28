using Fibonacci.Core;

namespace Fibonacci.Tests;

/// <summary>
/// Data-driven tests for Fibonacci calculations using various test data sources.
/// </summary>
public class FibonacciDataDrivenTests
{
    private readonly FibonacciCalculator _calculator;

    public FibonacciDataDrivenTests()
    {
        _calculator = new FibonacciCalculator();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 3)]
    [InlineData(5, 5)]
    [InlineData(6, 8)]
    [InlineData(7, 13)]
    [InlineData(8, 21)]
    [InlineData(9, 34)]
    [InlineData(10, 55)]
    public void CalculateNth_ShouldReturnCorrectValue_ForKnownPositions(int position, long expected)
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
    [InlineData(6, 8)]
    [InlineData(7, 13)]
    [InlineData(8, 21)]
    [InlineData(9, 34)]
    [InlineData(10, 55)]
    public void CalculateNthIterative_ShouldReturnCorrectValue_ForKnownPositions(int position, long expected)
    {
        // Act
        var result = _calculator.CalculateNthIterative(position);

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [MemberData(nameof(GetFibonacciSequenceData))]
    public void CalculateSequence_ShouldReturnCorrectSequence_ForVariousTermCounts(int terms, long[] expected)
    {
        // Act
        var result = _calculator.CalculateSequence(terms).ToArray();

        // Assert
        result.ShouldBe(expected);
    }

    [Theory]
    [MemberData(nameof(GetInvalidTermCountData))]
    public void CalculateSequence_ShouldThrowArgumentOutOfRangeException_ForInvalidTermCounts(int terms)
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateSequence(terms))
            .ParamName.ShouldBe("terms");
    }

    [Theory]
    [MemberData(nameof(GetInvalidPositionData))]
    public void CalculateNth_ShouldThrowArgumentOutOfRangeException_ForInvalidPositions(int position)
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateNth(position))
            .ParamName.ShouldBe("n");
    }

    [Theory]
    [MemberData(nameof(GetInvalidPositionData))]
    public void CalculateNthIterative_ShouldThrowArgumentOutOfRangeException_ForInvalidPositions(int position)
    {
        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => _calculator.CalculateNthIterative(position))
            .ParamName.ShouldBe("n");
    }

    [Theory]
    [MemberData(nameof(GetValidTermCountData))]
    public void IsValidTermCount_ShouldReturnTrue_ForValidTermCounts(int terms)
    {
        // Act
        var result = _calculator.IsValidTermCount(terms);

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidTermCountData))]
    public void IsValidTermCount_ShouldReturnFalse_ForInvalidTermCounts(int terms)
    {
        // Act
        var result = _calculator.IsValidTermCount(terms);

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(GetPerformanceTestData))]
    public void CalculateSequence_ShouldCompleteWithinTimeLimit_ForVariousTermCounts(int terms, int maxMilliseconds)
    {
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _calculator.CalculateSequence(terms).ToArray();
        stopwatch.Stop();

        // Assert
        result.Length.ShouldBe(terms);
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(maxMilliseconds);
    }

    /// <summary>
    /// Test data for Fibonacci sequence calculations.
    /// </summary>
    public static IEnumerable<object[]> GetFibonacciSequenceData()
    {
        yield return new object[] { 1, new long[] { 0 } };
        yield return new object[] { 2, new long[] { 0, 1 } };
        yield return new object[] { 3, new long[] { 0, 1, 1 } };
        yield return new object[] { 4, new long[] { 0, 1, 1, 2 } };
        yield return new object[] { 5, new long[] { 0, 1, 1, 2, 3 } };
        yield return new object[] { 6, new long[] { 0, 1, 1, 2, 3, 5 } };
        yield return new object[] { 7, new long[] { 0, 1, 1, 2, 3, 5, 8 } };
        yield return new object[] { 8, new long[] { 0, 1, 1, 2, 3, 5, 8, 13 } };
        yield return new object[] { 9, new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21 } };
        yield return new object[] { 10, new long[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 } };
    }

    /// <summary>
    /// Test data for invalid term counts.
    /// </summary>
    public static IEnumerable<object[]> GetInvalidTermCountData()
    {
        yield return new object[] { 0 };
        yield return new object[] { -1 };
        yield return new object[] { -10 };
        yield return new object[] { 93 }; // Exceeds MaxSafeTermCount (92)
        yield return new object[] { 100 };
        yield return new object[] { int.MaxValue };
        yield return new object[] { int.MinValue };
    }

    /// <summary>
    /// Test data for invalid positions.
    /// </summary>
    public static IEnumerable<object[]> GetInvalidPositionData()
    {
        yield return new object[] { -1 };
        yield return new object[] { -10 };
        yield return new object[] { 93 }; // Exceeds MaxSafeTermCount (92)
        yield return new object[] { 100 };
        yield return new object[] { int.MaxValue };
        yield return new object[] { int.MinValue };
    }

    /// <summary>
    /// Test data for valid term counts.
    /// </summary>
    public static IEnumerable<object[]> GetValidTermCountData()
    {
        for (int i = 1; i <= 92; i++)
        {
            yield return new object[] { i };
        }
    }

    /// <summary>
    /// Test data for performance tests.
    /// </summary>
    public static IEnumerable<object[]> GetPerformanceTestData()
    {
        yield return new object[] { 10, 100 }; // 10 terms, max 100ms
        yield return new object[] { 20, 200 }; // 20 terms, max 200ms
        yield return new object[] { 30, 500 }; // 30 terms, max 500ms
        yield return new object[] { 50, 1000 }; // 50 terms, max 1000ms
    }
}
