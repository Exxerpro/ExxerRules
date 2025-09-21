using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects names of classes and enums nested directly within a given origin class.
/// </summary>
public class NestedClassNameWalker : NameCollectorWalker
{
    private readonly ClassDeclarationSyntax _origin;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedClassNameWalker"/> class.
    /// </summary>
    /// <param name="origin">The origin class whose immediate nested types will be collected.</param>
    public NestedClassNameWalker(ClassDeclarationSyntax origin)
    {
        _origin = origin;
    }

    /// <inheritdoc />
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (node.Parent == _origin)
            Add(node.Identifier.ValueText);
        base.VisitClassDeclaration(node);
    }

    /// <inheritdoc />
    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        if (node.Parent == _origin)
            Add(node.Identifier.ValueText);
        base.VisitEnumDeclaration(node);
    }
}
