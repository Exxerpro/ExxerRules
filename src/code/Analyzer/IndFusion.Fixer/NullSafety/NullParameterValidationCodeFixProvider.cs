using System.Collections.Immutable;
using System.Composition;
using IndFusion.Analyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace IndFusion.Fixer.NullSafety;

/// <summary>
/// Code fix provider that adds null parameter validation using the Result&lt;T&gt; pattern.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NullParameterValidationCodeFixProvider)), Shared]
public class NullParameterValidationCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.ValidateNullParameters);

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

            // Find the containing method or constructor
            var methodDeclaration = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var constructorDeclaration = node.Ancestors().OfType<ConstructorDeclarationSyntax>().FirstOrDefault();

            if (methodDeclaration != null)
            {
                RegisterMethodValidationFixes(context, diagnostic, methodDeclaration);
            }
            else if (constructorDeclaration != null)
            {
                RegisterConstructorValidationFixes(context, diagnostic, constructorDeclaration);
            }
        }
    }

    /// <summary>
    /// Registers code fix options for method parameter validation.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="methodDeclaration">The method declaration.</param>
    private static void RegisterMethodValidationFixes(CodeFixContext context, Diagnostic diagnostic, MethodDeclarationSyntax methodDeclaration)
    {
        // Basic null validation
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"🔍 Add null validation for method '{methodDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddMethodNullValidationAsync(context.Document, methodDeclaration, c),
                equivalenceKey: $"AddMethodNullValidation_{methodDeclaration.Identifier.ValueText}"),
            diagnostic);

        // Null validation with Result pattern
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"✅ Add Result-based null validation for '{methodDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddMethodResultNullValidationAsync(context.Document, methodDeclaration, c),
                equivalenceKey: $"AddMethodResultNullValidation_{methodDeclaration.Identifier.ValueText}"),
            diagnostic);

        // Null validation with early return
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"⚡ Add early return null validation for '{methodDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddMethodEarlyReturnNullValidationAsync(context.Document, methodDeclaration, c),
                equivalenceKey: $"AddMethodEarlyReturnNullValidation_{methodDeclaration.Identifier.ValueText}"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix options for constructor parameter validation.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="constructorDeclaration">The constructor declaration.</param>
    private static void RegisterConstructorValidationFixes(CodeFixContext context, Diagnostic diagnostic, ConstructorDeclarationSyntax constructorDeclaration)
    {
        // Basic null validation
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "🔍 Add null validation for constructor",
                createChangedDocument: c => AddConstructorNullValidationAsync(context.Document, constructorDeclaration, c),
                equivalenceKey: "AddConstructorNullValidation"),
            diagnostic);

        // Null validation with Result pattern
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "✅ Add Result-based null validation for constructor",
                createChangedDocument: c => AddConstructorResultNullValidationAsync(context.Document, constructorDeclaration, c),
                equivalenceKey: "AddConstructorResultNullValidation"),
            diagnostic);
    }

    /// <summary>
    /// Adds basic null validation to a method.
    /// </summary>
    private static async Task<Document> AddMethodNullValidationAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var validationStatements = GenerateBasicNullValidationStatements(methodDeclaration.ParameterList.Parameters);
        var newMethod = methodDeclaration.WithBody(AddStatementsToMethodBody(methodDeclaration, validationStatements));

        editor.ReplaceNode(methodDeclaration, newMethod);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds Result-based null validation to a method.
    /// </summary>
    private static async Task<Document> AddMethodResultNullValidationAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        // Convert method to return Result<T> if it doesn't already
        var newReturnType = ConvertToResultReturnType(methodDeclaration.ReturnType);
        var newMethod = methodDeclaration.WithReturnType(newReturnType);

        // Add Result-based null validation
        var validationStatements = GenerateResultNullValidationStatements(methodDeclaration.ParameterList.Parameters);
        newMethod = newMethod.WithBody(AddStatementsToMethodBody(newMethod, validationStatements));

        editor.ReplaceNode(methodDeclaration, newMethod);

        // Add using statement if needed
        await AddResultUsingStatementAsync(editor, cancellationToken).ConfigureAwait(false);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds early return null validation to a method.
    /// </summary>
    private static async Task<Document> AddMethodEarlyReturnNullValidationAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var validationStatements = GenerateEarlyReturnNullValidationStatements(methodDeclaration.ParameterList.Parameters);
        var newMethod = methodDeclaration.WithBody(AddStatementsToMethodBody(methodDeclaration, validationStatements));

        editor.ReplaceNode(methodDeclaration, newMethod);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds basic null validation to a constructor.
    /// </summary>
    private static async Task<Document> AddConstructorNullValidationAsync(Document document, ConstructorDeclarationSyntax constructorDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var validationStatements = GenerateBasicNullValidationStatements(constructorDeclaration.ParameterList.Parameters);
        var newConstructor = constructorDeclaration.WithBody(AddStatementsToConstructorBody(constructorDeclaration, validationStatements));

        editor.ReplaceNode(constructorDeclaration, newConstructor);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds Result-based null validation to a constructor.
    /// </summary>
    private static async Task<Document> AddConstructorResultNullValidationAsync(Document document, ConstructorDeclarationSyntax constructorDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var validationStatements = GenerateResultNullValidationStatements(constructorDeclaration.ParameterList.Parameters);
        var newConstructor = constructorDeclaration.WithBody(AddStatementsToConstructorBody(constructorDeclaration, validationStatements));

        editor.ReplaceNode(constructorDeclaration, newConstructor);

        // Add using statement if needed
        await AddResultUsingStatementAsync(editor, cancellationToken).ConfigureAwait(false);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Generates basic null validation statements.
    /// </summary>
    private static List<StatementSyntax> GenerateBasicNullValidationStatements(SeparatedSyntaxList<ParameterSyntax> parameters)
    {
        var statements = new List<StatementSyntax>();

        foreach (var parameter in parameters)
        {
            if (IsReferenceType(parameter.Type))
            {
                var parameterName = parameter.Identifier.ValueText;
                var validationStatement = SyntaxFactory.ParseStatement($"ArgumentNullException.ThrowIfNull({parameterName});");
                statements.Add(validationStatement);
            }
        }

        return statements;
    }

    /// <summary>
    /// Generates Result-based null validation statements.
    /// </summary>
    private static List<StatementSyntax> GenerateResultNullValidationStatements(SeparatedSyntaxList<ParameterSyntax> parameters)
    {
        var statements = new List<StatementSyntax>();

        foreach (var parameter in parameters)
        {
            if (IsReferenceType(parameter.Type))
            {
                var parameterName = parameter.Identifier.ValueText;
                var validationStatement = SyntaxFactory.ParseStatement(
                    $"if ({parameterName} is null) return Result.WithFailure($\"Parameter '{parameterName}' cannot be null.\");");
                statements.Add(validationStatement);
            }
        }

        return statements;
    }

    /// <summary>
    /// Generates early return null validation statements.
    /// </summary>
    private static List<StatementSyntax> GenerateEarlyReturnNullValidationStatements(SeparatedSyntaxList<ParameterSyntax> parameters)
    {
        var statements = new List<StatementSyntax>();

        foreach (var parameter in parameters)
        {
            if (IsReferenceType(parameter.Type))
            {
                var parameterName = parameter.Identifier.ValueText;
                var validationStatement = SyntaxFactory.ParseStatement(
                    $"if ({parameterName} is null) throw new ArgumentNullException(nameof({parameterName}));");
                statements.Add(validationStatement);
            }
        }

        return statements;
    }

    /// <summary>
    /// Adds statements to a method body.
    /// </summary>
    private static BlockSyntax AddStatementsToMethodBody(MethodDeclarationSyntax methodDeclaration, List<StatementSyntax> validationStatements)
    {
        if (methodDeclaration.Body == null)
        {
            // Method has expression body, convert to block body
            var statements = new List<StatementSyntax>();
            statements.AddRange(validationStatements);

            // Add return statement if method has return type
            if (methodDeclaration.ReturnType.ToString() != "void")
            {
                statements.Add(SyntaxFactory.ParseStatement("return default;"));
            }

            return SyntaxFactory.Block(statements);
        }

        var existingStatements = methodDeclaration.Body.Statements.ToList();
        validationStatements.InsertRange(0, existingStatements);
        return methodDeclaration.Body.WithStatements(SyntaxFactory.List(validationStatements));
    }

    /// <summary>
    /// Adds statements to a constructor body.
    /// </summary>
    private static BlockSyntax AddStatementsToConstructorBody(ConstructorDeclarationSyntax constructorDeclaration, List<StatementSyntax> validationStatements)
    {
        if (constructorDeclaration.Body == null)
        {
            return SyntaxFactory.Block(validationStatements);
        }

        var existingStatements = constructorDeclaration.Body.Statements.ToList();
        validationStatements.InsertRange(0, existingStatements);
        return constructorDeclaration.Body.WithStatements(SyntaxFactory.List(validationStatements));
    }

    /// <summary>
    /// Converts a return type to Result&lt;T&gt;.
    /// </summary>
    private static TypeSyntax ConvertToResultReturnType(TypeSyntax currentReturnType)
    {
        if (IsTaskType(currentReturnType))
        {
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
    /// Adds the Result using statement if it doesn't already exist.
    /// </summary>
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
    /// Checks if a type is a reference type.
    /// </summary>
    private static bool IsReferenceType(TypeSyntax? type)
    {
        if (type == null)
        {
            return false;
        }

        var typeString = type.ToString();
        return !typeString.Contains("struct") &&
               !typeString.Contains("int") &&
               !typeString.Contains("long") &&
               !typeString.Contains("short") &&
               !typeString.Contains("byte") &&
               !typeString.Contains("uint") &&
               !typeString.Contains("ulong") &&
               !typeString.Contains("ushort") &&
               !typeString.Contains("sbyte") &&
               !typeString.Contains("float") &&
               !typeString.Contains("double") &&
               !typeString.Contains("decimal") &&
               !typeString.Contains("bool") &&
               !typeString.Contains("char") &&
               !typeString.Contains("enum");
    }

    /// <summary>
    /// Checks if a type is a Task type.
    /// </summary>
    private static bool IsTaskType(TypeSyntax type)
    {
        var typeString = type.ToString();
        return typeString.StartsWith("Task<") || typeString == "Task";
    }

    /// <summary>
    /// Gets the inner type from a Task&lt;T&gt;.
    /// </summary>
    private static string? GetTaskInnerType(TypeSyntax type)
    {
        if (type is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Count > 0)
        {
            return genericName.TypeArgumentList.Arguments[0].ToString();
        }
        return null;
    }
}
