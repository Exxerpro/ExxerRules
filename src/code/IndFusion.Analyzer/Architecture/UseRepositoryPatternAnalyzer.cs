using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IndFusion.Analyzers.Architecture;

/// <summary>
/// Encourages teams to rely on repository abstractions instead of leaking direct data-access dependencies into higher layers.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseRepositoryPatternAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the localized title that explains why repository abstractions are required.
    /// </summary>
    private static readonly LocalizableString Title = "Use Repository pattern with focused interfaces";

    /// <summary>
    /// Gets the message format used when reporting a repository-pattern violation.
    /// </summary>
    private static readonly LocalizableString MessageFormat = "Class '{0}' should use Repository pattern instead of direct data access - {1}";

    /// <summary>
    /// Gets the diagnostic description that provides architectural context for the rule.
    /// </summary>
    private static readonly LocalizableString Description = "Repository pattern provides abstraction over data access, making code more testable and maintainable. Use focused repository interfaces instead of direct DbContext or data access dependencies.";

    /// <summary>
    /// The diagnostic descriptor emitted when classes bypass repository abstractions.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.UseRepositoryPattern,
        Title,
        MessageFormat,
        DiagnosticCategories.Architecture,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostic descriptors supported by this analyzer.
    /// </summary>
    /// <value>An immutable array containing the repository-pattern requirement rule.</value>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// Registers the syntax node analysis callbacks required to detect repository-pattern violations.
    /// </summary>
    /// <param name="context">The Roslyn analysis context for registering actions and configuring execution.</param>
    /// <remarks>
    /// Generated code is excluded and concurrent execution is enabled before analyzing class declarations for direct data-access dependencies.
    /// </remarks>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TDD Green phase: Focus on class declarations and their dependencies
        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    /// <summary>
    /// Inspects a class declaration to identify direct data-access usage or repository implementations lacking interfaces.
    /// </summary>
    /// <param name="context">The syntax analysis context that supplies the current class declaration.</param>
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

    /// <summary>
    /// Confirms that a repository implementation exposes an interface abstraction for consumers.
    /// </summary>
    /// <param name="context">The syntax analysis context used to report diagnostics.</param>
    /// <param name="classDeclaration">The repository class under evaluation.</param>
    private static void CheckRepositoryHasInterface(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        var className = classDeclaration.Identifier.ValueText;

        // Exempt repository base classes and their implementations
        if (IsRepositoryBaseClass(classDeclaration, context.SemanticModel))
        {
            return;
        }

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

    /// <summary>
    /// Detects direct data-access dependencies (fields, properties, or constructor parameters) within non-repository classes.
    /// </summary>
    /// <param name="context">The syntax analysis context used to emit diagnostics.</param>
    /// <param name="classDeclaration">The class declaration being reviewed.</param>
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

    /// <summary>
    /// Determines whether a class name indicates a repository implementation or supporting construct.
    /// </summary>
    /// <param name="className">The class name to evaluate.</param>
    /// <returns><c>true</c> when the name suggests repository semantics; otherwise, <c>false</c>.</returns>
    private static bool IsRepositoryClass(string className) => className.Contains("Repository") ||
               className.Contains("DataAccess") ||
               className.Contains("Dal");

    /// <summary>
    /// Determines whether a class is declared inside the infrastructure layer namespace hierarchy.
    /// </summary>
    /// <param name="context">The syntax analysis context for namespace resolution.</param>
    /// <param name="classDeclaration">The class declaration under inspection.</param>
    /// <returns><c>true</c> when the namespace indicates infrastructure code; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Determines whether a type name represents a direct data-access construct such as a DbContext or SQL command.
    /// </summary>
    /// <param name="typeName">The type name extracted from fields, properties, or parameters.</param>
    /// <returns><c>true</c> when the type name matches known data-access abstractions; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the supplied class should be exempted from repository-pattern enforcement based on a series of heuristics.
    /// </summary>
    /// <param name="classDeclaration">The class declaration currently being analyzed.</param>
    /// <param name="context">The syntax analysis context used to inspect namespace, attributes, and semantic information.</param>
    /// <returns><c>true</c> when the class meets any documented exemption; otherwise, <c>false</c>.</returns>
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

    // Story 1.12: Exempt Repository Base Classes and Their Implementations
    if (IsRepositoryBaseClass(classDeclaration, context.SemanticModel))
    {
        return true;
    }

        return false;
    }

    /// <summary>
    /// Story 1.1: Exempts application-layer request handlers that legitimately coordinate repositories.
    /// </summary>
    /// <param name="classDeclaration">The class declaration under inspection.</param>
    /// <param name="context">The syntax context that enables namespace checks.</param>
    /// <returns><c>true</c> when the class appears to be an application handler; otherwise, <c>false</c>.</returns>
    private static bool IsApplicationLayerHandler(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        // Check if we're in an Application namespace
        if (!IsInApplicationNamespace(classDeclaration))
        {
            return false;
        }

        var className = classDeclaration.Identifier.ValueText;

        // Check if class name ends with "Handler" (e.g., CreateUserCommandHandler)
        if (className.EndsWith("Handler"))
        {
            return true;
        }

        // Check if class implements a handler interface
        var implementsHandler = classDeclaration.BaseList?.Types
            .Any(t => t.Type.ToString().Contains("IRequestHandler") || 
                     t.Type.ToString().Contains("ICommandHandler") ||
                     t.Type.ToString().Contains("IQueryHandler")) == true;

        return implementsHandler;
    }

    /// <summary>
    /// Story 1.2: Exempts classes declared inside the infrastructure layer so that infrastructure code can access data sources directly.
    /// </summary>
    /// <param name="classDeclaration">The class declaration being evaluated.</param>
    /// <param name="context">The syntax context used to determine the containing namespace.</param>
    /// <returns><c>true</c> when the class is located within infrastructure; otherwise, <c>false</c>.</returns>
    private static bool IsInfrastructureLayer(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        return IsInInfrastructureLayer(context, classDeclaration);
    }

    /// <summary>
    /// Story 1.3: Exempts test and fixture classes that intentionally exercise direct data access for verification.
    /// </summary>
    /// <param name="classDeclaration">The class declaration to inspect.</param>
    /// <param name="context">The syntax context used for name-based heuristics.</param>
    /// <returns><c>true</c> when the class name indicates a test or fixture; otherwise, <c>false</c>.</returns>
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
    /// Story 1.4: Exempts connection-wrapper classes that act as protective adapters over raw connections.
    /// </summary>
    /// <param name="className">The class name under evaluation.</param>
    /// <returns><c>true</c> when the name indicates a connection wrapper; otherwise, <c>false</c>.</returns>
    private static bool IsConnectionWrapperClass(string className)
    {
        // Check if class name suggests it's a connection wrapper
        return className.EndsWith("Connection") || 
               className.EndsWith("Connector") ||
               className.EndsWith("Factory") ||
               className.Contains("Connection");
    }

    /// <summary>
    /// Story 1.5: Exempts EF Core service and configuration classes that must interact with DbContext options.
    /// </summary>
    /// <param name="classDeclaration">The class declaration being inspected.</param>
    /// <returns><c>true</c> when the class name signals EF-related services; otherwise, <c>false</c>.</returns>
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
    /// Story 1.6: Exempts minimal API bootstrapping files and <c>Program.cs</c> which legitimately wire infrastructure.
    /// </summary>
    /// <param name="classDeclaration">The class declaration located inside the file.</param>
    /// <param name="context">The syntax context used to inspect file path and top-level statements.</param>
    /// <returns><c>true</c> when the file hosts minimal API setup; otherwise, <c>false</c>.</returns>
    private static bool IsMinimalAPIOrProgramCs(ClassDeclarationSyntax classDeclaration, SyntaxNodeAnalysisContext context)
    {
        // Check if we're in a file with top-level statements or Program.cs
        var filePath = context.Node.SyntaxTree.FilePath;
        return filePath.EndsWith("Program.cs") ||
               filePath.Contains("Program") ||
               HasTopLevelStatements(context.Node.SyntaxTree);
    }

    /// <summary>
    /// Story 1.7: Exempts generic infrastructure services such as unit-of-work or migration helpers.
    /// </summary>
    /// <param name="className">The class name to evaluate.</param>
    /// <returns><c>true</c> when the class name matches known infrastructure service patterns; otherwise, <c>false</c>.</returns>
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
    /// Story 1.8: Exempts generic repository base classes that provide shared data-access functionality.
    /// </summary>
    /// <param name="classDeclaration">The class declaration to inspect.</param>
    /// <returns><c>true</c> when the class inherits from a repository base type; otherwise, <c>false</c>.</returns>
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
    /// Story 1.9: Exempts domain-specific EF Core extension classes that provide persistence helpers.
    /// </summary>
    /// <param name="classDeclaration">The class declaration under evaluation.</param>
    /// <param name="context">The syntax context providing namespace information.</param>
    /// <returns><c>true</c> when the class is a static persistence extension; otherwise, <c>false</c>.</returns>
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
    /// Story 1.10: Allows authors to opt out of the rule via a custom <c>[AllowDirectDataAccess]</c> attribute.
    /// </summary>
    /// <param name="classDeclaration">The class declaration to inspect for the opt-out attribute.</param>
    /// <returns><c>true</c> when the attribute is present; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the class resides inside an application-layer namespace.
    /// </summary>
    /// <param name="classDeclaration">The class declaration whose namespace is being inspected.</param>
    /// <returns><c>true</c> when the namespace indicates application-layer code; otherwise, <c>false</c>.</returns>
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
    /// Determines whether the given syntax tree contains top-level statements.
    /// </summary>
    /// <param name="syntaxTree">The syntax tree representing the file.</param>
    /// <returns><c>true</c> when top-level statements exist; otherwise, <c>false</c>.</returns>
    private static bool HasTopLevelStatements(SyntaxTree syntaxTree)
    {
        var root = syntaxTree.GetRoot();
        return root.DescendantNodes().OfType<GlobalStatementSyntax>().Any();
    }

    /// <summary>
    /// Story 1.11: Exempts DbContext implementations from the rule.
    /// </summary>
    /// <param name="classDeclaration">The class declaration under analysis.</param>
    /// <returns><c>true</c> when the base type chain includes <c>DbContext</c>; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Story 1.12: Exempts repository base classes and derived implementations that inherit repository-specific abstractions.
    /// </summary>
    /// <param name="classDeclaration">The class declaration being inspected.</param>
    /// <param name="semanticModel">The semantic model needed to resolve base-type symbols.</param>
    /// <returns><c>true</c> when the class inherits from a repository-flavoured base type; otherwise, <c>false</c>.</returns>
    private static bool IsRepositoryBaseClass(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
    {
        // Only exempt classes that inherit from a repository base class
        // NOT classes that just have "Repository" in their name
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol?.BaseType != null)
        {
            var baseTypeName = classSymbol.BaseType.Name;
            // Check if base type contains "Repository" (e.g., RepositoryBase, GenericRepository)
            if (baseTypeName.Contains("Repository"))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
