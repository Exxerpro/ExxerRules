using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzer.NullSafety;

/// <summary>
/// Analyzer that enforces null parameter validation at method entry points.
/// Supports the fail-safe defaults and defensive programming principles.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValidateNullParametersAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Title displayed when null parameter validation is missing.
    /// </summary>
    private static readonly LocalizableString Title = "Validate null parameters at method entry";

    /// <summary>
    /// Message format identifying the member and parameters that require validation.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Method '{0}' should validate null parameters at method entry for parameter(s): {1}";

    /// <summary>
    /// Description explaining why null parameter validation is enforced.
    /// </summary>
    private static readonly LocalizableString Description = "Methods should validate reference type parameters for null values at the method entry point, following fail-safe defaults and defensive programming principles.";

    /// <summary>
    /// Diagnostic rule raised when null parameter validation is omitted.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.ValidateNullParameters,
        Title,
        MessageFormat,
        DiagnosticCategories.NullSafety,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the null-parameter validation rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax callbacks that analyze method declarations for null parameter validation.
    /// </summary>
    /// <param name="context">The analysis context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    /// <summary>
    /// Evaluates method declarations for missing null-parameter validation.
    /// </summary>
    /// <param name="context">The syntax analysis context for the method.</param>
    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        
        // Skip if this is a method that should be exempted
        if (IsSkippableMethod(methodDeclaration))
        {
            return;
        }

        // Get reference type parameters that need validation (semantic)
        var referenceParameters = GetReferenceTypeParameters(methodDeclaration, context.SemanticModel);
        
        if (!referenceParameters.Any())
        {
            return;
        }

        // Check if method has null validation for each reference parameter
        var unvalidatedParameters = GetUnvalidatedReferenceParameters(methodDeclaration, referenceParameters);

        // Report one diagnostic per unvalidated parameter
        foreach (var unvalidatedParameter in unvalidatedParameters)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text,
                unvalidatedParameter);
            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Determines whether the supplied method should be excluded from null-parameter validation.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the method does not require null validation; otherwise, <c>false</c>.</returns>
    private static bool IsSkippableMethod(MethodDeclarationSyntax method)
    {
        // Skip constructors, destructors, and event handlers
        if (method.Identifier.Text.StartsWith("On") && method.Modifiers.Any(SyntaxKind.ProtectedKeyword))
        {
            return true;
        }

        // Skip Main method
        if (method.Identifier.Text == "Main")
        {
            return true;
        }

        // Skip interface methods (they don't have bodies)
        if (method.Body == null && method.ExpressionBody == null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Collects the reference-type parameters that require null validation for the specified method.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <param name="semanticModel">The semantic model providing type information.</param>
    /// <returns>A list of parameter names that should be validated for null.</returns>
    private static List<string> GetReferenceTypeParameters(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        var referenceParams = new List<string>();

        if (IsGuardMethod(method))
        {
            return referenceParams;
        }

        foreach (var parameter in method.ParameterList.Parameters)
        {
            if (parameter.Type == null || parameter.Default != null || parameter.Modifiers.Any(SyntaxKind.ParamsKeyword))
            {
                continue;
            }

            var typeName = parameter.Type.ToString();

            // Per Option B, skip nullable value types using a string check.
            if (typeName.EndsWith("?"))
            {
                continue;
            }

            // Skip other known value types like CancellationToken by name.
            if (IsValueTypeByName(typeName))
            {
                continue;
            }
            
            var typeSymbol = semanticModel.GetTypeInfo(parameter.Type).Type;

            // If it's not a value type by name, and it's a reference type, it needs validation.
            // We also check for infrastructure types to exclude them.
            if (typeSymbol != null && typeSymbol.IsReferenceType)
            {
                 if (!IsInfrastructureType(typeSymbol))
                 {
                    referenceParams.Add(parameter.Identifier.ValueText);
                 }
            }
            // Fallback for when semantic model fails: if we haven't skipped it by name, assume it's a reference type.
            else if (typeSymbol == null)
            {
                referenceParams.Add(parameter.Identifier.ValueText);
            }
        }

        return referenceParams;
    }

    /// <summary>
    /// Checks if the method is a guard method that should be exempt from null validation.
    /// </summary>
    /// <summary>
    /// Determines whether the method acts as a guard helper and should bypass validation checks.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns><c>true</c> when the method behaves like a guard helper; otherwise, <c>false</c>.</returns>
    private static bool IsGuardMethod(MethodDeclarationSyntax method)
    {
        var methodName = method.Identifier.ValueText;
        
        // Only exempt specific guard method names that are clearly guard methods
        if (methodName == "ThrowIfNull" || methodName == "Guard" || methodName == "Validate" || 
            methodName == "Require" || methodName == "Ensure" || methodName == "Check")
        {
            return true;
        }

        // Check if method is an extension method with guard-like behavior
        if (method.Modifiers.Any(SyntaxKind.StaticKeyword) && 
            method.ParameterList.Parameters.Count > 0 &&
            method.ParameterList.Parameters[0].Modifiers.Any(SyntaxKind.ThisKeyword))
        {
            // Extension methods that throw exceptions for null validation
            if (methodName.Contains("Throw") || methodName.Contains("Guard") || 
                methodName.Contains("Validate") || methodName.Contains("Require"))
            {
                return true;
            }
        }

        // Check if method is a static factory method that creates failure results
        // Only exempt if it's clearly a Result type factory method
        if (method.Modifiers.Any(SyntaxKind.StaticKeyword) && 
            (methodName == "Failure" || methodName == "WithFailure") && 
            method.ReturnType.ToString().Contains("Result"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if the type is an infrastructure type that should be exempt from null validation.
    /// </summary>
    /// <summary>
    /// Determines whether the specified type represents an infrastructure or framework type that is exempt from validation.
    /// </summary>
    /// <param name="type">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the type is considered infrastructure; otherwise, <c>false</c>.</returns>
    private static bool IsInfrastructureType(ITypeSymbol type)
    {
        if (type == null)
            return false;

        var typeName = type.Name;
        var namespaceName = type.ContainingNamespace?.ToDisplayString();

        // CancellationToken is a value type but check for completeness
        if (typeName == "CancellationToken" && (namespaceName == "System.Threading" || namespaceName == null))
            return true;

        // Service provider and logging infrastructure
        if (typeName == "IServiceProvider" || 
            (typeName == "ILogger" && namespaceName?.Contains("Microsoft.Extensions.Logging") == true))
            return true;

        // Common DI interfaces
        if (namespaceName?.Contains("Microsoft.Extensions") == true && 
            (typeName.Contains("Service") || typeName.Contains("Provider")))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if a type name represents a value type (fallback for when semantic model is unavailable).
    /// </summary>
    /// <summary>
    /// Determines whether the provided type name represents a value type that does not require null validation.
    /// </summary>
    /// <param name="typeName">The type name to inspect.</param>
    /// <returns><c>true</c> when the name references a known value type; otherwise, <c>false</c>.</returns>
    private static bool IsValueTypeByName(string typeName)
    {
        // Remove nullable indicator for checking
        var cleanTypeName = typeName.TrimEnd('?');
        
        // Common value types
        return cleanTypeName is "int" or "long" or "short" or "byte" or "uint" or "ulong" or
                     "ushort" or "sbyte" or "float" or "double" or "decimal" or "bool" or
                     "char" or "DateTime" or "Guid" or "TimeSpan" or "DateTimeOffset" or
                     "CancellationToken" or "IntPtr" or "UIntPtr";
    }

    /// <summary>
    /// Determines which reference-type parameters in the method lack null validation.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <param name="referenceParameters">The list of reference-type parameter names requiring validation.</param>
    /// <returns>A list containing the names of parameters missing null validation.</returns>
    private static List<string> GetUnvalidatedReferenceParameters(MethodDeclarationSyntax method, List<string> referenceParameters)
    {
        var unvalidated = new List<string>(referenceParameters);

        // Check expression-bodied members for guard patterns
        if (method.ExpressionBody != null)
        {
            var validatedInExpression = FindValidatedParametersInExpression(method.ExpressionBody.Expression, referenceParameters);
            foreach (var validated in validatedInExpression)
            {
                unvalidated.Remove(validated);
            }
        }

        // Get method body statements
        var statements = GetMethodStatements(method);
        if (!statements.Any())
        {
            return unvalidated;
        }

        // Look for null validation patterns in all statements
        foreach (var statement in statements)
        {
            var validatedParameter = FindValidatedParameter(statement, referenceParameters);
            if (!string.IsNullOrEmpty(validatedParameter))
            {
                unvalidated.Remove(validatedParameter!);
            }
        }

        // Check for local functions with guards
        var localFunctions = method.DescendantNodes().OfType<LocalFunctionStatementSyntax>();
        foreach (var localFunction in localFunctions)
        {
            var validatedInLocalFunction = FindValidatedParametersInLocalFunction(localFunction, referenceParameters);
            foreach (var validated in validatedInLocalFunction)
            {
                unvalidated.Remove(validated);
            }
        }

        // Check for local function calls that might perform validation
        var localFunctionCalls = method.DescendantNodes().OfType<InvocationExpressionSyntax>()
            .Where(inv => inv.Expression is IdentifierNameSyntax identifier && 
                         localFunctions.Any(lf => lf.Identifier.ValueText == identifier.Identifier.ValueText));
        
        foreach (var call in localFunctionCalls)
        {
            // If a local function is called and it has guard logic, consider parameters validated
            var functionName = ((IdentifierNameSyntax)call.Expression).Identifier.ValueText;
            var correspondingFunction = localFunctions.FirstOrDefault(lf => lf.Identifier.ValueText == functionName);
            if (correspondingFunction != null)
            {
                var validatedInFunction = FindValidatedParametersInLocalFunction(correspondingFunction, referenceParameters);
                
                // Map local function parameters to main method parameters
                if (call.ArgumentList.Arguments.Count > 0 && correspondingFunction.ParameterList.Parameters.Count > 0)
                {
                    for (int i = 0; i < Math.Min(call.ArgumentList.Arguments.Count, correspondingFunction.ParameterList.Parameters.Count); i++)
                    {
                        var argument = call.ArgumentList.Arguments[i].Expression;
                        var localParam = correspondingFunction.ParameterList.Parameters[i];
                        var localParamName = localParam.Identifier.ValueText;
                        
                        // If the local function validates this parameter, mark the corresponding argument as validated
                        if (validatedInFunction.Contains(localParamName))
                        {
                            var argumentIdentifier = GetIdentifierFromExpression(argument);
                            if (!string.IsNullOrEmpty(argumentIdentifier) && referenceParameters.Contains(argumentIdentifier))
                            {
                                unvalidated.Remove(argumentIdentifier);
                            }
                        }
                    }
                }
            }
        }

        return unvalidated;
    }

    /// <summary>
    /// Retrieves the statements contained within the method for validation analysis.
    /// </summary>
    /// <param name="method">The method declaration to inspect.</param>
    /// <returns>An enumerable collection of statements within the method body.</returns>
    private static IEnumerable<StatementSyntax> GetMethodStatements(MethodDeclarationSyntax method)
    {
        if (method.Body != null)
        {
            return method.Body.Statements;
        }
        // For expression-bodied methods, synthesize a single statement representing the expression
        if (method.ExpressionBody != null)
        {
            return new[] { SyntaxFactory.ExpressionStatement(method.ExpressionBody.Expression) };
        }
        return [];
    }

    /// <summary>
    /// Searches the condition expression for null checks against known reference parameters.
    /// </summary>
    /// <param name="condition">The conditional expression to analyze.</param>
    /// <param name="referenceParameters">The set of reference parameters requiring validation.</param>
    /// <returns>The name of the parameter validated in the condition, or <c>null</c> when none is found.</returns>
    private static string? FindNullCheckInCondition(ExpressionSyntax condition, List<string> referenceParameters)
    {
        if (condition is BinaryExpressionSyntax binaryExpr)
        {
            // Handle 'param == null'
            if (binaryExpr.IsKind(SyntaxKind.EqualsExpression))
            {
                var leftIdentifier = GetIdentifierFromExpression(binaryExpr.Left);
                var rightIdentifier = GetIdentifierFromExpression(binaryExpr.Right);

                if (IsNullLiteral(binaryExpr.Right) && referenceParameters.Contains(leftIdentifier)) return leftIdentifier;
                if (IsNullLiteral(binaryExpr.Left) && referenceParameters.Contains(rightIdentifier)) return rightIdentifier;
            }
            // Recurse for '||'
            else if (binaryExpr.IsKind(SyntaxKind.LogicalOrExpression))
            {
                return FindNullCheckInCondition(binaryExpr.Left, referenceParameters) ??
                       FindNullCheckInCondition(binaryExpr.Right, referenceParameters);
            }
        }
        // Handle 'param is null'
        else if (condition is IsPatternExpressionSyntax isPattern)
        {
            var identifier = GetIdentifierFromExpression(isPattern.Expression);
            if (referenceParameters.Contains(identifier) && IsNullPattern(isPattern.Pattern))
            {
                return identifier;
            }
        }
        return null;
    }

    /// <summary>
    /// Attempts to detect null-validation logic within the supplied statement.
    /// </summary>
    /// <param name="statement">The statement to inspect.</param>
    /// <param name="referenceParameters">The reference parameters that require validation.</param>
    /// <returns>The name of the parameter validated within the statement, or <c>null</c> when no validation is found.</returns>
    private static string? FindValidatedParameter(StatementSyntax statement, List<string> referenceParameters)
    {
        // Check for if statements with null checks
        if (statement is IfStatementSyntax ifStatement)
        {
            var validatedParam = FindNullCheckInCondition(ifStatement.Condition, referenceParameters);
            if (validatedParam != null && HasAppropriateValidation(ifStatement))
            {
                return validatedParam;
            }
        }

        // Look for expression statements like ArgumentNullException.ThrowIfNull(parameter)
        if (statement is ExpressionStatementSyntax exprStatement &&
            exprStatement.Expression is InvocationExpressionSyntax invocation)
        {
            var validatedInInvocation = FindValidatedParametersInInvocation(invocation, referenceParameters);
            if (validatedInInvocation.Any())
            {
                return validatedInInvocation.First();
            }
        }
        
        // Check for return statements with null-conditional operators
        if (statement is ReturnStatementSyntax returnStmt && returnStmt.Expression != null)
        {
            var validatedInReturn = FindValidatedParametersInExpression(returnStmt.Expression, referenceParameters);
            if (validatedInReturn.Any())
            {
                return validatedInReturn.First();
            }
        }

        return null;
    }

    /// <summary>
    /// Determines whether the if-statement performs an appropriate null validation check.
    /// </summary>
    /// <param name="ifStatement">The if-statement to inspect.</param>
    /// <returns><c>true</c> when the statement validates null parameters; otherwise, <c>false</c>.</returns>
    private static bool HasAppropriateValidation(IfStatementSyntax ifStatement)
    {
        // Check if the if statement body contains appropriate validation patterns
        // Look for: throw new ArgumentNullException(nameof(parameter))
        // or: return Result.WithFailure(...)
        // or: ArgumentNullException.ThrowIfNull(parameter)

        if (ifStatement.Statement is BlockSyntax block)
        {
            foreach (var statement in block.Statements)
            {
                if (IsValidValidationStatement(statement))
                {
                    return true;
                }
            }
        }
        else if (IsValidValidationStatement(ifStatement.Statement))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the statement represents an acceptable null-validation pattern.
    /// </summary>
    /// <param name="statement">The statement to inspect.</param>
    /// <returns><c>true</c> when the statement guards against null values; otherwise, <c>false</c>.</returns>
    private static bool IsValidValidationStatement(StatementSyntax statement)
    {
        // Check for throw new ArgumentNullException(nameof(parameter))
        if (statement is ThrowStatementSyntax throwStatement &&
            throwStatement.Expression is ObjectCreationExpressionSyntax objectCreation &&
            objectCreation.Type.ToString().Contains("ArgumentNullException"))
        {
            return true;
        }

        // Check for return Result.WithFailure(...) or Result.Failure(...)
        if (statement is ReturnStatementSyntax returnStatement &&
            returnStatement.Expression is InvocationExpressionSyntax invocation &&
            (invocation.Expression.ToString().Contains("Result.WithFailure") ||
             invocation.Expression.ToString().Contains("Result.Failure")))
        {
            return true;
        }

        // Check for ArgumentNullException.ThrowIfNull(parameter)
        if (statement is ExpressionStatementSyntax exprStatement &&
            exprStatement.Expression is InvocationExpressionSyntax invocationExpr &&
            invocationExpr.Expression.ToString().Contains("ArgumentNullException.ThrowIfNull"))
        {
            return true;
        }

        // Check for any throw statement (more permissive)
        if (statement is ThrowStatementSyntax)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Extracts the identifier text from the supplied expression when available.
    /// </summary>
    /// <param name="expression">The expression to analyze.</param>
    /// <returns>The identifier name, or <c>string.Empty</c> when none is found.</returns>
    private static string GetIdentifierFromExpression(ExpressionSyntax expression) => expression switch
    {
        IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
        MemberAccessExpressionSyntax memberAccess => GetIdentifierFromExpression(memberAccess.Expression),
        _ => string.Empty
    };

    /// <summary>
    /// Determines whether the expression represents a null literal.
    /// </summary>
    /// <param name="expression">The expression to inspect.</param>
    /// <returns><c>true</c> when the expression is the null literal; otherwise, <c>false</c>.</returns>
    private static bool IsNullLiteral(ExpressionSyntax expression) => expression is LiteralExpressionSyntax literal &&
               literal.Token.IsKind(SyntaxKind.NullKeyword);

    /// <summary>
    /// Determines whether the pattern corresponds to a null constant.
    /// </summary>
    /// <param name="pattern">The pattern to inspect.</param>
    /// <returns><c>true</c> when the pattern represents null; otherwise, <c>false</c>.</returns>
    private static bool IsNullPattern(PatternSyntax pattern) => pattern is ConstantPatternSyntax constantPattern &&
               constantPattern.Expression is LiteralExpressionSyntax literal &&
               literal.Token.IsKind(SyntaxKind.NullKeyword);

    /// <summary>
    /// Finds validated parameters in expression-bodied members.
    /// </summary>
    /// <summary>
    /// Inspects an expression for nested guard calls validating reference parameters.
    /// </summary>
    /// <param name="expression">The expression to analyze.</param>
    /// <param name="referenceParameters">The reference parameters that require validation.</param>
    /// <returns>A list of parameter names validated within the expression.</returns>
    private static List<string> FindValidatedParametersInExpression(ExpressionSyntax expression, List<string> referenceParameters)
    {
        var validated = new List<string>();

        // Check for null-coalescing operators (handles nulls)
        if (expression is BinaryExpressionSyntax binaryExpr && binaryExpr.OperatorToken.IsKind(SyntaxKind.QuestionQuestionToken))
        {
            var leftIdentifier = GetIdentifierFromExpression(binaryExpr.Left);
            if (referenceParameters.Contains(leftIdentifier))
            {
                validated.Add(leftIdentifier);
            }
        }

        // Check for null-conditional operators (handles nulls)
        if (expression is ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var identifier = GetIdentifierFromExpression(conditionalAccess.Expression);
            if (referenceParameters.Contains(identifier))
            {
                validated.Add(identifier);
            }
        }

        // Check for null-conditional operators in binary expressions (e.g., text?.Length ?? 0)
        if (expression is BinaryExpressionSyntax binaryExpr2 && binaryExpr2.OperatorToken.IsKind(SyntaxKind.QuestionQuestionToken))
        {
            // Check if the left side contains a null-conditional operator
            if (binaryExpr2.Left is ConditionalAccessExpressionSyntax leftConditional)
            {
                var identifier = GetIdentifierFromExpression(leftConditional.Expression);
                if (referenceParameters.Contains(identifier))
                {
                    validated.Add(identifier);
                }
            }
        }

        // Check for guard helper invocations in expressions
        if (expression is InvocationExpressionSyntax invocation)
        {
            var validatedInInvocation = FindValidatedParametersInInvocation(invocation, referenceParameters);
            validated.AddRange(validatedInInvocation);
        }

        // Recursively check nested expressions for guard patterns
        foreach (var childExpression in expression.DescendantNodes().OfType<ExpressionSyntax>())
        {
            if (childExpression != expression) // Avoid infinite recursion
            {
                var validatedInChild = FindValidatedParametersInExpression(childExpression, referenceParameters);
                validated.AddRange(validatedInChild);
            }
        }

        return validated;
    }

    /// <summary>
    /// Finds validated parameters in local functions.
    /// </summary>
    /// <summary>
    /// Examines a local function for null-validation logic covering specified reference parameters.
    /// </summary>
    /// <param name="localFunction">The local function statement to inspect.</param>
    /// <param name="referenceParameters">The parameters that require validation.</param>
    /// <returns>A list of parameter names validated within the local function.</returns>
    private static List<string> FindValidatedParametersInLocalFunction(LocalFunctionStatementSyntax localFunction, List<string> referenceParameters)
    {
        var validated = new List<string>();

        // Get local function parameters
        var localFunctionParameters = localFunction.ParameterList.Parameters
            .Select(p => p.Identifier.ValueText)
            .ToList();

        if (localFunction.Body != null)
        {
            foreach (var statement in localFunction.Body.Statements)
            {
                var validatedParameter = FindValidatedParameter(statement, localFunctionParameters);
                if (!string.IsNullOrEmpty(validatedParameter))
                {
                    validated.Add(validatedParameter!);
                }
            }
        }

        return validated;
    }

    /// <summary>
    /// Finds validated parameters in invocation expressions (guard helpers, etc.).
    /// </summary>
    /// <summary>
    /// Analyzes a guard invocation to determine which reference parameters are validated.
    /// </summary>
    /// <param name="invocation">The invocation expression to inspect.</param>
    /// <param name="referenceParameters">The parameters requiring validation.</param>
    /// <returns>A list of parameter names validated by the invocation.</returns>
    private static List<string> FindValidatedParametersInInvocation(InvocationExpressionSyntax invocation, List<string> referenceParameters)
    {
        var validated = new List<string>();

        // Check for Guard.Against.Null patterns
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Expression is MemberAccessExpressionSyntax guardAccess &&
            guardAccess.Expression.ToString() == "Guard" &&
            guardAccess.Name.Identifier.ValueText == "Against" &&
            memberAccess.Name.Identifier.ValueText == "Null")
        {
            if (invocation.ArgumentList.Arguments.Count > 0)
            {
                var argument = invocation.ArgumentList.Arguments[0].Expression;
                var identifier = GetIdentifierFromExpression(argument);
                if (!string.IsNullOrEmpty(identifier) && referenceParameters.Contains(identifier))
                {
                    validated.Add(identifier);
                }
            }
        }

        // Check for extension method guard patterns (parameter.ThrowIfNull())
        if (invocation.Expression is MemberAccessExpressionSyntax extensionMemberAccess &&
            extensionMemberAccess.Name.Identifier.ValueText == "ThrowIfNull")
        {
            var identifier = GetIdentifierFromExpression(extensionMemberAccess.Expression);
            if (!string.IsNullOrEmpty(identifier) && referenceParameters.Contains(identifier))
            {
                validated.Add(identifier);
            }
        }

        // Check for other common guard method patterns
        if (invocation.Expression is MemberAccessExpressionSyntax guardMemberAccess)
        {
            var methodName = guardMemberAccess.Name.Identifier.ValueText;
            // Common guard method names
            if (methodName == "ThrowIfNull" || methodName == "Guard" || methodName == "Validate" || 
                methodName == "Require" || methodName == "Ensure")
            {
                var identifier = GetIdentifierFromExpression(guardMemberAccess.Expression);
                if (!string.IsNullOrEmpty(identifier) && referenceParameters.Contains(identifier))
                {
                    validated.Add(identifier);
                }
            }
        }

        // Check for ArgumentNullException.ThrowIfNull patterns
        if (invocation.Expression is MemberAccessExpressionSyntax throwIfNullAccess &&
            throwIfNullAccess.Name.Identifier.ValueText == "ThrowIfNull" &&
            throwIfNullAccess.Expression.ToString().Contains("ArgumentNullException"))
        {
            if (invocation.ArgumentList.Arguments.Count > 0)
            {
                var argument = invocation.ArgumentList.Arguments[0].Expression;
                var identifier = GetIdentifierFromExpression(argument);
                if (!string.IsNullOrEmpty(identifier) && referenceParameters.Contains(identifier))
                {
                    validated.Add(identifier);
                }
            }
        }

        // Check for Result.Failure and Result.WithFailure patterns
        if (invocation.Expression.ToString().Contains("Result.Failure") ||
            invocation.Expression.ToString().Contains("Result.WithFailure"))
        {
            // For Result.Failure patterns, we need to check if this is within a null check context
            // For now, we'll be conservative and not mark this as validation
            // unless it's clearly within a null check
        }

        return validated;
    }
}
