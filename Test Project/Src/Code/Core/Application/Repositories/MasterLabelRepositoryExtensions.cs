// <copyright file="MasterLabelRepositoryExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repositories;

using Microsoft.Extensions.Caching.Hybrid;

/// <summary>
/// Provides extension methods for <see cref="IReadOnlyRepository{MasterLabel}"/> to support common master label queries and caching.
/// </summary>
public static class MasterLabelRepositoryExtensions
{
    /// <summary>
    /// Gets a list of master label codes by part number, using cache if available.
    /// </summary>
    /// <param name="masterLabelRepository">The master label repository.</param>
    /// <param name="partNumber">The part number to search for.</param>
    /// <param name="cache">The hybrid cache instance.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of master label codes, or a failure result if not found.</returns>
    public static async Task<Result<List<string>>> GetMasterLabelByPartNumberAsync(
        this IReadOnlyRepository<MasterLabel> masterLabelRepository,
        string partNumber,
        HybridCache cache,
        CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"MasterLabel_{nameof(MasterLabel)}_{partNumber}";

            return await cache.GetOrCreateAsync<Result<List<string>>>(
                cacheKey,
                ct => new ValueTask<Result<List<string>>>(GetMasterLabelByPartNumberAsyncFromDb(masterLabelRepository, partNumber, ct)),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Result<List<string>>.WithFailure($"Error retrieving master labels: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a list of master label codes by part number directly from the database.
    /// </summary>
    /// <param name="masterLabelRepository">The master label repository.</param>
    /// <param name="partNumber">The part number to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of master label codes, or a failure result if not found.</returns>
    public static async Task<Result<List<string>>> GetMasterLabelByPartNumberAsyncFromDb(
        this IReadOnlyRepository<MasterLabel> masterLabelRepository,
        string partNumber,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<MasterLabel>(b => b.MasterLabelCode.Contains(partNumber));
        var masterLabels = await masterLabelRepository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        if (!masterLabels.IsFailure || masterLabels.Value is null || !masterLabels.Value.Any())
        {
            return Result<List<string>>.WithFailure("No master labels found matching the criteria");
        }

        var result = masterLabels.Value.Select(ml => ml.MasterLabelCode).ToList();
        return Result<List<string>>.Success(result);
    }
}
