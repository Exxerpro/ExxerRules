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

namespace IndFusion.CodeFixes.Logging;

/// <summary>
/// Code fix provider that replaces Console.WriteLine with proper logging.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConsoleWriteLineCodeFixProvider)), Shared]
public class ConsoleWriteLineCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.DoNotUseConsoleWriteLine);

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
            RegisterConsoleWriteLineFixes(context, diagnostic, node);
        }
    }

    /// <summary>
    /// Registers code fix options based on the type of Console.WriteLine that can be converted.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="node">The syntax node that can be converted.</param>
    private static void RegisterConsoleWriteLineFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
    {
        switch (node)
        {
            case InvocationExpressionSyntax invocationExpression:
                RegisterInvocationFixes(context, diagnostic, invocationExpression);
                break;
            case UsingDirectiveSyntax usingDirective:
                RegisterUsingFixes(context, diagnostic, usingDirective);
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
                case "WriteLine":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Replace Console.WriteLine with ILogger",
                            createChangedDocument: c => ReplaceConsoleWriteLineWithLoggerAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ReplaceConsoleWriteLineWithLogger"),
                        diagnostic);

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Replace Console.WriteLine with Debug.WriteLine",
                            createChangedDocument: c => ReplaceConsoleWriteLineWithDebugAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ReplaceConsoleWriteLineWithDebug"),
                        diagnostic);

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Replace Console.WriteLine with Trace.WriteLine",
                            createChangedDocument: c => ReplaceConsoleWriteLineWithTraceAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ReplaceConsoleWriteLineWithTrace"),
                        diagnostic);
                    break;

                case "Write":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Replace Console.Write with ILogger",
                            createChangedDocument: c => ReplaceConsoleWriteWithLoggerAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ReplaceConsoleWriteWithLogger"),
                        diagnostic);
                    break;
            }
        }
    }

    /// <summary>
    /// Registers code fix options for using directive conversion.
    /// </summary>
    private static void RegisterUsingFixes(CodeFixContext context, Diagnostic diagnostic, UsingDirectiveSyntax usingDirective)
    {
        var usingName = usingDirective.Name?.ToString();
        if (usingName == "System")
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "🔄 Add Microsoft.Extensions.Logging using",
                    createChangedDocument: c => AddLoggingUsingAsync(context.Document, usingDirective, c),
                    equivalenceKey: "AddLoggingUsing"),
                diagnostic);
        }
    }

    /// <summary>
    /// Replaces Console.WriteLine with ILogger.
    /// </summary>
    private static async Task<Document> ReplaceConsoleWriteLineWithLoggerAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new ConsoleToLoggerRewriter("LogInformation");
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        // Add logging using directive if not present
        await AddLoggingUsingIfNeededAsync(editor, cancellationToken).ConfigureAwait(false);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Replaces Console.WriteLine with Debug.WriteLine.
    /// </summary>
    private static async Task<Document> ReplaceConsoleWriteLineWithDebugAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new ConsoleToDebugRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        // Add System.Diagnostics using directive if not present
        await AddDebugUsingIfNeededAsync(editor, cancellationToken).ConfigureAwait(false);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Replaces Console.WriteLine with Trace.WriteLine.
    /// </summary>
    private static async Task<Document> ReplaceConsoleWriteLineWithTraceAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new ConsoleToTraceRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        // Add System.Diagnostics using directive if not present
        await AddTraceUsingIfNeededAsync(editor, cancellationToken).ConfigureAwait(false);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Replaces Console.Write with ILogger.
    /// </summary>
    private static async Task<Document> ReplaceConsoleWriteWithLoggerAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new ConsoleToLoggerRewriter("LogInformation");
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        // Add logging using directive if not present
        await AddLoggingUsingIfNeededAsync(editor, cancellationToken).ConfigureAwait(false);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds Microsoft.Extensions.Logging using directive.
    /// </summary>
    private static async Task<Document> AddLoggingUsingAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.Extensions.Logging"));
        editor.InsertAfter(usingDirective, newUsing);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds logging using directive if not present.
    /// </summary>
    private static async Task AddLoggingUsingIfNeededAsync(DocumentEditor editor, CancellationToken cancellationToken)
    {
        var document = editor.GetChangedDocument();
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is CompilationUnitSyntax compilationUnit && !root.ToString().Contains("Microsoft.Extensions.Logging"))
        {
            var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.Extensions.Logging"));
            var newCompilationUnit = compilationUnit.AddUsings(newUsing);
            editor.ReplaceNode(compilationUnit, newCompilationUnit);
        }
    }

    /// <summary>
    /// Adds System.Diagnostics using directive if not present.
    /// </summary>
    private static async Task AddDebugUsingIfNeededAsync(DocumentEditor editor, CancellationToken cancellationToken)
    {
        var document = editor.GetChangedDocument();
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is CompilationUnitSyntax compilationUnit && !root.ToString().Contains("System.Diagnostics"))
        {
            var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Diagnostics"));
            var newCompilationUnit = compilationUnit.AddUsings(newUsing);
            editor.ReplaceNode(compilationUnit, newCompilationUnit);
        }
    }

    /// <summary>
    /// Adds System.Diagnostics using directive if not present.
    /// </summary>
    private static async Task AddTraceUsingIfNeededAsync(DocumentEditor editor, CancellationToken cancellationToken)
    {
        var document = editor.GetChangedDocument();
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is CompilationUnitSyntax compilationUnit && !root.ToString().Contains("System.Diagnostics"))
        {
            var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Diagnostics"));
            var newCompilationUnit = compilationUnit.AddUsings(newUsing);
            editor.ReplaceNode(compilationUnit, newCompilationUnit);
        }
    }

    /// <summary>
    /// Rewriter that converts Console.WriteLine to ILogger.
    /// </summary>
    private class ConsoleToLoggerRewriter : CSharpSyntaxRewriter
    {
        private readonly string logMethod;

        public ConsoleToLoggerRewriter(string logMethod)
        {
            this.logMethod = logMethod;
        }

        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression.ToString() == "Console" &&
                (memberAccess.Name.Identifier.ValueText == "WriteLine" || memberAccess.Name.Identifier.ValueText == "Write"))
            {
                var arguments = node.ArgumentList?.Arguments;
                if (arguments != null && arguments.Value.Count > 0)
                {
                    return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("_logger"),
                            SyntaxFactory.IdentifierName(logMethod)),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(arguments.Value)));
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that converts Console.WriteLine to Debug.WriteLine.
    /// </summary>
    private class ConsoleToDebugRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression.ToString() == "Console" &&
                memberAccess.Name.Identifier.ValueText == "WriteLine")
            {
                return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Debug"),
                        SyntaxFactory.IdentifierName("WriteLine")),
                    node.ArgumentList);
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that converts Console.WriteLine to Trace.WriteLine.
    /// </summary>
    private class ConsoleToTraceRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression.ToString() == "Console" &&
                memberAccess.Name.Identifier.ValueText == "WriteLine")
            {
                return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Trace"),
                        SyntaxFactory.IdentifierName("WriteLine")),
                    node.ArgumentList);
            }

            return base.VisitInvocationExpression(node);
        }
    }
}
