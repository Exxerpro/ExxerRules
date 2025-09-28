namespace IndFusion.Mcp.Tests.SyntaxWalkers;

/// <summary>
/// Tests for SyntaxWalkers.
/// </summary>
public class PrivateFieldInfoWalkerTests
{
    /// <summary> ///  PrivateFieldInfoWalker CollectsPrivateFields. /// </summary>
    [Fact]
    public void PrivateFieldInfoWalker_CollectsPrivateFields()
    {
        var code = @"class C { private int a; string b; private string c; }";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var walker = new PrivateFieldInfoWalker();
        walker.Visit(tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(2, walker.Infos.Count);
        Assert.Contains("a", walker.Infos.Keys);
        Assert.Contains("c", walker.Infos.Keys);
    }
}
