namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// IdentifierRenameRewriter RenamesField.
    /// </summary>
    [Fact]
    public void IdentifierRenameRewriter_RenamesField()
    {
        var code = "class C{ int x; int M(){ return x; } }";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var compilation = CSharpCompilation.Create("test", new[] { tree });
        var model = compilation.GetSemanticModel(tree);
        var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);
        var field = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();
        var symbol = model.GetDeclaredSymbol(field, cancellationToken: TestContext.Current.CancellationToken)!;
        var map = new Dictionary<ISymbol, string>(SymbolEqualityComparer.Default)
        {
            [symbol] = "p"
        };
        var rewriter = new IdentifierRenameRewriter(model, map, null);
        var newRoot = rewriter.Visit(root)!;
        Assert.Contains("p", newRoot.ToFullString());
    }
}
