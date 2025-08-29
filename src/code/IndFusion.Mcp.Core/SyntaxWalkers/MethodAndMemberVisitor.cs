using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Visits methods and member declarations to collect simple metadata such as
/// whether a method is static and whether a member is a field or property.
/// </summary>
public class MethodAndMemberVisitor : CSharpSyntaxWalker
{
    /// <summary>Information about a discovered method.</summary>
    public class MethodInfo
    {
        /// <summary>Indicates whether the method is declared static.</summary>
        public bool IsStatic { get; set; }
    }

    /// <summary>Information about a discovered member.</summary>
    public class MemberInfo
    {
        /// <summary>The member kind, e.g., "field" or "property".</summary>
        public string Type { get; set; } = string.Empty; // "field" or "property"
    }

    /// <summary>Collected methods keyed by method name.</summary>
    public Dictionary<string, MethodInfo> Methods { get; } = new();
    /// <summary>Collected members keyed by member name.</summary>
    public Dictionary<string, MemberInfo> Members { get; } = new();

    /// <summary>
    /// Records method metadata such as the static modifier.
    /// </summary>
    /// <param name="node">The method declaration node.</param>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var methodName = node.Identifier.ValueText;
        if (!Methods.ContainsKey(methodName))
        {
            Methods[methodName] = new MethodInfo
            {
                IsStatic = node.Modifiers.Any(SyntaxKind.StaticKeyword)
            };
        }
    }

    /// <summary>
    /// Records field member names in the collection.
    /// </summary>
    /// <param name="node">The field declaration node.</param>
    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        foreach (var variable in node.Declaration.Variables)
        {
            var fieldName = variable.Identifier.ValueText;
            if (!Members.ContainsKey(fieldName))
            {
                Members[fieldName] = new MemberInfo { Type = "field" };
            }
        }
    }

    /// <summary>
    /// Records property member names in the collection.
    /// </summary>
    /// <param name="node">The property declaration node.</param>
    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var propertyName = node.Identifier.ValueText;
        if (!Members.ContainsKey(propertyName))
        {
            Members[propertyName] = new MemberInfo { Type = "property" };
        }
    }
}
