namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Architecture tests to ensure TDD compliance by detecting missing test coverage for services and classes.
/// Validates that every service has corresponding unit tests following ExxerAI naming conventions.
/// </summary>
public class TestCoverageArchitectureTests
{
    private readonly ILogger<TestCoverageArchitectureTests> _logger;

    /// <summary>
    /// Initializes the test coverage architecture test harness with structured logging.
    /// </summary>
    /// <param name="testOutputHelper">xUnit output helper used to emit test coverage diagnostics.</param>
    public TestCoverageArchitectureTests(ITestOutputHelper testOutputHelper)
    {
        _logger = XUnitLogger.CreateLogger<TestCoverageArchitectureTests>(testOutputHelper);
    }

    /// <summary>
    /// Verifies that all services in the Application layer have corresponding unit tests.
    /// This enforces TDD compliance by ensuring no service is left untested.
    /// </summary>
    [Fact]
    public void All_Application_Services_Should_Have_Unit_Tests()
    {
        _logger.LogInformation("=".PadRight(80, '='));
        _logger.LogInformation("CHECKING APPLICATION SERVICES TEST COVERAGE");
        _logger.LogInformation("=".PadRight(80, '='));

        // Load Application and Test assemblies using safer assembly loading
        var applicationAssembly = GetOrLoadAssembly("ExxerAI.Application");
        var testAssembly = GetOrLoadAssembly("ExxerAI.Application.Tests");
        
        if (testAssembly == null)
        {
            _logger.LogError("❌ Could not load test assembly ExxerAI.Application.Tests - skipping test coverage check");
            Assert.Fail("Could not load ExxerAI.Application.Tests assembly. Ensure the project is built and referenced properly.");
            return;
        }

        // Find all service classes in Application layer
        var serviceClasses = Types.InAssembly(applicationAssembly)
            .That()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .And()
            .HaveNameEndingWith("Service")
            .And()
            .DoNotHaveNameMatching(".*Base.*") // Exclude base classes
            .And()
            .DoNotHaveNameMatching(".*Abstract.*") // Exclude abstract classes
            .GetTypes()
            .Where(t => t.Namespace?.Contains("Services") == true)
            .OrderBy(t => t.Name)
            .ToList();

        // Find all test classes in Test assembly
        var testClasses = Types.InAssembly(testAssembly)
            .That()
            .AreClasses()
            .And()
            .HaveNameEndingWith("Tests")
            .GetTypes()
            .ToList();

        var missingTests = new List<string>();
        var sutViolations = new List<string>();

        _logger.LogInformation($"Found {serviceClasses.Count} service classes to check");
        _logger.LogInformation($"Found {testClasses.Count} test classes");

        foreach (var serviceClass in serviceClasses)
        {
            var expectedTestClassName = $"{serviceClass.Name}Tests";
            var foundTestClass = testClasses.FirstOrDefault(t => t.Name == expectedTestClassName);

            if (foundTestClass == null)
            {
                missingTests.Add($"{serviceClass.Name} (Expected: {expectedTestClassName})");
                _logger.LogError($"❌ MISSING TEST: {serviceClass.Name} -> Expected: {expectedTestClassName}");
            }
            else
            {
                _logger.LogInformation($"✅ HAS TEST: {serviceClass.Name} -> {foundTestClass.Name}");
                
                // Check for proper SUT (System Under Test) pattern
                var sutCheck = ValidateSystemUnderTestPattern(foundTestClass, serviceClass);
                if (!sutCheck.IsValid)
                {
                    sutViolations.Add($"{serviceClass.Name}: {sutCheck.Violation}");
                    _logger.LogWarning($"⚠️  SUT VIOLATION: {serviceClass.Name} -> {sutCheck.Violation}");
                }
            }
        }

        // Report results
        _logger.LogInformation("\n" + "=".PadRight(80, '='));
        _logger.LogInformation("TEST COVERAGE SUMMARY");
        _logger.LogInformation("=".PadRight(80, '='));
        
        if (missingTests.Any())
        {
            _logger.LogError($"🚨 MISSING TESTS: {missingTests.Count} services without unit tests:");
            foreach (var missing in missingTests)
            {
                _logger.LogError($"   • {missing}");
            }
        }

        if (sutViolations.Any())
        {
            _logger.LogWarning($"⚠️  SUT VIOLATIONS: {sutViolations.Count} test classes with SUT pattern issues:");
            foreach (var violation in sutViolations)
            {
                _logger.LogWarning($"   • {violation}");
            }
        }

        var coveragePercentage = serviceClasses.Count > 0 
            ? (double)(serviceClasses.Count - missingTests.Count) / serviceClasses.Count * 100 
            : 100;

        _logger.LogInformation($"📊 COVERAGE: {coveragePercentage:F1}% ({serviceClasses.Count - missingTests.Count}/{serviceClasses.Count} services have tests)");
        _logger.LogInformation("=".PadRight(80, '='));

        // Assert TDD compliance
        missingTests.ShouldBeEmpty($"TDD Violation: {missingTests.Count} Application services are missing unit tests. " +
            $"Every service must have corresponding unit tests. Missing tests: {string.Join(", ", missingTests)}");
    }

