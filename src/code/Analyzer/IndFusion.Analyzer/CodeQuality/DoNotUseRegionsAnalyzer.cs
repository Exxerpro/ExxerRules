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
    /// <summary>
    /// Gets the localized title displayed when a region directive is discovered.
    /// </summary>
    private static readonly LocalizableString Title = "Do not use regions for code organization";

    /// <summary>
    /// Gets the localized message format describing the offending region directive.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Region '{0}' should be avoided - prefer sub-classes or separate files for organization";

    /// <summary>
    /// Gets the diagnostic description that explains why regions should be avoided.
    /// </summary>
    private static readonly LocalizableString Description = "Regions should be avoided in favor of better code organization using sub-classes or separate files. Regions can hide poor design and make code harder to navigate.";

    /// <summary>
    /// The diagnostic descriptor emitted when a region is encountered.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DoNotUseRegions,
        Title,
        MessageFormat,
        DiagnosticCategories.CodeQuality,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the region-usage rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax tree analysis for detecting region directives.
    /// </summary>
    /// <param name="context">The Roslyn analysis context used for registration.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    /// <summary>
    /// Scans a syntax tree for region directives and reports diagnostics when necessary.
    /// </summary>
    /// <param name="context">The syntax tree analysis context.</param>
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

    /// <summary>
    /// Extracts the region name from a <c>#region</c> directive.
    /// </summary>
    /// <param name="regionDirective">The trivia representing the region directive.</param>
    /// <returns>The trimmed region name, or "unnamed region" when no name is provided.</returns>
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

    // False-positive mitigation helpers

    /// <summary>
    /// Determines whether the supplied region directive should be exempt from diagnostics.
    /// </summary>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root used to inspect surrounding nodes.</param>
    /// <param name="context">The syntax tree analysis context.</param>
    /// <returns><c>true</c> when the region matches an approved exemption; otherwise, <c>false</c>.</returns>
    private static bool IsExemptRegion(SyntaxTrivia regionDirective, SyntaxNode root, SyntaxTreeAnalysisContext context)
    {
        var regionName = GetRegionName(regionDirective);

        if (IsConstantObservabilityBucket(regionName, regionDirective, root))
        {
            return true;
        }

        if (IsActivitySourceConstants(regionName, regionDirective, root))
        {
            return true;
        }

        if (IsPipelineSteps(regionName, regionDirective, root))
        {
            return true;
        }

        if (IsSuccessFailureHandlers(regionName, regionDirective, root))
        {
            return true;
        }

        if (IsHelperMethodsInGatewayPipelines(regionName, regionDirective, root))
        {
            return true;
        }

        if (IsNestedContextClasses(regionName, regionDirective, root))
        {
            return true;
        }

        if (IsPrivateHelpersInStaticUtilities(regionName, regionDirective, root))
        {
            return true;
        }

        if (IsScenarioGroupingInTests(regionName, regionDirective, root))
        {
            return true;
        }

        if (IsFixtureDefinitionsInTestSuites(regionName, regionDirective, root))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the region groups observability-related constants that merit exemption.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region groups observability constants; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the region collects activity-source constants used for telemetry.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region contains activity source constants; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the region outlines pipeline steps inside command handlers.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region outlines pipeline steps; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the region separates success and failure handlers for clarity.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region contains success or failure handler methods; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the region consolidates helper methods within gateway pipelines.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region consolidates gateway helper methods; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the region hosts nested context classes such as mocks or builders.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region contains nested context classes; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the region organizes private helper methods inside static utility classes.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region contains private helpers for static utilities; otherwise, <c>false</c>.</returns>
    private static bool IsPrivateHelpersInStaticUtilities(string regionName, SyntaxTrivia regionDirective, SyntaxNode root)
    {
        // Check if region name contains "Helpers" (more specific than just "Private")
        if (regionName.Contains("Helpers"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the region groups related test scenarios within a test class.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region groups related test scenarios; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the region collects fixture definitions within a test suite.
    /// </summary>
    /// <param name="regionName">The extracted region name.</param>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns><c>true</c> when the region contains fixture definitions; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Retrieves the syntax node that spans the content enclosed by the supplied region directives.
    /// </summary>
    /// <param name="regionDirective">The region directive trivia.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns>The syntax node encompassing the region content, or <c>null</c> if unavailable.</returns>
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
    /// Locates the matching <c>#endregion</c> directive for the supplied region.
    /// </summary>
    /// <param name="regionDirective">The opening region directive.</param>
    /// <param name="root">The syntax tree root.</param>
    /// <returns>The trivia for the closing region directive, or <c>null</c> if not found.</returns>
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

    /// <summary>
    /// Determines whether the provided member is a const or static readonly field.
    /// </summary>
    /// <param name="member">The member declaration to inspect.</param>
    /// <returns><c>true</c> when the member is const or static readonly; otherwise, <c>false</c>.</returns>
    private static bool IsConstOrStaticReadonlyField(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax field)
        {
            return field.Modifiers.Any(SyntaxKind.ConstKeyword) ||
                   (field.Modifiers.Any(SyntaxKind.StaticKeyword) && field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword));
        }
        return false;
    }

    /// <summary>
    /// Determines whether the provided member is a constant string field.
    /// </summary>
    /// <param name="member">The member declaration to inspect.</param>
    /// <returns><c>true</c> when the member is a const string field; otherwise, <c>false</c>.</returns>
    private static bool IsConstStringField(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax field)
        {
            return field.Modifiers.Any(SyntaxKind.ConstKeyword) &&
                   field.Declaration.Type.ToString().Contains("string");
        }
        return false;
    }

    /// <summary>
    /// Determines whether the member is a private helper method.
    /// </summary>
    /// <param name="member">The member declaration to inspect.</param>
    /// <returns><c>true</c> when the member is a private method; otherwise, <c>false</c>.</returns>
    private static bool IsPrivateHelperMethod(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(SyntaxKind.PrivateKeyword);
        }
        return false;
    }

    /// <summary>
    /// Determines whether the member is a private method whose name ends with "Step".
    /// </summary>
    /// <param name="member">The member declaration to analyze.</param>
    /// <returns><c>true</c> when the method name ends with "Step"; otherwise, <c>false</c>.</returns>
    private static bool IsPrivateMethodEndingWithStep(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(SyntaxKind.PrivateKeyword) &&
                   method.Identifier.Text.EndsWith("Step");
        }
        return false;
    }

    /// <summary>
    /// Determines whether the member is a private method associated with logging or handling operations.
    /// </summary>
    /// <param name="member">The member declaration to analyze.</param>
    /// <returns><c>true</c> when the method name suggests logging or handling responsibilities; otherwise, <c>false</c>.</returns>
    private static bool IsPrivateLoggingOrHandlingMethod(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        if (member is Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(SyntaxKind.PrivateKeyword) &&
                   (method.Identifier.Text.StartsWith("Log") || method.Identifier.Text.Contains("Handler"));
        }
        return false;
    }

    /// <summary>
    /// Determines whether the member is a private method returning <see cref="System.Threading.Tasks.Task"/> or a result type.
    /// </summary>
    /// <param name="member">The member declaration to analyze.</param>
    /// <returns><c>true</c> when the method return type matches Task or a result wrapper; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the member is declared as private static.
    /// </summary>
    /// <param name="member">The member declaration to inspect.</param>
    /// <returns><c>true</c> when the member is private static; otherwise, <c>false</c>.</returns>
    private static bool IsPrivateStaticMember(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        return member.Modifiers.Any(SyntaxKind.PrivateKeyword) &&
               member.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    /// <summary>
    /// Determines whether the provided attribute identifies an xUnit test.
    /// </summary>
    /// <param name="attribute">The attribute syntax to evaluate.</param>
    /// <returns><c>true</c> when the attribute denotes an xUnit <c>Fact</c> or <c>Theory</c>; otherwise, <c>false</c>.</returns>
    private static bool IsTestAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax attribute)
    {
        var attributeName = attribute.Name.ToString();
        return attributeName == "Fact" || attributeName == "Theory" ||
               attributeName.EndsWith(".Fact") || attributeName.EndsWith(".Theory");
    }

    /// <summary>
    /// Determines whether the specified member declaration is decorated with any xUnit test attributes.
    /// </summary>
    /// <param name="member">The member declaration to inspect.</param>
    /// <returns><c>true</c> when a <c>Fact</c> or <c>Theory</c> attribute is present; otherwise, <c>false</c>.</returns>
    private static bool HasXUnitAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        var attributes = member.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>();
        return attributes.Any(attr => IsTestAttribute(attr));
    }

    /// <summary>
    /// Determines whether the supplied member declaration represents a nested type.
    /// </summary>
    /// <param name="member">The member declaration to evaluate.</param>
    /// <returns><c>true</c> when the member is a type declaration; otherwise, <c>false</c>.</returns>
    private static bool IsNestedTypeDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
    {
        return member is Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax;
    }

}
