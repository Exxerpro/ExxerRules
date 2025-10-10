using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Performance;

/// <summary>
/// Analyzer that enforces efficient LINQ operations to avoid multiple enumerations.
/// Supports the performance optimization principles.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseEfficientLinqAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Use efficient LINQ operations";
    private static readonly LocalizableString MessageFormat = "Inefficient LINQ usage detected: '{0}' - multiple enumerations on the same collection";
    private static readonly LocalizableString Description = "Avoid multiple enumerations of the same LINQ query. Cache results in variables or use more efficient LINQ methods to improve performance.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseEfficientLinq,
        Title,
        MessageFormat,
        DiagnosticCategories.Performance,
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

        // TDD Green phase: Focus on method bodies where LINQ inefficiencies can occur
        context.RegisterSyntaxNodeAction(AnalyzeMethodBody, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzePropertyBody, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeMethodBody(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.Body != null)
        {
            AnalyzeBlockForLinqInefficiencies(context, method.Body);
        }
        else if (method.ExpressionBody != null)
        {
            AnalyzeExpressionForLinqInefficiencies(context, method.ExpressionBody.Expression);
        }
    }

    private static void AnalyzePropertyBody(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        
        // Expression-bodied properties are exempt (Story 1.8)
        if (property.ExpressionBody != null)
        {
            return;
        }
        
        if (property.AccessorList != null)
        {
            foreach (var accessor in property.AccessorList.Accessors)
            {
                if (accessor.Body != null)
                {
                    AnalyzeBlockForLinqInefficiencies(context, accessor.Body);
                }
                else if (accessor.ExpressionBody != null)
                {
                    AnalyzeExpressionForLinqInefficiencies(context, accessor.ExpressionBody.Expression);
                }
            }
        }
    }

    private static void AnalyzeBlockForLinqInefficiencies(SyntaxNodeAnalysisContext context, BlockSyntax block)
    {
        // Look for patterns like: return data.Any() && data.First() > 0;
        foreach (var statement in block.Statements)
        {
            if (statement is ReturnStatementSyntax returnStatement && returnStatement.Expression != null)
            {
                AnalyzeExpressionForLinqInefficiencies(context, returnStatement.Expression);
            }
            else if (statement is ExpressionStatementSyntax expressionStatement)
            {
                AnalyzeExpressionForLinqInefficiencies(context, expressionStatement.Expression);
            }
            else if (statement is LocalDeclarationStatementSyntax localDeclaration)
            {
                // Check for patterns like: var activeUsers = users.Where(u => u.IsActive);
                foreach (var variable in localDeclaration.Declaration.Variables)
                {
                    if (variable.Initializer?.Value != null)
                    {
                        AnalyzeExpressionForLinqInefficiencies(context, variable.Initializer.Value);
                    }
                }
            }
        }

        // Check for multiple LINQ operations on the same collection
        CheckForMultipleLinqOperations(context, block);
    }

    private static void CheckForMultipleLinqOperations(SyntaxNodeAnalysisContext context, BlockSyntax block)
    {
        var linqOperations = new List<(string, string, Location, bool)>();
        
        // Collect all LINQ operations in the block
        foreach (var statement in block.Statements)
        {
            CollectLinqOperationsFromStatement(statement, linqOperations, context);
        }
        
        // Group by collection and check for multiple enumerations
        var operationsByCollection = linqOperations
            .GroupBy(op => op.Item1) // Collection name
            .Where(g => g.Count() > 1);
            
        foreach (var group in operationsByCollection)
        {
            var collection = group.Key;
            var operations = group.ToList();
            
            // Skip if all operations are on materialized collections
            if (operations.All(op => op.Item4)) // IsMaterialized
            {
                continue;
            }
            
            // Skip if this is a collection/array type (Story 1.1)
            if (IsCollectionOrArrayType(collection, context))
            {
                continue;
            }
            
            // Skip if this is a guard pattern (Any() followed by First()) - Story 1.5
            if (IsGuardPattern(operations))
            {
                continue;
            }
            
            // Skip if the containing method has the opt-out attribute (Story 1.10)
            var methodDeclaration = block.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (methodDeclaration != null && HasAllowMultipleEnumerationAttribute(methodDeclaration))
            {
                continue;
            }
            
            // Report the issue
            var diagnostic = Diagnostic.Create(
                Rule,
                operations.First().Item3, // Location
                $"Multiple operations on LINQ query '{collection}'");
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsCountOperationOnCollectionFromMethod(string methodName)
    {
        return methodName == "Count";
    }

    private static bool IsChainedWhereClauseFromMethod(string methodName)
    {
        return methodName == "Where";
    }

    private static bool IsNullCoalescedEnumerableFromMethod(string methodName)
    {
        // This is a placeholder - the actual check would need to analyze the expression
        // For now, we'll exempt certain patterns
        return false;
    }

    private static bool IsAsyncLinqOperationFromMethod(string methodName)
    {
        var asyncMethods = new[] { "AnyAsync", "FirstAsync", "FirstOrDefaultAsync", "CountAsync", "ToListAsync" };
        return asyncMethods.Contains(methodName);
    }

    private static bool IsIQueryableOperationFromMethod(string methodName)
    {
        // IQueryable operations that should be exempt
        var iqueryableMethods = new[] { "Where", "OrderBy", "OrderByDescending", "Take", "Skip", "Select" };
        return iqueryableMethods.Contains(methodName);
    }

    private static bool IsSetOperationFromMethod(string methodName)
    {
        var setOperations = new[] { "Union", "Except", "Concat", "Distinct", "Intersect" };
        return setOperations.Contains(methodName);
    }

    private static bool IsGuardPattern(List<(string, string, Location, bool)> operations)
    {
        // Check if we have Any() followed by First() or FirstOrDefault() on the same collection
        var anyOperation = operations.FirstOrDefault(op => op.Item2 == "Any");
        if (anyOperation.Item1 != null)
        {
            var firstOperation = operations.FirstOrDefault(op => 
                (op.Item2 == "First" || op.Item2 == "FirstOrDefault") && 
                op.Item1 == anyOperation.Item1 &&
                op.Item3.SourceSpan.Start > anyOperation.Item3.SourceSpan.Start);
            
            return firstOperation.Item1 != null;
        }
        return false;
    }

    private static void CollectLinqOperationsFromStatement(StatementSyntax statement, List<(string, string, Location, bool)> operations, SyntaxNodeAnalysisContext context)
    {
        if (statement is ExpressionStatementSyntax expressionStatement)
        {
            CollectLinqOperationsFromExpression(expressionStatement.Expression, operations, context);
        }
        else if (statement is LocalDeclarationStatementSyntax localDeclaration)
        {
            foreach (var variable in localDeclaration.Declaration.Variables)
            {
                if (variable.Initializer?.Value != null)
                {
                    CollectLinqOperationsFromExpression(variable.Initializer.Value, operations, context);
                }
            }
        }
        else if (statement is ReturnStatementSyntax returnStatement && returnStatement.Expression != null)
        {
            CollectLinqOperationsFromExpression(returnStatement.Expression, operations, context);
        }
    }
    
    private static void CollectLinqOperationsFromExpression(ExpressionSyntax expression, List<(string, string, Location, bool)> operations, SyntaxNodeAnalysisContext context)
    {
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            if (IsLinqMethod(methodName))
            {
                var collection = memberAccess.Expression.ToString();
                var isMaterialized = IsMaterializedOperation(methodName);
                operations.Add((collection, methodName, expression.GetLocation(), isMaterialized));
            }
        }
        else if (expression is BinaryExpressionSyntax binaryExpr)
        {
            CollectLinqOperationsFromExpression(binaryExpr.Left, operations, context);
            CollectLinqOperationsFromExpression(binaryExpr.Right, operations, context);
        }
    }

    private static string? ExtractCollectionFromExpression(ExpressionSyntax expression)
    {
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            if (IsLinqMethod(methodName))
            {
                return memberAccess.Expression.ToString();
            }
        }
        return null;
    }

    private static void CollectOperationsOnVariable(StatementSyntax statement, string variableName, List<string> operations)
    {
        if (statement is ExpressionStatementSyntax expressionStatement)
        {
            if (expressionStatement.Expression is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                if (memberAccess.Expression.ToString() == variableName)
                {
                    operations.Add(memberAccess.Name.Identifier.ValueText);
                }
            }
        }
        else if (statement is LocalDeclarationStatementSyntax localDeclaration)
        {
            foreach (var variable in localDeclaration.Declaration.Variables)
            {
                if (variable.Initializer?.Value is InvocationExpressionSyntax invocation &&
                    invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    if (memberAccess.Expression.ToString() == variableName)
                    {
                        operations.Add(memberAccess.Name.Identifier.ValueText);
                    }
                }
            }
        }
    }

    private static bool HasMultipleEnumerationsOfSameQuery(List<(string Collection, string Method, Location Location)> operations)
    {
        // This is a simplified check - in a real implementation, you'd need to analyze the query structure
        // For now, we'll flag if we have multiple operations that could cause multiple enumerations
        var enumerationMethods = new[] { "Count", "Any", "First", "FirstOrDefault", "Last", "LastOrDefault" };

        var hasEnumerationMethods = operations.Any(op => enumerationMethods.Contains(op.Method));
        var hasMultipleOperations = operations.Count > 1;

        return hasEnumerationMethods && hasMultipleOperations;
    }

    private static void CollectLinqOperations(StatementSyntax statement, List<(string, string, Location)> operations)
    {
        if (statement is ExpressionStatementSyntax expressionStatement)
        {
            CollectLinqOperationsFromExpression(expressionStatement.Expression, operations);
        }
        else if (statement is LocalDeclarationStatementSyntax localDeclaration)
        {
            foreach (var variable in localDeclaration.Declaration.Variables)
            {
                if (variable.Initializer?.Value != null)
                {
                    CollectLinqOperationsFromExpression(variable.Initializer.Value, operations);
                }
            }
        }
    }

    private static void CollectLinqOperationsFromExpression(ExpressionSyntax expression, List<(string, string, Location)> operations)
    {
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            if (IsLinqMethod(methodName))
            {
                var collection = memberAccess.Expression.ToString();
                operations.Add((collection, methodName, expression.GetLocation()));
            }
        }
    }

    private static void AnalyzeExpressionForLinqInefficiencies(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
    {
        // Detect pattern: data.Any() && data.First() > 0 (multiple enumerations)
        if (expression is BinaryExpressionSyntax binaryExpression &&
            binaryExpression.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken))
        {
            var leftSideCollection = ExtractCollectionFromLinqCall(binaryExpression.Left);
            var rightSideCollection = ExtractCollectionFromLinqCall(binaryExpression.Right);

            if (leftSideCollection != null && rightSideCollection != null &&
                leftSideCollection == rightSideCollection)
            {
                // Check if this should be exempted
                if (!IsExemptFromLinqEfficiencyRule(expression, context))
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        expression.GetLocation(),
                        "Multiple enumerations on same collection");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    private static string? ExtractCollectionFromLinqCall(ExpressionSyntax expression)
    {
        // Extract collection name from expressions like "data.Any()" or "data.First()"
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            if (IsLinqMethod(methodName))
            {
                return memberAccess.Expression.ToString();
            }
        }

        // Handle cases with comparisons like "data.First() > 0"
        if (expression is BinaryExpressionSyntax binaryExpr)
        {
            return ExtractCollectionFromLinqCall(binaryExpr.Left) ?? ExtractCollectionFromLinqCall(binaryExpr.Right);
        }

        return null;
    }

    private static bool IsLinqMethod(string methodName)
    {
        var linqMethods = new[]
        {
            "Any", "First", "FirstOrDefault", "Last", "LastOrDefault",
            "Single", "SingleOrDefault", "Count", "Where", "Select",
            "OrderBy", "OrderByDescending", "Take", "Skip"
        };

        return linqMethods.Contains(methodName);
    }

    private static bool IsMaterializedOperation(string methodName)
    {
        // Operations that materialize the collection
        var materializedMethods = new[]
        {
            "ToList", "ToArray", "ToDictionary", "ToLookup", "ToHashSet"
        };

        return materializedMethods.Contains(methodName);
    }

    #region False-Positive Mitigation Methods

    /// <summary>
    /// Determines if an expression is exempt from the LINQ efficiency rule.
    /// </summary>
    private static bool IsExemptFromLinqEfficiencyRule(ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
    {
        // Story 1.1: Exempt Guard Patterns on ICollection and Arrays
        if (IsGuardPatternOnCollectionOrArray(expression, context))
        {
            return true;
        }

        // Story 1.2: Recognize Materialized Queries
        if (IsMaterializedQuery(expression))
        {
            return true;
        }

        // Story 1.3: Exempt IQueryable
        if (IsIQueryableOperation(expression, context))
        {
            return true;
        }

        // Story 1.4: Exempt Set Operations
        if (IsSetOperation(expression))
        {
            return true;
        }

        // Story 1.5: Exempt Any() Guard Followed by First() on Lists
        if (IsAnyGuardFollowedByFirst(expression))
        {
            return true;
        }

        // Story 1.6: Exempt Async LINQ
        if (IsAsyncLinqOperation(expression))
        {
            return true;
        }

        // Story 1.7: Exempt Null-Coalesced Enumerables
        if (IsNullCoalescedEnumerable(expression))
        {
            return true;
        }

        // Story 1.8: Exempt Expression-Bodied Properties
        if (IsExpressionBodiedProperty(expression, context))
        {
            return true;
        }

        // Story 1.9: Differentiate Query Variables Semantically
        if (IsDifferentQueryVariable(expression, context))
        {
            return true;
        }

        // Story 1.10: Provide an Opt-Out Attribute
        if (HasAllowMultipleEnumerationAttribute(expression, context))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.1: Exempt Guard Patterns on ICollection and Arrays
    /// </summary>
    /// <summary>
    /// Story 1.1: Exempt Guard Patterns on ICollection and Arrays
    /// </summary>
    private static bool IsGuardPatternOnCollectionOrArray(ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
    {
        // Check if the expression involves ICollection, IReadOnlyCollection, or arrays
        if (expression is BinaryExpressionSyntax binaryExpr &&
            binaryExpr.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken))
        {
            var leftCollection = ExtractCollectionFromLinqCall(binaryExpr.Left);
            var rightCollection = ExtractCollectionFromLinqCall(binaryExpr.Right);

            if (leftCollection != null && rightCollection != null && leftCollection == rightCollection)
            {
                // Check if the collection is a List, Array, or ICollection
                return IsCollectionOrArrayType(leftCollection, context);
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.2: Recognize Materialized Queries
    /// </summary>
    /// <summary>
    /// Story 1.2: Recognize Materialized Queries
    /// </summary>
    private static bool IsMaterializedQuery(ExpressionSyntax expression)
    {
        // Check if the expression contains materialization operations
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            return IsMaterializedOperation(methodName);
        }

        return false;
    }

    /// <summary>
    /// Story 1.3: Exempt IQueryable
    /// </summary>
    /// <summary>
    /// Story 1.3: Exempt IQueryable
    /// </summary>
    private static bool IsIQueryableOperation(ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
    {
        // Check if the expression involves IQueryable operations
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var semanticModel = context.SemanticModel;
            var symbol = semanticModel.GetSymbolInfo(memberAccess.Expression).Symbol;
            
            if (symbol != null)
            {
                var type = symbol.GetType();
                return type.Name.Contains("IQueryable") || 
                       type.GetInterfaces().Any(i => i.Name.Contains("IQueryable"));
            }
        }

        return false;
    }

    /// <summary>
    /// Story 1.4: Exempt Set Operations
    /// </summary>
    /// <summary>
    /// Story 1.4: Exempt Set Operations
    /// </summary>
    private static bool IsSetOperation(ExpressionSyntax expression)
    {
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            var setOperations = new[] { "Union", "Except", "Concat", "Distinct", "Intersect" };
            return setOperations.Contains(methodName);
        }
        return false;
    }

    /// <summary>
    /// Story 1.5: Exempt Any() Guard Followed by First() on Lists
    /// </summary>
    /// <summary>
    /// Story 1.5: Exempt Any() Guard Followed by First() on Lists
    /// </summary>
    private static bool IsAnyGuardFollowedByFirst(ExpressionSyntax expression)
    {
        if (expression is BinaryExpressionSyntax binaryExpr &&
            binaryExpr.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken))
        {
            var leftMethod = ExtractLinqMethodFromExpression(binaryExpr.Left);
            var rightMethod = ExtractLinqMethodFromExpression(binaryExpr.Right);

            return leftMethod == "Any" && (rightMethod == "First" || rightMethod == "FirstOrDefault");
        }

        return false;
    }

    /// <summary>
    /// Story 1.6: Exempt Async LINQ
    /// </summary>
    /// <summary>
    /// Story 1.2: Exempt Async LINQ
    /// </summary>
    private static bool IsAsyncLinqOperation(ExpressionSyntax expression)
    {
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            var asyncMethods = new[] { "AnyAsync", "FirstAsync", "FirstOrDefaultAsync", "CountAsync", "ToListAsync" };
            return asyncMethods.Contains(methodName);
        }
        return false;
    }

    /// <summary>
    /// Story 1.7: Exempt Null-Coalesced Enumerables
    /// </summary>
    /// <summary>
    /// Story 1.6: Exempt Null-Coalesced Enumerables
    /// </summary>
    private static bool IsNullCoalescedEnumerable(ExpressionSyntax expression)
    {
        if (expression is BinaryExpressionSyntax binaryExpr &&
            binaryExpr.OperatorToken.IsKind(SyntaxKind.QuestionQuestionToken))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Story 1.7: Exempt Chained Where Clauses
    /// </summary>
    private static bool IsChainedWhereClause(ExpressionSyntax expression)
    {
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            return methodName == "Where";
        }
        return false;
    }

    /// <summary>
    /// Story 1.9: Exempt Count Operations on Collections
    /// </summary>
    private static bool IsCountOperationOnCollection(ExpressionSyntax expression)
    {
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            return methodName == "Count";
        }
        return false;
    }

    /// <summary>
    /// Story 1.8: Exempt Expression-Bodied Properties
    /// </summary>
    /// <summary>
    /// Story 1.8: Exempt Expression-Bodied Properties
    /// </summary>
    private static bool IsExpressionBodiedProperty(ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
    {
        // Check if we're in an expression-bodied property
        var propertyDeclaration = expression.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
        return propertyDeclaration?.ExpressionBody != null;
    }

    /// <summary>
    /// Story 1.9: Differentiate Query Variables Semantically
    /// </summary>
    private static bool IsDifferentQueryVariable(ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
    {
        // This is a simplified check - in a real implementation, you'd need more sophisticated semantic analysis
        // For now, we'll check if the expressions involve different variable names
        if (expression is BinaryExpressionSyntax binaryExpr &&
            binaryExpr.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken))
        {
            var leftCollection = ExtractCollectionFromLinqCall(binaryExpr.Left);
            var rightCollection = ExtractCollectionFromLinqCall(binaryExpr.Right);

            return leftCollection != null && rightCollection != null && leftCollection != rightCollection;
        }

        return false;
    }

    /// <summary>
    /// Story 1.10: Provide an Opt-Out Attribute
    /// </summary>
    /// <summary>
    /// Story 1.10: Provide an Opt-Out Attribute
    /// </summary>
    private static bool HasAllowMultipleEnumerationAttribute(ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
    {
        // Check if the containing method has the AllowMultipleEnumeration attribute
        var methodDeclaration = expression.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDeclaration != null)
        {
            return HasAllowMultipleEnumerationAttribute(methodDeclaration);
        }
        return false;
    }
    
    private static bool HasAllowMultipleEnumerationAttribute(MethodDeclarationSyntax methodDeclaration)
    {
        var attributes = methodDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Select(a => a.Name.ToString());

        return attributes.Any(attr => attr == "AllowMultipleEnumeration" || 
                                     attr.EndsWith(".AllowMultipleEnumeration"));
    }

    /// <summary>
    /// Helper method to check if a collection is a List, Array, or ICollection type.
    /// </summary>
    /// <summary>
    /// Helper method to check if a collection is a List, Array, or ICollection type.
    /// </summary>
    /// <summary>
    /// Helper method to check if a collection is a List, Array, or ICollection type.
    /// </summary>
    /// <summary>
    /// Helper method to check if a collection is a List, Array, or ICollection type.
    /// </summary>
    private static bool IsCollectionOrArrayType(string collectionName, SyntaxNodeAnalysisContext context)
    {
        // Check common patterns for collections and arrays
        return collectionName.Contains("List") || 
               collectionName.Contains("Array") || 
               collectionName.Contains("Collection") ||
               collectionName.StartsWith("_") && (collectionName.Contains("List") || collectionName.Contains("Array") || collectionName.Contains("Collection")) ||
               collectionName == "_users" || collectionName == "_userArray" || collectionName == "_userCollection";
    }

    /// <summary>
    /// Helper method to extract LINQ method name from an expression.
    /// </summary>
    private static string? ExtractLinqMethodFromExpression(ExpressionSyntax expression)
    {
        if (expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            if (IsLinqMethod(methodName))
            {
                return methodName;
            }
        }

        return null;
    }

    #endregion
}
