namespace IndTrace.DataStore.Models;

/// <summary>
/// Represents a product with its identifier, part number, and line association.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// Gets or sets the part number of the product.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the line identifier associated with the product.
    /// </summary>
    public int LineId { get; set; }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate product model logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
