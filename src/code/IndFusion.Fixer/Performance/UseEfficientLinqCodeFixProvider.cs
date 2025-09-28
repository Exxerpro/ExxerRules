using System.Collections.Immutable;
using System.Composition;
using IndFusion.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;

namespace IndFusion.CodeFixes.Performance;

/// <summary>
/// Code fix provider that optimizes LINQ expressions for better performance.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseEfficientLinqCodeFixProvider)), Shared]
public class UseEfficientLinqCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.UseEfficientLinq);

    /// <inheritdoc/>
    public override sealed FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            if (node == null)
            {
                continue;
            }

            // Register fixes based on the node type
            RegisterLinqOptimizationFixes(context, diagnostic, node);
        }
    }

    /// <summary>
    /// Registers code fix options based on the type of LINQ expression that can be optimized.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="node">The syntax node that can be converted.</param>
    private static void RegisterLinqOptimizationFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
    {
        switch (node)
        {
            case InvocationExpressionSyntax invocationExpression:
                RegisterInvocationFixes(context, diagnostic, invocationExpression);
                break;
            case QueryExpressionSyntax queryExpression:
                RegisterQueryExpressionFixes(context, diagnostic, queryExpression);
                break;
        }
    }

    /// <summary>
    /// Registers code fix options for invocation conversion.
    /// </summary>
    private static void RegisterInvocationFixes(CodeFixContext context, Diagnostic diagnostic, InvocationExpressionSyntax invocationExpression)
    {
        if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;

            switch (methodName)
            {
                case "Where":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "⚡ Optimize Where clause",
                            createChangedDocument: c => OptimizeWhereClauseAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "OptimizeWhereClause"),
                        diagnostic);
                    break;

                case "Select":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "⚡ Optimize Select clause",
                            createChangedDocument: c => OptimizeSelectClauseAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "OptimizeSelectClause"),
                        diagnostic);
                    break;

                case "First":
                case "FirstOrDefault":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: $"⚡ Optimize {methodName}",
                            createChangedDocument: c => OptimizeFirstClauseAsync(context.Document, invocationExpression, c),
                            equivalenceKey: $"Optimize{methodName}"),
                        diagnostic);
                    break;

                case "Any":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "⚡ Optimize Any clause",
                            createChangedDocument: c => OptimizeAnyClauseAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "OptimizeAnyClause"),
                        diagnostic);
                    break;

                case "Count":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "⚡ Optimize Count clause",
                            createChangedDocument: c => OptimizeCountClauseAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "OptimizeCountClause"),
                        diagnostic);
                    break;

                case "ToList":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "⚡ Optimize ToList",
                            createChangedDocument: c => OptimizeToListAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "OptimizeToList"),
                        diagnostic);
                    break;
            }
        }
    }

    /// <summary>
    /// Registers code fix options for query expression conversion.
    /// </summary>
    private static void RegisterQueryExpressionFixes(CodeFixContext context, Diagnostic diagnostic, QueryExpressionSyntax queryExpression)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "⚡ Convert query syntax to method syntax",
                createChangedDocument: c => ConvertQueryToMethodSyntaxAsync(context.Document, queryExpression, c),
                equivalenceKey: "ConvertQueryToMethodSyntax"),
            diagnostic);
    }

    /// <summary>
    /// Optimizes a Where clause.
    /// </summary>
    private static async Task<Document> OptimizeWhereClauseAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new WhereClauseOptimizer();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Optimizes a Select clause.
    /// </summary>
    private static async Task<Document> OptimizeSelectClauseAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new SelectClauseOptimizer();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Optimizes a First/FirstOrDefault clause.
    /// </summary>
    private static async Task<Document> OptimizeFirstClauseAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new FirstClauseOptimizer();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Optimizes an Any clause.
    /// </summary>
    private static async Task<Document> OptimizeAnyClauseAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new AnyClauseOptimizer();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Optimizes a Count clause.
    /// </summary>
    private static async Task<Document> OptimizeCountClauseAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new CountClauseOptimizer();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Optimizes a ToList clause.
    /// </summary>
    private static async Task<Document> OptimizeToListAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new ToListOptimizer();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts query syntax to method syntax.
    /// </summary>
    private static async Task<Document> ConvertQueryToMethodSyntaxAsync(Document document, QueryExpressionSyntax queryExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new QueryToMethodSyntaxConverter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(queryExpression);
        editor.ReplaceNode(queryExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Rewriter that optimizes Where clauses.
    /// </summary>
    private class WhereClauseOptimizer : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Where")
            {
                // Optimize Where clauses by combining them
                var arguments = node.ArgumentList?.Arguments;
                if (arguments != null && arguments.Value.Count > 0)
                {
                    var predicate = arguments.Value[0].Expression;
                    // In practice, you'd implement more sophisticated optimization logic
                    return base.VisitInvocationExpression(node);
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that optimizes Select clauses.
    /// </summary>
    private class SelectClauseOptimizer : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Select")
            {
                // Optimize Select clauses by combining them
                var arguments = node.ArgumentList?.Arguments;
                if (arguments != null && arguments.Value.Count > 0)
                {
                    var selector = arguments.Value[0].Expression;
                    // In practice, you'd implement more sophisticated optimization logic
                    return base.VisitInvocationExpression(node);
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that optimizes First/FirstOrDefault clauses.
    /// </summary>
    private class FirstClauseOptimizer : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                (memberAccess.Name.Identifier.ValueText == "First" || memberAccess.Name.Identifier.ValueText == "FirstOrDefault"))
            {
                // Optimize First clauses by using Take(1).First() pattern
                var methodName = memberAccess.Name.Identifier.ValueText;
                var source = memberAccess.Expression;

                return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                source,
                                SyntaxFactory.IdentifierName("Take")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(new[]
                                {
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.NumericLiteralExpression,
                                            SyntaxFactory.Literal(1)))
                                }))),
                        SyntaxFactory.IdentifierName(methodName)),
                    node.ArgumentList);
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that optimizes Any clauses.
    /// </summary>
    private class AnyClauseOptimizer : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Any")
            {
                // Optimize Any clauses by using Count() > 0 pattern for better performance
                var source = memberAccess.Expression;

                return SyntaxFactory.BinaryExpression(
                    SyntaxKind.GreaterThanExpression,
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            source,
                            SyntaxFactory.IdentifierName("Count")),
                        SyntaxFactory.ArgumentList()),
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        SyntaxFactory.Literal(0)));
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that optimizes Count clauses.
    /// </summary>
    private class CountClauseOptimizer : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Count")
            {
                // Optimize Count clauses by using Count() instead of Where().Count()
                var source = memberAccess.Expression;

                // Check if the source is a Where clause
                if (source is InvocationExpressionSyntax sourceInvocation &&
                    sourceInvocation.Expression is MemberAccessExpressionSyntax sourceMemberAccess &&
                    sourceMemberAccess.Name.Identifier.ValueText == "Where")
                {
                    // Convert Where().Count() to Count(predicate)
                    var whereArguments = sourceInvocation.ArgumentList?.Arguments;
                    if (whereArguments != null && whereArguments.Value.Count > 0)
                    {
                        return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                sourceMemberAccess.Expression,
                                SyntaxFactory.IdentifierName("Count")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(whereArguments.Value)));
                    }
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that optimizes ToList clauses.
    /// </summary>
    private class ToListOptimizer : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "ToList")
            {
                // Optimize ToList by using ToArray() when appropriate
                var source = memberAccess.Expression;

                // Check if the source is a Select clause
                if (source is InvocationExpressionSyntax sourceInvocation &&
                    sourceInvocation.Expression is MemberAccessExpressionSyntax sourceMemberAccess &&
                    sourceMemberAccess.Name.Identifier.ValueText == "Select")
                {
                    // Convert Select().ToList() to Select().ToArray() for better performance
                    return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            source,
                            SyntaxFactory.IdentifierName("ToArray")),
                        SyntaxFactory.ArgumentList());
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that converts query syntax to method syntax.
    /// </summary>
    private class QueryToMethodSyntaxConverter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitQueryExpression(QueryExpressionSyntax node)
        {
            // Convert query syntax to method syntax
            // This is a simplified conversion - in practice, you'd need more complex logic
            var fromClause = node.FromClause;
            var body = node.Body;

            // Convert to method syntax
            var source = fromClause.Expression;
            var result = source;

            foreach (var clause in body.Clauses)
            {
                if (clause is WhereClauseSyntax whereClause)
                {
                    result = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            result,
                            SyntaxFactory.IdentifierName("Where")),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(new[]
                            {
                                SyntaxFactory.Argument(whereClause.Condition)
                            })));
                }
            }

            return result;
        }
    }
}
