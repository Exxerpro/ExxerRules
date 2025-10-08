using IndTrace.Domain.Models;
using IndTrace.Persistence.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;

namespace IndTrace.Persistence.Repositories
{
    /// <summary>
    /// Generic repository implementation providing data access operations for entities.
    /// Implements the Repository pattern with specification pattern for flexible querying.
    /// </summary>
    /// <typeparam name="T">The entity type that this repository manages.</typeparam>
    /// <summary>
    /// Generic repository implementation for entity operations.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class Repository<T> : IRepository<T>, IReadOnlyRepository<T> where T : class
    {
        private const string DatabaseContextIsNotActive = "Database context is not active.";
        private readonly IIndTraceDbContextFactory contextFactory;
        private readonly ILogger<Repository<T>> logger;
        private readonly string key = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="contextFactory">The database context factory.</param>
        /// <param name="logger">The logger.</param>
        public Repository(IIndTraceDbContextFactory contextFactory, ILogger<Repository<T>> logger, [ServiceKey] string key = "")
        {
            this.contextFactory = contextFactory;
            this.logger = logger;
            this.key = key;
        }

        private bool InvalidContext(IIndTraceDbContext? context, string methodName)
        {
            if (context is null)
            {
                this.logger.LogError("Context is null in {MethodName} for entity type {EntityType}.", methodName, typeof(T).Name);
                return true;
            }

            if (!context.IsConnectionActive)
            {
                this.logger.LogError("Connection is inactive in {MethodName} for entity type {EntityType}.", methodName, typeof(T).Name);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies the given specification to the DbSet, returning a filtered IQueryable.
        /// </summary>
        /// <param name="specification">The specification to apply.</param>
        /// <param name="context">The database context.</param>
        /// <returns>An IQueryable filtered and shaped according to the specification.</returns>
        private IQueryable<T> ApplySpecification(ISpecification<T> specification, IIndTraceDbContext context)
        {
            var query = context.Set<T>().AsQueryable();

            // Apply criteria
            if (specification.Criteria is not null)
                query = query.Where(specification.Criteria);

            // Apply includes (expression-based)
            if (specification.Includes is not null)
            {
                foreach (var include in specification.Includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply includes (string-based)
            if (specification.IncludeStrings is not null)
            {
                foreach (var includeString in specification.IncludeStrings)
                {
                    query = query.Include(includeString);
                }
            }

            // Apply ordering (primary + secondary)
            IOrderedQueryable<T>? ordered = null;
            if (specification.OrderBy is not null)
            {
                ordered = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending is not null)
            {
                ordered = query.OrderByDescending(specification.OrderByDescending);
            }

            if (ordered is not null)
            {
                if (specification.ThenBy is not null)
                {
                    ordered = ordered.ThenBy(specification.ThenBy);
                }

                if (specification.ThenByDescending is not null)
                {
                    ordered = ordered.ThenByDescending(specification.ThenByDescending);
                }

                query = ordered;
            }

            // Apply paging
            if (specification.Skip.HasValue)
                query = query.Skip(specification.Skip.Value);
            if (specification.Take.HasValue)
                query = query.Take(specification.Take.Value);

            // Apply tracking
            if (!specification.IsTracking)
            {
                query = query.AsNoTracking();
            }
            else
            {
                query = query.AsTracking();
            }

            return query;
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A result containing the entity if found, or an error message.</returns>
        public async Task<Result<T?>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.GetByIdAsync);
            if (cancellationToken.IsCancellationRequested)
                return Result<T?>.WithFailure("Operation was canceled.");
            try
            {
                if (this.contextFactory is null)
                    return Result<T?>.WithFailure("Context factory is not initialized");

                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<T?>.WithFailure(DatabaseContextIsNotActive);

                var entity = await context.Set<T>().FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);

                return entity is not null
                    ? Result<T?>.Success(entity)
                    : Result<T?>.WithFailure($"Entity of type {typeof(T).Name} with ID {id} not found.");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<T?>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves an entity by its composite identifiers.
        /// </summary>
        public async Task<Result<T?>> GetByIdsAsync(CancellationToken cancellationToken, params object[] ids)
        {
            if (ids is null || ids.Length == 0)
                return Result<T?>.WithFailure("ids cannot be null or empty.");
            try
            {
                await using IIndTraceDbContext context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (InvalidContext(context, nameof(GetByIdsAsync)))
                    return Result<T?>.WithFailure(DatabaseContextIsNotActive);

                var entity = await context.Set<T>().FindAsync(ids, cancellationToken).ConfigureAwait(false);
                return entity is not null ? Result<T?>.Success(entity) : Result<T?>.WithFailure("Entity not found.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", nameof(GetByIdsAsync), typeof(T).Name);
                return Result<T?>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a list of entities that match the given specification.
        /// </summary>
        /// <param name="spec">The specification defining filtering, ordering, and inclusion criteria.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Result containing the list of matching entities or failure if error occurred.</returns>
        public async Task<Result<IEnumerable<T>>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.ListAsync);
            if (spec is null)
                return Result<IEnumerable<T>>.WithFailure("spec cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result<IEnumerable<T>>.WithFailure("Operation was canceled.");
            try
            {
                if (this.contextFactory is null)
                    return Result<IEnumerable<T>>.WithFailure("Context factory is not initialized");

                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<IEnumerable<T>>.WithFailure(DatabaseContextIsNotActive);

                var entities = await this.ApplySpecification(spec, context).AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
                return Result<IEnumerable<T>>.Success(entities);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<IEnumerable<T>>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all entities of type T from the repository.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Result containing all entities or failure if error occurred.</returns>
        public async Task<Result<IEnumerable<T>>> ListAsync(CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.ListAsync);
            if (cancellationToken.IsCancellationRequested)
                return Result<IEnumerable<T>>.WithFailure("Operation was canceled.");
            try
            {
                if (this.contextFactory is null)
                    return Result<IEnumerable<T>>.WithFailure("Context factory is not initialized");

                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<IEnumerable<T>>.WithFailure(DatabaseContextIsNotActive);

                var entities = await context.Set<T>().AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
                return Result<IEnumerable<T>>.Success(entities);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<IEnumerable<T>>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the first entity that matches the given specification, or null if none found.
        /// </summary>
        /// <param name="spec">The specification defining filtering, ordering, and inclusion criteria.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Result containing the first matching entity or null if none found.</returns>
        public async Task<Result<T?>> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.FirstOrDefaultAsync);
            if (spec is null)
                return Result<T?>.WithFailure("spec cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result<T?>.WithFailure("Operation was canceled.");
            try
            {
                if (this.contextFactory is null)
                    return Result<T?>.WithFailure("Context factory is not initialized");

                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<T?>.WithFailure(DatabaseContextIsNotActive);

                var entity = await this.ApplySpecification(spec, context).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return entity is not null ? Result<T?>.Success(entity) : Result<T?>.WithFailure("No matching entity found.");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<T?>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the first entity of type <typeparamref name="T"/>, or null if none found.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A result containing the entity or null.</returns>
        public async Task<Result<T?>> FirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.FirstOrDefaultAsync);
            if (cancellationToken.IsCancellationRequested)
                return Result<T?>.WithFailure("Operation was canceled.");
            try
            {
                if (this.contextFactory is null)
                    return Result<T?>.WithFailure("Context factory is not initialized");

                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<T?>.WithFailure(DatabaseContextIsNotActive);

                var entity = await context.Set<T>().FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return entity != null ? Result<T?>.Success(entity) : Result<T?>.WithFailure("No entity found.");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<T?>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Result containing the number of entities added or failure if error occurred.</returns>
        public async Task<Result<int>> AddAsync(T entity, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.AddAsync);
            if (entity is null)
                return Result<int>.WithFailure("entity cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result<int>.WithFailure("Operation was canceled.");
            try
            {
                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<int>.WithFailure(DatabaseContextIsNotActive);

                await context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
                var result = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<int>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Adds a collection of entities to the data store using bulk operations.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A result containing the number of affected rows.</returns>
        public async Task<Result<int>> AddRangeBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.AddRangeBulkAsync);
            if (entities is null)
                return Result<int>.WithFailure("entities cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result<int>.WithFailure("Operation was canceled.");
            try
            {
                //TODO [RESOURCE LEAK][CURSOR][20/JUNE/2025] -
                //SqlConnection is opened before using block.
                //If an exception is thrown before entering the using,
                //the connection may not be disposed.
                //Consider opening inside the using block.
                //See .NET best practices for resource management.
                //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] -
                //Exception handling: avoid catching general Exception,
                //catch specific exceptions where possible. See .NET best practices for exception handling.
                //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] -
                //Validate repository logic and handle edge cases defensively
                //. Ensure all required properties are set and handle missing/invalid values gracefully.
                //[Fix]
                //CLAUDE
                //Date: 22/09/2025
                //Reason: [Pattern: InMemory Database Support] - Added check for InMemory database to use AddRangeAsync instead of bulk operations
                // Use the real EF Core DbContext for bulk operations
                await using var efContext = this.contextFactory.CreateEfDbContext();

                // Check if database is relational (SQL Server) or InMemory
                var providerName = efContext.Database.ProviderName;
                var isInMemory = providerName != null && providerName.Contains("InMemory", StringComparison.OrdinalIgnoreCase);

                if (isInMemory)
                {
                    // For InMemory database, use regular AddRange instead of bulk operations
                    return await this.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                }

                // Optional: still validate connection from your abstract context
                var logicalContext = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(logicalContext, methodName))
                    return Result<int>.WithFailure(DatabaseContextIsNotActive);

                var dataTable = entities.ToDataTable();

                var connection = (SqlConnection)efContext.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                var table = efContext.Model.FindEntityType(typeof(T));

                if (table is null)
                    return Result<int>.WithFailure($"Entity type {typeof(T).Name} not found in the model.");

                var tableName = table.GetTableName();

                if (string.IsNullOrWhiteSpace(tableName))
                    return Result<int>.WithFailure($"Unable to determine table name for entity type {typeof(T).Name}");

                using var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = tableName,
                };

                foreach (DataColumn column in dataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken).ConfigureAwait(false);

                return Result<int>.Success(dataTable.Rows.Count);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<int>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Adds a collection of entities to the data store.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A result containing the number of affected rows.</returns>
        public async Task<Result<int>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.AddRangeAsync);
            if (entities is null)
                return Result<int>.WithFailure("entities cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result<int>.WithFailure("Operation was canceled.");
            try
            {
                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<int>.WithFailure(DatabaseContextIsNotActive);

                await context.Set<T>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                var result = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
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
            const string methodName = nameof(this.AddAsync);
            if (entity is null)
                return Result<int>.WithFailure("entity cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result<int>.WithFailure("Operation was canceled.");
            try
            {
                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<int>.WithFailure(DatabaseContextIsNotActive);

                //[Fix]
                //CLAUDE
                //Date: 23/09/2025
                //Reason: [CRITICAL: ID Validation Required] - Products don't have identity, must validate ID > 0
                // Restore critical validation removed by previous agent - Products need manual ID validation
                if (id <= 0 || string.IsNullOrWhiteSpace(tableName))
                {
                    this.logger.LogWarning("Repository: Invalid parameters in {MethodName} for entity type {EntityType}.", methodName, typeof(T).Name);
                    return Result<int>.WithFailure($"Invalid id or table name in {methodName} for entity type {typeof(T).Name}.");
                }

                // If provider is not relational, fall back to regular SaveChanges (no tableName overload)
                var isRelational = context.Database.IsRelational();

                await context.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
                if (!isRelational)
                {
                    var saved = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    return Result<int>.Success(saved);
                }

                var result = await context.SaveChangesAsync(tableName, cancellationToken).ConfigureAwait(false);
                return result ?? Result<int>.WithFailure("Failed to save changes.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<int>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Result indicating success or failure of the update operation.</returns>
        public async Task<Result> UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.UpdateAsync);
            if (entity is null)
                return Result.WithFailure("entity cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result.WithFailure("Operation was canceled.");
            try
            {
                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result.WithFailure(DatabaseContextIsNotActive);

                return await EntityUpdateHelper<T>.UpdateAsync(context, entity, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Result indicating success or failure of the delete operation.</returns>
        public async Task<Result> DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.DeleteAsync);
            if (entity is null)
                return Result.WithFailure("entity cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result.WithFailure("Operation was canceled.");
            try
            {
                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result.WithFailure(DatabaseContextIsNotActive);

                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Commits all changes to the data store.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A result indicating the outcome of the operation.</returns>
        public async Task<Result> CommitAsync(CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.CommitAsync);
            if (cancellationToken.IsCancellationRequested)
                return Result.WithFailure("Operation was canceled.");
            try
            {
                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result.WithFailure(DatabaseContextIsNotActive);

                //[Fix]
                //CLAUDE
                //Date: 09/09/2025
                //Reason: [Double SaveChanges Anti-Pattern] - Check if context has pending changes before saving
                // Only save if there are dirty entities, otherwise just return success
                // This prevents failures when CommitAsync is called after AddAsync already saved
                if (context.ChangeTracker.HasChanges())
                {
                    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Counts the number of entities matching the specified specification.
        /// </summary>
        /// <param name="spec">The specification to filter entities.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A result containing the count of entities.</returns>
        public async Task<Result<int>> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.CountAsync);
            if (spec is null)
                return Result<int>.WithFailure("spec cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result<int>.WithFailure("Operation was canceled.");
            try
            {
                await using var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<int>.WithFailure(DatabaseContextIsNotActive);

                var query = this.ApplySpecification(spec, context);
                var count = await query.CountAsync(cancellationToken).ConfigureAwait(false);
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Repository: Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<int>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/> for the entity type.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A result containing the queryable collection.</returns>
        public async Task<Result<IQueryable<T>>> AsQueryableAsync(CancellationToken cancellationToken = default)
        {
            const string methodName = nameof(this.AsQueryableAsync);
            if (cancellationToken.IsCancellationRequested)
                return Result<IQueryable<T>>.WithFailure("Operation was canceled.");
            try
            {
                var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<IQueryable<T>>.WithFailure(DatabaseContextIsNotActive);

                return Result<IQueryable<T>>.Success(context.Set<T>().AsQueryable());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<IQueryable<T>>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/> for the entity type, filtered by the specified specification.
        /// </summary>
        /// <param name="spec">The specification to filter entities.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A result containing the queryable collection.</returns>
        public async Task<Result<IQueryable<T>>> AsQueryableAsync(
            ISpecification<T> spec,
            CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.AsQueryableAsync);
            if (spec is null)
                return Result<IQueryable<T>>.WithFailure("spec cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result<IQueryable<T>>.WithFailure("Operation was canceled.");
            try
            {
                var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result<IQueryable<T>>.WithFailure(DatabaseContextIsNotActive);

                return Result<IQueryable<T>>.Success(this.ApplySpecification(spec, context));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result<IQueryable<T>>.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Detaches the specified entity from the context.
        /// </summary>
        /// <param name="entity">The entity to detach.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A result indicating the outcome of the operation.</returns>
        public async Task<Result> DetachAsync(T entity, CancellationToken cancellationToken)
        {
            const string methodName = nameof(this.DetachAsync);
            if (entity is null)
                return Result.WithFailure("entity cannot be null.");
            if (cancellationToken.IsCancellationRequested)
                return Result.WithFailure("Operation was canceled.");
            try
            {
                var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result.WithFailure(DatabaseContextIsNotActive);

                context.Entry(entity).State = EntityState.Detached;
                return Result.Success();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Applies no-tracking behavior to the context.
        /// </summary>
        /// <returns>A result indicating the outcome of the operation.</returns>
        public async Task<Result> ApplyNoTrackingAsync(CancellationToken cancellationToken = default)
        {
            const string methodName = nameof(this.ApplyNoTrackingAsync);
            if (cancellationToken.IsCancellationRequested)
                return Result.WithFailure("Operation was canceled.");
            try
            {
                var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result.WithFailure(DatabaseContextIsNotActive);

                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return Result.Success();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Applies tracking behavior to the context.
        /// </summary>
        /// <returns>A result indicating the outcome of the operation.</returns>
        public async Task<Result> ApplyTrackingAsync(CancellationToken cancellationToken = default)
        {
            const string methodName = nameof(this.ApplyTrackingAsync);
            if (cancellationToken.IsCancellationRequested)
                return Result.WithFailure("Operation was canceled.");
            try
            {
                var context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
                if (this.InvalidContext(context, methodName))
                    return Result.WithFailure(DatabaseContextIsNotActive);

                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                return Result.Success();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in {MethodName} for entity type {EntityType}", methodName, typeof(T).Name);
                return Result.WithFailure(ex.Message);
            }
        }

        /// <summary>
        /// Disposes the repository asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync() => await Task.CompletedTask;
    }
}
