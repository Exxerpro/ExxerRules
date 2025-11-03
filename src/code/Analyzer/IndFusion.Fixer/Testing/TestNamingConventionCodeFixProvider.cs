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
using System.Text.RegularExpressions;

namespace IndFusion.CodeFixes.Testing;

/// <summary>
/// Code fix provider that renames test methods to follow the Should_Action_When_Condition naming convention.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TestNamingConventionCodeFixProvider)), Shared]
public class TestNamingConventionCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.TestNamingConvention);

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

            // Find the containing test method
            var methodDeclaration = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (methodDeclaration != null && IsTestMethod(methodDeclaration))
            {
                RegisterTestNamingFixes(context, diagnostic, methodDeclaration);
            }
        }
    }

    /// <summary>
    /// Registers code fix options for test method naming.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="methodDeclaration">The test method declaration.</param>
    private static void RegisterTestNamingFixes(CodeFixContext context, Diagnostic diagnostic, MethodDeclarationSyntax methodDeclaration)
    {
        var currentName = methodDeclaration.Identifier.ValueText;
        var suggestions = GenerateTestNameSuggestions(currentName);

        foreach (var suggestion in suggestions)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"📝 Rename to '{suggestion}'",
                    createChangedDocument: c => RenameTestMethodAsync(context.Document, methodDeclaration, suggestion, c),
                    equivalenceKey: $"RenameTestMethod_{suggestion}"),
                diagnostic);
        }

        // Add a generic suggestion if no specific suggestions were generated
        if (!suggestions.Any())
        {
            var genericSuggestion = GenerateGenericTestName(currentName);
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"📝 Rename to '{genericSuggestion}'",
                    createChangedDocument: c => RenameTestMethodAsync(context.Document, methodDeclaration, genericSuggestion, c),
                    equivalenceKey: $"RenameTestMethod_{genericSuggestion}"),
                diagnostic);
        }
    }

    /// <summary>
    /// Renames a test method to the specified name.
    /// </summary>
    private static async Task<Document> RenameTestMethodAsync(Document document, MethodDeclarationSyntax methodDeclaration, string newName, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        // Rename the method declaration
        var newMethod = methodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newName));
        editor.ReplaceNode(methodDeclaration, newMethod);

        // Update method calls if they exist
        var updatedDocument = await UpdateMethodCallsAsync(editor.GetChangedDocument(), methodDeclaration.Identifier.ValueText, newName, cancellationToken).ConfigureAwait(false);

        return updatedDocument;
    }

    /// <summary>
    /// Generates test name suggestions based on the current method name.
    /// </summary>
    private static List<string> GenerateTestNameSuggestions(string currentName)
    {
        var suggestions = new List<string>();

        // Common test patterns and their conversions
        var patterns = new Dictionary<string, string>
        {
            { @"^Test(.+)$", "Should_$1_When_Called" },
            { @"^(.+)Test$", "Should_$1_When_Called" },
            { @"^(.+)Should(.+)$", "Should_$2_When_$1" },
            { @"^Should(.+)$", "Should_$1_When_Called" },
            { @"^(.+)When(.+)$", "Should_$1_When_$2" },
            { @"^(.+)Returns(.+)$", "Should_Return$2_When_$1" },
            { @"^(.+)Throws(.+)$", "Should_Throw$2_When_$1" },
            { @"^(.+)Is(.+)$", "Should_Be$2_When_$1" },
            { @"^(.+)Has(.+)$", "Should_Have$2_When_$1" },
            { @"^(.+)Can(.+)$", "Should_BeAbleTo$2_When_$1" },
            { @"^(.+)Will(.+)$", "Should_$2_When_$1" },
            { @"^(.+)Does(.+)$", "Should_$2_When_$1" }
        };

        foreach (var pattern in patterns)
        {
            var regex = new Regex(pattern.Key, RegexOptions.IgnoreCase);
            var match = regex.Match(currentName);
            if (match.Success)
            {
                var suggestion = regex.Replace(currentName, pattern.Value);
                suggestions.Add(suggestion);
            }
        }

        // Generate suggestions based on common test scenarios
        if (currentName.Contains("Null"))
        {
            suggestions.Add($"Should_ThrowArgumentNullException_When_{ExtractSubject(currentName)}IsNull");
        }

        if (currentName.Contains("Empty"))
        {
            suggestions.Add($"Should_HandleEmpty{ExtractSubject(currentName)}_When_Called");
        }

        if (currentName.Contains("Invalid"))
        {
            suggestions.Add($"Should_ThrowArgumentException_When_{ExtractSubject(currentName)}IsInvalid");
        }

        if (currentName.Contains("Exception"))
        {
            suggestions.Add($"Should_Throw{ExtractExceptionType(currentName)}_When_{ExtractSubject(currentName)}");
        }

        // Remove duplicates and return
        return suggestions.Distinct().ToList();
    }

    /// <summary>
    /// Generates a generic test name when no specific patterns match.
    /// </summary>
    private static string GenerateGenericTestName(string currentName)
    {
        // Convert to PascalCase and add Should prefix
        var words = SplitIntoWords(currentName);
        var action = string.Join("", words.Skip(1).Take(words.Count - 2));
        var subject = words.FirstOrDefault() ?? "Method";

        return $"Should_{action}_When_{subject}";
    }

    /// <summary>
    /// Extracts the subject from a test method name.
    /// </summary>
    private static string ExtractSubject(string methodName)
    {
        var words = SplitIntoWords(methodName);
        return words.FirstOrDefault() ?? "Parameter";
    }

    /// <summary>
    /// Extracts the exception type from a test method name.
    /// </summary>
    private static string ExtractExceptionType(string methodName)
    {
        if (methodName.Contains("ArgumentNullException"))
        {
            return "ArgumentNullException";
        }
        if (methodName.Contains("ArgumentException"))
        {
            return "ArgumentException";
        }
        if (methodName.Contains("InvalidOperationException"))
        {
            return "InvalidOperationException";
        }
        if (methodName.Contains("NotSupportedException"))
        {
            return "NotSupportedException";
        }
        if (methodName.Contains("TimeoutException"))
        {
            return "TimeoutException";
        }
        if (methodName.Contains("UnauthorizedAccessException"))
        {
            return "UnauthorizedAccessException";
        }
        if (methodName.Contains("FileNotFoundException"))
        {
            return "FileNotFoundException";
        }
        if (methodName.Contains("OutOfMemoryException"))
        {
            return "OutOfMemoryException";
        }

        return "Exception";
    }

    /// <summary>
    /// Splits a method name into words.
    /// </summary>
    private static List<string> SplitIntoWords(string methodName)
    {
        var words = new List<string>();
        var currentWord = "";

        foreach (var c in methodName)
        {
            if (char.IsUpper(c) && currentWord.Length > 0)
            {
                words.Add(currentWord);
                currentWord = "";
            }
            currentWord += c;
        }

        if (currentWord.Length > 0)
        {
            words.Add(currentWord);
        }

        return words;
    }

    /// <summary>
    /// Checks if a method is a test method.
    /// </summary>
    private static bool IsTestMethod(MethodDeclarationSyntax methodDeclaration)
    {
        // Check for test attributes
        var hasTestAttribute = methodDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(a => a.Name.ToString().Contains("Test") ||
                     a.Name.ToString().Contains("Fact") ||
                     a.Name.ToString().Contains("Theory"));

        // Check if method name contains test-related keywords
        var methodName = methodDeclaration.Identifier.ValueText.ToLower();
        var isTestName = methodName.Contains("test") ||
                        methodName.Contains("should") ||
                        methodName.Contains("when") ||
                        methodName.Contains("verify") ||
                        methodName.Contains("assert");

        return hasTestAttribute || isTestName;
    }

    /// <summary>
    /// Updates method calls to use the new method name.
    /// </summary>
    private static async Task<Document> UpdateMethodCallsAsync(Document document, string oldName, string newName, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return document;
        }

        var rewriter = new MethodCallRenamer(oldName, newName);
        var newRoot = rewriter.Visit(root);

        return document.WithSyntaxRoot(newRoot);
    }

    /// <summary>
    /// Rewriter that updates method calls to use the new method name.
    /// </summary>
    private class MethodCallRenamer : CSharpSyntaxRewriter
    {
        private readonly string oldName;
        private readonly string newName;

        public MethodCallRenamer(string oldName, string newName)
        {
            this.oldName = oldName;
            this.newName = newName;
        }

        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == oldName)
            {
                var newMemberAccess = memberAccess.WithName(
                    SyntaxFactory.IdentifierName(newName));
                return node.WithExpression(newMemberAccess);
            }

            return base.VisitInvocationExpression(node);
        }

        public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (node.Name.Identifier.ValueText == oldName)
            {
                return node.WithName(SyntaxFactory.IdentifierName(newName));
            }

            return base.VisitMemberAccessExpression(node);
        }
    }
}
