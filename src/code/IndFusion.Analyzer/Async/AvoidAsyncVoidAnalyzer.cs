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
			// Check all the exemption scenarios
			if (LooksLikeEventHandler(method) ||
				IsOverriddenMethod(method) ||
				IsInterfaceImplementationRequiringVoid(method, context.SemanticModel) ||
				IsICommandExecuteMethod(method, context.SemanticModel) ||
				IsBlazorComponentEventHandler(method, context.SemanticModel) ||
				HasFireAndForgetAttribute(method))
			{
				return;
			}

			var diagnostic = Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.Text);
			context.ReportDiagnostic(diagnostic);
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

		// Story 1.1: Allow nullable event handler parameters
		var firstType = first.TrimEnd('?');
		var secondType = second.TrimEnd('?');

		// Check for standard event handler pattern
		if ((firstType == "object" || firstType.EndsWith("Object")) && 
		    (secondType.EndsWith("EventArgs") || secondType.EndsWith("EventArgs")))
		{
			return true;
		}

		// Story 1.7: Allow custom event handler delegate patterns
		// Pattern: object sender, [any type] - common for custom event handlers
		if (firstType == "object" || firstType.EndsWith("Object"))
		{
			return true;
		}

		return false;
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

	#region Story 1.5: Allow Overridden async void Methods

	/// <summary>
	/// Checks if the method is an override method.
	/// </summary>
	private static bool IsOverriddenMethod(MethodDeclarationSyntax method)
	{
		return method.Modifiers.Any(SyntaxKind.OverrideKeyword);
	}

	#endregion

	#region Story 1.6: Allow Interface Implementations Requiring void

	/// <summary>
	/// Checks if the method is an interface implementation that requires void.
	/// </summary>
	private static bool IsInterfaceImplementationRequiringVoid(MethodDeclarationSyntax method, SemanticModel semanticModel)
	{
		var methodSymbol = semanticModel.GetDeclaredSymbol(method);
		if (methodSymbol == null)
		{
			return false;
		}

		// Check if method implements an interface member
		foreach (var interfaceSymbol in methodSymbol.ContainingType.AllInterfaces)
		{
			foreach (var interfaceMember in interfaceSymbol.GetMembers())
			{
				if (interfaceMember is IMethodSymbol interfaceMethod &&
					interfaceMethod.Name == methodSymbol.Name &&
					interfaceMethod.ReturnType.SpecialType == SpecialType.System_Void)
				{
					return true;
				}
			}
		}

		return false;
	}

	#endregion

	#region Story 1.4: Allow ICommand.Execute Methods

	/// <summary>
	/// Checks if the method is an ICommand.Execute implementation.
	/// </summary>
	private static bool IsICommandExecuteMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
	{
		// Check if method name is Execute
		if (method.Identifier.Text != "Execute")
		{
			return false;
		}

		// Check if the containing class implements ICommand by looking at the syntax
		var classDeclaration = method.FirstAncestorOrSelf<ClassDeclarationSyntax>();
		if (classDeclaration?.BaseList?.Types != null)
		{
			foreach (var baseType in classDeclaration.BaseList.Types)
			{
				var typeName = baseType.Type.ToString();
				if (typeName == "ICommand" || typeName.EndsWith(".ICommand"))
				{
					return true;
				}
			}
		}

		// Fallback: Check using semantic model if available
		var methodSymbol = semanticModel.GetDeclaredSymbol(method);
		if (methodSymbol != null)
		{
			// Check if the containing type implements ICommand
			foreach (var interfaceSymbol in methodSymbol.ContainingType.AllInterfaces)
			{
				if (interfaceSymbol.Name == "ICommand")
				{
					// Check if it's from System.Windows.Input namespace (handles both explicit and global usings)
					var containingNamespace = interfaceSymbol.ContainingNamespace;
					var namespaceDisplayString = containingNamespace?.ToDisplayString();

					if (containingNamespace?.Name == "Input" ||
						namespaceDisplayString == "System.Windows.Input" ||
						namespaceDisplayString == "global::System.Windows.Input")
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	#endregion

	#region Story 1.8: Allow Partial Methods in Blazor Components

	/// <summary>
	/// Checks if the method is a Blazor component event handler.
	/// </summary>
	private static bool IsBlazorComponentEventHandler(MethodDeclarationSyntax method, SemanticModel semanticModel)
	{
		// Check if method is private (typical for event handlers)
		if (!method.Modifiers.Any(SyntaxKind.PrivateKeyword))
		{
			return false;
		}

		// Check if the containing class inherits from ComponentBase
		var containingClass = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (containingClass == null)
		{
			return false;
		}

		var classSymbol = semanticModel.GetDeclaredSymbol(containingClass);
		if (classSymbol == null)
		{
			return false;
		}

		// Check if class inherits from ComponentBase
		if (!InheritsFromComponentBase(classSymbol))
		{
			return false;
		}

		// Check for common Blazor event handler patterns
		var methodName = method.Identifier.Text;
		return methodName.StartsWith("On") && methodName.EndsWith("Async") ||
		       methodName.Contains("Click") ||
		       methodName.Contains("Submit") ||
		       methodName.Contains("Change");
	}

	/// <summary>
	/// Checks if the class inherits from ComponentBase.
	/// </summary>
	private static bool InheritsFromComponentBase(INamedTypeSymbol classSymbol)
	{
		var current = classSymbol.BaseType;
		while (current != null)
		{
			if (current.Name == "ComponentBase" &&
				current.ContainingNamespace?.Name == "Components" &&
				current.ContainingNamespace.ContainingNamespace?.Name == "AspNetCore")
			{
				return true;
			}
			current = current.BaseType;
		}
		return false;
	}

	#endregion

	#region Story 1.9: Allow Fire-and-Forget Methods with an Attribute

	/// <summary>
	/// Checks if the method has a FireAndForget attribute.
	/// </summary>
	private static bool HasFireAndForgetAttribute(MethodDeclarationSyntax method)
	{
		var attributes = method.AttributeLists.SelectMany(al => al.Attributes);
		foreach (var attribute in attributes)
		{
			var attributeName = attribute.Name.ToString();
			if (attributeName == "FireAndForget" ||
				attributeName.EndsWith(".FireAndForget") ||
				attributeName.Contains("FireAndForget"))
			{
				return true;
			}
		}
		return false;
	}

	#endregion
}
