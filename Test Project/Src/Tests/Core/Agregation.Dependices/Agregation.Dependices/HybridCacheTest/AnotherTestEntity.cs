namespace IndTrace.Agregation.Dependices.HybridCacheTest;

/// <summary>
/// Represents another test entity with additional properties for testing.
/// </summary>
public class AnotherTestEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the entity.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the entity is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }
}
