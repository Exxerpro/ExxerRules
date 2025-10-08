namespace IndTrace.Application.BarCodes.Services.Interfaces;

/// <summary>
/// Handles product lookup and validation for barcode creation.
/// Provides product entity retrieval by part number.
/// </summary>
public interface IProductLookupService
{
    /// <summary>
    /// Retrieves a product by its part number.
    /// </summary>
    /// <param name="partNumber">The part number to search for</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Success with Product entity if found, Failure if not found</returns>
    Task<Result<Product>> GetProductByPartNumberAsync(string partNumber, CancellationToken cancellationToken);
}
