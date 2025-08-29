using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Removes a method declaration by name.
/// </summary>
public class MethodRemovalRewriter : DeclarationRemovalRewriter<MethodDeclarationSyntax>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodRemovalRewriter"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to remove.</param>
    public MethodRemovalRewriter(string methodName)
        : base(methodName)
    {
    }

    /// <inheritdoc />
    protected override bool IsTarget(MethodDeclarationSyntax node)
        => node.Identifier.ValueText == Name;
}


