using System.Collections.Immutable;
using IndFusion.Analyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.Testing;

/// <summary>
/// Analyzer that enforces test naming convention: Should_Action_When_Condition.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestNamingConventionAnalyzer : DiagnosticAnalyzer
{
#pragma warning disable IDE1006
    /// <summary>
    /// Gets the localized title displayed when a test method violates the naming convention.
    /// </summary>
    private static readonly LocalizableString Title = "Test methods should follow naming convention";

    /// <summary>
    /// Gets the diagnostic message format describing the offending test method.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Test method '{0}' should follow naming convention: Should_Action_When_Condition";

    /// <summary>
    /// Gets the diagnostic description explaining the prescribed naming pattern.
    /// </summary>
    private static readonly LocalizableString Description = "Test methods should use descriptive names following the pattern Should_Action_When_Condition for better readability and maintainability.";

    /// <summary>
    /// The diagnostic emitted when a test method name falls outside the accepted pattern.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.TestNamingConvention,
        Title,
        MessageFormat,
        DiagnosticCategories.Testing,
        DiagnosticSeverity.Info,  // Changed from Warning to Info (suggestion)
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the test naming convention rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax callbacks that validate test method names against the project convention.
    /// </summary>
    /// <param name="context">The analysis context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    /// <summary>
    /// Validates a test method declaration against the configured naming convention.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // Check if this is a test method using ExxerRules.Analyzers.Operations pattern
        var testAttributeResult = PatternDetector.DetectTestAttributes(methodDeclaration, context.SemanticModel);
        if (testAttributeResult.IsFailure || testAttributeResult.Value is null || !testAttributeResult.Value.HasTestAttributes)
        {
            return;
        }

        // 1) Opt-out via custom attribute at method or containing type level
        if (HasOptOutAttribute(methodDeclaration, context))
        {
            return;
        }

        // 2) Honor DisplayName/Description overrides on test attributes
        if (HasDisplayNameOrDescriptionOverride(methodDeclaration, context))
        {
            return;
        }

        // 3) Relax for nested context classes (e.g., class When_... / Given_...)
        if (IsWithinContextClass(methodDeclaration))
        {
            return;
        }

        var methodName = methodDeclaration.Identifier.ValueText;

        // Validate naming convention using a more flexible pattern:
        // - Optional leading prefixes before "Should_" (e.g., MethodUnderTest_Should_... or Feature_Should_...)
        // - Allow behavior-only names (no explicit condition)
        // - Accept alternate connectors: When|For|With|If|On
        // - Support compound conditions with _And_/_With_ segments
        // - Permit lowercase tokens and optional Async suffixes (handled via IgnoreCase)
        const string flexiblePattern =
            @"^(?:[A-Za-z][A-Za-z0-9]*_)*Should_" +
            @"(?:[A-Za-z0-9]+(?:_[A-Za-z0-9]+)*)" +
            @"(?:_(?:When|For|With|If|On)_[A-Za-z0-9]+(?:_(?:And|With)_[A-Za-z0-9]+)*)?$";

        var namingValidationResult = PatternDetector.ValidateMethodNaming(methodName, flexiblePattern);

        // Use the extension method to report diagnostic if validation failed
        context.ReportDiagnosticIfFalse(
            namingValidationResult,
            Rule,
            methodDeclaration.Identifier.GetLocation(),
            methodName);
    }

    /// <summary>
    /// Determines whether the test method supplies a DisplayName or Description override.
    /// </summary>
    /// <param name="methodDeclaration">The method declaration under analysis.</param>
    /// <param name="context">The syntax node analysis context.</param>
    /// <returns><c>true</c> when an override is present; otherwise, <c>false</c>.</returns>
    private static bool HasDisplayNameOrDescriptionOverride(MethodDeclarationSyntax methodDeclaration, SyntaxNodeAnalysisContext context)
    {
        foreach (var attributeList in methodDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                // Consider only test attributes that could carry DisplayName/Description
                var attributeSymbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
                var attributeTypeName = attributeSymbol?.ContainingType?.ToDisplayString() ?? attribute.Name.ToString();

                // Heuristic: xUnit [Fact]/[Theory] commonly provide DisplayName; accept any attribute with named args DisplayName/Description
                if (attribute.ArgumentList != null)
                {
                    foreach (var arg in attribute.ArgumentList.Arguments)
                    {
                        if (arg.NameEquals is { } named && (named.Name.Identifier.ValueText == "DisplayName" || named.Name.Identifier.ValueText == "Description"))
                        {
                            return true;
                        }
                    }
                }

                // In case attribute is specified with named type, ensure we only consider known test attributes
                if (attributeTypeName.EndsWith(".Fact", StringComparison.OrdinalIgnoreCase) ||
                    attributeTypeName.EndsWith(".Theory", StringComparison.OrdinalIgnoreCase) ||
                    attributeTypeName.EndsWith("Fact", StringComparison.OrdinalIgnoreCase) ||
                    attributeTypeName.EndsWith("Theory", StringComparison.OrdinalIgnoreCase))
                {
                    // Already checked arguments above
                    continue;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the test method or containing class opts out of naming enforcement.
    /// </summary>
    /// <param name="methodDeclaration">The method declaration under analysis.</param>
    /// <param name="context">The syntax node analysis context.</param>
    /// <returns><c>true</c> when an opt-out attribute is present; otherwise, <c>false</c>.</returns>
    private static bool HasOptOutAttribute(MethodDeclarationSyntax methodDeclaration, SyntaxNodeAnalysisContext context)
    {
        static bool MatchesOptOut(AttributeSyntax attribute, SemanticModel semanticModel)
        {
            var name = attribute.Name.ToString();
            if (NameMatches(name))
            {
                return true;
            }

            var symbol = semanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;
            var typeName = symbol?.ContainingType?.Name;
            return typeName != null && NameMatches(typeName);

            static bool NameMatches(string n) =>
                n.Equals("AllowTestNamingVariations", StringComparison.OrdinalIgnoreCase) ||
                n.Equals("AllowTestNamingVariationsAttribute", StringComparison.OrdinalIgnoreCase);
        }

        // Method-level
        foreach (var list in methodDeclaration.AttributeLists)
        {
            foreach (var attr in list.Attributes)
            {
                if (MatchesOptOut(attr, context.SemanticModel))
                {
                    return true;
                }
            }
        }

        // Containing type-level (walk up to find nearest class)
        for (SyntaxNode? node = methodDeclaration.Parent; node != null; node = node.Parent)
        {
            if (node is ClassDeclarationSyntax classDecl)
            {
                foreach (var list in classDecl.AttributeLists)
                {
                    foreach (var attr in list.Attributes)
                    {
                        if (MatchesOptOut(attr, context.SemanticModel))
                        {
                            return true;
                        }
                    }
                }
                break;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the test method is nested in a context class that relaxes naming requirements.
    /// </summary>
    /// <param name="methodDeclaration">The method declaration under analysis.</param>
    /// <returns><c>true</c> when the method is within a contextual test class; otherwise, <c>false</c>.</returns>
    private static bool IsWithinContextClass(MethodDeclarationSyntax methodDeclaration)
    {
        for (SyntaxNode? node = methodDeclaration.Parent; node != null; node = node.Parent)
        {
            if (node is ClassDeclarationSyntax classDecl)
            {
                var name = classDecl.Identifier.ValueText;
                if (name.StartsWith("When_", StringComparison.OrdinalIgnoreCase) ||
                    name.StartsWith("Given_", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
