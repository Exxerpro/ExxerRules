namespace IndFusion.Mcp.Tests.SyntaxWalkers;

/// <summary>
/// Tests for SyntaxWalkers.
/// </summary>
public class ComplexityWalkerTests
{
    /// <summary> ///  ComplexityWalker ComputesComplexityAndDepth. /// </summary>
    [Fact]
    public void ComplexityWalker_ComputesComplexityAndDepth()
    {
        var code = @"class C
{
    void M()
    {
        if (true)
        {
            for (int i = 0; i < 10; i++)
            {
                if (false) { }
            }
        }
    }
}";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var walker = new ComplexityWalker();
        walker.Visit(tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(4, walker.Complexity);
        Assert.Equal(3, walker.MaxDepth);
    }
}
