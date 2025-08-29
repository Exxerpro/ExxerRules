using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Computes simple class-level metrics and emits suggestions for oversized types.
/// </summary>
public class ClassMetricsWalker : CSharpSyntaxWalker
{
    /// <summary>Collected suggestions derived from class metrics.</summary>
    public List<string> Suggestions { get; } = new();

    /// <summary>
    /// Visits class declarations and records metrics such as member count and line span.
    /// </summary>
    /// <param name="node">The class declaration node.</param>
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        base.VisitClassDeclaration(node);

        var members = node.Members.Count;
        var span = node.GetLocation().GetLineSpan();
        var lines = span.EndLinePosition.Line - span.StartLinePosition.Line + 1;
        if (members > 15 || lines > 300)
            Suggestions.Add($"Class '{node.Identifier}' is large ({members} members) -> consider splitting or move-method");
    }
}
