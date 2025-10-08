namespace IndTrace.DataStore.DataAccess;

/// <summary>
/// Represents a record for a barcode in the PLC database.
/// </summary>
public class BarcodeDbRecord
{
    /// <summary>
    /// Gets or sets the barcode string.
    /// </summary>
    public required string Barcode { get; set; }
    /// <summary>
    /// Gets or sets the barcode identifier.
    /// </summary>
    public int BarCodeId { get; set; }              // Needed for downstream logic
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }              // Optional if statically known
    /// <summary>
    /// Gets or sets the machine identifier where the barcode originated.
    /// </summary>
    public int MachineId { get; set; }              // Barcode's machine origin
    /// <summary>
    /// Gets or sets the flow status.
    /// </summary>
    public int FlowStatus { get; set; }
    /// <summary>
    /// Gets or sets the part status.
    /// </summary>
    public int PartStatus { get; set; }
    /// <summary>
    /// Gets or sets the cycle status.
    /// </summary>
    public int CycleStatus { get; set; }
    /// <summary>
    /// Gets or sets the cycle identifier.
    /// </summary>
    public int CycleId { get; set; }
    /// <summary>
    /// Gets or sets the cycle machine identifier.
    /// </summary>
    public int CycleMachineId { get; set; }
    /// <summary>
    /// Gets or sets the result validation value.
    /// </summary>
    public int ResultValidation { get; set; }
    /// <summary>
    /// Gets or sets the last modified date and time.
    /// </summary>
    public DateTime? LastModified { get; set; }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate barcode DB record logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
