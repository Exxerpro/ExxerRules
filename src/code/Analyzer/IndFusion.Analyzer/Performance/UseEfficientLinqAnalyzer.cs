using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Performance;

/// <summary>
/// Reports EXXER700 when the same deferred LINQ pipeline (or other non-materialised enumerable)
/// is enumerated multiple times. Collections that are already materialised (arrays, List&lt;T&gt;, ICollection)
/// are exempt so that common guard patterns remain valid.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseEfficientLinqAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseEfficientLinq,
        title: "Use efficient LINQ operations",
        messageFormat: "Inefficient LINQ usage detected: '{0}' is enumerated multiple times",
        DiagnosticCategories.Performance,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Avoid multiple enumerations of deferred LINQ queries or IEnumerable sources that are not materialised. Cache the results before reusing them.");

    private static readonly ImmutableHashSet<string> EnumerationMethods = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "Any",
        "All",
        "Count",
        "First",
        "FirstOrDefault",
        "Last",
        "LastOrDefault",
        "Single",
        "SingleOrDefault",
        "ElementAt",
        "ElementAtOrDefault",
        "Sum",
        "Average",
        "Min",
        "Max");

    private static readonly ImmutableHashSet<string> QueryMethods = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "Where",
        "Select",
        "SelectMany",
        "OrderBy",
        "OrderByDescending",
        "ThenBy",
        "ThenByDescending",
        "Distinct",
        "Skip",
        "SkipWhile",
        "Take",
        "TakeWhile",
        "GroupBy",
        "Join",
        "GroupJoin");

    private static readonly ImmutableHashSet<string> MaterializationMethods = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "ToList",
        "ToArray",
        "ToDictionary",
        "ToHashSet",
        "ToLookup");

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the inefficient LINQ usage rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax callbacks that analyze methods and properties for repeated LINQ enumerations.
    /// </summary>
    /// <param name="context">The analysis context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (HasAllowMultipleEnumerationAttribute(method.AttributeLists, context.SemanticModel, context.CancellationToken) ||
            HasAllowMultipleEnumerationOnParent(method.Parent, context.SemanticModel, context.CancellationToken))
        {
            return;
        }

        if (method.Body is { } body)
        {
            AnalyzeBlock(context, body);
        }
        else if (method.ExpressionBody is { Expression: var expression })
        {
            AnalyzeExpression(context, expression);
        }
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;

        if (HasAllowMultipleEnumerationAttribute(property.AttributeLists, context.SemanticModel, context.CancellationToken) ||
            HasAllowMultipleEnumerationOnParent(property.Parent, context.SemanticModel, context.CancellationToken))
        {
            return;
        }

        if (property.ExpressionBody is { Expression: var expressionBody })
        {
            AnalyzeExpression(context, expressionBody);
            return;
        }

        if (property.AccessorList is null)
        {
            return;
        }

        foreach (var accessor in property.AccessorList.Accessors)
        {
            if (HasAllowMultipleEnumerationAttribute(accessor.AttributeLists, context.SemanticModel, context.CancellationToken))
            {
                continue;
            }

            if (accessor.Body is { } accessorBody)
            {
                AnalyzeBlock(context, accessorBody);
            }
            else if (accessor.ExpressionBody is { Expression: var accessorExpression })
            {
                AnalyzeExpression(context, accessorExpression);
            }
        }
    }

    private static void AnalyzeBlock(SyntaxNodeAnalysisContext context, BlockSyntax block)
    {
        var semanticModel = context.SemanticModel;
        var cancellationToken = context.CancellationToken;

        var materialisedSymbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
        var querySymbols = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

        PopulateSymbolSets(block, semanticModel, cancellationToken, materialisedSymbols, querySymbols);

        var enumerations = new Dictionary<EnumerationTarget, List<InvocationExpressionSyntax>>(EnumerationTargetComparer.Instance);
        foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            ProcessInvocation(invocation, semanticModel, cancellationToken, materialisedSymbols, querySymbols, enumerations);
        }

        ReportDiagnostics(enumerations, context);
    }

    private static void AnalyzeExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
    {
        var semanticModel = context.SemanticModel;
        var cancellationToken = context.CancellationToken;

        var enumerations = new Dictionary<EnumerationTarget, List<InvocationExpressionSyntax>>(EnumerationTargetComparer.Instance);
        foreach (var invocation in expression.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
        {
            ProcessInvocation(invocation, semanticModel, cancellationToken, materialisedSymbols: null, querySymbols: null, enumerations);
        }

        ReportDiagnostics(enumerations, context);
    }

    private static void PopulateSymbolSets(
        BlockSyntax block,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        HashSet<ISymbol> materialisedSymbols,
        HashSet<ISymbol> querySymbols)
    {
        foreach (var localDeclaration in block.DescendantNodes().OfType<LocalDeclarationStatementSyntax>())
        {
            foreach (var variable in localDeclaration.Declaration.Variables)
            {
                if (semanticModel.GetDeclaredSymbol(variable, cancellationToken) is not ISymbol symbol)
                {
                    continue;
                }

                var initializer = variable.Initializer?.Value;
                if (initializer is not InvocationExpressionSyntax invocation ||
                    invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
                {
                    continue;
                }

                var methodName = memberAccess.Name.Identifier.ValueText;
                if (MaterializationMethods.Contains(methodName))
                {
                    materialisedSymbols.Add(symbol);
                }
                else if (QueryMethods.Contains(methodName))
                {
                    querySymbols.Add(symbol);
                }
            }
        }
    }

    private static void ProcessInvocation(
        InvocationExpressionSyntax invocation,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        HashSet<ISymbol>? materialisedSymbols,
        HashSet<ISymbol>? querySymbols,
        Dictionary<EnumerationTarget, List<InvocationExpressionSyntax>> enumerations)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return;
        }

        var methodName = memberAccess.Name.Identifier.ValueText;
        if (!EnumerationMethods.Contains(methodName))
        {
            return;
        }

        var targetExpression = memberAccess.Expression;
        var symbolInfo = semanticModel.GetSymbolInfo(targetExpression, cancellationToken);
        var targetSymbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
        if (targetSymbol is null && targetExpression is IdentifierNameSyntax identifierName)
        {
            targetSymbol = semanticModel.LookupSymbols(invocation.SpanStart, name: identifierName.Identifier.ValueText)
                .FirstOrDefault();
        }
        var typeInfo = semanticModel.GetTypeInfo(targetExpression, cancellationToken);
        var targetType = typeInfo.Type;
        var convertedType = typeInfo.ConvertedType;
        var symbolType = targetSymbol switch
        {
            ILocalSymbol local => local.Type,
            IParameterSymbol parameter => parameter.Type,
            IPropertySymbol property => property.Type,
            IFieldSymbol field => field.Type,
            _ => null
        };

        if (targetSymbol is not null && materialisedSymbols is not null && materialisedSymbols.Contains(targetSymbol))
        {
            return;
        }

        if (!IsDeferredSource(targetSymbol, symbolType, targetType, convertedType, querySymbols))
        {
            return;
        }

        var key = new EnumerationTarget(targetSymbol, targetExpression.ToString());
        if (!enumerations.TryGetValue(key, out var list))
        {
            list = new List<InvocationExpressionSyntax>();
            enumerations.Add(key, list);
        }

        list.Add(invocation);
    }

    private static void ReportDiagnostics(
        Dictionary<EnumerationTarget, List<InvocationExpressionSyntax>> enumerations,
        SyntaxNodeAnalysisContext context)
    {
        foreach (var entry in enumerations)
        {
            if (entry.Value.Count < 2)
            {
                continue;
            }

            var invocation = entry.Value[0];
            var collectionName = entry.Key.Symbol?.Name ?? entry.Key.ExpressionText;

            var diagnostic = Diagnostic.Create(
                Rule,
                invocation.GetLocation(),
                collectionName);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool HasAllowMultipleEnumerationAttribute(
        SyntaxList<AttributeListSyntax> attributeLists,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        foreach (var attribute in attributeLists.SelectMany(list => list.Attributes))
        {
            if (IsAllowMultipleEnumerationAttribute(attribute, semanticModel, cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasAllowMultipleEnumerationOnParent(
        SyntaxNode? node,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (node is not MemberDeclarationSyntax member)
        {
            return false;
        }

        return HasAllowMultipleEnumerationAttribute(member.AttributeLists, semanticModel, cancellationToken);
    }

    private static bool IsAllowMultipleEnumerationAttribute(
        AttributeSyntax attribute,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var symbol = semanticModel.GetSymbolInfo(attribute, cancellationToken).Symbol;
        if (symbol is IMethodSymbol methodSymbol)
        {
            var attributeType = methodSymbol.ContainingType;
            if (attributeType is not null &&
                (attributeType.Name.Equals("AllowMultipleEnumeration", StringComparison.Ordinal) ||
                 attributeType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).EndsWith(".AllowMultipleEnumeration", StringComparison.Ordinal)))
            {
                return true;
            }
        }

        var name = attribute.Name.ToString();
        return name.Equals("AllowMultipleEnumeration", StringComparison.Ordinal) ||
               name.EndsWith(".AllowMultipleEnumeration", StringComparison.Ordinal);
    }

    private static bool IsDeferredSource(
        ISymbol? symbol,
        ITypeSymbol? symbolType,
        ITypeSymbol? targetType,
        ITypeSymbol? convertedType,
        HashSet<ISymbol>? querySymbols)
    {
        if (symbol is not null && querySymbols is not null && querySymbols.Contains(symbol))
        {
            return true;
        }

        var effectiveType = symbolType ?? targetType ?? convertedType;
        if (effectiveType is null)
        {
            return false;
        }

        if (IsCollectionLike(effectiveType))
        {
            return false;
        }

        return ImplementsEnumerable(effectiveType);
    }

    private static bool IsCollectionLike(ITypeSymbol? type)
    {
        if (type is null)
        {
            return false;
        }

        if (type is IArrayTypeSymbol)
        {
            return true;
        }

        if (type.SpecialType == SpecialType.System_String)
        {
            return true;
        }

        if (type.AllInterfaces.Any(IsCollectionInterface))
        {
            return true;
        }

        for (var current = type; current is not null; current = current.BaseType)
        {
            if (IsCollectionInterface(current))
            {
                return true;
            }
        }

        return false;

        static bool IsCollectionInterface(ITypeSymbol symbol)
        {
            if (symbol.Name.Equals("ICollection", StringComparison.Ordinal))
            {
                var ns = symbol.ContainingNamespace?.ToDisplayString();
                if (ns is "System.Collections.Generic" or "System.Collections")
                {
                    return true;
                }
            }
            return false;
        }
    }

    private static bool ImplementsEnumerable(ITypeSymbol type)
    {
        if (type.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T or SpecialType.System_Collections_IEnumerable)
        {
            return true;
        }

        return type.AllInterfaces.Any(iface =>
            iface.SpecialType is SpecialType.System_Collections_Generic_IEnumerable_T or SpecialType.System_Collections_IEnumerable);
    }

    private readonly struct EnumerationTarget
    {
        public EnumerationTarget(ISymbol? symbol, string expressionText)
        {
            Symbol = symbol;
            ExpressionText = expressionText;
        }

        /// <summary>
        /// Gets the symbol associated with the deferred enumerable, when available.
        /// </summary>
        public ISymbol? Symbol { get; }

        /// <summary>
        /// Gets the textual representation of the enumeration target used when no symbol is available.
        /// </summary>
        public string ExpressionText { get; }
    }

    private sealed class EnumerationTargetComparer : IEqualityComparer<EnumerationTarget>
    {
        /// <summary>
        /// Gets the singleton comparer instance used to deduplicate enumeration targets.
        /// </summary>
        public static EnumerationTargetComparer Instance { get; } = new();

        /// <summary>
        /// Determines whether two enumeration targets refer to the same deferred enumerable.
        /// </summary>
        /// <param name="x">The first enumeration target.</param>
        /// <param name="y">The second enumeration target.</param>
        /// <returns><c>true</c> when both targets represent the same enumerable; otherwise, <c>false</c>.</returns>
        public bool Equals(EnumerationTarget x, EnumerationTarget y)
        {
            if (x.Symbol is not null && y.Symbol is not null)
            {
                return SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol);
            }

            if (x.Symbol is null ^ y.Symbol is null)
            {
                return false;
            }

            return string.Equals(x.ExpressionText, y.ExpressionText, StringComparison.Ordinal);
        }

        /// <summary>
        /// Computes a hash code for the specified enumeration target.
        /// </summary>
        /// <param name="obj">The enumeration target to hash.</param>
        /// <returns>A hash code representing the target.</returns>
        public int GetHashCode(EnumerationTarget obj)
        {
            if (obj.Symbol is not null)
            {
                return SymbolEqualityComparer.Default.GetHashCode(obj.Symbol);
            }

            return StringComparer.Ordinal.GetHashCode(obj.ExpressionText);
        }
    }
}
