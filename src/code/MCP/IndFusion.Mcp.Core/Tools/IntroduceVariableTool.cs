using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Core.SyntaxRewriters;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Provides operations to introduce a local variable from a selected expression.
/// Supports solution-aware and single-file operation modes.
/// </summary>
[McpServerToolType]
public static class IntroduceVariableTool
{
    /// <summary>
    /// Introduces a new local variable from a selected expression.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file.</param>
    /// <param name="selectionRange">Range in format 'startLine:startColumn-endLine:endColumn'.</param>
    /// <param name="variableName">Name for the new variable.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A status message describing the outcome.</returns>
    [McpServerTool, Description("Introduce a new variable from selected expression (preferred for large C# file ExxerFactoring)")]
    public static async Task<string> IntroduceVariable(
        [Description("Absolute path to the solution file (.sln)")] string? solutionPath,
        [Description("Path to the C# file")] string filePath,
        [Description("Range in format 'startLine:startColumn-endLine:endColumn'")] string selectionRange,
        [Description("Name for the new variable")] string variableName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(solutionPath))
            {
                return await ExxerFactoringHelpers.RunWithSolutionOrFile(
                    solutionPath,
                    filePath,
                    doc => IntroduceVariableWithSolution(doc, selectionRange, variableName, cancellationToken),
                    path => IntroduceVariableSingleFile(path, selectionRange, variableName, cancellationToken));
            }

