using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that qualifies references to nested class types with their containing outer class name.
/// </summary>
public class NestedClassRewriter : CSharpSyntaxRewriter
{
    private readonly HashSet<string> _classNames;
    private readonly string _outerClass;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedClassRewriter"/> class.
    /// </summary>
    /// <param name="classNames">Set of nested class names that should be qualified.</param>
    /// <param name="outerClass">The name of the containing (outer) class.</param>
    public NestedClassRewriter(HashSet<string> classNames, string outerClass)
    {
        _classNames = classNames;
        _outerClass = outerClass;
    }

    /// <inheritdoc />
    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (_classNames.Contains(node.Identifier.ValueText))
        {
            var parent = node.Parent;

            if (parent is QualifiedNameSyntax qn && qn.Right == node)
            {
                var qualified = SyntaxFactory.QualifiedName(
                    SyntaxFactory.IdentifierName(_outerClass),
                    node.WithoutTrivia());
                return qualified.WithTriviaFrom(node);
            }

            if (IsTypeContext(node) || parent is TypeArgumentListSyntax)
            {
                var qualified = SyntaxFactory.QualifiedName(
                    SyntaxFactory.IdentifierName(_outerClass),
                    node.WithoutTrivia());
                return qualified.WithTriviaFrom(node);
            }

            if (parent is MemberAccessExpressionSyntax mae && mae.Expression == node)
            {
                var qualifiedExpr = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(_outerClass),
                    node.WithoutTrivia());
                return qualifiedExpr.WithTriviaFrom(node);
            }

            if (parent is not MemberAccessExpressionSyntax)
            {
                var qualifiedExpr = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(_outerClass),
                    node.WithoutTrivia());
                return qualifiedExpr.WithTriviaFrom(node);
            }
        }

        return base.VisitIdentifierName(node)!;
    }

    private static bool IsTypeContext(IdentifierNameSyntax node)
    {
        var parent = node.Parent;
        return (parent is VariableDeclarationSyntax v && v.Type == node)
               || (parent is ParameterSyntax p && p.Type == node)
               || (parent is MethodDeclarationSyntax md && md.ReturnType == node)
               || (parent is LocalFunctionStatementSyntax lf && lf.ReturnType == node)
               || (parent is ObjectCreationExpressionSyntax o && o.Type == node)
               || (parent is ForEachStatementSyntax f && f.Type == node)
               || (parent is ForEachVariableStatementSyntax fv && fv.Variable == node)
               || (parent is CastExpressionSyntax c && c.Type == node)
               || (parent is TypeOfExpressionSyntax t && t.Type == node)
               || (parent is DefaultExpressionSyntax d && d.Type == node)
               || (parent is AttributeSyntax attr && attr.Name == node)
               || (parent is TypeConstraintSyntax tc && tc.Type == node)
               || (parent is BaseTypeSyntax bt && bt.Type == node)
               || (parent is UsingStatementSyntax us && us.Declaration?.Type == node)
               || parent is TypeSyntax;
    }
}
