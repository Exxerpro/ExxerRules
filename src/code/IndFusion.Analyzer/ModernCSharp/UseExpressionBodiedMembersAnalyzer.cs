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
    private static readonly LocalizableString Title = "Use expression-bodied members where appropriate";
    private static readonly LocalizableString MessageFormat = "Member '{0}' can be simplified to an expression-bodied member";
    private static readonly LocalizableString Description = "Expression-bodied members provide a more concise syntax for simple methods and properties, improving code readability and reducing boilerplate.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseExpressionBodiedMembers,
        Title,
        MessageFormat,
        DiagnosticCategories.CodeQuality,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TDD Green phase: Focus on methods and properties that can be expression-bodied
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

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

    #region False-Positive Mitigation Methods

    /// <summary>
    /// Determines if a method body is exempt from expression-bodied member suggestions.
    /// </summary>
    private static bool IsExemptFromExpressionBodied(BlockSyntax body)
    {
        // Story 1.1: Exempt ICommandData Factory Methods
        if (IsICommandDataFactoryMethod(body))
        {
            return true;
        }

        // Story 1.2: Exempt Fluent TODO Stubs
        if (IsFluentTodoStub(body))
        {
            return true;
        }

        // Story 1.3: Exempt IResettable.TryReset Methods
        if (IsIResettableTryResetMethod(body))
        {
            return true;
        }

        // Story 1.4: Exempt Fluent WithData Methods
        if (IsFluentWithDataMethod(body))
        {
            return true;
        }

        // Story 1.5: Exempt Domain Entity Resetters
        if (IsDomainEntityResetter(body))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.1: Exempt ICommandData Factory Methods
    /// </summary>
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
    /// Story 1.2: Exempt Fluent TODO Stubs
    /// </summary>
    private static bool IsFluentTodoStub(BlockSyntax body)
    {
        // Check if the body contains TODO or FIXME comments
        var bodyText = body.ToString();
        return bodyText.Contains("// TODO") || bodyText.Contains("// FIXME");
    }

    /// <summary>
    /// Story 1.3: Exempt IResettable.TryReset Methods
    /// </summary>
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
    /// Story 1.4: Exempt Fluent WithData Methods
    /// </summary>
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
    /// Story 1.5: Exempt Domain Entity Resetters
    /// </summary>
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

    #endregion
}
