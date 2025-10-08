// <copyright file="IRepository.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repository;

/// <summary>
/// Defines a generic repository interface for data access operations on entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type managed by this repository.</typeparam>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Review interface segregation: ensure IRepository does not force implementers to depend on methods they do not use (ISP - SOLID). Consider splitting if needed.
// TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated method signatures or patterns that could be abstracted. Refactor for maintainability if necessary.
// TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider using asynchronous streams (IAsyncEnumerable) for large data sets to improve scalability and performance.
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate repository interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the entity if found, or an error message.</returns>
    Task<Result<T?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an entity by its composite identifiers.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <param name="ids">The ordered key values that compose the entity primary key.</param>
    /// <returns>A result containing the entity if found, or an error message.</returns>
    Task<Result<T?>> GetByIdsAsync(CancellationToken cancellationToken, params object[] ids);

    /// <summary>
    /// Retrieves a list of entities matching the specified specification.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the list of entities.</returns>
    Task<Result<IEnumerable<T>>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the list of entities.</returns>
    Task<Result<IEnumerable<T>>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the first entity matching the specified specification, or null if none found.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the entity or null.</returns>
    Task<Result<T?>> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the first entity of type <typeparamref name="T"/>, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the entity or null.</returns>
    Task<Result<T?>> FirstOrDefaultAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the number of affected rows.</returns>
    Task<Result<int>> AddAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a collection of entities to the data store using bulk operations.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the number of affected rows.</returns>
    Task<Result<int>> AddRangeBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new entity to the data store with a specified table name and identifier.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="id">The identifier for the entity.</param>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the number of affected rows.</returns>
    Task<Result<int>> AddAsync(T entity, int id, string tableName, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a collection of entities to the data store.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the number of affected rows.</returns>
    Task<Result<int>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing entity in the data store.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result indicating the outcome of the operation.</returns>
    Task<Result> UpdateAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an entity from the data store.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result indicating the outcome of the operation.</returns>
    Task<Result> DeleteAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Commits all changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result indicating the outcome of the operation.</returns>
    Task<Result> CommitAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Counts the number of entities matching the specified specification.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the count of entities.</returns>
    Task<Result<int>> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> for the entity type.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A result containing the queryable collection.</returns>
    Task<Result<IQueryable<T>>> AsQueryableAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> for the entity type, filtered by the specified specification.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A result containing the queryable collection.</returns>
    Task<Result<IQueryable<T>>> AsQueryableAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    /// <summary>
    /// Detaches the specified entity from the context.
    /// </summary>
    /// <param name="entity">The entity to detach.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A result indicating the outcome of the operation.</returns>
    Task<Result> DetachAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Applies no-tracking behavior to the context.
    /// </summary>
    /// <returns>A result indicating the outcome of the operation.</returns>
    Task<Result> ApplyNoTrackingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Applies tracking behavior to the context.
    /// </summary>
    /// <returns>A result indicating the outcome of the operation.</returns>
    Task<Result> ApplyTrackingAsync(CancellationToken cancellationToken);
}
