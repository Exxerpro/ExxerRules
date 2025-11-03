using System;
using System.Collections.Immutable;
using System.Linq;
using IndFusion.Analyzers.Common;
using IndFusion.Analyzers.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.ErrorHandling;

/// <summary>
/// Analyzer that detects methods throwing exceptions instead of returning Result&lt;T&gt;.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseResultPatternAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Title displayed with diagnostics emitted by this analyzer.
    /// </summary>
    private static readonly LocalizableString Title = "Use Result<T> pattern instead of throwing exceptions";

    /// <summary>
    /// Message format describing the violating member.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Method '{0}' throws exceptions but should return Result<T>";

    /// <summary>
    /// Description explaining why Result-based error handling is encouraged.
    /// </summary>
    private static readonly LocalizableString Description = "Methods should return Result<T> for error handling instead of throwing exceptions, following functional programming principles.";

    /// <summary>
    /// Diagnostic rule raised when a member throws exceptions instead of returning a Result.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseResultPattern,
        Title,
        MessageFormat,
        DiagnosticCategories.ErrorHandling,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the Result-pattern rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax node callbacks for methods, properties, local functions, and lambdas.
    /// </summary>
    /// <param name="context">The analyzer context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeLocalFunction, SyntaxKind.LocalFunctionStatement);
        context.RegisterSyntaxNodeAction(AnalyzeLambda, SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
    }

    /// <summary>
    /// Evaluates method declarations for inappropriate exception throwing.
    /// </summary>
    /// <param name="context">The syntax analysis context for the method.</param>
    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // Skip if method already returns Result<T>
        if (IsResultReturnType(methodDeclaration.ReturnType))
        {
            return;
        }

		// Opt-out via attributes on method or containing type
		if (HasOptOutAttribute(methodDeclaration))
		{
			return;
		}

		// Skip if this is a method that should be exempted
		if (IsSkippableMethod(methodDeclaration)
			|| IsInTestType(methodDeclaration)
			|| IsProgramOrStartup(methodDeclaration)
			|| IsIdentityComponentsNamespace(methodDeclaration)
			|| IsGuardTypeOrMethod(methodDeclaration)
			|| IsValueObjectContext(methodDeclaration)
			|| IsBackgroundTaskMethod(methodDeclaration)
			|| IsDomainGuardBoolMethod(methodDeclaration))
        {
            return;
        }

		// Skip boundary layers (e.g., controllers, web endpoints)
		if (IsInBoundaryLayer(methodDeclaration))
		{
			return;
		}


		// Check for throw statements and expressions, excluding those inside local functions or lambdas
		var throwStatements = methodDeclaration
			.DescendantNodes()
			.OfType<ThrowStatementSyntax>()
			.Where(t => !t.Ancestors().OfType<LocalFunctionStatementSyntax>().Any()
					&& !t.Ancestors().OfType<LambdaExpressionSyntax>().Any())
			.ToList();

		var throwExpressions = methodDeclaration
			.DescendantNodes()
			.OfType<ThrowExpressionSyntax>()
			.Where(t => !t.Ancestors().OfType<LocalFunctionStatementSyntax>().Any()
					&& !t.Ancestors().OfType<LambdaExpressionSyntax>().Any())
			.ToList();

        if (throwStatements.Any() || throwExpressions.Any())
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }

	/// <summary>
	/// Determines whether the supplied method should be excluded from Result-pattern enforcement.
	/// </summary>
	/// <param name="method">The method declaration to inspect.</param>
	/// <returns><c>true</c> when the method name or attributes indicate it should be skipped; otherwise, <c>false</c>.</returns>
	private static bool IsSkippableMethod(MethodDeclarationSyntax method)
    {
        // Skip constructors, destructors, and event handlers
        if (method.Identifier.Text.StartsWith("On") && method.Modifiers.Any(SyntaxKind.ProtectedKeyword))
        {
            return true;
        }

        // Skip Main method
        if (method.Identifier.Text == "Main")
        {
            return true;
        }

        // Skip methods with exception-related names
        if (method.Identifier.Text.Contains("Throw") || method.Identifier.Text.Contains("Exception"))
        {
            return true;
        }

        // Skip test methods
        var attributes = method.AttributeLists.SelectMany(al => al.Attributes);
        var testAttributeNames = new[] { "Fact", "Theory", "Test", "TestMethod", "TestCase" };

        if (attributes.Any(attr =>
			testAttributeNames.Any(name => attr.Name.ToString().Contains(name, StringComparison.OrdinalIgnoreCase))))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Evaluates property declarations for throw statements and expressions.
    /// </summary>
    /// <param name="context">The syntax analysis context for the property.</param>
    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

        // Skip if property already returns Result<T>
        if (IsResultReturnType(propertyDeclaration.Type))
        {
            return;
        }

		// Skip boundary layers (e.g., controllers, web endpoints)
		if (IsInBoundaryLayer(propertyDeclaration))
		{
			return;
		}

		// Check accessors for throw statements using functional approach
		var accessorThrows = CheckAccessorsForThrows(propertyDeclaration);
		var expressionThrows = CheckExpressionBodyForThrows(propertyDeclaration);

		// Allow property-level null-coalescing guard: `get => _x ?? throw ...;`
		if (IsNullCoalescingThrowGuard(propertyDeclaration))
		{
			return;
		}

		if (accessorThrows || expressionThrows)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                propertyDeclaration.Identifier.GetLocation(),
                propertyDeclaration.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Determines whether any property accessor contains throw statements or expressions.
    /// </summary>
    /// <param name="property">The property declaration to scan.</param>
    /// <returns><c>true</c> when an accessor throws; otherwise, <c>false</c>.</returns>
    private static bool CheckAccessorsForThrows(PropertyDeclarationSyntax property)
    {
        if (property.AccessorList == null)
        {
            return false;
        }

        return property.AccessorList.Accessors.Any(accessor =>
            accessor.DescendantNodes().OfType<ThrowStatementSyntax>().Any() ||
            accessor.DescendantNodes().OfType<ThrowExpressionSyntax>().Any());
    }

    /// <summary>
    /// Determines whether an expression-bodied property contains throw expressions.
    /// </summary>
    /// <param name="property">The property declaration to inspect.</param>
    /// <returns><c>true</c> when the expression body contains a throw; otherwise, <c>false</c>.</returns>
    private static bool CheckExpressionBodyForThrows(PropertyDeclarationSyntax property) => property.ExpressionBody?.DescendantNodes().OfType<ThrowExpressionSyntax>().Any() == true;

	/// <summary>
	/// Evaluates local functions for inappropriate exception throwing.
	/// </summary>
	/// <param name="context">The syntax analysis context for the local function.</param>
	private static void AnalyzeLocalFunction(SyntaxNodeAnalysisContext context)
    {
        var localFunction = (LocalFunctionStatementSyntax)context.Node;

        // Skip if function already returns Result<T>
        if (IsResultReturnType(localFunction.ReturnType))
        {
            return;
        }

		// Opt-out via containing type attributes
		if (HasOptOutAttribute(localFunction))
		{
			return;
		}

		// Skip boundary layers
		if (IsInBoundaryLayer(localFunction))
		{
			return;
		}

		// Skip typical validation helpers
		if (localFunction.Identifier.Text.Contains("Validate", StringComparison.OrdinalIgnoreCase)
			|| localFunction.Identifier.Text.Contains("Ensure", StringComparison.OrdinalIgnoreCase)
			|| localFunction.Identifier.Text.Contains("Guard", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		// Check for throw statements and expressions
        var throwStatements = localFunction.DescendantNodes().OfType<ThrowStatementSyntax>().ToList();
        var throwExpressions = localFunction.DescendantNodes().OfType<ThrowExpressionSyntax>().ToList();

        if (throwStatements.Any() || throwExpressions.Any())
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                localFunction.Identifier.GetLocation(),
                localFunction.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }

	/// <summary>
	/// Evaluates lambda expressions for throw usage that should be replaced with Result patterns.
	/// </summary>
	/// <param name="context">The syntax analysis context for the lambda expression.</param>
	private static void AnalyzeLambda(SyntaxNodeAnalysisContext context)
    {
        LambdaExpressionSyntax? lambda = context.Node switch
        {
            SimpleLambdaExpressionSyntax simple => simple,
            ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized,
            _ => null
        };

        if (lambda == null)
        {
            return;
        }

		// Skip boundary layers
		if (IsInBoundaryLayer(lambda))
		{
			return;
		}

		// Opt-out via containing type attributes
		if (HasOptOutAttribute(lambda))
		{
			return;
		}

		// Check for throw statements and expressions
        var throwStatements = lambda.DescendantNodes().OfType<ThrowStatementSyntax>().ToList();
        var throwExpressions = lambda.DescendantNodes().OfType<ThrowExpressionSyntax>().ToList();

        if (throwStatements.Any() || throwExpressions.Any())
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                lambda.GetLocation(),
                "Lambda expression");
            context.ReportDiagnostic(diagnostic);
        }
    }

	/// <summary>
	/// Determines whether the provided type syntax already represents a Result-based return type.
	/// </summary>
	/// <param name="typeSyntax">The type syntax to evaluate.</param>
	/// <returns><c>true</c> when the type is <c>Result</c> or wraps a <c>Result</c>; otherwise, <c>false</c>.</returns>
	private static bool IsResultReturnType(TypeSyntax? typeSyntax)
    {
        if (typeSyntax == null)
        {
            return false;
        }

        // Check for Result<T>, Task<Result<T>>, ValueTask<Result<T>>, etc.
        var typeText = typeSyntax.ToString();

        // Direct Result patterns
        if (typeText.Contains("Result<") || typeText.Contains("Result "))
        {
            return true;
        }

        // Task/ValueTask wrapping Result patterns
        if (typeText.Contains("Task<") && typeText.Contains("Result<"))
        {
            return true;
        }

        if (typeText.Contains("ValueTask<") && typeText.Contains("Result<"))
        {
            return true;
        }

        return false;
    }

	/// <summary>
	/// Determines whether the node is located within a boundary layer (controllers, API endpoints, etc.).
	/// </summary>
	/// <param name="node">The syntax node representing the analyzed member.</param>
	/// <returns><c>true</c> when the heuristics indicate a boundary layer; otherwise, <c>false</c>.</returns>
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

        var ns = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ?? string.Empty;
        if (!string.IsNullOrEmpty(ns))
        {
			if (ns.Contains(".Web", StringComparison.OrdinalIgnoreCase)
				|| ns.Contains(".Api", StringComparison.OrdinalIgnoreCase)
				|| ns.Contains(".Endpoints", StringComparison.OrdinalIgnoreCase)
				|| ns.Contains(".Presentation", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

	/// <summary>
	/// Determines whether the node or its containing type explicitly opts out of Result enforcement.
	/// </summary>
	/// <param name="node">The syntax node to inspect for opt-out attributes.</param>
	/// <returns><c>true</c> when an allow-throwing attribute is present; otherwise, <c>false</c>.</returns>
	private static bool HasOptOutAttribute(SyntaxNode node)
	{
		// Check attributes on method/local function parents and containing class
		var member = node as MemberDeclarationSyntax
			?? node.Ancestors().OfType<MemberDeclarationSyntax>().FirstOrDefault();
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();

		static bool HasAllowAttributes(SyntaxList<AttributeListSyntax> lists)
		{
			foreach (var attr in lists.SelectMany(a => a.Attributes))
			{
				var text = attr.Name.ToString();
				if (text.Contains("AllowExceptions", StringComparison.OrdinalIgnoreCase)
					|| text.Contains("AllowThrowing", StringComparison.OrdinalIgnoreCase)
					|| text.Contains("AllowGuardThrows", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		if (member is not null && HasAllowAttributes(member.AttributeLists)) return true;
		if (cls is not null && HasAllowAttributes(cls.AttributeLists)) return true;
		return false;
	}

	/// <summary>
	/// Determines whether the node resides within a Program or Startup-style bootstrapper.
	/// </summary>
	/// <param name="node">The syntax node to evaluate.</param>
	/// <returns><c>true</c> when the surrounding class is a bootstrap type; otherwise, <c>false</c>.</returns>
	private static bool IsProgramOrStartup(SyntaxNode node)
	{
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (cls is null) return false;
		var name = cls.Identifier.Text;
		return name.Equals("Program", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("Startup", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Determines whether the node is inside an Identity component namespace that permits throws.
	/// </summary>
	/// <param name="node">The syntax node to evaluate.</param>
	/// <returns><c>true</c> when the namespace indicates Identity Razor components; otherwise, <c>false</c>.</returns>
	private static bool IsIdentityComponentsNamespace(SyntaxNode node)
	{
		var ns = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ?? string.Empty;
		return ns.Contains(".Components.Account", StringComparison.OrdinalIgnoreCase)
			|| ns.Contains(".Identity.Components", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Determines whether the method or its containing type is explicitly designated as a guard helper.
	/// </summary>
	/// <param name="method">The method declaration to inspect.</param>
	/// <returns><c>true</c> when the method or containing type name indicates guard semantics; otherwise, <c>false</c>.</returns>
	private static bool IsGuardTypeOrMethod(MethodDeclarationSyntax method)
	{
		var type = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		var typeName = type?.Identifier.Text ?? string.Empty;
		if (typeName.Contains("Guard", StringComparison.OrdinalIgnoreCase)
			|| typeName.Contains("ThrowHelper", StringComparison.OrdinalIgnoreCase)
			|| typeName.Contains("Ensure", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		var name = method.Identifier.Text;
		return name.Contains("Guard", StringComparison.OrdinalIgnoreCase)
			|| name.Contains("ThrowHelper", StringComparison.OrdinalIgnoreCase)
			|| name.Contains("Ensure", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Determines whether the method belongs to a value-object type where guard throws are acceptable.
	/// </summary>
	/// <param name="method">The method declaration to inspect.</param>
	/// <returns><c>true</c> when the containing type name signals a value object; otherwise, <c>false</c>.</returns>
	private static bool IsValueObjectContext(MethodDeclarationSyntax method)
	{
		var type = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		var typeName = type?.Identifier.Text ?? string.Empty;
		string[] hints = [
			"Value", "Id", "Identifier", "Amount", "Money", "Metric", "Metrics", "Percentage", "Email", "Address"
		];
		return hints.Any(h => typeName.Contains(h, StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// Determines whether the method represents a background or hosted task where throws remain acceptable.
	/// </summary>
	/// <param name="method">The method declaration to inspect.</param>
	/// <returns><c>true</c> when heuristics identify a background task implementation; otherwise, <c>false</c>.</returns>
	private static bool IsBackgroundTaskMethod(MethodDeclarationSyntax method)
	{
		var returnText = method.ReturnType.ToString();
		if (!(returnText.Contains("Task", StringComparison.Ordinal) || returnText.Contains("ValueTask", StringComparison.Ordinal)))
		{
			return false;
		}
		var name = method.Identifier.Text;
		if (name.EndsWith("Async", StringComparison.OrdinalIgnoreCase) &&
			(name.StartsWith("Execute", StringComparison.OrdinalIgnoreCase)
				|| name.StartsWith("Handle", StringComparison.OrdinalIgnoreCase)
				|| name.StartsWith("Process", StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		// Attribute-based hint (e.g., HostedService) on method or type
		if (method.AttributeLists.SelectMany(a => a.Attributes).Any(a => a.Name.ToString().Contains("HostedService", StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		var type = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (type != null && type.AttributeLists.SelectMany(a => a.Attributes).Any(a => a.Name.ToString().Contains("HostedService", StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Determines whether a boolean-returning method functions as a domain guard and may throw.
	/// </summary>
	/// <param name="method">The method declaration to inspect.</param>
	/// <returns><c>true</c> when the method name indicates guard semantics for boolean checks; otherwise, <c>false</c>.</returns>
	private static bool IsDomainGuardBoolMethod(MethodDeclarationSyntax method)
	{
		// return type bool and name hints like Validate/AppliesTo/Ensure/IsValid
		var returnText = method.ReturnType.ToString();
		if (!returnText.Equals("bool", StringComparison.OrdinalIgnoreCase)) return false;
		var name = method.Identifier.Text;
		return name.Contains("Validate", StringComparison.OrdinalIgnoreCase)
			|| name.Contains("AppliesTo", StringComparison.OrdinalIgnoreCase)
			|| name.Contains("Ensure", StringComparison.OrdinalIgnoreCase)
			|| name.Contains("IsValid", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Determines whether the node is contained within a test-oriented type.
	/// </summary>
	/// <param name="node">The syntax node representing the analyzed member.</param>
	/// <returns><c>true</c> when the containing type name indicates tests or benchmarks; otherwise, <c>false</c>.</returns>
	private static bool IsInTestType(SyntaxNode node)
	{
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (cls is null) return false;
		var name = cls.Identifier.Text;
		return name.EndsWith("Tests", StringComparison.OrdinalIgnoreCase)
			|| name.EndsWith("Specs", StringComparison.OrdinalIgnoreCase)
			|| name.EndsWith("Benchmarks", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Determines whether a property uses a null-coalescing guard that should be allowed.
	/// </summary>
	/// <param name="property">The property declaration to inspect.</param>
	/// <returns><c>true</c> when the expression body employs <c>?? throw</c>; otherwise, <c>false</c>.</returns>
	private static bool IsNullCoalescingThrowGuard(PropertyDeclarationSyntax property)
	{
		if (property.ExpressionBody?.Expression is BinaryExpressionSyntax binary
			&& binary.IsKind(SyntaxKind.CoalesceExpression)
			&& binary.Right is ThrowExpressionSyntax)
		{
			return true;
		}
		return false;
	}
}
