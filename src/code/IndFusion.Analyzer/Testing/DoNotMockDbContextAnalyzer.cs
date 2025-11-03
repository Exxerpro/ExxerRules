using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Testing;

/// <summary>
/// Analyzer that prevents mocking of EF Core DbContext, enforcing InMemory provider usage.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DoNotMockDbContextAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the localized title displayed when an EF Core DbContext mock is detected.
    /// </summary>
    private static readonly LocalizableString Title = "Do not mock DbContext";

    /// <summary>
    /// Gets the diagnostic message format describing the offending DbContext.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Do not mock DbContext '{0}'. Use InMemory provider instead.";

    /// <summary>
    /// Gets the diagnostic description explaining the preference for the InMemory provider.
    /// </summary>
    private static readonly LocalizableString Description = "EF Core DbContext should not be mocked. Use the InMemory database provider for testing instead.";

    /// <summary>
    /// The diagnostic emitted when DbContext mocking is discovered in test code.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DoNotMockDbContext,
        Title,
        MessageFormat,
        DiagnosticCategories.Testing,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the DbContext mocking rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers syntax callbacks that flag mocking frameworks targeting EF Core DbContext types.
    /// </summary>
    /// <param name="context">The analysis context coordinating callbacks.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeGenericName, SyntaxKind.GenericName);
    }

    /// <summary>
    /// Inspects object creation expressions for mocking frameworks targeting DbContext types.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
        var typeName = objectCreation.Type.ToString();

        // Check for Mock<DbContext> or Mock<CustomDbContext>
        if (typeName.StartsWith("Mock<") && IsDbContextType(typeName, context.SemanticModel, objectCreation.Type))
        {
            var dbContextType = ExtractGenericTypeArgument(typeName);
            var diagnostic = Diagnostic.Create(
                Rule,
                objectCreation.GetLocation(),
                dbContextType);

            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Inspects invocation expressions for helper methods that produce mocked DbContext instances.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // Check for Mock.Of<DbContext>() calls
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.ValueText == "Of" &&
            memberAccess.Expression?.ToString() == "Mock")
        {
            if (memberAccess.Name is GenericNameSyntax genericName)
            {
                var typeArgument = genericName.TypeArgumentList.Arguments.FirstOrDefault();
                if (typeArgument != null && IsDbContextType(typeArgument, context.SemanticModel))
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        invocation.GetLocation(),
                        typeArgument.ToString());

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        // Check for Substitute.For<DbContext>() calls (this is also discouraged)
        if (invocation.Expression is MemberAccessExpressionSyntax substituteMemberAccess &&
            substituteMemberAccess.Name.Identifier.ValueText == "For" &&
            substituteMemberAccess.Expression?.ToString() == "Substitute")
        {
            if (substituteMemberAccess.Name is GenericNameSyntax substituteGenericName)
            {
                var typeArgument = substituteGenericName.TypeArgumentList.Arguments.FirstOrDefault();
                if (typeArgument != null && IsDbContextType(typeArgument, context.SemanticModel))
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        invocation.GetLocation(),
                        typeArgument.ToString());

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    /// <summary>
    /// Examines generic types such as <c>Mock&lt;T&gt;</c> to ensure DbContext types are not mocked.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeGenericName(SyntaxNodeAnalysisContext context)
    {
        var genericName = (GenericNameSyntax)context.Node;

        // Check for Mock<DbContext> in variable declarations and method parameters
        if (genericName.Identifier.ValueText == "Mock")
        {
            var typeArgument = genericName.TypeArgumentList.Arguments.FirstOrDefault();
            if (typeArgument != null && IsDbContextType(typeArgument, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    genericName.GetLocation(),
                    typeArgument.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    /// <summary>
    /// Determines whether a mocked generic type argument represents an EF Core DbContext.
    /// </summary>
    /// <param name="typeName">The textual representation of the mocked type.</param>
    /// <param name="semanticModel">The semantic model used to resolve symbols.</param>
    /// <param name="typeSyntax">The syntax node representing the type.</param>
    /// <returns><c>true</c> when the type resolves to a DbContext; otherwise, <c>false</c>.</returns>
    private static bool IsDbContextType(string typeName, SemanticModel semanticModel, TypeSyntax typeSyntax)
    {
        // Extract the generic type argument from Mock<T>
        var genericArg = ExtractGenericTypeArgument(typeName);
        if (string.IsNullOrEmpty(genericArg))
        {
            return false;
        }

        // Use semantic model to check if the type inherits from DbContext
        var typeInfo = semanticModel.GetTypeInfo(typeSyntax);
        if (typeInfo.Type is INamedTypeSymbol namedType)
        {
            return IsActualDbContext(namedType);
        }

        return false;
    }

    /// <summary>
    /// Determines whether the supplied type syntax resolves to an EF Core DbContext.
    /// </summary>
    /// <param name="typeSyntax">The syntax representing the type under analysis.</param>
    /// <param name="semanticModel">The semantic model used to resolve symbols.</param>
    /// <returns><c>true</c> when the type resolves to a DbContext; otherwise, <c>false</c>.</returns>
    private static bool IsDbContextType(TypeSyntax typeSyntax, SemanticModel semanticModel)
    {
        var typeInfo = semanticModel.GetTypeInfo(typeSyntax);
        if (typeInfo.Type is INamedTypeSymbol namedType)
        {
            return IsActualDbContext(namedType);
        }

        return false;
    }

    /// <summary>
    /// Determines if a type is an actual EF Core DbContext that should not be mocked.
    /// </summary>
    /// <param name="type">The type symbol to evaluate.</param>
    /// <returns><c>true</c> when the type is an eligible DbContext; otherwise, <c>false</c>.</returns>
    private static bool IsActualDbContext(INamedTypeSymbol type)
    {
        if (type == null)
            return false;

        // Check if it's a record (domain context records should not be flagged)
        if (type.IsRecord)
            return false;

        // Check if it's an interface (interfaces should not be flagged unless they inherit from DbContext)
        if (type.TypeKind == TypeKind.Interface)
        {
            // Only flag if the interface actually inherits from DbContext
            return InheritsFromDbContext(type);
        }

        // Check for framework-provided contexts that should be allowed
        if (IsFrameworkContext(type))
            return false;

        // Check for test framework contexts
        if (IsTestFrameworkContext(type))
            return false;

        // Check for ASP.NET Core HTTP contexts
        if (IsAspNetHttpContext(type))
            return false;

        // Check for DataAnnotations validation contexts
        if (IsDataAnnotationsValidationContext(type))
            return false;

        // Check for test helper contexts in specific namespaces
        if (IsTestHelperContext(type))
            return false;

        // Check for IDbContextFactory (should be allowed)
        if (IsDbContextFactory(type))
            return false;

        // Finally, check if it actually inherits from EF Core DbContext
        return InheritsFromDbContext(type);
    }

    /// <summary>
    /// Checks if the type inherits from <c>Microsoft.EntityFrameworkCore.DbContext</c>.
    /// </summary>
    /// <param name="type">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the type inherits from DbContext; otherwise, <c>false</c>.</returns>
    private static bool InheritsFromDbContext(INamedTypeSymbol type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.Name == "DbContext" &&
                baseType.ContainingNamespace?.ToDisplayString() == "Microsoft.EntityFrameworkCore")
            {
                return true;
            }
            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Checks if the type is a framework-provided context that should be allowed.
    /// </summary>
    /// <param name="type">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the type represents a framework context; otherwise, <c>false</c>.</returns>
    private static bool IsFrameworkContext(INamedTypeSymbol type)
    {
        var fullName = type.ToDisplayString();
        
        // Allow framework contexts
        return fullName == "System.ComponentModel.DataAnnotations.ValidationContext" ||
               fullName.StartsWith("Microsoft.AspNetCore.Http.") ||
               fullName == "Microsoft.AspNetCore.Http.HttpContext" ||
               fullName == "Microsoft.AspNetCore.Http.HttpRequest" ||
               fullName == "Microsoft.AspNetCore.Http.HttpResponse";
    }

    /// <summary>
    /// Checks if the type is a test framework context that should be allowed.
    /// </summary>
    /// <param name="type">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the type represents a test framework context; otherwise, <c>false</c>.</returns>
    private static bool IsTestFrameworkContext(INamedTypeSymbol type)
    {
        var fullName = type.ToDisplayString();
        
        // Allow test framework contexts
        return fullName == "Microsoft.VisualStudio.TestTools.UnitTesting.TestContext" ||
               fullName == "NUnit.Framework.TestContext" ||
               fullName == "Xunit.TestContext" ||
               fullName == "TestContext"; // Generic test context
    }

    /// <summary>
    /// Checks if the type is an ASP.NET Core HTTP context that should be allowed.
    /// </summary>
    /// <param name="type">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the type is an HTTP context abstraction; otherwise, <c>false</c>.</returns>
    private static bool IsAspNetHttpContext(INamedTypeSymbol type)
    {
        var fullName = type.ToDisplayString();
        return fullName.StartsWith("Microsoft.AspNetCore.Http.") ||
               fullName == "HttpContext" ||
               fullName == "HttpRequest" ||
               fullName == "HttpResponse";
    }

    /// <summary>
    /// Checks if the type is a DataAnnotations validation context that should be allowed.
    /// </summary>
    /// <param name="type">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the type represents a validation context; otherwise, <c>false</c>.</returns>
    private static bool IsDataAnnotationsValidationContext(INamedTypeSymbol type)
    {
        var fullName = type.ToDisplayString();
        return fullName == "System.ComponentModel.DataAnnotations.ValidationContext" ||
               fullName == "ValidationContext";
    }

    /// <summary>
    /// Checks if the type is a test helper context in specific namespaces that should be allowed.
    /// </summary>
    /// <param name="type">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the type represents a permitted helper context; otherwise, <c>false</c>.</returns>
    private static bool IsTestHelperContext(INamedTypeSymbol type)
    {
        var containingNamespace = type.ContainingNamespace?.ToDisplayString();
        
        // Allow test helper contexts in specific namespaces
        return containingNamespace?.StartsWith("Integration.Tests.Infrastructure") == true ||
               containingNamespace?.Contains("Test") == true && 
               (type.Name.EndsWith("Context") || type.Name.EndsWith("Logger"));
    }

    /// <summary>
    /// Checks if the type is an <c>IDbContextFactory</c> that should be allowed.
    /// </summary>
    /// <param name="type">The type symbol to inspect.</param>
    /// <returns><c>true</c> when the type represents an allowed factory; otherwise, <c>false</c>.</returns>
    private static bool IsDbContextFactory(INamedTypeSymbol type)
    {
        var fullName = type.ToDisplayString();
        return fullName.StartsWith("Microsoft.EntityFrameworkCore.IDbContextFactory") ||
               fullName.StartsWith("IDbContextFactory");
    }

    /// <summary>
    /// Extracts the underlying type argument from a generic type name such as <c>Mock&lt;T&gt;</c>.
    /// </summary>
    /// <param name="typeName">The generic type name to parse.</param>
    /// <returns>The extracted type name when present; otherwise, <see cref="string.Empty"/>.</returns>
    private static string ExtractGenericTypeArgument(string typeName)
    {
        // Extract T from Mock<T>
        var startIndex = typeName.IndexOf('<');
        var endIndex = typeName.LastIndexOf('>');

        if (startIndex > 0 && endIndex > startIndex)
        {
            return typeName.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
        }

        return string.Empty;
    }
}
