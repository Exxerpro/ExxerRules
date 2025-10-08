namespace IndTrace.DataStore.ModelsComs;

/// <summary>
/// Represents database information for a PLC, including address and block number.
/// </summary>
public class DbInfo
{
    /// <summary>
    /// Gets or sets the address of the database.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Address { get; set; } = null!;
    /// <summary>
    /// Gets or sets the block number of the database.
    /// </summary>
    public int BlockNumber { get; set; }
}
