using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using IndFusion.Mcp.Core.Tools;

namespace IndFusion.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that introduces a new parameter to a method and updates invocations to pass a target expression.
/// </summary>
public class ParameterIntroductionRewriter : ExpressionIntroductionRewriter<MethodDeclarationSyntax>
{
    private readonly string _methodName;
    private readonly SyntaxGenerator _generator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterIntroductionRewriter"/> class.
    /// </summary>
    /// <param name="targetExpression">The expression to pass at call sites.</param>
    /// <param name="methodName">The method name to modify.</param>
    /// <param name="parameter">The parameter to introduce on the target method.</param>
    /// <param name="parameterReference">Identifier used to reference the new parameter.</param>
    /// <param name="generator">Syntax generator for argument construction.</param>
    public ParameterIntroductionRewriter(
        ExpressionSyntax targetExpression,
        string methodName,
        ParameterSyntax parameter,
        IdentifierNameSyntax parameterReference,
        SyntaxGenerator generator)
        : base(targetExpression, parameterReference, parameter, null)
    {
        _methodName = methodName;
        _generator = generator;
    }

    /// <inheritdoc />
    protected override MethodDeclarationSyntax InsertDeclaration(MethodDeclarationSyntax node, SyntaxNode declaration)
    {
        return node.AddParameterListParameters((ParameterSyntax)declaration);
    }

    /// <inheritdoc />
    public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        var visited = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;
        if (InvocationHelpers.IsInvocationOf(visited, _methodName))
        {
            visited = AstTransformations.AddArgument(
                visited,
                TargetExpression.WithoutTrivia(),
                _generator);
        }

        return visited;
    }

    /// <inheritdoc />
    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var visited = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node)!;
        bool shouldInsert = node.Identifier.ValueText == _methodName;
        return MaybeInsertDeclaration(node, visited, shouldInsert);
    }
}
