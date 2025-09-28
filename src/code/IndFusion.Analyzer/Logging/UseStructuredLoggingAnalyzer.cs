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
/// Analyzer that enforces structured logging instead of string concatenation.
/// Supports the "use structured logging" principle for better observability.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseStructuredLoggingAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Use structured logging instead of string concatenation";
    private static readonly LocalizableString MessageFormat = "Use structured logging with named parameters instead of {0}";
    private static readonly LocalizableString Description = "Structured logging improves observability, searchability, and performance. Use named parameters like logger.LogInformation(\"User {UserId} logged in\", userId) instead of string concatenation or interpolation.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseStructuredLogging,
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

		// Check if this is a logging method call on ILogger
		if (!IsILoggerLoggingCall(invocation, context.SemanticModel))
        {
            return;
        }

        // Get the first argument (the message template)
        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count == 0)
        {
            return;
        }

		var messageArgument = arguments[0].Expression;

		// If the target method uses an interpolated string handler overload, allow interpolated strings
		var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
		var allowsInterpolatedHandler = MethodAllowsInterpolatedHandler(methodSymbol);

		// Check if the message uses string concatenation or (when not allowed) interpolation
		if (messageArgument is BinaryExpressionSyntax binaryExpr && IsStringConcatenation(binaryExpr))
        {
            ReportDiagnostic(context, messageArgument, "string concatenation");
        }
		else if (messageArgument is InterpolatedStringExpressionSyntax && !allowsInterpolatedHandler)
        {
            ReportDiagnostic(context, messageArgument, "string interpolation");
        }
		// Only analyze the first argument; do not scan unrelated descendants
    }

	private static bool IsILoggerLoggingCall(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
		// We expect a member access; if not, try to bind the method and check extension receiver
		if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
			var method = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
			return IsILoggerMethod(method);
        }

        var methodName = memberAccess.Name.Identifier.ValueText;

        // Check if it's a logging method
        if (!IsLoggingMethodName(methodName))
        {
            return false;
        }

		// Ensure the receiver type is ILogger or implements ILogger
		var receiverType = semanticModel.GetTypeInfo(memberAccess.Expression).Type;
		if (receiverType == null)
		{
			return false;
		}

		return IsILoggerType(receiverType);
    }

    private static bool IsLoggingMethodName(string methodName)
    {
        // Common logging method names
        var loggingMethods = new[]
        {
            "LogTrace", "LogDebug", "LogInformation", "LogWarning",
            "LogError", "LogCritical", "Log"
        };

        return loggingMethods.Contains(methodName);
    }

	private static bool IsLoggerType(ITypeSymbol typeSymbol)
    {
		// Handles ILogger and ILogger<T>
		if (typeSymbol.Name == "ILogger")
		{
			var containingNamespace = GetFullNamespace(typeSymbol.ContainingNamespace);
			return containingNamespace == "Microsoft.Extensions.Logging";
		}

		// For ILogger<T>, the constructed type has OriginalDefinition named ILogger
		if (typeSymbol is INamedTypeSymbol named && named.OriginalDefinition?.Name == "ILogger")
		{
			var ns = GetFullNamespace(named.OriginalDefinition.ContainingNamespace);
			return ns == "Microsoft.Extensions.Logging";
		}

		return false;
    }

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

    private static bool IsStringConcatenation(BinaryExpressionSyntax binaryExpression)
    {
        // Check if it's a + operator with string operands
        if (!binaryExpression.OperatorToken.IsKind(SyntaxKind.PlusToken))
        {
            return false;
        }

        // Check if either operand is a string literal
        var leftIsString = binaryExpression.Left is LiteralExpressionSyntax leftLiteral &&
                          leftLiteral.Token.IsKind(SyntaxKind.StringLiteralToken);
        var rightIsString = binaryExpression.Right is LiteralExpressionSyntax rightLiteral &&
                           rightLiteral.Token.IsKind(SyntaxKind.StringLiteralToken);

        // If either operand is a string literal, it's string concatenation
        if (leftIsString || rightIsString)
        {
            return true;
        }

        // Recursively check for string concatenation patterns
        return ContainsStringConcatenation(binaryExpression);
    }

    private static bool ContainsStringConcatenation(ExpressionSyntax expression) => expression switch
    {
        BinaryExpressionSyntax binaryExpr when binaryExpr.OperatorToken.IsKind(SyntaxKind.PlusToken) => true,
        LiteralExpressionSyntax literal when literal.Token.IsKind(SyntaxKind.StringLiteralToken) => false,
        _ => false
    };

	private static bool MethodAllowsInterpolatedHandler(IMethodSymbol? methodSymbol)
	{
		if (methodSymbol == null)
		{
			return false;
		}

		// If any parameter type name ends with "InterpolatedStringHandler", allow
		foreach (var p in methodSymbol.Parameters)
		{
			var pType = p.Type;
			if (pType is INamedTypeSymbol pNamed)
			{
				if (pNamed.Name.EndsWith("InterpolatedStringHandler", System.StringComparison.Ordinal))
				{
					return true;
				}
			}
		}

		return false;
	}

	private static bool IsILoggerMethod(IMethodSymbol? method)
	{
		if (method == null)
		{
			return false;
		}

		// Extension methods like LoggerExtensions.LogInformation(ILogger,...)
		if (method.IsExtensionMethod && method.Parameters.Length > 0)
		{
			var first = method.Parameters[0].Type;
			return IsLoggerType(first);
		}

		// Instance methods on ILogger (rare)
		var containing = method.ContainingType;
		return containing != null && IsLoggerType(containing);
	}

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node, string violationType)
    {
        var diagnostic = Diagnostic.Create(
            Rule,
            node.GetLocation(),
            violationType);
        context.ReportDiagnostic(diagnostic);
    }
}
