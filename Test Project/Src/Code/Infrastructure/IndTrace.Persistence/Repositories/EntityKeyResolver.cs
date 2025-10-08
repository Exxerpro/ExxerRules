using IndTrace.Persistence.Interfaces;

namespace IndTrace.Persistence.Repositories;

/// <summary>
/// Provides helper methods for resolving primary key values from entities using the EF Core model.
/// </summary>
public static class EntityKeyResolver
{
    /// <summary>
    /// Gets the value of the primary key property for the specified entity.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity instance.</param>
    /// <param name="context">The database context containing the EF Core model.</param>
    /// <returns>The value of the primary key property, or null if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="entity"/> or <paramref name="context"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the entity type or primary key is not defined in the model.</exception>
    public static IndQuestResults.Result<object?> GetPrimaryKeyValue<T>(T entity, IIndTraceDbContext context)
    {
        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate entity key resolver logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Exception handling: avoid catching general Exception, catch specific exceptions where possible. See .NET best practices for exception handling.

        if (entity is null)
        {
            return IndQuestResults.Result<object?>.WithFailure("entity cannot be null");
        }

        if (context is null)
        {
            return IndQuestResults.Result<object?>.WithFailure("context cannot be null");
        }

        var entityType = context.Model.FindEntityType(typeof(T));
        if (entityType is null)
        {
            return IndQuestResults.Result<object?>.WithFailure(
                $"Entity type '{typeof(T).Name}' not found in EF Core model.");
        }

        var key = entityType.FindPrimaryKey();
        if (key is null)
        {
            return IndQuestResults.Result<object?>.WithFailure(
                $"Primary key not defined for entity type '{typeof(T).Name}'.");
        }

        var keyProperty = key.Properties.FirstOrDefault();
        if (keyProperty is null)
        {
            return IndQuestResults.Result<object?>.WithFailure(
                $"Primary key properties missing for entity type '{typeof(T).Name}'.");
        }

        var propertyInfo = typeof(T).GetProperty(keyProperty.Name);
        if (propertyInfo is null)
        {
            return IndQuestResults.Result<object?>.WithFailure(
                $"Property '{keyProperty.Name}' not found on type '{typeof(T).Name}'.");
        }

        //Success result with the primary key value
        return IndQuestResults.Result<object?>.Success(propertyInfo.GetValue(entity));
    }
}
