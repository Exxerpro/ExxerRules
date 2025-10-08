using IndTrace.DataStore.Models;

namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines methods for accessing product data from the repository.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IProductRepository logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IProductRepository
{
    /// <summary>
    /// Gets the list of active products asynchronously.
    /// </summary>
    /// <returns>An enumerable of active products.</returns>
    Task<IEnumerable<Product>> GetActiveProductsAsync();
}
