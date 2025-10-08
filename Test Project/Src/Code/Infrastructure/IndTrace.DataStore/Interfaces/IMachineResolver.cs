namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines a resolver for obtaining the sequence of machines for a product.
/// </summary>
public interface IMachineResolver
{
    /// <summary>
    /// Gets the sequence of machine IDs for the specified product asynchronously.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <returns>A list of machine IDs representing the workflow sequence.</returns>
    Task<List<int>> GetMachineSequenceAsync(int productId);
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IMachineResolver logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
