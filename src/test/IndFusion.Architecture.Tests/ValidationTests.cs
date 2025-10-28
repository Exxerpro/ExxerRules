namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Unit tests for validation architecture rules
/// </summary>
public class ValidationTests
{
    /// <summary>
    /// Verifies that FlowStatus enumeration does not have duplicate names.
    /// </summary>
    [Fact]
    public void FlowStatus_Should_Not_Have_Duplicate_Names()
    {
        var flowStatusFields = typeof(FlowStatus)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.FieldType == typeof(FlowStatus))
            .Select(f => (FlowStatus)f.GetValue(null)!);

        var names = flowStatusFields.Select(fs => fs.Name).ToList();

        Assert.Equal(names.Count(), names.Distinct().Count());
    }
}