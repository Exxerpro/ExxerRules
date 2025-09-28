namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary> ///  InstanceMemberRewriter QualifiesMembers. /// </summary>
    [Fact]
    public void InstanceMemberRewriter_QualifiesMembers()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test(){ Value = 1; }") as MethodDeclarationSyntax;
        var rewriter = new InstanceMemberRewriter("inst", new HashSet<string> { "Value" });
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("inst.Value", result);
    }

    /// <summary> ///  InstanceMemberRewriter QualifiesThisMember. /// </summary>
    [Fact]
    public void InstanceMemberRewriter_QualifiesThisMember()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test(){ var x = this.Value; }") as MethodDeclarationSyntax;
        var rewriter = new InstanceMemberRewriter("inst", new HashSet<string> { "Value" });
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("inst.Value", result);
        Assert.DoesNotContain("this.Value", result);
    }

    /// <summary> ///  InstanceMemberRewriter IgnoresObjectInitializerPropertyNames. /// </summary>
    [Fact]
    public void InstanceMemberRewriter_IgnoresObjectInitializerPropertyNames()
    {
        var method = SyntaxFactory.ParseMemberDeclaration(@"void Test(){ var h = new RequestHeaders{ Username = strCurrentOperatorCode, SiteId = ConnectedSiteID, GroupId = ConnectedGroupID }; }") as MethodDeclarationSyntax;
        var members = new HashSet<string> { "strCurrentOperatorCode", "ConnectedSiteID", "ConnectedGroupID" };
        var rewriter = new InstanceMemberRewriter("inst", members);
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("Username = inst.strCurrentOperatorCode", result);
        Assert.Contains("SiteId = inst.ConnectedSiteID", result);
        Assert.Contains("GroupId = inst.ConnectedGroupID", result);
        Assert.DoesNotContain("inst.Username", result);
        Assert.DoesNotContain("inst.SiteId", result);
        Assert.DoesNotContain("inst.GroupId", result);
    }

    /// <summary> ///  InstanceMemberRewriter QualifiesBasePropertyAccess. /// </summary>
    [Fact]
    public void InstanceMemberRewriter_QualifiesBasePropertyAccess()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test(){ var n = base.Value; }") as MethodDeclarationSyntax;
        var rewriter = new InstanceMemberRewriter("inst", new HashSet<string> { "Value" });
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("inst.Value", result);
        Assert.DoesNotContain("base.Value", result);
    }

    /// <summary> ///  InstanceMemberRewriter IgnoresPropertyPatternNames. /// </summary>
    [Fact]
    public void InstanceMemberRewriter_IgnoresPropertyPatternNames()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test(){ if(this is { Value: > 0 }) { } }") as MethodDeclarationSyntax;
        var rewriter = new InstanceMemberRewriter("inst", new HashSet<string> { "Value" });
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("this is { Value: > 0 }", result);
        Assert.DoesNotContain("inst.Value", result);
    }
}
