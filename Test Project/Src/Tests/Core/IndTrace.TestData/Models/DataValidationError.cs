namespace IndTrace.TestData.Models;

/// <summary>
/// Represents an error that occurred during data validation.
/// </summary>
internal sealed class DataValidationError
{
    public required string FileName { get; set; }
    public required string EntityType { get; set; }
    public required string ErrorMessage { get; set; }
}
