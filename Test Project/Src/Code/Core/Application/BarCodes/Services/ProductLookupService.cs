using IndTrace.Application.BarCodes.Services.Interfaces;

namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Handles product lookup and validation for barcode creation.
/// Ensures products exist before barcode generation can proceed.
/// </summary>
public class ProductLookupService : IProductLookupService
{
    private readonly IReadOnlyRepository<Product> _productRepository;
    private readonly ILogger<ProductLookupService> _logger;

    public ProductLookupService(
        IReadOnlyRepository<Product> productRepository,
        ILogger<ProductLookupService> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a product by its part number.
    /// Implements business rule: Product must exist for barcode creation.
    /// </summary>
    /// <param name="partNumber">The part number to search for</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Success with Product entity if found, Failure if not found</returns>
    public async Task<Result<Product>> GetProductByPartNumberAsync(string partNumber, CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<Product>.WithFailure(["Operation was canceled."]);
        }

        // Null guard for dependencies
        if (_productRepository is null)
        {
            return Result<Product>.WithFailure(["_productRepository cannot be null."]);
        }

        // Input validation
        if (string.IsNullOrWhiteSpace(partNumber))
        {
            return Result<Product>.WithFailure(["Part number cannot be null or empty."]);
        }

        try
        {
            _logger.LogDebug("Looking up product by part number: {PartNumber}", partNumber);

            // Create specification for part number lookup
            var specification = new Specification<Product>(p => p.PartNumber == partNumber);

            var productResult = await _productRepository.FirstOrDefaultAsync(specification, cancellationToken);

            if (productResult.IsFailure || productResult.Value is null)
            {
                _logger.LogWarning("Product lookup failed: Product for part number {PartNumber} does not exist", partNumber);
                return Result<Product>.WithFailure([$"product for  {partNumber} does not exist"]);
            }

            var product = productResult.Value;
            _logger.LogDebug("Product lookup successful: Found ProductId={ProductId} for PartNumber={PartNumber}",
                product.ProductId, partNumber);

            return Result<Product>.Success(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Product lookup error for PartNumber={PartNumber}", partNumber);
            return Result<Product>.WithFailure([$"Product lookup failed: {ex.Message}"]);
        }
    }
}
