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

namespace IndFusion.CodeFixes.Async;

/// <summary>
/// Code fix provider that adds CancellationToken parameters to async methods.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CancellationTokenCodeFixProvider)), Shared]
public class CancellationTokenCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken);

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

            // Find the containing method
            var methodDeclaration = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (methodDeclaration != null && IsAsyncMethod(methodDeclaration))
            {
                RegisterCancellationTokenFixes(context, diagnostic, methodDeclaration);
            }
        }
    }

    /// <summary>
    /// Registers code fix options for adding CancellationToken parameters.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="methodDeclaration">The method declaration.</param>
    private static void RegisterCancellationTokenFixes(CodeFixContext context, Diagnostic diagnostic, MethodDeclarationSyntax methodDeclaration)
    {
        // Add CancellationToken as last parameter
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"⏱️ Add CancellationToken parameter to '{methodDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddCancellationTokenParameterAsync(context.Document, methodDeclaration, c),
                equivalenceKey: $"AddCancellationToken_{methodDeclaration.Identifier.ValueText}"),
            diagnostic);

        // Add CancellationToken with default value
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"⏱️ Add CancellationToken with default value to '{methodDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddCancellationTokenWithDefaultAsync(context.Document, methodDeclaration, c),
                equivalenceKey: $"AddCancellationTokenWithDefault_{methodDeclaration.Identifier.ValueText}"),
            diagnostic);

        // Add CancellationToken and update method calls
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"🔄 Add CancellationToken and update calls to '{methodDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddCancellationTokenAndUpdateCallsAsync(context.Document, methodDeclaration, c),
                equivalenceKey: $"AddCancellationTokenAndUpdateCalls_{methodDeclaration.Identifier.ValueText}"),
            diagnostic);
    }

    /// <summary>
    /// Adds a CancellationToken parameter to the method.
    /// </summary>
    private static async Task<Document> AddCancellationTokenParameterAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var newParameterList = AddCancellationTokenToParameterList(methodDeclaration.ParameterList);
        var newMethod = methodDeclaration.WithParameterList(newParameterList);

        editor.ReplaceNode(methodDeclaration, newMethod);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds a CancellationToken parameter with default value to the method.
    /// </summary>
    private static async Task<Document> AddCancellationTokenWithDefaultAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var newParameterList = AddCancellationTokenWithDefaultToParameterList(methodDeclaration.ParameterList);
        var newMethod = methodDeclaration.WithParameterList(newParameterList);

        editor.ReplaceNode(methodDeclaration, newMethod);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds a CancellationToken parameter and updates method calls.
    /// </summary>
    private static async Task<Document> AddCancellationTokenAndUpdateCallsAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        // Add CancellationToken parameter
        var newParameterList = AddCancellationTokenToParameterList(methodDeclaration.ParameterList);
        var newMethod = methodDeclaration.WithParameterList(newParameterList);

        editor.ReplaceNode(methodDeclaration, newMethod);

        // Update method calls in the document
        var updatedDocument = await UpdateMethodCallsAsync(editor.GetChangedDocument(), methodDeclaration.Identifier.ValueText, cancellationToken).ConfigureAwait(false);

        return updatedDocument;
    }

    /// <summary>
    /// Adds a CancellationToken parameter to the parameter list.
    /// </summary>
    private static ParameterListSyntax AddCancellationTokenToParameterList(ParameterListSyntax parameterList)
    {
        var cancellationTokenParameter = SyntaxFactory.Parameter(
            SyntaxFactory.Identifier("cancellationToken"))
            .WithType(SyntaxFactory.ParseTypeName("CancellationToken"));

        var newParameters = parameterList.Parameters.Add(cancellationTokenParameter);
        return parameterList.WithParameters(newParameters);
    }

    /// <summary>
    /// Adds a CancellationToken parameter with default value to the parameter list.
    /// </summary>
    private static ParameterListSyntax AddCancellationTokenWithDefaultToParameterList(ParameterListSyntax parameterList)
    {
        var cancellationTokenParameter = SyntaxFactory.Parameter(
            SyntaxFactory.Identifier("cancellationToken"))
            .WithType(SyntaxFactory.ParseTypeName("CancellationToken"))
            .WithDefault(SyntaxFactory.EqualsValueClause(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("CancellationToken"),
                    SyntaxFactory.IdentifierName("None"))));

        var newParameters = parameterList.Parameters.Add(cancellationTokenParameter);
        return parameterList.WithParameters(newParameters);
    }

    /// <summary>
    /// Updates method calls to include CancellationToken parameter.
    /// </summary>
    private static async Task<Document> UpdateMethodCallsAsync(Document document, string methodName, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return document;
        }

        var rewriter = new MethodCallRewriter(methodName);
        var newRoot = rewriter.Visit(root);

        return document.WithSyntaxRoot(newRoot);
    }

    /// <summary>
    /// Checks if a method is async.
    /// </summary>
    private static bool IsAsyncMethod(MethodDeclarationSyntax methodDeclaration)
    {
        return methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AsyncKeyword));
    }

    /// <summary>
    /// Rewriter that updates method calls to include CancellationToken parameter.
    /// </summary>
    private class MethodCallRewriter : CSharpSyntaxRewriter
    {
        private readonly string _methodName;

        public MethodCallRewriter(string methodName)
        {
            _methodName = methodName;
        }

        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // Check if this is a call to the method we're updating
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == _methodName)
            {
                // Add CancellationToken.None to the argument list
                var cancellationTokenArgument = SyntaxFactory.Argument(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("CancellationToken"),
                        SyntaxFactory.IdentifierName("None")));

                var newArguments = node.ArgumentList?.Arguments.Add(cancellationTokenArgument) ??
                    SyntaxFactory.SeparatedList<ArgumentSyntax>().Add(cancellationTokenArgument);

                var newArgumentList = node.ArgumentList?.WithArguments(newArguments) ??
                    SyntaxFactory.ArgumentList(newArguments);

                return node.WithArgumentList(newArgumentList);
            }

            return base.VisitInvocationExpression(node);
        }
    }
}
