using Xunit;

namespace IndFusion.SemanticRag.Tests.System.Infrastructure.Fixtures;

/// <summary>
/// xUnit collection definition for system tests.
/// Ensures container fixtures are shared across test classes in the collection.
/// Containers are started once per collection and disposed after all tests complete.
/// </summary>
[CollectionDefinition("System")]
public class SystemTestCollection : ICollectionFixture<QdrantContainerFixture>, ICollectionFixture<Neo4jContainerFixture>
{
	// Collection definition only - no implementation needed
}