    /// <summary>
    /// Verifies that all infrastructure services have corresponding unit tests.
    /// </summary>
    [Fact]
    public void All_Infrastructure_Services_Should_Have_Unit_Tests()
    {
        _logger.LogInformation("=".PadRight(80, '='));
        _logger.LogInformation("CHECKING INFRASTRUCTURE SERVICES TEST COVERAGE");
        _logger.LogInformation("=".PadRight(80, '='));

        // Load Infrastructure and Test assemblies using safer assembly loading
        var infrastructureAssembly = GetOrLoadAssembly("ExxerAI.Infrastructure");
        var testAssembly = GetOrLoadAssembly("ExxerAI.Infrastructure.Tests");
        
        if (testAssembly == null)
        {
            _logger.LogError("❌ Could not load test assembly ExxerAI.Infrastructure.Tests - skipping test coverage check");
            Assert.Fail("Could not load ExxerAI.Infrastructure.Tests assembly. Ensure the project is built and referenced properly.");
            return;
        }

        // Find all service classes in Infrastructure layer
        var serviceClasses = Types.InAssembly(infrastructureAssembly)
            .That()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .And()
            .HaveNameEndingWith("Service")
            .And()
            .DoNotHaveNameMatching(".*Base.*") // Exclude base classes
            .And()
            .DoNotHaveNameMatching(".*Abstract.*") // Exclude abstract classes
            .GetTypes()
            .OrderBy(t => t.Name)
            .ToList();

        // Find all test classes in Test assembly
        var testClasses = Types.InAssembly(testAssembly)
            .That()
            .AreClasses()
            .And()
            .HaveNameEndingWith("Tests")
            .GetTypes()
            .ToList();

        var missingTests = new List<string>();

        _logger.LogInformation($"Found {serviceClasses.Count} infrastructure service classes to check");
        _logger.LogInformation($"Found {testClasses.Count} test classes");

        foreach (var serviceClass in serviceClasses)
        {
            var expectedTestClassName = $"{serviceClass.Name}Tests";
            var foundTestClass = testClasses.FirstOrDefault(t => t.Name == expectedTestClassName);

            if (foundTestClass == null)
            {
                missingTests.Add($"{serviceClass.Name} (Expected: {expectedTestClassName})");
                _logger.LogError($"❌ MISSING TEST: {serviceClass.Name} -> Expected: {expectedTestClassName}");
            }
            else
            {
                _logger.LogInformation($"✅ HAS TEST: {serviceClass.Name} -> {foundTestClass.Name}");
            }
        }

        var coveragePercentage = serviceClasses.Count > 0 
            ? (double)(serviceClasses.Count - missingTests.Count) / serviceClasses.Count * 100 
            : 100;

        _logger.LogInformation($"\n📊 INFRASTRUCTURE COVERAGE: {coveragePercentage:F1}% ({serviceClasses.Count - missingTests.Count}/{serviceClasses.Count} services have tests)");

        // Assert TDD compliance for Infrastructure services
        missingTests.ShouldBeEmpty($"TDD Violation: {missingTests.Count} Infrastructure services are missing unit tests. " +
            $"Every service must have corresponding unit tests. Missing tests: {string.Join(", ", missingTests)}");
    }

