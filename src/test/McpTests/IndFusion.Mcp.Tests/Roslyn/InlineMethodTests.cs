namespace IndFusion.Mcp.Tests.Roslyn;

/// <summary>
/// Roslyn transformation tests for inlining a method into its call site.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// Replaces an invocation with the body of the inlined method.
    /// </summary>
    [Fact]
    public void InlineMethodInSource_ReplacesInvocationWithBody()
    {
        var input = @"class InlineSample
{
    private void Helper()
    {
        Console.WriteLine(""Hi"");
    }

    public void Call()
    {
        Helper();
        Console.WriteLine(""Done"");
    }
}";
        var expected = "class InlineSample\n{\n\n    public void Call()\n    {\n        Console.WriteLine(\"Hi\");\n        Console.WriteLine(\"Done\");\n    }\n}";
        var output = InlineMethodTool.InlineMethodInSource(input, "Helper");
        Assert.Equal(expected, output.Trim());
    }
}
