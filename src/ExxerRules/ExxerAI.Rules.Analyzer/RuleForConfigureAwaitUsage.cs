using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExxerAI.Analyzers
{
    /// <summary>
    /// Diagnostic analyzer that detects await expressions missing ConfigureAwait(false) calls.
    /// This analyzer enforces the best practice of using ConfigureAwait(false) in library code
    /// to prevent potential deadlocks and improve performance by avoiding unnecessary
    /// SynchronizationContext captures.
    /// </summary>
    /// <remarks>
    /// ConfigureAwait(false) Best Practices:
    ///
    /// **Why ConfigureAwait(false) is Important:**
    /// - Prevents deadlocks in applications with synchronization contexts (WinForms, WPF, ASP.NET Framework)
    /// - Improves performance by avoiding context switching overhead
    /// - Ensures library code doesn't depend on specific synchronization contexts
    /// - Reduces thread pool starvation in high-concurrency scenarios
    ///
    /// **When to Use ConfigureAwait(false):**
    /// - In library code that doesn't need to return to the original context
    /// - In service layers and business logic that don't interact with UI
    /// - In background processing and data access operations
    /// - In any code where the continuation doesn't require the original synchronization context
    ///
    /// **When ConfigureAwait(false) Might Not Be Needed:**
    /// - In application-level code that needs to return to UI context
    /// - When specifically requiring the original synchronization context for operations
    /// - In ASP.NET Core applications (which don't have problematic synchronization contexts by default)
    ///
    /// **Deadlock Prevention:**
    /// The analyzer helps prevent the classic deadlock scenario where:
    /// 1. Async method is called from synchronous context using .Result or .Wait()
    /// 2. The async method awaits without ConfigureAwait(false)
    /// 3. The continuation tries to return to the original context
    /// 4. The original context is blocked waiting for the async operation
    /// 5. Deadlock occurs as neither can proceed
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RuleForConfigureAwaitUsage : DiagnosticAnalyzer
    {
        /// <summary>
        /// The unique diagnostic identifier for the ConfigureAwait usage rule.
        /// </summary>
        public const string DiagnosticId = "EXX1001";

        /// <summary>
        /// The diagnostic descriptor that defines the warning properties for missing ConfigureAwait detection.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            title: "ConfigureAwait must be used",
            messageFormat: "Await expression is missing .ConfigureAwait(false)",
            category: "Reliability",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <summary>
        /// Gets the collection of diagnostic descriptors supported by this analyzer.
        /// </summary>
        /// <value>An immutable array containing the ConfigureAwait usage rule descriptor.</value>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <summary>
        /// Initializes the analyzer by registering analysis actions with the provided context.
        /// This method configures the analyzer to exclude generated code and enable concurrent execution
        /// for optimal performance, then registers the syntax node analysis action for await expressions.
        /// </summary>
        /// <param name="context">The analysis context used to register analysis actions and configure analyzer behavior.</param>
        /// <remarks>
        /// The analyzer is configured to:
        /// - Exclude generated code from analysis to avoid false positives on auto-generated async code
        /// - Enable concurrent execution for better performance when analyzing large codebases with many async operations
        /// - Register syntax node analysis specifically for AwaitExpression nodes to examine each await statement
        ///
        /// This focused approach ensures that only user-written await expressions are analyzed,
        /// avoiding noise from compiler-generated or tool-generated async code patterns.
        /// </remarks>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeAwaitExpression, SyntaxKind.AwaitExpression);
        }

        /// <summary>
        /// Analyzes await expressions to detect missing ConfigureAwait(false) calls.
        /// This method examines each await expression to determine if it includes a ConfigureAwait call,
        /// reporting a diagnostic for any await that doesn't use this pattern.
        /// </summary>
        /// <param name="context">The syntax node analysis context containing the await expression to analyze and methods for reporting diagnostics.</param>
        /// <remarks>
        /// The analysis process:
        /// 1. Casts the syntax node to an AwaitExpressionSyntax for detailed examination
        /// 2. Converts the awaited expression to string representation for text-based analysis
        /// 3. Checks if the expression contains "ConfigureAwait" text anywhere within it
        /// 4. Reports a diagnostic if ConfigureAwait is not found in the expression
        ///
        /// **Analysis Approach:**
        /// The current implementation uses text-based matching to detect ConfigureAwait usage.
        /// This approach is chosen for simplicity and broad coverage, catching patterns like:
        /// - `await someTask.ConfigureAwait(false)`
        /// - `await SomeMethodAsync().ConfigureAwait(false)`
        /// - `await (condition ? task1 : task2).ConfigureAwait(false)`
        ///
        /// **Detected Patterns:**
        /// ✅ Compliant: `await httpClient.GetAsync(url).ConfigureAwait(false)`
        /// ✅ Compliant: `await Task.Delay(1000).ConfigureAwait(false)`
        /// ❌ Non-compliant: `await httpClient.GetAsync(url)`
        /// ❌ Non-compliant: `await Task.Delay(1000)`
        ///
        /// **Limitations:**
        /// - Text-based matching may miss complex expressions or unusual formatting
        /// - Does not distinguish between ConfigureAwait(true) and ConfigureAwait(false)
        /// - May produce false positives for expressions containing "ConfigureAwait" in strings or comments
        /// - Does not perform semantic analysis to verify actual method calls
        ///
        /// **Future Enhancements:**
        /// - Semantic model analysis to verify actual ConfigureAwait method calls
        /// - Differentiation between ConfigureAwait(true) and ConfigureAwait(false)
        /// - Context-aware analysis to exclude UI-related code where ConfigureAwait(false) might not be appropriate
        /// - Integration with code fix providers to automatically add ConfigureAwait(false)
        /// - Configuration options to specify which projects or namespaces should enforce this rule
        /// </remarks>
        private static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            var awaitExpr = (AwaitExpressionSyntax)context.Node;
            var expr = awaitExpr.Expression.ToString();

            if (!expr.Contains("ConfigureAwait"))
            {
                var diagnostic = Diagnostic.Create(Rule, awaitExpr.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
