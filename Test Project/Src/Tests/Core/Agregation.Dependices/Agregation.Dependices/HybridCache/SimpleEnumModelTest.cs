using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Persistence.Converters;
using System.Text.Json;

namespace IndTrace.Agregation.Dependices.HybridCache;

/// <summary>
/// Simple test to verify EnumModel serialization works correctly.
/// </summary>
public class SimpleEnumModelTest
{
    [Fact]
    public void EnumModelJsonConverter_RoundTrip_ShouldWork()
    {
        // Arrange
        var original = MachineType.Printer;
        var options = new JsonSerializerOptions();
        options.Converters.Add(new EnumModelJsonConverter());

        // Act - Serialize
        var json = JsonSerializer.Serialize(original, options);

        // Act - Deserialize
        var deserialized = JsonSerializer.Deserialize<MachineType>(json, options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Value.ShouldBe(original.Value);
        deserialized.Name.ShouldBe(original.Name);
        deserialized.DisplayName.ShouldBe(original.DisplayName);
    }

    [Fact]
    public void MachineType_Properties_ShouldNotBeNull()
    {
        // Arrange & Act
        var machineType = MachineType.Printer;

        // Assert - This is the core issue we're fixing
        machineType.Name.ShouldNotBeNull();
        machineType.DisplayName.ShouldNotBeNull();
        machineType.Value.ShouldBeGreaterThan(0);

        machineType.Name.ShouldBe("Printer");
        machineType.Value.ShouldBe(1);
    }
}
