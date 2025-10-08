using IndTrace.DataStore.Interfaces;

namespace IndTrace.DataStore.DataAccess;

/// <summary>
/// Resolves the sequence of machines for a given product based on workflow transitions.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate machine resolver logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the MachineResolver.
/// </summary>
public class MachineResolver(IDbConnection db) : IMachineResolver
{
    /// <summary>
    /// Gets the sequence of machine IDs for the specified product asynchronously.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <returns>A list of machine IDs representing the workflow sequence.</returns>
    public async Task<List<int>> GetMachineSequenceAsync(int productId)
    {
        const string sql = @"SELECT LastMachineId, NextMachineId FROM WorkFlows WHERE ProductId = @ProductId";
        var transitions = (await db.QueryAsync<(int Last, int Next)>(sql, new { ProductId = productId })).ToList();

        var path = new List<int>();
        var current = transitions.FirstOrDefault(t => t.Last == 0).Next;
        while (current != 0)
        {
            path.Add(current);
            current = transitions.FirstOrDefault(t => t.Last == current).Next;
        }
        return path;
    }
}
