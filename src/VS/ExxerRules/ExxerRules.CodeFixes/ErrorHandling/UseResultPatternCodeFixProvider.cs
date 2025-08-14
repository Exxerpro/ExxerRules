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

namespace ExxerRules.CodeFixes.ErrorHandling;

/// <summary>
/// Code fix provider that converts exception throwing methods to Result&lt;T&gt; pattern.
/// </summary>
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

namespace ExxerRules.CodeFixes.ErrorHandling;

/// <summary>
/// Code fix provider that converts exception throwing methods to Result&lt;T&gt; pattern.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseResultPatternCodeFixProvider)), Shared]
public class UseResultPatternCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.UseResultPattern);

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

		var diagnostic = context.Diagnostics.First();
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		var node = root.FindNode(diagnosticSpan);
		if (node == null)
		{
			return;
		}

		// Find the containing method
		var methodDeclaration = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
		if (methodDeclaration != null)
		{
			// Register multiple code fix options
			RegisterCodeFixOptions(context, diagnostic, methodDeclaration);
		}
	}

	/// <summary>
	/// Registers multiple code fix options for different conversion strategies.
	/// </summary>
	/// <param name="context">The code fix context.</param>
	/// <param name="diagnostic">The diagnostic to fix.</param>
	/// <param name="methodDeclaration">The method declaration to convert.</param>
	private static void RegisterCodeFixOptions(CodeFixContext context, Diagnostic diagnostic, MethodDeclarationSyntax methodDeclaration)
	{
		// Basic conversion
		context.RegisterCodeFix(
			CodeAction.Create(
				title: "🔄 Convert to Result<T> pattern",
				createChangedDocument: c => ConvertMethodToResultPatternAsync(context.Document, methodDeclaration, c),
				equivalenceKey: "ConvertToResultPattern"),
			diagnostic);

		// Conversion with detailed error messages
		context.RegisterCodeFix(
			CodeAction.Create(
				title: "📝 Convert with detailed error messages",
				createChangedDocument: c => ConvertMethodToResultPatternWithDetailedErrorsAsync(context.Document, methodDeclaration, c),
				equivalenceKey: "ConvertToResultPatternWithDetailedErrors"),
			diagnostic);

		// Conversion with async support
		if (IsAsyncMethod(methodDeclaration))
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "⚡ Convert async method to Result<T>",
					createChangedDocument: c => ConvertAsyncMethodToResultPatternAsync(context.Document, methodDeclaration, c),
					equivalenceKey: "ConvertAsyncMethodToResultPattern"),
				diagnostic);
		}
	}

	/// <summary>
	/// Converts a method to use the Result&lt;T&gt; pattern with basic error handling.
	/// </summary>
	/// <param name="document">The document containing the method.</param>
	/// <param name="methodDeclaration">The method declaration to convert.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The modified document.</returns>
	private static async Task<Document> ConvertMethodToResultPatternAsync(
		Document document,
		MethodDeclarationSyntax methodDeclaration,
		CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// Determine the new return type
		var newReturnType = DetermineResultReturnType(methodDeclaration.ReturnType);

		// Create new method with Result return type
		var newMethod = methodDeclaration.WithReturnType(newReturnType);

		// Convert throw statements to Result.WithFailure with enhanced error extraction
		var rewriter = new EnhancedThrowToResultRewriter();
		newMethod = (MethodDeclarationSyntax)rewriter.Visit(newMethod);

		// Replace the method
		editor.ReplaceNode(methodDeclaration, newMethod);

		// Add using statement if needed
		await AddResultUsingStatementAsync(editor, cancellationToken).ConfigureAwait(false);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts a method to use the Result&lt;T&gt; pattern with detailed error messages.
	/// </summary>
	/// <param name="document">The document containing the method.</param>
	/// <param name="methodDeclaration">The method declaration to convert.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The modified document.</returns>
	private static async Task<Document> ConvertMethodToResultPatternWithDetailedErrorsAsync(
		Document document,
		MethodDeclarationSyntax methodDeclaration,
		CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// Determine the new return type
		var newReturnType = DetermineResultReturnType(methodDeclaration.ReturnType);

		// Create new method with Result return type
		var newMethod = methodDeclaration.WithReturnType(newReturnType);

		// Convert throw statements to Result.WithFailure with detailed error extraction
		var rewriter = new DetailedErrorThrowToResultRewriter();
		newMethod = (MethodDeclarationSyntax)rewriter.Visit(newMethod);

		// Replace the method
		editor.ReplaceNode(methodDeclaration, newMethod);

		// Add using statement if needed
		await AddResultUsingStatementAsync(editor, cancellationToken).ConfigureAwait(false);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Converts an async method to use the Result&lt;T&gt; pattern with proper async support.
	/// </summary>
	/// <param name="document">The document containing the method.</param>
	/// <param name="methodDeclaration">The method declaration to convert.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The modified document.</returns>
	private static async Task<Document> ConvertAsyncMethodToResultPatternAsync(
		Document document,
		MethodDeclarationSyntax methodDeclaration,
		CancellationToken cancellationToken)
	{
		var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

		// Determine the new return type for async methods
		var newReturnType = DetermineAsyncResultReturnType(methodDeclaration.ReturnType);

		// Create new method with Result return type
		var newMethod = methodDeclaration.WithReturnType(newReturnType);

		// Convert throw statements to Result.WithFailure with async support
		var rewriter = new AsyncThrowToResultRewriter();
		newMethod = (MethodDeclarationSyntax)rewriter.Visit(newMethod);

		// Replace the method
		editor.ReplaceNode(methodDeclaration, newMethod);

		// Add using statement if needed
		await AddResultUsingStatementAsync(editor, cancellationToken).ConfigureAwait(false);

		return editor.GetChangedDocument();
	}

	/// <summary>
	/// Determines the appropriate Result return type for a given return type.
	/// </summary>
	/// <param name="currentReturnType">The current return type.</param>
	/// <returns>The Result return type.</returns>
	private static TypeSyntax DetermineResultReturnType(TypeSyntax currentReturnType)
	{
		if (IsTaskType(currentReturnType))
		{
			// Extract the inner type from Task<T> or just use Task
			var innerType = GetTaskInnerType(currentReturnType);
			if (innerType != null)
			{
				return SyntaxFactory.ParseTypeName($"Task<Result<{innerType}>>");
			}
			else
			{
				return SyntaxFactory.ParseTypeName("Task<Result>");
			}
		}
		else if (currentReturnType.ToString() == "void")
		{
			return SyntaxFactory.ParseTypeName("Result");
		}
		else
		{
			return SyntaxFactory.ParseTypeName($"Result<{currentReturnType}>");
		}
	}

	/// <summary>
	/// Determines the appropriate Result return type for async methods.
	/// </summary>
	/// <param name="currentReturnType">The current return type.</param>
	/// <returns>The async Result return type.</returns>
	private static TypeSyntax DetermineAsyncResultReturnType(TypeSyntax currentReturnType)
	{
		if (IsTaskType(currentReturnType))
		{
			// Extract the inner type from Task<T> or just use Task
			var innerType = GetTaskInnerType(currentReturnType);
			if (innerType != null)
			{
				return SyntaxFactory.ParseTypeName($"Task<Result<{innerType}>>");
			}
			else
			{
				return SyntaxFactory.ParseTypeName("Task<Result>");
			}
		}
		else if (IsValueTaskType(currentReturnType))
		{
			// Extract the inner type from ValueTask<T> or just use ValueTask
			var innerType = GetValueTaskInnerType(currentReturnType);
			if (innerType != null)
			{
				return SyntaxFactory.ParseTypeName($"ValueTask<Result<{innerType}>>");
			}
			else
			{
				return SyntaxFactory.ParseTypeName("ValueTask<Result>");
			}
		}
		else
		{
			// For non-async methods, wrap in Task
			return SyntaxFactory.ParseTypeName($"Task<Result<{currentReturnType}>>");
		}
	}

	/// <summary>
	/// Adds the Result using statement if it doesn't already exist.
	/// </summary>
	/// <param name="editor">The document editor.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	private static async Task AddResultUsingStatementAsync(DocumentEditor editor, CancellationToken cancellationToken)
	{
		var root = await editor.GetChangedDocument().GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
		if (root is CompilationUnitSyntax compilationUnit)
		{
			var hasResultUsing = compilationUnit.Usings.Any(u => u.Name?.ToString().Contains("Result") == true);
			if (!hasResultUsing)
			{
				var firstNode = compilationUnit.Usings.FirstOrDefault() as SyntaxNode ?? compilationUnit.Members.FirstOrDefault();
				if (firstNode != null)
				{
					editor.InsertBefore(firstNode,
						SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ExxerRules.Analyzers.Operations")));
				}
			}
		}
	}

	/// <summary>
	/// Checks if a method is async.
	/// </summary>
	/// <param name="methodDeclaration">The method declaration.</param>
	/// <returns>True if the method is async.</returns>
	private static bool IsAsyncMethod(MethodDeclarationSyntax methodDeclaration)
	{
		return methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AsyncKeyword));
	}

	/// <summary>
	/// Checks if a type is a Task type.
	/// </summary>
	/// <param name="type">The type to check.</param>
	/// <returns>True if the type is a Task.</returns>
	private static bool IsTaskType(TypeSyntax type)
	{
		var typeString = type.ToString();
		return typeString.StartsWith("Task<") || typeString == "Task";
	}

	/// <summary>
	/// Checks if a type is a ValueTask type.
	/// </summary>
	/// <param name="type">The type to check.</param>
	/// <returns>True if the type is a ValueTask.</returns>
	private static bool IsValueTaskType(TypeSyntax type)
	{
		var typeString = type.ToString();
		return typeString.StartsWith("ValueTask<") || typeString == "ValueTask";
	}

	/// <summary>
	/// Gets the inner type from a Task&lt;T&gt;.
	/// </summary>
	/// <param name="type">The Task type.</param>
	/// <returns>The inner type if available.</returns>
	private static string? GetTaskInnerType(TypeSyntax type)
	{
		if (type is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Count > 0)
		{
			return genericName.TypeArgumentList.Arguments[0].ToString();
		}
		return null;
	}

	/// <summary>
	/// Gets the inner type from a ValueTask&lt;T&gt;.
	/// </summary>
	/// <param name="type">The ValueTask type.</param>
	/// <returns>The inner type if available.</returns>
	private static string? GetValueTaskInnerType(TypeSyntax type)
	{
		if (type is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Count > 0)
		{
			return genericName.TypeArgumentList.Arguments[0].ToString();
		}
		return null;
	}

	/// <summary>
	/// Enhanced rewriter that converts throw statements to Result.WithFailure with better error extraction.
	/// </summary>
	private class EnhancedThrowToResultRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitThrowStatement(ThrowStatementSyntax node)
		{
			if (node.Expression == null)
			{
				// Rethrow - keep as is for now
				return base.VisitThrowStatement(node);
			}

			// Extract error message from exception with enhanced logic
			var errorMessage = ExtractEnhancedErrorMessage(node.Expression);

			// Create Result.WithFailure statement
			var resultStatement = SyntaxFactory.ParseStatement($"return Result.WithFailure({errorMessage});")
				.WithLeadingTrivia(node.GetLeadingTrivia())
				.WithTrailingTrivia(node.GetTrailingTrivia())
				.WithAdditionalAnnotations(Formatter.Annotation);

			return resultStatement;
		}

		public override SyntaxNode? VisitThrowExpression(ThrowExpressionSyntax node)
		{
			// Extract error message from exception with enhanced logic
			var errorMessage = ExtractEnhancedErrorMessage(node.Expression);

			// Create Result.WithFailure expression
			var resultExpression = SyntaxFactory.ParseExpression($"Result.WithFailure({errorMessage})")
				.WithAdditionalAnnotations(Formatter.Annotation);

			return resultExpression;
		}

		/// <summary>
		/// Extracts error messages with enhanced logic for better error categorization.
		/// </summary>
		/// <param name="expression">The exception expression.</param>
		/// <returns>The extracted error message.</returns>
		private static string ExtractEnhancedErrorMessage(ExpressionSyntax expression)
		{
			// Try to extract the message from exception constructor
			if (expression is ObjectCreationExpressionSyntax objectCreation &&
				objectCreation.ArgumentList?.Arguments.Count > 0)
			{
				var firstArg = objectCreation.ArgumentList.Arguments[0].Expression;
				return firstArg.ToString();
			}

			// Handle different exception types with specific messages
			if (expression is IdentifierNameSyntax identifierName)
			{
				var exceptionName = identifierName.Identifier.ValueText;
				return GetSpecificErrorMessageForExceptionType(exceptionName);
			}

			// Default message
			return "\"Operation failed\"";
		}

		/// <summary>
		/// Gets specific error messages for common exception types.
		/// </summary>
		/// <param name="exceptionType">The exception type name.</param>
		/// <returns>A specific error message.</returns>
		private static string GetSpecificErrorMessageForExceptionType(string exceptionType)
		{
			return exceptionType.ToLower() switch
			{
				"argumentnullexception" => "\"Required parameter is null\"",
				"argumentexception" => "\"Invalid argument provided\"",
				"invalidoperationexception" => "\"Operation is not valid in current state\"",
				"notsupportedexception" => "\"Operation is not supported\"",
				"notimplementedexception" => "\"Operation is not implemented\"",
				"timeoutexception" => "\"Operation timed out\"",
				"unauthorizedaccessexception" => "\"Access denied\"",
				"filenotfoundexception" => "\"File not found\"",
				"directorynotfoundexception" => "\"Directory not found\"",
				"outofmemoryexception" => "\"Insufficient memory\"",
				"stackoverflowexception" => "\"Stack overflow occurred\"",
				_ => $"\"{exceptionType} occurred\""
			};
		}
	}

	/// <summary>
	/// Rewriter that converts throw statements to Result.WithFailure with detailed error messages.
	/// </summary>
	private class DetailedErrorThrowToResultRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitThrowStatement(ThrowStatementSyntax node)
		{
			if (node.Expression == null)
			{
				return base.VisitThrowStatement(node);
			}

			var errorMessage = ExtractDetailedErrorMessage(node.Expression);

			var resultStatement = SyntaxFactory.ParseStatement($"return Result.WithFailure({errorMessage});")
				.WithLeadingTrivia(node.GetLeadingTrivia())
				.WithTrailingTrivia(node.GetTrailingTrivia())
				.WithAdditionalAnnotations(Formatter.Annotation);

			return resultStatement;
		}

		public override SyntaxNode? VisitThrowExpression(ThrowExpressionSyntax node)
		{
			var errorMessage = ExtractDetailedErrorMessage(node.Expression);

			var resultExpression = SyntaxFactory.ParseExpression($"Result.WithFailure({errorMessage})")
				.WithAdditionalAnnotations(Formatter.Annotation);

			return resultExpression;
		}

		/// <summary>
		/// Extracts detailed error messages with context information.
		/// </summary>
		/// <param name="expression">The exception expression.</param>
		/// <returns>The detailed error message.</returns>
		private static string ExtractDetailedErrorMessage(ExpressionSyntax expression)
		{
			if (expression is ObjectCreationExpressionSyntax objectCreation &&
				objectCreation.ArgumentList?.Arguments.Count > 0)
			{
				var firstArg = objectCreation.ArgumentList.Arguments[0].Expression;
				return firstArg.ToString();
			}

			if (expression is IdentifierNameSyntax identifierName)
			{
				var exceptionName = identifierName.Identifier.ValueText;
				return GetDetailedErrorMessageForExceptionType(exceptionName);
			}

			return "\"Operation failed with unknown error\"";
		}

		/// <summary>
		/// Gets detailed error messages for exception types with context.
		/// </summary>
		/// <param name="exceptionType">The exception type name.</param>
		/// <returns>A detailed error message.</returns>
		private static string GetDetailedErrorMessageForExceptionType(string exceptionType)
		{
			return exceptionType.ToLower() switch
			{
				"argumentnullexception" => "\"Required parameter is null. Please provide a valid value.\"",
				"argumentexception" => "\"Invalid argument provided. Please check the parameter values.\"",
				"invalidoperationexception" => "\"Operation is not valid in the current state. Please ensure proper initialization.\"",
				"notsupportedexception" => "\"Operation is not supported in this context.\"",
				"notimplementedexception" => "\"Operation is not implemented yet.\"",
				"timeoutexception" => "\"Operation timed out. Please try again or increase timeout value.\"",
				"unauthorizedaccessexception" => "\"Access denied. Please check permissions.\"",
				"filenotfoundexception" => "\"File not found. Please verify the file path.\"",
				"directorynotfoundexception" => "\"Directory not found. Please verify the directory path.\"",
				"outofmemoryexception" => "\"Insufficient memory available for this operation.\"",
				"stackoverflowexception" => "\"Stack overflow occurred. Check for infinite recursion.\"",
				_ => $"\"{exceptionType} occurred during operation execution.\""
			};
		}
	}

	/// <summary>
	/// Rewriter that converts throw statements to Result.WithFailure with async support.
	/// </summary>
	private class AsyncThrowToResultRewriter : CSharpSyntaxRewriter
	{
		public override SyntaxNode? VisitThrowStatement(ThrowStatementSyntax node)
		{
			if (node.Expression == null)
			{
				return base.VisitThrowStatement(node);
			}

			var errorMessage = ExtractEnhancedErrorMessage(node.Expression);

			var resultStatement = SyntaxFactory.ParseStatement($"return Result.WithFailure({errorMessage});")
				.WithLeadingTrivia(node.GetLeadingTrivia())
				.WithTrailingTrivia(node.GetTrailingTrivia())
				.WithAdditionalAnnotations(Formatter.Annotation);

			return resultStatement;
		}

		public override SyntaxNode? VisitThrowExpression(ThrowExpressionSyntax node)
		{
			var errorMessage = ExtractEnhancedErrorMessage(node.Expression);

			var resultExpression = SyntaxFactory.ParseExpression($"Result.WithFailure({errorMessage})")
				.WithAdditionalAnnotations(Formatter.Annotation);

			return resultExpression;
		}

		/// <summary>
		/// Extracts error messages with async context consideration.
		/// </summary>
		/// <param name="expression">The exception expression.</param>
		/// <returns>The extracted error message.</returns>
		private static string ExtractEnhancedErrorMessage(ExpressionSyntax expression)
		{
			if (expression is ObjectCreationExpressionSyntax objectCreation &&
				objectCreation.ArgumentList?.Arguments.Count > 0)
			{
				var firstArg = objectCreation.ArgumentList.Arguments[0].Expression;
				return firstArg.ToString();
			}

			if (expression is IdentifierNameSyntax identifierName)
			{
				var exceptionName = identifierName.Identifier.ValueText;
				return GetAsyncSpecificErrorMessageForExceptionType(exceptionName);
			}

			return "\"Async operation failed\"";
		}

		/// <summary>
		/// Gets async-specific error messages for exception types.
		/// </summary>
		/// <param name="exceptionType">The exception type name.</param>
		/// <returns>An async-specific error message.</returns>
		private static string GetAsyncSpecificErrorMessageForExceptionType(string exceptionType)
		{
			return exceptionType.ToLower() switch
			{
				"operationcanceledexception" => "\"Async operation was canceled\"",
				"timeoutexception" => "\"Async operation timed out\"",
				"taskcanceledexception" => "\"Async task was canceled\"",
				"argumentnullexception" => "\"Required parameter is null for async operation\"",
				"invalidoperationexception" => "\"Async operation is not valid in current state\"",
				_ => $"\"Async operation failed: {exceptionType}\""
			};
		}
	}
}
