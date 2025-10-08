namespace GateWay.Tests.Gateway;
/// <summary>
/// Represents the NonParallelCollectionDefinition.
/// </summary>

[CollectionDefinition(nameof(NonParallelCollectionDefinition), DisableParallelization = true)]

public class NonParallelCollectionDefinition
{
    // This class has no code, and is never instantiated.
    // Its purpose is solely to be the place to apply [CollectionDefinition] and all the ICollectionFixture interfaces.
}
