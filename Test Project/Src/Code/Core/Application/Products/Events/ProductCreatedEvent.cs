using IndTrace.Application.Models.Interfaces;
using IndTrace.Application.Notifications.Models;

namespace IndTrace.Application.Products.Events;

/// <summary>
/// Domain event raised when a Product is successfully created.
/// Pure POCO event containing product data for downstream consumers.
/// </summary>
public record ProductCreatedEvent : INotification
{
    public int ProductId { get; init; }
    public string PartNumber { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty; // Maps from ProductName
    public string Description { get; init; } = string.Empty;
    public string CustomerPartNumber { get; init; } = string.Empty;
    public string AliasPartNumber { get; init; } = string.Empty;
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int LineId { get; init; }
    public int IsActive { get; init; }
    public int Version { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedOn { get; init; }
    public string ModifiedBy { get; init; } = string.Empty;
    public DateTime ModifiedOn { get; init; }
    public int RuleId { get; init; }

    /// <summary>
    /// Creates a ProductCreatedEvent from a Product entity.
    /// Handles property name mapping (ProductName → Name) and null safety.
    /// </summary>
    /// <param name="product">Product entity to create event from</param>
    /// <returns>Result containing ProductCreatedEvent or failure</returns>
    public static Result<ProductCreatedEvent> FromProduct(Product product)
    {
        if (product is null)
        {
            return Result<ProductCreatedEvent>.WithFailure("Product cannot be null for event creation.");
        }

        var eventData = new ProductCreatedEvent
        {
            ProductId = product.ProductId,
            PartNumber = product.PartNumber ?? string.Empty,
            Name = product.ProductName ?? string.Empty, // Key mapping: ProductName → Name
            Description = product.Description ?? string.Empty,
            CustomerPartNumber = product.CustomerPartNumber ?? string.Empty,
            AliasPartNumber = product.AliasPartNumber ?? string.Empty,
            CustomerId = product.CustomerId,
            CustomerName = product.CustomerName ?? string.Empty,
            LineId = product.LineId,
            IsActive = product.IsActive,
            Version = product.Version,
            CreatedBy = product.CreatedBy ?? string.Empty,
            CreatedOn = product.CreatedOn ?? DateTime.UtcNow,
            ModifiedBy = product.ModifiedBy ?? string.Empty,
            ModifiedOn = product.ModifiedOn ?? DateTime.UtcNow,
            RuleId = product.RuleId
        };

        return Result<ProductCreatedEvent>.Success(eventData);
    }

    /// <summary>
    /// Creates a ProductCreatedEvent from a Product entity.
    /// Handles property name mapping (ProductName → Name) and null safety.
    /// </summary>
    /// <param name="productCreated">Product entity to create event from</param>
    /// <returns>Result containing ProductCreatedEvent or failure</returns>
    public static Result<Product> ToProduct(ProductCreatedEvent productCreated)
    {
        if (productCreated is null)
        {
            return Result<Product>.WithFailure("Product cannot be null for event creation.");
        }

        var eventData = new Product
        {
            ProductId = productCreated.ProductId,
            PartNumber = productCreated.PartNumber ?? string.Empty,
            ProductName = productCreated.Name ?? string.Empty, // Key mapping: ProductName → Name
            Description = productCreated.Description ?? string.Empty,
            CustomerPartNumber = productCreated.CustomerPartNumber ?? string.Empty,
            AliasPartNumber = productCreated.AliasPartNumber ?? string.Empty,
            CustomerId = productCreated.CustomerId,
            CustomerName = productCreated.CustomerName ?? string.Empty,
            LineId = productCreated.LineId,
            IsActive = productCreated.IsActive,
            Version = productCreated.Version,
            CreatedBy = productCreated.CreatedBy ?? string.Empty,
            CreatedOn = productCreated.CreatedOn,
            ModifiedBy = productCreated.ModifiedBy ?? string.Empty,
            ModifiedOn = productCreated.ModifiedOn,
            RuleId = productCreated.RuleId
        };

        return Result<Product>.Success(eventData);
    }
    /// <summary>
    /// Handler for ProductCreatedEvent.
    /// </summary>
    public class ProductCreatedHandler : INotificationHandler<ProductCreatedEvent>
    {
        private INotificationService notificationService;

        /// <summary>
        /// Constructor with dependency injection for notification service.
        /// </summary>
        /// <param name="notificationService"></param>
        public ProductCreatedHandler(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        /// <summary>
        /// Processes the ProductCreatedEvent.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> Process(ProductCreatedEvent @event, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.WithFailure("Operation was canceled.");
            }

            // Create a notification message using the factory method
            var message = MessageDto.CreateMessage<ProductCreatedEvent>(@event);

            var result = await notificationService.SendAsync(message, cancellationToken);
            return result;
        }

        /// <summary>
        /// Processes the ProductCreatedEvent.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        Task<Result> INotificationHandler<ProductCreatedEvent>.Process(ProductCreatedEvent @event, CancellationToken cancellationToken)
        {
            return Process(@event, cancellationToken);
        }
    }
}
