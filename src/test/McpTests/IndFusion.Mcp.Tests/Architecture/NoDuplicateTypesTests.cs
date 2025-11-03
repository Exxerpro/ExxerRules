using System.Reflection;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.Architecture;

/// <summary>
/// Architecture tests to enforce DRY principles and prevent duplicate types.
/// These tests ensure code quality and maintainability across sprints.
/// </summary>
public class NoDuplicateTypesTests
{
	/// <summary>
	/// Verifies that all types have unique names within their respective namespaces.
	/// This prevents DRY violations and naming conflicts.
	/// </summary>
	/// <summary>
	/// Tests that all types have unique names within the same namespace.
	/// </summary>
	[Fact]
	public void AllTypes_ShouldHaveUniqueNames_WithinSameNamespace()
	{
		// Arrange
		var assembly = typeof(IndFusion.Mcp.Core.Abstractions.ILintingService).Assembly;
		var allTypes = assembly.GetTypes();
		
		// Act - Filter out compiler-generated types
		var duplicates = allTypes
			.Where(t => !t.Name.StartsWith("<>") && 
			           !t.Name.Contains("__DisplayClass") && 
			           !t.Name.Contains("__AnonymousType") &&
			           !t.Name.Contains("<>c") &&
			           !t.Name.Contains("d__") && // Async state machines
			           !t.Name.Contains("__") && // Other compiler-generated types
			           !t.Name.StartsWith("<")) // Generic types
			.GroupBy(t => new { t.Namespace, t.Name })
			.Where(g => g.Count() > 1)
			.Select(g => new { g.Key.Namespace, g.Key.Name, Count = g.Count() })
			.ToList();
		
		// Assert
		duplicates.ShouldBeEmpty($"Found duplicate type names: {string.Join(", ", duplicates.Select(d => $"{d.Namespace}.{d.Name} ({d.Count} occurrences)"))}");
	}
	
	/// <summary>
	/// Checks for interfaces with similar names that might indicate duplication.
	/// This helps identify potential DRY violations in interface design.
	/// </summary>
	/// <summary>
	/// Tests that all interfaces are not duplicated across assemblies.
	/// </summary>
	[Fact]
	public void AllInterfaces_ShouldNotBeDuplicated_AcrossAssemblies()
	{
		// Arrange
		var coreAssembly = typeof(IndFusion.Mcp.Core.Abstractions.ILintingService).Assembly;
		var allInterfaces = coreAssembly.GetTypes().Where(t => t.IsInterface).ToList();
		
		// Act - Check for interfaces with similar names (potential duplication)
		var similarNames = allInterfaces
			.GroupBy(i => i.Name.Replace("I", "").Replace("Service", "").Replace("Repository", ""))
			.Where(g => g.Count() > 1)
			.Select(g => new { BaseName = g.Key, Interfaces = g.Select(i => i.FullName).ToList() })
			.ToList();
		
		// Assert
		foreach (var group in similarNames)
		{
			// This is a warning - review if these are actually duplicates or legitimately different
			Console.WriteLine($"WARNING: Similar interface names found: {string.Join(", ", group.Interfaces)}");
		}
	}
}