            return await IntroduceVariableSingleFile(filePath, selectionRange, variableName, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new McpException($"Error introducing variable: {ex.Message}", ex);
        }
    }

    private static async Task<string> IntroduceVariableWithSolution(Document document, string selectionRange, string variableName, CancellationToken cancellationToken)
    {
        var sourceText = await document.GetTextAsync(cancellationToken);
        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);

        if (!ExxerFactoringHelpers.TryParseRange(selectionRange, out var startLine, out var startColumn, out var endLine, out var endColumn))
            throw new McpException("Error: Invalid selection range format");

        if (!ExxerFactoringHelpers.ValidateRange(sourceText, startLine, startColumn, endLine, endColumn, out var error))
            throw new McpException(error);

        var startPosition = sourceText.Lines[startLine - 1].Start + startColumn - 1;
        var endPosition = sourceText.Lines[endLine - 1].Start + endColumn - 1;
        var span = TextSpan.FromBounds(startPosition, endPosition);

        var selectedExpression = syntaxRoot!.DescendantNodes()
            .OfType<ExpressionSyntax>()
            .Where(e => span.Contains(e.Span) || e.Span.Contains(span))
            .OrderBy(e => Math.Abs(e.Span.Length - span.Length))
            .ThenBy(e => e.Span.Length)
            .FirstOrDefault();
        var initializerExpression = selectedExpression;
        if (selectedExpression?.Parent is ParenthesizedExpressionSyntax paren && paren.Span.Contains(span))
            selectedExpression = paren;

        if (selectedExpression == null)
            throw new McpException("Error: Selected code is not a valid expression");

        // Get the semantic model to determine the type
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        var typeInfo = semanticModel!.GetTypeInfo(selectedExpression);
        var typeName = typeInfo.Type?.ToDisplayString() ?? "var";

        // Create the variable declaration
        var variableDeclaration = SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(typeName))
                .WithVariables(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(variableName)
                        .WithInitializer(SyntaxFactory.EqualsValueClause(initializerExpression!)))));

        var variableReference = SyntaxFactory.IdentifierName(variableName);

        var containingStatement = selectedExpression.Ancestors().OfType<StatementSyntax>().FirstOrDefault();
        var containingBlock = containingStatement?.Parent as BlockSyntax;
        var rewriter = new VariableIntroductionRewriter(
            selectedExpression,
            variableReference,
            variableDeclaration,
            containingStatement,
            containingBlock);
        var newRoot = rewriter.Visit(syntaxRoot);

        var formattedRoot = Formatter.Format(newRoot, document.Project.Solution.Workspace);
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
        editor.ReplaceNode(syntaxRoot, formattedRoot);
        var newDocument = editor.GetChangedDocument();
        var newText = await newDocument.GetTextAsync(cancellationToken);
        var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        await File.WriteAllTextAsync(document.FilePath!, newText.ToString(), encoding, cancellationToken);
        ExxerFactoringHelpers.UpdateSolutionCache(newDocument);

        return $"Successfully introduced variable '{variableName}' from {selectionRange} in {document.FilePath} (solution mode)";
    }

    private static async Task<string> IntroduceVariableSingleFile(string filePath, string selectionRange, string variableName, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
            throw new McpException($"Error: File {filePath} not found");

        var (sourceText, encoding) = await ExxerFactoringHelpers.ReadFileWithEncodingAsync(filePath);
        var model = await ExxerFactoringHelpers.GetOrCreateSemanticModelAsync(filePath);
        var newText = IntroduceVariableInSource(sourceText, selectionRange, variableName, model);
        await File.WriteAllTextAsync(filePath, newText, encoding, cancellationToken);
        ExxerFactoringHelpers.UpdateFileCaches(filePath, newText);
        return $"Successfully introduced variable '{variableName}' from {selectionRange} in {filePath} (single file mode)";
    }

    /// <summary>
    /// Introduces a new local variable into the provided source text from a selected range.
    /// </summary>
    /// <param name="sourceText">The C# source text.</param>
    /// <param name="selectionRange">Range in format 'startLine:startColumn-endLine:endColumn'.</param>
    /// <param name="variableName">Name for the new variable.</param>
    /// <param name="model">Optional semantic model used to resolve types.</param>
    /// <returns>The updated source text.</returns>
    public static string IntroduceVariableInSource(string sourceText, string selectionRange, string variableName, SemanticModel? model = null)
    {
        var syntaxTree = model?.SyntaxTree ?? CSharpSyntaxTree.ParseText(sourceText);
        var syntaxRoot = syntaxTree.GetRoot();
        var text = syntaxTree.GetText();
        var textLines = text.Lines;

        if (!ExxerFactoringHelpers.TryParseRange(selectionRange, out var startLine, out var startColumn, out var endLine, out var endColumn))
            throw new McpException("Error: Invalid selection range format");

        if (!ExxerFactoringHelpers.ValidateRange(text, startLine, startColumn, endLine, endColumn, out var error))
            throw new McpException(error);

        var startPosition = textLines[startLine - 1].Start + startColumn - 1;
        var endPosition = textLines[endLine - 1].Start + endColumn - 1;
        var span = TextSpan.FromBounds(startPosition, endPosition);

        var selectedExpression = syntaxRoot.DescendantNodes()
            .OfType<ExpressionSyntax>()
            .Where(e => span.Contains(e.Span) || e.Span.Contains(span))
            .OrderBy(e => Math.Abs(e.Span.Length - span.Length))
            .ThenBy(e => e.Span.Length)
            .FirstOrDefault();
        var initializerExpression = selectedExpression;
        if (selectedExpression?.Parent is ParenthesizedExpressionSyntax paren && paren.Span.Contains(span))
            selectedExpression = paren;

        if (selectedExpression == null)
            throw new McpException("Error: Selected code is not a valid expression");

        var typeName = "var";
        if (model != null)
        {
            var typeInfo = model.GetTypeInfo(initializerExpression ?? selectedExpression!);
            if (typeInfo.Type != null)
                typeName = typeInfo.Type.ToDisplayString();
        }

        var variableDeclaration = SyntaxFactory.LocalDeclarationStatement(
            SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(typeName))
                .WithVariables(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(variableName)
                        .WithInitializer(SyntaxFactory.EqualsValueClause(initializerExpression!)))));

        var variableReference = SyntaxFactory.IdentifierName(variableName);
        var containingStatement = selectedExpression.Ancestors().OfType<StatementSyntax>().FirstOrDefault();
        var containingBlock = containingStatement?.Parent as BlockSyntax;
        var rewriter = new VariableIntroductionRewriter(
            selectedExpression,
            variableReference,
            variableDeclaration,
            containingStatement,
            containingBlock);
        var newRoot = rewriter.Visit(syntaxRoot);

        var formattedRoot = Formatter.Format(newRoot, ExxerFactoringHelpers.SharedWorkspace);
        var result = formattedRoot.ToFullString();
        // Normalize line endings to Unix style for consistent test results
        return result.Replace("\r\n", "\n").Replace("\r", "\n");
    }

}
