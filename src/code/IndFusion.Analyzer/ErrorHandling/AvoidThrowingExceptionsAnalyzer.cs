using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.ErrorHandling;

/// <summary>
/// Analyzer that detects direct exception throwing in code.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AvoidThrowingExceptionsAnalyzer : DiagnosticAnalyzer
{
	private static readonly LocalizableString Title = "Avoid throwing exceptions";
	private static readonly LocalizableString MessageFormat = "Throwing '{0}' detected. Use Result<T> pattern instead.";
	private static readonly LocalizableString Description = "Exceptions should be avoided in favor of the Result<T> pattern for better functional programming and error handling.";

	private static readonly DiagnosticDescriptor Rule = new(
		DiagnosticIds.AvoidThrowingExceptions,
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

		context.RegisterSyntaxNodeAction(AnalyzeThrowStatement, SyntaxKind.ThrowStatement);
		context.RegisterSyntaxNodeAction(AnalyzeThrowExpression, SyntaxKind.ThrowExpression);
	}

	private static void AnalyzeThrowStatement(SyntaxNodeAnalysisContext context)
	{
		var throwStatement = (ThrowStatementSyntax)context.Node;

		// Skip if in catch block and just rethrowing
		if (throwStatement.Expression == null && throwStatement.Parent is BlockSyntax block &&
			block.Parent is CatchClauseSyntax)
		{
			return;
		}

		// Skip in exception filter
		if (throwStatement.Ancestors().OfType<CatchFilterClauseSyntax>().Any())
		{
			return;
		}

		// Context-based exemptions
		if (HasOptOutAttribute(throwStatement)
			|| IsInTestContext(throwStatement)
			|| IsInBoundaryLayer(throwStatement)
			|| IsProgramOrStartup(throwStatement)
			|| IsIdentityComponentsNamespace(throwStatement)
			|| IsThrowHelperOrGuardContext(throwStatement)
			|| IsValueObjectContext(throwStatement)
			|| IsRangeOrNullGuardPattern(throwStatement, context.SemanticModel)
			|| IsExceptionWrappingPattern(throwStatement))
		{
			return;
		}

		var exceptionType = GetExceptionType(throwStatement.Expression, context.SemanticModel);
		var diagnostic = Diagnostic.Create(
			Rule,
			throwStatement.GetLocation(),
			exceptionType);

		context.ReportDiagnostic(diagnostic);
	}

	private static void AnalyzeThrowExpression(SyntaxNodeAnalysisContext context)
	{
		var throwExpression = (ThrowExpressionSyntax)context.Node;

		// Context-based exemptions
		if (HasOptOutAttribute(throwExpression)
			|| IsInTestContext(throwExpression)
			|| IsInBoundaryLayer(throwExpression)
			|| IsProgramOrStartup(throwExpression)
			|| IsIdentityComponentsNamespace(throwExpression)
			|| IsThrowHelperOrGuardContext(throwExpression)
			|| IsValueObjectContext(throwExpression)
			|| IsNullCoalescingGuard(throwExpression, context.SemanticModel))
		{
			return;
		}

		var exceptionType = GetExceptionType(throwExpression.Expression, context.SemanticModel);
		var diagnostic = Diagnostic.Create(
			Rule,
			throwExpression.GetLocation(),
			exceptionType);

		context.ReportDiagnostic(diagnostic);
	}

	private static bool IsInBoundaryLayer(SyntaxNode node)
	{
		// Heuristics: class names ending with Controller, or namespaces containing .Web, .Api, .Endpoints
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
			if (ns.Contains(".Web") || ns.Contains(".Api") || ns.Contains(".Endpoints") || ns.Contains(".Presentation"))
			{
				return true;
			}
		}

		return false;
	}

	private static bool IsInTestContext(SyntaxNode node)
	{
		// Method attribute check or type name suffixes
		var containingMethod = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
		if (containingMethod != null)
		{
			var attributes = containingMethod.AttributeLists.SelectMany(al => al.Attributes);
			var testAttributeNames = new[] { "Fact", "Theory", "Test", "TestMethod", "TestCase" };
			if (attributes.Any(a => testAttributeNames.Any(n => a.Name.ToString().Contains(n))))
			{
				return true;
			}
		}
		var containingClass = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (containingClass != null)
		{
			var className = containingClass.Identifier.Text;
			if (className.EndsWith("Tests") || className.EndsWith("Test") || className.EndsWith("Specs") || className.EndsWith("Benchmarks"))
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasOptOutAttribute(SyntaxNode node)
	{
		var member = node.Ancestors().OfType<MemberDeclarationSyntax>().FirstOrDefault();
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		bool HasAllow(AttributeListSyntax list)
			=> list.Attributes.Any(a =>
				{
					var text = a.Name.ToString();
					return text.Contains("AllowExceptions") || text.Contains("AllowThrowing") || text.Contains("AllowGuardThrows");
				});
		if (member?.AttributeLists.Any(HasAllow) == true) return true;
		if (cls?.AttributeLists.Any(HasAllow) == true) return true;
		return false;
	}

	private static bool IsProgramOrStartup(SyntaxNode node)
	{
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		var name = cls?.Identifier.Text ?? string.Empty;
		return name == "Program" || name == "Startup" || name == "Bootstrap" || name == "AppHost";
	}

	private static bool IsIdentityComponentsNamespace(SyntaxNode node)
	{
		var ns = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ?? string.Empty;
		return ns.Contains(".Components.Account") || ns.Contains(".Identity.Components");
	}

	private static bool IsThrowHelperOrGuardContext(SyntaxNode node)
	{
		var method = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		var methodName = method?.Identifier.Text ?? string.Empty;
		var className = cls?.Identifier.Text ?? string.Empty;
		return methodName.Contains("ThrowHelper", System.StringComparison.OrdinalIgnoreCase)
			|| methodName.Contains("ThrowIf", System.StringComparison.OrdinalIgnoreCase)
			|| methodName.Contains("Guard", System.StringComparison.OrdinalIgnoreCase)
			|| methodName.Contains("Ensure", System.StringComparison.OrdinalIgnoreCase)
			|| methodName.Contains("ExceptionHelper", System.StringComparison.OrdinalIgnoreCase)
			|| className.Contains("ThrowHelper", System.StringComparison.OrdinalIgnoreCase)
			|| className.Contains("Guard", System.StringComparison.OrdinalIgnoreCase)
			|| className.Contains("Ensure", System.StringComparison.OrdinalIgnoreCase)
			|| className.Contains("ExceptionHelper", System.StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsValueObjectContext(SyntaxNode node)
	{
		var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		var className = cls?.Identifier.Text ?? string.Empty;
		string[] hints = { "Value", "Id", "Identifier", "Amount", "Money", "Metric", "Metrics", "Percentage", "Email", "Address" };
		return hints.Any(h => className.Contains(h, System.StringComparison.OrdinalIgnoreCase));
	}

	private static bool IsRangeOrNullGuardPattern(SyntaxNode node, SemanticModel semanticModel)
	{
		// Only treat as allowed when:
		// - Inside an if-statement with comparison (<, >, <=, >=)
		// - And the thrown exception is ArgumentOutOfRangeException
		var ifStmt = node.Ancestors().OfType<IfStatementSyntax>().FirstOrDefault();
		if (ifStmt is null)
		{
			return false;
		}
		var hasComparison = ifStmt.Condition.DescendantNodesAndSelf().OfType<BinaryExpressionSyntax>()
			.Any(be => be.IsKind(SyntaxKind.LessThanExpression)
				|| be.IsKind(SyntaxKind.GreaterThanExpression)
				|| be.IsKind(SyntaxKind.LessThanOrEqualExpression)
				|| be.IsKind(SyntaxKind.GreaterThanOrEqualExpression));
		if (!hasComparison)
		{
			return false;
		}
		if (node is ThrowStatementSyntax ts && ts.Expression is ObjectCreationExpressionSyntax oc)
		{
			var typeText = oc.Type.ToString();
			if (!typeText.Contains("ArgumentOutOfRangeException", System.StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			// Gate to well-known guard classes (e.g., Clock/Rules) to avoid suppressing generic services
			var cls = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
			var className = cls?.Identifier.Text ?? string.Empty;
			string[] allowed = { "Clock", "Rule", "Rules", "Shift" };
			return allowed.Any(a => className.Contains(a, System.StringComparison.OrdinalIgnoreCase));
		}
		return false;
	}

	private static bool IsNullCoalescingGuard(ThrowExpressionSyntax throwExpr, SemanticModel semanticModel)
	{
		// parent should be right side of coalesce
		if (throwExpr.Parent is BinaryExpressionSyntax binary && binary.IsKind(SyntaxKind.CoalesceExpression))
		{
			return true;
		}
		return false;
	}

	private static bool IsExceptionWrappingPattern(ThrowStatementSyntax throwStatement)
	{
		// catch (Exception ex) { throw new InvalidOperationException("...", ex); }
		if (throwStatement.Expression is ObjectCreationExpressionSyntax creation && creation.ArgumentList != null)
		{
			var hasInner = creation.ArgumentList.Arguments.Any(a => a.Expression is IdentifierNameSyntax id && id.Identifier.Text is "ex" or "exception" or "inner");
			if (hasInner && throwStatement.Ancestors().OfType<CatchClauseSyntax>().Any())
			{
				return true;
			}
		}
		return false;
	}

	private static string GetExceptionType(ExpressionSyntax? expression, SemanticModel semanticModel)
	{
		if (expression == null)
		{
			return "exception";
		}

		var typeInfo = semanticModel.GetTypeInfo(expression);
		if (typeInfo.Type != null)
		{
			return typeInfo.Type.Name;
		}

		// Try to get type from object creation
		if (expression is ObjectCreationExpressionSyntax objectCreation)
		{
			return objectCreation.Type.ToString();
		}

		return "exception";
	}
}
