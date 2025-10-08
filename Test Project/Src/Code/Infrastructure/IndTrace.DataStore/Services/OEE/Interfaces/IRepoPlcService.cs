using IndTrace.DataStore.ModelsComs;
using IndTrace.Domain.Models;

namespace IndTrace.DataStore.Services.OEE.Interfaces;

/// <summary>
/// Defines a contract for a service that manages PLC data repository operations.
/// </summary>
public interface IRepoPlcService
{
    /// <summary>
    /// Loads PLC data asynchronously from the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, containing a dictionary of PLC data.</returns>
    Task<Result<IReadOnlyDictionary<int, PlcData>>> LoadPlcsDataAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Loads tag data asynchronously for specified PLCs and tag group.
    /// </summary>
    /// <param name="plcIds">The collection of PLC IDs to load tags for.</param>
    /// <param name="tagGroupId">The ID of the tag group to load.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, containing a nested dictionary of PLC tags.</returns>
    Task<Result<IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>>>> LoadTagsDataAsync(IEnumerable<int> plcIds, int tagGroupId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifies that OEE functionality is enabled for the specified PLCs and their tags.
    /// </summary>
    /// <param name="plcs">The dictionary of PLC data to verify.</param>
    /// <param name="performanceTags">The nested dictionary of performance tags to verify.</param>
    /// <returns>A result indicating whether OEE is enabled and properly configured.</returns>
    Result VerifyOeeIsEnabled(IReadOnlyDictionary<int, PlcData> plcs, IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>> performanceTags);
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IRepoPlcService logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
