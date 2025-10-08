using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.Models;

namespace IndTrace.DataStore.DataAccess;

/// <summary>
/// Provides methods for accessing product data from the database.
/// </summary>
public class ProductRepository(IDbConnection db) : IProductRepository
{
    /// <summary>
    /// Gets the list of active products asynchronously.
    /// </summary>
    /// <returns>An enumerable of active products.</returns>
    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        const string sql = @"SELECT ProductId, PartNumber, LineId FROM Products WHERE IsActive = 1";
        return await db.QueryAsync<Product>(sql).ConfigureAwait(false);
    }
}
