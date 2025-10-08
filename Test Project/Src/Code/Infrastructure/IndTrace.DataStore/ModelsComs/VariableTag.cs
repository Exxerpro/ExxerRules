namespace IndTrace.DataStore.ModelsComs;

/// <summary>
/// Represents a variable tag for a PLC, including its name, address, type, and values.
/// </summary>
public class VariableTag
{
    /// <summary>
    /// Gets or sets the name of the variable tag.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// Gets or sets the address of the variable tag.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Address { get; set; } = null!;
    /// <summary>
    /// Gets or sets the network type of the variable tag.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string NetType { get; set; } = null!;
    /// <summary>
    /// Gets or sets the .NET type of the variable tag.
    /// </summary>
    public Type Type { get; set; } = null!;
    /// <summary>
    /// Gets or sets the integer value of the variable tag.
    /// </summary>
    public int ValueInt { get; set; }
    /// <summary>
    /// Gets or sets the string value of the variable tag.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string ValueString { get; set; } = null!;
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate VariableTag logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
