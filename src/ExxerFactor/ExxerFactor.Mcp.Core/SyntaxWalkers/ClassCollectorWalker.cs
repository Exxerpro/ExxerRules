using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects class declarations found during a syntax tree walk.
/// </summary>
public class ClassCollectorWalker : TypeCollectorWalker<ClassDeclarationSyntax>
{
    /// <summary>
    /// Gets the map of discovered classes keyed by their identifier.
    /// </summary>
    public Dictionary<string, ClassDeclarationSyntax> Classes => Types;
}