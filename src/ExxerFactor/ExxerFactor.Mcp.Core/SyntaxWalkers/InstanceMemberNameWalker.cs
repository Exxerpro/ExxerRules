using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects instance member identifiers (fields and properties) from a type.
/// </summary>
public class InstanceMemberNameWalker : NameCollectorWalker
{
    /// <summary>
    /// Adds field names declared in the type to the collector.
    /// </summary>
    /// <param name="node">The field declaration node.</param>
    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        foreach (var variable in node.Declaration.Variables)
            Add(variable.Identifier.ValueText);
        base.VisitFieldDeclaration(node);
    }

    /// <summary>
    /// Adds property names declared in the type to the collector.
    /// </summary>
    /// <param name="node">The property declaration node.</param>
    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        Add(node.Identifier.ValueText);
        base.VisitPropertyDeclaration(node);
    }
}