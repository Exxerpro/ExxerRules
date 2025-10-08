using IndTrace.Application.Generic.Commands.Create;

namespace IndTrace.Agregation.Dependices.Generic.Helpers;

/// <summary>
/// Generic test command for create operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class TestGenericCreateCommand<TEntity> : ICreateCommand<TEntity> where TEntity : class, new()
{
    // Create commands typically don't need additional properties beyond the entity
}
