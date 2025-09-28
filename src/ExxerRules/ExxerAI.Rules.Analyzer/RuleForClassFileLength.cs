using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExxerAI.Analyzers;

/// <summary>
/// Diagnostic analyzer that detects classes that exceed recommended file length limits based on their method composition.
/// This analyzer enforces code organization standards by identifying classes that may have grown too large
/// and should be refactored into smaller, more focused components.
/// </summary>
/// <remarks>
/// File length limits are important for:
/// - Code maintainability and readability
/// - Adherence to Single Responsibility Principle
/// - Easier code reviews and debugging
/// - Better team collaboration and code comprehension
///
/// The analyzer applies different limits based on method types:
/// - Classes with both generic and non-generic methods: 240 lines (higher complexity tolerance)
/// - Classes with only generic or only non-generic methods: 160 lines (standard limit)
///
/// These limits are based on common industry practices and can help identify classes that might
/// benefit from decomposition into smaller, more cohesive units.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RuleForClassFileLength : DiagnosticAnalyzer
{
    /// <summary>
    /// The unique diagnostic identifier for the class file length rule.
    /// </summary>
    public const string DiagnosticId = "EXX1005";

    /// <summary>
    /// The diagnostic descriptor that defines the warning properties for excessive class file length detection.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "File exceeds recommended length for class type",
        messageFormat: "Class with {0} methods exceeds {1} lines",
        category: "Style",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Gets the collection of diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the class file length rule descriptor.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Initializes the analyzer by registering analysis actions with the provided context.
    /// This method configures the analyzer to exclude generated code and enable concurrent execution
    /// for optimal performance, then registers the syntax node analysis action for class declarations.
    /// </summary>
    /// <param name="context">The analysis context used to register analysis actions and configure analyzer behavior.</param>
    /// <remarks>
    /// The analyzer is configured to:
    /// - Exclude generated code from analysis to avoid false positives on auto-generated files
    /// - Enable concurrent execution for better performance in large codebases
    /// - Register syntax node analysis specifically for ClassDeclaration nodes
    /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClassLength, SyntaxKind.ClassDeclaration);
    }

    /// <summary>
    /// Analyzes class declaration syntax to determine if the class exceeds recommended file length limits
    /// based on its method composition. This method calculates the total lines of the class and applies
    /// different thresholds depending on whether the class contains generic methods, non-generic methods, or both.
    /// </summary>
    /// <param name="context">The syntax node analysis context containing the class declaration to analyze and methods for reporting diagnostics.</param>
    /// <remarks>
    /// The analysis process:
    /// 1. Calculates the total line count by examining the class's syntax span
    /// 2. Analyzes all method declarations to determine generic vs non-generic composition
    /// 3. Applies appropriate line limits:
    ///    - 240 lines for classes with both generic and non-generic methods (mixed complexity)
    ///    - 160 lines for classes with only generic or only non-generic methods (standard limit)
    /// 4. Reports a diagnostic if the calculated line count exceeds the determined limit
    ///
    /// Method classification:
    /// - Generic methods: Methods with type parameter lists (e.g., public T Method&lt;T&gt;())
    /// - Non-generic methods: Methods without type parameter lists
    ///
    /// The higher limit for mixed-method classes acknowledges that such classes often serve as
    /// generic utilities or adapters that legitimately require more code due to their complexity.
    ///
    /// Line counting includes:
    /// - Class declaration and closing brace
    /// - All class members (fields, properties, methods, nested types)
    /// - Comments and whitespace within the class boundaries
    /// </remarks>
    private static void AnalyzeClassLength(SyntaxNodeAnalysisContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var tree = context.Node.SyntaxTree;
        var lineSpan = classDecl.Span;
        var startLine = tree.GetLineSpan(lineSpan).StartLinePosition.Line;
        var endLine = tree.GetLineSpan(lineSpan).EndLinePosition.Line;
        var totalLines = endLine - startLine;

        var methodTypes = classDecl.Members.OfType<MethodDeclarationSyntax>();
        var hasGeneric = methodTypes.Any(m => m.TypeParameterList != null);
        var hasNonGeneric = methodTypes.Any(m => m.TypeParameterList == null);

        int limit = hasGeneric && hasNonGeneric ? 240 : 160;
        string type = hasGeneric ? (hasNonGeneric ? "generic and non-generic" : "generic") : "non-generic";

        if (totalLines > limit)
        {
            var diagnostic = Diagnostic.Create(Rule, classDecl.Identifier.GetLocation(), type, limit);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
