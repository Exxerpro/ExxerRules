namespace IndTrace.Domain.UnitTests.EnumTests;
/// <summary>
/// Represents the EnumerationTestsFlowStatus.
/// </summary>

public class EnumerationTestsFlowStatus
{
    /// <summary>
    /// Executes GetInvalidEnum operation.
    /// </summary>
    [Fact]
    public void GetInvalidEnum()
    {
        // Arrange & Act
        var statusCicloEnums = EnumModel.FromValue<FlowStatus>(-1);

        // Assert
        statusCicloEnums.ShouldNotBeNull();
        statusCicloEnums.Name.ShouldBe("Invalid");
    }
    /// <summary>
    /// Executes GetInvalidEnumWhenValueIsNotValid operation.
    /// </summary>

    [Fact]
    public void GetInvalidEnumWhenValueIsNotValid()
    {
        // Arrange & Act
        var statusCicloEnums = EnumModel.FromValue<FlowStatus>(-10);

        // Assert
        statusCicloEnums.ShouldNotBeNull();
        statusCicloEnums.Name.ShouldBe("Invalid");
    }
}
