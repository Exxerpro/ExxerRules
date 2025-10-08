namespace IndTrace.DataStore.ModelsComs;

/// <summary>
/// Represents a PLC record in the database, including machine ID, name, and IP address.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate PlcRecordDb logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the PlcRecordDb.
/// </summary>
public class PlcRecordDb
{
    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }
    /// <summary>
    /// Gets or sets the name of the PLC.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// Gets or sets the IP address of the PLC.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string IpAddress { get; set; } = null!;
}
