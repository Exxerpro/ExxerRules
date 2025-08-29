using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// An analysis walker that suggests replacing concrete parameter types with
/// interfaces when only interface members are used within a method body.
/// </summary>
public class UseInterfaceWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel? _model;
    /// <summary>Collected suggestions in human-readable form.</summary>
    public List<string> Suggestions { get; } = new();

    /// <summary>
    /// Creates a new instance bound to an optional <see cref="SemanticModel"/>.
    /// </summary>
    /// <param name="model">The semantic model used to resolve symbols, if available.</param>
    public UseInterfaceWalker(SemanticModel? model)
    {
        _model = model;
    }

    /// <summary>
    /// Visits method declarations and records suggestions for parameters that can
    /// be typed to an interface instead of a concrete class.
    /// </summary>
    /// <param name="node">The method syntax node.</param>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        base.VisitMethodDeclaration(node);
        if (_model == null) return;

        foreach (var param in node.ParameterList.Parameters)
        {
            if (param.Type == null) continue;
            var typeInfo = _model.GetTypeInfo(param.Type);
            if (typeInfo.Type is not INamedTypeSymbol named || named.TypeKind != TypeKind.Class)
                continue;
            var interfaces = named.AllInterfaces;
            if (interfaces.Length == 0) continue;

            var collector = new ParameterMemberCollector(_model, param.Identifier.ValueText);
            collector.Visit(node);
            if (collector.Members.Count == 0) continue;

            foreach (var iface in interfaces)
            {
                if (collector.Members.All(m => iface.GetMembers(m.Name).Any()))
                {
                    Suggestions.Add($"Parameter '{param.Identifier.ValueText}' in method '{node.Identifier.ValueText}' only uses members of interface '{iface.Name}' -> use-interface");
                    break;
                }
            }
        }
    }

    private class ParameterMemberCollector : CSharpSyntaxWalker
    {
        private readonly SemanticModel _model;
        private readonly string _name;
        public List<ISymbol> Members { get; } = new();

        public ParameterMemberCollector(SemanticModel model, string name)
        {
            _model = model;
            _name = name;
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (node.Expression is IdentifierNameSyntax id && id.Identifier.ValueText == _name)
            {
                var symbol = _model.GetSymbolInfo(node).Symbol;
                if (symbol != null)
                    Members.Add(symbol);
            }
            base.VisitMemberAccessExpression(node);
        }
    }
}
