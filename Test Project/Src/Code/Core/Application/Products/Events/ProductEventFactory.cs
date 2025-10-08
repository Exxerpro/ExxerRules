namespace IndTrace.Application.Products.Events;

/// <summary>
/// Product event creation without external dependencies.
/// Handles ProductCreatedEvent generation from Product entities using existing patterns.
/// </summary>
public class ProductEventFactory : IProductEventFactory
{
    /// <summary>
    /// Creates a ProductCreatedEvent from a Product entity.
    /// Leverages existing ProductCreatedEvent.FromProduct method for consistency.
    /// </summary>
    public Result<ProductCreatedEvent> CreateProductCreatedEvent(Product product)
    {
        if (product is null)
        {
            return Result<ProductCreatedEvent>.WithFailure("Product cannot be null for event creation.");
        }

        // Use the existing static factory method which already handles:
        // - Null checks
        // - Property mapping (ProductName → Name)
        // - String null safety (null → string.Empty)
        // - All required transformations
        return ProductCreatedEvent.FromProduct(product);
    }

    /// <summary>
    /// Validates product entity readiness for event creation.
    /// Ensures product has all required data before creating events.
    /// </summary>
    public Result ValidateProductForEventCreation(Product product)
    {
        if (product is null)
        {
            return Result.WithFailure("Product cannot be null.");
        }

        var errors = new List<string>();

        // ProductId must be assigned (greater than 0)
        if (product.ProductId <= 0)
        {
            errors.Add("ProductId must be assigned and greater than 0 before creating events.");
        }

        // Essential properties validation
        if (string.IsNullOrWhiteSpace(product.PartNumber))
        {
            errors.Add("Product PartNumber is required for event creation.");
        }

        if (string.IsNullOrWhiteSpace(product.ProductName))
        {
            errors.Add("Product ProductName is required for event creation.");
        }

        // CustomerId must be valid
        if (product.CustomerId <= 0)
        {
            errors.Add("Product must have a valid CustomerId for event creation.");
        }

        // Ensure product is in a valid state
        if (product.IsActive < 0)
        {
            errors.Add("Product IsActive status must be valid (0 or greater).");
        }

        return errors.Count > 0
            ? Result.WithFailure(errors)
            : Result.Success();
    }

    /// <summary>
    /// Creates event payload with enhanced metadata.
    /// Validates product state before creating events to ensure data integrity.
    /// </summary>
    public Result<ProductCreatedEvent> CreateEnhancedProductCreatedEvent(Product product, ProductCreationContext context)
    {
        if (product is null)
        {
            return Result<ProductCreatedEvent>.WithFailure("Product cannot be null for enhanced event creation.");
        }

        if (context is null)
        {
            return Result<ProductCreatedEvent>.WithFailure("ProductCreationContext cannot be null.");
        }

        // Validate product state before creating events
        var validationResult = ValidateProductForEventCreation(product);
        if (validationResult.IsFailure)
        {
            return Result<ProductCreatedEvent>.WithFailure(validationResult.Errors);
        }

        // Create event after validation passes
        var eventResult = CreateProductCreatedEvent(product);

        if (eventResult.IsFailure)
        {
            return eventResult;
        }

        // Future: Enrich event with context metadata
        // eventResult.Value.Metadata = context.AdditionalMetadata;
        // eventResult.Value.CreatedBy = context.CreatedBy;
        // eventResult.Value.Source = context.Source;

        return eventResult;
    }
}
