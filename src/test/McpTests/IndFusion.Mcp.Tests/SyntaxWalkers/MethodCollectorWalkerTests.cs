namespace IndFusion.Mcp.Tests.SyntaxWalkers;

/// <summary>
/// Tests for SyntaxWalkers.
/// </summary>
public class MethodCollectorWalkerTests
{
    /// <summary> ///  MethodCollectorWalker CollectsSpecifiedMethods. /// </summary>
    [Fact]
    public void MethodCollectorWalker_CollectsSpecifiedMethods()
    {
        var code = @"class A { void X(){} void Y(){} } class B { void X(){} }";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var walker = new MethodCollectorWalker(["A.X", "B.X"]);
        walker.Visit(tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Contains("A.X", walker.Methods.Keys);
        Assert.Contains("B.X", walker.Methods.Keys);
        Assert.DoesNotContain("A.Y", walker.Methods.Keys);
    }
}
