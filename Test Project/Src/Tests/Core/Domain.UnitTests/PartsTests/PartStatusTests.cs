namespace IndTrace.Domain.UnitTests.PartsTests;

/// <summary>
/// Unit tests for PartStatus
/// </summary>
public class PartStatusTests
{
    /// <summary>
    /// Executes PartStatus_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully operation.
    /// </summary>
    [Fact]
    public void PartStatus_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully()
    {
        // Arrange & Act
        var instance = new PartStatus();

        // Assert
        instance.ShouldNotBeNull();
        instance.ShouldBeAssignableTo<EnumModel>();
    }
    /// <summary>
    /// Executes PartStatus_WhenDefaultValues_ShouldInitializePropertiesCorrectly operation.
    /// </summary>

    [Fact]
    public void PartStatus_WhenDefaultValues_ShouldInitializePropertiesCorrectly()
    {
        // Arrange & Act
        var status = new PartStatus();

        // Assert
        status.HasValue.ShouldBe(false);
    }
    /// <summary>
    /// Executes StaticProperties_WhenAccessed_WhenQueried_ShouldReturnExpectedData operation.
    /// </summary>
    /// <param name="expectedValue">The expectedValue.</param>
    /// <param name="expectedName">The expectedName.</param>
    /// <param name="expectedDisplay">The expectedDisplay.</param>

    [Theory]
    [InlineData(-1, "Invalid", "Invalid Value")]
    [InlineData(0, "None", "None")]
    [InlineData(1, "Ok", "Ok")]
    [InlineData(2, "NOk", "nOK")]
    [InlineData(4, "Restored", "Restored")]
    [InlineData(8, "Rejected", "Rejected")]
    [InlineData(512, "Scrap", "Scrap")]
    public void StaticProperties_WhenAccessed_WhenQueried_ShouldReturnExpectedData(int expectedValue, string expectedName, string expectedDisplay)
    {
        // Arrange & Act
        var status = expectedName switch
        {
            "Invalid" => PartStatus.Invalid,
            "None" => PartStatus.None,
            "Ok" => PartStatus.Ok,
            "NOk" => PartStatus.NOk,
            "Restored" => PartStatus.Restored,
            "Rejected" => PartStatus.Rejected,
            "Scrap" => PartStatus.Scrap,
            _ => throw new ArgumentException($"Unknown status: {expectedName}")
        };

        // Assert
        status.Value.ShouldBe(expectedValue);
        status.Name.ShouldBe(expectedDisplay);
    }
    /// <summary>
    /// Executes ImplicitOperator_FromIntToPartStatus_ShouldConvertCorrectly operation.
    /// </summary>
    /// <param name="value">The value.</param>

    [Theory]
    [InlineData(1)] // Ok
    [InlineData(2)] // NOk
    [InlineData(4)] // Restored
    [InlineData(8)] // Rejected
    [InlineData(512)] // Scrap
    public void ImplicitOperator_FromIntToPartStatus_ShouldConvertCorrectly(int value)
    {
        // Arrange & Act
        PartStatus status = value;

        // Assert
        status.ShouldNotBeNull();
        status.Value.ShouldBe(value);
    }
    /// <summary>
    /// Executes ManufacturingPartStatusScenarios_WithDifferentProducts_ShouldHandleCorrectly operation.
    /// </summary>
    /// <param name="statusValue">The statusValue.</param>
    /// <param name="scenario">The scenario.</param>

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(512)]
    public void ManufacturingPartStatusScenarios_WithDifferentProducts_ShouldHandleCorrectly(int statusValue)
    {
        // Arrange
        var partStatus = PartStatus.FromValue(statusValue);

        // Act & Assert
        partStatus.ShouldNotBeNull();
        partStatus.Value.ShouldBe(statusValue);

        // Verify power-of-two values for bitwise operations
        if (statusValue > 0)
        {
            bool IsPowerOfTwo(int num) => num > 0 && (num & (num - 1)) == 0;
            IsPowerOfTwo(statusValue).ShouldBeTrue($"Status value {statusValue} should be power of two for bitwise operations");
        }
    }
    /// <summary>
    /// Executes FromValue_WithValidValues_ShouldReturnCorrectPartStatus operation.
    /// </summary>

    [Fact]
    public void FromValue_WithValidValues_ShouldReturnCorrectPartStatus()
    {
        // Arrange & Act
        var okStatus = PartStatus.FromValue(1);
        var nokStatus = PartStatus.FromValue(2);
        var restoredStatus = PartStatus.FromValue(4);

        // Assert
        okStatus.ShouldBe(PartStatus.Ok);
        nokStatus.ShouldBe(PartStatus.NOk);
        restoredStatus.ShouldBe(PartStatus.Restored);
    }
}
