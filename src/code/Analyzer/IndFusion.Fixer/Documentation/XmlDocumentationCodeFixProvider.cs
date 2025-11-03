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

namespace IndFusion.CodeFixes.Documentation;

/// <summary>
/// Code fix provider that generates XML documentation for public members.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XmlDocumentationCodeFixProvider)), Shared]
public class XmlDocumentationCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.PublicMembersShouldHaveXmlDocumentation);

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

            // Register different code fix options based on the node type
            RegisterCodeFixOptions(context, diagnostic, node);
        }
    }

    /// <summary>
    /// Registers code fix options based on the type of member that needs documentation.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="node">The syntax node that needs documentation.</param>
    private static void RegisterCodeFixOptions(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
    {
        switch (node)
        {
            case ClassDeclarationSyntax classDeclaration:
                RegisterClassDocumentationFix(context, diagnostic, classDeclaration);
                break;
            case MethodDeclarationSyntax methodDeclaration:
                RegisterMethodDocumentationFix(context, diagnostic, methodDeclaration);
                break;
            case PropertyDeclarationSyntax propertyDeclaration:
                RegisterPropertyDocumentationFix(context, diagnostic, propertyDeclaration);
                break;
            case ConstructorDeclarationSyntax constructorDeclaration:
                RegisterConstructorDocumentationFix(context, diagnostic, constructorDeclaration);
                break;
            case InterfaceDeclarationSyntax interfaceDeclaration:
                RegisterInterfaceDocumentationFix(context, diagnostic, interfaceDeclaration);
                break;
            case EnumDeclarationSyntax enumDeclaration:
                RegisterEnumDocumentationFix(context, diagnostic, enumDeclaration);
                break;
            case FieldDeclarationSyntax fieldDeclaration:
                RegisterFieldDocumentationFix(context, diagnostic, fieldDeclaration);
                break;
            case EventDeclarationSyntax eventDeclaration:
                RegisterEventDocumentationFix(context, diagnostic, eventDeclaration);
                break;
        }
    }

    /// <summary>
    /// Registers code fix for class documentation.
    /// </summary>
    private static void RegisterClassDocumentationFix(CodeFixContext context, Diagnostic diagnostic, ClassDeclarationSyntax classDeclaration)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Add XML documentation for class '{classDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddClassDocumentationAsync(context.Document, classDeclaration, c),
                equivalenceKey: $"AddClassDocumentation_{classDeclaration.Identifier.ValueText}"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for method documentation.
    /// </summary>
    private static void RegisterMethodDocumentationFix(CodeFixContext context, Diagnostic diagnostic, MethodDeclarationSyntax methodDeclaration)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Add XML documentation for method '{methodDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddMethodDocumentationAsync(context.Document, methodDeclaration, c),
                equivalenceKey: $"AddMethodDocumentation_{methodDeclaration.Identifier.ValueText}"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for property documentation.
    /// </summary>
    private static void RegisterPropertyDocumentationFix(CodeFixContext context, Diagnostic diagnostic, PropertyDeclarationSyntax propertyDeclaration)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Add XML documentation for property '{propertyDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddPropertyDocumentationAsync(context.Document, propertyDeclaration, c),
                equivalenceKey: $"AddPropertyDocumentation_{propertyDeclaration.Identifier.ValueText}"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for constructor documentation.
    /// </summary>
    private static void RegisterConstructorDocumentationFix(CodeFixContext context, Diagnostic diagnostic, ConstructorDeclarationSyntax constructorDeclaration)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Add XML documentation for constructor",
                createChangedDocument: c => AddConstructorDocumentationAsync(context.Document, constructorDeclaration, c),
                equivalenceKey: $"AddConstructorDocumentation"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for interface documentation.
    /// </summary>
    private static void RegisterInterfaceDocumentationFix(CodeFixContext context, Diagnostic diagnostic, InterfaceDeclarationSyntax interfaceDeclaration)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Add XML documentation for interface '{interfaceDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddInterfaceDocumentationAsync(context.Document, interfaceDeclaration, c),
                equivalenceKey: $"AddInterfaceDocumentation_{interfaceDeclaration.Identifier.ValueText}"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for enum documentation.
    /// </summary>
    private static void RegisterEnumDocumentationFix(CodeFixContext context, Diagnostic diagnostic, EnumDeclarationSyntax enumDeclaration)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Add XML documentation for enum '{enumDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddEnumDocumentationAsync(context.Document, enumDeclaration, c),
                equivalenceKey: $"AddEnumDocumentation_{enumDeclaration.Identifier.ValueText}"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for field documentation.
    /// </summary>
    private static void RegisterFieldDocumentationFix(CodeFixContext context, Diagnostic diagnostic, FieldDeclarationSyntax fieldDeclaration)
    {
        var fieldName = fieldDeclaration.Declaration.Variables.FirstOrDefault()?.Identifier.ValueText ?? "field";
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Add XML documentation for field '{fieldName}'",
                createChangedDocument: c => AddFieldDocumentationAsync(context.Document, fieldDeclaration, c),
                equivalenceKey: $"AddFieldDocumentation_{fieldName}"),
            diagnostic);
    }

    /// <summary>
    /// Registers code fix for event documentation.
    /// </summary>
    private static void RegisterEventDocumentationFix(CodeFixContext context, Diagnostic diagnostic, EventDeclarationSyntax eventDeclaration)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: $"📝 Add XML documentation for event '{eventDeclaration.Identifier.ValueText}'",
                createChangedDocument: c => AddEventDocumentationAsync(context.Document, eventDeclaration, c),
                equivalenceKey: $"AddEventDocumentation_{eventDeclaration.Identifier.ValueText}"),
            diagnostic);
    }

    /// <summary>
    /// Adds XML documentation for a class.
    /// </summary>
    private static async Task<Document> AddClassDocumentationAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        var classSummary = GenerateClassSummary(classDeclaration);

        // Use the correct summary variable for XML trivia construction
        var xmlTrivia = SyntaxFactory.Trivia(
            SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(new XmlNodeSyntax[] { (XmlNodeSyntax)classSummary })
            )
        );
        var newClassDeclaration = classDeclaration.WithLeadingTrivia(xmlTrivia);
        editor.ReplaceNode(classDeclaration, newClassDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds XML documentation for a method.
    /// </summary>
    private static async Task<Document> AddMethodDocumentationAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        var methodSummary = GenerateMethodSummary(methodDeclaration);
        var parameters = GenerateMethodParameters(methodDeclaration);
        var returns = GenerateMethodReturns(methodDeclaration);

        // Replace generator.DocumentationComment(summary, parameters, returns) with direct XML trivia creation
        var xmlNodes = new List<XmlNodeSyntax> { (XmlNodeSyntax)methodSummary };
        xmlNodes.AddRange(parameters.Cast<XmlNodeSyntax>());
        if (returns != null) { xmlNodes.Add((XmlNodeSyntax)returns); }
        var xmlTrivia = SyntaxFactory.Trivia(
            SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(xmlNodes)
            )
        );
        var newMethodDeclaration = methodDeclaration.WithLeadingTrivia(xmlTrivia);
        editor.ReplaceNode(methodDeclaration, newMethodDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds XML documentation for a property.
    /// </summary>
    private static async Task<Document> AddPropertyDocumentationAsync(Document document, PropertyDeclarationSyntax propertyDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        var summary = GeneratePropertySummary(propertyDeclaration);
        // Replace generator.DocumentationComment(summary) with direct XML trivia creation for property
        var xmlTrivia = SyntaxFactory.Trivia(
            SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(new XmlNodeSyntax[] { (XmlNodeSyntax)summary })
            )
        );
        var newPropertyDeclaration = propertyDeclaration.WithLeadingTrivia(xmlTrivia);
        editor.ReplaceNode(propertyDeclaration, newPropertyDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds XML documentation for a constructor.
    /// </summary>
    private static async Task<Document> AddConstructorDocumentationAsync(Document document, ConstructorDeclarationSyntax constructorDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        var summary = GenerateConstructorSummary(constructorDeclaration);
        var parameters = GenerateMethodParameters(constructorDeclaration);

        // Replace generator.DocumentationComment(summary, parameters) with direct XML trivia creation for constructor
        var xmlNodes = new List<XmlNodeSyntax> { (XmlNodeSyntax)summary };
        xmlNodes.AddRange(parameters.Cast<XmlNodeSyntax>());
        var xmlTrivia = SyntaxFactory.Trivia(
            SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(xmlNodes)
            )
        );
        var newConstructorDeclaration = constructorDeclaration.WithLeadingTrivia(xmlTrivia);
        editor.ReplaceNode(constructorDeclaration, newConstructorDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds XML documentation for an interface.
    /// </summary>
    private static async Task<Document> AddInterfaceDocumentationAsync(Document document, InterfaceDeclarationSyntax interfaceDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        var summary = GenerateInterfaceSummary(interfaceDeclaration);
        // Replace generator.DocumentationComment(summary) with direct XML trivia creation for interface
        var xmlTrivia = SyntaxFactory.Trivia(
            SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(new XmlNodeSyntax[] { (XmlNodeSyntax)summary })
            )
        );
        var newInterfaceDeclaration = interfaceDeclaration.WithLeadingTrivia(xmlTrivia);
        editor.ReplaceNode(interfaceDeclaration, newInterfaceDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds XML documentation for an enum.
    /// </summary>
    private static async Task<Document> AddEnumDocumentationAsync(Document document, EnumDeclarationSyntax enumDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        var summary = GenerateEnumSummary(enumDeclaration);
        // Replace generator.DocumentationComment(summary) with direct XML trivia creation for enum
        var xmlTrivia = SyntaxFactory.Trivia(
            SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(new XmlNodeSyntax[] { (XmlNodeSyntax)summary })
            )
        );
        var newEnumDeclaration = enumDeclaration.WithLeadingTrivia(xmlTrivia);
        editor.ReplaceNode(enumDeclaration, newEnumDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds XML documentation for a field.
    /// </summary>
    private static async Task<Document> AddFieldDocumentationAsync(Document document, FieldDeclarationSyntax fieldDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        var summary = GenerateFieldSummary(fieldDeclaration);
        // Replace generator.DocumentationComment(summary) with direct XML trivia creation for field
        var xmlTrivia = SyntaxFactory.Trivia(
            SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(new XmlNodeSyntax[] { (XmlNodeSyntax)summary })
            )
        );
        var newFieldDeclaration = fieldDeclaration.WithLeadingTrivia(xmlTrivia);
        editor.ReplaceNode(fieldDeclaration, newFieldDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds XML documentation for an event.
    /// </summary>
    private static async Task<Document> AddEventDocumentationAsync(Document document, EventDeclarationSyntax eventDeclaration, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var generator = editor.Generator;

        var summary = GenerateEventSummary(eventDeclaration);
        // Replace generator.DocumentationComment(summary) with direct XML trivia creation for event
        var xmlTrivia = SyntaxFactory.Trivia(
            SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(new XmlNodeSyntax[] { (XmlNodeSyntax)summary })
            )
        );
        var newEventDeclaration = eventDeclaration.WithLeadingTrivia(xmlTrivia);
        editor.ReplaceNode(eventDeclaration, newEventDeclaration);
        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Generates a summary for a class.
    /// </summary>
    private static SyntaxNode GenerateClassSummary(ClassDeclarationSyntax classDeclaration)
    {
        var className = classDeclaration.Identifier.ValueText;
        var summary = $"Represents the {ToLowerFirst(className)} class.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
    }

    /// <summary>
    /// Generates a summary for a method.
    /// </summary>
    private static SyntaxNode GenerateMethodSummary(MethodDeclarationSyntax methodDeclaration)
    {
        var methodName = methodDeclaration.Identifier.ValueText;
        var summary = $"Executes the {ToLowerFirst(methodName)} operation.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
    }

    /// <summary>
    /// Generates a summary for a property.
    /// </summary>
    private static SyntaxNode GeneratePropertySummary(PropertyDeclarationSyntax propertyDeclaration)
    {
        var propertyName = propertyDeclaration.Identifier.ValueText;
        var summary = $"Gets or sets the {ToLowerFirst(propertyName)}.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
    }

    /// <summary>
    /// Generates a summary for a constructor.
    /// </summary>
    private static SyntaxNode GenerateConstructorSummary(ConstructorDeclarationSyntax constructorDeclaration)
    {
        var summary = "Initializes a new instance of the class.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
    }

    /// <summary>
    /// Generates a summary for an interface.
    /// </summary>
    private static SyntaxNode GenerateInterfaceSummary(InterfaceDeclarationSyntax interfaceDeclaration)
    {
        var interfaceName = interfaceDeclaration.Identifier.ValueText;
        var summary = $"Defines the contract for {ToLowerFirst(interfaceName)}.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
    }

    /// <summary>
    /// Generates a summary for an enum.
    /// </summary>
    private static SyntaxNode GenerateEnumSummary(EnumDeclarationSyntax enumDeclaration)
    {
        var enumName = enumDeclaration.Identifier.ValueText;
        var summary = $"Defines the {ToLowerFirst(enumName)} enumeration values.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
    }

    /// <summary>
    /// Generates a summary for a field.
    /// </summary>
    private static SyntaxNode GenerateFieldSummary(FieldDeclarationSyntax fieldDeclaration)
    {
        var fieldName = fieldDeclaration.Declaration.Variables.FirstOrDefault()?.Identifier.ValueText ?? "field";
        var summary = $"The {ToLowerFirst(fieldName)} field.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
    }

    /// <summary>
    /// Generates a summary for an event.
    /// </summary>
    private static SyntaxNode GenerateEventSummary(EventDeclarationSyntax eventDeclaration)
    {
        var eventName = eventDeclaration.Identifier.ValueText;
        var summary = $"Occurs when {ToLowerFirst(eventName)} happens.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary")));
    }

    /// <summary>
    /// Generates parameter documentation for a method or constructor.
    /// </summary>
    private static IEnumerable<SyntaxNode> GenerateMethodParameters(BaseMethodDeclarationSyntax methodDeclaration)
    {
        var parameters = new List<SyntaxNode>();

        foreach (var parameter in methodDeclaration.ParameterList.Parameters)
        {
            var parameterName = parameter.Identifier.ValueText;
            var parameterType = parameter.Type?.ToString() ?? "object";
            var summary = $"The {ToLowerFirst(parameterName)} parameter of type {parameterType}.";

            var paramElement = SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(
                    SyntaxFactory.XmlName("param"),
                    SyntaxFactory.List(new XmlAttributeSyntax[] {
                        SyntaxFactory.XmlTextAttribute(
                            "name",
                            SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                            SyntaxFactory.Identifier(parameterName),
                            SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken))
                    })
                ),
                SyntaxFactory.SingletonList<XmlNodeSyntax>(
                    SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
                SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("param")));

            parameters.Add(paramElement);
        }

        return parameters;
    }

    /// <summary>
    /// Generates return documentation for a method.
    /// </summary>
    private static SyntaxNode? GenerateMethodReturns(MethodDeclarationSyntax methodDeclaration)
    {
        var returnType = methodDeclaration.ReturnType.ToString();

        if (returnType == "void")
        {
            return null; // No return documentation for void methods
        }

        var summary = $"The result of type {returnType}.";

        return SyntaxFactory.XmlElement(
            SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("returns")),
            SyntaxFactory.SingletonList<XmlNodeSyntax>(
                SyntaxFactory.XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summary)))),
            SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("returns")));
    }

    /// <summary>
    /// Converts the first character of a string to lowercase.
    /// </summary>
    /// <param name="str">The input string.</param>
    /// <returns>The string with first character in lowercase.</returns>
    private static string ToLowerFirst(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
