using IndTrace.DataStore.Models;

namespace IndTrace.DataStore.DataAccess;

/// <summary>
/// Represents a static snapshot of a fixture, including product and workflow information.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture static snapshot logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the FixtureStaticSnapshot.
/// </summary>
public class FixtureStaticSnapshot
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }
    /// <summary>
    /// Gets or sets the part number.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether the product is active.
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the list of workflow records associated with the product.
    /// </summary>
    public List<WorkFlowDbRecord> WorkFlows { get; set; } = [];
}
