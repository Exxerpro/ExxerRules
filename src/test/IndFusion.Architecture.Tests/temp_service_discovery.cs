using System.Reflection;
using NetArchTest.Rules;

namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Temporary test to discover services that need test coverage
/// </summary>
public class TempServiceDiscovery
{
    [Fact]
    public void DiscoverServicesNeedingTests()
    {
        // Load Application assembly
        var applicationAssembly = Assembly.Load("ExxerAI.Application");
        
        // Find all service classes in Application layer
        var serviceClasses = Types.InAssembly(applicationAssembly)
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

        // Output all found services
        foreach (var service in serviceClasses)
        {
            Console.WriteLine($"Found service: {service.FullName}");
        }
        
        Assert.True(serviceClasses.Count > 0, "Should find some services");
    }
}