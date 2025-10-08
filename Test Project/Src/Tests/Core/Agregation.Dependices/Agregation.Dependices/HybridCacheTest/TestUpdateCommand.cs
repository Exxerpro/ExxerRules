using IndTrace.Application.Generic.Commands.Update;

namespace IndTrace.Agregation.Dependices.HybridCacheTest;

/// <summary>
/// Represents a test update command for TestEntity.
/// </summary>
public class TestUpdateCommand : IUpdateCommand<Register>
{
    /// <summary>
    /// Gets or sets the unique identifier for the command.
    /// </summary>
    public int EntitieId { get; set; }

    /// <summary>
    /// Gets or sets the name to update in the entity.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last modified date and time for the update.
    /// </summary>
    public DateTime TimeStamp { get; set; }
}
