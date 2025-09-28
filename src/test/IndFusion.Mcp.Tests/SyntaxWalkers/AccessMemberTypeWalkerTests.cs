namespace IndFusion.Mcp.Tests.SyntaxWalkers;

/// <summary>
/// Tests for SyntaxWalkers.
/// </summary>
public class AccessMemberTypeWalkerTests
{
    /// <summary> ///  AccessMemberTypeWalker FindsField. /// </summary>
    [Fact]
    public void AccessMemberTypeWalker_FindsField()
    {
        var code = @"class C { int _a; int P { get; set; } }";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var walker = new AccessMemberTypeWalker("_a");
        walker.Visit(tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal("field", walker.MemberType);
    }

    /// <summary> ///  AccessMemberTypeWalker FindsProperty. /// </summary>
    [Fact]
    public void AccessMemberTypeWalker_FindsProperty()
    {
        var code = @"class C { int _a; int P { get; set; } }";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var walker = new AccessMemberTypeWalker("P");
        walker.Visit(tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal("property", walker.MemberType);
    }
}
