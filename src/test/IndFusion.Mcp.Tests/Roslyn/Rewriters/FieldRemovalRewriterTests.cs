using System.Threading;

namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// FieldRemovalRewriter RemovesField.
    /// </summary>
    [Fact]
    public void FieldRemovalRewriter_RemovesField()
    {
        var tree = CSharpSyntaxTree.ParseText("class A{int x;}", cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);
        var rewriter = new FieldRemovalRewriter("x");
        var newRoot = Formatter.Format(rewriter.Visit(root)!, new AdhocWorkspace(), cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("class A { }", newRoot.ToFullString().Trim());
    }
}
