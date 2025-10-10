using System.Collections.Immutable;
using System.Linq;
using IndFusion.Analyzers.Common;
using IndFusion.Analyzers.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.CodeQuality;

/// <summary>
/// Analyzer that enforces not using regions for code organization.
/// Supports the "prefer subclasses over regions" principle.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseRegionsAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Do not use regions for code organization";
    private static readonly LocalizableString MessageFormat = "Region '{0}' should be avoided - prefer sub-classes or separate files for organization";
    private static readonly LocalizableString Description = "Regions should be avoided in favor of better code organization using sub-classes or separate files. Regions can hide poor design and make code harder to navigate.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DoNotUseRegions,
        Title,
        MessageFormat,
        DiagnosticCategories.CodeQuality,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);

        // Find all region directives in the syntax tree
        var regionDirectives = root.DescendantTrivia()
            .Where(trivia => trivia.IsKind(SyntaxKind.RegionDirectiveTrivia))
            .ToList();

        foreach (var regionDirective in regionDirectives)
        {
            // Extract region name from the directive
            var regionName = GetRegionName(regionDirective);

            // Check if this region is exempt from reporting
            if (IsExemptRegion(regionDirective, root, context))
            {
                continue;
            }

            // Report diagnostic for each region
            var diagnostic = Diagnostic.Create(
                Rule,
                regionDirective.GetLocation(),
                regionName);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static string GetRegionName(SyntaxTrivia regionDirective)
    {
        // Get the text of the region directive and extract the name
        var text = regionDirective.ToString();

        // Remove "#region" prefix and trim whitespace
        var name = text.Replace("#region", "").Trim();

        // If no name was provided, use a default
        if (string.IsNullOrEmpty(name))
        {
            return "unnamed region";
        }

        return name;
    }

    #region False-Positive Mitigation Methods

    /// <summary>
    /// Determines if a region is exempt from the "do not use regions" rule.
    /// </summary>
    /// <summary>
    /// Determines if a region is exempt from the "do not use regions" rule.
    /// </summary>
    private static bool IsExemptRegion(SyntaxTrivia regionDirective, SyntaxNode root, SyntaxTreeAnalysisContext context)
    {
        var regionName = GetRegionName(regionDirective);

        // Story 1.1: Allow Regions for Constant Observability Buckets
        if (IsConstantObservabilityBucket(regionName, regionDirective, root))
        {
            return true;
        }

        // Story 1.2: Allow Regions for Activity Source Constants
        if (IsActivitySourceConstants(regionName, regionDirective, root))
        {
            return true;
        }

        // Story 1.3: Allow Regions for Pipeline Steps in Command Handlers
        if (IsPipelineSteps(regionName, regionDirective, root))
        {
            return true;
        }

        // Story 1.4: Allow Regions for Success/Failure Handlers
        if (IsSuccessFailureHandlers(regionName, regionDirective, root))
        {
            return true;
        }

        // Story 1.5: Allow Regions for Helper Methods in Gateway Pipelines
        if (IsHelperMethodsInGatewayPipelines(regionName, regionDirective, root))
        {
            return true;
        }

        // Story 1.6: Allow Regions for Nested Context Classes
        if (IsNestedContextClasses(regionName, regionDirective, root))
        {
            return true;
        }

        // Story 1.7: Allow Regions for Private Helpers in Static Utilities
        if (IsPrivateHelpersInStaticUtilities(regionName, regionDirective, root))
        {
            return true;
        }

        // Story 1.8: Allow Regions for Scenario Grouping in Tests
        if (IsScenarioGroupingInTests(regionName, regionDirective, root))
        {
            return true;
        }

        // Story 1.9: Allow Regions for Fixture Definitions in Test Suites
        if (IsFixtureDefinitionsInTestSuites(regionName, regionDirective, root))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.1: Allow Regions for Constant Observability Buckets
    /// </summary>
    /// <summary>
    /// Story 1.1: Allow Regions for Constant Observability Buckets
    /// </summary>
    private static bool IsConstantObservabilityBucket(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if region name suggests observability/logging constants
        if (regionName.Contains("Events") || regionName.Contains("Logging") || regionName.Contains("Constants"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.2: Allow Regions for Activity Source Constants
    /// </summary>
    /// <summary>
    /// Story 1.2: Allow Regions for Activity Source Constants
    /// </summary>
    private static bool IsActivitySourceConstants(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if region name suggests activity source constants
        if (regionName.Contains("Activities") || regionName.Contains("Activity") || regionName.Contains("Source"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.3: Allow Regions for Pipeline Steps in Command Handlers
    /// </summary>
    /// <summary>
    /// Story 1.3: Allow Regions for Pipeline Steps in Command Handlers
    /// </summary>
    private static bool IsPipelineSteps(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if region name contains "Pipeline"
        if (regionName.Contains("Pipeline"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.4: Allow Regions for Success/Failure Handlers
    /// </summary>
    /// <summary>
    /// Story 1.4: Allow Regions for Success/Failure Handlers
    /// </summary>
    private static bool IsSuccessFailureHandlers(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if region name contains "Success" or "Failure"
        if (regionName.Contains("Success") || regionName.Contains("Failure"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.5: Allow Regions for Helper Methods in Gateway Pipelines
    /// </summary>
    /// <summary>
    /// Story 1.5: Allow Regions for Helper Methods in Gateway Pipelines
    /// </summary>
    private static bool IsHelperMethodsInGatewayPipelines(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if region name contains "Helper"
        if (regionName.Contains("Helper"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.6: Allow Regions for Nested Context Classes
    /// </summary>
    /// <summary>
    /// Story 1.6: Allow Regions for Nested Context Classes
    /// </summary>
    private static bool IsNestedContextClasses(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if region name suggests context classes
        if (regionName.Contains("Context") || regionName.Contains("Result"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.7: Allow Regions for Private Helpers in Static Utilities
    /// </summary>
    /// <summary>
    /// Story 1.7: Allow Regions for Private Helpers in Static Utilities
    /// </summary>
    private static bool IsPrivateHelpersInStaticUtilities(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if region name contains "Private" or "Helpers"
        if (regionName.Contains("Private") || regionName.Contains("Helpers"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.8: Allow Regions for Scenario Grouping in Tests
    /// </summary>
    /// <summary>
    /// Story 1.8: Allow Regions for Scenario Grouping in Tests
    /// </summary>
    private static bool IsScenarioGroupingInTests(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if we're in a test file (contains test attributes)
        var hasTestAttributes = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>()
            .Any(attr => IsTestAttribute(attr));

        if (!hasTestAttributes)
        {
            return false;
        }

        // Check if region name suggests test scenarios
        if (regionName.Contains("Tests") || regionName.Contains("Test"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.9: Allow Regions for Fixture Definitions in Test Suites
    /// </summary>
    /// <summary>
    /// Story 1.9: Allow Regions for Fixture Definitions in Test Suites
    /// </summary>
    private static bool IsFixtureDefinitionsInTestSuites(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if we're in a test file
        var hasTestAttributes = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>()
            .Any(attr => IsTestAttribute(attr));

        if (!hasTestAttributes)
        {
            return false;
        }

        // Check if region name suggests fixtures
        if (regionName.Contains("Fixtures") || regionName.Contains("Fixture"))
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the content of a region (the nodes between #region and #endregion).
    /// </summary>
    /// <summary>
    /// Gets the content of a region (the nodes between #region and #endregion).
    /// </summary>
    /// <summary>
    /// Gets the content of a region (the nodes between #region and #endregion).
    /// </summary>
    private static SyntaxNode? GetRegionContent(SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Find the corresponding #endregion directive
        var endRegionDirective = FindEndRegionDirective(regionDirective, root);
        if (endRegionDirective == null)
        {
            return null;
        }

        // Get the parent node that contains both directives
        var regionParent = regionDirective.Token.Parent;
        var endRegionParent = endRegionDirective.Value.Token.Parent;

        if (regionParent == null || endRegionParent == null)
        {
            return null;
        }

        // Find the common ancestor that contains both directives
        var commonAncestor = regionParent.FirstAncestorOrSelf<SyntaxNode>(node => 
            node.Contains(endRegionParent));

        if (commonAncestor == null)
        {
            return null;
        }

        // For now, return the common ancestor as the region content
        // This is a simplified approach - in a real implementation, we'd need to
        // extract only the nodes between the two directives
        return commonAncestor;
    }

    /// <summary>
    /// Finds the corresponding #endregion directive for a #region directive.
    /// </summary>
    /// <summary>
    /// Finds the corresponding #endregion directive for a #region directive.
    /// </summary>
    private static SyntaxTrivia? FindEndRegionDirective(SyntaxTrivia regionDirective, SyntaxNode root)
    {
        var regionToken = regionDirective.Token;
        var regionPosition = regionToken.SpanStart;

        // Find all #endregion directives after this #region
        var endRegionDirectives = root.DescendantTrivia()
            .Where(trivia => trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia) && 
                           trivia.SpanStart > regionPosition)
            .OrderBy(trivia => trivia.SpanStart);

        // Return the first one (closest match)
        return endRegionDirectives.FirstOrDefault();
    }

    private static bool IsConstOrStaticReadonlyField(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax field)
        {
            return field.Modifiers.Any(SyntaxKind.ConstKeyword) ||
                   (field.Modifiers.Any(SyntaxKind.StaticKeyword) && field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword));
        }
        return false;
    }

    private static bool IsConstStringField(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax field)
        {
            return field.Modifiers.Any(SyntaxKind.ConstKeyword) &&
                   field.Declaration.Type.ToString().Contains("string");
        }
        return false;
    }

    private static bool IsPrivateHelperMethod(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(SyntaxKind.PrivateKeyword);
        }
        return false;
    }

    private static bool IsPrivateMethodEndingWithStep(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(SyntaxKind.PrivateKeyword) &&
                   method.Identifier.Text.EndsWith("Step");
        }
        return false;
    }

    private static bool IsPrivateLoggingOrHandlingMethod(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(SyntaxKind.PrivateKeyword) &&
                   (method.Identifier.Text.StartsWith("Log") || method.Identifier.Text.Contains("Handler"));
        }
        return false;
    }

    private static bool IsPrivateMethodReturningTaskOrResult(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method)
        {
            var returnType = method.ReturnType.ToString();
            return method.Modifiers.Any(SyntaxKind.PrivateKeyword) &&
                   (returnType.Contains("Task") || returnType.Contains("Result"));
        }
        return false;
    }

    private static bool IsPrivateStaticMember(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        return member.Modifiers.Any(SyntaxKind.PrivateKeyword) && 
               member.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    private static bool IsTestAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax attribute)
    {
        var attributeName = attribute.Name.ToString();
        return attributeName == "Fact" || attributeName == "Theory" || 
               attributeName.EndsWith(".Fact") || attributeName.EndsWith(".Theory");
    }

    private static bool HasXUnitAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        var attributes = member.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>();
        return attributes.Any(attr => IsTestAttribute(attr));
    }

    private static bool IsNestedTypeDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        return member is Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax;
    }

    #endregion
}
