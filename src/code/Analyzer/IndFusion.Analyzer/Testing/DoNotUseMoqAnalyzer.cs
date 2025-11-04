using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.Testing;

/// <summary>
/// Analyzer that enforces using NSubstitute instead of Moq for mocking.
/// Supports the testing standards compliance principle.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseMoqAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the localized analyzer title displayed when Moq usage is detected.
    /// </summary>
    private static readonly LocalizableString Title = "Use NSubstitute instead of Moq for mocking";

    /// <summary>
    /// Gets the diagnostic message format describing the discovered Moq artifact.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Moq usage detected: '{0}' - use NSubstitute for consistent testing patterns";

    /// <summary>
    /// Gets the diagnostic description explaining the preference for NSubstitute.
    /// </summary>
    private static readonly LocalizableString Description = "NSubstitute provides a cleaner, more readable mocking syntax than Moq. It's the preferred mocking framework for this project to ensure consistent testing patterns and better maintainability.";

    /// <summary>
    /// The diagnostic emitted when Moq constructs are found in tests.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseNSubstitute,
        Title,
        MessageFormat,
        DiagnosticCategories.Testing,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the Moq usage rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax callbacks that flag Moq namespaces, object creation, and static helpers.
    /// </summary>
    /// <param name="context">The analysis context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TDD Green phase: Register all detection methods
        context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
    }


    /// <summary>
    /// Inspects using directives for Moq namespaces.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
    {
        var usingDirective = (UsingDirectiveSyntax)context.Node;

        var nameString = usingDirective.Name?.ToString();

        // Exact match for Moq (not NSubstitute or other frameworks)
        if (nameString == "Moq")
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                usingDirective.GetLocation(),
                "using Moq");
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Examines object creation expressions for <c>Mock</c> instantiations.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        // Check if creating Mock<T> instances (syntactic analysis first)
        if (objectCreation.Type is GenericNameSyntax genericName &&
            genericName.Identifier.ValueText == "Mock")
        {
            // For test scenarios, assume Mock<T> refers to Moq
            var diagnostic = Diagnostic.Create(
                Rule,
                objectCreation.GetLocation(),
                $"new {genericName}");
            context.ReportDiagnostic(diagnostic);
        }

        // Also check for non-generic Mock usage
        if (objectCreation.Type is IdentifierNameSyntax identifierName &&
            identifierName.Identifier.ValueText == "Mock")
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                objectCreation.GetLocation(),
                "new Mock");
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Detects Moq static helper usage accessed via the <c>Mock</c> type.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        var memberAccess = (MemberAccessExpressionSyntax)context.Node;

        // Check for Mock.* static method calls
        if (memberAccess.Expression is IdentifierNameSyntax identifier &&
            identifier.Identifier.ValueText == "Mock")
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                memberAccess.GetLocation(),
                $"Mock.{memberAccess.Name}");
            context.ReportDiagnostic(diagnostic);
        }
    }
}
