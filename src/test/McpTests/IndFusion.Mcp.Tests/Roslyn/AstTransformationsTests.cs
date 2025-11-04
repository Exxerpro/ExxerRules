namespace IndFusion.Mcp.Tests.Roslyn;

/// <summary>
/// Type RoslynTransformationTests
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// AddParameter AddsParameterToMethod.
    /// </summary>
    [Fact]
    public void AddParameter_AddsParameterToMethod()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test() { }") as MethodDeclarationSyntax;
        var updated = AstTransformations.AddParameter(method!, "value", "int");

        Assert.Single(updated.ParameterList.Parameters);
        Assert.Equal("value", updated.ParameterList.Parameters[0].Identifier.ValueText);
        Assert.Equal("int", updated.ParameterList.Parameters[0].Type!.ToString());
    }

    /// <summary>
    /// ReplaceThisReferences ReplacesWithParameter.
    /// </summary>
    [Fact]
    public void ReplaceThisReferences_ReplacesWithParameter()
    {
        var method = SyntaxFactory.ParseMemberDeclaration(
            "void Test() { Console.WriteLine(this.Value); }") as MethodDeclarationSyntax;
        var updated = AstTransformations.ReplaceThisReferences(method!, "instance");
        var output = updated.NormalizeWhitespace().ToFullString();

        Assert.Contains("instance.Value", output);
        Assert.DoesNotContain("this.Value", output);
    }

    /// <summary>
    /// QualifyInstanceMembers AddsParameterQualification.
    /// </summary>
    [Fact]
    public void QualifyInstanceMembers_AddsParameterQualification()
    {
        var method = SyntaxFactory.ParseMemberDeclaration(
            "void Test() { Console.WriteLine(Value); }") as MethodDeclarationSyntax;
        var qualified = AstTransformations.QualifyInstanceMembers(
            method!,
            "instance",
            ["Value"]);

        var output = qualified.NormalizeWhitespace().ToFullString();
        Assert.Contains("instance.Value", output);
    }

    /// <summary>
    /// EnsureStaticModifier AddsStaticIfMissing.
    /// </summary>
    [Fact]
    public void EnsureStaticModifier_AddsStaticIfMissing()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test() { }") as MethodDeclarationSyntax;
        var updated = AstTransformations.EnsureStaticModifier(method!);

        Assert.Contains("static", updated.Modifiers.ToFullString());
    }

    /// <summary>
    /// AddArgument AddsArgumentToInvocation.
    /// </summary>
    [Fact]
    public void AddArgument_AddsArgumentToInvocation()
    {
        var invocation = SyntaxFactory.ParseExpression("M()") as InvocationExpressionSyntax;
        var expr = SyntaxFactory.IdentifierName("x");
        var generator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);
        var updated = AstTransformations.AddArgument(invocation!, expr, generator);

        Assert.Equal("M(x)", updated.NormalizeWhitespace().ToFullString());
    }

    /// <summary>
    /// RemoveArgument RemovesArgumentFromInvocation.
    /// </summary>
    [Fact]
    public void RemoveArgument_RemovesArgumentFromInvocation()
    {
        var invocation = SyntaxFactory.ParseExpression("M(a, b)") as InvocationExpressionSyntax;
        var updated = AstTransformations.RemoveArgument(invocation!, 0);

        Assert.Equal("M(b)", updated.NormalizeWhitespace().ToFullString());
    }
}
