using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Architecture;

/// <summary>
/// Flags references from the domain layer into the infrastructure layer to preserve the Clean Architecture dependency direction.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DomainShouldNotReferenceInfrastructureAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the localized title surfaced for violations that cross the domain-to-infrastructure boundary.
    /// </summary>
    private static readonly LocalizableString Title = "Domain layer should not reference Infrastructure layer";

    /// <summary>
    /// Gets the localized message format providing details about the offending domain type and infrastructure namespace.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Domain layer class '{0}' should not reference Infrastructure namespace '{1}' - violates Clean Architecture";

    /// <summary>
    /// Gets the localized diagnostic description that explains the architectural guideline enforced by this analyzer.
    /// </summary>
    private static readonly LocalizableString Description = "In Clean Architecture, the Domain layer should be independent and not reference the Infrastructure layer. Dependencies should flow inward, with Infrastructure depending on Domain, not the reverse.";

    /// <summary>
    /// Diagnostic descriptor that represents the domain-to-infrastructure dependency violation.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.DomainShouldNotReferenceInfrastructure,
        Title,
        MessageFormat,
        DiagnosticCategories.Architecture,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>A single-element array containing the rule that protects the domain layer from infrastructure dependencies.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers the syntax node actions that enforce the domain-to-infrastructure dependency rule.
    /// </summary>
    /// <param name="context">The analysis context used to register callbacks and configure execution behavior.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TDD Green phase: Focus on using directives in Domain namespace files
        context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
    }

    /// <summary>
    /// Evaluates a <c>using</c> directive to determine whether it introduces an infrastructure dependency within the domain layer.
    /// </summary>
    /// <param name="context">The syntax analysis context that supplies the directive and semantic information.</param>
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

    /// <summary>
    /// Determines whether the syntax node is declared within a namespace that belongs to the domain layer.
    /// </summary>
    /// <param name="node">The node to inspect for enclosing namespace information.</param>
    /// <returns><c>true</c> when the namespace identifies domain code; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Tests whether the provided namespace represents an infrastructure concern.
    /// </summary>
    /// <param name="namespaceName">The namespace referenced by a <c>using</c> directive.</param>
    /// <returns><c>true</c> when the namespace maps to known infrastructure assemblies; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Retrieves the name of the nearest containing class for diagnostic messaging.
    /// </summary>
    /// <param name="node">The syntax node from which to ascend toward a class declaration.</param>
    /// <returns>The parent class name, or <see langword="null"/> when no class scope exists.</returns>
    private static string? GetContainingClassName(SyntaxNode node)
    {
        // Find the containing class declaration
        var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        return classDeclaration?.Identifier.ValueText;
    }

    //  False-Positive Mitigation Methods

    /// <summary>
    /// Determines whether the supplied <paramref name="usingDirective"/> falls under one of the sanctioned exemption rules.
    /// </summary>
    /// <param name="usingDirective">The directive currently under analysis.</param>
    /// <param name="context">The syntax context that provides semantic information for the file.</param>
    /// <returns><c>true</c> when the directive matches a known exception; otherwise, <c>false</c>.</returns>
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
    /// Story 1.1: Exempts EF Core attribute references applied to domain value objects.
    /// </summary>
    /// <param name="usingDirective">The directive referencing a namespace potentially belonging to EF Core.</param>
    /// <param name="context">The syntax analysis context for the current file.</param>
    /// <returns><c>true</c> when the directive exists solely to support EF Core owned entity annotations; otherwise, <c>false</c>.</returns>
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
    /// Story 1.2: Exempts domain enum seeding extensions that rely on EF Core infrastructure namespaces.
    /// </summary>
    /// <param name="usingDirective">The directive currently being evaluated for exemptions.</param>
    /// <param name="context">The syntax context that enables structural inspection of the file.</param>
    /// <returns><c>true</c> when the directive supports static seeding helpers intended for domain enums; otherwise, <c>false</c>.</returns>
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
    /// Story 1.3: Exempts nested <c>IEntityTypeConfiguration</c> implementations contained inside domain types.
    /// </summary>
    /// <param name="usingDirective">The directive under consideration.</param>
    /// <param name="context">The syntax context that exposes descendant declarations.</param>
    /// <returns><c>true</c> when the directive exists solely to facilitate nested EF Core configuration; otherwise, <c>false</c>.</returns>
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
    /// Story 1.4: Exempts domain test files that bring in EF Core in-memory providers as part of test infrastructure.
    /// </summary>
    /// <param name="usingDirective">The directive being analyzed.</param>
    /// <param name="context">The syntax context supplying file-level information.</param>
    /// <returns><c>true</c> when the directive appears inside a domain-focused test that utilises in-memory EF providers; otherwise, <c>false</c>.</returns>
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
    /// Story 1.5: Exempts domain tests that validate <c>ModelBuilder</c> projections requiring EF Core metadata namespaces.
    /// </summary>
    /// <param name="usingDirective">The directive that possibly references EF Core metadata.</param>
    /// <param name="context">The analysis context used to inspect the surrounding syntax tree.</param>
    /// <returns><c>true</c> when the directive appears in a domain test verifying projections; otherwise, <c>false</c>.</returns>
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
    /// Story 1.6: Exempts <c>SqlConnectionStringBuilder</c> references used exclusively for guard logic inside domain validators.
    /// </summary>
    /// <param name="usingDirective">The directive referencing SQL client namespaces.</param>
    /// <param name="context">The syntax context that exposes the containing class structure.</param>
    /// <returns><c>true</c> when the directive is used by guard utilities validating connection strings; otherwise, <c>false</c>.</returns>
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
    /// Story 1.7: Exempts provider-specific validation logic embedded inside domain rules.
    /// </summary>
    /// <param name="usingDirective">The directive that may reference provider namespaces.</param>
    /// <param name="context">The syntax context granting access to file-level information.</param>
    /// <returns><c>true</c> when the directive supports provider-specific domain validation; otherwise, <c>false</c>.</returns>
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
    /// Story 1.8: Exempts domain enum synchronization scripts that legitimately touch SQL client APIs.
    /// </summary>
    /// <param name="usingDirective">The directive referencing SQL client namespaces.</param>
    /// <param name="context">The syntax context utilised to identify synchronization helper classes.</param>
    /// <returns><c>true</c> when the directive assists in enum synchronization scripts; otherwise, <c>false</c>.</returns>
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
    /// Story 1.9: Exempts domain tests that rely on EF Core <c>ValueComparer</c> instances.
    /// </summary>
    /// <param name="usingDirective">The directive referencing change-tracking infrastructure.</param>
    /// <param name="context">The syntax context used to recognise test scenarios.</param>
    /// <returns><c>true</c> when the directive exists in a domain test leveraging value comparers; otherwise, <c>false</c>.</returns>
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
    /// Story 1.10: Exempts domain tests that verify migration snapshots and therefore reference migrations namespaces.
    /// </summary>
    /// <param name="usingDirective">The directive referencing EF Core migration APIs.</param>
    /// <param name="context">The syntax context that confirms the file is a domain test.</param>
    /// <returns><c>true</c> when the directive belongs to a migration snapshot verification test; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the specified attribute represents a unit-testing attribute from common frameworks.
    /// </summary>
    /// <param name="attribute">The attribute syntax node to evaluate.</param>
    /// <returns><c>true</c> when the attribute denotes a test-case decorator; otherwise, <c>false</c>.</returns>
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

     // 
}
