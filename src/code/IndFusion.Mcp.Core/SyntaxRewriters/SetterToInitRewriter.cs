using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that converts a property setter to an init accessor for the specified property.
/// </summary>
public class SetterToInitRewriter : CSharpSyntaxRewriter
{
    private readonly string _propertyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetterToInitRewriter"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property to transform.</param>
    public SetterToInitRewriter(string propertyName)
    {
        _propertyName = propertyName;
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (node.Identifier.ValueText != _propertyName)
            return base.VisitPropertyDeclaration(node);

        var setter = node.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));
        if (setter == null)
            return base.VisitPropertyDeclaration(node);

        var initAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
            .WithSemicolonToken(setter.SemicolonToken);
        var newAccessorList = node.AccessorList!.ReplaceNode(setter, initAccessor);
        return node.WithAccessorList(newAccessorList);
    }
}
