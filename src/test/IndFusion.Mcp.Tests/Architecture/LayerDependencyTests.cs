using System.Reflection;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.Architecture;

/// <summary>
/// Architecture tests to enforce clean architecture layer separation.
/// Ensures domain layer doesn't depend on infrastructure.
/// </summary>
public class LayerDependencyTests
{
	/// <summary>
	/// Ensures that domain layer types do not reference infrastructure layer types.
	/// This maintains clean architecture separation and prevents circular dependencies.
	/// </summary>
	[Fact]
	public void DomainLayer_ShouldNotDependOn_InfrastructureLayer()
	{
		// Arrange
		var coreAssembly = typeof(IndFusion.Mcp.Core.Abstractions.ILintingService).Assembly;
		var domainTypes = coreAssembly.GetTypes()
			.Where(t => t.Namespace?.Contains(".Abstractions") == true || t.Namespace?.Contains(".Models") == true)
			.ToList();
		
		// Act - Check if domain types reference infrastructure types
		var infrastructureDependencies = domainTypes
			.SelectMany(t => t.GetProperties()
				.Select(p => p.PropertyType)
				.Concat(t.GetMethods().SelectMany(m => m.GetParameters().Select(p => p.ParameterType)))
				.Where(type => type.Namespace?.Contains(".Infrastructure") == true || 
				              type.Namespace?.Contains(".Services") == true))
			.Distinct()
			.ToList();
		
		// Assert
		infrastructureDependencies.ShouldBeEmpty("Domain layer should not depend on infrastructure layer");
	}
}
