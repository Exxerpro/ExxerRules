// <copyright file="DatabaseSafetyInterceptor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Services;

using IndTrace.Application.Attributes;

/// <summary>
/// Interceptor that validates entity operations against their designated database environment.
/// </summary>
/// <remarks>
/// This interceptor prevents simulation data from contaminating production databases
/// by validating entity attributes before database operations.
/// </remarks>
public class DatabaseSafetyInterceptor
{
    private readonly ILogger<DatabaseSafetyInterceptor> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseSafetyInterceptor"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public DatabaseSafetyInterceptor(ILogger<DatabaseSafetyInterceptor> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Validates that an entity can be safely used in the given data context.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entity">The entity instance.</param>
    /// <param name="context">The data context.</param>
    /// <exception cref="InvalidOperationException">Thrown when the entity cannot be used in the given context.</exception>
    /// <returns></returns>
    public IndQuestResults.Result ValidateEntity<T>(T entity, IDataContext context)
        where T : class
    {
        if (entity == null)
        {
            return IndQuestResults.Result.WithFailure("entity cannot be null");
        }

        if (context == null)
        {
            return IndQuestResults.Result.WithFailure("context cannot be null");
        }

        var entityType = typeof(T);
        var attribute = entityType.GetCustomAttribute<DatabaseSafeAttribute>();

        if (attribute == null)
        {
            this.logger.LogWarning("Entity type {EntityType} does not have DatabaseSafe attribute. Proceeding with caution.", entityType.Name);
            return IndQuestResults.Result.Success();
        }

        // Allow "Any" environment entities to work everywhere
        if (attribute.Environment == "Any" || attribute.AllowCrossEnvironment)
        {
            return IndQuestResults.Result.Success();
        }

        // Strict validation for environment-specific entities
        if (attribute.Environment != context.Environment)
        {
            var errorMessage = $"Cannot save {attribute.Environment} entity '{entityType.Name}' in {context.Environment} context. " +
                              $"This prevents data contamination between environments.";

            this.logger.LogError("Database safety violation: {ErrorMessage}", errorMessage);

            return IndQuestResults.Result.WithFailure(errorMessage);
        }

        // Additional check for simulation data reaching production
        if (context.Environment == "Production" && attribute.Environment == "Simulation")
        {
            var errorMessage = $"CRITICAL: Simulation entity '{entityType.Name}' attempted to reach production database. Operation blocked.";

            this.logger.LogCritical("CRITICAL database safety violation: {ErrorMessage}", errorMessage);

            return IndQuestResults.Result.WithFailure(errorMessage);
        }

        this.logger.LogDebug("Entity {EntityType} validated for {Environment} environment", entityType.Name, context.Environment);
        return IndQuestResults.Result.Success();
    }

    /// <summary>
    /// Validates a collection of entities against the given data context.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="entities">The collection of entities.</param>
    /// <param name="context">The data context.</param>
    /// <returns></returns>
    public IndQuestResults.Result ValidateEntities<T>(IEnumerable<T> entities, IDataContext context)
        where T : class
    {
        if (entities == null)
        {
            return IndQuestResults.Result.WithFailure("entities cannot be null");
        }

        foreach (var entity in entities)
        {
            var res = this.ValidateEntity(entity, context);
            if (res.IsFailure)
            {
                return res;
            }
        }

        return IndQuestResults.Result.Success();
    }
}
