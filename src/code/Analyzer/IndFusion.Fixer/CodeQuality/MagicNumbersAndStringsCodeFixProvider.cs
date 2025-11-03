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

namespace IndFusion.CodeFixes.CodeQuality;

/// <summary>
/// Code fix provider that extracts magic numbers and strings to constants.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MagicNumbersAndStringsCodeFixProvider)), Shared]
public class MagicNumbersAndStringsCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.AvoidMagicNumbersAndStrings);

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
            RegisterMagicNumberAndStringFixes(context, diagnostic, node);
        }
    }

    /// <summary>
    /// Registers code fix options based on the type of magic number/string that can be extracted.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="node">The syntax node that can be converted.</param>
    private static void RegisterMagicNumberAndStringFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
    {
        switch (node)
        {
            case LiteralExpressionSyntax literalExpression:
                RegisterLiteralFixes(context, diagnostic, literalExpression);
                break;
            case InvocationExpressionSyntax invocationExpression:
                RegisterInvocationFixes(context, diagnostic, invocationExpression);
                break;
            case BinaryExpressionSyntax binaryExpression:
                RegisterBinaryExpressionFixes(context, diagnostic, binaryExpression);
                break;
        }
    }

    /// <summary>
    /// Registers code fix options for literal conversion.
    /// </summary>
    private static void RegisterLiteralFixes(CodeFixContext context, Diagnostic diagnostic, LiteralExpressionSyntax literalExpression)
    {
        var value = literalExpression.Token.ValueText;
        var constantName = GenerateConstantName(value, literalExpression.Kind());

        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Extract '{value}' to constant '{constantName}'",
                createChangedDocument: c => ExtractLiteralToConstantAsync(context.Document, literalExpression, constantName, c),
                equivalenceKey: $"ExtractLiteralToConstant_{constantName}"),
            diagnostic);

        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Extract '{value}' to local constant",
                createChangedDocument: c => ExtractLiteralToLocalConstantAsync(context.Document, literalExpression, constantName, c),
                equivalenceKey: $"ExtractLiteralToLocalConstant_{constantName}"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix options for invocation conversion.
    /// </summary>
    private static void RegisterInvocationFixes(CodeFixContext context, Diagnostic diagnostic, InvocationExpressionSyntax invocationExpression)
    {
        if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            var arguments = invocationExpression.ArgumentList?.Arguments;

            if (arguments != null && arguments.Value.Count > 0)
            {
                var firstArgument = arguments.Value[0].Expression;
                if (firstArgument is LiteralExpressionSyntax literal)
                {
                    var value = literal.Token.ValueText;
                    var constantName = GenerateConstantName(value, literal.Kind());

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: $"📝 Extract '{value}' to constant '{constantName}'",
                            createChangedDocument: c => ExtractInvocationLiteralToConstantAsync(context.Document, invocationExpression, literal, constantName, c),
                            equivalenceKey: $"ExtractInvocationLiteralToConstant_{constantName}"),
                        diagnostic);
                }
            }
        }
    }

    /// <summary>
    /// Registers code fix options for binary expression conversion.
    /// </summary>
    private static void RegisterBinaryExpressionFixes(CodeFixContext context, Diagnostic diagnostic, BinaryExpressionSyntax binaryExpression)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "📝 Extract magic numbers to constants",
                createChangedDocument: c => ExtractBinaryExpressionToConstantsAsync(context.Document, binaryExpression, c),
                equivalenceKey: "ExtractBinaryExpressionToConstants"),
            diagnostic);
    }

    /// <summary>
    /// Extracts a literal to a constant.
    /// </summary>
    private static async Task<Document> ExtractLiteralToConstantAsync(Document document, LiteralExpressionSyntax literalExpression, string constantName, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        // Find the containing class
        var classDeclaration = literalExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDeclaration != null)
        {
            // Create constant declaration
            var constantDeclaration = CreateConstantDeclaration(constantName, literalExpression);

            // Insert constant at the beginning of the class
            var newClass = classDeclaration.AddMembers(constantDeclaration);
            editor.ReplaceNode(classDeclaration, newClass);

            // Replace literal with constant reference
            var constantReference = SyntaxFactory.IdentifierName(constantName);
            editor.ReplaceNode(literalExpression, constantReference);
        }

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Extracts a literal to a local constant.
    /// </summary>
    private static async Task<Document> ExtractLiteralToLocalConstantAsync(Document document, LiteralExpressionSyntax literalExpression, string constantName, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        // Find the containing method
        var methodDeclaration = literalExpression.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (methodDeclaration != null)
        {
            // Create local constant declaration
            var localConstantDeclaration = CreateLocalConstantDeclaration(constantName, literalExpression);

            // Insert local constant at the beginning of the method
            var newMethod = methodDeclaration.AddBodyStatements(localConstantDeclaration);
            editor.ReplaceNode(methodDeclaration, newMethod);

            // Replace literal with constant reference
            var constantReference = SyntaxFactory.IdentifierName(constantName);
            editor.ReplaceNode(literalExpression, constantReference);
        }

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Extracts a literal from an invocation to a constant.
    /// </summary>
    private static async Task<Document> ExtractInvocationLiteralToConstantAsync(Document document, InvocationExpressionSyntax invocationExpression, LiteralExpressionSyntax literal, string constantName, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        // Find the containing class
        var classDeclaration = invocationExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDeclaration != null)
        {
            // Create constant declaration
            var constantDeclaration = CreateConstantDeclaration(constantName, literal);

            // Insert constant at the beginning of the class
            var newClass = classDeclaration.AddMembers(constantDeclaration);
            editor.ReplaceNode(classDeclaration, newClass);

            // Replace literal with constant reference
            var constantReference = SyntaxFactory.IdentifierName(constantName);
            editor.ReplaceNode(literal, constantReference);
        }

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Extracts magic numbers from a binary expression to constants.
    /// </summary>
    private static async Task<Document> ExtractBinaryExpressionToConstantsAsync(Document document, BinaryExpressionSyntax binaryExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new MagicNumberExtractorRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(binaryExpression);
        editor.ReplaceNode(binaryExpression, newExpression);

        // Add constants to the containing class
        var classDeclaration = binaryExpression.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDeclaration != null && rewriter.ExtractedConstants.Any())
        {
            var newClass = classDeclaration.AddMembers(rewriter.ExtractedConstants.ToArray());
            editor.ReplaceNode(classDeclaration, newClass);
        }

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Creates a constant declaration.
    /// </summary>
    private static FieldDeclarationSyntax CreateConstantDeclaration(string constantName, LiteralExpressionSyntax literal)
    {
        var type = GetTypeFromLiteral(literal);
        var value = literal.Token.ValueText;

        return SyntaxFactory.FieldDeclaration(
            SyntaxFactory.List<AttributeListSyntax>(),
            SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.ConstKeyword)),
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.ParseTypeName(type),
                SyntaxFactory.SeparatedList(new[]
                {
                    SyntaxFactory.VariableDeclarator(
                        SyntaxFactory.Identifier(constantName),
                        null,
                        SyntaxFactory.EqualsValueClause(literal))
                })));
    }

    /// <summary>
    /// Creates a local constant declaration.
    /// </summary>
    private static LocalDeclarationStatementSyntax CreateLocalConstantDeclaration(string constantName, LiteralExpressionSyntax literal)
    {
        var type = GetTypeFromLiteral(literal);

        return SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.ParseTypeName(type),
                SyntaxFactory.SeparatedList(new[]
                {
                    SyntaxFactory.VariableDeclarator(
                        SyntaxFactory.Identifier(constantName),
                        null,
                        SyntaxFactory.EqualsValueClause(literal))
                })));
    }

    /// <summary>
    /// Gets the type from a literal expression.
    /// </summary>
    private static string GetTypeFromLiteral(LiteralExpressionSyntax literal)
    {
        return literal.Kind() switch
        {
            SyntaxKind.StringLiteralExpression => "string",
            SyntaxKind.NumericLiteralExpression => "int",
            SyntaxKind.CharacterLiteralExpression => "char",
            SyntaxKind.TrueLiteralExpression or SyntaxKind.FalseLiteralExpression => "bool",
            _ => "object"
        };
    }

    /// <summary>
    /// Generates a constant name from a value.
    /// </summary>
    private static string GenerateConstantName(string value, SyntaxKind kind)
    {
        var prefix = kind switch
        {
            SyntaxKind.StringLiteralExpression => "DEFAULT_",
            SyntaxKind.NumericLiteralExpression => "MAX_",
            SyntaxKind.CharacterLiteralExpression => "CHAR_",
            SyntaxKind.TrueLiteralExpression or SyntaxKind.FalseLiteralExpression => "IS_",
            _ => "VALUE_"
        };

        // Convert value to uppercase and replace non-alphanumeric characters
        var cleanValue = value.Replace(" ", "_").Replace("-", "_").Replace(".", "_");
        return prefix + cleanValue.ToUpperInvariant();
    }

    /// <summary>
    /// Rewriter that extracts magic numbers to constants.
    /// </summary>
    private class MagicNumberExtractorRewriter : CSharpSyntaxRewriter
    {
        public List<FieldDeclarationSyntax> ExtractedConstants { get; } = new();

        public override SyntaxNode? VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.NumericLiteralExpression)
            {
                var value = node.Token.ValueText;
                var constantName = GenerateConstantName(value, node.Kind());
                var constantDeclaration = CreateConstantDeclaration(constantName, node);
                ExtractedConstants.Add(constantDeclaration);

                return SyntaxFactory.IdentifierName(constantName);
            }

            return base.VisitLiteralExpression(node);
        }

        private static FieldDeclarationSyntax CreateConstantDeclaration(string constantName, LiteralExpressionSyntax literal)
        {
            return SyntaxFactory.FieldDeclaration(
                SyntaxFactory.List<AttributeListSyntax>(),
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.ConstKeyword)),
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.ParseTypeName("int"),
                    SyntaxFactory.SeparatedList(new[]
                    {
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier(constantName),
                            null,
                            SyntaxFactory.EqualsValueClause(literal))
                    })));
        }

        private static string GenerateConstantName(string value, SyntaxKind kind)
        {
            var prefix = kind switch
            {
                SyntaxKind.StringLiteralExpression => "DEFAULT_",
                SyntaxKind.NumericLiteralExpression => "MAX_",
                SyntaxKind.CharacterLiteralExpression => "CHAR_",
                SyntaxKind.TrueLiteralExpression or SyntaxKind.FalseLiteralExpression => "IS_",
                _ => "VALUE_"
            };

            var cleanValue = value.Replace(" ", "_").Replace("-", "_").Replace(".", "_");
            return prefix + cleanValue.ToUpperInvariant();
        }
    }
}
