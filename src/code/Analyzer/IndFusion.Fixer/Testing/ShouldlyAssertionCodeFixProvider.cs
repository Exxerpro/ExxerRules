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
/// Code fix provider that replaces FluentAssertions with Shouldly assertions.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ShouldlyAssertionCodeFixProvider)), Shared]
public class ShouldlyAssertionCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.UseShouldly);

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
            RegisterShouldlyFixes(context, diagnostic, node);
        }
    }

    /// <summary>
    /// Registers code fix options based on the type of FluentAssertions that can be converted.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="node">The syntax node that can be converted.</param>
    private static void RegisterShouldlyFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
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
                case "Should":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "🔄 Convert FluentAssertions to Shouldly",
                            createChangedDocument: c => ConvertFluentAssertionsToShouldlyAsync(context.Document, invocationExpression, c),
                            equivalenceKey: "ConvertFluentAssertionsToShouldly"),
                        diagnostic);
                    break;

                case "Be":
                case "NotBe":
                case "BeNull":
                case "NotBeNull":
                case "BeEmpty":
                case "NotBeEmpty":
                case "Contain":
                case "NotContain":
                case "HaveCount":
                case "BeGreaterThan":
                case "BeLessThan":
                case "BeGreaterOrEqualTo":
                case "BeLessOrEqualTo":
                case "BeTrue":
                case "BeFalse":
                case "Throw":
                case "NotThrow":
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: $"🔄 Convert {methodName} to Shouldly",
                            createChangedDocument: c => ConvertSpecificAssertionToShouldlyAsync(context.Document, invocationExpression, methodName, c),
                            equivalenceKey: $"Convert{methodName}ToShouldly"),
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
        if (usingName == "FluentAssertions")
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "🔄 Replace FluentAssertions with Shouldly using",
                    createChangedDocument: c => ReplaceFluentAssertionsUsingAsync(context.Document, usingDirective, c),
                    equivalenceKey: "ReplaceFluentAssertionsUsing"),
                diagnostic);
        }
    }

    /// <summary>
    /// Converts FluentAssertions to Shouldly.
    /// </summary>
    private static async Task<Document> ConvertFluentAssertionsToShouldlyAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new FluentAssertionsToShouldlyRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts a specific FluentAssertions method to Shouldly.
    /// </summary>
    private static async Task<Document> ConvertSpecificAssertionToShouldlyAsync(Document document, InvocationExpressionSyntax invocationExpression, string methodName, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var shouldlyExpression = ConvertToShouldlyExpression(invocationExpression, methodName);
        editor.ReplaceNode(invocationExpression, shouldlyExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Replaces FluentAssertions using directive with Shouldly.
    /// </summary>
    private static async Task<Document> ReplaceFluentAssertionsUsingAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var newUsing = usingDirective.WithName(SyntaxFactory.ParseName("Shouldly"));
        editor.ReplaceNode(usingDirective, newUsing);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts a FluentAssertions expression to Shouldly.
    /// </summary>
    private static ExpressionSyntax ConvertToShouldlyExpression(InvocationExpressionSyntax invocationExpression, string methodName)
    {
        // Get the target expression (the object being asserted)
        var targetExpression = GetTargetExpression(invocationExpression);
        if (targetExpression == null)
        {
            return invocationExpression;
        }

        // Convert based on the method name
        switch (methodName)
        {
            case "Be":
                return CreateShouldlyBeExpression(targetExpression, invocationExpression.ArgumentList);
            case "NotBe":
                return CreateShouldlyNotBeExpression(targetExpression, invocationExpression.ArgumentList);
            case "BeNull":
                return CreateShouldlyBeNullExpression(targetExpression);
            case "NotBeNull":
                return CreateShouldlyNotBeNullExpression(targetExpression);
            case "BeEmpty":
                return CreateShouldlyBeEmptyExpression(targetExpression);
            case "NotBeEmpty":
                return CreateShouldlyNotBeEmptyExpression(targetExpression);
            case "Contain":
                return CreateShouldlyContainExpression(targetExpression, invocationExpression.ArgumentList);
            case "NotContain":
                return CreateShouldlyNotContainExpression(targetExpression, invocationExpression.ArgumentList);
            case "HaveCount":
                return CreateShouldlyHaveCountExpression(targetExpression, invocationExpression.ArgumentList);
            case "BeGreaterThan":
                return CreateShouldlyBeGreaterThanExpression(targetExpression, invocationExpression.ArgumentList);
            case "BeLessThan":
                return CreateShouldlyBeLessThanExpression(targetExpression, invocationExpression.ArgumentList);
            case "BeGreaterOrEqualTo":
                return CreateShouldlyBeGreaterOrEqualToExpression(targetExpression, invocationExpression.ArgumentList);
            case "BeLessOrEqualTo":
                return CreateShouldlyBeLessOrEqualToExpression(targetExpression, invocationExpression.ArgumentList);
            case "BeTrue":
                return CreateShouldlyBeTrueExpression(targetExpression);
            case "BeFalse":
                return CreateShouldlyBeFalseExpression(targetExpression);
            case "Throw":
                return CreateShouldlyThrowExpression(targetExpression, invocationExpression.ArgumentList);
            case "NotThrow":
                return CreateShouldlyNotThrowExpression(targetExpression);
            default:
                return invocationExpression;
        }
    }

    /// <summary>
    /// Gets the target expression from a FluentAssertions invocation.
    /// </summary>
    private static ExpressionSyntax? GetTargetExpression(InvocationExpressionSyntax invocationExpression)
    {
        if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.Expression;
        }
        return null;
    }

    /// <summary>
    /// Creates a Shouldly Be expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("Should")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly NotBe expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyNotBeExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldNot")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly BeNull expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeNullExpression(ExpressionSyntax target)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                target,
                SyntaxFactory.IdentifierName("ShouldBeNull")));
    }

    /// <summary>
    /// Creates a Shouldly NotBeNull expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyNotBeNullExpression(ExpressionSyntax target)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                target,
                SyntaxFactory.IdentifierName("ShouldNotBeNull")));
    }

    /// <summary>
    /// Creates a Shouldly BeEmpty expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeEmptyExpression(ExpressionSyntax target)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                target,
                SyntaxFactory.IdentifierName("ShouldBeEmpty")));
    }

    /// <summary>
    /// Creates a Shouldly NotBeEmpty expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyNotBeEmptyExpression(ExpressionSyntax target)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                target,
                SyntaxFactory.IdentifierName("ShouldNotBeEmpty")));
    }

    /// <summary>
    /// Creates a Shouldly Contain expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyContainExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldContain")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly NotContain expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyNotContainExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldNotContain")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly HaveCount expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyHaveCountExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldHaveSingleItem")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly BeGreaterThan expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeGreaterThanExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldBeGreaterThan")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly BeLessThan expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeLessThanExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldBeLessThan")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly BeGreaterOrEqualTo expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeGreaterOrEqualToExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldBeGreaterThanOrEqualTo")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly BeLessOrEqualTo expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeLessOrEqualToExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldBeLessThanOrEqualTo")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly BeTrue expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeTrueExpression(ExpressionSyntax target)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                target,
                SyntaxFactory.IdentifierName("ShouldBeTrue")));
    }

    /// <summary>
    /// Creates a Shouldly BeFalse expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyBeFalseExpression(ExpressionSyntax target)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                target,
                SyntaxFactory.IdentifierName("ShouldBeFalse")));
    }

    /// <summary>
    /// Creates a Shouldly Throw expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyThrowExpression(ExpressionSyntax target, ArgumentListSyntax? argumentList)
    {
        var argument = argumentList?.Arguments.FirstOrDefault()?.Expression;
        if (argument != null)
        {
            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    target,
                    SyntaxFactory.IdentifierName("ShouldThrow")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(argument))));
        }
        return target;
    }

    /// <summary>
    /// Creates a Shouldly NotThrow expression.
    /// </summary>
    private static ExpressionSyntax CreateShouldlyNotThrowExpression(ExpressionSyntax target)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                target,
                SyntaxFactory.IdentifierName("ShouldNotThrow")));
    }

    /// <summary>
    /// Rewriter that converts FluentAssertions to Shouldly.
    /// </summary>
    private class FluentAssertionsToShouldlyRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Should")
            {
                // Convert FluentAssertions.Should() to Shouldly
                var target = memberAccess.Expression;
                return SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        target,
                        SyntaxFactory.IdentifierName("Should")));
            }

            return base.VisitInvocationExpression(node);
        }
    }
}
