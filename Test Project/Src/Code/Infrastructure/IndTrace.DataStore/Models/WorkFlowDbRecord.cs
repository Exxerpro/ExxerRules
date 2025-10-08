namespace IndTrace.DataStore.Models;

/// <summary>
/// Represents a workflow record in the database.
/// </summary>
public class WorkFlowDbRecord
{
    /// <summary>
    /// Gets or sets the workflow identifier.
    /// </summary>
    public int WorkFlowId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the next machine identifier in the workflow.
    /// </summary>
    public int NextMachineId { get; set; }
    /// <summary>
    /// Gets or sets the LastMachineId.
    /// </summary>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the last machine identifier in the workflow.
    /// </summary>

    /// <summary>
    /// Represents a workflow record in the database.
    /// </summary>
}
