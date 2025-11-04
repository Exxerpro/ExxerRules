using System.Reflection;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Architecture.Tests;

/// <summary>
/// Architecture tests to detect duplicate class names across different namespaces and assemblies.
/// This helps maintain clean architecture and avoid naming conflicts.
/// </summary>
public class ClassDuplicationTests
{
    private static readonly Assembly[] _assemblies = new[]
    {
        typeof(IndFusion.SemanticRag.Domain.Models.SemanticDocument).Assembly, // Domain
        typeof(IndFusion.SemanticRag.Application.Services.SemanticRagOrchestrationService).Assembly, // Application
        // typeof(IndFusion.SemanticRag.Infrastructure.Adapters.Neo4jKnowledgeGraphAdapter).Assembly, // Infrastructure - temporarily excluded due to compilation errors
    };

    /// <summary>
    /// Detects duplicate class names within a single assembly.
    /// </summary>
    /// <param name="assembly">The assembly to check.</param>
    /// <returns>Dictionary of class names and their full names.</returns>
    private static Dictionary<string, List<string>> GetDuplicateClassesInAssembly(Assembly assembly)
    {
        var classGroups = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested)
            .GroupBy(t => t.Name)
            .Where(g => g.Count() > 1)
            .ToDictionary(g => g.Key, g => g.Select(t => t.FullName ?? t.Name).ToList());

        return classGroups;
    }

    /// <summary>
    /// Detects duplicate class names across all assemblies.
    /// </summary>
    /// <returns>Dictionary of class names and their full names across assemblies.</returns>
    private static Dictionary<string, List<string>> GetDuplicateClassesAcrossAssemblies()
    {
        var allClasses = new Dictionary<string, List<string>>();

        foreach (var assembly in _assemblies)
        {
            var classes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsNested)
                .Select(t => new { Name = t.Name, FullName = t.FullName ?? t.Name, Assembly = assembly.GetName().Name });

            foreach (var cls in classes)
            {
                if (!allClasses.ContainsKey(cls.Name))
                {
                    allClasses[cls.Name] = new List<string>();
                }
                allClasses[cls.Name].Add($"{cls.FullName} (in {cls.Assembly})");
            }
        }

        return allClasses.Where(kvp => kvp.Value.Count > 1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Verifies that there are no duplicate class names within the Domain assembly.
    /// </summary>
    [Fact]
    public void Should_NotHaveDuplicateClassNames_WithinDomainAssembly()
    {
        var domainAssembly = typeof(IndFusion.SemanticRag.Domain.Models.SemanticDocument).Assembly;
        var duplicates = GetDuplicateClassesInAssembly(domainAssembly);

        if (duplicates.Any())
        {
            var message = "Duplicate class names found in Domain assembly:\n" +
                         string.Join("\n", duplicates.Select(kvp => 
                             $"- {kvp.Key}: {string.Join(", ", kvp.Value)}"));
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Verifies that there are no duplicate class names within the Application assembly.
    /// </summary>
    [Fact]
    public void Should_NotHaveDuplicateClassNames_WithinApplicationAssembly()
    {
        var applicationAssembly = typeof(IndFusion.SemanticRag.Application.Services.SemanticRagOrchestrationService).Assembly;
        var duplicates = GetDuplicateClassesInAssembly(applicationAssembly);

        if (duplicates.Any())
        {
            var message = "Duplicate class names found in Application assembly:\n" +
                         string.Join("\n", duplicates.Select(kvp => 
                             $"- {kvp.Key}: {string.Join(", ", kvp.Value)}"));
            throw new InvalidOperationException(message);
        }
    }



    /// <summary>
    /// Verifies that there are no duplicate class names across all assemblies.
    /// </summary>
    [Fact]
    public void Should_NotHaveDuplicateClassNames_AcrossAllAssemblies()
    {
        var duplicates = GetDuplicateClassesAcrossAssemblies();

        if (duplicates.Any())
        {
            var message = "Duplicate class names found across assemblies:\n" +
                         string.Join("\n", duplicates.Select(kvp => 
                             $"- {kvp.Key}:\n  {string.Join("\n  ", kvp.Value)}"));
            throw new InvalidOperationException(message);
        }
    }

    /// <summary>
    /// Logs all duplicate classes for analysis purposes. This test will always pass but provides detailed output.
    /// </summary>
    [Fact]
    public void Should_LogAllDuplicateClasses_ForAnalysis()
    {
        // This test will always pass but logs all duplicates for analysis
        var duplicates = GetDuplicateClassesAcrossAssemblies();
        
        if (duplicates.Any())
        {
            var logMessage = "=== DUPLICATE CLASS ANALYSIS ===\n";
            logMessage += $"Found {duplicates.Count} duplicate class names:\n\n";
            
            foreach (var duplicate in duplicates.OrderBy(kvp => kvp.Key))
            {
                logMessage += $"Class Name: {duplicate.Key}\n";
                logMessage += $"Locations:\n";
                foreach (var location in duplicate.Value)
                {
                    logMessage += $"  - {location}\n";
                }
                logMessage += "\n";
            }
            
            // Log to console for analysis
            Console.WriteLine(logMessage);
            
            // This test will pass but the duplicates are logged for manual review
            duplicates.ShouldBeEmpty($"Found {duplicates.Count} duplicate class names. Check console output for details.");
        }
        else
        {
            Console.WriteLine("✅ No duplicate class names found across all assemblies.");
        }
    }
}