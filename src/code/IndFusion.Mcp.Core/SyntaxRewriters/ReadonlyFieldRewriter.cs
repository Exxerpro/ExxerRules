using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that ensures a field is marked readonly and optionally assigns an initializer in the constructor.
/// </summary>
public class ReadonlyFieldRewriter : CSharpSyntaxRewriter
{
    private readonly string _fieldName;
    private readonly ExpressionSyntax? _initializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadonlyFieldRewriter"/> class.
    /// </summary>
    /// <param name="fieldName">The name of the field to make readonly.</param>
    /// <param name="initializer">Optional initializer expression to assign in the constructor.</param>
    public ReadonlyFieldRewriter(string fieldName, ExpressionSyntax? initializer)
    {
        _fieldName = fieldName;
        _initializer = initializer;
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        var variable = node.Declaration.Variables.FirstOrDefault(v => v.Identifier.ValueText == _fieldName);
        if (variable == null)
            return base.VisitFieldDeclaration(node);

        var newVariable = variable.WithInitializer(null);
        var newDecl = node.Declaration.ReplaceNode(variable, newVariable);
        var modifiers = node.Modifiers;
        if (!modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)))
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

        return node.WithDeclaration(newDecl).WithModifiers(modifiers);
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        var visited = (ConstructorDeclarationSyntax)base.VisitConstructorDeclaration(node)!;
        if (_initializer != null)
        {
            var assignment = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(_fieldName),
                    _initializer));
            var body = visited.Body ?? SyntaxFactory.Block();
            visited = visited.WithBody(body.AddStatements(assignment));
        }
        return visited;
    }
}