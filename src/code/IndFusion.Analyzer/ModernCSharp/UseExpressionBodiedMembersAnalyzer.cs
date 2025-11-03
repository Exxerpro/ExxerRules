using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.ModernCSharp;

/// <summary>
/// Analyzer that enforces using expression-bodied members where appropriate.
/// Supports the modern C# coding standards.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseExpressionBodiedMembersAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Title displayed for diagnostics when a member can be reduced to an expression-bodied form.
    /// </summary>
    private static readonly LocalizableString Title = "Use expression-bodied members where appropriate";

    /// <summary>
    /// Message format outlining the member that can be simplified.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Member '{0}' can be simplified to an expression-bodied member";

    /// <summary>
    /// Description explaining the benefits of expression-bodied members.
    /// </summary>
    private static readonly LocalizableString Description = "Expression-bodied members provide a more concise syntax for simple methods and properties, improving code readability and reducing boilerplate.";

    /// <summary>
    /// Diagnostic rule used to flag members that should adopt expression-bodied syntax.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseExpressionBodiedMembers,
        Title,
        MessageFormat,
        DiagnosticCategories.CodeQuality,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the expression-bodied members rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax callbacks that inspect methods and properties for expression-bodied opportunities.
    /// </summary>
    /// <param name="context">The analyzer context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TDD Green phase: Focus on methods and properties that can be expression-bodied
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    /// <summary>
    /// Examines method declarations and reports when they can be expression-bodied.
    /// </summary>
    /// <param name="context">The syntax analysis context for the method.</param>
    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        // Skip if already expression-bodied
        if (method.ExpressionBody != null)
        {
            return;
        }

        // Check if method has a simple single-return body
        if (method.Body != null && CanBeExpressionBodied(method.Body))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                method.Identifier.GetLocation(),
                method.Identifier.ValueText);
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Examines property declarations and reports when they can be expression-bodied.
    /// </summary>
    /// <param name="context">The syntax analysis context for the property.</param>
    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;

        // Skip if already expression-bodied
        if (property.ExpressionBody != null)
        {
            return;
        }

        // Check getter accessors that can be expression-bodied
        if (property.AccessorList != null)
        {
            foreach (var accessor in property.AccessorList.Accessors)
            {
                if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration) &&
                    accessor.Body != null &&
                    CanBeExpressionBodied(accessor.Body))
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        property.Identifier.GetLocation(),
                        property.Identifier.ValueText);
                    context.ReportDiagnostic(diagnostic);
                    break; // Only report once per property
                }
            }
        }
    }

    /// <summary>
    /// Determines whether the specified block body can be rewritten as an expression-bodied member.
    /// </summary>
    /// <param name="body">The block syntax to analyze.</param>
    /// <returns><c>true</c> when the block contains a simple return expression; otherwise, <c>false</c>.</returns>
    private static bool CanBeExpressionBodied(BlockSyntax body)
    {
        // Check if body contains only a single return statement
        if (body.Statements.Count != 1)
        {
            return false;
        }

        // Must be a return statement
        if (body.Statements[0] is not ReturnStatementSyntax returnStatement)
        {
            return false;
        }

        // Must have an expression to return
        if (returnStatement.Expression == null)
        {
            return false;
        }

        // Check for various exemption scenarios
        if (IsExemptFromExpressionBodied(body))
        {
            return false;
        }

        // Simple heuristic: expression should be reasonably simple
        // Avoid very complex expressions that would hurt readability
        var expressionText = returnStatement.Expression.ToString();
        return expressionText.Length < 100; // Arbitrary threshold for simplicity
    }

    //  False-Positive Mitigation Methods

    /// <summary>
    /// Determines whether a method body should be exempt from expression-bodied suggestions.
    /// </summary>
    /// <param name="body">The block syntax to evaluate.</param>
    /// <returns><c>true</c> when the block matches an exemption scenario; otherwise, <c>false</c>.</returns>
    private static bool IsExemptFromExpressionBodied(BlockSyntax body)
    {
        // Exemption: ICommandData factory methods maintain clarity in block form
        if (IsICommandDataFactoryMethod(body))
        {
            return true;
        }

        // Exemption: Fluent TODO stubs should remain verbose for future edits
        if (IsFluentTodoStub(body))
        {
            return true;
        }

        // Exemption: IResettable.TryReset conveys intent with block syntax
        if (IsIResettableTryResetMethod(body))
        {
            return true;
        }

        // Exemption: Fluent With*/Set* methods often expand with future logic
        if (IsFluentWithDataMethod(body))
        {
            return true;
        }

        // Exemption: Domain resetters frequently hold additional invariants
        if (IsDomainEntityResetter(body))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the enclosing method is an ICommandData factory that should remain block-bodied.
    /// </summary>
    /// <param name="body">The method body under evaluation.</param>
    /// <returns><c>true</c> when the method appears to create command or query data; otherwise, <c>false</c>.</returns>
    private static bool IsICommandDataFactoryMethod(BlockSyntax body)
    {
        // Check if we're in a method named "Create" that implements ICommandData.Create
        var methodDeclaration = body.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration == null || methodDeclaration.Identifier.Text != "Create")
        {
            return false;
        }

        // Check if the method returns a type that could be a command/query
        var returnType = methodDeclaration.ReturnType.ToString();
        if (returnType.Contains("Command") || returnType.Contains("Query") || returnType.Contains("ICommandData"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the body contains TODO scaffolding that should remain block-bodied.
    /// </summary>
    /// <param name="body">The method or accessor body being analyzed.</param>
    /// <returns><c>true</c> when TODO or FIXME comments are present; otherwise, <c>false</c>.</returns>
    private static bool IsFluentTodoStub(BlockSyntax body)
    {
        // Check if the body contains TODO or FIXME comments
        var bodyText = body.ToString();
        return bodyText.Contains("// TODO") || bodyText.Contains("// FIXME");
    }

    /// <summary>
    /// Determines whether the body belongs to an <c>IResettable.TryReset</c> implementation that should remain block-bodied.
    /// </summary>
    /// <param name="body">The method body under evaluation.</param>
    /// <returns><c>true</c> when the method name is <c>TryReset</c> and returns <c>bool</c>; otherwise, <c>false</c>.</returns>
    private static bool IsIResettableTryResetMethod(BlockSyntax body)
    {
        // Check if we're in a method named "TryReset"
        var methodDeclaration = body.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration == null || methodDeclaration.Identifier.Text != "TryReset")
        {
            return false;
        }

        // Check if the method returns bool
        var returnType = methodDeclaration.ReturnType.ToString();
        if (returnType == "bool")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the body belongs to a fluent configuration method expected to grow.
    /// </summary>
    /// <param name="body">The method body being analyzed.</param>
    /// <returns><c>true</c> when the method name starts with <c>With</c> or <c>Set</c> and contains future-work comments; otherwise, <c>false</c>.</returns>
    private static bool IsFluentWithDataMethod(BlockSyntax body)
    {
        // Check if we're in a method that starts with "With" or "Set"
        var methodDeclaration = body.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration == null)
        {
            return false;
        }

        var methodName = methodDeclaration.Identifier.Text;
        if (methodName.StartsWith("With") || methodName.StartsWith("Set"))
        {
            // Check if the body contains comments indicating future extension
            var bodyText = body.ToString();
            if (bodyText.Contains("// Future") || bodyText.Contains("// TODO") || bodyText.Contains("// FIXME"))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the body belongs to a domain reset method that should remain block-bodied.
    /// </summary>
    /// <param name="body">The method body being analyzed.</param>
    /// <returns><c>true</c> when the method name includes <c>Reset</c> or documentation indicates reset semantics; otherwise, <c>false</c>.</returns>
    private static bool IsDomainEntityResetter(BlockSyntax body)
    {
        // Check if we're in a method with "Reset" in the name
        var methodDeclaration = body.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration == null)
        {
            return false;
        }

        var methodName = methodDeclaration.Identifier.Text;
        if (methodName.Contains("Reset"))
        {
            // Check if the method returns bool
            var returnType = methodDeclaration.ReturnType.ToString();
            if (returnType == "bool")
            {
                return true;
            }
        }

        // Check if the method has XML documentation containing "reset"
        var xmlComments = methodDeclaration.GetLeadingTrivia()
            .Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                       t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
            .Select(t => t.ToString())
            .FirstOrDefault();

        if (xmlComments != null && xmlComments.ToLowerInvariant().Contains("reset"))
        {
            return true;
        }

        return false;
    }

     // 
}
