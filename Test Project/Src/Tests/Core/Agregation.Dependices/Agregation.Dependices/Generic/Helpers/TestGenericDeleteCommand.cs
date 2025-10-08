using IndTrace.Application.Generic.Commands.Delete;

namespace IndTrace.Agregation.Dependices.Generic.Helpers;

/// <summary>
/// Generic test command for delete operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class TestGenericDeleteCommand<TEntity> : IDeleteCommand<TEntity> where TEntity : class, new()
{
    public int Id { get; set; }
}
