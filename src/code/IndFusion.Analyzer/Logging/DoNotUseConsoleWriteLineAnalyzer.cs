using System.Collections.Immutable;
using System.Linq;
using IndFusion.Analyzers.Common;
using IndFusion.Analyzers.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Logging;

/// <summary>
/// Analyzer that enforces not using Console.WriteLine in production code.
/// Supports the "use structured logging" principle by preventing console output.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseConsoleWriteLineAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Do not use Console.WriteLine in production code";
    private static readonly LocalizableString MessageFormat = "Do not use {0} in production code - use structured logging instead";
    private static readonly LocalizableString Description = "Console.WriteLine and Console.Write should not be used in production code. Use structured logging with ILogger instead for better observability, configuration, and performance.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DoNotUseConsoleWriteLine,
        Title,
        MessageFormat,
        DiagnosticCategories.Logging,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // Check if this is a Console.WriteLine or Console.Write call
        if (!IsConsoleWriteCall(invocation, context.SemanticModel))
        {
            return;
        }

        // Check if this should be exempted from reporting
        if (IsExemptFromConsoleWriteCheck(invocation, context))
        {
            return;
        }

        // Get the method name for reporting
        var methodName = GetConsoleMethodName(invocation);

        // Report diagnostic for Console write usage
        var diagnostic = Diagnostic.Create(
            Rule,
            invocation.GetLocation(),
            methodName);
        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsConsoleWriteCall(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        // Check if it's a member access on Console
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        // Check if the method is WriteLine, Write, or similar
        var methodName = memberAccess.Name.Identifier.ValueText;
        if (!IsConsoleWriteMethodName(methodName))
        {
            return false;
        }

        // Check if the receiver is Console (syntactic check first)
        if (memberAccess.Expression is not IdentifierNameSyntax identifierName)
        {
            return false;
        }

        if (identifierName.Identifier.ValueText != "Console")
        {
            return false;
        }

        // Try to verify it's the System.Console type through semantic model
        // If semantic model fails, we'll rely on the syntactic check
        try
        {
            var symbolInfo = semanticModel.GetSymbolInfo(memberAccess.Expression);
            if (symbolInfo.Symbol is INamedTypeSymbol namedTypeSymbol)
            {
                return IsSystemConsole(namedTypeSymbol);
            }
        }
        catch
        {
            // Semantic model failed, fall back to syntactic check
        }

        // If semantic analysis fails, assume it's System.Console based on name
        // This is less precise but works when semantic model is incomplete
        return true;
    }

    private static bool IsConsoleWriteMethodName(string methodName)
    {
        // Console methods that should be avoided in production code
        var consoleMethods = new[]
        {
            "WriteLine", "Write", "Error", "Out"
        };

        return consoleMethods.Contains(methodName);
    }

    private static bool IsSystemConsole(INamedTypeSymbol typeSymbol) =>
        // Check if it's System.Console
        typeSymbol.Name == "Console" &&
               GetFullNamespace(typeSymbol.ContainingNamespace) == "System";

    private static string GetFullNamespace(INamespaceSymbol? namespaceSymbol)
    {
        if (namespaceSymbol == null || namespaceSymbol.IsGlobalNamespace)
        {
            return string.Empty;
        }

        var parts = new List<string>();
        var current = namespaceSymbol;

        while (current != null && !current.IsGlobalNamespace)
        {
            parts.Insert(0, current.Name);
            current = current.ContainingNamespace;
        }

        return string.Join(".", parts);
    }

    private static string GetConsoleMethodName(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            return $"Console.{memberAccess.Name.Identifier.ValueText}";
        }

        return "Console method";
    }

    #region False-Positive Mitigation

    /// <summary>
    /// Central method to check if a Console write call should be exempted from reporting.
    /// </summary>
    private static bool IsExemptFromConsoleWriteCheck(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        return IsInConsoleApplicationOrTooling(invocation, context) ||
               IsInBuildOrDeploymentScript(invocation, context) ||
               IsInConditionalCompilationBlock(invocation) ||
               IsInConditionalAttributeMethod(invocation, context) ||
               IsInTestClass(invocation, context) ||
               IsInRedirectedConsoleOutput(invocation, context) ||
               IsInCliPrompting(invocation, context) ||
               IsInExceptionReportingDuringStartup(invocation, context) ||
               IsInConsoleLoggerAdapter(invocation, context) ||
               IsInGeneratedCode(invocation, context);
    }

    /// <summary>
    /// Story 1.1: Exempt Console Applications and Tooling
    /// </summary>
    private static bool IsInConsoleApplicationOrTooling(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        // Check if we're in a Main method
        var methodDeclaration = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration != null && IsMainMethod(methodDeclaration))
        {
            return true;
        }

        // Check if we're in a class named Program
        var classDeclaration = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration != null && IsProgramClass(classDeclaration))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.2: Exempt Build and Deployment Scripts
    /// </summary>
    private static bool IsInBuildOrDeploymentScript(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration == null)
        {
            return false;
        }

        var className = classDeclaration.Identifier.ValueText;
        var namespaceDeclaration = invocation.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
        var namespaceName = namespaceDeclaration?.Name.ToString() ?? "";

        // Check for build/deployment/script related names
        return className.Contains("Build") || className.Contains("Deployment") || className.Contains("Script") ||
               namespaceName.Contains("Build") || namespaceName.Contains("Deployment") || namespaceName.Contains("Scripts");
    }

    /// <summary>
    /// Story 1.3: Exempt Conditional Compilation Blocks
    /// </summary>
    private static bool IsInConditionalCompilationBlock(InvocationExpressionSyntax invocation)
    {
        // Check if the invocation is within a conditional compilation directive
        var trivia = invocation.GetLeadingTrivia();
        foreach (var trivium in trivia)
        {
            if (trivium.IsKind(SyntaxKind.IfDirectiveTrivia))
            {
                var directive = trivium.ToString();
                if (directive.Contains("#if DEBUG") || directive.Contains("#if TRACE"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.4: Exempt ConditionalAttribute Methods
    /// </summary>
    private static bool IsInConditionalAttributeMethod(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration == null)
        {
            return false;
        }

        // Check if the method has Conditional attributes
        var attributes = methodDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Select(a => a.Name.ToString());

        return attributes.Any(attr => attr == "Conditional" || 
                                     attr.EndsWith(".Conditional") ||
                                     attr.Contains("Conditional"));
    }

    /// <summary>
    /// Story 1.5: Exempt Unit and Integration Tests
    /// </summary>
    private static bool IsInTestClass(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration == null)
        {
            return false;
        }

        var className = classDeclaration.Identifier.ValueText;
        return className.EndsWith("Tests") || className.EndsWith("Test");
    }

    /// <summary>
    /// Story 1.6: Exempt Redirected Console Output
    /// </summary>
    private static bool IsInRedirectedConsoleOutput(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration == null)
        {
            return false;
        }

        // Check if the method contains Console.SetOut or Console.SetError calls
        var methodBody = methodDeclaration.Body;
        if (methodBody == null)
        {
            return false;
        }

        var setOutCalls = methodBody.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(inv => IsConsoleSetOutCall(inv));

        return setOutCalls.Any();
    }

    /// <summary>
    /// Story 1.7: Exempt CLI Prompting
    /// </summary>
    private static bool IsInCliPrompting(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration == null)
        {
            return false;
        }

        var methodBody = methodDeclaration.Body;
        if (methodBody == null)
        {
            return false;
        }

        // Check if this is a Console.Write call followed by Console.ReadLine or Console.ReadKey
        var methodName = GetConsoleMethodName(invocation);
        if (!methodName.Contains("Console.Write"))
        {
            return false;
        }

        // Look for Console.ReadLine or Console.ReadKey calls in the same method
        var readCalls = methodBody.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(inv => IsConsoleReadCall(inv));

        return readCalls.Any();
    }

    /// <summary>
    /// Story 1.8: Exempt Exception Reporting During Startup
    /// </summary>
    private static bool IsInExceptionReportingDuringStartup(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration == null)
        {
            return false;
        }

        var methodBody = methodDeclaration.Body;
        if (methodBody == null)
        {
            return false;
        }

        // Check if we're in a catch block and there's an Environment.Exit call
        var catchClause = invocation.FirstAncestorOrSelf<CatchClauseSyntax>();
        if (catchClause == null)
        {
            return false;
        }

        // Check if the method contains Environment.Exit calls
        var exitCalls = methodBody.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(inv => IsEnvironmentExitCall(inv));

        return exitCalls.Any();
    }

    /// <summary>
    /// Story 1.9: Exempt Console Logger Adapters
    /// </summary>
    private static bool IsInConsoleLoggerAdapter(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration == null)
        {
            return false;
        }

        var className = classDeclaration.Identifier.ValueText;
        
        // Check if it's a ConsoleLogger class
        if (className == "ConsoleLogger")
        {
            return true;
        }

        // Check if the class has AllowConsoleLogging attribute
        var attributes = classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Select(a => a.Name.ToString());

        return attributes.Any(attr => attr == "AllowConsoleLogging" || 
                                     attr.EndsWith(".AllowConsoleLogging"));
    }

    /// <summary>
    /// Story 1.10: Exempt Generated Code
    /// </summary>
    private static bool IsInGeneratedCode(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration == null)
        {
            return false;
        }

        // Check if the class has GeneratedCode attribute
        var attributes = classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Select(a => a.Name.ToString());

        if (attributes.Any(attr => attr == "GeneratedCode" || attr.EndsWith(".GeneratedCode")))
        {
            return true;
        }

        // Check if we're in a Generated namespace
        var namespaceDeclaration = invocation.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
        var namespaceName = namespaceDeclaration?.Name.ToString() ?? "";
        
        return namespaceName.Contains("Generated");
    }

    #endregion

    #region Helper Methods

    private static bool IsMainMethod(MethodDeclarationSyntax methodDeclaration)
    {
        return methodDeclaration.Identifier.ValueText == "Main" &&
               methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
    }

    private static bool IsProgramClass(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.Identifier.ValueText == "Program";
    }

    private static bool IsConsoleSetOutCall(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        var methodName = memberAccess.Name.Identifier.ValueText;
        return methodName == "SetOut" || methodName == "SetError";
    }

    private static bool IsConsoleReadCall(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        var methodName = memberAccess.Name.Identifier.ValueText;
        return methodName == "ReadLine" || methodName == "ReadKey";
    }

    private static bool IsEnvironmentExitCall(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        var methodName = memberAccess.Name.Identifier.ValueText;
        var receiver = memberAccess.Expression.ToString();
        
        return methodName == "Exit" && receiver == "Environment";
    }

    #endregion
}
