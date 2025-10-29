namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Unit tests for class duplication architecture validation
/// </summary>
public class ClassDuplicationTests
{
    private readonly ILogger<ClassDuplicationTests> _logger;

    /// <summary>
    /// Initializes the architecture duplication test harness with structured logging.
    /// </summary>
    /// <param name="testOutputHelper">xUnit output helper used to emit rule evaluation diagnostics.</param>
    public ClassDuplicationTests(ITestOutputHelper testOutputHelper)
    {
        _logger = XUnitLogger.CreateLogger<ClassDuplicationTests>(testOutputHelper);
    }

    /// <summary>
    /// Verifies that All classes should not have duplicate names in different should namespaces.
    /// </summary>
    /// <param name="assemblyName">Test value provided by the data set.</param>
    [Theory]
    [MemberData(nameof(ProductionAssemblies))]
    public void All_Classes_Should_Not_Have_Duplicate_Names_In_Different_Namespaces(string assemblyName)
    {
        _logger.LogInformation($"Checking assembly: {assemblyName}");

        // Try to load the assembly, skip if not available
        Assembly assembly;
        try
        {
            assembly = Assembly.Load(assemblyName);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning($"Assembly {assemblyName} not found in test context: {ex.Message}. Skipping test.");
            // Skip the test for assemblies that are not available in the test context
            Assert.Skip($"Assembly {assemblyName} is not available in the test context. This could be due to platform-specific builds or missing project references.");
            return;
        }
        catch (Exception ex) when (ex is BadImageFormatException || ex is FileLoadException)
        {
            _logger.LogWarning($"Assembly {assemblyName} could not be loaded: {ex.Message}. Skipping test.");
            // Skip the test for assemblies that have loading issues
            Assert.Skip($"Assembly {assemblyName} could not be loaded: {ex.Message}");
            return;
        }

        // Find all types in the assembly, excluding Blazor legitimate patterns
        var types = Types.InAssembly(assembly)
            .That()
            .AreClasses()
            .And()
            .DoNotHaveNameMatching("TypeInference") // Blazor auto-generated
            .And()
            .DoNotHaveNameMatching("_Imports") // Blazor _Imports.razor files
            .And()
            .DoNotHaveNameMatching("InputModel") // Blazor page-specific form models
            .And()
            .DoNotResideInNamespaceMatching("__Blazor.*") // Blazor internal namespaces
            .And()
            .DoNotResideInNamespace("ExxerAI")
            .GetTypes();

        // Group by class name and check for duplicates in different namespaces
        var duplicates = types.GroupBy(t => t.Name)
            .Where(g => g.Count() > 1)
            .Select(g => new { ClassName = g.Key, Namespaces = g.Select(t => t.Namespace).Distinct().ToList() })
            .Where(g => g.Namespaces.Count > 1)
            .ToList();

        bool any = false;
        foreach (var dup in duplicates)
        {
            any = true;
            _logger.LogError("  DUPLICATE: {ClassName} in {Namespaces}", dup.ClassName, dup.Namespaces);
        }

        if (any)
        {
            _logger.LogError("FAILED: {assemblyName} has duplicate class names in different namespaces:", assemblyName);
        }
        else
        {
            _logger.LogInformation("PASSED: {assemblyName} has no duplicate class", assemblyName);
        }

        // Use Shouldly to assert that there are no duplicated class names across different namespaces
        duplicates.ShouldBeEmpty($"Duplicate class names found in assembly '{assemblyName}': {string.Join(", ", duplicates.Select(d => $"{d.ClassName} in [{string.Join(", ", d.Namespaces)}]"))}");
    }

