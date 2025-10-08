using IndTrace.Domain.Models;
using IndTrace.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IndTrace.Persistence.Repositories;

/// <summary>
/// Provides static helper methods for updating entities in the database context.
/// </summary>
/// <typeparam name="T">The entity type to update.</typeparam>
public static class EntityUpdateHelper<T>
    where T : class
{
    /// <summary>
    /// Updates the specified entity in the context, matching by primary key.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="Result"/> indicating the outcome of the update operation.</returns>
    public static async Task<Result> UpdateAsync(
        IIndTraceDbContext context,
        T entity,
        CancellationToken cancellationToken)
    {
        // Early validation with clear error messages
        var validationResult = ValidateInputs(context, entity);
        if (validationResult.IsFailure)
            return validationResult;

        // Extract primary key with proper error handling
        var keyResult = EntityKeyResolver.GetPrimaryKeyValue(entity, context);
        if (keyResult.IsFailure)
            return Result.WithFailure($"Failed to get primary key: {string.Join(", ", keyResult.Errors)}");

        var keyValue = keyResult.Value;

        // Try to update entity using pattern matching for cleaner flow
        var updateResult = await TryUpdateEntity(context, entity, keyValue, cancellationToken);
        if (updateResult.IsFailure)
            return updateResult;

        // Save changes with proper result handling
        return await SaveChangesWithValidation(context, cancellationToken);
    }

    /// <summary>
    /// Updates specific properties of an entity in the context, matching by primary key.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="entity">The entity with updated property values.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <param name="propertiesToUpdate">The names of the properties to update. If empty, all properties are updated.</param>
    /// <returns>A <see cref="Result"/> indicating the outcome of the update operation.</returns>
    public static async Task<Result> UpdateAsync(
        IIndTraceDbContext context,
        T entity,
        CancellationToken cancellationToken,
        params string[] propertiesToUpdate)
    {
        // Early validation
        var validationResult = ValidateInputs(context, entity);
        if (validationResult.IsFailure)
            return validationResult;

        // Extract primary key
        var keyResult = EntityKeyResolver.GetPrimaryKeyValue(entity, context);
        if (keyResult.IsFailure)
            return Result.WithFailure($"Failed to get primary key: {string.Join(", ", keyResult.Errors)}");

        // Find entity to update
        var entityToUpdate = await FindEntityToUpdate(context, keyResult.Value, cancellationToken);
        if (entityToUpdate is null)
            return Result.WithFailure($"Entity of type {typeof(T).Name} with key '{keyResult.Value}' not found.");

        // Apply updates based on whether specific properties are requested
        var updateMode = propertiesToUpdate?.Length > 0
            ? UpdateMode.Partial
            : UpdateMode.Full;

        var applyResult = ApplyUpdates(context, entityToUpdate, entity, updateMode, propertiesToUpdate);
        if (applyResult.IsFailure)
            return applyResult;

        // Save changes
        return await SaveChangesWithValidation(context, cancellationToken);
    }

    #region Private Helper Methods

    /// <summary>
    /// Validates input parameters.
    /// </summary>
    private static Result ValidateInputs(IIndTraceDbContext? context, T? entity)
    {
        return (context, entity) switch
        {
            (null, _) => Result.WithFailure("Database context cannot be null."),
            (_, null) => Result.WithFailure($"Entity of type {typeof(T).Name} cannot be null."),
            _ => Result.Success()
        };
    }

    /// <summary>
    /// Attempts to update an entity, handling both tracked and untracked scenarios.
    /// </summary>
    private static async Task<Result> TryUpdateEntity(
        IIndTraceDbContext context,
        T entity,
        object? keyValue,
        CancellationToken cancellationToken)
    {
        // Check for already tracked entity
        var trackedEntry = FindTrackedEntity(context, keyValue);

        if (trackedEntry is not null)
        {
            // Update tracked entity
            trackedEntry.CurrentValues.SetValues(entity);
            trackedEntry.State = EntityState.Modified;
            return Result.Success();
        }

        // Entity not tracked, find it in database
        var existing = await context.Set<T>().FindAsync(new[] { keyValue! }, cancellationToken);
        if (existing is null)
            return Result.WithFailure($"Entity of type {typeof(T).Name} with key '{keyValue}' not found in database.");

        // Update untracked entity
        context.Entry(existing).CurrentValues.SetValues(entity);
        context.Entry(existing).State = EntityState.Modified;

        return Result.Success();
    }

    /// <summary>
    /// Finds an already tracked entity by its primary key value.
    /// </summary>
    private static EntityEntry<T>? FindTrackedEntity(IIndTraceDbContext context, object? keyValue)
    {
        return context.ChangeTracker
            .Entries<T>()
            .FirstOrDefault(entry =>
            {
                var trackedKeyResult = EntityKeyResolver.GetPrimaryKeyValue(entry.Entity, context);
                return trackedKeyResult.IsSuccess && Equals(trackedKeyResult.Value, keyValue);
            });
    }

    /// <summary>
    /// Finds an entity to update, checking tracked entities first, then the database.
    /// </summary>
    private static async Task<T?> FindEntityToUpdate(
        IIndTraceDbContext context,
        object? keyValue,
        CancellationToken cancellationToken)
    {
        // Check tracked entities first (avoids database round trip)
        var trackedEntry = FindTrackedEntity(context, keyValue);
        if (trackedEntry is not null)
            return trackedEntry.Entity;

        // Not tracked, load from database
        return await context.Set<T>().FindAsync(new[] { keyValue! }, cancellationToken);
    }

    /// <summary>
    /// Applies updates to an entity based on the update mode.
    /// </summary>
    private static Result ApplyUpdates(
        IIndTraceDbContext context,
        T entityToUpdate,
        T sourceEntity,
        UpdateMode mode,
        string[]? propertiesToUpdate)
    {
        var entry = context.Entry(entityToUpdate);

        if (entry is null)
        {
            return Result.WithFailure("Invalid update mode.");
        }

        return mode switch
        {
            UpdateMode.Full => ApplyFullUpdate(entry, sourceEntity),
            UpdateMode.Partial => ApplyPartialUpdate(entry, sourceEntity, propertiesToUpdate ?? Array.Empty<string>()),
            _ => Result.WithFailure("Invalid update mode.")
        };
    }

    /// <summary>
    /// Applies a full update to all properties.
    /// </summary>
    private static Result ApplyFullUpdate(EntityEntry entry, T sourceEntity)
    {
        try
        {
            entry.CurrentValues.SetValues(sourceEntity);
            entry.State = EntityState.Modified;
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.WithFailure($"Failed to apply full update: {ex.Message}");
        }
    }

    /// <summary>
    /// Applies a partial update to specific properties only.
    /// </summary>
    private static Result ApplyPartialUpdate(EntityEntry entry, T sourceEntity, string[] propertiesToUpdate)
    {
        var entityType = typeof(T);
        var failedProperties = new List<string>();

        foreach (var propertyName in propertiesToUpdate)
        {
            var property = entityType.GetProperty(propertyName);

            if (property is null)
            {
                failedProperties.Add($"Property '{propertyName}' not found on type {entityType.Name}");
                continue;
            }

            try
            {
                var value = property.GetValue(sourceEntity);
                entry.Property(propertyName).CurrentValue = value;
                entry.Property(propertyName).IsModified = true;
            }
            catch (Exception ex)
            {
                failedProperties.Add($"Failed to update property '{propertyName}': {ex.Message}");
            }
        }

        return failedProperties.Count > 0
            ? Result.WithFailure(failedProperties.ToArray())
            : Result.Success();
    }

    /// <summary>
    /// Saves changes with proper validation and error handling.
    /// </summary>
    private static async Task<Result> SaveChangesWithValidation(
        IIndTraceDbContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            var affected = await context.SaveChangesAsync(cancellationToken);

            return affected switch
            {
                0 => Result.WithFailure("No records were updated. Possible concurrency issue or no changes detected."),
                1 => Result.Success(),
                _ => Result.Success() // Multiple records affected (e.g., triggers, cascades)
            };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.WithFailure($"Concurrency conflict: {ex.Message}");
        }
        catch (DbUpdateException ex)
        {
            return Result.WithFailure($"Database update failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.WithFailure($"Unexpected error during save: {ex.Message}");
        }
    }

    #endregion Private Helper Methods

    /// <summary>
    /// Defines the update mode for entity updates.
    /// </summary>
    private enum UpdateMode
    {
        Full,
        Partial
    }
}

//[Fix]
//CLAUDE
//Date: 02/09/2025
//Reason: [READABILITY] - Refactored using modern C# patterns, improved error handling, and clearer method separation
