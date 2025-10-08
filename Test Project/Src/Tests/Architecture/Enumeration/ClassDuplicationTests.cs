using System.Reflection;
using Shouldly;
using NetArchTest.Rules;

namespace Architecture.Tests.Enumeration;
/// <summary>
/// Represents the ClassDuplicationTests.
/// </summary>

public class ClassDuplicationTests
{
    /// <summary>
    /// Executes All_Classes_Should_Not_Have_Duplicate_Names_In_Different_Namespaces operation.
    /// </summary>
    [Fact]
    public void All_Classes_Should_Not_Have_Duplicate_Names_In_Different_Namespaces()
    {
        // Load all IndTrace assemblies in the current AppDomain
        var indTraceAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => (a.GetName().Name ?? string.Empty).StartsWith("IndTrace."))
            .ToArray();

        // Collect all classes across assemblies
        var types = indTraceAssemblies
            .SelectMany(a => Types.InAssembly(a).That().AreClasses().GetTypes())
            .ToList();

        // Group by class name and check for duplicates in different namespaces
        var duplicates = types.GroupBy(t => t.Name)
            .Where(g => g.Count() > 1) // Find groups with more than one class with the same name
            .Select(g => new { ClassName = g.Key, Namespaces = g.Select(t => t.Namespace).Distinct() })
            .Where(g => g.Namespaces.Count() > 1) // Check if the same class name exists in more than one namespace
            .ToList();

        // Assert that there are no duplicated class names across different namespaces
        duplicates.ShouldBeEmpty("no class name should be duplicated across different namespaces or assemblies");
    }
}
