// <copyright file="QueryHelpers.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Common;

/// <summary>
/// Centralized queryable acquisition with Result&lt;T&gt; pattern
/// </summary>
/// <remarks>
/// This static class provides manufacturing-grade queryable acquisition helpers
/// that work with any repository type. All methods follow industrial safety
/// compliance patterns with comprehensive error handling.
///
/// Industrial safety compliance:
/// - Complete Result pattern adoption - no exceptions for control flow
/// - Defensive validation on all parameters
/// - Cancellation token support for responsive systems
/// - ConfigureAwait(false) for library code performance
/// - Generic type safety with where constraints
/// </remarks>
public static class QueryHelpers
{
    /// <summary>
    /// Gets queryable from repository with comprehensive error handling
    /// </summary>
    /// <typeparam name="T">The entity type. Must be a reference type.</typeparam>
    /// <param name="repository">The repository to get queryable from. Cannot be null.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A Result containing the queryable or failure information.</returns>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Validates input parameters with CLAUDE.md compliance
    /// 2. Checks for cancellation before repository access
    /// 3. Delegates to repository.AsQueryableAsync with error handling
    /// 4. Returns appropriate success/failure Result
    ///
    /// Performance characteristics:
    /// - Single repository call with no additional overhead
    /// - ConfigureAwait(false) for optimal async performance
    /// - Early cancellation detection
    /// - Exception-safe with Result pattern error handling
    /// </remarks>
    public static async Task<Result<IQueryable<T>>> GetQueryableAsync<T>(
        IRepository<T> repository,
        CancellationToken cancellationToken) where T : class
    {
        if (repository is null)
            return Result<IQueryable<T>>.WithFailure(["Repository cannot be null."]);
        if (cancellationToken.IsCancellationRequested)
            return Result<IQueryable<T>>.WithFailure(["Operation was canceled."]);

        try
        {
            var queryable = await repository.AsQueryableAsync(cancellationToken).ConfigureAwait(false);
            if (queryable.IsSuccess && queryable.Value is not null)
                return Result<IQueryable<T>>.Success(queryable.Value);
            else
                return Result<IQueryable<T>>.WithFailure(queryable.Errors ?? ["Unknown error retrieving queryable"]);
        }
        catch (OperationCanceledException)
        {
            return Result<IQueryable<T>>.WithFailure(["Operation was canceled."]);
        }
        catch (Exception ex)
        {
            return Result<IQueryable<T>>.WithFailure([$"Failed to get queryable: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Gets multiple queryables from different repositories in a single operation
    /// </summary>
    /// <typeparam name="T1">The first entity type. Must be a reference type.</typeparam>
    /// <typeparam name="T2">The second entity type. Must be a reference type.</typeparam>
    /// <param name="repository1">The first repository. Cannot be null.</param>
    /// <param name="repository2">The second repository. Cannot be null.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A Result containing both queryables or failure information.</returns>
    /// <remarks>
    /// Efficiently obtains multiple queryables with proper error handling.
    /// Useful for operations that need multiple entity types.
    /// </remarks>
    public static async Task<Result<(IQueryable<T1> Query1, IQueryable<T2> Query2)>> GetQueryablesAsync<T1, T2>(
        IRepository<T1> repository1,
        IRepository<T2> repository2,
        CancellationToken cancellationToken)
        where T1 : class
        where T2 : class
    {
        if (repository1 is null)
            return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure(["Repository1 cannot be null."]);
        if (repository2 is null)
            return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure(["Repository2 cannot be null."]);
        if (cancellationToken.IsCancellationRequested)
            return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure(["Operation was canceled."]);

        try
        {
            // Get both queryables in parallel for better performance
            var task1 = repository1.AsQueryableAsync(cancellationToken);
            var task2 = repository2.AsQueryableAsync(cancellationToken);

            await Task.WhenAll(task1, task2).ConfigureAwait(false);

            var result1 = await task1.ConfigureAwait(false);
            var result2 = await task2.ConfigureAwait(false);

            if (result1.IsFailure)
                return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure(result1.Errors);
            if (result2.IsFailure)
                return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure(result2.Errors);
            if (result1.Value is null)
                return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure(["First queryable is null"]);
            if (result2.Value is null)
                return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure(["Second queryable is null"]);

            return Result<(IQueryable<T1>, IQueryable<T2>)>.Success((result1.Value, result2.Value));
        }
        catch (OperationCanceledException)
        {
            return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure(["Operation was canceled."]);
        }
        catch (Exception ex)
        {
            return Result<(IQueryable<T1>, IQueryable<T2>)>.WithFailure([$"Failed to get queryables: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Gets three queryables from different repositories in a single operation
    /// </summary>
    /// <typeparam name="T1">The first entity type. Must be a reference type.</typeparam>
    /// <typeparam name="T2">The second entity type. Must be a reference type.</typeparam>
    /// <typeparam name="T3">The third entity type. Must be a reference type.</typeparam>
    /// <param name="repository1">The first repository. Cannot be null.</param>
    /// <param name="repository2">The second repository. Cannot be null.</param>
    /// <param name="repository3">The third repository. Cannot be null.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A Result containing all three queryables or failure information.</returns>
    /// <remarks>
    /// Efficiently obtains three queryables with proper error handling.
    /// Useful for complex operations that need multiple entity types.
    /// </remarks>
    public static async Task<Result<(IQueryable<T1> Query1, IQueryable<T2> Query2, IQueryable<T3> Query3)>> GetQueryablesAsync<T1, T2, T3>(
        IRepository<T1> repository1,
        IRepository<T2> repository2,
        IRepository<T3> repository3,
        CancellationToken cancellationToken)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        if (repository1 is null)
            return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(["Repository1 cannot be null."]);
        if (repository2 is null)
            return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(["Repository2 cannot be null."]);
        if (repository3 is null)
            return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(["Repository3 cannot be null."]);
        if (cancellationToken.IsCancellationRequested)
            return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(["Operation was canceled."]);

        try
        {
            // Get all queryables in parallel for optimal performance
            var task1 = repository1.AsQueryableAsync(cancellationToken);
            var task2 = repository2.AsQueryableAsync(cancellationToken);
            var task3 = repository3.AsQueryableAsync(cancellationToken);

            await Task.WhenAll(task1, task2, task3).ConfigureAwait(false);

            var result1 = await task1.ConfigureAwait(false);
            var result2 = await task2.ConfigureAwait(false);
            var result3 = await task3.ConfigureAwait(false);

            if (result1.IsFailure)
                return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(result1.Errors);
            if (result2.IsFailure)
                return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(result2.Errors);
            if (result3.IsFailure)
                return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(result3.Errors);
            if (result1.Value is null)
                return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(["First queryable is null"]);
            if (result2.Value is null)
                return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(["Second queryable is null"]);
            if (result3.Value is null)
                return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(["Third queryable is null"]);

            return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.Success((result1.Value, result2.Value, result3.Value));
        }
        catch (OperationCanceledException)
        {
            return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure(["Operation was canceled."]);
        }
        catch (Exception ex)
        {
            return Result<(IQueryable<T1>, IQueryable<T2>, IQueryable<T3>)>.WithFailure([$"Failed to get queryables: {ex.Message}"]);
        }
    }
}
