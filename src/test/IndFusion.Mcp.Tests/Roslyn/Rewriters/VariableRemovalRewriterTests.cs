using System.Threading;

namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// VariableRemovalRewriter RemovesVariable.
    /// </summary>
    [Fact]
    public void VariableRemovalRewriter_RemovesVariable()
    {
        var tree = CSharpSyntaxTree.ParseText("void M(){int x=0;}", cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);
        var varNode = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();
        var rewriter = new VariableRemovalRewriter("x", varNode.Span);
        var newRoot = Formatter.Format(rewriter.Visit(root)!, new AdhocWorkspace(), cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("void M() { }", newRoot.ToFullString().Trim());
    }
}
