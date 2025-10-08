using IndTrace.DataStore.ModelsComs;

namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Represents the context for a fixture, including part, barcode, product, machine, and task information.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IFixtureContext logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IFixtureContext
{
    /// <summary>
    /// Gets or sets the part number.
    /// </summary>
    string PartNumber { get; set; }
    /// <summary>
    /// Gets or sets the barcode.
    /// </summary>
    string Barcode { get; set; }
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    int ProductId { get; set; }
    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    int MachineId { get; set; }
    /// <summary>
    /// Gets or sets the expected state.
    /// </summary>
    string ExpectedState { get; set; }
    /// <summary>
    /// Gets or sets the task name.
    /// </summary>
    string TaskName { get; set; }
    /// <summary>
    /// Gets or sets the tags associated with the fixture.
    /// </summary>
    Dictionary<string, VariableS7> Tags { get; set; }
    /// <summary>
    /// Gets or sets the list of cycle steps (tasks).
    /// </summary>
    List<ICycleStep> Tasks { get; set; }
    /// <summary>
    /// Gets or sets the current cycle step.
    /// </summary>
    ICycleStep CycleStep { get; set; }
    /// <summary>
    /// Returns a string representation of the fixture context.
    /// </summary>
    /// <returns>A string describing the fixture context.</returns>
    string ToString();
}