    /// <summary>
    /// Comprehensive report of all missing test coverage across all service layers.
    /// This test provides actionable information for creating missing tests.
    /// </summary>
    [Fact]
    public void Generate_Comprehensive_Missing_Test_Coverage_Report()
    {
        _logger.LogInformation("=".PadRight(80, '='));
        _logger.LogInformation("COMPREHENSIVE MISSING TEST COVERAGE REPORT");
        _logger.LogInformation("=".PadRight(80, '='));

        var allMissingTests = new Dictionary<string, List<(Type ServiceClass, string ExpectedTestName)>>();

        // Check Application services
        var applicationResults = CheckTestCoverageForAssembly("ExxerAI.Application", "ExxerAI.Application.Tests", "Application");
        if (applicationResults.MissingTests.Any())
        {
            allMissingTests["Application"] = applicationResults.MissingTests;
        }

        // Check Infrastructure services  
        var infrastructureResults = CheckTestCoverageForAssembly("ExxerAI.Infrastructure", "ExxerAI.Infrastructure.Tests", "Infrastructure");
        if (infrastructureResults.MissingTests.Any())
        {
            allMissingTests["Infrastructure"] = infrastructureResults.MissingTests;
        }

        // Generate actionable test creation guide
        if (allMissingTests.Any())
        {
            _logger.LogInformation("\n📋 ACTIONABLE TEST CREATION GUIDE:");
            _logger.LogInformation("-".PadRight(60, '-'));

            var totalMissing = allMissingTests.Values.SelectMany(x => x).Count();
            
            foreach (var layerGroup in allMissingTests)
            {
                var layerName = layerGroup.Key;
                var missingTests = layerGroup.Value;

                _logger.LogError($"\n🏗️  {layerName.ToUpper()} LAYER - {missingTests.Count} missing tests:");
                
                foreach (var (serviceClass, expectedTestName) in missingTests)
                {
                    var interfaceName = $"I{serviceClass.Name}";
                    var testFilePath = $"ExxerAI.{layerName}.Tests/Services/{expectedTestName}.cs";
                    
                    _logger.LogError($"   📄 {expectedTestName}");
                    _logger.LogError($"      Service: {serviceClass.FullName}");
                    _logger.LogError($"      Interface: {interfaceName}");
                    _logger.LogError($"      Test File: {testFilePath}");
                    _logger.LogError($"      SUT Pattern: private readonly {interfaceName} _service;");
                    _logger.LogError("");
                }
            }

            // Provide template for test creation
            _logger.LogInformation("\n🔧 TEST TEMPLATE EXAMPLE:");
            _logger.LogInformation("-".PadRight(40, '-'));
            _logger.LogInformation("namespace ExxerAI.Application.Tests.Services;");
            _logger.LogInformation("");
            _logger.LogInformation("public class [ServiceName]Tests");
            _logger.LogInformation("{");
            _logger.LogInformation("    private readonly I[ServiceName] _service;");
            _logger.LogInformation("    private readonly ILogger<[ServiceName]> _logger;");
            _logger.LogInformation("");
            _logger.LogInformation("    public [ServiceName]Tests(ITestOutputHelper output)");
            _logger.LogInformation("    {");
            _logger.LogInformation("        _logger = XUnitLogger.CreateLogger<[ServiceName]>(output);");
            _logger.LogInformation("        _service = new [ServiceName](_logger /* + dependencies */);");
            _logger.LogInformation("    }");
            _logger.LogInformation("");
            _logger.LogInformation("    [Fact]");
            _logger.LogInformation("    public async Task Method_Should_ReturnSuccess_When_ValidInput()");
            _logger.LogInformation("    {");
            _logger.LogInformation("        // Arrange - Setup test data");
            _logger.LogInformation("        // Act - Call the method under test");
            _logger.LogInformation("        // Assert - Verify expected behavior");
            _logger.LogInformation("    }");
            _logger.LogInformation("}");

            _logger.LogInformation($"\n🎯 PRIORITY ACTION: Create {totalMissing} missing test files to achieve 100% TDD compliance");
        }
        else
        {
            _logger.LogInformation("🎉 EXCELLENT: All services have corresponding unit tests!");
        }

        _logger.LogInformation("=".PadRight(80, '='));

        // This is an informational test - log findings for action planning
        var totalMissingCount = allMissingTests.Values.SelectMany(x => x).Count();
        if (totalMissingCount > 0)
        {
            _logger.LogWarning($"⚠️  INFORMATIONAL: Found {totalMissingCount} services without unit tests that need TDD compliance");
        }
    }

