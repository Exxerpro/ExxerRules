using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExxerAI.Analyzers;

/// <summary>
/// Diagnostic analyzer that detects and reports usage of #region directives in C# code.
/// This analyzer enforces a coding style that discourages the use of regions as they can be
/// considered a code smell that indicates overly complex or poorly organized code.
/// </summary>
/// <remarks>
/// Regions can hide code complexity and make navigation more difficult. Instead of using regions,
/// consider breaking large classes into smaller, more focused classes that follow the Single Responsibility Principle.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RuleForRegionUsage : DiagnosticAnalyzer
{
    /// <summary>
    /// The unique diagnostic identifier for the region usage rule.
    /// </summary>
    public const string DiagnosticId = "EXX1004";

    /// <summary>
    /// The diagnostic descriptor that defines the warning properties for region usage detection.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Avoid #region usage",
        messageFormat: "#region directives are discouraged",
        category: "Style",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Gets the collection of diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the region usage rule descriptor.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Initializes the analyzer by registering analysis actions with the provided context.
    /// This method configures the analyzer to exclude generated code and enable concurrent execution
    /// for optimal performance, then registers the syntax tree analysis action.
    /// </summary>
    /// <param name="context">The analysis context used to register analysis actions and configure analyzer behavior.</param>
    /// <remarks>
    /// The analyzer is configured to:
    /// - Exclude generated code from analysis to avoid false positives
    /// - Enable concurrent execution for better performance in large codebases
    /// - Register syntax tree analysis to examine trivia for region directives
    /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    /// <summary>
    /// Analyzes a syntax tree to detect #region directive trivia and reports diagnostics for each occurrence.
    /// This method traverses the entire syntax tree, examining all trivia to identify region directives
    /// and creates diagnostic reports for each one found.
    /// </summary>
    /// <param name="context">The syntax tree analysis context containing the tree to analyze and methods for reporting diagnostics.</param>
    /// <remarks>
    /// The analysis process:
    /// 1. Obtains the syntax tree root with proper cancellation token handling
    /// 2. Searches all descendant trivia for RegionDirectiveTrivia syntax kinds
    /// 3. Creates and reports a diagnostic for each region directive found
    ///
    /// This approach ensures comprehensive detection of all #region usages regardless of their
    /// position in the code structure (class level, method level, etc.).
    /// </remarks>
    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);
        var regionDirectives = root.DescendantTrivia()
            .Where(trivia => trivia.IsKind(SyntaxKind.RegionDirectiveTrivia));

        foreach (var regionDirective in regionDirectives)
        {
            var diagnostic = Diagnostic.Create(Rule, regionDirective.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
