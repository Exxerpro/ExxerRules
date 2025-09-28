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

namespace IndFusion.CodeFixes.Architecture;

/// <summary>
/// Code fix provider that enforces domain layer separation from infrastructure.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DomainShouldNotReferenceInfrastructureCodeFixProvider)), Shared]
public class DomainShouldNotReferenceInfrastructureCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc/>
    public override sealed ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.DomainShouldNotReferenceInfrastructure);

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
            RegisterDomainInfrastructureFixes(context, diagnostic, node);
        }
    }

    /// <summary>
    /// Registers code fix options based on the type of infrastructure reference that can be fixed.
    /// </summary>
    /// <param name="context">The code fix context.</param>
    /// <param name="diagnostic">The diagnostic to fix.</param>
    /// <param name="node">The syntax node that can be converted.</param>
    private static void RegisterDomainInfrastructureFixes(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
    {
        switch (node)
        {
            case UsingDirectiveSyntax usingDirective:
                RegisterUsingFixes(context, diagnostic, usingDirective);
                break;
            case ObjectCreationExpressionSyntax objectCreation:
                RegisterObjectCreationFixes(context, diagnostic, objectCreation);
                break;
            case InvocationExpressionSyntax invocationExpression:
                RegisterInvocationFixes(context, diagnostic, invocationExpression);
                break;
            case TypeSyntax typeSyntax:
                RegisterTypeFixes(context, diagnostic, typeSyntax);
                break;
        }
    }

    /// <summary>
    /// Registers code fix options for using directive conversion.
    /// </summary>
    private static void RegisterUsingFixes(CodeFixContext context, Diagnostic diagnostic, UsingDirectiveSyntax usingDirective)
    {
        var usingName = usingDirective.Name?.ToString();
        if (IsInfrastructureUsing(usingName))
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "🔄 Replace infrastructure using with domain abstraction",
                    createChangedDocument: c => ReplaceInfrastructureUsingAsync(context.Document, usingDirective, c),
                    equivalenceKey: "ReplaceInfrastructureUsing"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "🔄 Add domain interface using",
                    createChangedDocument: c => AddDomainInterfaceUsingAsync(context.Document, usingDirective, c),
                    equivalenceKey: "AddDomainInterfaceUsing"),
                diagnostic);
        }
    }

    /// <summary>
    /// Registers code fix options for object creation conversion.
    /// </summary>
    private static void RegisterObjectCreationFixes(CodeFixContext context, Diagnostic diagnostic, ObjectCreationExpressionSyntax objectCreation)
    {
        var typeName = objectCreation.Type.ToString();
        if (IsInfrastructureType(typeName))
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "🔄 Replace infrastructure type with domain abstraction",
                    createChangedDocument: c => ReplaceInfrastructureTypeAsync(context.Document, objectCreation, c),
                    equivalenceKey: "ReplaceInfrastructureType"),
                diagnostic);
        }
    }

    /// <summary>
    /// Registers code fix options for invocation conversion.
    /// </summary>
    private static void RegisterInvocationFixes(CodeFixContext context, Diagnostic diagnostic, InvocationExpressionSyntax invocationExpression)
    {
        if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var methodName = memberAccess.Name.Identifier.ValueText;
            var expressionType = memberAccess.Expression.ToString();

            if (IsInfrastructureMethod(methodName, expressionType))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "🔄 Replace infrastructure method with domain abstraction",
                        createChangedDocument: c => ReplaceInfrastructureMethodAsync(context.Document, invocationExpression, c),
                        equivalenceKey: "ReplaceInfrastructureMethod"),
                    diagnostic);
            }
        }
    }

    /// <summary>
    /// Registers code fix options for type conversion.
    /// </summary>
    private static void RegisterTypeFixes(CodeFixContext context, Diagnostic diagnostic, TypeSyntax typeSyntax)
    {
        var typeName = typeSyntax.ToString();
        if (IsInfrastructureType(typeName))
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "🔄 Replace infrastructure type with domain interface",
                    createChangedDocument: c => ReplaceInfrastructureTypeWithInterfaceAsync(context.Document, typeSyntax, c),
                    equivalenceKey: "ReplaceInfrastructureTypeWithInterface"),
                diagnostic);
        }
    }

    /// <summary>
    /// Replaces infrastructure using directive with domain abstraction.
    /// </summary>
    private static async Task<Document> ReplaceInfrastructureUsingAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var newUsing = usingDirective.WithName(SyntaxFactory.ParseName("YourNamespace.Domain.Interfaces"));
        editor.ReplaceNode(usingDirective, newUsing);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds domain interface using directive.
    /// </summary>
    private static async Task<Document> AddDomainInterfaceUsingAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("YourNamespace.Domain.Interfaces"));
        editor.InsertAfter(usingDirective, newUsing);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Replaces infrastructure type with domain abstraction.
    /// </summary>
    private static async Task<Document> ReplaceInfrastructureTypeAsync(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new InfrastructureToDomainRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(objectCreation);
        editor.ReplaceNode(objectCreation, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Replaces infrastructure method with domain abstraction.
    /// </summary>
    private static async Task<Document> ReplaceInfrastructureMethodAsync(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new InfrastructureMethodToDomainRewriter();
        var newExpression = (ExpressionSyntax)rewriter.Visit(invocationExpression);
        editor.ReplaceNode(invocationExpression, newExpression);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Replaces infrastructure type with domain interface.
    /// </summary>
    private static async Task<Document> ReplaceInfrastructureTypeWithInterfaceAsync(Document document, TypeSyntax typeSyntax, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var rewriter = new InfrastructureTypeToInterfaceRewriter();
        var newType = (TypeSyntax)rewriter.Visit(typeSyntax);
        editor.ReplaceNode(typeSyntax, newType);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Checks if a using directive is for infrastructure.
    /// </summary>
    private static bool IsInfrastructureUsing(string? usingName)
    {
        if (string.IsNullOrEmpty(usingName))
            return false;

        // In netstandard2.0, nullable annotations on string helpers are not
        // available to flow analysis; assert non-null after guard above.
        var name = usingName!;
        return name.Contains("Microsoft.EntityFrameworkCore") ||
               name.Contains("System.Data.SqlClient") ||
               name.Contains("System.Data.OleDb") ||
               name.Contains("System.Data.Odbc") ||
               name.Contains("System.Data.Entity") ||
               name.Contains("Infrastructure") ||
               name.Contains("DataAccess") ||
               name.Contains("Persistence");
    }

    /// <summary>
    /// Checks if a type is an infrastructure type.
    /// </summary>
    private static bool IsInfrastructureType(string typeName)
    {
        return typeName.Contains("DbContext") ||
               typeName.Contains("SqlConnection") ||
               typeName.Contains("OleDbConnection") ||
               typeName.Contains("OdbcConnection") ||
               typeName.Contains("EntityFramework") ||
               typeName.Contains("Repository") ||
               typeName.Contains("DataContext") ||
               typeName.Contains("UnitOfWork");
    }

    /// <summary>
    /// Checks if a method is an infrastructure method.
    /// </summary>
    private static bool IsInfrastructureMethod(string methodName, string expressionType)
    {
        return (methodName == "SaveChanges" || methodName == "SaveChangesAsync") && expressionType.Contains("DbContext") ||
               (methodName == "ExecuteNonQuery" || methodName == "ExecuteReader") && expressionType.Contains("SqlCommand") ||
               (methodName == "Open" || methodName == "Close") && expressionType.Contains("Connection");
    }

    /// <summary>
    /// Rewriter that converts infrastructure types to domain abstractions.
    /// </summary>
    private class InfrastructureToDomainRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var typeName = node.Type.ToString();
            if (IsInfrastructureType(typeName))
            {
                // Replace infrastructure type with domain abstraction
                var domainType = GetDomainType(typeName);
                return SyntaxFactory.ObjectCreationExpression(
                    SyntaxFactory.ParseTypeName(domainType),
                    node.ArgumentList,
                    node.Initializer);
            }

            return base.VisitObjectCreationExpression(node);
        }

        private static string GetDomainType(string infrastructureType)
        {
            return infrastructureType switch
            {
                var t when t.Contains("DbContext") => "IDataContext",
                var t when t.Contains("SqlConnection") => "IConnection",
                var t when t.Contains("Repository") => "IRepository",
                var t when t.Contains("UnitOfWork") => "IUnitOfWork",
                _ => "IDomainService"
            };
        }
    }

    /// <summary>
    /// Rewriter that converts infrastructure methods to domain abstractions.
    /// </summary>
    private class InfrastructureMethodToDomainRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var methodName = memberAccess.Name.Identifier.ValueText;
                var expressionType = memberAccess.Expression.ToString();

                if (IsInfrastructureMethod(methodName, expressionType))
                {
                    // Replace infrastructure method with domain abstraction
                    var domainMethod = GetDomainMethod(methodName);
                    return SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("_domainService"),
                            SyntaxFactory.IdentifierName(domainMethod)),
                        node.ArgumentList);
                }
            }

            return base.VisitInvocationExpression(node);
        }

        private static string GetDomainMethod(string infrastructureMethod)
        {
            return infrastructureMethod switch
            {
                "SaveChanges" or "SaveChangesAsync" => "Commit",
                "ExecuteNonQuery" => "Execute",
                "ExecuteReader" => "Query",
                "Open" => "Connect",
                "Close" => "Disconnect",
                _ => infrastructureMethod
            };
        }
    }

    /// <summary>
    /// Rewriter that converts infrastructure types to domain interfaces.
    /// </summary>
    private class InfrastructureTypeToInterfaceRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
        {
            var typeName = node.ToString();
            if (IsInfrastructureType(typeName))
            {
                // Replace infrastructure type with domain interface
                var domainType = GetDomainType(typeName);
                return SyntaxFactory.IdentifierName(domainType);
            }

            return base.VisitIdentifierName(node);
        }

        public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
        {
            var typeName = node.ToString();
            if (IsInfrastructureType(typeName))
            {
                // Replace infrastructure type with domain interface
                var domainType = GetDomainType(typeName);
                return SyntaxFactory.ParseTypeName(domainType);
            }

            return base.VisitGenericName(node);
        }

        private static string GetDomainType(string infrastructureType)
        {
            return infrastructureType switch
            {
                var t when t.Contains("DbContext") => "IDataContext",
                var t when t.Contains("SqlConnection") => "IConnection",
                var t when t.Contains("Repository") => "IRepository",
                var t when t.Contains("UnitOfWork") => "IUnitOfWork",
                _ => "IDomainService"
            };
        }
    }
}
