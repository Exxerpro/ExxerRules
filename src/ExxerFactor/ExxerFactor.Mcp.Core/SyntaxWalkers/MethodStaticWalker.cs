using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Walks method declarations and records whether specific methods are declared static.
/// </summary>
public class MethodStaticWalker : CSharpSyntaxWalker
{
    private readonly HashSet<string> _methodNames;

    /// <summary>
    /// Gets a map from method name to whether it is declared static.
    /// </summary>
    public Dictionary<string, bool> IsStaticMap { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodStaticWalker"/> class.
    /// </summary>
    /// <param name="methodNames">The method names to observe.</param>
    public MethodStaticWalker(IEnumerable<string> methodNames)
    {
        _methodNames = new HashSet<string>(methodNames);
    }

    /// <inheritdoc />
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var name = node.Identifier.ValueText;
        if (_methodNames.Contains(name))
            IsStaticMap[name] = node.Modifiers.Any(SyntaxKind.StaticKeyword);
        base.VisitMethodDeclaration(node);
    }
}