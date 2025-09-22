namespace IndFusion.Tools.Mcp.App.SyntaxWalkers;

/// <summary>
/// Walks C# syntax to collect public information about methods and members
/// (fields and properties) present in a type declaration.
/// </summary>
public class MethodAndMemberVisitor : CSharpSyntaxWalker
{
    /// <summary>
    /// Metadata about a discovered method.
    /// </summary>
    public class MethodInfo
    {
        /// <summary>
        /// Indicates whether the discovered method is declared as static.
        /// </summary>
        public bool IsStatic { get; set; }
    }

    /// <summary>
    /// Metadata about a discovered member (field or property).
    /// </summary>
    public class MemberInfo
    {
        /// <summary>
        /// Kind of the member: "field" or "property".
        /// </summary>
        public string Type { get; set; } = string.Empty; // "field" or "property"
    }

    /// <summary>
    /// Collected methods keyed by method name.
    /// </summary>
    public Dictionary<string, MethodInfo> Methods { get; } = new();
    /// <summary>
    /// Collected members keyed by member name.
    /// </summary>
    public Dictionary<string, MemberInfo> Members { get; } = new();

    /// <summary>
    /// Visits method declarations and records their basic metadata.
    /// </summary>
    /// <param name="node">The method declaration syntax node.</param>
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
    /// Visits field declarations and records field names.
    /// </summary>
    /// <param name="node">The field declaration syntax node.</param>
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
    /// Visits property declarations and records property names.
    /// </summary>
    /// <param name="node">The property declaration syntax node.</param>
    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var propertyName = node.Identifier.ValueText;
        if (!Members.ContainsKey(propertyName))
        {
            Members[propertyName] = new MemberInfo { Type = "property" };
        }
    }
}
