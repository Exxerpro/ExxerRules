namespace IndTrace.TestData.Models;

/// <summary>
/// Represents the result of a data validation operation.
/// </summary>
internal sealed class DataValidationResult
{
    public List<string> FilesValidated { get; } = new();
    public Dictionary<string, List<DuplicateEntry>> FilesWithDuplicates { get; } = new();
    public List<DataValidationError> Errors { get; } = new();
    public int TotalDuplicatesFound { get; set; }

    public bool IsValid => TotalDuplicatesFound == 0 && Errors.Count == 0;
}
