namespace IndTrace.DataStore.ModelsComs;

/// <summary>
/// Represents a tag associated with a PLC, containing an address and a description.
/// </summary>
public class TagDataStore
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TagDataStore"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <param name="address">The address.</param>
    /// <param name="value">The value.</param>
    public TagDataStore(string description, string address, string value)
    {
        this.Address = address;
        this.Description = description;
        this.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TagDataStore"/> class.
    /// Initializes a new instance of the class.
    /// </summary>

    public TagDataStore()
    {
    }

    /// <summary>
    /// Gets or sets the address of the tag.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the tag.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Gets or sets the value of the tag.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Value { get; set; } = null!;

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate TagDataStore logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
