namespace IndFusion.Mcp.Tests.Roslyn;

/// <summary>
/// Tests for Roslyn.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary> ///  IntroduceParameterInSource AddsParameter. /// </summary>
    [Fact]
    public void IntroduceParameterInSource_AddsParameter()
    {
        var input = @"class MathHelper
{
    int AddNumbers()
    {
        return 1 + 2;
    }

    int Calculate()
    {
        return AddNumbers();
    }
}";
        var expected = @"class MathHelper
{
    int AddNumbers(object result)
    {
        return result;
    }

    int Calculate()
    {
        return AddNumbers(1 + 2);
    }
}";
        var output = IntroduceParameterTool.IntroduceParameterInSource(input, "AddNumbers", "5:16-5:20", "result");
        Assert.Equal(expected, output);
    }
}
