using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Architecture;

/// <summary>
/// Analyzer that enforces using Repository pattern with focused interfaces.
/// Supports Clean Architecture and dependency inversion principles.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseRepositoryPatternAnalyzer : DiagnosticAnalyzer
{
    private static readonly LocalizableString Title = "Use Repository pattern with focused interfaces";
    private static readonly LocalizableString MessageFormat = "Class '{0}' should use Repository pattern instead of direct data access - {1}";
    private static readonly LocalizableString Description = "Repository pattern provides abstraction over data access, making code more testable and maintainable. Use focused repository interfaces instead of direct DbContext or data access dependencies.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseRepositoryPattern,
        Title,
        MessageFormat,
        DiagnosticCategories.Architecture,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TDD Green phase: Focus on class declarations and their dependencies
        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var className = classDeclaration.Identifier.ValueText;

        // Skip if this is already a repository interface or implementation
        if (IsRepositoryClass(className))
        {
            // Check if repository implementation has corresponding interface
            if (className.EndsWith("Repository") && !className.StartsWith("I"))
            {
                CheckRepositoryHasInterface(context, classDeclaration);
            }
            return;
        }

        // Check for direct DbContext usage in non-repository classes
        CheckForDirectDataAccessUsage(context, classDeclaration);
    }

    private static void CheckRepositoryHasInterface(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        var className = classDeclaration.Identifier.ValueText;

        // Check if class implements an interface
        if (classDeclaration.BaseList?.Types.Count > 0)
        {
            var implementsInterface = classDeclaration.BaseList.Types
                .Any(t => t.Type.ToString().StartsWith("I") && t.Type.ToString().Contains("Repository"));

            if (implementsInterface)
            {
                return; // Has interface, good!
            }
        }

        // Repository class without interface
        var diagnostic = Diagnostic.Create(
            Rule,
            classDeclaration.Identifier.GetLocation(),
            className,
            "Repository implementation should implement a focused interface");
        context.ReportDiagnostic(diagnostic);
    }

    private static void CheckForDirectDataAccessUsage(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        var className = classDeclaration.Identifier.ValueText;
        var foundDirectAccess = false;

        // Check for various exemption scenarios first
        if (IsExemptFromRepositoryPatternRule(classDeclaration, context))
        {
            return;
        }

        // Look for DbContext fields/properties
        foreach (var member in classDeclaration.Members)
        {
            if (member is FieldDeclarationSyntax field)
            {
                var fieldType = field.Declaration.Type?.ToString() ?? "";
                if (IsDirectDataAccessType(fieldType))
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        field.Declaration.GetLocation(),
                        className,
                        $"Use repository pattern instead of direct {fieldType} field");
                    context.ReportDiagnostic(diagnostic);
                    foundDirectAccess = true;
                }
            }

            if (member is PropertyDeclarationSyntax property)
            {
                var propertyType = property.Type.ToString();
                if (IsDirectDataAccessType(propertyType))
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        property.Identifier.GetLocation(),
                        className,
                        $"Use repository pattern instead of direct {propertyType} property");
                    context.ReportDiagnostic(diagnostic);
                    foundDirectAccess = true;
                }
            }
        }

        // Look for DbContext constructor parameters (only if no field found)
        if (!foundDirectAccess)
        {
            foreach (var constructor in classDeclaration.Members.OfType<ConstructorDeclarationSyntax>())
            {
                if (constructor.ParameterList?.Parameters != null)
                {
                    foreach (var parameter in constructor.ParameterList.Parameters)
                    {
                        var paramType = parameter.Type?.ToString() ?? "";
                        if (IsDirectDataAccessType(paramType))
                        {
                            var diagnostic = Diagnostic.Create(
                                Rule,
                                parameter.GetLocation(),
                                className,
                                $"Use repository pattern instead of direct {paramType} parameter");
                            context.ReportDiagnostic(diagnostic);
                            return;
                        }
                    }
                }
            }
        }
    }

    private static bool IsRepositoryClass(string className) => className.Contains("Repository") ||
               className.Contains("DataAccess") ||
               className.Contains("Dal");

    private static bool IsInInfrastructureLayer(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        // Find the containing namespace declaration
        var namespaceDeclaration = classDeclaration.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();

        if (namespaceDeclaration == null)
        {
            // Look for namespace declarations in the file
            var root = classDeclaration.SyntaxTree.GetRoot();
            var allNamespaces = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>();
            namespaceDeclaration = allNamespaces.FirstOrDefault();
        }

        if (namespaceDeclaration == null)
        {
            return false;
        }

        var namespaceName = namespaceDeclaration.Name.ToString();

        // Check if namespace contains "Infrastructure"
        return namespaceName.Contains(".Infrastructure.") ||
               namespaceName.StartsWith("Infrastructure.") ||
               namespaceName.EndsWith(".Infrastructure") ||
               namespaceName == "Infrastructure";
    }

    private static bool IsDirectDataAccessType(string typeName)
    {
        var dataAccessTypes = new[]
        {
            "DbContext",
            "IDbContext",
            "DataContext",
            "ObjectContext",
            "SqlConnection",
            "IDbConnection",
            "SqlCommand",
            "IDbCommand"
        };

        return dataAccessTypes.Any(t => typeName.Contains(t));
    }

    #region False-Positive Mitigation Methods

    /// <summary>
    /// Determines if a class is exempt from the repository pattern rule.
    /// </summary>
    private static bool IsExemptFromRepositoryPatternRule(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        var className = classDeclaration.Identifier.ValueText;

        // Story 1.1: Exempt Application Layer Handlers
        if (IsApplicationLayerHandler(classDeclaration, context))
        {
            return true;
        }

        // Story 1.2: Exempt Infrastructure Layer
        if (IsInfrastructureLayer(classDeclaration, context))
        {
            return true;
        }

        // Story 1.3: Exempt Test and Fixture Classes
        if (IsTestOrFixtureClass(classDeclaration, context))
        {
            return true;
        }

        // Story 1.4: Exempt Connection Wrapper Classes
        if (IsConnectionWrapperClass(className))
        {
            return true;
        }

        // Story 1.5: Exempt DbContextOptions and EF Services
        if (IsDbContextOptionsOrEFService(classDeclaration))
        {
            return true;
        }

        // Story 1.6: Exempt Minimal APIs and Program.cs
        if (IsMinimalAPIOrProgramCs(classDeclaration, context))
        {
            return true;
        }

        // Story 1.7: Exempt Generic Infrastructure Services
        if (IsGenericInfrastructureService(className))
        {
            return true;
        }

        // Story 1.8: Exempt Generic Repository Base Classes
        if (IsGenericRepositoryBaseClass(classDeclaration))
        {
            return true;
        }

        // Story 1.9: Exempt Domain-Specific EF Extensions
        if (IsDomainSpecificEFExtension(classDeclaration, context))
        {
            return true;
        }

        // Story 1.10: Provide an Opt-Out Attribute
        if (HasAllowDirectDataAccessAttribute(classDeclaration))
        {
            return true;
        }

        // Story 1.11: Exempt DbContext Classes
        if (IsDbContextClass(classDeclaration))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Story 1.1: Exempt Application Layer Handlers
    /// </summary>
    private static bool IsApplicationLayerHandler(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        // Check if we're in an Application namespace
        if (!IsInApplicationNamespace(classDeclaration))
        {
            return false;
        }

        // Check if class implements a handler interface
        var implementsHandler = classDeclaration.BaseList?.Types
            .Any(t => t.Type.ToString().Contains("IRequestHandler") || 
                     t.Type.ToString().Contains("ICommandHandler") ||
                     t.Type.ToString().Contains("IQueryHandler")) == true;

        return implementsHandler;
    }

    /// <summary>
    /// Story 1.2: Exempt Infrastructure Layer
    /// </summary>
    private static bool IsInfrastructureLayer(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        return IsInInfrastructureLayer(context, classDeclaration);
    }

    /// <summary>
    /// Story 1.3: Exempt Test and Fixture Classes
    /// </summary>
    private static bool IsTestOrFixtureClass(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        var className = classDeclaration.Identifier.ValueText;
        
        // Check if class name suggests it's a test or fixture
        return className.EndsWith("Tests") || 
               className.EndsWith("Test") ||
               className.EndsWith("Fixture") ||
               className.EndsWith("TestBase");
    }

    /// <summary>
    /// Story 1.4: Exempt Connection Wrapper Classes
    /// </summary>
    private static bool IsConnectionWrapperClass(string className)
    {
        // Check if class name suggests it's a connection wrapper
        return className.EndsWith("Connection") || 
               className.EndsWith("Connector") ||
               className.EndsWith("Factory") ||
               className.Contains("Connection");
    }

    /// <summary>
    /// Story 1.5: Exempt DbContextOptions and EF Services
    /// </summary>
    private static bool IsDbContextOptionsOrEFService(ClassDeclarationSyntax classDeclaration)
    {
        // Check if class name suggests it's an EF service
        var className = classDeclaration.Identifier.ValueText;
        return className.Contains("DbContextFactory") ||
               className.Contains("DbContextManager") ||
               className.Contains("ServiceScope") ||
               className.Contains("EFService");
    }

    /// <summary>
    /// Story 1.6: Exempt Minimal APIs and Program.cs
    /// </summary>
    private static bool IsMinimalAPIOrProgramCs(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        // Check if we're in a file with top-level statements or Program.cs
        var filePath = context.Node.SyntaxTree.FilePath;
        return filePath.EndsWith("Program.cs") ||
               filePath.Contains("Program") ||
               HasTopLevelStatements(context.Node.SyntaxTree);
    }

    /// <summary>
    /// Story 1.7: Exempt Generic Infrastructure Services
    /// </summary>
    private static bool IsGenericInfrastructureService(string className)
    {
        // Check if class name suggests it's a generic infrastructure service
        return className.Contains("UnitOfWork") ||
               className.Contains("Transaction") ||
               className.Contains("Migration") ||
               className.Contains("Seeder") ||
               className.Contains("InfrastructureService");
    }

    /// <summary>
    /// Story 1.8: Exempt Generic Repository Base Classes
    /// </summary>
    /// <summary>
    /// Story 1.8: Exempt Generic Repository Base Classes
    /// </summary>
    private static bool IsGenericRepositoryBaseClass(ClassDeclarationSyntax classDeclaration)
    {
        // Check if class inherits from a generic repository base
        var baseTypes = classDeclaration.BaseList?.Types;
        if (baseTypes == null)
        {
            return false;
        }

        return baseTypes.Value.Any(t => t.Type.ToString().Contains("RepositoryBase") ||
                                       t.Type.ToString().Contains("Repository<T"));
    }

    /// <summary>
    /// Story 1.9: Exempt Domain-Specific EF Extensions
    /// </summary>
    private static bool IsDomainSpecificEFExtension(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        // Check if it's a static class in a persistence-related namespace
        var isStatic = classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
        if (!isStatic)
        {
            return false;
        }

        // Check if we're in a persistence-related namespace
        var namespaceDeclaration = classDeclaration.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        if (namespaceDeclaration == null)
        {
            return false;
        }

        var namespaceName = namespaceDeclaration.Name.ToString();
        return namespaceName.Contains("Persistence") ||
               namespaceName.Contains("Extensions") ||
               namespaceName.Contains("EF");
    }

    /// <summary>
    /// Story 1.10: Provide an Opt-Out Attribute
    /// </summary>
    private static bool HasAllowDirectDataAccessAttribute(ClassDeclarationSyntax classDeclaration)
    {
        // Check for [AllowDirectDataAccess] attribute
        var attributes = classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Select(a => a.Name.ToString());

        return attributes.Any(attr => attr == "AllowDirectDataAccess" || 
                                     attr.EndsWith(".AllowDirectDataAccess"));
    }

    /// <summary>
    /// Helper method to check if we're in an Application namespace.
    /// </summary>
    private static bool IsInApplicationNamespace(ClassDeclarationSyntax classDeclaration)
    {
        var namespaceDeclaration = classDeclaration.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        if (namespaceDeclaration == null)
        {
            return false;
        }

        var namespaceName = namespaceDeclaration.Name.ToString();
        return namespaceName.Contains(".Application.") ||
               namespaceName.StartsWith("Application.") ||
               namespaceName.EndsWith(".Application") ||
               namespaceName == "Application";
    }

    /// <summary>
    /// Helper method to check if a file has top-level statements.
    /// </summary>
    private static bool HasTopLevelStatements(SyntaxTree syntaxTree)
    {
        var root = syntaxTree.GetRoot();
        return root.DescendantNodes().OfType<GlobalStatementSyntax>().Any();
    }

    /// <summary>
    /// Story 1.11: Exempt DbContext Classes
    /// </summary>
    private static bool IsDbContextClass(ClassDeclarationSyntax classDeclaration)
    {
        // Check if class inherits from DbContext
        var baseTypes = classDeclaration.BaseList?.Types;
        if (baseTypes == null)
        {
            return false;
        }

        return baseTypes.Value.Any(t => t.Type.ToString().Contains("DbContext"));
    }

    #endregion
}
