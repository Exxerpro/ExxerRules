namespace IndFusion.Mcp.Tests;

/// <summary>
/// Type ClassLengthMetricsTests : IDisposable
/// </summary>
///<summary>
///Type ClassLengthMetricsTests : IDisposable.
///</summary>
///<summary>
///Type ClassLengthMetricsTests : IDisposable.
///</summary>
///<summary>
///Type ClassLengthMetricsTests : IDisposable.
///</summary>
public class ClassLengthMetricsTests : IDisposable
{
    private static readonly string SolutionPath = TestHelpers.GetSolutionPath();
    private readonly string _originalDir = Directory.GetCurrentDirectory();

    /// <summary>
    /// Dispose.
    /// </summary>
    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalDir);
    }

    /// <summary>
    /// ListClassLengths ReturnsMetrics.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ListClassLengths_ReturnsMetrics()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath);
        var result = await ClassLengthMetricsTool.ListClassLengths(SolutionPath);
        Assert.Contains("Calculator", result);
        Assert.Contains("MathUtilities", result);
    }
}
