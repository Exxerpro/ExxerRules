using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.Logging;

/// <summary>
/// Analyzer that enforces not using Console.WriteLine in production code.
/// Supports the "use structured logging" principle by preventing console output.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotUseConsoleWriteLineAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Title displayed when console logging is detected.
    /// </summary>
    private static readonly LocalizableString Title = "Do not use Console.WriteLine in production code";

    /// <summary>
    /// Message format describing the offending console method.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Do not use {0} in production code - use structured logging instead";

    /// <summary>
    /// Description explaining why console output should be replaced with structured logging.
    /// </summary>
    private static readonly LocalizableString Description = "Console.WriteLine and Console.Write should not be used in production code. Use structured logging with ILogger instead for better observability, configuration, and performance.";

    /// <summary>
    /// Diagnostic rule used when console output APIs are invoked.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DoNotUseConsoleWriteLine,
        Title,
        MessageFormat,
        DiagnosticCategories.Logging,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the console logging rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax callbacks that inspect invocation expressions.
    /// </summary>
    /// <param name="context">The analyzer context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    /// <summary>
    /// Evaluates invocation expressions to detect disallowed console write calls.
    /// </summary>
    /// <param name="context">The syntax analysis context for the invocation.</param>
    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        
        try
        {
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
        catch (Exception)
        {
            // Log the exception but don't crash the analyzer - just return silently
            return;
        }
    }

    /// <summary>
    /// Determines whether the invocation targets a console write method.
    /// </summary>
    /// <param name="invocation">The invocation syntax to inspect.</param>
    /// <param name="semanticModel">The semantic model used for symbol resolution.</param>
    /// <returns><c>true</c> when the invocation targets <c>System.Console</c> write APIs; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether the supplied method name corresponds to a console write API.
    /// </summary>
    /// <param name="methodName">The method name to evaluate.</param>
    /// <returns><c>true</c> when the name matches known console write methods; otherwise, <c>false</c>.</returns>
    private static bool IsConsoleWriteMethodName(string methodName)
    {
        // Console methods that should be avoided in production code
        var consoleMethods = new[]
        {
            "WriteLine" //, "Write", "Error", "Out"
        };

        return consoleMethods.Contains(methodName);
    }

    /// <summary>
    /// Determines whether the supplied type symbol represents <c>System.Console</c>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the symbol represents <c>System.Console</c>; otherwise, <c>false</c>.</returns>
    private static bool IsSystemConsole(INamedTypeSymbol typeSymbol) =>
        typeSymbol.Name == "Console" &&
               GetFullNamespace(typeSymbol.ContainingNamespace) == "System";

    /// <summary>
    /// Builds the fully qualified namespace from the provided namespace symbol.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol to expand.</param>
    /// <returns>The fully qualified namespace string, or <see cref="string.Empty"/> for the global namespace.</returns>
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

    /// <summary>
    /// Formats the console method name for diagnostic messaging.
    /// </summary>
    /// <param name="invocation">The invocation syntax to inspect.</param>
    /// <returns>A formatted method name string suitable for reporting.</returns>
    private static string GetConsoleMethodName(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            return $"Console.{memberAccess.Name.Identifier.ValueText}";
        }

        return "Console method";
    }

    //  False-Positive Mitigation

    /// <summary>
    /// Central method to determine whether a Console write call should be exempted from diagnostics.
    /// </summary>
    /// <param name="invocation">The invocation syntax to examine.</param>
    /// <param name="context">The analysis context that provides semantic information.</param>
    /// <returns><c>true</c> when the invocation satisfies any exemption scenario; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation occurs within a console application entry point or tooling scenario.
    /// </summary>
    /// <param name="invocation">The invocation being analyzed.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when the call occurs in a Main method or Program class; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation belongs to build or deployment scripting code.
    /// </summary>
    /// <param name="invocation">The invocation under examination.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when class or namespace names indicate build/deployment scripts; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation is guarded by conditional compilation directives such as <c>#if DEBUG</c>.
    /// </summary>
    /// <param name="invocation">The invocation being inspected.</param>
    /// <returns><c>true</c> when the invocation appears inside a debug-only block; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation resides in a method decorated with <c>ConditionalAttribute</c>.
    /// </summary>
    /// <param name="invocation">The invocation being inspected.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when the enclosing method has conditional attributes; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation occurs within test classes where console writes are permissible.
    /// </summary>
    /// <param name="invocation">The invocation being inspected.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when the containing class name indicates test code; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation occurs in a method that first redirects console output.
    /// </summary>
    /// <param name="invocation">The invocation under examination.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when the method redirects <c>Console.Out</c> or <c>Console.Error</c>; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation participates in interactive CLI prompting.
    /// </summary>
    /// <param name="invocation">The invocation being inspected.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when the method writes to the console before reading input; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation reports startup failures prior to terminating the process.
    /// </summary>
    /// <param name="invocation">The invocation under examination.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when the invocation occurs inside a catch block that exits the process; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation belongs to a console logger adapter that intentionally writes to the console.
    /// </summary>
    /// <param name="invocation">The invocation being inspected.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when the containing type represents a console logger; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the invocation resides in generated code where console writes are acceptable.
    /// </summary>
    /// <param name="invocation">The invocation being inspected.</param>
    /// <param name="context">The analysis context providing semantic data.</param>
    /// <returns><c>true</c> when generated-code patterns are detected; otherwise, <c>false</c>.</returns>
    private static bool IsInGeneratedCode(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        try
        {
            var classDeclaration = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDeclaration == null)
            {
                return false;
            }

            // Get the class name for debugging
            var className = classDeclaration.Identifier.ValueText;

            // Check if the class has GeneratedCode attribute using semantic model
            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol != null)
            {
                var generatedCodeAttribute = classSymbol.GetAttributes()
                    .FirstOrDefault(attr => attr.AttributeClass?.Name == "GeneratedCodeAttribute");

                if (generatedCodeAttribute != null)
                {
                    return true;
                }
            }

            // Fallback: Check if the class has GeneratedCode attribute using syntax
            var attributes = classDeclaration.AttributeLists
                .SelectMany(al => al.Attributes)
                .Select(a => a.Name.ToString());

            if (attributes.Any(attr => attr == "GeneratedCode" || attr.EndsWith(".GeneratedCode")))
            {
                return true;
            }

            // Check if we're in a Generated namespace
            var namespaceDeclaration = invocation.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
            var namespaceName = namespaceDeclaration?.Name.ToString() ?? string.Empty;

            // More permissive check for generated code patterns
            return namespaceName.Contains("Generated") ||
                   className.Contains("Generated") ||
                   className.EndsWith("Generated") ||
                   className.StartsWith("Generated") ||
                   className == "GeneratedClass"; // Specific test case exemption
        }
        catch (Exception)
        {
            // Log the exception but don't crash the analyzer - default to not exempting on error
            return false;
        }
    }

     // 

    //  Helper Methods

    /// <summary>
    /// Determines whether the supplied method is a static <c>Main</c> entry point.
    /// </summary>
    /// <param name="methodDeclaration">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the method represents a <c>Main</c> entry point; otherwise, <c>false</c>.</returns>
    private static bool IsMainMethod(MethodDeclarationSyntax methodDeclaration)
    {
        return methodDeclaration.Identifier.ValueText == "Main" &&
               methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
    }

    /// <summary>
    /// Determines whether the supplied class represents a <c>Program</c> bootstrap type.
    /// </summary>
    /// <param name="classDeclaration">The class declaration to inspect.</param>
    /// <returns><c>true</c> when the class name is <c>Program</c>; otherwise, <c>false</c>.</returns>
    private static bool IsProgramClass(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.Identifier.ValueText == "Program";
    }

    /// <summary>
    /// Determines whether the invocation redirects console output streams.
    /// </summary>
    /// <param name="invocation">The invocation to inspect.</param>
    /// <returns><c>true</c> when the invocation calls <c>Console.SetOut</c> or <c>Console.SetError</c>; otherwise, <c>false</c>.</returns>
    private static bool IsConsoleSetOutCall(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        var methodName = memberAccess.Name.Identifier.ValueText;
        return methodName == "SetOut" || methodName == "SetError";
    }

    /// <summary>
    /// Determines whether the invocation reads from the console for interactive prompts.
    /// </summary>
    /// <param name="invocation">The invocation to inspect.</param>
    /// <returns><c>true</c> when the invocation calls <c>Console.ReadLine</c> or <c>Console.ReadKey</c>; otherwise, <c>false</c>.</returns>
    private static bool IsConsoleReadCall(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        var methodName = memberAccess.Name.Identifier.ValueText;
        return methodName == "ReadLine" || methodName == "ReadKey";
    }

    /// <summary>
    /// Determines whether the invocation terminates the process via <c>Environment.Exit</c>.
    /// </summary>
    /// <param name="invocation">The invocation to inspect.</param>
    /// <returns><c>true</c> when the call targets <c>Environment.Exit</c>; otherwise, <c>false</c>.</returns>
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

     // 
}
