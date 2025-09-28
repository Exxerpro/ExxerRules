namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// MethodRemovalRewriter RemovesMethod.
    /// </summary>
    [Fact]
    public void MethodRemovalRewriter_RemovesMethod()
    {
        var tree = CSharpSyntaxTree.ParseText("class A{void M(){}}", cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);
        var rewriter = new MethodRemovalRewriter("M");
        var newRoot = Formatter.Format(rewriter.Visit(root)!, new AdhocWorkspace(), cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("class A { }", newRoot.ToFullString().Trim());
    }
}
