namespace IndFusion.Mcp.Tests.SyntaxWalkers;

/// <summary>
/// Tests for SyntaxWalkers.
/// </summary>
public class AccessMemberTypeWalkerTests
{
    [Fact]
    public void AccessMemberTypeWalker_FindsField()
    {
        var code = @"class C { int _a; int P { get; set; } }";
        var tree = CSharpSyntaxTree.ParseText(code);
        var walker = new AccessMemberTypeWalker("_a");
        walker.Visit(tree.GetRoot());
        Assert.Equal("field", walker.MemberType);
    }

    [Fact]
    public void AccessMemberTypeWalker_FindsProperty()
    {
        var code = @"class C { int _a; int P { get; set; } }";
        var tree = CSharpSyntaxTree.ParseText(code);
        var walker = new AccessMemberTypeWalker("P");
        walker.Visit(tree.GetRoot());
        Assert.Equal("property", walker.MemberType);
    }
}
