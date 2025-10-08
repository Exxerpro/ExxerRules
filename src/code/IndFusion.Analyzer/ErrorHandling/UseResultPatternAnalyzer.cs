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
    private static readonly LocalizableString Title = "Use Result<T> pattern instead of throwing exceptions";
    private static readonly LocalizableString MessageFormat = "Method '{0}' throws exceptions but should return Result<T>";
    private static readonly LocalizableString Description = "Methods should return Result<T> for error handling instead of throwing exceptions, following functional programming principles.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseResultPattern,
        Title,
        MessageFormat,
        DiagnosticCategories.ErrorHandling,
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

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeLocalFunction, SyntaxKind.LocalFunctionStatement);
        context.RegisterSyntaxNodeAction(AnalyzeLambda, SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
    }

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

    private static bool CheckExpressionBodyForThrows(PropertyDeclarationSyntax property) => property.ExpressionBody?.DescendantNodes().OfType<ThrowExpressionSyntax>().Any() == true;

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

	private static bool IsProgramOrStartup(SyntaxNode node)
	{
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (cls is null) return false;
		var name = cls.Identifier.Text;
		return name.Equals("Program", StringComparison.OrdinalIgnoreCase)
			|| name.Equals("Startup", StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsIdentityComponentsNamespace(SyntaxNode node)
	{
		var ns = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ?? string.Empty;
		return ns.Contains(".Components.Account", StringComparison.OrdinalIgnoreCase)
			|| ns.Contains(".Identity.Components", StringComparison.OrdinalIgnoreCase);
	}

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

	private static bool IsValueObjectContext(MethodDeclarationSyntax method)
	{
		var type = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		var typeName = type?.Identifier.Text ?? string.Empty;
		string[] hints = [
			"Value", "Id", "Identifier", "Amount", "Money", "Metric", "Metrics", "Percentage", "Email", "Address"
		];
		return hints.Any(h => typeName.Contains(h, StringComparison.OrdinalIgnoreCase));
	}

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

	private static bool IsInTestType(SyntaxNode node)
	{
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (cls is null) return false;
		var name = cls.Identifier.Text;
		return name.EndsWith("Tests", StringComparison.OrdinalIgnoreCase)
			|| name.EndsWith("Specs", StringComparison.OrdinalIgnoreCase)
			|| name.EndsWith("Benchmarks", StringComparison.OrdinalIgnoreCase);
	}

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
