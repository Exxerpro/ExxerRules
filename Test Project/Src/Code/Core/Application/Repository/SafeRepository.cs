// <copyright file="SafeRepository.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repository;

/// <summary>
/// A database-safe repository implementation that prevents simulation data from contaminating production databases.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository.</typeparam>
/// <remarks>
/// This repository wraps the standard repository with additional safety checks
/// to ensure data integrity across different environments.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="SafeRepository{T}"/> class.
/// </remarks>
/// <param name="innerRepository">The inner repository to wrap.</param>
/// <param name="dataContext">The data context for this repository.</param>
/// <param name="safetyInterceptor">The database safety interceptor.</param>
/// <param name="logger">The logger instance.</param>
/// <remarks>
/// Initializes a new instance of the SafeRepository class.
/// </remarks>
/// <param name="innerRepository">The inner repository.</param>
/// <param name="dataContext">The data context.</param>
/// <param name="safetyInterceptor">The safety interceptor.</param>
/// <param name="logger">The logger.</param>
public class SafeRepository<T>(
    IRepository<T> innerRepository,
    IDataContext dataContext,
    DatabaseSafetyInterceptor safetyInterceptor,
    ILogger<SafeRepository<T>> logger) : IRepository<T> where T : class
{
    private readonly IRepository<T> innerRepository = innerRepository;
    private readonly IDataContext dataContext = dataContext;
    private readonly DatabaseSafetyInterceptor safetyInterceptor = safetyInterceptor;
    private readonly ILogger<SafeRepository<T>> logger = logger;

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the entity if found, or an error message.</returns>
    public async Task<Result<T?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await this.innerRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves an entity by its composite identifiers.
    /// </summary>
    public async Task<Result<T?>> GetByIdsAsync(CancellationToken cancellationToken, params object[] ids)
    {
        return await this.innerRepository.GetByIdsAsync(cancellationToken, ids).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a list of entities matching the specified specification.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the list of entities.</returns>
    public async Task<Result<IEnumerable<T>>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        return await this.innerRepository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the list of entities.</returns>
    public async Task<Result<IEnumerable<T>>> ListAsync(CancellationToken cancellationToken)
    {
        return await this.innerRepository.ListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the first entity matching the specified specification, or null if none found.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the entity or null.</returns>
    public async Task<Result<T?>> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        return await this.innerRepository.FirstOrDefaultAsync(spec, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the first entity of type <typeparamref name="T"/>, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the entity or null.</returns>
    public async Task<Result<T?>> FirstOrDefaultAsync(CancellationToken cancellationToken)
    {
        return await this.innerRepository.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds a new entity to the data store with safety validation.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the number of affected rows.</returns>
    public async Task<Result<int>> AddAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            this.safetyInterceptor.ValidateEntity(entity, this.dataContext);

            // Block simulation data from reaching production database
            if (this.dataContext.IsSimulation && !this.dataContext.AllowsDatabaseWrites)
            {
                this.logger.LogInformation("Simulation data blocked from database write for entity type {EntityType}", typeof(T).Name);
                return Result<int>.Success(0); // Simulate success without actual DB write
            }

            return await this.innerRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Database safety validation failed for entity type {EntityType}", typeof(T).Name);
            return Result<int>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Adds a collection of entities to the data store using bulk operations with safety validation.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the number of affected rows.</returns>
    public async Task<Result<int>> AddRangeBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        try
        {
            this.safetyInterceptor.ValidateEntities(entities, this.dataContext);

            // Block simulation data from reaching production database
            if (this.dataContext.IsSimulation && !this.dataContext.AllowsDatabaseWrites)
            {
                var count = entities.Count();
                this.logger.LogInformation("Simulation bulk data blocked from database write for {Count} entities of type {EntityType}", count, typeof(T).Name);
                return Result<int>.Success(count); // Simulate success without actual DB write
            }

            return await this.innerRepository.AddRangeBulkAsync(entities, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Database safety validation failed for bulk entities of type {EntityType}", typeof(T).Name);
            return Result<int>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Adds a new entity to the data store with a specified table name and identifier.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="id">The identifier for the entity.</param>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the number of affected rows.</returns>
    public async Task<Result<int>> AddAsync(T entity, int id, string tableName, CancellationToken cancellationToken)
    {
        try
        {
            this.safetyInterceptor.ValidateEntity(entity, this.dataContext);

            // Block simulation data from reaching production database
            if (this.dataContext.IsSimulation && !this.dataContext.AllowsDatabaseWrites)
            {
                this.logger.LogInformation("Simulation data blocked from database write for entity type {EntityType} to table {TableName}", typeof(T).Name, tableName);
                return Result<int>.Success(0); // Simulate success without actual DB write
            }

            return await this.innerRepository.AddAsync(entity, id, tableName, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Database safety validation failed for entity type {EntityType}", typeof(T).Name);
            return Result<int>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Adds a collection of entities to the data store with safety validation.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the number of affected rows.</returns>
    public async Task<Result<int>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        try
        {
            this.safetyInterceptor.ValidateEntities(entities, this.dataContext);

            // Block simulation data from reaching production database
            if (this.dataContext.IsSimulation && !this.dataContext.AllowsDatabaseWrites)
            {
                var count = entities.Count();
                this.logger.LogInformation("Simulation range data blocked from database write for {Count} entities of type {EntityType}", count, typeof(T).Name);
                return Result<int>.Success(count); // Simulate success without actual DB write
            }

            return await this.innerRepository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Database safety validation failed for range entities of type {EntityType}", typeof(T).Name);
            return Result<int>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing entity in the data store with safety validation.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result indicating the outcome of the operation.</returns>
    public async Task<Result> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            this.safetyInterceptor.ValidateEntity(entity, this.dataContext);

            // Block simulation data from reaching production database
            if (this.dataContext.IsSimulation && !this.dataContext.AllowsDatabaseWrites)
            {
                this.logger.LogInformation("Simulation data update blocked for entity type {EntityType}", typeof(T).Name);
                return Result.Success(); // Simulate success without actual DB write
            }

            return await this.innerRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Database safety validation failed for update of entity type {EntityType}", typeof(T).Name);
            return Result.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an entity from the data store with safety validation.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result indicating the outcome of the operation.</returns>
    public async Task<Result> DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            this.safetyInterceptor.ValidateEntity(entity, this.dataContext);

            // Block simulation data from reaching production database
            if (this.dataContext.IsSimulation && !this.dataContext.AllowsDatabaseWrites)
            {
                this.logger.LogInformation("Simulation data delete blocked for entity type {EntityType}", typeof(T).Name);
                return Result.Success(); // Simulate success without actual DB operation
            }

            return await this.innerRepository.DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Database safety validation failed for delete of entity type {EntityType}", typeof(T).Name);
            return Result.WithFailure(ex.Message);
        }
    }

    // Read operations don't need safety validation - they can read from any environment

    /// <summary>
    /// Executes CommitAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of CommitAsync.</returns>
    public async Task<Result> CommitAsync(CancellationToken cancellationToken)
    {
        return await this.innerRepository.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes CountAsync operation.
    /// </summary>
    /// <param name="spec">The spec.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of CountAsync.</returns>
    public async Task<Result<int>> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        return await this.innerRepository.CountAsync(spec, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes AsQueryableAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of AsQueryableAsync.</returns>
    public async Task<Result<IQueryable<T>>> AsQueryableAsync(CancellationToken cancellationToken)
    {
        return await this.innerRepository.AsQueryableAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes AsQueryableAsync operation.
    /// </summary>
    /// <param name="spec">The spec.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of AsQueryableAsync.</returns>
    public async Task<Result<IQueryable<T>>> AsQueryableAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        return await this.innerRepository.AsQueryableAsync(spec, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes DetachAsync operation.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of DetachAsync.</returns>
    public async Task<Result> DetachAsync(T entity, CancellationToken cancellationToken)
    {
        return await this.innerRepository.DetachAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes ApplyNoTrackingAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ApplyNoTrackingAsync.</returns>
    public async Task<Result> ApplyNoTrackingAsync(CancellationToken cancellationToken)
    {
        return await this.innerRepository.ApplyNoTrackingAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes ApplyTrackingAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ApplyTrackingAsync.</returns>
    public async Task<Result> ApplyTrackingAsync(CancellationToken cancellationToken)
    {
        return await this.innerRepository.ApplyTrackingAsync(cancellationToken).ConfigureAwait(false);
    }
}
