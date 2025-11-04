using System.Collections.Immutable;
using System.Composition;
using IndFusion.Analyzer;
using IndFusion.Fixer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace IndFusion.Fixer.CodeFormatting;

/// <summary>
/// Code fix provider that provides project-level formatting actions.
/// SRP: Responsible only for providing code fix actions for project-wide formatting issues.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ProjectFormattingCodeFixProvider)), Shared]
public class ProjectFormattingCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.ProjectFormatting);

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

        // Find the diagnostic for project formatting issues
        var diagnostic = context.Diagnostics.FirstOrDefault(d => d.Id == DiagnosticIds.ProjectFormatting);
        if (diagnostic == null)
        {
            return;
        }

        // Register project formatting actions
        RegisterProjectFormattingActions(context, diagnostic);
    }

    /// <summary>
    /// Registers project-level formatting actions.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    private static void RegisterProjectFormattingActions(CodeFixContext context, Diagnostic diagnostic)
    {
        // Format entire project
        var formatProjectAction = CodeAction.Create(
            title: "📁 Format Entire Project",
            createChangedDocument: c => FormatProjectAsync(context.Document, c),
            equivalenceKey: "FormatEntireProject");

        context.RegisterCodeFix(formatProjectAction, diagnostic);

        // Format project with .NET standards
        var formatProjectDotNetAction = CodeAction.Create(
            title: "🔧 Format Project with .NET Standards",
            createChangedDocument: c => FormatProjectWithDotNetStandardsAsync(context.Document, c),
            equivalenceKey: "FormatProjectDotNet");

        context.RegisterCodeFix(formatProjectDotNetAction, diagnostic);

        // Format solution
        var formatSolutionAction = CodeAction.Create(
            title: "🏗️ Format Entire Solution",
            createChangedDocument: c => FormatSolutionAsync(context.Document, c),
            equivalenceKey: "FormatEntireSolution");

        context.RegisterCodeFix(formatSolutionAction, diagnostic);
    }

    /// <summary>
    /// Formats the entire project using Roslyn formatting.
    /// </summary>
    private static async Task<Document> FormatProjectAsync(Document document, CancellationToken cancellationToken)
    {
        try
        {
            var solution = document.Project.Solution;
            var projectId = document.Project.Id;

            var formattedSolution = await RoslynFormattingService.FormatProjectAsync(solution, projectId, cancellationToken).ConfigureAwait(false);
            var formattedDocument = formattedSolution.GetDocument(document.Id);

            return formattedDocument ?? document;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting project: {ex.Message}");
            return document;
        }
    }

    /// <summary>
    /// Formats the project with .NET formatting standards.
    /// </summary>
    private static async Task<Document> FormatProjectWithDotNetStandardsAsync(Document document, CancellationToken cancellationToken)
    {
        try
        {
            var solution = document.Project.Solution;
            var projectId = document.Project.Id;
            var dotNetOptions = RoslynFormattingService.CreateDotNetFormattingOptions();

            // Format each document in the project with .NET standards
            var newSolution = solution;
            var project = solution.GetProject(projectId);

            if (project != null)
            {
                foreach (var documentId in project.DocumentIds)
                {
                    var doc = newSolution.GetDocument(documentId);
                    if (doc != null)
                    {
                        var formattedDoc = await RoslynFormattingService.FormatDocumentAsync(doc, dotNetOptions, cancellationToken).ConfigureAwait(false);
                        newSolution = formattedDoc.Project.Solution;
                    }
                }
            }

            var formattedDocument = newSolution.GetDocument(document.Id);
            return formattedDocument ?? document;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting project with .NET standards: {ex.Message}");
            return document;
        }
    }

    /// <summary>
    /// Formats the entire solution.
    /// </summary>
    private static async Task<Document> FormatSolutionAsync(Document document, CancellationToken cancellationToken)
    {
        try
        {
            var solution = document.Project.Solution;
            var formattedSolution = await RoslynFormattingService.FormatSolutionAsync(solution, cancellationToken).ConfigureAwait(false);
            var formattedDocument = formattedSolution.GetDocument(document.Id);

            return formattedDocument ?? document;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting solution: {ex.Message}");
            return document;
        }
    }
}
