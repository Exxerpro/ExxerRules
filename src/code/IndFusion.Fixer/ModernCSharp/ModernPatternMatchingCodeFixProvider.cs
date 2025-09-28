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

namespace IndFusion.CodeFixes.ModernCSharp;

/// <summary>
/// Code fix provider that converts traditional pattern matching to modern C# pattern matching.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ModernPatternMatchingCodeFixProvider)), Shared]
public class ModernPatternMatchingCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.UseModernPatternMatching);

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
            RegisterModernPatternMatchingFixes(context, diagnostic, node);
        }
    }

    /// <summary>
    /// Registers code fix options based on the type of pattern matching that can be modernized.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="node">The syntax node that can be modernized.</param>
    private static void RegisterModernPatternMatchingFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
    {
        switch (node)
        {
            case IfStatementSyntax ifStatement:
                RegisterIfStatementPatternMatchingFix(context, diagnostic, ifStatement);
                break;
            case SwitchStatementSyntax switchStatement:
                RegisterSwitchStatementPatternMatchingFix(context, diagnostic, switchStatement);
                break;
            case IsPatternExpressionSyntax isPatternExpression:
                RegisterIsPatternExpressionFix(context, diagnostic, isPatternExpression);
                break;
            case SwitchExpressionSyntax switchExpression:
                RegisterSwitchExpressionFix(context, diagnostic, switchExpression);
                break;
        }
    }

    /// <summary>
    /// Registers code fix for converting if statements to modern pattern matching.
    /// </summary>
    private static void RegisterIfStatementPatternMatchingFix(CodeFixContext context, Diagnostic diagnostic, IfStatementSyntax ifStatement)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "🔄 Convert to modern pattern matching",
                createChangedDocument: c => ConvertIfStatementToPatternMatchingAsync(context.Document, ifStatement, c),
                equivalenceKey: "ConvertIfStatementToPatternMatching"),
            diagnostic);

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "⚡ Convert to switch expression",
                createChangedDocument: c => ConvertIfStatementToSwitchExpressionAsync(context.Document, ifStatement, c),
                equivalenceKey: "ConvertIfStatementToSwitchExpression"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for converting switch statements to modern pattern matching.
    /// </summary>
    private static void RegisterSwitchStatementPatternMatchingFix(CodeFixContext context, Diagnostic diagnostic, SwitchStatementSyntax switchStatement)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "🔄 Convert to modern switch statement",
                createChangedDocument: c => ConvertSwitchStatementToModernAsync(context.Document, switchStatement, c),
                equivalenceKey: "ConvertSwitchStatementToModern"),
            diagnostic);

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "⚡ Convert to switch expression",
                createChangedDocument: c => ConvertSwitchStatementToExpressionAsync(context.Document, switchStatement, c),
                equivalenceKey: "ConvertSwitchStatementToExpression"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for improving is pattern expressions.
    /// </summary>
    private static void RegisterIsPatternExpressionFix(CodeFixContext context, Diagnostic diagnostic, IsPatternExpressionSyntax isPatternExpression)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "🔄 Improve pattern matching expression",
                createChangedDocument: c => ImproveIsPatternExpressionAsync(context.Document, isPatternExpression, c),
                equivalenceKey: "ImproveIsPatternExpression"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for improving switch expressions.
    /// </summary>
    private static void RegisterSwitchExpressionFix(CodeFixContext context, Diagnostic diagnostic, SwitchExpressionSyntax switchExpression)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "🔄 Improve switch expression",
                createChangedDocument: c => ImproveSwitchExpressionAsync(context.Document, switchExpression, c),
                equivalenceKey: "ImproveSwitchExpression"),
            diagnostic);
    }

    /// <summary>
    /// Converts an if statement to modern pattern matching.
    /// </summary>
    private static async Task<Document> ConvertIfStatementToPatternMatchingAsync(Document document, IfStatementSyntax ifStatement, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new IfStatementPatternMatchingRewriter();
        var newIfStatement = (IfStatementSyntax)rewriter.Visit(ifStatement);

        editor.ReplaceNode(ifStatement, newIfStatement);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts an if statement to a switch expression.
    /// </summary>
    private static async Task<Document> ConvertIfStatementToSwitchExpressionAsync(Document document, IfStatementSyntax ifStatement, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var switchExpression = ConvertIfStatementToSwitchExpression(ifStatement);
        if (switchExpression != null)
        {
            editor.ReplaceNode(ifStatement, switchExpression);
        }

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts a switch statement to modern pattern matching.
    /// </summary>
    private static async Task<Document> ConvertSwitchStatementToModernAsync(Document document, SwitchStatementSyntax switchStatement, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new SwitchStatementModernRewriter();
        var newSwitchStatement = (SwitchStatementSyntax)rewriter.Visit(switchStatement);

        editor.ReplaceNode(switchStatement, newSwitchStatement);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts a switch statement to a switch expression.
    /// </summary>
    private static async Task<Document> ConvertSwitchStatementToExpressionAsync(Document document, SwitchStatementSyntax switchStatement, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var switchExpression = ConvertSwitchStatementToSwitchExpression(switchStatement);
        if (switchExpression != null)
        {
            editor.ReplaceNode(switchStatement, switchExpression);
        }

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Improves an is pattern expression.
    /// </summary>
    private static async Task<Document> ImproveIsPatternExpressionAsync(Document document, IsPatternExpressionSyntax isPatternExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var improvedExpression = ImproveIsPatternExpression(isPatternExpression);
        editor.ReplaceNode(isPatternExpression, improvedExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Improves a switch expression.
    /// </summary>
    private static async Task<Document> ImproveSwitchExpressionAsync(Document document, SwitchExpressionSyntax switchExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new SwitchExpressionImprover();
        var improvedExpression = (SwitchExpressionSyntax)rewriter.Visit(switchExpression);

        editor.ReplaceNode(switchExpression, improvedExpression);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Converts an if statement to a switch expression.
    /// </summary>
    private static ExpressionSyntax? ConvertIfStatementToSwitchExpression(IfStatementSyntax ifStatement)
    {
        // This is a simplified conversion - in practice, you'd need more complex logic
        // to handle various if-else patterns
        if (ifStatement.Condition is IsPatternExpressionSyntax isPattern)
        {
            var arms = new List<SwitchExpressionArmSyntax>
            {
                SyntaxFactory.SwitchExpressionArm(
                    SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)),
                    isPattern.Expression)
            };

            return SyntaxFactory.SwitchExpression(
                isPattern.Expression,
                SyntaxFactory.SeparatedList(arms));
        }

        return null;
    }

    /// <summary>
    /// Converts a switch statement to a switch expression.
    /// </summary>
    private static ExpressionSyntax? ConvertSwitchStatementToSwitchExpression(SwitchStatementSyntax switchStatement)
    {
        var arms = new List<SwitchExpressionArmSyntax>();

        foreach (var section in switchStatement.Sections)
        {
            foreach (var label in section.Labels)
            {
                if (section.Statements.Count == 1 && section.Statements[0] is ReturnStatementSyntax returnStatement && returnStatement.Expression != null)
                {
                    var pattern = ConvertSwitchLabelToPattern(label);
                    if (pattern != null)
                    {
                        // returnStatement.Expression null-checked above; '!' ensures flow in netstandard2.0
                        arms.Add(SyntaxFactory.SwitchExpressionArm(pattern, returnStatement.Expression!));
                    }
                }
            }
        }

        if (arms.Count > 0)
        {
            return SyntaxFactory.SwitchExpression(
                switchStatement.Expression,
                SyntaxFactory.SeparatedList(arms));
        }

        return null;
    }

    /// <summary>
    /// Converts a switch label to a pattern.
    /// </summary>
    private static PatternSyntax? ConvertSwitchLabelToPattern(SwitchLabelSyntax label)
    {
        return label switch
        {
            CasePatternSwitchLabelSyntax casePattern => casePattern.Pattern,
            CaseSwitchLabelSyntax caseSwitch => SyntaxFactory.ConstantPattern(caseSwitch.Value),
            DefaultSwitchLabelSyntax => SyntaxFactory.DiscardPattern(),
            _ => null
        };
    }

    /// <summary>
    /// Improves an is pattern expression.
    /// </summary>
    private static ExpressionSyntax ImproveIsPatternExpression(IsPatternExpressionSyntax isPatternExpression)
    {
        // Add declaration patterns where appropriate
        if (isPatternExpression.Pattern is TypePatternSyntax typePattern)
        {
            var declarationPattern = SyntaxFactory.DeclarationPattern(
                typePattern.Type,
                SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("value")));

            return isPatternExpression.WithPattern(declarationPattern);
        }

        return isPatternExpression;
    }

    /// <summary>
    /// Rewriter that converts if statements to modern pattern matching.
    /// </summary>
    private class IfStatementPatternMatchingRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitIfStatement(IfStatementSyntax node)
        {
            // Convert traditional is expressions to pattern matching
            if (node.Condition is BinaryExpressionSyntax binaryExpression &&
                binaryExpression.OperatorToken.IsKind(SyntaxKind.IsKeyword) &&
                binaryExpression.Right is TypeSyntax typeSyntax)
            {
                var isPatternExpression = SyntaxFactory.IsPatternExpression(
                    binaryExpression.Left,
                    SyntaxFactory.TypePattern(typeSyntax));

                var newCondition = SyntaxFactory.ParenthesizedExpression(isPatternExpression);
                return node.WithCondition(newCondition);
            }

            return base.VisitIfStatement(node);
        }
    }

    /// <summary>
    /// Rewriter that converts switch statements to modern pattern matching.
    /// </summary>
    private class SwitchStatementModernRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitSwitchStatement(SwitchStatementSyntax node)
        {
            // Convert traditional case labels to pattern matching
            var newSections = new List<SwitchSectionSyntax>();

            foreach (var section in node.Sections)
            {
                var newLabels = new List<SwitchLabelSyntax>();

                foreach (var label in section.Labels)
                {
                    if (label is CaseSwitchLabelSyntax caseSwitch)
                    {
                        // Convert to constant pattern
                        var constantPattern = SyntaxFactory.ConstantPattern(caseSwitch.Value);
                        var whenClause = (WhenClauseSyntax?)null; // No when clause for simple constant pattern
                        var colonToken = SyntaxFactory.Token(SyntaxKind.ColonToken);
                        var newLabel = SyntaxFactory.CasePatternSwitchLabel(constantPattern, whenClause, colonToken);
                        newLabels.Add(newLabel);
                    }
                    else
                    {
                        newLabels.Add(label);
                    }
                }

                var newSection = section.WithLabels(SyntaxFactory.List(newLabels));
                newSections.Add(newSection);
            }

            return node.WithSections(SyntaxFactory.List(newSections));
        }
    }

    /// <summary>
    /// Rewriter that improves switch expressions.
    /// </summary>
    private class SwitchExpressionImprover : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            // Add discard patterns for completeness
            var arms = node.Arms.ToList();
            var hasDiscard = arms.Any(arm => arm.Pattern is DiscardPatternSyntax);

            if (!hasDiscard && arms.Count > 0)
            {
                // Add a discard arm at the end for completeness
                var discardArm = SyntaxFactory.SwitchExpressionArm(
                    SyntaxFactory.DiscardPattern(),
                    SyntaxFactory.ThrowExpression(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.IdentifierName("ArgumentException"))
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            SyntaxFactory.Literal("Unexpected value"))))))));

                arms.Add(discardArm);
                return node.WithArms(SyntaxFactory.SeparatedList(arms));
            }

            return base.VisitSwitchExpression(node);
        }
    }
}
