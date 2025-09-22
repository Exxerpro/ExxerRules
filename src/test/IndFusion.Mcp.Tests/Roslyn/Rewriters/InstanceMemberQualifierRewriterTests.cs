namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// InstanceMemberQualifierRewriter QualifiesMember.
    /// </summary>
    [Fact]
    public void InstanceMemberQualifierRewriter_QualifiesMember()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void M(){ Value = 1; }") as MethodDeclarationSyntax;
        var rewriter = new InstanceMemberQualifierRewriter("inst", knownMembers: new HashSet<string> { "Value" });
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("inst.Value", result);
    }
}
