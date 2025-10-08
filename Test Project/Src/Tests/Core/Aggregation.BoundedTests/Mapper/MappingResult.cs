namespace IndTrace.Aggregation.BoundedTests.Mapper;
/// <summary>
/// Represents the MappingResult.
/// </summary>

public class MappingResult
{
    /// <summary>
    /// Gets or sets the SourceType.
    /// </summary>
    public string SourceType { get; set; } = "";
    /// <summary>
    /// Gets or sets the TargetType.
    /// </summary>
    public string TargetType { get; set; } = "";
    /// <summary>
    /// Gets or sets the Property.
    /// </summary>
    public string Property { get; set; } = "";
    /// <summary>
    /// Gets or sets the Status.
    /// </summary>
    public string Status { get; set; } = "";
    /// <summary>
    /// Gets or sets the SourceValue.
    /// </summary>
    public string SourceValue { get; set; } = "";
    /// <summary>
    /// Gets or sets the TargetValue.
    /// </summary>
    public string TargetValue { get; set; } = "";
    /// <summary>
    /// Executes ToString operation.
    /// </summary>
    /// <returns>The result of ToString.</returns>

    public override string ToString()
    {
        return
            $"[{Status}] {SourceType} → {TargetType} | Property: '{Property}' | Source Value: {SourceValue}, Target Value: {TargetValue}";
    }
}
