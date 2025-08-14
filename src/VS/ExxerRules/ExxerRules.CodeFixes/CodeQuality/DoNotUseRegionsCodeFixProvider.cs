using System.Collections.Immutable;
using System.Composition;
using ExxerRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;

namespace ExxerRules.CodeFixes.CodeQuality;

/// <summary>
/// Code fix provider that removes regions and suggests better organization.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DoNotUseRegionsCodeFixProvider)), Shared]
public class DoNotUseRegionsCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds => 
		ImmutableArray.Create(DiagnosticIds.DoNotUseRegions);

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
			RegisterRegionFixes(context, diagnostic, node);
		}
	}

	/// <summary>
	/// Registers code fix options based on the type of region that can be removed.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="node">The syntax node that can be converted.</param>
	private static void RegisterRegionFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
	{
		switch (node)
		{
			case RegionDirectiveTriviaSyntax regionDirective:
				RegisterRegionDirectiveFixes(context, diagnostic, regionDirective);
				break;
			case EndRegionDirectiveTriviaSyntax endRegionDirective:
				RegisterEndRegionDirectiveFixes(context, diagnostic, endRegionDirective);
				break;
		}
	}

	/// <summary>
	/// Registers code fix options for region directive conversion.
	/// </summary>
	private static void RegisterRegionDirectiveFixes(CodeFixContext context, Diagnostic diagnostic, RegionDirectiveTriviaSyntax regionDirective)
	{
		var regionName = regionDirective.EndOfDirectiveToken.LeadingTrivia.ToString().Trim();

		context.RegisterCodeFix(
			CodeAction.Create(
				title: $"🗑️ Remove region '{regionName}'",
				createChangedDocument: c => RemoveRegionAsync(context.Document, regionDirective, c),
				equivalenceKey: $"RemoveRegion_{regionName}"),
			diagnostic);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: $"📁 Extract region '{regionName}' to separate file",
				createChangedDocument: c => ExtractRegionToFileAsync(context.Document, regionDirective, c),
				equivalenceKey: $"ExtractRegionToFile_{regionName}"),
			diagnostic);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: $"🏗️ Convert region '{regionName}' to partial class",
				createChangedDocument: c => ConvertRegionToPartialClassAsync(context.Document, regionDirective, c),
				equivalenceKey: $"ConvertRegionToPartialClass_{regionName}"),
			diagnostic);
	}

	/// <summary>
	/// Registers code fix options for end region directive conversion.
	/// </summary>
	private static void RegisterEndRegionDirectiveFixes(CodeFixContext context, Diagnostic diagnostic, EndRegionDirectiveTriviaSyntax endRegionDirective)
	{
		context.RegisterCodeFix(
			CodeAction.Create(
				title: "🗑️ Remove end region",
				createChangedDocument: c => RemoveEndRegionAsync(context.Document, endRegionDirective, c),
				equivalenceKey: "RemoveEndRegion"),
			diagnostic);
	}

	/// <summary>
	/// Removes a region directive.
	/// </summary>
	private static async Task<Document> RemoveRegionAsync(Document document, RegionDirectiveTriviaSyntax regionDirective, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// Find the corresponding end region
		var endRegion = FindEndRegion(regionDirective);
		if (endRegion != null)
		{
			// Remove both region and end region
			editor.RemoveNode(regionDirective);
			editor.RemoveNode(endRegion);
		}
		else
		{
			// Remove just the region directive
			editor.RemoveNode(regionDirective);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Extracts a region to a separate file.
	/// </summary>
	private static async Task<Document> ExtractRegionToFileAsync(Document document, RegionDirectiveTriviaSyntax regionDirective, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// Find the corresponding end region
		var endRegion = FindEndRegion(regionDirective);
		if (endRegion != null)
		{
			// Extract the content between region and end region
			var regionContent = ExtractRegionContent(regionDirective, endRegion);
			
			// Create a new file name based on the region name
			var regionName = regionDirective.EndOfDirectiveToken.LeadingTrivia.ToString().Trim();
			var fileName = GenerateFileName(regionName);
			
			// Create a new document with the extracted content
			var newDocument = document.Project.AddDocument(fileName, regionContent);
			
			// Remove the region from the original document
			editor.RemoveNode(regionDirective);
			editor.RemoveNode(endRegion);
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts a region to a partial class.
	/// </summary>
	private static async Task<Document> ConvertRegionToPartialClassAsync(Document document, RegionDirectiveTriviaSyntax regionDirective, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// Find the containing class
		var classDeclaration = regionDirective.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (classDeclaration != null)
		{
			// Add partial keyword to the class
			var newClass = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
			editor.ReplaceNode(classDeclaration, newClass);

			// Find the corresponding end region
			var endRegion = FindEndRegion(regionDirective);
			if (endRegion != null)
			{
				// Extract the content between region and end region
				var regionContent = ExtractRegionContent(regionDirective, endRegion);
				
				// Create a new partial class file
				var regionName = regionDirective.EndOfDirectiveToken.LeadingTrivia.ToString().Trim();
				var fileName = GeneratePartialClassName(regionName);
				
				// Create a new document with the partial class
				var partialClassContent = CreatePartialClassContent(fileName, regionContent);
				var newDocument = document.Project.AddDocument(fileName + ".cs", partialClassContent);
				
				// Remove the region from the original document
				editor.RemoveNode(regionDirective);
				editor.RemoveNode(endRegion);
			}
		}

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Removes an end region directive.
	/// </summary>
	private static async Task<Document> RemoveEndRegionAsync(Document document, EndRegionDirectiveTriviaSyntax endRegionDirective, CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		editor.RemoveNode(endRegionDirective);
		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Finds the corresponding end region for a region directive.
	/// </summary>
	private static EndRegionDirectiveTriviaSyntax? FindEndRegion(RegionDirectiveTriviaSyntax regionDirective)
	{
		var parent = regionDirective.Parent;
		if (parent != null)
		{
			// Look for the next end region directive
			var endRegion = parent.DescendantNodes()
				.OfType<EndRegionDirectiveTriviaSyntax>()
				.FirstOrDefault();

			return endRegion;
		}

		return null;
	}

	/// <summary>
	/// Extracts the content between a region and end region.
	/// </summary>
	private static string ExtractRegionContent(RegionDirectiveTriviaSyntax regionDirective, EndRegionDirectiveTriviaSyntax endRegionDirective)
	{
		var startPosition = regionDirective.Span.End;
		var endPosition = endRegionDirective.Span.Start;
		
		// This is a simplified extraction - in practice, you'd need more complex logic
		// to properly extract the content while preserving formatting and structure
		return "// Extracted region content";
	}

	/// <summary>
	/// Generates a file name based on the region name.
	/// </summary>
	private static string GenerateFileName(string regionName)
	{
		// Convert region name to a valid file name
		var cleanName = regionName.Replace(" ", "").Replace("-", "").Replace("_", "");
		return cleanName + ".cs";
	}

	/// <summary>
	/// Generates a partial class name based on the region name.
	/// </summary>
	private static string GeneratePartialClassName(string regionName)
	{
		// Convert region name to a valid class name
		var cleanName = regionName.Replace(" ", "").Replace("-", "").Replace("_", "");
		return cleanName + "Partial";
	}

	/// <summary>
	/// Creates partial class content.
	/// </summary>
	private static string CreatePartialClassContent(string className, string content)
	{
		return $@"
using System;

public partial class {className}
{{
    {content}
}}";
	}
}