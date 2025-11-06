using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Tools.Cli.Core.Services;

/// <summary>
/// Analyzer for detecting refactoring opportunities
/// </summary>
public class OpportunityAnalyzer : CSharpSyntaxWalker
{
    /// <summary>
    /// Gets the list of refactoring opportunities
    /// </summary>
    public List<OpportunityInfo> Opportunities { get; } = [];

    /// <summary>
    /// Visits a method declaration
    /// </summary>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        // Check for long methods
        var lineCount = node.GetLocation().GetLineSpan().EndLinePosition.Line - node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        if (lineCount > 50)
        {
            Opportunities.Add(new OpportunityInfo
            {
                Type = "ExtractMethod",
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Description = $"Method '{node.Identifier.ValueText}' is too long ({lineCount} lines). Consider extracting methods."
            });
        }

        // Check for too many parameters
        if (node.ParameterList.Parameters.Count > 5)
        {
            Opportunities.Add(new OpportunityInfo
            {
                Type = "IntroduceParameterObject",
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Description = $"Method '{node.Identifier.ValueText}' has too many parameters ({node.ParameterList.Parameters.Count}). Consider introducing a parameter object."
            });
        }

        base.VisitMethodDeclaration(node);
    }

    /// <summary>
    /// Visits a class declaration
    /// </summary>
    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        // Check for large classes
        var memberCount = node.Members.Count;
        if (memberCount > 20)
        {
            Opportunities.Add(new OpportunityInfo
            {
                Type = "ExtractClass",
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Description = $"Class '{node.Identifier.ValueText}' is too large ({memberCount} members). Consider extracting classes."
            });
        }

        base.VisitClassDeclaration(node);
    }
}