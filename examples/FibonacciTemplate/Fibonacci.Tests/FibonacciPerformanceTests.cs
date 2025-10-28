using Fibonacci.Core;

namespace Fibonacci.Tests;

/// <summary>
/// Performance tests for Fibonacci calculations.
/// </summary>
public class FibonacciPerformanceTests
{
    private readonly FibonacciCalculator _calculator;

    public FibonacciPerformanceTests()
    {
        _calculator = new FibonacciCalculator();
    }

    [Fact]
    public void CalculateNthIterative_ShouldBeFasterThanRecursive_ForLargeNumbers()
    {
        // Arrange
        const int position = 30; // Large enough to show performance difference

        // Act - Recursive
        var recursiveStopwatch = System.Diagnostics.Stopwatch.StartNew();
        var recursiveResult = _calculator.CalculateNth(position);
        recursiveStopwatch.Stop();

        // Act - Iterative
        var iterativeStopwatch = System.Diagnostics.Stopwatch.StartNew();
        var iterativeResult = _calculator.CalculateNthIterative(position);
        iterativeStopwatch.Stop();

        // Assert
        recursiveResult.ShouldBe(iterativeResult); // Results should be the same
        iterativeStopwatch.ElapsedMilliseconds.ShouldBeLessThan(recursiveStopwatch.ElapsedMilliseconds);
    }

    [Fact]
    public void CalculateSequence_ShouldCompleteWithinReasonableTime_ForLargeTermCount()
    {
        // Arrange
        const int terms = 50;
        const int maxMilliseconds = 1000; // 1 second max

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _calculator.CalculateSequence(terms).ToArray();
        stopwatch.Stop();

        // Assert
        result.Length.ShouldBe(terms);
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(maxMilliseconds);
    }

    [Fact]
    public void CalculateNthIterative_ShouldCompleteWithinReasonableTime_ForLargePosition()
    {
        // Arrange
        const int position = 50;
        const int maxMilliseconds = 100; // 100ms max

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _calculator.CalculateNthIterative(position);
        stopwatch.Stop();

        // Assert
        result.ShouldBeGreaterThan(0);
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(maxMilliseconds);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(30)]
    [InlineData(40)]
    public void CalculateSequence_ShouldScaleLinearly_ForVariousTermCounts(int terms)
    {
        // Arrange
        const int maxMillisecondsPerTerm = 1; // 1ms per term max

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _calculator.CalculateSequence(terms).ToArray();
        stopwatch.Stop();

        // Assert
        result.Length.ShouldBe(terms);
        var averageTimePerTerm = stopwatch.ElapsedMilliseconds / (double)terms;
        averageTimePerTerm.ShouldBeLessThan(maxMillisecondsPerTerm);
    }

    [Fact]
    public void CalculateSequence_ShouldHandleMaxSafeTermCount_WithinReasonableTime()
    {
        // Arrange
        var maxTerms = _calculator.GetMaxSafeTermCount();
        const int maxMilliseconds = 5000; // 5 seconds max

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _calculator.CalculateSequence(maxTerms).ToArray();
        stopwatch.Stop();

        // Assert
        result.Length.ShouldBe(maxTerms);
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(maxMilliseconds);
    }

    [Fact]
    public async Task CalculateSequence_ShouldBeThreadSafe_WhenCalledConcurrently()
    {
        // Arrange
        const int terms = 20;
        const int concurrentTasks = 10;

        // Act
        var tasks = Enumerable.Range(0, concurrentTasks)
            .Select(_ => Task.Run(() => _calculator.CalculateSequence(terms).ToArray()))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Length.ShouldBe(concurrentTasks);
        foreach (var result in results)
        {
            result.Length.ShouldBe(terms);
            result.ShouldNotBeEmpty();
        }

        // All results should be identical
        var firstResult = results[0];
        foreach (var result in results.Skip(1))
        {
            result.ShouldBe(firstResult);
        }
    }
}
