namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Represents the result of a fixture validation operation.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IFixtureValidationResult logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IFixtureValidationResult : IEquatable<IFixtureValidationResult>
{
    /// <summary>
    /// Gets the barcode identifier.
    /// </summary>
    public int BarCodeId { get; init; }
    /// <summary>
    /// Gets the barcode string.
    /// </summary>
    public string Barcode { get; init; }
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public int ProductId { get; init; }
    /// <summary>
    /// Gets the cycle identifier.
    /// </summary>
    public int CycleId { get; init; }
    /// <summary>
    /// Gets the cycle status.
    /// </summary>
    public int CycleStatus { get; init; }
    /// <summary>
    /// Gets the part status.
    /// </summary>
    public int PartStatus { get; init; }
    /// <summary>
    /// Gets the flow status.
    /// </summary>
    public int FlowStatus { get; init; }
    /// <summary>
    /// Gets the result validation value.
    /// </summary>
    public int ResultValidation { get; init; }
    /// <summary>
    /// Gets the last machine identifier.
    /// </summary>
    public int LastMachineId { get; init; }
    /// <summary>
    /// Gets the next machine identifier.
    /// </summary>
    public int NextMachineId { get; init; }
    /// <summary>
    /// Gets the cycle machine identifier.
    /// </summary>
    public int CycleMachineId { get; init; }
    /// <summary>
    /// Gets the barcode machine identifier.
    /// </summary>
    public int BarcodeMachineId { get; init; }
    /// <summary>
    /// Gets the actual station identifier.
    /// </summary>
    public int ActualStation { get; init; }
    /// <summary>
    /// Gets the source of the validation result.
    /// </summary>
    public string Source { get; init; }
}
