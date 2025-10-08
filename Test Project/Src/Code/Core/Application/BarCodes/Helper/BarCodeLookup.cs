// <copyright file="BarCodeLookup.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Helper;

/// <summary>
/// Pure database lookup helpers with comprehensive error handling
/// </summary>
/// <remarks>
/// This static class provides manufacturing-grade database lookup operations
/// for BarCode entities. All methods are pure helper functions with no side
/// effects beyond database queries.
///
/// Industrial safety compliance:
/// - Complete Result pattern adoption - no exceptions for control flow
/// - Defensive validation with 3-character minimums
/// - Cancellation token support for responsive systems
/// - ConfigureAwait(false) for library code performance
/// - Structured error messages for manufacturing traceability
/// </remarks>
public static class BarCodeLookup
{
    /// <summary>
    /// Fetches BarCode by label with validation and error handling.
    /// Pure helper function with no side effects - manufacturing-grade safety.
    /// </summary>
    /// <param name="repository">The repository to query. Cannot be null.</param>
    /// <param name="label">The label to search for. Must be at least 3 characters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A Result containing the found BarCode or failure information.</returns>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Validates all input parameters with CLAUDE.md compliance
    /// 2. Checks for cancellation before database access
    /// 3. Obtains queryable from repository with error handling
    /// 4. Executes exact label match query
    /// 5. Returns appropriate success/failure Result
    ///
    /// Performance characteristics:
    /// - Single database query with exact match filter
    /// - ConfigureAwait(false) for optimal async performance
    /// - Early cancellation detection
    /// - Exception-safe with Result pattern error handling
    /// </remarks>
    public static async Task<Result<BarCode>> FetchByLabelAsync(
        IRepository<BarCode> repository,
        string label,
        CancellationToken cancellationToken)
    {
        // CLAUDE.md compliance: defensive validation
        if (repository is null)
            return Result<BarCode>.WithFailure(["Repository cannot be null."]);
        if (string.IsNullOrWhiteSpace(label) || label.Length < 3)
            return Result<BarCode>.WithFailure(["Label must be at least 3 characters long."]);
        if (cancellationToken.IsCancellationRequested)
            return Result<BarCode>.WithFailure(["Operation was canceled."]);

        try
        {
            // Get queryable from repository with error handling
            var queryableResult = await repository
                .AsQueryableAsync(cancellationToken)
                .ConfigureAwait(false);

            if (queryableResult.IsFailure)
                return Result<BarCode>.WithFailure(queryableResult.Errors);

            // Execute exact label match query
            var queryable = queryableResult.Value;
            if (queryable is null)
                return Result<BarCode>.WithFailure(["Queryable is null."]);

            var result = await queryable
                .Where(b => b.Label == label)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return result is null
                ? Result<BarCode>.WithFailure(["BarCode not found."])
                : Result<BarCode>.Success(result);
        }
        catch (OperationCanceledException)
        {
            return Result<BarCode>.WithFailure(["Operation was canceled."]);
        }
        catch (Exception ex)
        {
            return Result<BarCode>.WithFailure([$"Lookup failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Fetches BarCode by ID with validation and error handling.
    /// </summary>
    /// <param name="repository">The repository to query. Cannot be null.</param>
    /// <param name="barCodeId">The ID to search for. Must be greater than 0.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A Result containing the found BarCode or failure information.</returns>
    /// <remarks>
    /// Provides ID-based lookup with the same safety guarantees as label lookup.
    /// Useful for cases where the primary key is known.
    /// </remarks>
    public static async Task<Result<BarCode>> FetchByIdAsync(
        IRepository<BarCode> repository,
        int barCodeId,
        CancellationToken cancellationToken)
    {
        // CLAUDE.md compliance: defensive validation
        if (repository is null)
            return Result<BarCode>.WithFailure(["Repository cannot be null."]);
        if (barCodeId <= 0)
            return Result<BarCode>.WithFailure(["BarCodeId must be greater than 0."]);
        if (cancellationToken.IsCancellationRequested)
            return Result<BarCode>.WithFailure(["Operation was canceled."]);

        try
        {
            // Get queryable from repository with error handling
            var queryableResult = await repository
                .AsQueryableAsync(cancellationToken)
                .ConfigureAwait(false);

            if (queryableResult.IsFailure)
                return Result<BarCode>.WithFailure(queryableResult.Errors);

            // Execute exact ID match query
            var queryable = queryableResult.Value;
            if (queryable is null)
                return Result<BarCode>.WithFailure(["Queryable is null."]);

            var result = await queryable
                .Where(b => b.BarCodeId == barCodeId)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return result is null
                ? Result<BarCode>.WithFailure(["BarCode not found."])
                : Result<BarCode>.Success(result);
        }
        catch (OperationCanceledException)
        {
            return Result<BarCode>.WithFailure(["Operation was canceled."]);
        }
        catch (Exception ex)
        {
            return Result<BarCode>.WithFailure([$"Lookup failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Checks if a BarCode with the specified label exists.
    /// </summary>
    /// <param name="repository">The repository to query. Cannot be null.</param>
    /// <param name="label">The label to check for existence. Must be at least 3 characters.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A Result containing true if exists, false if not found, or failure information.</returns>
    /// <remarks>
    /// Optimized existence check that doesn't load the full entity.
    /// Useful for validation scenarios where only existence matters.
    /// </remarks>
    public static async Task<Result<bool>> ExistsAsync(
        IRepository<BarCode> repository,
        string label,
        CancellationToken cancellationToken)
    {
        // CLAUDE.md compliance: defensive validation
        if (repository is null)
            return Result<bool>.WithFailure(["Repository cannot be null."]);
        if (string.IsNullOrWhiteSpace(label) || label.Length < 3)
            return Result<bool>.WithFailure(["Label must be at least 3 characters long."]);
        if (cancellationToken.IsCancellationRequested)
            return Result<bool>.WithFailure(["Operation was canceled."]);

        try
        {
            // Get queryable from repository with error handling
            var queryableResult = await repository
                .AsQueryableAsync(cancellationToken)
                .ConfigureAwait(false);

            if (queryableResult.IsFailure)
                return Result<bool>.WithFailure(queryableResult.Errors);

            // Execute optimized existence check
            var queryable = queryableResult.Value;
            if (queryable is null)
                return Result<bool>.WithFailure(["Queryable is null."]);

            var exists = await queryable
                .Where(b => b.Label == label)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);

            return Result<bool>.Success(exists);
        }
        catch (OperationCanceledException)
        {
            return Result<bool>.WithFailure(["Operation was canceled."]);
        }
        catch (Exception ex)
        {
            return Result<bool>.WithFailure([$"Existence check failed: {ex.Message}"]);
        }
    }
}
