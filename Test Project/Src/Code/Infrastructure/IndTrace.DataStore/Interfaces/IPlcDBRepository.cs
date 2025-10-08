using IndTrace.DataStore.ModelsComs;

namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines methods for accessing PLC database information and virtual PLCs.
/// </summary>
public interface IPlcDBRepository
{
    /// <summary>
    /// Retrieves all virtual PLCs asynchronously from the repository.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An enumerable of virtual PLC data.</returns>
    Task<IEnumerable<PlcData>> GetPlcsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves distinct database information asynchronously from the repository.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An enumerable of distinct database information.</returns>
    Task<IEnumerable<Db>> GetDistinctDbInfosAsync(CancellationToken cancellationToken);
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IPlcDBRepository logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
