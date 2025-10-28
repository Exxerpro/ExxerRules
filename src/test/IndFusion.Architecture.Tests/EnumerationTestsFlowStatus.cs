namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Unit tests for FlowStatus enumeration behavior
/// </summary>
public class EnumerationTestsFlowStatus
{
    /// <summary>
    /// Verifies that GetInvalidEnum returns an invalid enum when an invalid value is provided.
    /// </summary>
    [Fact]
    public void GetInvalidEnum()
    {
        // Arrange & Act
        var statusCicloEnums = EnumModel.FromValue<FlowStatus>(-1);

        // Assert
        Assert.NotNull(statusCicloEnums);
        Assert.Equal("Invalid", statusCicloEnums.Name);
    }

    /// <summary>
    /// Verifies that GetInvalidEnum returns an invalid enum when a non-valid value is provided.
    /// </summary>
    [Fact]
    public void GetInvalidEnumWhenValueIsNotValid()
    {
        // Arrange & Act
        var statusCicloEnums = EnumModel.FromValue<FlowStatus>(-10);

        // Assert
        Assert.NotNull(statusCicloEnums);
        Assert.Equal("Invalid", statusCicloEnums.Name);
    }
}