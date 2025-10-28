using System.Reflection;
using NetArchTest.Rules;

namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Utility to discover services and their test coverage status
/// </summary>
public static class ServiceDiscoveryUtility
{
    public static void DiscoverServicesAndTestCoverage()
    {
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("SERVICE DISCOVERY AND TEST COVERAGE ANALYSIS");
        Console.WriteLine("=".PadRight(80, '='));

        // Check Application services
        AnalyzeServicesInAssembly("ExxerAI.Application", "ExxerAI.Application.Tests", "Application");
        
        // Check Infrastructure services
        AnalyzeServicesInAssembly("ExxerAI.Infrastructure", "ExxerAI.Infrastructure.Tests", "Infrastructure");
    }

    private static void AnalyzeServicesInAssembly(string serviceAssemblyName, string testAssemblyName, string layerName)
    {
        Console.WriteLine($"\n🔍 ANALYZING {layerName.ToUpper()} LAYER");
        Console.WriteLine("-".PadRight(60, '-'));

        try
        {
            // Load service assembly
            var serviceAssembly = Assembly.Load(serviceAssemblyName);
            Console.WriteLine($"✅ Loaded {serviceAssemblyName}");

            // Find all service classes
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
                .Where(t => t.Namespace?.Contains("Services") == true)
                .OrderBy(t => t.Name)
                .ToList();

            Console.WriteLine($"📋 Found {serviceClasses.Count} service classes:");
            foreach (var service in serviceClasses)
            {
                Console.WriteLine($"   • {service.Name} ({service.Namespace})");
            }

            // Try to load test assembly
            try
            {
                var testAssembly = Assembly.Load(testAssemblyName);
                Console.WriteLine($"✅ Loaded {testAssemblyName}");

                var testClasses = Types.InAssembly(testAssembly)
                    .That()
                    .AreClasses()
                    .And()
                    .HaveNameEndingWith("Tests")
                    .GetTypes()
                    .ToList();

                Console.WriteLine($"🧪 Found {testClasses.Count} test classes");

                // Check coverage
                var missingTests = new List<string>();
                foreach (var serviceClass in serviceClasses)
                {
                    var expectedTestClassName = $"{serviceClass.Name}Tests";
                    var foundTestClass = testClasses.FirstOrDefault(t => t.Name == expectedTestClassName);

                    if (foundTestClass == null)
                    {
                        missingTests.Add(serviceClass.Name);
                        Console.WriteLine($"❌ MISSING: {serviceClass.Name} -> Expected: {expectedTestClassName}");
                    }
                    else
                    {
                        Console.WriteLine($"✅ HAS TEST: {serviceClass.Name} -> {foundTestClass.Name}");
                    }
                }

                // Summary
                var coverage = serviceClasses.Count > 0 
                    ? (double)(serviceClasses.Count - missingTests.Count) / serviceClasses.Count * 100 
                    : 100;

                Console.WriteLine($"\n📊 {layerName} Coverage: {coverage:F1}% ({serviceClasses.Count - missingTests.Count}/{serviceClasses.Count})");
                
                if (missingTests.Any())
                {
                    Console.WriteLine($"🚨 MISSING TESTS ({missingTests.Count}):");
                    foreach (var missing in missingTests)
                    {
                        Console.WriteLine($"   • {missing}Tests");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Could not load test assembly {testAssemblyName}: {ex.Message}");
                Console.WriteLine($"🚨 ALL SERVICES MISSING TESTS ({serviceClasses.Count}):");
                foreach (var service in serviceClasses)
                {
                    Console.WriteLine($"   • {service.Name}Tests");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Could not load service assembly {serviceAssemblyName}: {ex.Message}");
        }
    }
}