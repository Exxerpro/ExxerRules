using System.Collections.Immutable;
using System.Composition;
using IndFusion.Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace IndFusion.Fixer.Testing;

/// <summary>
/// Code fix provider that replaces Moq with NSubstitute mocking.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NSubstituteMockingCodeFixProvider)), Shared]
public class NSubstituteMockingCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.UseNSubstitute);

    /// <inheritdoc/>
    /// <summary>
    /// This method is called to register code fixes for a diagnostic.
    /// It is an asynchronous method, and the code fixes should be registered
    /// using the context.RegisterCodeFix method.
    /// </summary>
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
            RegisterNSubstituteFixes(context, diagnostic, node);
        }
    }

    /// <summary>
    /// Registers code fix options based on the type of Moq syntax that can be converted.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="node">The syntax node that can be converted.</param>
    private static void RegisterNSubstituteFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
    {
        switch (node)
        {
            case InvocationExpressionSyntax invocationExpression:
                RegisterInvocationFixes(context, diagnostic, invocationExpression);
                break;
            case UsingDirectiveSyntax usingDirective:
                RegisterUsingFixes(context, diagnostic, usingDirective);
                break;
            case ObjectCreationExpressionSyntax objectCreation:
                RegisterObjectCreationFixes(context, diagnostic, objectCreation);
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
                case "Setup":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Convert Moq Setup to NSubstitute",
                            createChangedDocument: c => ConvertMoqSetupToNSubstituteAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ConvertMoqSetupToNSubstitute"),
                        diagnostic);
                    break;

                case "Returns":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Convert Moq Returns to NSubstitute",
                            createChangedDocument: c => ConvertMoqReturnsToNSubstituteAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ConvertMoqReturnsToNSubstitute"),
                        diagnostic);
                    break;

                case "Throws":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Convert Moq Throws to NSubstitute",
                            createChangedDocument: c => ConvertMoqThrowsToNSubstituteAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ConvertMoqThrowsToNSubstitute"),
                        diagnostic);
                    break;

                case "Verify":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Convert Moq Verify to NSubstitute",
                            createChangedDocument: c => ConvertMoqVerifyToNSubstituteAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ConvertMoqVerifyToNSubstitute"),
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
        if (usingName == "Moq")
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "🔄 Replace Moq with NSubstitute using",
                    createChangedDocument: c => ReplaceMoqUsingAsync(context.Document, usingDirective, c),
                    equivalenceKey: "ReplaceMoqUsing"),
                diagnostic);
        }
    }

    /// <summary>
    /// Registers code fix options for object creation conversion.
    /// </summary>
    private static void RegisterObjectCreationFixes(CodeFixContext context, Diagnostic diagnostic, ObjectCreationExpressionSyntax objectCreation)
    {
        var typeName = objectCreation.Type.ToString();
        if (typeName.Contains("Mock<"))
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "🔄 Convert Moq Mock to NSubstitute",
                    createChangedDocument: c => ConvertMoqMockToNSubstituteAsync(context.Document, objectCreation, c),
                    equivalenceKey: "ConvertMoqMockToNSubstitute"),
                diagnostic);
        }
    }

    /// <summary>
    /// Converts Moq Setup to NSubstitute.
    /// </summary>
    private static async Task<Document> ConvertMoqSetupToNSubstituteAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new MoqSetupToNSubstituteRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts Moq Returns to NSubstitute.
    /// </summary>
    private static async Task<Document> ConvertMoqReturnsToNSubstituteAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new MoqReturnsToNSubstituteRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts Moq Throws to NSubstitute.
    /// </summary>
    private static async Task<Document> ConvertMoqThrowsToNSubstituteAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new MoqThrowsToNSubstituteRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts Moq Verify to NSubstitute.
    /// </summary>
    private static async Task<Document> ConvertMoqVerifyToNSubstituteAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new MoqVerifyToNSubstituteRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Replaces Moq using directive with NSubstitute.
    /// </summary>
    private static async Task<Document> ReplaceMoqUsingAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var newUsing = usingDirective.WithName(SyntaxFactory.ParseName("NSubstitute"));
        editor.ReplaceNode(usingDirective, newUsing);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts Moq Mock to NSubstitute.
    /// </summary>
    private static async Task<Document> ConvertMoqMockToNSubstituteAsync(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new MoqMockToNSubstituteRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(objectCreation);
        editor.ReplaceNode(objectCreation, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Rewriter that converts Moq Setup to NSubstitute.
    /// </summary>
    private class MoqSetupToNSubstituteRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Setup")
            {
                // Convert Moq Setup to NSubstitute
                // Moq: mock.Setup(x => x.Method()).Returns(value)
                // NSubstitute: mock.Method().Returns(value)
                var mockExpression = memberAccess.Expression;
                var setupArgument = node.ArgumentList?.Arguments.FirstOrDefault()?.Expression;

                if (setupArgument is LambdaExpressionSyntax lambda)
                {
                    if (lambda.Body is InvocationExpressionSyntax methodCall)
                    {
                        return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                mockExpression,
                                (methodCall.Expression as SimpleNameSyntax) ?? SyntaxFactory.IdentifierName(methodCall.Expression.ToString())));
                    }
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that converts Moq Returns to NSubstitute.
    /// </summary>
    private class MoqReturnsToNSubstituteRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Returns")
            {
                // Convert Moq Returns to NSubstitute
                // Moq: mock.Setup(x => x.Method()).Returns(value)
                // NSubstitute: mock.Method().Returns(value)
                var argument = node.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
                if (argument != null)
                {
                    return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            memberAccess.Expression,
                            SyntaxFactory.IdentifierName("Returns")),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(argument))));
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that converts Moq Throws to NSubstitute.
    /// </summary>
    private class MoqThrowsToNSubstituteRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Throws")
            {
                // Convert Moq Throws to NSubstitute
                // Moq: mock.Setup(x => x.Method()).Throws(exception)
                // NSubstitute: mock.Method().Throws(exception)
                var argument = node.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
                if (argument != null)
                {
                    return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            memberAccess.Expression,
                            SyntaxFactory.IdentifierName("Throws")),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(argument))));
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that converts Moq Verify to NSubstitute.
    /// </summary>
    private class MoqVerifyToNSubstituteRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Verify")
            {
                // Convert Moq Verify to NSubstitute
                // Moq: mock.Verify(x => x.Method(), Times.Once())
                // NSubstitute: mock.Received(1).Method()
                var verifyArgument = node.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
                var timesArgument = node.ArgumentList?.Arguments.Skip(1).FirstOrDefault()?.Expression;

                if (verifyArgument is LambdaExpressionSyntax lambda)
                {
                    if (lambda.Body is InvocationExpressionSyntax methodCall)
                    {
                        var receivedCall = SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                memberAccess.Expression,
                                SyntaxFactory.IdentifierName("Received")),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))))));

                        return SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                receivedCall,
                                (methodCall.Expression as SimpleNameSyntax) ?? SyntaxFactory.IdentifierName(methodCall.Expression.ToString())));
                    }
                }
            }

            return base.VisitInvocationExpression(node);
        }
    }

    /// <summary>
    /// Rewriter that converts Moq Mock to NSubstitute.
    /// </summary>
    private class MoqMockToNSubstituteRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (node.Type.ToString().Contains("Mock<"))
            {
                // Convert Moq Mock to NSubstitute
                // Moq: new Mock<IInterface>()
                // NSubstitute: Substitute.For<IInterface>()
                var typeArgument = ExtractTypeArgument(node.Type);
                if (typeArgument != null)
                {
                    return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Substitute"),
                            SyntaxFactory.GenericName(
                                SyntaxFactory.Identifier("For"),
                                SyntaxFactory.TypeArgumentList(
                                    SyntaxFactory.SingletonSeparatedList(typeArgument)))));
                }
            }

            return base.VisitObjectCreationExpression(node);
        }

        /// <summary>
        /// Extracts the type argument from a Mock&lt;T&gt; type.
        /// </summary>
        private static TypeSyntax? ExtractTypeArgument(TypeSyntax type)
        {
            if (type is GenericNameSyntax genericName &&
                genericName.TypeArgumentList.Arguments.Count > 0)
            {
                return genericName.TypeArgumentList.Arguments[0];
            }
            return null;
        }
    }
}
