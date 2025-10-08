namespace IndTrace.DataStore.Models;

/// <summary>
/// Represents a product record in the database.
/// </summary>
public class ProductDbRecord
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// Gets or sets the part number of the product.
    /// </summary>
    public required string PartNumber { get; set; }
    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public required string ProductName { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the product is active.
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Gets or sets the description of the product.
    /// </summary>
    public required string Description { get; set; }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate product DB record logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
