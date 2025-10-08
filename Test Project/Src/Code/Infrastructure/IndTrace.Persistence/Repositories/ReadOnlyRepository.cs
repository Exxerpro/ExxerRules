using IndTrace.Domain.Models;
using IndTrace.Persistence.Caching;
using IndTrace.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IndTrace.Persistence.Repositories;

/// <summary>
/// Provides a read-only repository for querying entities with caching and specification support.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository.</typeparam>
/// <summary>
/// Read-only repository implementation for entity operations with caching.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ReadOnlyRepository{T}"/> class.
/// </remarks>
/// <param name="contextFactory">The database context factory.</param>
/// <param name="cache">The cache.</param>
/// <param name="logger">The logger.</param>
public class ReadOnlyRepository<T>(
    IIndTraceDbContextFactory contextFactory,
    ICacheService cache,
    ILogger<ReadOnlyRepository<T>> logger,
    [ServiceKey] string serviceKey = "",
    IOptions<CacheToggleOptions>? toggleOptions = null
) : IReadOnlyRepository<T> where T : class
{
    private readonly IIndTraceDbContextFactory contextFactory = contextFactory;
    private readonly ILogger<ReadOnlyRepository<T>> logger = logger;
    private readonly ICacheService cache = cache;
    private readonly bool _cacheEnabled = true;
    private readonly string _key = serviceKey;
    private readonly IOptions<CacheToggleOptions>? _toggleOptions = toggleOptions;

    private bool EffectiveCacheEnabled => _cacheEnabled && (_toggleOptions?.Value.Enabled != false);

    /// <summary>
    /// Gets an entity by its ID, using cache if available.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> containing the entity or an error.</returns>
    public async Task<Result<T?>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Result<T?>.WithFailure("Operation was canceled.");
        try
        {
            if (!EffectiveCacheEnabled)
            {
                logger.LogDebug("ReadOnlyRepository: Cache disabled, retrieving from repository");

                return await this.GetByIdFromDbAsync(id, cancellationToken).ConfigureAwait(false);
            }

            var cacheKey = CacheKeyBuilderReadOnlyRepos.BuildKey<T>("GetById", id);
            logger.LogDebug("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey}", typeof(T).Name, nameof(GetByIdAsync), cacheKey);

            var result = await this.cache.GetOrSetAsync<Result<T?>>(
                cacheKey,
                async ct => await this.GetByIdFromDbAsync(id, ct).ConfigureAwait(false),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            var finalResult = result ?? Result<T?>.WithFailure("Failed to retrieve data from cache");

            if (finalResult.IsFailure)
            {
                logger.LogError("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey} Error={Error}", typeof(T).Name, nameof(GetByIdAsync), cacheKey, result?.Error);
            }
            return finalResult;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "ReadOnlyRepository: Error in GetByIdAsync for type {EntityType}", typeof(T).Name);
            return Result<T?>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Gets an entity by its ID directly from the database.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> containing the entity or an error.</returns>
    private async Task<Result<T?>> GetByIdFromDbAsync(int id, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Result<T?>.WithFailure("Operation was canceled.");
        try
        {
            await using IIndTraceDbContext context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

            if (context.IsConnectionInactive)
                return Result<T?>.WithFailure("Database context is not active.");

            T? entity = await context.Set<T>().FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);

            if (entity is not null)
            {
                context.Entry(entity).State = EntityState.Detached; // Ensure no tracking
            }

            return entity is not null
                ? Result<T?>.Success(entity)
                : Result<T?>.WithFailure($"Entity of type {typeof(T).Name} with ID {id} not found.");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "ReadOnlyRepository: Error in GetByIdFromDbAsync for type {EntityType}", typeof(T).Name);
            return Result<T?>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Lists entities matching a specification, using cache if available.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{IEnumerable{T}}"/> containing the entities or an error.</returns>
    public async Task<Result<IEnumerable<T>>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        if (spec is null)
            return Result<IEnumerable<T>>.WithFailure("spec cannot be null.");
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<IEnumerable<T>>.WithFailure("Operation was canceled.");
        }
        this.logger.LogDebug("ReadOnlyRepository: {Operation}<{Entity}> CacheEnabled={CacheEnabled} ServiceKey={ServiceKey}", nameof(ListAsync), typeof(T).Name, EffectiveCacheEnabled, _key);
        try
        {
            if (!EffectiveCacheEnabled)
            {
                logger.LogDebug("ReadOnlyRepository: Cache disabled, retrieving from repository");

                return await this.ListFromDbAsync(spec, cancellationToken).ConfigureAwait(false);
            }

            var cacheKey = CacheKeyBuilderReadOnlyRepos.BuildKey<T>("ListAsync", spec);
            logger.LogDebug("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey}", typeof(T).Name, nameof(ListAsync), cacheKey);
            var result = await this.cache.GetOrSetAsync<Result<IEnumerable<T>>>(
                cacheKey,
                async ct => await this.ListFromDbAsync(spec, ct).ConfigureAwait(false),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            var finalResult = result ?? Result<IEnumerable<T>>.WithFailure("Failed to retrieve data from cache");
            if (finalResult.IsFailure)
                logger.LogError("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey} Error={Error}", typeof(T).Name, nameof(ListAsync), cacheKey, result?.Error);
            return finalResult;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "ReadOnlyRepository: Error in ListAsync(ISpecification) for type {EntityType}", typeof(T).Name);
            return Result<IEnumerable<T>>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Lists entities matching a specification directly from the database.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{IEnumerable{T}}"/> containing the entities or an error.</returns>
    private async Task<Result<IEnumerable<T>>> ListFromDbAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Result<IEnumerable<T>>.WithFailure("Operation was canceled.");
        try
        {
            await using IIndTraceDbContext context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (context.IsConnectionInactive)
            {
                return Result<IEnumerable<T>>.WithFailure("Database context is not active.");
            }

            IQueryable<T> query = this.ApplySpecification(spec, context);
            List<T> result = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            return Result<IEnumerable<T>>.Success(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "ReadOnlyRepository: Error in ListAsync(ISpecification) for type {EntityType}", typeof(T).Name);
            return Result<IEnumerable<T>>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Gets the first entity matching a specification, using cache if available.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> containing the entity or an error.</returns>
    public async Task<Result<T?>> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        if (spec is null)
        {
            return Result<T?>.WithFailure("spec cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<T?>.WithFailure("Operation was canceled.");
        }
        this.logger.LogDebug("ReadOnlyRepository: {Operation}<{Entity}> CacheEnabled={CacheEnabled} ServiceKey={ServiceKey}", nameof(FirstOrDefaultAsync), typeof(T).Name, EffectiveCacheEnabled, _key);
        try
        {
            if (!EffectiveCacheEnabled)
            {
                logger.LogDebug("ReadOnlyRepository: Cache disabled, retrieving from repository");

                return await this.FirstOrDefaultFromDbAsync(spec, cancellationToken).ConfigureAwait(false);
            }
            var cacheKey = CacheKeyBuilderReadOnlyRepos.BuildKey<T>("FirstOrDefault", spec);

            logger.LogDebug("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey}", typeof(T).Name, nameof(FirstOrDefaultAsync), cacheKey);
            var result = await this.cache.GetOrSetAsync<Result<T?>>(
                cacheKey,
                async ct => await this.FirstOrDefaultFromDbAsync(spec, ct).ConfigureAwait(false),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            var finalResult = result ?? Result<T?>.WithFailure("Failed to retrieve data from cache");
            if (finalResult.IsFailure)
            {
                logger.LogError("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey} Error={Error}", typeof(T).Name, nameof(FirstOrDefaultAsync), cacheKey, result?.Error);
            }
            return finalResult;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "ReadOnlyRepository: Error in FirstOrDefaultAsync(ISpecification) for type {EntityType}", typeof(T).Name);
            return Result<T?>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Gets the first entity matching a specification directly from the database.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> containing the entity or an error.</returns>
    private async Task<Result<T?>> FirstOrDefaultFromDbAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Result<T?>.WithFailure("Operation was canceled.");
        await using IIndTraceDbContext context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        if (context.IsConnectionInactive)
        {
            return Result<T?>.WithFailure("Database context is not active.");
        }

        T? entity = await this.ApplySpecification(spec, context)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        return entity is not null
            ? Result<T?>.Success(entity)
            : Result<T?>.WithFailure("No matching entity found.");
    }

    /// <summary>
    /// Lists all entities, using cache if available.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{IEnumerable{T}}"/> containing the entities or an error.</returns>
    public async Task<Result<IEnumerable<T>>> ListAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<IEnumerable<T>>.WithFailure("Operation was canceled.");
        }
        this.logger.LogDebug("ReadOnlyRepository: {Operation}<{Entity}> CacheEnabled={CacheEnabled} ServiceKey={ServiceKey}", nameof(ListAsync), typeof(T).Name, EffectiveCacheEnabled, _key);
        try
        {
            if (!EffectiveCacheEnabled)
            {
                logger.LogDebug("ReadOnlyRepository: Cache disabled, retrieving from repository");

                return await this.ListAsyncFromDbAsync(cancellationToken).ConfigureAwait(false);
            }

            var cacheKey = CacheKeyBuilderReadOnlyRepos.BuildKey<T>("ListAsync");

            logger.LogDebug("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey}", typeof(T).Name, nameof(ListAsync), cacheKey);

            var result = await this.cache.GetOrSetAsync<Result<IEnumerable<T>>>(
                cacheKey,
                async ct => await this.ListAsyncFromDbAsync(ct).ConfigureAwait(false),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            var finalResult = result ?? Result<IEnumerable<T>>.WithFailure("Failed to retrieve data from cache");
            if (finalResult.IsFailure)
            {
                logger.LogError("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey} Error={Error}", typeof(T).Name, nameof(ListAsync), cacheKey, result?.Error);
            }
            return finalResult;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "ReadOnlyRepository: Error in ListAsync() for type {EntityType}", typeof(T).Name);
            return Result<IEnumerable<T>>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Lists all entities directly from the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{IEnumerable{T}}"/> containing the entities or an error.</returns>
    private async Task<Result<IEnumerable<T>>> ListAsyncFromDbAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Result<IEnumerable<T>>.WithFailure("Operation was canceled.");
        try
        {
            await using IIndTraceDbContext context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (context.IsConnectionInactive)
                return Result<IEnumerable<T>>.WithFailure("Database context is not active.");

            List<T> entities = await context.Set<T>().AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
            return Result<IEnumerable<T>>.Success(entities);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "ReadOnlyRepository: Error in ListAsync() for type {EntityType}", typeof(T).Name);
            return Result<IEnumerable<T>>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Gets the first entity, using cache if available.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> containing the entity or an error.</returns>
    public async Task<Result<T?>> FirstOrDefaultAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Result<T?>.WithFailure("Operation was canceled.");
        this.logger.LogDebug("ReadOnlyRepository: {Operation}<{Entity}> CacheEnabled={CacheEnabled} ServiceKey={ServiceKey}", nameof(FirstOrDefaultAsync), typeof(T).Name, EffectiveCacheEnabled, _key);
        try
        {
            if (!EffectiveCacheEnabled)
            {
                logger.LogDebug("ReadOnlyRepository: Cache disabled, retrieving from repository");

                return await this.FirstOrDefaultFromDbAsync(cancellationToken).ConfigureAwait(false);
            }

            var cacheKey = CacheKeyBuilderReadOnlyRepos.BuildKey<T>("FirstOrDefault");
            logger.LogDebug("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey}", typeof(T).Name, nameof(FirstOrDefaultAsync), cacheKey);

            var result = await this.cache.GetOrSetAsync<Result<T?>>(
                cacheKey,
                async ct => await this.FirstOrDefaultFromDbAsync(ct).ConfigureAwait(false),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            var finalResult = result ?? Result<T?>.WithFailure("Failed to retrieve data from cache");
            if (finalResult.IsFailure)
                logger.LogError("ReadOnlyRepository: Entity={Entity} Operation={Operation} CacheKey={CacheKey} Error={Error}", typeof(T).Name, nameof(FirstOrDefaultAsync), cacheKey, result?.Error);
            return finalResult;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "ReadOnlyRepository: Error in FirstOrDefaultAsync() for type {EntityType}", typeof(T).Name);
            return Result<T?>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Gets the first entity directly from the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> containing the entity or an error.</returns>
    private async Task<Result<T?>> FirstOrDefaultFromDbAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<T?>.WithFailure("Operation was canceled.");
        }
        try
        {
            await using IIndTraceDbContext context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (context.IsConnectionInactive)
            {
                return Result<T?>.WithFailure("Database context is not active.");
            }

            T? entity = await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return entity is not null
                ? Result<T?>.Success(entity)
                : Result<T?>.WithFailure("No entity found.");
        }
        catch (Exception ex)
        {
            if (this.logger.IsEnabled(LogLevel.Error))
                this.logger.LogError(ex, "Repository Readonly :: Error in FirstOrDefaultAsync() for type {EntityType}", typeof(T).Name);
            return Result<T?>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Counts entities matching a specification, using cache if available.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{int}"/> containing the count or an error.</returns>
    public async Task<Result<int>> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        if (spec is null)
        {
            return Result<int>.WithFailure("spec cannot be null.");
        }
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<int>.WithFailure("Operation was canceled.");
        }
        if (this.logger.IsEnabled(LogLevel.Debug))
        {
            this.logger.LogDebug("ReadOnlyRepository :: {Operation}<{Entity}> CacheEnabled={CacheEnabled} ServiceKey={ServiceKey}", nameof(CountAsync), typeof(T).Name, EffectiveCacheEnabled, _key);
        }
        try
        {
            if (!EffectiveCacheEnabled)
            {
                if (this.logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Repository Readonly :: Cache disabele retrieving from the repository");
                }

                return await this.CountAsyncFromDbAsync(spec, cancellationToken).ConfigureAwait(false);
            }
            var cacheKey = CacheKeyBuilderReadOnlyRepos.BuildKey<T>("CountAsync", spec);
            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Repository Readonly :: for entity {entity} operation {operation} Using Cache Key: {CacheKey}", typeof(T).Name, nameof(CountAsync), cacheKey);
            }

            var result = await this.cache.GetOrSetAsync<Result<int>>(
                cacheKey,
                async ct => await this.CountAsyncFromDbAsync(spec, ct).ConfigureAwait(false),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            var finalResult = result ?? Result<int>.WithFailure("Failed to retrieve data from cache");
            if (finalResult.IsFailure && this.logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogError("Repository Readonly :: for entity {entity} operation {operation} Using Cache Key: {CacheKey} resulted on error {error}", typeof(T).Name, nameof(CountAsync), cacheKey, result?.Error);
            }
            return finalResult;
        }
        catch (Exception ex)
        {
            if (this.logger.IsEnabled(LogLevel.Error))
            {
                this.logger.LogError(ex, "Repository Readonly :: Error in GetByIdsAsync for type {EntityType}", typeof(int).Name);
            }
            return Result<int>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Gets an entity by its composite identifiers, using cache if available.
    /// </summary>
    public async Task<Result<T?>> GetByIdsAsync(CancellationToken cancellationToken, params object[] ids)
    {
        if (ids is null || ids.Length == 0)
        {
            return Result<T?>.WithFailure("ids cannot be null or empty.");
        }
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<T?>.WithFailure("Operation was canceled.");
        }
        if (this.logger.IsEnabled(LogLevel.Debug))
        {
            this.logger.LogDebug("ReadOnlyRepository :: {Operation}<{Entity}> CacheEnabled={CacheEnabled} ServiceKey={ServiceKey}", nameof(GetByIdsAsync), typeof(T).Name, EffectiveCacheEnabled, _key);
        }
        try
        {
            var cacheKey = CacheKeyBuilderReadOnlyRepos.BuildKey<T>("GetByIds", ids);
            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Repository Readonly :: for entity {entity } operation {operation} Using Cache Key: {CacheKey}", typeof(T).Name, nameof(GetByIdsAsync), cacheKey);
            }

            var result = await this.cache.GetOrSetAsync<Result<T?>>(cacheKey,
                async ct => await this.GetByIdsFromDbAsync(ids, ct).ConfigureAwait(false),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var finalResult = result ?? Result<T?>.WithFailure("Failed to retrieve data from cache");
            if (finalResult.IsFailure && this.logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogError("Repository Readonly :: for entity {entity } operation {operation} Using Cache Key: {CacheKey} resulted on error {error}", typeof(T).Name, nameof(GetByIdsAsync), cacheKey, result?.Error);
            }
            return finalResult;
        }
        catch (Exception ex)
        {
            if (this.logger.IsEnabled(LogLevel.Error))
            {
                this.logger.LogError(ex, "Repository Readonly :: Error in GetByIdsAsync for type {EntityType}", typeof(T).Name);
            }
            return Result<T?>.WithFailure(ex.Message);
        }
    }

    private async Task<Result<T?>> GetByIdsFromDbAsync(object[] ids, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<T?>.WithFailure("Operation was canceled.");
        }
        try
        {
            await using IIndTraceDbContext context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (context.IsConnectionInactive)
            {
                return Result<T?>.WithFailure("Database context is not active.");
            }

            var entity = await context.Set<T>().FindAsync(ids, cancellationToken).ConfigureAwait(false);
            if (entity is not null)
            {
                context.Entry(entity).State = EntityState.Detached;
            }

            return entity is not null ? Result<T?>.Success(entity) : Result<T?>.WithFailure("Entity not found.");
        }
        catch (Exception ex)
        {
            if (this.logger.IsEnabled(LogLevel.Error))
            {
                this.logger.LogError(ex, "Repository Readonly :: Error in GetByIdsFromDbAsync for type {EntityType}", typeof(T).Name);
            }
            return Result<T?>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Counts entities matching a specification directly from the database.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Result{int}"/> containing the count or an error.</returns>
    private async Task<Result<int>> CountAsyncFromDbAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<int>.WithFailure("Operation was canceled.");
        }
        try
        {
            await using IIndTraceDbContext context = await this.contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (context.IsConnectionInactive)
            {
                return Result<int>.WithFailure("Database context is not active.");
            }

            int count = await this.ApplySpecification(spec, context).CountAsync(cancellationToken).ConfigureAwait(false);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            if (this.logger.IsEnabled(LogLevel.Error))
            {
                this.logger.LogError(ex, "Repository Readonly :: Error in CountAsync for type {EntityType}", typeof(T).Name);
            }
            return Result<int>.WithFailure(ex.Message);
        }
    }

    /// <summary>
    /// Applies a specification to the entity set, including filters, includes, ordering, and paging.
    /// Always returns a no-tracking query as per read-only policy.
    /// </summary>
    /// <param name="spec">The specification to apply.</param>
    /// <param name="context">The database context.</param>
    /// <returns>An <see cref="IQueryable{T}"/> filtered and shaped according to the specification.</returns>
    private IQueryable<T> ApplySpecification(ISpecification<T> spec, IIndTraceDbContext context)
    {
        IQueryable<T> query = context.Set<T>();

        if (spec.Criteria is not null)
        {
            query = query.Where(spec.Criteria);
        }

        if (spec.Includes is not null && spec.Includes.Count > 0)
        {
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        }

        if (spec.IncludeStrings is not null && spec.IncludeStrings.Count > 0)
        {
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
        }

        // Apply ordering and secondary ordering when present
        IOrderedQueryable<T>? ordered = null;
        if (spec.OrderBy is not null)
        {
            ordered = query.OrderBy(spec.OrderBy);
        }
        else if (spec.OrderByDescending is not null)
        {
            ordered = query.OrderByDescending(spec.OrderByDescending);
        }

        if (ordered is not null)
        {
            if (spec.ThenBy is not null)
            {
                ordered = ordered.ThenBy(spec.ThenBy);
            }

            if (spec.ThenByDescending is not null)
            {
                ordered = ordered.ThenByDescending(spec.ThenByDescending);
            }

            query = ordered;
        }

        if (spec.Skip.HasValue && spec.Take.HasValue)
        {
            query = query.Skip(spec.Skip.Value).Take(spec.Take.Value);
        }

        return query.AsNoTracking(); // Always enforce no-tracking
    }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate read-only repository logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
