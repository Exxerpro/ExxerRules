using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.IO;
using ExxerRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace ExxerRules.CodeFixes.CodeFormatting;

/// <summary>
/// Code fix provider that executes 'dotnet format --severity info --verbosity d' on the current project.
/// SRP: Responsible only for providing code fix actions that trigger project formatting.
/// </summary>
using System.Collections.Immutable;
using System.Composition;
using ExxerRules.Analyzers;
using ExxerRules.CodeFixes.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace ExxerRules.CodeFixes.CodeFormatting;

/// <summary>
/// Code fix provider that provides project-wide formatting actions.
/// SRP: Responsible only for providing code fix actions that trigger project formatting.
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

		// Find the diagnostic for project formatting
		var diagnostic = context.Diagnostics.FirstOrDefault(d => d.Id == DiagnosticIds.ProjectFormatting);
		if (diagnostic == null)
		{
			return;
		}

		// Register different formatting actions
		RegisterFormattingActions(context, diagnostic, context.Document.Project.Name);
	}

	/// <summary>
	/// Registers various formatting code actions.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="projectName">The name of the project.</param>
	private static void RegisterFormattingActions(CodeFixContext context, Diagnostic diagnostic, string projectName)
	{
		// Standard project formatting
		var formatAction = CodeAction.Create(
			title: $"🔧 Format Project '{projectName}'",
			createChangedDocument: c => FormatProjectAsync(context.Document, c),
			equivalenceKey: "FormatProject");

		context.RegisterCodeFix(formatAction, diagnostic);

		// Format with whitespace fixes
		var formatWhitespaceAction = CodeAction.Create(
			title: $"📝 Format Whitespace Only '{projectName}'",
			createChangedDocument: c => FormatProjectWhitespaceAsync(context.Document, c),
			equivalenceKey: "FormatWhitespace");

		context.RegisterCodeFix(formatWhitespaceAction, diagnostic);

		// Format with .NET standards
		var formatDotNetAction = CodeAction.Create(
			title: $"🎨 Format with .NET Standards '{projectName}'",
			createChangedDocument: c => FormatProjectWithDotNetStandardsAsync(context.Document, c),
			equivalenceKey: "FormatDotNet");

		context.RegisterCodeFix(formatDotNetAction, diagnostic);

		// Format entire solution
		var formatSolutionAction = CodeAction.Create(
			title: $"🏗️ Format Entire Solution",
			createChangedDocument: c => FormatSolutionAsync(context.Document, c),
			equivalenceKey: "FormatSolution");

		context.RegisterCodeFix(formatSolutionAction, diagnostic);
	}

	/// <summary>
	/// Formats the entire project using Roslyn formatting.
	/// </summary>
	/// <param name="document">The current document.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The formatted document.</returns>
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
	/// Formats the project with whitespace-only formatting.
	/// </summary>
	/// <param name="document">The current document.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The formatted document.</returns>
	private static async Task<Document> FormatProjectWhitespaceAsync(Document document, CancellationToken cancellationToken)
	{
		try
		{
			var solution = document.Project.Solution;
			var projectId = document.Project.Id;
			
			// Format each document in the project with whitespace-only formatting
			var newSolution = solution;
			var project = solution.GetProject(projectId);
			
			if (project != null)
			{
				foreach (var documentId in project.DocumentIds)
				{
					var doc = newSolution.GetDocument(documentId);
					if (doc != null)
					{
						var formattedDoc = await RoslynFormattingService.FormatWhitespaceAsync(doc, cancellationToken).ConfigureAwait(false);
						newSolution = formattedDoc.Project.Solution;
					}
				}
			}
			
			var formattedDocument = newSolution.GetDocument(document.Id);
			return formattedDocument ?? document;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error formatting project whitespace: {ex.Message}");
			return document;
		}
	}

	/// <summary>
	/// Formats the project with .NET formatting standards.
	/// </summary>
	/// <param name="document">The current document.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The formatted document.</returns>
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
	/// <param name="document">The current document.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The formatted document.</returns>
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
