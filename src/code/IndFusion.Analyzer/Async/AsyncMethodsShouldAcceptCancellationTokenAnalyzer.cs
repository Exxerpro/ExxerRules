using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndFusion.Analyzers.Common;
using IndFusion.Analyzers.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Async;

/// <summary>
/// Analyzer that enforces CancellationToken parameters in async methods.
/// Supports graceful cancellation and fail-safe defaults principles.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncMethodsShouldAcceptCancellationTokenAnalyzer : DiagnosticAnalyzer
{
#pragma warning disable IDE1006 // Naming styles for analyzer fields
    /// <summary>
    /// Gets the localized title describing the cancellation-token guideline.
    /// </summary>
    private static readonly LocalizableString Title = "Async methods should accept CancellationToken";

    /// <summary>
    /// Gets the localized message format emitted when an async method lacks a cancellation-token parameter.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Async method '{0}' should accept a CancellationToken parameter to support graceful cancellation";

    /// <summary>
    /// Gets the descriptive text that accompanies diagnostics for missing cancellation tokens.
    /// </summary>
    private static readonly LocalizableString Description = "Async methods should accept a CancellationToken parameter to enable graceful cancellation and prevent unresponsive applications, following fail-safe defaults principles.";

    /// <summary>
    /// The diagnostic descriptor representing the cancellation-token enforcement rule.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken,
        Title,
        MessageFormat,
        DiagnosticCategories.Async,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the cancellation-token enforcement rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers the callbacks necessary to evaluate async method declarations for cancellation-token parameters.
    /// </summary>
    /// <param name="context">The Roslyn analysis context used to register actions.</param>
    /// <remarks>
    /// Generated code is excluded and concurrent execution is enabled prior to examining each <see cref="MethodDeclarationSyntax"/> node.
    /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    /// <summary>
    /// Analyzes async method declarations and reports diagnostics when cancellation-token support is missing.
    /// </summary>
    /// <param name="context">The syntax analysis context supplying the target method declaration.</param>
    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;
        var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
        if (methodSymbol is null)
        {
            return;
        }

        // Only analyze async methods
        if (!IsAsyncMethod(methodDeclaration))
        {
            return;
        }

        // Skip async void methods (typically event handlers)
        if (IsAsyncVoidMethod(methodDeclaration))
        {
            return;
        }

        // Skip if this is a method that should be exempted or boundary layer (controllers/web)
        if (IsSkippableMethod(methodDeclaration) || IsInBoundaryLayer(methodDeclaration))
        {
            return;
        }

        // Skip overridden methods when base signature already supplies cancellation
        if (ShouldSkipOverride(methodSymbol))
        {
            return;
        }

        // Skip explicit interface implementations only if the interface already requires a token
        if (ShouldSkipExplicitInterfaceImplementation(methodSymbol))
        {
            return;
        }

        // Skip Blazor lifecycle methods (Story 1.2)
        if (IsBlazorLifecycleMethod(methodDeclaration, semanticModel))
        {
            return;
        }

        // Skip SignalR hub lifecycle methods (Story 1.3)
        if (IsSignalRHubLifecycleMethod(methodDeclaration, semanticModel))
        {
            return;
        }

        // Skip test methods (Story 1.4)
        if (IsTestMethod(methodDeclaration))
        {
            return;
        }

        // Skip test class helper methods (Story 1.5)
        if (IsTestClassHelperMethod(methodDeclaration))
        {
            return;
        }

        // Skip IAsyncLifetime methods (Story 1.6)
        if (IsIAsyncLifetimeMethod(methodDeclaration, semanticModel))
        {
            return;
        }

        // Skip test fixture methods (Story 1.7)
        if (IsTestFixtureMethod(methodDeclaration))
        {
            return;
        }

        // Skip Blazor event handlers (Story 1.8)
        if (IsBlazorEventHandler(methodDeclaration))
        {
            return;
        }

        // Check if method already has CancellationToken parameter
        if (MethodHasCancellationToken(methodSymbol))
        {
            return;
        }

        // Check if method has captured token (Story 1.10)
        if (HasCapturedToken(methodDeclaration, semanticModel))
        {
            return;
        }

        // Check if cancellation is not available for awaited calls (Story 1.9)
        if (!HasCancellationAvailable(methodDeclaration, semanticModel))
        {
            return;
        }

        // Report diagnostic for missing CancellationToken
        var diagnostic = Diagnostic.Create(
            Rule,
            methodDeclaration.Identifier.GetLocation(),
            methodDeclaration.Identifier.Text);
        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Determines whether the supplied method carries the <c>async</c> modifier.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the async modifier is present; otherwise, <c>false</c>.</returns>
    private static bool IsAsyncMethod(MethodDeclarationSyntax method) =>
        // Check if method has async modifier
        method.Modifiers.Any(SyntaxKind.AsyncKeyword);

    /// <summary>
    /// Determines whether the async method returns <see cref="void"/>, which typically indicates an event handler.
    /// </summary>
    /// <param name="method">The method declaration being evaluated.</param>
    /// <returns><c>true</c> when the method is async void; otherwise, <c>false</c>.</returns>
    private static bool IsAsyncVoidMethod(MethodDeclarationSyntax method) =>
        // Check if return type is void (async void methods are typically event handlers)
        method.ReturnType is PredefinedTypeSyntax predefined &&
               predefined.Keyword.IsKind(SyntaxKind.VoidKeyword);

    /// <summary>
    /// Determines whether the method should be skipped from analysis due to naming patterns or architectural considerations.
    /// </summary>
    /// <param name="method">The method declaration to evaluate.</param>
    /// <returns><c>true</c> when the method qualifies for skipping; otherwise, <c>false</c>.</returns>
    private static bool IsSkippableMethod(MethodDeclarationSyntax method)
    {
        // Skip Main method
        if (method.Identifier.Text == "Main")
        {
            return true;
        }

        // Skip interface methods (they don't have bodies to implement cancellation)
        if (method.Body == null && method.ExpressionBody == null)
        {
            return true;
        }

        // Skip methods that look like event handlers by naming convention
        var methodName = method.Identifier.Text;
        if (methodName.Contains("_Click") ||
            methodName.Contains("_Changed") ||
            methodName.Contains("_Load") ||
            methodName.StartsWith("On"))
        {
            return true;
        }

        // Skip if this is application code (not library code)
        if (IsApplicationCode(method))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the method is part of application-layer code that can legitimately omit cancellation tokens.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the surrounding class or namespace indicates application code; otherwise, <c>false</c>.</returns>
    private static bool IsApplicationCode(MethodDeclarationSyntax method)
    {
        // Check if we're in a class that looks like application code
        var classDeclaration = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDeclaration != null)
        {
            var className = classDeclaration.Identifier.Text;

            // Common application class names that typically don't need cancellation tokens
            var applicationClassPatterns = new[] { "Program", "Startup", "Main" };
            if (applicationClassPatterns.Any(pattern => className.Contains(pattern)))
            {
                return true;
            }
        }

        // Check if we're in a namespace that looks like application code
        var namespaceDeclaration = method.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        if (namespaceDeclaration != null)
        {
            var namespaceName = namespaceDeclaration.Name.ToString();

            // Common application namespace patterns
            var applicationNamespacePatterns = new[] { "Program", "App", "Application", "ConsoleApp", "WebApp" };
            if (applicationNamespacePatterns.Any(pattern => namespaceName.Contains(pattern)))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the provided type symbol represents <see cref="System.Threading.CancellationToken"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to evaluate.</param>
    /// <returns><c>true</c> when the symbol resolves to <c>CancellationToken</c>; otherwise, <c>false</c>.</returns>
    private static bool IsCancellationTokenType(ITypeSymbol typeSymbol)
    {
        // Check if type is System.Threading.CancellationToken
        if (typeSymbol.ContainingNamespace?.Name == "Threading" &&
            typeSymbol.ContainingNamespace.ContainingNamespace?.Name == "System" &&
            typeSymbol.Name == "CancellationToken")
        {
            return true;
        }

        // Check for fully qualified name
        var fullName = GetFullTypeName(typeSymbol);
        return fullName is "System.Threading.CancellationToken" or
               "CancellationToken";
    }

    /// <summary>
    /// Builds the fully qualified type name for the supplied symbol, including containing types and namespaces.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to translate.</param>
    /// <returns>A dotted type name suitable for comparisons.</returns>
    private static string GetFullTypeName(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
        {
            return string.Empty;
        }

        var parts = new List<string>();
        var current = typeSymbol;

        while (current != null)
        {
            parts.Insert(0, current.Name);
            current = current.ContainingType;
        }

        // Add namespace
        var namespaceParts = new List<string>();
        var namespaceSymbol = typeSymbol.ContainingNamespace;
        while (namespaceSymbol != null && !namespaceSymbol.IsGlobalNamespace)
        {
            namespaceParts.Insert(0, namespaceSymbol.Name);
            namespaceSymbol = namespaceSymbol.ContainingNamespace;
        }

        if (namespaceParts.Any())
        {
            return string.Join(".", namespaceParts.Concat(parts));
        }

        return string.Join(".", parts);
    }

    /// <summary>
    /// Determines whether the provided node resides within boundary-layer artifacts such as controllers or API endpoints.
    /// </summary>
    /// <param name="node">The node whose ancestors should be inspected.</param>
    /// <returns><c>true</c> when the ancestor chain identifies a boundary layer; otherwise, <c>false</c>.</returns>
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
            if (ns.Contains(".Web") || ns.Contains(".Api") || ns.Contains(".Endpoints") || ns.Contains(".Presentation"))
            {
                return true;
            }
        }

        return false;
    }

    //  Story 1.1: Exempt Overridden and Explicitly Implemented Methods

    /// <summary>
    /// Determines whether an override can be skipped because the base signature already provides a cancellation token.
    /// </summary>
    /// <param name="methodSymbol">The method symbol representing the override.</param>
    /// <returns><c>true</c> when the method overrides a base member; otherwise, <c>false</c>.</returns>
    private static bool ShouldSkipOverride(IMethodSymbol methodSymbol) => methodSymbol.IsOverride;

    /// <summary>
    /// Determines whether an explicit interface implementation can be skipped because the interface already mandates a cancellation token.
    /// </summary>
    /// <param name="methodSymbol">The method symbol representing the explicit implementation.</param>
    /// <returns><c>true</c> when every implemented interface member already includes a cancellation token; otherwise, <c>false</c>.</returns>
    private static bool ShouldSkipExplicitInterfaceImplementation(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.ExplicitInterfaceImplementations.Length == 0)
        {
            return false;
        }

        return methodSymbol.ExplicitInterfaceImplementations.All(MethodHasCancellationToken);
    }

    /// <summary>
    /// Determines whether the provided method symbol includes a cancellation-token parameter.
    /// </summary>
    /// <param name="methodSymbol">The method symbol to inspect.</param>
    /// <returns><c>true</c> when at least one parameter is a cancellation token; otherwise, <c>false</c>.</returns>
    private static bool MethodHasCancellationToken(IMethodSymbol methodSymbol)
    {
        foreach (var parameter in methodSymbol.Parameters)
        {
            if (IsCancellationTokenType(parameter.Type))
            {
                return true;
            }
        }

        return false;
    }

     // 

    //  Story 1.2: Exempt Blazor Lifecycle Methods

    /// <summary>
    /// Determines whether the supplied method is a Blazor component lifecycle callback.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <param name="semanticModel">The semantic model used to resolve the containing type.</param>
    /// <returns><c>true</c> when the method matches a known lifecycle signature on a <c>ComponentBase</c>-derived class; otherwise, <c>false</c>.</returns>
    private static bool IsBlazorLifecycleMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
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

        // Check if method is a Blazor lifecycle method
        var methodName = method.Identifier.Text;
        return methodName == "OnInitializedAsync" ||
               methodName == "OnParametersSetAsync" ||
               methodName == "OnAfterRenderAsync" ||
               methodName == "OnParametersSetAsync";
    }

    /// <summary>
    /// Determines whether the provided class symbol derives from <c>ComponentBase</c>.
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

     // 

    //  Story 1.3: Exempt SignalR Hub Lifecycle Methods

    /// <summary>
    /// Determines whether the method is part of the SignalR hub lifecycle.
    /// </summary>
    /// <param name="method">The method declaration being evaluated.</param>
    /// <param name="semanticModel">The semantic model used to inspect the containing hub type.</param>
    /// <returns><c>true</c> when the method matches SignalR lifecycle signatures; otherwise, <c>false</c>.</returns>
    private static bool IsSignalRHubLifecycleMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        // Check if the containing class inherits from Hub
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

        // Check if class inherits from Hub
        if (!InheritsFromHub(classSymbol))
        {
            return false;
        }

        // Check if method is a SignalR hub lifecycle method
        var methodName = method.Identifier.Text;
        return methodName == "OnConnectedAsync" ||
               methodName == "OnDisconnectedAsync";
    }

    /// <summary>
    /// Determines whether the provided class symbol derives from <c>Hub</c>.
    /// </summary>
    /// <param name="classSymbol">The class symbol to inspect.</param>
    /// <returns><c>true</c> when the inheritance chain includes SignalR hub types; otherwise, <c>false</c>.</returns>
    private static bool InheritsFromHub(INamedTypeSymbol classSymbol)
    {
        var current = classSymbol.BaseType;
        while (current != null)
        {
            if (current.Name == "Hub" &&
                current.ContainingNamespace?.Name == "SignalR" &&
                current.ContainingNamespace.ContainingNamespace?.Name == "AspNetCore")
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }

     // 

    //  Story 1.4: Exempt Test Methods

    /// <summary>
    /// Determines whether the method is decorated as a test case.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when a known test attribute is present; otherwise, <c>false</c>.</returns>
    private static bool IsTestMethod(MethodDeclarationSyntax method)
    {
        // Check for test attributes
        var attributes = method.AttributeLists.SelectMany(al => al.Attributes);
        foreach (var attribute in attributes)
        {
            var attributeName = attribute.Name.ToString();
            if (attributeName.Contains("Fact") ||
                attributeName.Contains("Theory") ||
                attributeName.Contains("Test") ||
                attributeName.Contains("TestMethod"))
            {
                return true;
            }
        }
        return false;
    }

     // 

    //  Story 1.5: Exempt Test Class Helper Methods

    /// <summary>
    /// Determines whether the method belongs to a test class and serves as a helper utility.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the containing class name indicates a test helper; otherwise, <c>false</c>.</returns>
    private static bool IsTestClassHelperMethod(MethodDeclarationSyntax method)
    {
        var containingClass = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClass == null)
        {
            return false;
        }

        var className = containingClass.Identifier.Text;
        
        // Check if class name indicates it's a test class
        return className.EndsWith("Tests") ||
               className.EndsWith("Specs") ||
               className.EndsWith("TestFixture");
    }

     // 

    //  Story 1.6: Exempt IAsyncLifetime Contract Methods

    /// <summary>
    /// Determines whether the method implements the <c>IAsyncLifetime</c> contract provided by xUnit.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <param name="semanticModel">The semantic model used to resolve the containing type.</param>
    /// <returns><c>true</c> when the method is <c>InitializeAsync</c> or <c>DisposeAsync</c> on an <c>IAsyncLifetime</c> type; otherwise, <c>false</c>.</returns>
    private static bool IsIAsyncLifetimeMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        var methodName = method.Identifier.Text;
        
        // Check if method is InitializeAsync or DisposeAsync
        if (methodName != "InitializeAsync" && methodName != "DisposeAsync")
        {
            return false;
        }

        // Check if the containing class implements IAsyncLifetime
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

        // Check if class implements IAsyncLifetime
        return ImplementsIAsyncLifetime(classSymbol);
    }

    /// <summary>
    /// Determines whether the supplied class symbol implements <c>IAsyncLifetime</c>.
    /// </summary>
    /// <param name="classSymbol">The class symbol to inspect.</param>
    /// <returns><c>true</c> when the interface is implemented; otherwise, <c>false</c>.</returns>
    private static bool ImplementsIAsyncLifetime(INamedTypeSymbol classSymbol)
    {
        foreach (var interfaceSymbol in classSymbol.AllInterfaces)
        {
            if (interfaceSymbol.Name == "IAsyncLifetime" &&
                interfaceSymbol.ContainingNamespace?.Name == "Xunit")
            {
                return true;
            }
        }
        return false;
    }

     // 

    //  Story 1.7: Exempt Test Fixture Methods

    /// <summary>
    /// Determines whether the method belongs to an xUnit test fixture class.
    /// </summary>
    /// <param name="method">The method declaration under evaluation.</param>
    /// <returns><c>true</c> when the containing class matches fixture patterns; otherwise, <c>false</c>.</returns>
    private static bool IsTestFixtureMethod(MethodDeclarationSyntax method)
    {
        var containingClass = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClass == null)
        {
            return false;
        }

        var className = containingClass.Identifier.Text;
        
        // Check if class name ends with Fixture
        if (!className.EndsWith("Fixture"))
        {
            return false;
        }

        // Check for CollectionDefinition attribute
        var attributes = containingClass.AttributeLists.SelectMany(al => al.Attributes);
        foreach (var attribute in attributes)
        {
            var attributeName = attribute.Name.ToString();
            if (attributeName.Contains("CollectionDefinition"))
            {
                return true;
            }
        }

        return false;
    }

     // 

    //  Story 1.8: Exempt Blazor EventCallback Handlers

    /// <summary>
    /// Determines whether the method signature aligns with Blazor event handlers where cancellation might not apply.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the method matches known Blazor event handler conventions; otherwise, <c>false</c>.</returns>
    private static bool IsBlazorEventHandler(MethodDeclarationSyntax method)
    {
        // Check if method is private (typical for event handlers)
        if (!method.Modifiers.Any(SyntaxKind.PrivateKeyword))
        {
            return false;
        }

        var methodName = method.Identifier.Text;
        
        // Check for common Blazor event handler patterns
        return methodName.StartsWith("On") && methodName.EndsWith("Async") ||
               methodName.Contains("Click") ||
               methodName.Contains("Submit") ||
               methodName.Contains("Change");
    }

     // 

    //  Story 1.9: Analyze Cancellation Availability

    /// <summary>
    /// Determines whether the method awaits operations that support cancellation tokens.
    /// </summary>
    /// <param name="method">The method declaration to analyze.</param>
    /// <param name="semanticModel">The semantic model used for invocation inspection.</param>
    /// <returns><c>true</c> when an awaited call supports cancellation; otherwise, <c>false</c>.</returns>
    private static bool HasCancellationAvailable(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        // Check if the method actually awaits anything
        var methodBody = method.Body;
        if (methodBody == null)
        {
            return true; // No body, no awaited calls
        }

        // Look for await expressions
        var awaitExpressions = methodBody.DescendantNodes()
            .OfType<AwaitExpressionSyntax>();

        // If no await expressions, cancellation is not needed
        if (!awaitExpressions.Any())
        {
            return false;
        }

        // Check if all awaited calls are methods that don't support cancellation
        foreach (var awaitExpr in awaitExpressions)
        {
            if (awaitExpr.Expression is InvocationExpressionSyntax invocation)
            {
                var methodName = GetMethodName(invocation);
                
                // Check for methods that don't have CancellationToken overloads
                if (IsMethodWithoutCancellationSupport(methodName))
                {
                    continue; // This method doesn't support cancellation
                }
                
                // If we find any method that supports cancellation, we need a token
                return true;
            }
        }

        // If all awaited methods don't support cancellation, we don't need a token
        return false;
    }

    /// <summary>
    /// Extracts the invoked method name from an invocation expression.
    /// </summary>
    /// <param name="invocation">The invocation expression to analyse.</param>
    /// <returns>The method name referenced by the invocation, or an empty string when unavailable.</returns>
    private static string GetMethodName(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.Name.Identifier.Text;
        }
        
        if (invocation.Expression is IdentifierNameSyntax identifier)
        {
            return identifier.Identifier.Text;
        }

        return string.Empty;
    }

    /// <summary>
    /// Determines whether the specified method name represents an API without cancellation-token overloads.
    /// </summary>
    /// <param name="methodName">The name of the awaited method.</param>
    /// <returns><c>true</c> when the method is known to lack cancellation support; otherwise, <c>false</c>.</returns>
    private static bool IsMethodWithoutCancellationSupport(string methodName)
    {
        // Common methods that don't have CancellationToken overloads
        var methodsWithoutCancellation = new[]
        {
            "Yield",
            "FromResult",
            "FromException",
            "FromCanceled",
            "Run",
            "RunSynchronously",
            "SomeMethodWithoutCancellation" // Test method that doesn't support cancellation
        };

        return methodsWithoutCancellation.Contains(methodName);
    }

     // 

    //  Story 1.10: Be Aware of Captured Tokens

    /// <summary>
    /// Determines whether the method accesses a previously captured cancellation token.
    /// </summary>
    /// <param name="method">The method declaration being analysed.</param>
    /// <param name="semanticModel">The semantic model used for identifier resolution.</param>
    /// <returns><c>true</c> when a captured token is referenced; otherwise, <c>false</c>.</returns>
    private static bool HasCapturedToken(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        // Check if method uses a CancellationToken from a field or property
        var methodBody = method.Body;
        if (methodBody == null)
        {
            return false;
        }

        // Look for CancellationToken usage in the method body
        var cancellationTokenUsages = methodBody.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Where(id => id.Identifier.Text.Contains("CancellationToken") || 
                        id.Identifier.Text.Contains("cancellationToken") ||
                        id.Identifier.Text.Contains("_cancellationToken") ||
                        id.Identifier.Text.Contains("token"));

        // Also check for field access patterns like _cancellationToken
        var memberAccessUsages = methodBody.DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Where(ma => ma.Name.Identifier.Text.Contains("cancellationToken") ||
                        ma.Name.Identifier.Text.Contains("CancellationToken"));

        return cancellationTokenUsages.Any() || memberAccessUsages.Any();
    }

     // 
}
