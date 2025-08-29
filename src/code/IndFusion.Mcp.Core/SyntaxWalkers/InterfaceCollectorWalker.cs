using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects interface declarations found during a syntax tree walk.
/// </summary>
public class InterfaceCollectorWalker : TypeCollectorWalker<InterfaceDeclarationSyntax>
{
    /// <summary>
    /// Gets the map of discovered interfaces keyed by their identifier.
    /// </summary>
    public Dictionary<string, InterfaceDeclarationSyntax> Interfaces => Types;
}
