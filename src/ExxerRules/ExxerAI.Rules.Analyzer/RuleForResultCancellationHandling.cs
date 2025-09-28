using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExxerAI.Analyzers;

/// <summary>
/// Diagnostic analyzer that detects method invocations that should include cancellation token parameters
/// but are missing them. This analyzer helps ensure proper cancellation support in asynchronous operations
/// and long-running tasks by identifying calls that could benefit from cancellation token usage.
/// </summary>
/// <remarks>
/// Proper cancellation token handling is crucial for:
/// - Responsive user interfaces that can cancel long-running operations
/// - Resource management and cleanup in distributed systems
/// - Graceful shutdown scenarios in web applications and services
/// - Preventing resource leaks and improving application reliability
///
/// This analyzer performs a basic text-based check for "CancellationToken" in method arguments.
/// For production use, consider enhancing this with semantic analysis to detect specific method signatures.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RuleForResultCancellationHandling : DiagnosticAnalyzer
{
    /// <summary>
    /// The unique diagnostic identifier for the cancellation token handling rule.
    /// </summary>
    public const string DiagnosticId = "EXX1002";

    /// <summary>
    /// The diagnostic descriptor that defines the warning properties for missing cancellation token detection.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Missing cancellation token handling",
        messageFormat: "Call to method with cancellable result is missing a CancellationToken",
        category: "Reliability",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Gets the collection of diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the cancellation token handling rule descriptor.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Initializes the analyzer by registering analysis actions with the provided context.
    /// This method configures the analyzer to exclude generated code and enable concurrent execution
    /// for optimal performance, then registers the syntax node analysis action for method invocations.
    /// </summary>
    /// <param name="context">The analysis context used to register analysis actions and configure analyzer behavior.</param>
    /// <remarks>
    /// The analyzer is configured to:
    /// - Exclude generated code from analysis to avoid false positives
    /// - Enable concurrent execution for better performance in large codebases
    /// - Register syntax node analysis specifically for InvocationExpression nodes
    /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    /// <summary>
    /// Analyzes method invocation expressions to detect calls that should include cancellation tokens
    /// but are missing them. This method examines the argument list of each invocation to determine
    /// if a CancellationToken parameter is present.
    /// </summary>
    /// <param name="context">The syntax node analysis context containing the invocation expression to analyze and methods for reporting diagnostics.</param>
    /// <remarks>
    /// The analysis process:
    /// 1. Casts the syntax node to an InvocationExpressionSyntax
    /// 2. Examines all arguments in the invocation's argument list
    /// 3. Checks if any argument contains "CancellationToken" text
    /// 4. Reports a diagnostic if no cancellation token is found
    ///
    /// Limitations of current implementation:
    /// - Uses text-based matching which may produce false positives/negatives
    /// - Does not perform semantic analysis to verify actual parameter types
    /// - May flag methods that legitimately don't need cancellation tokens
    ///
    /// Future enhancements could include:
    /// - Semantic model analysis to check actual method signatures
    /// - Configuration to specify which methods require cancellation tokens
    /// - Detection of async/await patterns that would benefit from cancellation
    /// </remarks>
    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.ArgumentList.Arguments.All(arg => !arg.ToString().Contains("CancellationToken")))
        {
            var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
