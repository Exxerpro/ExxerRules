using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Removes a local variable declaration (or a single variable within it) by name within a specified span.
/// </summary>
public class VariableRemovalRewriter : DeclarationRemovalRewriter<LocalDeclarationStatementSyntax>
{
    private readonly TextSpan _span;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRemovalRewriter"/> class.
    /// </summary>
    /// <param name="variableName">The name of the local variable to remove.</param>
    /// <param name="span">The exact span of the variable to match.</param>
    public VariableRemovalRewriter(string variableName, TextSpan span)
        : base(variableName)
    {
        _span = span;
    }

    /// <inheritdoc />
    protected override bool IsTarget(LocalDeclarationStatementSyntax node)
        => node.Declaration.Variables.Any(v => v.Identifier.ValueText == Name && v.Span.Equals(_span));

    /// <inheritdoc />
    protected override SeparatedSyntaxList<VariableDeclaratorSyntax>? GetDeclarators(LocalDeclarationStatementSyntax node)
        => node.Declaration.Variables;

    /// <inheritdoc />
    protected override LocalDeclarationStatementSyntax WithDeclarators(LocalDeclarationStatementSyntax node, SeparatedSyntaxList<VariableDeclaratorSyntax> declarators)
        => node.WithDeclaration(node.Declaration.WithVariables(declarators));
}

