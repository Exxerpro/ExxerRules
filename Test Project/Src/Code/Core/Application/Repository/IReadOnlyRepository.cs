// <copyright file="IReadOnlyRepository.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repository;

using System.Security.Principal;

/// <summary>
/// Defines a read-only repository interface for accessing entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IReadOnlyRepository<T> where T : class
{
    /// <summary>
    /// Gets an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the entity if found; otherwise, null.</returns>
    Task<Result<T?>> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an entity by its composite identifiers asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <param name="ids">The ordered key values that compose the entity primary key.</param>
    Task<Result<T?>> GetByIdsAsync(CancellationToken cancellationToken, params object[] ids);

    /// <summary>
    /// Lists entities matching the given specification asynchronously.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the list of entities.</returns>
    Task<Result<IEnumerable<T>>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    /// <summary>
    /// Lists all entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the list of entities.</returns>
    Task<Result<IEnumerable<T>>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the first entity matching the given specification asynchronously, or null if none found.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the first entity if found; otherwise, null.</returns>
    Task<Result<T?>> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the first entity asynchronously, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the first entity if found; otherwise, null.</returns>
    Task<Result<T?>> FirstOrDefaultAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Counts the number of entities matching the given specification asynchronously.
    /// </summary>
    /// <param name="spec">The specification to filter entities.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the count of entities.</returns>
    Task<Result<int>> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate read-only repository interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
// TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider using asynchronous streams (IAsyncEnumerable) for large data sets to improve scalability and performance.
