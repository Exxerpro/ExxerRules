using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Architecture;

/// <summary>
/// Analyzer that enforces Domain layer should not reference Infrastructure layer.
/// Supports Clean Architecture principles.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DomainShouldNotReferenceInfrastructureAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Domain layer should not reference Infrastructure layer";
    private static readonly LocalizableString MessageFormat = "Domain layer class '{0}' should not reference Infrastructure namespace '{1}' - violates Clean Architecture";
    private static readonly LocalizableString Description = "In Clean Architecture, the Domain layer should be independent and not reference the Infrastructure layer. Dependencies should flow inward, with Infrastructure depending on Domain, not the reverse.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DomainShouldNotReferenceInfrastructure,
        Title,
        MessageFormat,
        DiagnosticCategories.Architecture,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TDD Green phase: Focus on using directives in Domain namespace files
        context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
    }

    private static void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
    {
        var usingDirective = (UsingDirectiveSyntax)context.Node;

        // Check if we're in a Domain namespace
        if (!IsInDomainNamespace(context.Node))
        {
            return;
        }

        var namespaceName = usingDirective.Name?.ToString();
        if (namespaceName == null)
        {
            return;
        }

        // Check if the using directive references Infrastructure
        if (IsInfrastructureNamespace(namespaceName))
        {
            // Check for various exemption scenarios
            if (IsExemptFromInfrastructureReference(usingDirective, context))
            {
                return;
            }

            var containingClass = GetContainingClassName(context.Node);

            var diagnostic = Diagnostic.Create(
                Rule,
                usingDirective.GetLocation(),
                containingClass ?? "Domain class",
                namespaceName);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsInDomainNamespace(SyntaxNode node)
    {
        // Find the containing namespace declaration
        var namespaceDeclaration = node.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();

        // If no namespace declaration found, check the compilation unit for namespace
        if (namespaceDeclaration == null)
        {
            // Look for namespace declarations in the file
            var root = node.SyntaxTree.GetRoot();
            var allNamespaces = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>();
            namespaceDeclaration = allNamespaces.FirstOrDefault();
        }

        if (namespaceDeclaration == null)
        {
            return false;
        }

        var namespaceName = namespaceDeclaration.Name.ToString();

        // Check if namespace contains "Domain" (case-sensitive)
        return namespaceName.Contains(".Domain.") ||
               namespaceName.StartsWith("Domain.") ||
               namespaceName.EndsWith(".Domain") ||
               namespaceName == "Domain";
    }

    private static bool IsInfrastructureNamespace(string namespaceName)
    {
        // Check if namespace contains "Infrastructure" (case-sensitive)
        if (namespaceName.Contains(".Infrastructure.") ||
            namespaceName.StartsWith("Infrastructure.") ||
            namespaceName.EndsWith(".Infrastructure") ||
            namespaceName == "Infrastructure")
        {
            return true;
        }

        // Check for Entity Framework Core (infrastructure concern)
        if (namespaceName.Contains("Microsoft.EntityFrameworkCore") ||
            namespaceName.StartsWith("Microsoft.EntityFrameworkCore") ||
            namespaceName.Contains("EntityFrameworkCore"))
        {
            return true;
        }

        // Check for other common infrastructure namespaces
        var infrastructureNamespaces = new[]
        {
            "Microsoft.EntityFrameworkCore",
            "System.Data.SqlClient",
            "System.Data.Odbc",
            "System.Data.OleDb",
            "Npgsql",
            "MySql.Data",
            "Oracle.ManagedDataAccess",
            "Microsoft.Data.SqlClient"
        };

        return infrastructureNamespaces.Any(ns => namespaceName.Contains(ns));
    }

    private static string? GetContainingClassName(SyntaxNode node)
    {
        // Find the containing class declaration
        var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        return classDeclaration?.Identifier.ValueText;
    }

    #region False-Positive Mitigation Methods

    /// <summary>
    /// Determines if a using directive is exempt from the infrastructure reference rule.
    /// </summary>
    private static bool IsExemptFromInfrastructureReference(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;

        // Story 1.1: Exempt EF Core Attributes on Domain Value Objects
        if (IsEFCoreAttributesOnDomainValueObjects(usingDirective, context))
        {
            return true;
        }

        // Story 1.2: Exempt Domain Enum Seeding Extensions
        if (IsDomainEnumSeedingExtensions(usingDirective, context))
        {
            return true;
        }

        // Story 1.3: Exempt Nested IEntityTypeConfiguration
        if (IsNestedIEntityTypeConfiguration(usingDirective, context))
        {
            return true;
        }

        // Story 1.4: Exempt Domain Tests Using EF InMemory Providers
        if (IsDomainTestsUsingEFInMemoryProviders(usingDirective, context))
        {
            return true;
        }

        // Story 1.5: Exempt Domain Tests Validating ModelBuilder Projections
        if (IsDomainTestsValidatingModelBuilderProjections(usingDirective, context))
        {
            return true;
        }

        // Story 1.6: Exempt SqlConnectionStringBuilder for Guard Logic
        if (IsSqlConnectionStringBuilderForGuardLogic(usingDirective, context))
        {
            return true;
        }

        // Story 1.7: Exempt Provider-Specific Validation in Domain Rules
        if (IsProviderSpecificValidationInDomainRules(usingDirective, context))
        {
            return true;
        }

        // Story 1.8: Exempt Domain Enum Synchronization Scripts
        if (IsDomainEnumSynchronizationScripts(usingDirective, context))
        {
            return true;
        }

        // Story 1.9: Exempt ValueComparer Usage in Domain Tests
        if (IsValueComparerUsageInDomainTests(usingDirective, context))
        {
            return true;
        }

        // Story 1.10: Exempt Migration Snapshot Verification in Domain Tests
        if (IsMigrationSnapshotVerificationInDomainTests(usingDirective, context))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.1: Exempt EF Core Attributes on Domain Value Objects
    /// </summary>
    private static bool IsEFCoreAttributesOnDomainValueObjects(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("Microsoft.EntityFrameworkCore"))
        {
            return false;
        }

        var root = usingDirective.SyntaxTree.GetRoot(context.CancellationToken);
        return root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Any(cls => cls.AttributeLists
                .SelectMany(static al => al.Attributes)
                .Select(static attr => attr.Name.ToString())
                .Any(attr => attr == "Owned" || attr.EndsWith(".Owned")));
    }

    /// <summary>
    /// Story 1.2: Exempt Domain Enum Seeding Extensions
    /// </summary>
    private static bool IsDomainEnumSeedingExtensions(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("Microsoft.EntityFrameworkCore"))
        {
            return false;
        }

        var root = usingDirective.SyntaxTree.GetRoot(context.CancellationToken);
        return root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Any(method =>
            {
                if (!method.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    return false;
                }

                if (!method.Identifier.Text.Contains("Seed", StringComparison.Ordinal))
                {
                    return false;
                }

                if (!method.ParameterList.Parameters.Any(p => p.Type?.ToString().Contains("ModelBuilder") == true))
                {
                    return false;
                }

                return method.FirstAncestorOrSelf<ClassDeclarationSyntax>()?.Modifiers.Any(SyntaxKind.StaticKeyword) == true;
            });
    }

    /// <summary>
    /// Story 1.3: Exempt Nested IEntityTypeConfiguration
    /// </summary>
    private static bool IsNestedIEntityTypeConfiguration(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("Microsoft.EntityFrameworkCore"))
        {
            return false;
        }

        var root = usingDirective.SyntaxTree.GetRoot(context.CancellationToken);
        return root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Any(inner =>
            {
                var parent = inner.Parent?.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                if (parent is null || parent == inner)
                {
                    return false;
                }

                return inner.BaseList?.Types.Any(t => t.Type.ToString().Contains("IEntityTypeConfiguration")) == true;
            });
    }

    /// <summary>
    /// Story 1.4: Exempt Domain Tests Using EF InMemory Providers
    /// </summary>
    private static bool IsDomainTestsUsingEFInMemoryProviders(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("Microsoft.EntityFrameworkCore"))
        {
            return false;
        }

        // Check if we're in a test file by looking at the file content
        var root = context.Node.SyntaxTree.GetRoot();
        var hasTestAttributes = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>()
            .Any(attr => IsTestAttribute(attr));

        if (!hasTestAttributes)
        {
            return false;
        }

        // Check if the test code contains domain namespaces
        var hasDomainNamespace = root.DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Any(ns => ns.Name.ToString().Contains("Domain"));

        return hasDomainNamespace;
    }

    /// <summary>
    /// Story 1.5: Exempt Domain Tests Validating ModelBuilder Projections
    /// </summary>
    private static bool IsDomainTestsValidatingModelBuilderProjections(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("Microsoft.EntityFrameworkCore.Metadata"))
        {
            return false;
        }

        // Check if we're in a test file by looking at the file content
        var root = context.Node.SyntaxTree.GetRoot();
        var hasTestAttributes = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>()
            .Any(attr => IsTestAttribute(attr));

        if (!hasTestAttributes)
        {
            return false;
        }

        // Check if the test code contains domain namespaces
        var hasDomainNamespace = root.DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Any(ns => ns.Name.ToString().Contains("Domain"));

        return hasDomainNamespace;
    }

    /// <summary>
    /// Story 1.6: Exempt SqlConnectionStringBuilder for Guard Logic
    /// </summary>
    private static bool IsSqlConnectionStringBuilderForGuardLogic(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("Microsoft.Data.SqlClient") && !namespaceName.Contains("System.Data.SqlClient"))
        {
            return false;
        }

        // Check if we're in a class that only uses SqlConnectionStringBuilder for validation
        var containingClass = usingDirective.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (containingClass == null)
        {
            return false;
        }

        // Check if the class name suggests it's for validation/parsing
        var className = containingClass.Identifier.Text;
        return className.Contains("Validator") || className.Contains("Parser") || className.Contains("Builder") ||
               className.Contains("ConnectionString"); // Also exempt connection string related classes
    }

    /// <summary>
    /// Story 1.7: Exempt Provider-Specific Validation in Domain Rules
    /// </summary>
    private static bool IsProviderSpecificValidationInDomainRules(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("NpgsqlTypes") && !namespaceName.Contains("MySql.Data") && !namespaceName.Contains("Oracle.ManagedDataAccess"))
        {
            return false;
        }

        // Check if we're in a test file by looking at the file content
        var root = context.Node.SyntaxTree.GetRoot();
        var hasTestAttributes = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>()
            .Any(attr => IsTestAttribute(attr));

        if (!hasTestAttributes)
        {
            return false;
        }

        // Check if the test code contains domain namespaces
        var hasDomainNamespace = root.DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Any(ns => ns.Name.ToString().Contains("Domain"));

        return hasDomainNamespace;
    }

    /// <summary>
    /// Story 1.8: Exempt Domain Enum Synchronization Scripts
    /// </summary>
    private static bool IsDomainEnumSynchronizationScripts(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("System.Data.SqlClient") && !namespaceName.Contains("Microsoft.Data.SqlClient"))
        {
            return false;
        }

        var root = usingDirective.SyntaxTree.GetRoot(context.CancellationToken);
        return root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Any(cls =>
            {
                var name = cls.Identifier.Text;
                return name.Contains("Synchronizer", StringComparison.Ordinal) ||
                       name.Contains("Sync", StringComparison.Ordinal) ||
                       name.Contains("Utility", StringComparison.Ordinal);
            });
    }

    /// <summary>
    /// Story 1.9: Exempt ValueComparer Usage in Domain Tests
    /// </summary>
    private static bool IsValueComparerUsageInDomainTests(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("Microsoft.EntityFrameworkCore.ChangeTracking"))
        {
            return false;
        }

        // Check if we're in a test file by looking at the file content
        var root = context.Node.SyntaxTree.GetRoot();
        var hasTestAttributes = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>()
            .Any(attr => IsTestAttribute(attr));

        if (!hasTestAttributes)
        {
            return false;
        }

        // Check if the test code contains domain namespaces
        var hasDomainNamespace = root.DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Any(ns => ns.Name.ToString().Contains("Domain"));

        return hasDomainNamespace;
    }

    /// <summary>
    /// Story 1.10: Exempt Migration Snapshot Verification in Domain Tests
    /// </summary>
    private static bool IsMigrationSnapshotVerificationInDomainTests(UsingDirectiveSyntax usingDirective, SyntaxNodeAnalysisContext context)
    {
        var namespaceName = usingDirective.Name?.ToString() ?? string.Empty;
        if (!namespaceName.Contains("Microsoft.EntityFrameworkCore.Migrations"))
        {
            return false;
        }

        // Check if we're in a test file by looking at the file content
        var root = context.Node.SyntaxTree.GetRoot();
        var hasTestAttributes = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>()
            .Any(attr => IsTestAttribute(attr));

        if (!hasTestAttributes)
        {
            return false;
        }

        // Check if the test code contains domain namespaces
        var hasDomainNamespace = root.DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Any(ns => ns.Name.ToString().Contains("Domain"));

        return hasDomainNamespace;
    }

    /// <summary>
    /// Helper method to check if an attribute is a test attribute.
    /// </summary>
    private static bool IsTestAttribute(Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax attribute)
    {
        var attributeName = attribute.Name.ToString();
        return attributeName == "Fact" || 
               attributeName == "Theory" || 
               attributeName == "Test" ||
               attributeName.EndsWith(".Fact") ||
               attributeName.EndsWith(".Theory") ||
               attributeName.EndsWith(".Test");
    }

    #endregion
}
