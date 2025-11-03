using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Analyzers.CodeFormatting;

/// <summary>
/// Analyzer that provides a mechanism to trigger project formatting on demand.
/// This analyzer will always report a hidden diagnostic that can be used to trigger formatting.
/// SRP: Responsible only for providing a trigger point for project-wide formatting actions.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ProjectFormattingAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the localized title displayed by the project-formatting diagnostic.
    /// </summary>
    private static readonly LocalizableString Title = "Format project with dotnet format";

    /// <summary>
    /// Gets the localized message format surfaced to the user when triggering project formatting.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Click to format the entire project using 'dotnet format --severity info --verbosity d'";

    /// <summary>
    /// Gets the descriptive text that explains how the project-formatting command operates.
    /// </summary>
    private static readonly LocalizableString Description = "Provides an action to run 'dotnet format --severity info --verbosity d' on the current project. This action will format all files in the project according to EditorConfig settings and code style rules.";

    /// <summary>
    /// The diagnostic descriptor emitted to expose the project-formatting command.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.ProjectFormatting,
        Title,
        MessageFormat,
        DiagnosticCategories.CodeQuality,
        DiagnosticSeverity.Hidden,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the project-formatting trigger diagnostic.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers the compilation-start action that enables project-wide formatting diagnostics.
    /// </summary>
    /// <param name="context">The Roslyn analysis context used for registration.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register on compilation start to provide project-wide formatting option
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    /// <summary>
    /// Registers the syntax-tree action used to surface project-formatting diagnostics for each file.
    /// </summary>
    /// <param name="context">The compilation-start context provided by Roslyn.</param>
    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        // Register syntax tree analysis to provide formatting action on every file
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    /// <summary>
    /// Analyzes a syntax tree and reports a hidden diagnostic that can trigger project-wide formatting.
    /// </summary>
    /// <param name="context">The syntax tree analysis context.</param>
    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        // Check if this file should be exempted from formatting diagnostics
        if (IsExemptFromProjectFormatting(context))
        {
            return;
        }

        // Only report on the first line of the file to provide a consistent location
        var root = context.Tree.GetRoot(context.CancellationToken);
        if (!(root.HasLeadingTrivia || root.ChildNodes().Any()))
        {
            return;
        }

        var location = Location.Create(context.Tree, new Microsoft.CodeAnalysis.Text.TextSpan(0, 0));

        // Report a diagnostic that can be used to trigger formatting
        var diagnostic = Diagnostic.Create(
            Rule,
            location,
            context.Tree.FilePath ?? "Current Project");

        context.ReportDiagnostic(diagnostic);
    }

    //  False-Positive Mitigation

    /// <summary>
    /// Determines whether the current syntax tree should be exempt from project-formatting diagnostics.
    /// </summary>
    /// <param name="context">The syntax tree analysis context providing file information.</param>
    /// <returns><c>true</c> when the file meets any exemption criteria; otherwise, <c>false</c>.</returns>
    private static bool IsExemptFromProjectFormatting(SyntaxTreeAnalysisContext context)
    {
        return IsGeneratedFile(context) ||
               IsEmptyOrWhitespaceOnlyFile(context);
    }

    /// <summary>
    /// Story 1.1: Exempt Generated Files.
    /// </summary>
    /// <param name="context">The syntax tree analysis context.</param>
    /// <returns><c>true</c> when the file appears to be generated; otherwise, <c>false</c>.</returns>
    private static bool IsGeneratedFile(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);
        
        // Check for auto-generated comments
        var leadingTrivia = root.GetLeadingTrivia();
        foreach (var trivia in leadingTrivia)
        {
            if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || 
                trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
            {
                var commentText = trivia.ToString();
                if (commentText.Contains("<auto-generated>") || 
                    commentText.Contains("This code was generated"))
                {
                    return true;
                }
            }
        }

        // Check file path for generated indicators
        var filePath = context.Tree.FilePath ?? "";
        return filePath.Contains("obj") || 
               filePath.Contains("bin") ||
               filePath.Contains("Generated");
    }

    /// <summary>
    /// Story 1.2: Exempt Empty or Whitespace-Only Files.
    /// </summary>
    /// <param name="context">The syntax tree analysis context.</param>
    /// <returns><c>true</c> when the file contains only whitespace or comments; otherwise, <c>false</c>.</returns>
    private static bool IsEmptyOrWhitespaceOnlyFile(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);

        // Whitespace-only file (no tokens besides EOF)
        var tokens = root.DescendantTokens(descendIntoTrivia: true);
        if (!tokens.Any(static token => !token.IsKind(SyntaxKind.EndOfFileToken)))
        {
            return true;
        }

        // Comment-only file (no member declarations or global statements)
        var hasContent = root.DescendantNodes()
            .Any(node => node is MemberDeclarationSyntax or GlobalStatementSyntax);

        return !hasContent;
    }

     // 
}
