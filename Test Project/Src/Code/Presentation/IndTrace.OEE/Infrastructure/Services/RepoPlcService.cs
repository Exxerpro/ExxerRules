// <copyright file="RepoPlcService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Infrastructure.Services;

using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.ModelsComs;
using IndTrace.Domain.Models;

/// <summary>
/// Provides methods for loading PLC and tag data from the repository and verifying OEE enablement.
/// </summary>
public class RepoPlcService : IRepoPlcService
{
    private readonly ITagsRepository tagsRepository;
    private readonly ILogger<RepoPlcService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoPlcService"/> class.
    /// </summary>
    /// <param name="tagsRepository">The tags repository dependency.</param>
    /// <param name="logger">The logger dependency.</param>
    public RepoPlcService(
        ITagsRepository tagsRepository,
        ILogger<RepoPlcService> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null for RepoPlcService.");

        if (tagsRepository == null)
        {
            this.logger.LogError("ITagsRepository dependency is null in RepoPlcService constructor.");
            throw new ArgumentNullException(nameof(tagsRepository), "ITagsRepository cannot be null.");
        }

        this.tagsRepository = tagsRepository;
    }

    /// <summary>
    /// Loads PLC data asynchronously from the repository.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A result containing a read-only dictionary of PLC data.</returns>
    public async Task<Result<IReadOnlyDictionary<int, PlcData>>> LoadPlcsDataAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            this.logger.LogWarning("LoadPlcsDataAsync was cancelled before execution.");
            return Result<IReadOnlyDictionary<int, PlcData>>.WithFailure("Operation was cancelled.");
        }

        var resultPlcs = await this.tagsRepository.GetPlcsAsync(cancellationToken).ConfigureAwait(false);
        if (resultPlcs is null || !resultPlcs.Any())
        {
            this.logger.LogWarning("No PLCs found in the database.");
            return Result<IReadOnlyDictionary<int, PlcData>>.WithFailure("No PLCs found in the database.");
        }

        var plcsDict = resultPlcs.ToDictionary(p => p.PlcId, p => p);
        return Result<IReadOnlyDictionary<int, PlcData>>.Success(plcsDict);
    }

    /// <summary>
    /// Loads tag data asynchronously for the specified PLC IDs and tag group.
    /// </summary>
    /// <param name="plcIds">The PLC IDs to load tags for.</param>
    /// <param name="tagGroupId">The tag group ID.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A result containing a read-only dictionary of tag data grouped by PLC ID.</returns>
    public async Task<Result<IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>>>> LoadTagsDataAsync(
        IEnumerable<int> plcIds, int tagGroupId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            this.logger.LogWarning("LoadTagsDataAsync was cancelled before execution.");
            return Result<IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>>>.WithFailure("Operation was cancelled.");
        }

        if (plcIds == null || !plcIds.Any())
        {
            this.logger.LogWarning("plcIds argument is null or empty.");
            return Result<IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>>>.WithFailure("plcIds cannot be null or empty.");
        }

        if (tagGroupId <= 0)
        {
            this.logger.LogWarning("tagGroupId argument is not valid: {TagGroupId}", tagGroupId);
            return Result<IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>>>.WithFailure($"tagGroupId must be greater than zero. Provided: {tagGroupId}");
        }

        var tags = await this.tagsRepository.GetTagsGroupedByMachineAsync(plcIds, tagGroupId, cancellationToken).ConfigureAwait(false);

        if (tags is null || !tags.Any())
        {
            this.logger.LogWarning("No tags found for the given PLCs.");
            return Result<IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>>>.WithFailure("No tags found for the given PLCs.");
        }

        var result = tags
            .ToDictionary(
                outer => outer.Key,
                outer => (IReadOnlyDictionary<string, VariableS7>)outer.Value.ToDictionary(
                    inner => inner.Key,
                    inner => inner.Value));

        return Result<IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>>>.Success(result);
    }

    /// <summary>
    /// Verifies that OEE is enabled for the given PLCs and performance tags.
    /// </summary>
    /// <param name="plcs">The PLCs to verify.</param>
    /// <param name="performanceTags">The performance tags to check.</param>
    /// <returns>A result indicating success or failure, with an error message if any PLCs are missing tags.</returns>
    public Result VerifyOeeIsEnabled(IReadOnlyDictionary<int, PlcData> plcs, IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>> performanceTags)
    {
        var result = 0;
        foreach (var plc in plcs.Values)
        {
            this.logger.LogInformation("PLC found: {PlcId}, Name: {Name}, IP: {IpAddress}", plc.PlcId, plc.Name, plc.IpAddress);

            if (performanceTags.TryGetValue(plc.PlcId, out var tags) && tags.Any())
            {
                foreach (var tag in tags.Values)
                {
                    this.logger.LogInformation("TagDataStore found for PLC {PlcId}: {TagName} at {Address}", plc.PlcId, tag.Name, tag.Address);
                }

                plc.OeeEnabled = true;
            }
            else
            {
                // This validation is redundant, but we made it to ensure that the PLC is disabled if no tags are found.
                // This is to ensure that the PLC is not enabled if no tags are found.
                // Just to be very defensive in the code.
                result++;
                plc.OeeEnabled = false;
                plc.Enabled = -1;
                this.logger.LogError("No tags found for PLC {PlcId}. PLC will be disabled", plc.PlcId);
                this.logger.LogWarning("No tags found for PLC {PlcId}.", plc.PlcId);
            }
        }

        return result > 0
            ? Result.WithFailure($"There are {result} PLCs without tags. Please check the configuration.")
            : Result.Success();
    }
}
