using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects method declarations for a provided set of fully qualified targets.
/// </summary>
public class MethodCollectorWalker : CSharpSyntaxWalker
{
    private readonly HashSet<string> _targets;
    /// <summary>Discovered methods keyed as <c>ClassName.MethodName</c>.</summary>
    public Dictionary<string, MethodDeclarationSyntax> Methods { get; } = new();

    /// <summary>
    /// Initializes a new instance with a set of target identifiers to collect.
    /// </summary>
    /// <param name="targets">The set of <c>ClassName.MethodName</c> keys to capture.</param>
    public MethodCollectorWalker(HashSet<string> targets)
    {
        _targets = targets;
    }

    /// <summary>
    /// Adds methods that match the requested target keys to the collection.
    /// </summary>
    /// <param name="node">The method declaration node.</param>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (node.Parent is ClassDeclarationSyntax cls)
        {
            var key = $"{cls.Identifier.ValueText}.{node.Identifier.ValueText}";
            if (_targets.Contains(key) && !Methods.ContainsKey(key))
            {
                Methods[key] = node;
            }
        }
        base.VisitMethodDeclaration(node);
    }
}
