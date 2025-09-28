using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Determines whether a named member in a type is a field or a property by
/// walking declarations in the syntax tree.
/// </summary>
public class AccessMemberTypeWalker : CSharpSyntaxWalker
{
    private readonly string _memberName;
    /// <summary>
    /// Gets the discovered member kind for <c>_memberName</c>. Values are "field" or "property".
    /// </summary>
    public string? MemberType { get; private set; }

    /// <summary>
    /// Initializes a new instance targeting the specified member name.
    /// </summary>
    /// <param name="memberName">The member identifier to classify.</param>
    public AccessMemberTypeWalker(string memberName)
    {
        _memberName = memberName;
    }

    /// <summary>
    /// Visits field declarations and sets <see cref="MemberType"/> when a match is found.
    /// </summary>
    /// <param name="node">The field declaration node.</param>
    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        foreach (var variable in node.Declaration.Variables)
        {
            if (variable.Identifier.ValueText == _memberName)
            {
                MemberType = "field";
                return;
            }
        }
        base.VisitFieldDeclaration(node);
    }

    /// <summary>
    /// Visits property declarations and sets <see cref="MemberType"/> when a match is found.
    /// </summary>
    /// <param name="node">The property declaration node.</param>
    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (node.Identifier.ValueText == _memberName)
        {
            MemberType = "property";
            return;
        }
        base.VisitPropertyDeclaration(node);
    }
}
