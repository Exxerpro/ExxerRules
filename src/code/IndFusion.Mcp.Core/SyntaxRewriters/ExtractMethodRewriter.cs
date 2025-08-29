using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that extracts a selection of statements into a new private method and updates the original method to call it.
/// </summary>
public class ExtractMethodRewriter : CSharpSyntaxRewriter
{
    private readonly MethodDeclarationSyntax _containingMethod;
    private readonly ClassDeclarationSyntax? _containingClass;
    private readonly List<StatementSyntax> _statements;
    private readonly string _methodName;
    private readonly MethodDeclarationSyntax _newMethod;
    private readonly MethodDeclarationSyntax _updatedMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtractMethodRewriter"/> class.
    /// </summary>
    /// <param name="containingMethod">The method containing the statements to extract.</param>
    /// <param name="containingClass">Optional containing class where the new method should be inserted.</param>
    /// <param name="statements">The sequence of statements to extract.</param>
    /// <param name="methodName">The name of the newly created method.</param>
    public ExtractMethodRewriter(
        MethodDeclarationSyntax containingMethod,
        ClassDeclarationSyntax? containingClass,
        List<StatementSyntax> statements,
        string methodName)
    {
        _containingMethod = containingMethod;
        _containingClass = containingClass;
        _statements = statements;
        _methodName = methodName;

        _newMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                methodName)
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
            .WithBody(SyntaxFactory.Block(statements));

        var methodCall = SyntaxFactory.ExpressionStatement(
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.IdentifierName(methodName)));

        var body = containingMethod.Body!;
        var updated = body.ReplaceNode(statements.First(), methodCall)!;
        foreach (var stmt in statements.Skip(1))
            updated = updated.RemoveNode(stmt, SyntaxRemoveOptions.KeepNoTrivia)!;

        _updatedMethod = containingMethod.WithBody(updated);
    }

    /// <inheritdoc />
    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (node == _containingMethod)
            return _updatedMethod;
        return base.VisitMethodDeclaration(node)!;
    }

    /// <inheritdoc />
    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var visited = (ClassDeclarationSyntax)base.VisitClassDeclaration(node)!;
        if (_containingClass != null && node == _containingClass)
        {
            visited = visited.AddMembers(_newMethod);
        }
        return visited;
    }
}
