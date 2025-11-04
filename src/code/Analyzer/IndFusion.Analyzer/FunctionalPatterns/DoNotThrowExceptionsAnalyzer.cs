using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.FunctionalPatterns;

/// <summary>
/// Analyzer that enforces Result&lt;T&gt; pattern instead of throwing exceptions.
/// Supports the core architectural principle of functional error handling.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotThrowExceptionsAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Title displayed for diagnostics emitted by this analyzer.
    /// </summary>
    private static readonly LocalizableString Title = "Do not throw exceptions - use Result<T> pattern instead";

    /// <summary>
    /// Message format describing the offending exception type.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Method throws exception '{0}' - use Result<T> pattern for functional error handling instead";

    /// <summary>
    /// Description explaining the benefits of Result-based error handling.
    /// </summary>
    private static readonly LocalizableString Description = "Exceptions should not be thrown in business logic. Use Result<T> pattern to represent success/failure states functionally. This improves composability, testability, and makes error paths explicit.";

    /// <summary>
    /// Diagnostic rule issued when disallowed exceptions are thrown.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DoNotThrowExceptions,
        Title,
        MessageFormat,
        DiagnosticCategories.FunctionalPatterns,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the Result enforcement rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax callbacks for throw statements and expressions.
    /// </summary>
    /// <param name="context">The analysis context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeThrowStatement, SyntaxKind.ThrowStatement);
        context.RegisterSyntaxNodeAction(AnalyzeThrowExpression, SyntaxKind.ThrowExpression);
    }

    /// <summary>
    /// Evaluates throw statements and reports diagnostics when they are not exempt.
    /// </summary>
    /// <param name="context">The syntax analysis context for the throw statement.</param>
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

    /// <summary>
    /// Evaluates throw expressions and reports diagnostics when they are not exempt.
    /// </summary>
    /// <param name="context">The syntax analysis context for the throw expression.</param>
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

    /// <summary>
    /// Determines whether a throw statement is part of a catch block rethrow pattern that should be allowed.
    /// </summary>
    /// <param name="throwStatement">The throw statement under evaluation.</param>
    /// <returns><c>true</c> when the throw wraps the original exception within a catch block; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Resolves a human-readable exception type name from the supplied expression.
    /// </summary>
    /// <param name="expression">The expression associated with the throw.</param>
    /// <returns>The resolved exception type name, or <c>Exception</c> when it cannot be determined.</returns>
    private static string GetExceptionTypeName(ExpressionSyntax expression) => expression switch
    {
        ObjectCreationExpressionSyntax objectCreation when objectCreation.Type != null =>
            objectCreation.Type.ToString(),
        ThrowExpressionSyntax throwExpression when throwExpression.Expression is ObjectCreationExpressionSyntax obj =>
            obj.Type?.ToString() ?? "Exception",
        _ => "Exception"
    };

    /// <summary>
    /// Determines whether the throw occurs inside a boundary layer such as controllers or middleware.
    /// </summary>
    /// <param name="node">The syntax node representing the throw location.</param>
    /// <returns><c>true</c> when the surrounding type matches boundary heuristics; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Applies exemption heuristics to determine whether a throw should be ignored.
    /// </summary>
    /// <param name="node">The syntax node representing the throw statement or expression.</param>
    /// <param name="semanticModel">The semantic model for additional context.</param>
    /// <returns><c>true</c> when any exemption applies; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Retrieves the textual exception type name from the supplied throw syntax.
    /// </summary>
    /// <param name="node">The syntax node representing the throw.</param>
    /// <returns>The exception type name when available; otherwise, <see cref="string.Empty"/>.</returns>
    private static string GetThrownTypeName(SyntaxNode node)
    {
        if (node is ThrowStatementSyntax ts && ts.Expression is ObjectCreationExpressionSyntax oc) return oc.Type.ToString();
        if (node is ThrowExpressionSyntax te && te.Expression is ObjectCreationExpressionSyntax oce) return oce.Type.ToString();
        return string.Empty;
    }

    /// <summary>
    /// Determines whether the throw implements an <see cref="System.ArgumentNullException"/> guard.
    /// </summary>
    /// <param name="node">The syntax node representing the throw.</param>
    /// <returns><c>true</c> when the throw enforces argument null checks; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the throw enforces an argument range guard.
    /// </summary>
    /// <param name="node">The syntax node representing the throw.</param>
    /// <returns><c>true</c> when the guard matches range-check heuristics; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the throw occurs within infrastructural or framework boundary code.
    /// </summary>
    /// <param name="className">The name of the containing class.</param>
    /// <param name="namespaceName">The containing namespace.</param>
    /// <returns><c>true</c> when heuristics indicate a framework boundary; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the throw belongs to domain validation or factory routines where exceptions are tolerated.
    /// </summary>
    /// <param name="className">The containing class name.</param>
    /// <param name="methodName">The containing method name.</param>
    /// <param name="namespaceName">The containing namespace.</param>
    /// <returns><c>true</c> when the heuristics identify domain validation scenarios; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the throw occurs within configuration code where exceptions are acceptable.
    /// </summary>
    /// <param name="className">The containing class name.</param>
    /// <param name="namespaceName">The containing namespace.</param>
    /// <returns><c>true</c> when the context represents configuration scenarios; otherwise, <c>false</c>.</returns>
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