    /// <summary>
    /// Provides the list of production assemblies that should be scanned for duplicate class names.
    /// </summary>
    public static IEnumerable<object[]> ProductionAssemblies => new[]
    {
        // Core Layer
        new object[] { "ExxerAI.Domain" },
        new object[] { "ExxerAI.Application" },
        
        // Infrastructure Layer
        new object[] { "ExxerAI.Infrastructure" },
        new object[] { "ExxerAI.Agents" },
        new object[] { "ExxerAI.Api" },
        new object[] { "ExxerAI.CLI.Library" },
        new object[] { "ExxerAI.CLI" },
        new object[] { "ExxerAI.Composition" },
        new object[] { "ExxerAI.DocSentinel" },
        new object[] { "ExxerAI.MCPServer" },
        new object[] { "ExxerAI.Nexus" },
        
        // Presentation Layer
        new object[] { "ExxerAI.UI.Library" },
        new object[] { "ExxerAI.UI" },
        new object[] { "ExxerAI.UI.Web" },
        new object[] { "ExxerAI.Agents.UI" },
        new object[] { "CubeXplorer.UI.Web" },
        new object[] { "ExxerAI.Aspire.Dashboard" },
        
        // Orchestration Layer
        new object[] { "ExxerAI.AddLogginTelemetry" },
        new object[] { "CubeXplorer.UI.ServiceDefaults" },
        
        // Contracts Layer
        new object[] { "ExxerAI.Contracts" },
        
        // SemanticRag Layer
        new object[] { "IndFusion.SemanticRag.Domain" },
        new object[] { "IndFusion.SemanticRag.Application" },
        new object[] { "IndFusion.SemanticRag.Infrastructure" },
        new object[] { "IndFusion.SemanticRag.WebAPI" },
    };

    // Individual tests for each production project to ensure no duplicate types across namespaces within the same assembly

