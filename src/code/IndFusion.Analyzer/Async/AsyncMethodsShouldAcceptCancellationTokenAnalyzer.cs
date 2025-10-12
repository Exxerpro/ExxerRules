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
    private static readonly LocalizableString Title = "Async methods should accept CancellationToken";
    private static readonly LocalizableString MessageFormat = "Async method '{0}' should accept a CancellationToken parameter to support graceful cancellation";
    private static readonly LocalizableString Description = "Async methods should accept a CancellationToken parameter to enable graceful cancellation and prevent unresponsive applications, following fail-safe defaults principles.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken,
        Title,
        MessageFormat,
        DiagnosticCategories.Async,
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

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

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

    private static bool IsAsyncMethod(MethodDeclarationSyntax method) =>
        // Check if method has async modifier
        method.Modifiers.Any(SyntaxKind.AsyncKeyword);

    private static bool IsAsyncVoidMethod(MethodDeclarationSyntax method) =>
        // Check if return type is void (async void methods are typically event handlers)
        method.ReturnType is PredefinedTypeSyntax predefined &&
               predefined.Keyword.IsKind(SyntaxKind.VoidKeyword);

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

    #region Story 1.1: Exempt Overridden and Explicitly Implemented Methods

    /// <summary>
    /// Determines whether an override can be skipped because the base signature already provides a cancellation token.
    /// </summary>
    private static bool ShouldSkipOverride(IMethodSymbol methodSymbol) => methodSymbol.IsOverride;

    /// <summary>
    /// Determines whether an explicit interface implementation can be skipped because the interface already mandates a cancellation token.
    /// </summary>
    private static bool ShouldSkipExplicitInterfaceImplementation(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.ExplicitInterfaceImplementations.Length == 0)
        {
            return false;
        }

        return methodSymbol.ExplicitInterfaceImplementations.All(MethodHasCancellationToken);
    }

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

    #endregion

    #region Story 1.2: Exempt Blazor Lifecycle Methods

    /// <summary>
    /// Checks if the method is a Blazor lifecycle method.
    /// </summary>
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

    #region Story 1.3: Exempt SignalR Hub Lifecycle Methods

    /// <summary>
    /// Checks if the method is a SignalR hub lifecycle method.
    /// </summary>
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
    /// Checks if the class inherits from Hub.
    /// </summary>
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

    #endregion

    #region Story 1.4: Exempt Test Methods

    /// <summary>
    /// Checks if the method is a test method.
    /// </summary>
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

    #endregion

    #region Story 1.5: Exempt Test Class Helper Methods

    /// <summary>
    /// Checks if the method is a helper method in a test class.
    /// </summary>
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

    #endregion

    #region Story 1.6: Exempt IAsyncLifetime Contract Methods

    /// <summary>
    /// Checks if the method is an IAsyncLifetime method.
    /// </summary>
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
    /// Checks if the class implements IAsyncLifetime.
    /// </summary>
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

    #endregion

    #region Story 1.7: Exempt Test Fixture Methods

    /// <summary>
    /// Checks if the method is in a test fixture class.
    /// </summary>
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

    #endregion

    #region Story 1.8: Exempt Blazor EventCallback Handlers

    /// <summary>
    /// Checks if the method is a Blazor event handler.
    /// </summary>
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

    #endregion

    #region Story 1.9: Analyze Cancellation Availability

    /// <summary>
    /// Checks if cancellation is available for awaited calls in the method.
    /// </summary>
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
    /// Gets the method name from an invocation expression.
    /// </summary>
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
    /// Checks if a method doesn't support cancellation.
    /// </summary>
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

    #endregion

    #region Story 1.10: Be Aware of Captured Tokens

    /// <summary>
    /// Checks if the method has access to a captured CancellationToken.
    /// </summary>
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

    #endregion
}
