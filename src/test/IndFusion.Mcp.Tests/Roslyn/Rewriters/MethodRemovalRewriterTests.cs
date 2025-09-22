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
        var tree = CSharpSyntaxTree.ParseText("class A{void M(){}}");
        var root = tree.GetRoot();
        var rewriter = new MethodRemovalRewriter("M");
        var newRoot = Formatter.Format(rewriter.Visit(root)!, new AdhocWorkspace());
        Assert.Equal("class A { }", newRoot.ToFullString().Trim());
    }
}
