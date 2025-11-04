using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.Async;

/// <summary>
/// Flags <c>async void</c> methods so teams prefer <see cref="System.Threading.Tasks.Task"/>-returning async APIs outside valid event-handling scenarios.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AvoidAsyncVoidAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the localized diagnostic title surfaced when <c>async void</c> methods are discovered.
    /// </summary>
    private static readonly LocalizableString Title = "Avoid async void methods";

    /// <summary>
    /// Gets the localized message format describing the offending method name.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Method '{0}' should not be async void; return Task instead";

    /// <summary>
    /// Gets the descriptive text that explains why <c>async void</c> should be avoided.
    /// </summary>
    private static readonly LocalizableString Description = "Async void methods are hard to test and can swallow exceptions. Prefer Task-returning async methods, except for event handlers.";

    /// <summary>
    /// The diagnostic descriptor emitted when an <c>async void</c> method violates the rule.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.AvoidAsyncVoid,
        Title,
        MessageFormat,
        DiagnosticCategories.Async,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the async-void warning rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers the syntax callbacks that detect prohibited <c>async void</c> usage.
    /// </summary>
    /// <param name="context">The Roslyn analysis context used for registration.</param>
    /// <remarks>
    /// Generated code is excluded and concurrent execution is enabled before scanning <see cref="MethodDeclarationSyntax"/> nodes.
    /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    /// <summary>
    /// Analyzes method declarations and reports diagnostics for disallowed <c>async void</c> patterns.
    /// </summary>
    /// <param name="context">The syntax analysis context supplying the method declaration.</param>
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

    /// <summary>
    /// Determines whether the method signature matches common event-handler patterns that may legitimately remain <c>async void</c>.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the signature resembles an event handler; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the method resides within boundary layers or other contexts that are exempt from this rule.
    /// </summary>
    /// <param name="node">The syntax node whose ancestors are inspected.</param>
    /// <returns><c>true</c> when the containing namespace or class indicates boundary code; otherwise, <c>false</c>.</returns>
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

    //  Story 1.5: Allow Overridden async void Methods

    /// <summary>
    /// Determines whether the method overrides a base member that dictates the return type.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the method carries the <c>override</c> modifier; otherwise, <c>false</c>.</returns>
    private static bool IsOverriddenMethod(MethodDeclarationSyntax method)
    {
        return method.Modifiers.Any(SyntaxKind.OverrideKeyword);
    }

     // Story 1.5: Allow Overridden async void Methods

    //  Story 1.6: Allow Interface Implementations Requiring void

    /// <summary>
    /// Determines whether the method explicitly implements an interface member whose signature requires <c>void</c>.
    /// </summary>
    /// <param name="method">The method declaration under evaluation.</param>
    /// <param name="semanticModel">The semantic model used to resolve implemented members.</param>
    /// <returns><c>true</c> when an implemented interface member forces a <c>void</c> return type; otherwise, <c>false</c>.</returns>
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

     // Story 1.6: Allow Interface Implementations Requiring void

    //  Story 1.4: Allow ICommand.Execute Methods

    /// <summary>
    /// Determines whether the method implements <c>ICommand.Execute</c>, which must remain <c>void</c>.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <param name="semanticModel">The semantic model used to discover implemented interfaces.</param>
    /// <returns><c>true</c> when the containing type implements <c>ICommand</c>; otherwise, <c>false</c>.</returns>
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

     // Story 1.4: Allow ICommand.Execute Methods

    //  Story 1.8: Allow Partial Methods in Blazor Components

    /// <summary>
    /// Determines whether the method behaves as a Blazor component event handler where <c>async void</c> is acceptable.
    /// </summary>
    /// <param name="method">The method declaration under evaluation.</param>
    /// <param name="semanticModel">The semantic model used to examine the containing component.</param>
    /// <returns><c>true</c> when the method matches Blazor event-handler patterns; otherwise, <c>false</c>.</returns>
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
        // Support both partial methods and regular methods in Blazor components
        var methodName = method.Identifier.Text;
        return (methodName.StartsWith("On") && methodName.EndsWith("Async")) ||
               methodName.StartsWith("On") ||
               methodName.Contains("Click") ||
               methodName.Contains("Submit") ||
               methodName.Contains("Change");
    }

    /// <summary>
    /// Determines whether the supplied class symbol derives from <c>ComponentBase</c>.
    /// </summary>
    /// <param name="classSymbol">The class symbol to inspect.</param>
    /// <returns><c>true</c> when the inheritance chain includes <c>ComponentBase</c>; otherwise, <c>false</c>.</returns>
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

     // Story 1.8: Allow Partial Methods in Blazor Components

    //  Story 1.9: Allow Fire-and-Forget Methods with an Attribute

    /// <summary>
    /// Determines whether the method is decorated with a <c>FireAndForget</c> attribute that explicitly opts into <c>async void</c> behavior.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the attribute is present; otherwise, <c>false</c>.</returns>
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

     // Story 1.9: Allow Fire-and-Forget Methods with an Attribute
}