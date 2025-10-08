namespace IndTrace.TestData.Models;

/// <summary>
/// Represents the validation result for a JSON file.
/// </summary>
internal sealed class JsonFileValidationResult
{
    public required string FileName { get; set; }
    public bool IsValid { get; set; }
    public string Error { get; set; } = string.Empty;
    public int RecordCount { get; set; }
}

//[Fix] CLAUDE - Date: 26/08/2025
//Reason: [CS8632] - Removed nullable annotation (string?) from Error to eliminate nullable reference type warnings since project has <Nullable>disable</Nullable>
