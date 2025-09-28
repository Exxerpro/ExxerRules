using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Async;

/// <summary>
/// Analyzer that avoids async void methods except for event handlers.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AvoidAsyncVoidAnalyzer : DiagnosticAnalyzer
{
	private static readonly LocalizableString Title = "Avoid async void methods";
	private static readonly LocalizableString MessageFormat = "Method '{0}' should not be async void; return Task instead";
	private static readonly LocalizableString Description = "Async void methods are hard to test and can swallow exceptions. Prefer Task-returning async methods, except for event handlers.";

	private static readonly DiagnosticDescriptor Rule = new(
		DiagnosticIds.AvoidAsyncVoid,
		Title,
		MessageFormat,
		DiagnosticCategories.Async,
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: Description);

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
	}

	private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
	{
		var method = (MethodDeclarationSyntax)context.Node;

		// Only consider methods marked async
		if (!method.Modifiers.Any(SyntaxKind.AsyncKeyword))
		{
			return;
		}

		// Skip boundary layers (controllers/web) and skippables
		if (IsBoundaryOrSkippable(method))
		{
			return;
		}

		// If return type is void, and not an event handler, flag
		if (method.ReturnType is PredefinedTypeSyntax predefined && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword))
		{
			if (!LooksLikeEventHandler(method))
			{
				var diagnostic = Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}

	private static bool LooksLikeEventHandler(MethodDeclarationSyntax method)
	{
		// Common event handler signatures: object sender, EventArgs e (or derived)
		var parameters = method.ParameterList?.Parameters;
		if (parameters == null || parameters.Value.Count != 2)
		{
			return false;
		}

		var first = parameters.Value[0].Type?.ToString();
		var second = parameters.Value[1].Type?.ToString();
		if (first == null || second == null)
		{
			return false;
		}

		return first == "object" || first.EndsWith("Object") && (second.EndsWith("EventArgs") || second.EndsWith("EventArgs?"));
	}

	private static bool IsBoundaryOrSkippable(SyntaxNode node)
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
			if (ns.Contains(".Web") || ns.Contains(".Api") || ns.Contains(".Endpoints") || ns.Contains(".Presentation"))
			{
				return true;
			}
		}

		return false;
	}
}
