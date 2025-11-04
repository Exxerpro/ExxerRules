using System.Collections.Immutable;
using System.Composition;
using IndFusion.Analyzer;
using IndFusion.Fixer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace IndFusion.Fixer.CodeFormatting;

/// <summary>
/// Code fix provider that provides formatting actions for detected formatting issues.
/// SRP: Responsible only for providing code fix actions for formatting inconsistencies.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeFormattingCodeFixProvider)), Shared]
public class CodeFormattingCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.CodeFormattingIssue);

    /// <inheritdoc/>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return;
        }

        // Find the diagnostic for formatting issues
        var diagnostic = context.Diagnostics.FirstOrDefault(d => d.Id == DiagnosticIds.CodeFormattingIssue);
        if (diagnostic == null)
        {
            return;
        }

        // Register formatting actions for the current document
        RegisterDocumentFormattingActions(context, diagnostic, context.Document.Name);
    }

    /// <summary>
    /// Registers formatting actions specific to the current document.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="documentName">The name of the document.</param>
    private static void RegisterDocumentFormattingActions(CodeFixContext context, Diagnostic diagnostic, string documentName)
    {
        // Format current file with full formatting
        var formatFileAction = CodeAction.Create(
            title: $"📄 Format Current File '{documentName}'",
            createChangedDocument: c => RoslynFormattingService.FormatDocumentAsync(context.Document, c),
            equivalenceKey: "FormatCurrentFile");

        context.RegisterCodeFix(formatFileAction, diagnostic);

        // Format current file with whitespace only
        var formatFileWhitespaceAction = CodeAction.Create(
            title: $"📝 Format Whitespace in '{documentName}'",
            createChangedDocument: c => RoslynFormattingService.FormatWhitespaceAsync(context.Document, c),
            equivalenceKey: "FormatFileWhitespace");

        context.RegisterCodeFix(formatFileWhitespaceAction, diagnostic);

        // Format current file with .NET standards
        var formatFileDotNetAction = CodeAction.Create(
            title: $"🔧 Format with .NET Standards '{documentName}'",
            createChangedDocument: c => RoslynFormattingService.FormatDocumentAsync(
                context.Document,
                RoslynFormattingService.CreateDotNetFormattingOptions(),
                c),
            equivalenceKey: "FormatFileDotNet");

        context.RegisterCodeFix(formatFileDotNetAction, diagnostic);
    }
}
