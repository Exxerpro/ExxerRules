namespace IndFusion.Mcp.Tests.Roslyn;

/// <summary>
/// Tests for Roslyn.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary> ///  IntroduceVariableInSource AddsVariable. /// </summary>
    [Fact]
    public void IntroduceVariableInSource_AddsVariable()
    {
        var input = @"class Calculator
{
    int Calculate()
    {
        return (1 + 2) * 3;
    }
}";
        var expected = @"class Calculator
{
    int Calculate()
    {
        var sum = 1 + 2;
        return sum * 3;
    }
}";
        var output = IntroduceVariableTool.IntroduceVariableInSource(input, "5:17-5:21", "sum");
        Assert.Equal(expected, output);
    }

    /// <summary> ///  IntroduceVariableInSource ReplacesAllOccurrences. /// </summary>
    [Fact]
    public void IntroduceVariableInSource_ReplacesAllOccurrences()
    {
        var input = @"class Calculator
{
    int Calculate()
    {
        return (1 + 2) * (1 + 2);
    }
}";
        var expected = @"class Calculator
{
    int Calculate()
    {
        var sum = 1 + 2;
        return sum * sum;
    }
}";
        var output = IntroduceVariableTool.IntroduceVariableInSource(input, "5:17-5:21", "sum");
        Assert.Equal(expected, output);
    }
}
