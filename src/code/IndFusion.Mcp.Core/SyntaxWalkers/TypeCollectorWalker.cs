using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects type declarations of the specified kind into a name-to-declaration map.
/// </summary>
/// <typeparam name="T">The type of declaration to collect.</typeparam>
public class TypeCollectorWalker<T> : CSharpSyntaxWalker where T : TypeDeclarationSyntax
{
    /// <summary>
    /// Gets a map from type name to its declaration node.
    /// </summary>
    public Dictionary<string, T> Types { get; } = new();

    /// <inheritdoc />
    public override void Visit(SyntaxNode? node)
    {
        if (node is T typed)
        {
            var name = typed.Identifier.ValueText;
            if (!Types.ContainsKey(name))
                Types[name] = typed;
        }
        base.Visit(node);
    }
}
