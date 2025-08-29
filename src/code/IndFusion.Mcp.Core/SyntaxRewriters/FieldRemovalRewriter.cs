using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Removes a field declaration (or a single variable within a multi-variable declaration) by name.
/// </summary>
public class FieldRemovalRewriter : DeclarationRemovalRewriter<FieldDeclarationSyntax>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldRemovalRewriter"/> class.
    /// </summary>
    /// <param name="fieldName">The name of the field to remove.</param>
    public FieldRemovalRewriter(string fieldName)
        : base(fieldName)
    {
    }

    /// <inheritdoc />
    protected override bool IsTarget(FieldDeclarationSyntax node)
        => node.Declaration.Variables.Any(v => v.Identifier.ValueText == Name);

    /// <inheritdoc />
    protected override SeparatedSyntaxList<VariableDeclaratorSyntax>? GetDeclarators(FieldDeclarationSyntax node)
        => node.Declaration.Variables;

    /// <inheritdoc />
    protected override FieldDeclarationSyntax WithDeclarators(FieldDeclarationSyntax node, SeparatedSyntaxList<VariableDeclaratorSyntax> declarators)
        => node.WithDeclaration(node.Declaration.WithVariables(declarators));
}

