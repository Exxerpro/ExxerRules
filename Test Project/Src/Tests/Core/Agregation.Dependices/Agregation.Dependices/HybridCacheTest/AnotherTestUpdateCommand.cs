using IndTrace.Application.Generic.Commands.Update;

namespace IndTrace.Agregation.Dependices.HybridCacheTest;

/// <summary>
/// Represents another test update command used for testing multiple entity types.
/// </summary>
public class VariableTestUpdateCommand : IUpdateCommand<Variable>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity to update.
    /// </summary>
    public int EntitieId { get; set; }

    /// <summary>
    /// Gets or sets the title of the entity.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the entity is enabled.
    /// </summary>
    public int MachineId { get; set; }
}