    /// <summary>
    /// Validates that a test class follows proper System Under Test (SUT) patterns.
    /// </summary>
    private (bool IsValid, string Violation) ValidateSystemUnderTestPattern(Type testClass, Type serviceClass)
    {
        // Check for SUT field (should be _service or _serviceName pattern)
        var fields = testClass.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var expectedInterfaceName = $"I{serviceClass.Name}";
        
        // Look for SUT field patterns
        var sutField = fields.FirstOrDefault(f => 
            f.Name == "_service" || 
            f.Name == $"_{serviceClass.Name.ToCamelCase()}" ||
            f.FieldType.Name == expectedInterfaceName);

        if (sutField == null)
        {
            return (false, $"Missing SUT field. Expected field like '_service' of type '{expectedInterfaceName}'");
        }

        // Check if SUT uses interface type (TDD best practice)
        if (!sutField.FieldType.IsInterface)
        {
            return (false, $"SUT field '{sutField.Name}' should use interface type '{expectedInterfaceName}', not concrete type");
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// Checks test coverage for a specific assembly pair.
    /// </summary>
    private TestCoverageResult CheckTestCoverageForAssembly(string serviceAssemblyName, string testAssemblyName, string layerName)
    {
        try
        {
            var serviceAssembly = GetOrLoadAssembly(serviceAssemblyName);
            var testAssembly = GetOrLoadAssembly(testAssemblyName);
            
            if (testAssembly == null)
            {
                _logger.LogError($"❌ Could not load test assembly {testAssemblyName} for {layerName} layer");
                return new TestCoverageResult
                {
                    LayerName = layerName,
                    TotalServices = 0,
                    TestedServices = 0,
                    MissingTests = new List<(Type ServiceClass, string ExpectedTestName)>()
                };
            }

            var serviceClasses = Types.InAssembly(serviceAssembly)
                .That()
                .AreClasses()
                .And()
                .AreNotAbstract()
                .And()
                .HaveNameEndingWith("Service")
                .And()
                .DoNotHaveNameMatching(".*Base.*")
                .And()
                .DoNotHaveNameMatching(".*Abstract.*")
                .GetTypes()
                .OrderBy(t => t.Name)
                .ToList();

            var testClasses = Types.InAssembly(testAssembly)
                .That()
                .AreClasses()
                .And()
                .HaveNameEndingWith("Tests")
                .GetTypes()
                .ToList();

            var missingTests = new List<(Type ServiceClass, string ExpectedTestName)>();

            foreach (var serviceClass in serviceClasses)
            {
                var expectedTestClassName = $"{serviceClass.Name}Tests";
                var foundTestClass = testClasses.FirstOrDefault(t => t.Name == expectedTestClassName);

                if (foundTestClass == null)
                {
                    missingTests.Add((serviceClass, expectedTestClassName));
                }
            }

            return new TestCoverageResult
            {
                LayerName = layerName,
                TotalServices = serviceClasses.Count,
                TestedServices = serviceClasses.Count - missingTests.Count,
                MissingTests = missingTests
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Could not check {layerName} test coverage: {ex.Message}");
            return new TestCoverageResult
            {
                LayerName = layerName,
                TotalServices = 0,
                TestedServices = 0,
                MissingTests = new List<(Type ServiceClass, string ExpectedTestName)>()
            };
        }
    }

    /// <summary>
    /// Result data for test coverage analysis.
    /// </summary>
    private class TestCoverageResult
    {
        public string LayerName { get; set; } = string.Empty;
        public int TotalServices { get; set; }
        public int TestedServices { get; set; }
        public List<(Type ServiceClass, string ExpectedTestName)> MissingTests { get; set; } = new();
        public double CoveragePercentage => TotalServices > 0 ? (double)TestedServices / TotalServices * 100 : 100;
    }

    /// <summary>
    /// Safely loads an assembly by name, trying multiple approaches to handle test execution context.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to load</param>
    /// <returns>The loaded assembly, or null if it cannot be found</returns>
    private Assembly? GetOrLoadAssembly(string assemblyName)
    {
        try
        {
            // First, try to find the assembly in the current app domain
            var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name?.Equals(assemblyName, StringComparison.OrdinalIgnoreCase) == true);
            
            if (loadedAssembly != null)
            {
                _logger.LogDebug($"✅ Found {assemblyName} in current app domain");
                return loadedAssembly;
            }

            // Try Assembly.Load (works if assembly is in GAC or already referenced)
            try
            {
                var assembly = Assembly.Load(assemblyName);
                _logger.LogDebug($"✅ Loaded {assemblyName} via Assembly.Load");
                return assembly;
            }
            catch (FileNotFoundException)
            {
                _logger.LogDebug($"❌ Assembly.Load failed for {assemblyName}, trying file-based loading");
            }

            // Try to find the assembly file in common test locations
            var possiblePaths = new[]
            {
                $@"bin\Debug\net10.0\{assemblyName}.dll",
                $@"..\{assemblyName}\bin\Debug\net10.0\{assemblyName}.dll",
                $@"..\..\{assemblyName}\bin\Debug\net10.0\{assemblyName}.dll",
                $@"..\..\..\{assemblyName}\bin\Debug\net10.0\{assemblyName}.dll",
                $@"tests\{assemblyName}\bin\Debug\net10.0\{assemblyName}.dll"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(Path.GetFullPath(path));
                        _logger.LogDebug($"✅ Loaded {assemblyName} from {path}");
                        return assembly;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug($"❌ Failed to load {assemblyName} from {path}: {ex.Message}");
                    }
                }
            }

            _logger.LogWarning($"⚠️ Could not locate assembly {assemblyName} in any standard locations");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error loading assembly {assemblyName}: {ex.Message}");
            return null;
        }
    }
}

/// <summary>
/// Extension methods for string manipulation in test coverage analysis.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input) || !char.IsUpper(input[0]))
            return input;

        return char.ToLowerInvariant(input[0]) + input[1..];
    }
}