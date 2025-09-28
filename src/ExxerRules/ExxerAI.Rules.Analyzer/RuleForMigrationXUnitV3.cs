using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExxerAI.Analyzers;

/// <summary>
/// Diagnostic analyzer that detects test classes using patterns that are incompatible with xUnit v3.
/// This analyzer helps identify classes that need refactoring during migration from xUnit v2 to v3
/// by detecting the combination of constructor-based setup and IDisposable cleanup patterns.
/// </summary>
/// <remarks>
/// xUnit v3 Migration Context:
/// xUnit v3 introduces breaking changes that affect test class lifecycle management.
/// The combination of constructor-based initialization and IDisposable cleanup that was
/// common in xUnit v2 may not work correctly in v3 due to changes in test execution models.
///
/// Detected Pattern:
/// - Classes that have constructors (for test setup/initialization)
/// - AND implement IDisposable interface (for test cleanup)
///
/// Migration Recommendations:
/// - Replace constructor setup with xUnit v3-compatible initialization patterns
/// - Replace IDisposable cleanup with xUnit v3-compatible cleanup mechanisms
/// - Consider using xUnit v3's new lifecycle hooks and test context features
///
/// This analyzer provides early detection of migration requirements, allowing teams
/// to proactively update their test code before upgrading to xUnit v3.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RuleForMigrationXUnitV3 : DiagnosticAnalyzer
{
    /// <summary>
    /// The unique diagnostic identifier for the xUnit v3 migration rule.
    /// </summary>
    public const string DiagnosticId = "EXX1003";

    /// <summary>
    /// The diagnostic descriptor that defines the warning properties for xUnit v3 incompatible patterns detection.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "xUnit v3 migration required",
        messageFormat: "Test class uses obsolete pattern incompatible with xUnit v3",
        category: "Migration",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Gets the collection of diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the xUnit v3 migration rule descriptor.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);    /// <summary>

                                                                                                                 /// Initializes the analyzer by registering analysis actions with the provided context.
                                                                                                                 /// This method configures the analyzer to exclude generated code and enable concurrent execution
                                                                                                                 /// for optimal performance, then registers the syntax node analysis action for class declarations.
                                                                                                                 /// </summary>
                                                                                                                 /// <param name="context">The analysis context used to register analysis actions and configure analyzer behavior.</param>
                                                                                                                 /// <remarks>
                                                                                                                 /// The analyzer is configured to:
                                                                                                                 /// - Exclude generated code from analysis to avoid false positives on auto-generated test files
                                                                                                                 /// - Enable concurrent execution for better performance when analyzing large test suites
                                                                                                                 /// - Register syntax node analysis specifically for ClassDeclaration nodes to examine test classes
                                                                                                                 /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    /// <summary>
    /// Analyzes class declarations to detect xUnit v2 patterns that are incompatible with xUnit v3.
    /// This method examines each class to determine if it uses the problematic combination of
    /// constructor-based setup and IDisposable-based cleanup that may not work correctly in xUnit v3.
    /// </summary>
    /// <param name="context">The syntax node analysis context containing the class declaration to analyze and methods for reporting diagnostics.</param>
    /// <remarks>
    /// The analysis process:
    /// 1. Examines the class members to detect any constructor declarations
    /// 2. Checks the class's base type list for IDisposable interface implementation
    /// 3. Reports a diagnostic if both conditions are met (constructor + IDisposable)
    ///
    /// Detection Logic:
    /// - Constructor Detection: Uses LINQ to check if any class members are ConstructorDeclarationSyntax
    /// - IDisposable Detection: Uses text-based matching on base type list (simple but effective)
    ///
    /// Limitations:
    /// - Text-based IDisposable detection may produce false positives for similarly named types
    /// - Does not distinguish between test classes and regular classes (assumes test context)
    /// - Does not verify if constructors are actually used for test setup
    ///
    /// Future Enhancements:
    /// - Semantic analysis to verify actual IDisposable interface implementation
    /// - Detection of xUnit-specific attributes to confirm test class context
    /// - More sophisticated pattern recognition for xUnit v2 vs v3 lifecycle patterns
    /// - Integration with xUnit version detection to only warn when v3 upgrade is planned
    /// </remarks>
    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var hasCtor = classDecl.Members.OfType<ConstructorDeclarationSyntax>().Any();
        var hasDisposable = classDecl.BaseList?.Types.Any(t => t.ToString().Contains("IDisposable")) ?? false;

        if (hasCtor && hasDisposable)
        {
            var diagnostic = Diagnostic.Create(Rule, classDecl.Identifier.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
