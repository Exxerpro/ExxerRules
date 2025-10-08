using IndTrace.DataStore.ModelsComs;

namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines methods for accessing and grouping PLC tags in the repository.
/// </summary>
public interface ITagsRepository
{
    /// <summary>
    /// Retrieves all PLCs asynchronously from the repository.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An enumerable of PLC data.</returns>
    Task<IEnumerable<PlcData>> GetPlcsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves tags grouped by machine asynchronously for all machines.
    /// </summary>
    /// <param name="cancelationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping machine IDs to their tags.</returns>
    public Task<Dictionary<int, Dictionary<string, VariableS7>>> GetTagsGroupedByMachineAsync(
        CancellationToken cancelationToken);

    /// <summary>
    /// Retrieves tags grouped by machine asynchronously for the specified machine IDs.
    /// </summary>
    /// <param name="machineId">The machine IDs to filter by.</param>
    /// <param name="cancelationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping machine IDs to their tags.</returns>
    public Task<Dictionary<int, Dictionary<string, VariableS7>>> GetTagsGroupedByMachineAsync(
        IEnumerable<int> machineId,
        CancellationToken cancelationToken);

    /// <summary>
    /// Retrieves tags grouped by machine asynchronously for the specified machine IDs and group ID.
    /// </summary>
    /// <param name="machineId">The machine IDs to filter by.</param>
    /// <param name="groupId">The group ID to filter by.</param>
    /// <param name="cancelationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping machine IDs to their tags.</returns>
    public Task<Dictionary<int, Dictionary<string, VariableS7>>> GetTagsGroupedByMachineAsync(IEnumerable<int> machineId, int groupId,
        CancellationToken cancelationToken);

    /// <summary>
    /// Retrieves tags for a specific machine asynchronously.
    /// </summary>
    /// <param name="machineId">The machine ID to filter by.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A dictionary mapping tag names to their VariableS7 objects.</returns>
    public Task<Dictionary<string, VariableS7>> GetTagsAsync(int machineId, CancellationToken cancellationToken);
}
