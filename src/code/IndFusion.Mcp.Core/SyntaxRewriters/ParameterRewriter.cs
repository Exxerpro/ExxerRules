using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that substitutes parameter and member references based on a provided mapping.
/// </summary>
public class ParameterRewriter : CSharpSyntaxRewriter
{
    private readonly Dictionary<string, ExpressionSyntax> _map;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRewriter"/> class.
    /// </summary>
    /// <param name="map">Mapping from parameter/member names to replacement expressions.</param>
    public ParameterRewriter(Dictionary<string, ExpressionSyntax> map)
    {
        _map = map;
    }

    /// <inheritdoc />
    public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        if (node.Expression is ThisExpressionSyntax && node.Name is IdentifierNameSyntax id &&
            _map.TryGetValue(id.Identifier.ValueText, out var expr))
        {
            return expr;
        }

        return base.VisitMemberAccessExpression(node)!;
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (_map.TryGetValue(node.Identifier.ValueText, out var expr))
        {
            // Only replace if the parent context allows it
            var parent = node.Parent;

            return parent switch
            {
                // Don't replace if this identifier is part of a member access expression on the left side
                MemberAccessExpressionSyntax memberAccess when memberAccess.Expression == node => base
                    .VisitIdentifierName(node),
                // Don't replace if this is a property/field name in an object initializer
                AssignmentExpressionSyntax assign when assign.Left == node => base.VisitIdentifierName(node),
                // Don't replace if this is in a parameter declaration or type context
                ParameterSyntax or TypeSyntax => base.VisitIdentifierName(node),
                _ => expr
            };
        }
        return base.VisitIdentifierName(node);
    }
}
