using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Host;

namespace IndFusion.CodeFixes.Common;

/// <summary>
/// Provides Roslyn-based formatting services to replace shell command execution.
/// </summary>
public static class RoslynFormattingService
{
    /// <summary>
    /// Formats a single document using Roslyn's built-in formatting capabilities.
    /// </summary>
    /// <param name="document">The document to format.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The formatted document.</returns>
    public static async Task<Document> FormatDocumentAsync(Document document, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the syntax root
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null)
            {
                return document;
            }

            // Apply formatting with default options
            var formattedRoot = Formatter.Format(root, document.Project.Solution.Workspace, cancellationToken: cancellationToken);

            // Return the document with formatted syntax
            return document.WithSyntaxRoot(formattedRoot);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting document: {ex.Message}");
            return document;
        }
    }

    /// <summary>
    /// Formats a document with specific formatting options.
    /// </summary>
    /// <param name="document">The document to format.</param>
    /// <param name="formattingOptions">The formatting options to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The formatted document.</returns>
    public static async Task<Document> FormatDocumentAsync(Document document, OptionSet formattingOptions, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the syntax root
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null)
            {
                return document;
            }

            // Apply formatting with custom options
            var formattedRoot = Formatter.Format(root, document.Project.Solution.Workspace, formattingOptions, cancellationToken: cancellationToken);

            // Return the document with formatted syntax
            return document.WithSyntaxRoot(formattedRoot);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting document with custom options: {ex.Message}");
            return document;
        }
    }

    /// <summary>
    /// Formats only whitespace in a document.
    /// </summary>
    /// <param name="document">The document to format.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document with formatted whitespace.</returns>
    public static async Task<Document> FormatWhitespaceAsync(Document document, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the syntax root
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null)
            {
                return document;
            }

            // Create formatting options that only affect whitespace
            var workspace = document.Project.Solution.Workspace;
            var options = document.Project.Solution.Workspace.Options;

            // Apply whitespace-only formatting
            var formattedRoot = Formatter.Format(root, workspace, options, cancellationToken: cancellationToken);

            return document.WithSyntaxRoot(formattedRoot);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting whitespace: {ex.Message}");
            return document;
        }
    }

    /// <summary>
    /// Formats a project by formatting all documents in the project.
    /// </summary>
    /// <param name="solution">The solution containing the project.</param>
    /// <param name="projectId">The ID of the project to format.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The solution with formatted documents.</returns>
    public static async Task<Solution> FormatProjectAsync(Solution solution, ProjectId projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var project = solution.GetProject(projectId);
            if (project == null)
            {
                return solution;
            }

            var newSolution = solution;

            // Format each document in the project
            foreach (var documentId in project.DocumentIds)
            {
                var document = newSolution.GetDocument(documentId);
                if (document == null)
                {
                    continue;
                }

                var formattedDocument = await FormatDocumentAsync(document, cancellationToken).ConfigureAwait(false);
                newSolution = formattedDocument.Project.Solution;
            }

            return newSolution;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting project: {ex.Message}");
            return solution;
        }
    }

    /// <summary>
    /// Formats a solution by formatting all documents in all projects.
    /// </summary>
    /// <param name="solution">The solution to format.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The formatted solution.</returns>
    public static async Task<Solution> FormatSolutionAsync(Solution solution, CancellationToken cancellationToken = default)
    {
        try
        {
            var newSolution = solution;

            // Format each project in the solution
            foreach (var projectId in solution.ProjectIds)
            {
                newSolution = await FormatProjectAsync(newSolution, projectId, cancellationToken).ConfigureAwait(false);
            }

            return newSolution;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error formatting solution: {ex.Message}");
            return solution;
        }
    }

    /// <summary>
    /// Creates default C# formatting options.
    /// </summary>
    /// <param name="workspace">The workspace to get options from.</param>
    /// <returns>Default formatting options for C#.</returns>
    public static OptionSet CreateDefaultFormattingOptions(Workspace workspace)
    {
        var options = workspace.Options;
        // Set common C# formatting options
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInLambdaExpressionBody, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInAnonymousMethods, false);
        // options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInAnonymousObjectInitializers, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInControlBlocks, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInTypes, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, false);
        // options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInObjectInitializers, false);
        // options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInCollectionInitializers, false);
        // options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInArrayInitializers, false);
        return options;
    }

    /// <summary>
    /// Creates .NET formatting options for consistent code style.
    /// </summary>
    /// <param name="workspace">The workspace to get options from.</param>
    /// <returns>OptionSet with .NET formatting rules.</returns>
    public static OptionSet CreateDotNetFormattingOptions(Workspace workspace)
    {
        var options = workspace.Options;
        // Set .NET formatting standards
        options = options.WithChangedOption(CSharpFormattingOptions.IndentBraces, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInTypes, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInObjectCollectionArrayInitializers, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInLambdaExpressionBody, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInAnonymousTypes, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInControlBlocks, false);
        options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInAnonymousMethods, false);
        // options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInObjectInitializers, false);
        // options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInCollectionInitializers, false);
        // options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInArrayInitializers, false);
        return options;
    }

    /// <summary>
    /// Creates default C# formatting options.
    /// </summary>
    /// <returns>Default formatting options for C#.</returns>
    public static OptionSet CreateDefaultFormattingOptions()
    {
        var workspace = new AdhocWorkspace();
        return CreateDefaultFormattingOptions(workspace);
    }

    /// <summary>
    /// Creates .NET formatting options for consistent code style.
    /// </summary>
    /// <returns>OptionSet with .NET formatting rules.</returns>
    public static OptionSet CreateDotNetFormattingOptions()
    {
        var workspace = new AdhocWorkspace();
        return CreateDotNetFormattingOptions(workspace);
    }
}