    /// <summary>
    /// Verifies that ExxerAI.Domain should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void Domain_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Domain");
    }

    /// <summary>
    /// Verifies that ExxerAI.Application should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void Application_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Application");
    }

    /// <summary>
    /// Verifies that ExxerAI.Infrastructure should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void Infrastructure_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Infrastructure");
    }

    /// <summary>
    /// Verifies that ExxerAI.Agents should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void Agents_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Agents");
    }

    /// <summary>
    /// Verifies that ExxerAI.Api should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void Api_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Api");
    }

    /// <summary>
    /// Verifies that ExxerAI.CLI.Library should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void CLILibrary_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.CLI.Library");
    }

    /// <summary>
    /// Verifies that ExxerAI.CLI should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void CLI_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.CLI");
    }

    /// <summary>
    /// Verifies that ExxerAI.Composition should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void Composition_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Composition");
    }

    /// <summary>
    /// Verifies that ExxerAI.DocSentinel should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void DocSentinel_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.DocSentinel");
    }

    /// <summary>
    /// Verifies that ExxerAI.MCPServer should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void MCPServer_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.MCPServer");
    }

    /// <summary>
    /// Verifies that ExxerAI.Nexus should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void Nexus_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Nexus");
    }

    /// <summary>
    /// Verifies that ExxerAI.UI.Library should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void UILibrary_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.UI.Library");
    }

    /// <summary>
    /// Verifies that ExxerAI.UI should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void UI_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.UI");
    }

    /// <summary>
    /// Verifies that ExxerAI.UI.Web should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void UIWeb_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.UI.Web");
    }

    /// <summary>
    /// Verifies that ExxerAI.Agents.UI should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void AgentsUI_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Agents.UI");
    }

    /// <summary>
    /// Verifies that CubeXplorer.UI.Web should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void CubeXplorerUIWeb_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("CubeXplorer.UI.Web");
    }

    /// <summary>
    /// Verifies that ExxerAI.Aspire.Dashboard should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void AspireDashboard_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Aspire.Dashboard");
    }

    /// <summary>
    /// Verifies that ExxerAI.AddLogginTelemetry should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void AddLogginTelemetry_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.AddLogginTelemetry");
    }

    /// <summary>
    /// Verifies that CubeXplorer.UI.ServiceDefaults should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void CubeXplorerUIServiceDefaults_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("CubeXplorer.UI.ServiceDefaults");
    }

    /// <summary>
    /// Verifies that ExxerAI.Contracts should not have duplicate types across different namespaces within the same assembly.
    /// </summary>
    [Fact]
    public void Contracts_Should_Not_Duplicate_Types_Across_Namespaces()
    {
        CheckSingleAssemblyForDuplicateTypesAcrossNamespaces("ExxerAI.Contracts");
    }

    /// <summary>
    /// Helper method to check a single assembly for duplicate types across different namespaces.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to check</param>
    private void CheckSingleAssemblyForDuplicateTypesAcrossNamespaces(string assemblyName)
    {
        _logger.LogInformation($"Checking assembly: {assemblyName} for duplicate types across namespaces");

        // Try to load the assembly, skip if not available
        Assembly assembly;
        try
        {
            assembly = Assembly.Load(assemblyName);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning($"Assembly {assemblyName} not found in test context: {ex.Message}. Skipping test.");
            // Skip the test for assemblies that are not available in the test context
            Assert.Skip($"Assembly {assemblyName} is not available in the test context. This could be due to platform-specific builds or missing project references.");
            return;
        }
        catch (Exception ex) when (ex is BadImageFormatException || ex is FileLoadException)
        {
            _logger.LogWarning($"Assembly {assemblyName} could not be loaded: {ex.Message}. Skipping test.");
            // Skip the test for assemblies that have loading issues
            Assert.Skip($"Assembly {assemblyName} could not be loaded: {ex.Message}");
            return;
        }

        // Find all types in the assembly, excluding Blazor legitimate patterns
        var types = Types.InAssembly(assembly)
            .That()
            .AreClasses()
            .And()
            .DoNotHaveNameMatching("TypeInference") // Blazor auto-generated
            .And()
            .DoNotHaveNameMatching("_Imports") // Blazor _Imports.razor files
            .And()
            .DoNotHaveNameMatching("InputModel") // Blazor page-specific form models
            .And()
            .DoNotResideInNamespaceMatching("__Blazor.*") // Blazor internal namespaces
            .And()
            .DoNotResideInNamespace("ExxerAI")
            .GetTypes();

        // Group by class name and check for duplicates in different namespaces
        var duplicates = types.GroupBy(t => t.Name)
            .Where(g => g.Count() > 1)
            .Select(g => new { ClassName = g.Key, Namespaces = g.Select(t => t.Namespace).Distinct().ToList() })
            .Where(g => g.Namespaces.Count > 1)
            .ToList();

        bool any = false;
        foreach (var dup in duplicates)
        {
            any = true;
            _logger.LogError("  DUPLICATE: {ClassName} in {Namespaces}", dup.ClassName, dup.Namespaces);
        }

        if (any)
        {
            _logger.LogError("FAILED: {assemblyName} has duplicate class names in different namespaces:", assemblyName);
        }
        else
        {
            _logger.LogInformation("PASSED: {assemblyName} has no duplicate class names across namespaces", assemblyName);
        }

        // Use Shouldly to assert that there are no duplicated class names across different namespaces
        duplicates.ShouldBeEmpty($"Duplicate class names found in assembly '{assemblyName}': {string.Join(", ", duplicates.Select(d => $"{d.ClassName} in [{string.Join(", ", d.Namespaces)}]"))}");
    }

    /// <summary>
    /// Verifies that Orchestration should not duplicate domain or application should types.
    /// </summary>
    [Fact]
    public void Infrastructure_Should_Not_Duplicate_Domain_Or_Application_Types()
    {
        _logger.LogInformation("Starting cross-assembly duplication check between Orchestration, Domain, and Application layers.");

        var infrastructureAssembly = Assembly.Load("ExxerAI.Infrastructure");
        var domainAssembly = Assembly.Load("ExxerAI.Domain");
        var applicationAssembly = Assembly.Load("ExxerAI.Application");

        var infrastructureTypes = Types.InAssembly(infrastructureAssembly)
            .That().AreClasses().Or().AreInterfaces().GetTypes()
            .Where(t => t.IsPublic)
            .ToList();

        var domainTypes = Types.InAssembly(domainAssembly)
            .That().AreClasses().Or().AreInterfaces().GetTypes()
            .Where(t => t.IsPublic)
            .ToList();

        var applicationTypes = Types.InAssembly(applicationAssembly)
            .That().AreClasses().Or().AreInterfaces().GetTypes()
            .Where(t => t.IsPublic)
            .ToList();

        var allRelevantTypes = new List<Type>();
        allRelevantTypes.AddRange(infrastructureTypes);
        allRelevantTypes.AddRange(domainTypes);
        allRelevantTypes.AddRange(applicationTypes);

        var duplicatedTypeNames = allRelevantTypes
            .GroupBy(t => t.Name)
            .Where(g => g.Count() > 1 && g.Any(t => t.Assembly == infrastructureAssembly) && g.Any(t => t.Assembly == domainAssembly || t.Assembly == applicationAssembly))
            .Select(g => g.Key)
            .ToList();

        var violations = new List<string>();

        foreach (var typeName in duplicatedTypeNames)
        {
            var typesWithSameName = allRelevantTypes.Where(t => t.Name == typeName).ToList();
            _logger.LogError($"Detected potential duplicate type name '{typeName}' across assemblies:");

            foreach (var type in typesWithSameName)
            {
                _logger.LogError($"  - {type.FullName}, Assembly: {type.Assembly.GetName().Name}");
            }

            // Further inspect properties for duplication
            var infrastructureVersion = typesWithSameName.FirstOrDefault(t => t.Assembly == infrastructureAssembly);
            var domainOrApplicationVersion = typesWithSameName.FirstOrDefault(t => t.Assembly == domainAssembly || t.Assembly == applicationAssembly);

            if (infrastructureVersion != null && domainOrApplicationVersion != null)
            {
                var infrastructureProperties = infrastructureVersion.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new PropertyInfoLite(p.Name, p.PropertyType.Name))
                    .ToList();
                var otherProperties = domainOrApplicationVersion.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new PropertyInfoLite(p.Name, p.PropertyType.Name))
                    .ToList();

                var commonProperties = infrastructureProperties
                    .Intersect(otherProperties)
                    .ToList();

                if (commonProperties.Any())
                {
                    violations.Add($"Type '{typeName}' has {commonProperties.Count} common properties between '{infrastructureVersion.Assembly.GetName().Name}' and '{domainOrApplicationVersion.Assembly.GetName().Name}'. This indicates duplication.");
                    _logger.LogError($"  Common properties for '{typeName}':");
                    foreach (var prop in commonProperties)
                    {
                        _logger.LogError($"    - {prop.Name} ({prop.PropertyType})");
                    }
                }
            }
        }

        if (violations.Any())
        {
            _logger.LogError("Architecture Violation: Duplicated types and/or properties found across Orchestration, Domain, and Application layers.");
            Assert.Fail(string.Join(Environment.NewLine, violations));
        }
        else
        {
            _logger.LogInformation("PASSED: No significant type duplication detected between Orchestration, Domain, and Application layers.");
        }
    }

    /// <summary>
    /// Verifies that Application layer should not duplicate Domain types.
    /// Application layer should reference Domain types, not create duplicates.
    /// </summary>
    [Fact]
    public void Application_Should_Not_Duplicate_Domain_Types()
    {
        _logger.LogInformation("Starting cross-assembly duplication check between Application and Domain layers.");

        var applicationAssembly = Assembly.Load("ExxerAI.Application");
        var domainAssembly = Assembly.Load("ExxerAI.Domain");

        var applicationTypes = Types.InAssembly(applicationAssembly)
            .That().AreClasses().Or().AreInterfaces().GetTypes()
            .Where(t => t.IsPublic)
            .ToList();

        var domainTypes = Types.InAssembly(domainAssembly)
            .That().AreClasses().Or().AreInterfaces().GetTypes()
            .Where(t => t.IsPublic)
            .ToList();

        var allRelevantTypes = new List<Type>();
        allRelevantTypes.AddRange(applicationTypes);
        allRelevantTypes.AddRange(domainTypes);

        var duplicatedTypeNames = allRelevantTypes
            .GroupBy(t => t.Name)
            .Where(g => g.Count() > 1 && g.Any(t => t.Assembly == applicationAssembly) && g.Any(t => t.Assembly == domainAssembly))
            .Select(g => g.Key)
            .ToList();

        var violations = new List<string>();

        foreach (var typeName in duplicatedTypeNames)
        {
            var typesWithSameName = allRelevantTypes.Where(t => t.Name == typeName).ToList();
            _logger.LogError($"Detected potential duplicate type name '{typeName}' across assemblies:");

            foreach (var type in typesWithSameName)
            {
                _logger.LogError($"  - {type.FullName}, Assembly: {type.Assembly.GetName().Name}");
            }

            // Further inspect properties for duplication
            var applicationVersion = typesWithSameName.FirstOrDefault(t => t.Assembly == applicationAssembly);
            var domainVersion = typesWithSameName.FirstOrDefault(t => t.Assembly == domainAssembly);

            if (applicationVersion != null && domainVersion != null)
            {
                var applicationProperties = applicationVersion.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new PropertyInfoLite(p.Name, p.PropertyType.Name))
                    .ToList();
                var domainProperties = domainVersion.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new PropertyInfoLite(p.Name, p.PropertyType.Name))
                    .ToList();

                var commonProperties = applicationProperties
                    .Intersect(domainProperties)
                    .ToList();

                if (commonProperties.Any())
                {
                    violations.Add($"Type '{typeName}' has {commonProperties.Count} common properties between '{applicationVersion.Assembly.GetName().Name}' and '{domainVersion.Assembly.GetName().Name}'. This indicates duplication.");
                    _logger.LogError($"  Common properties for '{typeName}':");
                    foreach (var prop in commonProperties)
                    {
                        _logger.LogError($"    - {prop.Name} ({prop.PropertyType})");
                    }
                }
            }
        }

        if (violations.Any())
        {
            _logger.LogError("Architecture Violation: Duplicated types and/or properties found across Application and Domain layers.");
            Assert.Fail(string.Join(Environment.NewLine, violations));
        }
        else
        {
            _logger.LogInformation("PASSED: No significant type duplication detected between Application and Domain layers.");
        }
    }

    private class PropertyInfoLite
    {
        public string Name { get; }
        public string PropertyType { get; }

        public PropertyInfoLite(string name, string propertyType)
        {
            Name = name;
            PropertyType = propertyType;
        }

        public override bool Equals(object? obj)
        {
            return obj is PropertyInfoLite other &&
                   Name == other.Name &&
                   PropertyType == other.PropertyType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, PropertyType);
        }
    }

    /// <summary>
    /// Comprehensive method to log all duplicate classes across ALL projects in the solution.
    /// This method provides a complete report of class name conflicts that need resolution.
    /// </summary>
    [Fact]
    public void LogAllDuplicatedClassesAcrossAllProjects()
    {
        _logger.LogInformation("=".PadRight(80, '='));
        _logger.LogInformation("COMPREHENSIVE DUPLICATE CLASS ANALYSIS ACROSS ALL PROJECTS");
        _logger.LogInformation("=".PadRight(80, '='));

        var allAssemblies = new Dictionary<string, Assembly>();
        var allTypes = new List<(Type Type, string AssemblyName)>();

        // Load all production assemblies
        foreach (var assemblyData in ProductionAssemblies)
        {
            var assemblyName = assemblyData[0].ToString()!;
            try
            {
                var assembly = Assembly.Load(assemblyName);
                allAssemblies[assemblyName] = assembly;

                var types = assembly.GetTypes()
                    .Where(t => t.IsClass || t.IsInterface)
                    .Where(t => t.IsPublic || t.IsNestedPublic)
                    .Where(t => !t.Name.Contains("TypeInference"))
                    .Where(t => !t.Name.Contains("_Imports"))
                    .Where(t => !t.Name.Contains("<"))  // Exclude compiler-generated types
                    .Where(t => !t.Namespace?.Contains("__Blazor") == true)
                    .ToList();

                foreach (var type in types)
                {
                    allTypes.Add((type, assemblyName));
                }

                _logger.LogInformation($"✅ Loaded {assemblyName}: {types.Count} types");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"⚠️  Could not load {assemblyName}: {ex.Message}");
            }
        }

        _logger.LogInformation($"\n📊 TOTAL ANALYSIS: {allTypes.Count} types across {allAssemblies.Count} assemblies");

        // Group by class name to find duplicates
        var duplicateGroups = allTypes
            .GroupBy(t => t.Type.Name)
            .Where(g => g.Count() > 1)
            .OrderBy(g => g.Key)
            .ToList();

        _logger.LogInformation($"\n🔍 FOUND {duplicateGroups.Count} DUPLICATE CLASS NAMES");
        _logger.LogInformation("-".PadRight(80, '-'));

        var totalConflicts = 0;
        var conflictsByCategory = new Dictionary<string, List<string>>();

        foreach (var group in duplicateGroups)
        {
            var className = group.Key;
            var instances = group.ToList();

            // Check if duplicates are across different assemblies (real conflicts)
            var assemblyGroups = instances.GroupBy(i => i.AssemblyName).ToList();
            if (assemblyGroups.Count > 1)
            {
                totalConflicts++;
                _logger.LogError($"\n🚨 DUPLICATE CLASS: {className}");

                foreach (var asmGroup in assemblyGroups)
                {
                    var assemblyName = asmGroup.Key;
                    var typesInAssembly = asmGroup.ToList();

                    foreach (var (type, _) in typesInAssembly)
                    {
                        var category = GetTypeCategory(type.Namespace ?? "Unknown");
                        if (!conflictsByCategory.ContainsKey(category))
                            conflictsByCategory[category] = new List<string>();

                        conflictsByCategory[category].Add($"{className} ({assemblyName})");

                        _logger.LogError($"   📍 {type.FullName}");
                        _logger.LogError($"       Assembly: {assemblyName}");
                        _logger.LogError($"       Category: {category}");

                        // Show properties for context
                        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Take(5)  // Show first 5 properties
                            .Select(p => $"{p.Name}: {p.PropertyType.Name}")
                            .ToList();

                        if (properties.Any())
                        {
                            _logger.LogError($"       Properties: {string.Join(", ", properties)}{(type.GetProperties().Length > 5 ? "..." : "")}");
                        }
                    }
                }

                // Suggest resolution strategy
                var suggestedResolution = GetResolutionSuggestion(className, instances);
                _logger.LogInformation($"   💡 SUGGESTED RESOLUTION: {suggestedResolution}");
            }
        }

        // Summary by category
        _logger.LogInformation("\n📋 CONFLICTS BY CATEGORY:");
        _logger.LogInformation("-".PadRight(50, '-'));

        foreach (var category in conflictsByCategory.OrderBy(kv => kv.Key))
        {
            _logger.LogError($"🏷️  {category.Key}: {category.Value.Count} conflicts");
            foreach (var conflict in category.Value.Take(3)) // Show first 3 examples
            {
                _logger.LogError($"     • {conflict}");
            }
            if (category.Value.Count > 3)
            {
                _logger.LogError($"     ... and {category.Value.Count - 3} more");
            }
        }

        // Final summary
        _logger.LogInformation("\n" + "=".PadRight(80, '='));
        _logger.LogInformation($"🎯 SUMMARY: {totalConflicts} class name conflicts require resolution");
        _logger.LogInformation($"📊 Categories affected: {conflictsByCategory.Keys.Count}");
        _logger.LogInformation($"🔧 Action required: Review and consolidate duplicate classes");
        _logger.LogInformation("=".PadRight(80, '='));

        // This test is informational - we'll assert based on expectations
        // For now, let's not fail the test but provide comprehensive logging
        if (totalConflicts > 0)
        {
            _logger.LogWarning($"⚠️  INFORMATIONAL: Found {totalConflicts} class name conflicts that should be resolved for better architecture");
        }
    }

    /// <summary>
    /// Categorizes types based on their namespace to help organize cleanup efforts.
    /// </summary>
    private static string GetTypeCategory(string namespaceName)
    {
        return namespaceName switch
        {
            var ns when ns.Contains("CubeXplorer") => "CubeXplorer",
            var ns when ns.Contains("A2AHub") || ns.Contains("A2A") => "A2A Communication",
            var ns when ns.Contains("DocumentIngestion") || ns.Contains("Document") => "Document Processing",
            var ns when ns.Contains("Entities") => "Domain Entities",
            var ns when ns.Contains("ValueObjects") => "Value Objects",
            var ns when ns.Contains("Interfaces") => "Interfaces",
            var ns when ns.Contains("Services") => "Services",
            var ns when ns.Contains("Models") => "Models",
            var ns when ns.Contains("Infrastructure") => "Infrastructure",
            var ns when ns.Contains("Application") => "Application",
            var ns when ns.Contains("Domain") => "Domain",
            var ns when ns.Contains("API") || ns.Contains("Api") => "API Layer",
            var ns when ns.Contains("UI") => "UI Layer",
            _ => "Other"
        };
    }

    /// <summary>
    /// Provides resolution suggestions based on the class name and instances.
    /// </summary>
    private static string GetResolutionSuggestion(string className, List<(Type Type, string AssemblyName)> instances)
    {
        var assemblies = instances.Select(i => i.AssemblyName).Distinct().ToList();

        return className switch
        {
            var name when name.Contains("Document") && assemblies.Contains("ExxerAI.Domain") =>
                "Keep Domain version, update references in other assemblies",
            var name when name.Contains("EIAAgent") && assemblies.Contains("ExxerAI.Domain") =>
                "Keep Domain version, move interfaces to Application layer",
            var name when name.Contains("Options") || name.Contains("Config") =>
                "Consolidate into Infrastructure configuration classes",
            var name when name.Contains("Result") && assemblies.Contains("ExxerAI.Domain") =>
                "Keep Domain value objects, remove duplicates elsewhere",
            var name when assemblies.Contains("ExxerAI.Domain") =>
                "Prefer Domain layer version, update references",
            _ => "Manual review required - determine authoritative version"
        };
    }
}