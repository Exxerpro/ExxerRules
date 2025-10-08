namespace IndTrace.Domain.UnitTests.SettingsTests;

/// <summary>
/// Unit tests for the <see cref="Setting"/> entity.
/// </summary>
public class SettingTests
{
    /// <summary>
    /// Executes Setting_Setting_Properties_ShouldSetAndGetCorrectly operation.
    /// </summary>
    [Fact]
    public void Setting_Setting_Properties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var setting = new Setting();

        // Act
        setting.SettingId = 1;
        setting.MachineId = 1000;
        setting.Config = "{\"key\":\"value\"}";

        // Assert
        setting.SettingId.ShouldBe(1);
        setting.MachineId.ShouldBe(1000);
        setting.Config.ShouldBe("{\"key\":\"value\"}");
    }
}
