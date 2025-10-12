using System.Collections.Immutable;
using System.Linq;
using IndFusion.Analyzers.Common;
using IndFusion.Analyzers.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.FunctionalPatterns;

/// <summary>
/// Analyzer that enforces Result&lt;T&gt; pattern instead of throwing exceptions.
/// Supports the core architectural principle of functional error handling.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotThrowExceptionsAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Do not throw exceptions - use Result<T> pattern instead";
    private static readonly LocalizableString MessageFormat = "Method throws exception '{0}' - use Result<T> pattern for functional error handling instead";
    private static readonly LocalizableString Description = "Exceptions should not be thrown in business logic. Use Result<T> pattern to represent success/failure states functionally. This improves composability, testability, and makes error paths explicit.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DoNotThrowExceptions,
        Title,
        MessageFormat,
        DiagnosticCategories.FunctionalPatterns,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeThrowStatement, SyntaxKind.ThrowStatement);
        context.RegisterSyntaxNodeAction(AnalyzeThrowExpression, SyntaxKind.ThrowExpression);
    }

    private static void AnalyzeThrowStatement(SyntaxNodeAnalysisContext context)
    {
        var throwStatement = (ThrowStatementSyntax)context.Node;

        // Skip rethrow statements (throw; without expression)
        if (throwStatement.Expression == null)
        {
            return;
        }

        // Heuristic-based exemptions
        if (ShouldSkipThrow(throwStatement, context.SemanticModel))
        {
            return;
        }

        // Get exception type name for reporting
        var exceptionType = GetExceptionTypeName(throwStatement.Expression);

        // Report diagnostic for exception throwing
        var diagnostic = Diagnostic.Create(
            Rule,
            throwStatement.GetLocation(),
            exceptionType);
        context.ReportDiagnostic(diagnostic);
    }

    private static void AnalyzeThrowExpression(SyntaxNodeAnalysisContext context)
    {
        var throwExpression = (ThrowExpressionSyntax)context.Node;

        if (ShouldSkipThrow(throwExpression, context.SemanticModel))
        {
            return;
        }

        var exceptionType = GetExceptionTypeName(throwExpression);
        var diagnostic = Diagnostic.Create(
            Rule,
            throwExpression.GetLocation(),
            exceptionType);
        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsInCatchBlockRethrow(ThrowStatementSyntax throwStatement)
    {
        // Check if this throw is inside a catch block
        var catchClause = throwStatement.FirstAncestorOrSelf<CatchClauseSyntax>();
        if (catchClause == null)
        {
            return false;
        }

        // Allow wrapping: throw new X(..., ex)
        if (throwStatement.Expression is ObjectCreationExpressionSyntax creation && creation.ArgumentList != null)
        {
            if (creation.ArgumentList.Arguments.Any(a => a.Expression is IdentifierNameSyntax id && (id.Identifier.Text == "ex" || id.Identifier.Text == "exception")))
            {
                return true;
            }
        }

        // Bare rethrow handled earlier; any other explicit throw in catch considered allowed when wrapping present
        return false;
    }

    private static string GetExceptionTypeName(ExpressionSyntax expression) => expression switch
    {
        ObjectCreationExpressionSyntax objectCreation when objectCreation.Type != null =>
            objectCreation.Type.ToString(),
        ThrowExpressionSyntax throwExpression when throwExpression.Expression is ObjectCreationExpressionSyntax obj =>
            obj.Type?.ToString() ?? "Exception",
        _ => "Exception"
    };

    private static bool IsInBoundaryLayer(SyntaxNode node)
    {
        var containingClass = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClass != null)
        {
            var name = containingClass.Identifier.Text;
            if (name.EndsWith("Controller") || name.EndsWith("Controllers"))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ShouldSkipThrow(SyntaxNode node, SemanticModel semanticModel)
    {
        // === Refactored: Get ancestors once at the top ===
        var containingClass = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        var containingMethod = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        var containingNamespace = containingClass?.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

        var className = containingClass?.Identifier.Text ?? string.Empty;
        var methodName = containingMethod?.Identifier.Text ?? string.Empty;
        var namespaceName = containingNamespace?.Name.ToString() ?? string.Empty;
        // === End Refactored ===

        var typeName = GetThrownTypeName(node);

        // Use the pre-calculated names instead of traversing the tree again in helpers
        if (IsInFrameworkBoundary(className, namespaceName)) return true;
        if (IsDomainValidation(className, methodName, namespaceName)) return true;
        if (IsConfigurationContext(className, namespaceName)) return true;

        // Test/benchmark/spec contexts
        if (className.EndsWith("Tests") || className.EndsWith("Specs") || className.EndsWith("Benchmarks")) return true;

        // Program/Startup/Bootstrap contexts
        if (className == "Program" || className == "Startup" || className == "Bootstrap") return true;

        // NotSupported/NotImplemented defensive throws
        if (!string.IsNullOrEmpty(typeName) &&
            (typeName.Contains("NotSupportedException") || typeName.Contains("NotImplementedException")))
        {
            return true;
        }

        // ArgumentNullException guard
        if (IsArgumentNullGuard(node)) return true;

        // Range guards
        if (IsRangeGuard(node)) return true;

        // Switch expression default arm throw
        if (node.Ancestors().OfType<SwitchExpressionArmSyntax>().Any()) return true;

        // Constructor invariants
        if (node.Ancestors().OfType<ConstructorDeclarationSyntax>().Any())
        {
            if (!string.IsNullOrEmpty(typeName) && (typeName.Contains("Argument") || typeName.Contains("InvalidOperationException"))) return true;
        }

        // Factory methods
        if ((methodName.StartsWith("Create") || methodName.StartsWith("Build") || methodName.Contains("Factory"))
            && !string.IsNullOrEmpty(typeName)
            && (typeName.Contains("ArgumentException") || typeName.Contains("ArgumentOutOfRangeException")))
        {
            return true;
        }

        // Expression-bodied guard
        if (node is ThrowExpressionSyntax te && te.Parent is ConditionalExpressionSyntax)
        {
            return true;
        }

        // Catch wrapping
        if (node is ThrowStatementSyntax ts && IsInCatchBlockRethrow(ts)) return true;

        return false;
    }

    private static string GetThrownTypeName(SyntaxNode node)
    {
        if (node is ThrowStatementSyntax ts && ts.Expression is ObjectCreationExpressionSyntax oc) return oc.Type.ToString();
        if (node is ThrowExpressionSyntax te && te.Expression is ObjectCreationExpressionSyntax oce) return oce.Type.ToString();
        return string.Empty;
    }

    private static bool IsArgumentNullGuard(SyntaxNode node)
    {
        // if (...) throw new ArgumentNullException(...)
        if (node is ThrowStatementSyntax ts && ts.Expression is ObjectCreationExpressionSyntax oc)
        {
            if (oc.Type.ToString().Contains("ArgumentNullException"))
            {
                var ifStmt = ts.Ancestors().OfType<IfStatementSyntax>().FirstOrDefault();
                if (ifStmt != null) return true;
            }
        }
        // x ?? throw new ArgumentNullException(...)
        if (node is ThrowExpressionSyntax te && te.Parent is BinaryExpressionSyntax be && be.IsKind(SyntaxKind.CoalesceExpression))
        {
            if (te.Expression is ObjectCreationExpressionSyntax oc2 && oc2.Type.ToString().Contains("ArgumentNullException")) return true;
        }
        return false;
    }

    private static bool IsRangeGuard(SyntaxNode node)
    {
        if (node is ThrowStatementSyntax ts && ts.Expression is ObjectCreationExpressionSyntax oc)
        {
            var typeText = oc.Type.ToString();
            if (typeText.Contains("ArgumentOutOfRangeException") || typeText.Contains("ArgumentException"))
            {
                var ifStmt = ts.Ancestors().OfType<IfStatementSyntax>().FirstOrDefault();
                if (ifStmt == null) return false;
                return ifStmt.Condition.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>().Any(b =>
                    b.IsKind(SyntaxKind.LessThanExpression) || b.IsKind(SyntaxKind.GreaterThanExpression) ||
                    b.IsKind(SyntaxKind.LessThanOrEqualExpression) || b.IsKind(SyntaxKind.GreaterThanOrEqualExpression));
            }
        }
        return false;
    }

    private static bool IsInFrameworkBoundary(string className, string namespaceName)
    {
        // ASP.NET Core patterns
        if (className.EndsWith("Controller") || 
            className.EndsWith("Middleware") ||
            className.EndsWith("Filter") ||
            className.EndsWith("Attribute") ||
            className.EndsWith("Hub"))
            return true;
        
        // Entity Framework patterns
        if (className.EndsWith("DbContext") ||
            className.EndsWith("Repository") ||
            className.EndsWith("UnitOfWork"))
            return true;
        
        // Infrastructure namespaces
        if (namespaceName.Contains("Infrastructure") ||
            namespaceName.Contains("Persistence") ||
            namespaceName.Contains("DataAccess"))
            return true;
        
        return false;
    }

    private static bool IsDomainValidation(string className, string methodName, string namespaceName)
    {
        // Domain validation patterns
        if (className.EndsWith("Validator") ||
            className.EndsWith("Parser") || // Added this line to fix the regression
            className.EndsWith("Rule") ||
            className.EndsWith("Policy") ||
            className.EndsWith("Specification"))
            return true;
        
        // Domain factory patterns
        if (methodName.StartsWith("Create") ||
            methodName.StartsWith("Build") ||
            methodName.Contains("Factory"))
            return true;
        
        // Domain namespace patterns
        if (namespaceName.Contains("Domain") ||
            namespaceName.Contains("Business"))
            return true;
        
        return false;
    }

    private static bool IsConfigurationContext(string className, string namespaceName)
    {
        // Configuration classes
        if (className.EndsWith("Settings") ||
            className.EndsWith("Config") ||
            className.EndsWith("Options") ||
            className.EndsWith("Configuration"))
            return true;
        
        // Configuration namespaces
        if (namespaceName.Contains("Configuration") ||
            namespaceName.Contains("Settings"))
            return true;
        
        return false;
    }
}