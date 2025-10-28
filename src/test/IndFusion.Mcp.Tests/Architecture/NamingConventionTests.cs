using System.Reflection;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.Architecture;

/// <summary>
/// Architecture tests to enforce consistent naming conventions.
/// </summary>
public class NamingConventionTests
{
	/// <summary>
	/// Tests that all interfaces start with 'I'.
	/// </summary>
	[Fact]
	public void AllInterfaces_ShouldStartWithI()
	{
		// Arrange
		var assembly = typeof(IndFusion.Mcp.Core.Abstractions.ILintingService).Assembly;
		var allInterfaces = assembly.GetTypes().Where(t => t.IsInterface).ToList();
		
		// Act
		var badlyNamedInterfaces = allInterfaces
			.Where(i => !i.Name.StartsWith("I"))
			.Select(i => i.FullName)
			.ToList();
		
		// Assert
		badlyNamedInterfaces.ShouldBeEmpty($"All interfaces should start with 'I': {string.Join(", ", badlyNamedInterfaces)}");
	}
	
	/// <summary>
	/// Verifies that async methods in service classes follow the naming convention of ending with 'Async'.
	/// MCP tool methods are exempt as they follow a different naming pattern.
	/// </summary>
	[Fact]
	public void ServiceAsyncMethods_ShouldHaveAsyncSuffix()
	{
		// Arrange
		var assembly = typeof(IndFusion.Mcp.Core.Abstractions.ILintingService).Assembly;
		var serviceTypes = assembly.GetTypes()
			.Where(t => t.Namespace?.Contains(".Services") == true || 
			           t.Namespace?.Contains(".Abstractions") == true)
			.ToList();
		
		// Act - Check only service classes for async naming
		var badlyNamedAsyncMethods = serviceTypes
			.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
			.Where(m => m.ReturnType.Name.Contains("Task") && 
			           !m.Name.EndsWith("Async") && 
			           !m.Name.StartsWith("get_") && 
			           !m.Name.StartsWith("set_") &&
			           !m.Name.Contains("__") &&
			           m.DeclaringType?.IsInterface != true)
			.Select(m => $"{m.DeclaringType?.FullName}.{m.Name}")
			.ToList();
		
		// Assert
		badlyNamedAsyncMethods.ShouldBeEmpty($"Service async methods should end with 'Async': {string.Join(", ", badlyNamedAsyncMethods)}");
	}
}
