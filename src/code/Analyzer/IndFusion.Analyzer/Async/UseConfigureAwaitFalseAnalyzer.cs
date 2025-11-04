using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.Async;

/// <summary>
/// Ensures <c>await</c> expressions in library code opt into <c>ConfigureAwait(false)</c> to avoid deadlocks and improve responsiveness.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseConfigureAwaitFalseAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the localized title describing the ConfigureAwait guideline.
    /// </summary>
    private static readonly LocalizableString Title = "Use ConfigureAwait(false) in library code";

    /// <summary>
    /// Gets the localized message format reported when <c>ConfigureAwait(false)</c> is missing.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Await expression should use ConfigureAwait(false) to avoid deadlocks in library code";

    /// <summary>
    /// Gets the diagnostic description that explains the rationale behind the rule.
    /// </summary>
    private static readonly LocalizableString Description = "In library code, await expressions should use ConfigureAwait(false) to prevent potential deadlocks when called from synchronous contexts. This improves performance and prevents threading issues.";

    /// <summary>
    /// The diagnostic descriptor emitted when an await expression omits <c>ConfigureAwait(false)</c>.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseConfigureAwaitFalse,
        Title,
        MessageFormat,
        DiagnosticCategories.Async,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the ConfigureAwait enforcement rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers the syntax callbacks that evaluate await expressions for ConfigureAwait usage.
    /// </summary>
    /// <param name="context">The Roslyn analysis context used for action registration.</param>
    /// <remarks>
    /// Generated code is excluded and concurrent execution is enabled before scanning <see cref="AwaitExpressionSyntax"/> nodes.
    /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TDD Green phase: Focus on await expressions
        context.RegisterSyntaxNodeAction(AnalyzeAwaitExpression, SyntaxKind.AwaitExpression);
    }

    /// <summary>
    /// Analyzes await expressions and reports diagnostics when <c>ConfigureAwait(false)</c> is missing outside approved exception scenarios.
    /// </summary>
    /// <param name="context">The syntax analysis context supplying the await expression.</param>
    private static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
    {
        var awaitExpression = (AwaitExpressionSyntax)context.Node;
        var expression = awaitExpression.Expression;

		// Skip if this is application code (not library code) or boundary layers (controllers/web)
		if (IsApplicationCode(awaitExpression) || IsInBoundaryLayer(awaitExpression))
        {
            return;
        }

        // Skip test methods (Story 1.1)
        if (IsInTestMethod(awaitExpression))
        {
            return;
        }

        // Skip test helper methods (Story 1.2)
        if (IsInTestHelperMethod(awaitExpression))
        {
            return;
        }

        // Skip test-related namespaces (Story 1.3)
        if (IsInTestRelatedNamespace(awaitExpression))
        {
            return;
        }

        // Skip IAsyncLifetime methods (Story 1.4)
        if (IsInIAsyncLifetimeMethod(awaitExpression, context.SemanticModel))
        {
            return;
        }

        // Skip collection and assembly fixtures (Story 1.5)
        if (IsInCollectionOrAssemblyFixture(awaitExpression))
        {
            return;
        }

        // Skip Blazor component lifecycle methods (Story 1.6)
        if (IsInBlazorComponentLifecycleMethod(awaitExpression, context.SemanticModel))
        {
            return;
        }

        // Skip Blazor event handlers (Story 1.7)
        if (IsInBlazorEventHandler(awaitExpression))
        {
            return;
        }

        // Skip expressions without ConfigureAwait overloads (Story 1.8)
        if (IsExpressionWithoutConfigureAwaitOverload(expression))
        {
            return;
        }

        // Check if the await expression already has ConfigureAwait
        if (HasConfigureAwait(expression))
        {
            return;
        }

        // Report diagnostic for missing ConfigureAwait(false)
        var diagnostic = Diagnostic.Create(
            Rule,
            awaitExpression.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Determines whether the await expression belongs to application-level code where ConfigureAwait enforcement is relaxed.
    /// </summary>
    /// <param name="awaitExpression">The await expression being analyzed.</param>
    /// <returns><c>true</c> when class or namespace patterns indicate application code; otherwise, <c>false</c>.</returns>
    private static bool IsApplicationCode(AwaitExpressionSyntax awaitExpression)
    {
        // Check if we're in a class that looks like application code
        var classDeclaration = awaitExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDeclaration != null)
        {
            var className = classDeclaration.Identifier.Text;

            // Common application class names that typically don't need ConfigureAwait
            var applicationClassPatterns = new[] { "Program", "Startup", "Main" };
            if (applicationClassPatterns.Any(pattern => className.Contains(pattern)))
            {
                return true;
            }
        }

        // Check if we're in a namespace that looks like application code
        var namespaceDeclaration = awaitExpression.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
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
    /// Determines whether the awaited expression already invokes <c>ConfigureAwait</c>.
    /// </summary>
    /// <param name="expression">The expression being awaited.</param>
    /// <returns><c>true</c> when <c>ConfigureAwait</c> is present; otherwise, <c>false</c>.</returns>
    private static bool HasConfigureAwait(ExpressionSyntax expression)
    {
        // Check for ConfigureAwait method call
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.ValueText == "ConfigureAwait")
        {
            return true;
        }

        // Check for chained method calls ending with ConfigureAwait
        var current = expression;
        while (current is InvocationExpressionSyntax currentInvocation &&
               currentInvocation.Expression is MemberAccessExpressionSyntax currentMember)
        {
            if (currentMember.Name.Identifier.ValueText == "ConfigureAwait")
            {
                return true;
            }

            current = currentMember.Expression;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the containing type or namespace forms part of a boundary layer (e.g., controllers or API endpoints).
    /// </summary>
    /// <param name="node">The node whose ancestors are inspected.</param>
    /// <returns><c>true</c> when the await resides in a boundary layer; otherwise, <c>false</c>.</returns>
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

    //  Story 1.1: Exempt Test Methods

    /// <summary>
    /// Checks if the await expression is inside a test method.
    /// </summary>
    /// <summary>
    /// Determines whether the await expression executes inside a test method decorated with common unit-testing attributes.
    /// </summary>
    /// <param name="awaitExpression">The await expression under inspection.</param>
    /// <returns><c>true</c> when a test attribute is present; otherwise, <c>false</c>.</returns>
    private static bool IsInTestMethod(AwaitExpressionSyntax awaitExpression)
    {
        var methodDeclaration = awaitExpression.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (methodDeclaration == null)
        {
            return false;
        }

        // Check for test attributes
        var attributes = methodDeclaration.AttributeLists.SelectMany(al => al.Attributes);
        foreach (var attribute in attributes)
        {
            var attributeName = attribute.Name.ToString();
            // Check for exact matches and partial matches
            if (attributeName == "Fact" ||
                attributeName == "Theory" ||
                attributeName == "Test" ||
                attributeName == "TestMethod" ||
                attributeName.EndsWith(".Fact") ||
                attributeName.EndsWith(".Theory") ||
                attributeName.EndsWith(".Test") ||
                attributeName.EndsWith(".TestMethod") ||
                attributeName.Contains("Fact") ||
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

    //  Story 1.2: Exempt Test Helper Methods

    /// <summary>
    /// Determines whether the await expression resides inside helper methods declared on test classes.
    /// </summary>
    /// <param name="awaitExpression">The await expression being analyzed.</param>
    /// <returns><c>true</c> when the containing class name indicates test helpers; otherwise, <c>false</c>.</returns>
    private static bool IsInTestHelperMethod(AwaitExpressionSyntax awaitExpression)
    {
        var containingClass = awaitExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClass == null)
        {
            return false;
        }

        var className = containingClass.Identifier.Text;
        
        // Check if class name indicates it's a test class
        return className.EndsWith("Tests") ||
               className.EndsWith("Test") ||
               className.EndsWith("Specs") ||
               className.EndsWith("Spec");
    }

     // 

    //  Story 1.3: Exempt Test-Related Namespaces

    /// <summary>
    /// Determines whether the await expression is declared in a namespace dedicated to testing infrastructure.
    /// </summary>
    /// <param name="awaitExpression">The await expression to inspect.</param>
    /// <returns><c>true</c> when the namespace path includes test-related segments; otherwise, <c>false</c>.</returns>
    private static bool IsInTestRelatedNamespace(AwaitExpressionSyntax awaitExpression)
    {
        var namespaceDeclaration = awaitExpression.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
        if (namespaceDeclaration == null)
        {
            return false;
        }

        var namespaceName = namespaceDeclaration.Name.ToString();
        return namespaceName.Contains(".Tests") || namespaceName.Contains(".TestUtilities");
    }

     // 

    //  Story 1.4: Exempt IAsyncLifetime Implementations

    /// <summary>
    /// Determines whether the await expression appears within an <c>IAsyncLifetime</c> lifecycle method.
    /// </summary>
    /// <param name="awaitExpression">The await expression under inspection.</param>
    /// <param name="semanticModel">The semantic model used to inspect the containing class.</param>
    /// <returns><c>true</c> when the method is part of <c>IAsyncLifetime</c>; otherwise, <c>false</c>.</returns>
    private static bool IsInIAsyncLifetimeMethod(AwaitExpressionSyntax awaitExpression, SemanticModel semanticModel)
    {
        var methodDeclaration = awaitExpression.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (methodDeclaration == null)
        {
            return false;
        }

        var methodName = methodDeclaration.Identifier.Text;
        
        // Check if method is InitializeAsync or DisposeAsync
        if (methodName != "InitializeAsync" && methodName != "DisposeAsync")
        {
            return false;
        }

        // Check if the containing class implements IAsyncLifetime
        var containingClass = awaitExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
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
    /// Determines whether the class symbol implements the xUnit <c>IAsyncLifetime</c> interface.
    /// </summary>
    /// <param name="classSymbol">The class symbol to inspect.</param>
    /// <returns><c>true</c> when the interface is implemented from the xUnit namespace; otherwise, <c>false</c>.</returns>
    private static bool ImplementsIAsyncLifetime(INamedTypeSymbol classSymbol)
    {
        foreach (var interfaceSymbol in classSymbol.AllInterfaces)
        {
            if (interfaceSymbol.Name == "IAsyncLifetime")
            {
                // Check if it's from Xunit namespace (handles both explicit and global usings)
                var containingNamespace = interfaceSymbol.ContainingNamespace;
                var namespaceDisplayString = containingNamespace?.ToDisplayString();
                
                if (containingNamespace?.Name == "Xunit" || 
                    namespaceDisplayString == "Xunit" ||
                    namespaceDisplayString == "global::Xunit")
                {
                    return true;
                }
            }
        }
        return false;
    }

     // 

    //  Story 1.5: Exempt Collection and Assembly Fixtures

    /// <summary>
    /// Determines whether the await expression resides inside collection or assembly fixture classes.
    /// </summary>
    /// <param name="awaitExpression">The await expression under inspection.</param>
    /// <returns><c>true</c> when the containing class name and attributes indicate fixture usage; otherwise, <c>false</c>.</returns>
    private static bool IsInCollectionOrAssemblyFixture(AwaitExpressionSyntax awaitExpression)
    {
        var containingClass = awaitExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
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

        // Check for CollectionDefinition or Collection attribute
        var attributes = containingClass.AttributeLists.SelectMany(al => al.Attributes);
        foreach (var attribute in attributes)
        {
            var attributeName = attribute.Name.ToString();
            if (attributeName.Contains("CollectionDefinition") || attributeName.Contains("Collection"))
            {
                return true;
            }
        }

        return false;
    }

     // 

    //  Story 1.6: Exempt Blazor Component Lifecycle Methods

    /// <summary>
    /// Determines whether the await expression resides within a Blazor component lifecycle method.
    /// </summary>
    /// <param name="awaitExpression">The await expression to inspect.</param>
    /// <param name="semanticModel">The semantic model used to resolve the component type.</param>
    /// <returns><c>true</c> when the containing method matches a known lifecycle callback; otherwise, <c>false</c>.</returns>
    private static bool IsInBlazorComponentLifecycleMethod(AwaitExpressionSyntax awaitExpression, SemanticModel semanticModel)
    {
        var methodDeclaration = awaitExpression.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (methodDeclaration == null)
        {
            return false;
        }

        // Check if the containing class inherits from ComponentBase
        var containingClass = awaitExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
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
        var methodName = methodDeclaration.Identifier.Text;
        return methodName == "OnInitializedAsync" ||
               methodName == "OnParametersSetAsync" ||
               methodName == "OnAfterRenderAsync";
    }

    /// <summary>
    /// Determines whether the supplied class symbol derives from <c>ComponentBase</c>, indicating a Blazor component.
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

    //  Story 1.7: Exempt Blazor EventCallback Handlers

    /// <summary>
    /// Determines whether the await expression belongs to a Blazor event handler method that may omit ConfigureAwait.
    /// </summary>
    /// <param name="awaitExpression">The await expression under inspection.</param>
    /// <returns><c>true</c> when the containing method matches common event-handler naming patterns; otherwise, <c>false</c>.</returns>
    private static bool IsInBlazorEventHandler(AwaitExpressionSyntax awaitExpression)
    {
        var methodDeclaration = awaitExpression.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (methodDeclaration == null)
        {
            return false;
        }

        // Check if method is private (typical for event handlers)
        if (!methodDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
        {
            return false;
        }

        var methodName = methodDeclaration.Identifier.Text;
        
        // Check for common Blazor event handler patterns
        return methodName.StartsWith("On") && methodName.EndsWith("Async") ||
               methodName.Contains("Click") ||
               methodName.Contains("Submit") ||
               methodName.Contains("Change");
    }

     // 

    //  Story 1.8: Exempt Awaits on Expressions Without ConfigureAwait Overloads

    /// <summary>
    /// Determines whether the awaited expression targets APIs without <c>ConfigureAwait</c> overloads.
    /// </summary>
    /// <param name="expression">The awaited expression.</param>
    /// <returns><c>true</c> when the call lacks <c>ConfigureAwait</c> support; otherwise, <c>false</c>.</returns>
    private static bool IsExpressionWithoutConfigureAwaitOverload(ExpressionSyntax expression)
    {
        // Get the method name from the expression
        var methodName = GetMethodName(expression);
        
        // Check for methods that don't have ConfigureAwait overloads
        var methodsWithoutConfigureAwait = new[]
        {
            "WhenAll",
            "WhenAny",
            "FromResult",
            "FromException",
            "FromCanceled",
            "Yield",
            "Run",
            "RunSynchronously"
        };

        return methodsWithoutConfigureAwait.Contains(methodName);
    }

    /// <summary>
    /// Extracts the method name from the awaited expression, handling both member-access and identifier invocations.
    /// </summary>
    /// <param name="expression">The expression representing the awaited call.</param>
    /// <returns>The invoked method name, or an empty string when indeterminable.</returns>
    private static string GetMethodName(ExpressionSyntax expression)
    {
        if (expression is InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                return memberAccess.Name.Identifier.Text;
            }
            
            if (invocation.Expression is IdentifierNameSyntax identifier)
            {
                return identifier.Identifier.Text;
            }
        }

        return string.Empty;
    }

     // 
}
