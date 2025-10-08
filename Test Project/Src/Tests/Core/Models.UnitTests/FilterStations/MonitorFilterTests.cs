using Meziantou.Extensions.Logging.Xunit;
using Meziantou.Extensions.Logging.Xunit.v3;

namespace IndTrace.Models.UnitTests.FilterStations;

/// <summary>
/// Represents the MonitorFilterTests.
/// </summary>

public class MonitorFilterTests
{
    private ApplicationConfiguration CreateTestConfiguration()
    {
        return new ApplicationConfiguration
        {
            MachineProductCompatibility =
            [
                new() { MachineId = 10000, PartNumber = "L823566", CustomerId = 2 },
                new() { MachineId = 400, PartNumber = "L823581", CustomerId = 2 },
                new() { MachineId = 500, PartNumber = "422290", CustomerId = 3 }
            ]
        };
    }

    // TODO: Flagged for further analysis — possible behavior change or environment interaction.
    // Not confirmed as bug. Root causes may include:
    // - Changes in .NET 10 async scheduling or timing.
    // - Test relying on external state (e.g. temp files, shared mocks).
    // - Subtle concurrency or initialization differences.
    // → Review after remaining core features and infrastructure are finalized.
    /// <summary>
    /// Executes Should_Filter_ControllerMonitors_By_Customer operation.
    /// </summary>
    [Fact]
    public void Should_Filter_ControllerMonitors_By_Customer()
    {
        var config = CreateTestConfiguration();

        var monitors = new Dictionary<int, ControllerMonitor>
        {
            { 1, new ControllerMonitor { MachineId = 10000, PartNumber = "L823566" } },
            { 2, new ControllerMonitor { MachineId = 400, PartNumber = "L823581" } },
            { 3, new ControllerMonitor { MachineId = 500, PartNumber = "L422290" } },
        };

        var result = monitors.FilterByModel();
        var logger = XUnitLogger.CreateLogger<MonitorFilterTests>();

        foreach (var val in result.Values)
        {
            logger.LogInformation("Part Number {part number}", val.PartNumber);
        }

        result.Count.ShouldBe(1);
        result.Values.ShouldAllBe(m => m.PartNumber == "L422290");
    }

    /// <summary>
    /// Executes Should_Filter_StationMonitors_By_Customer operation.
    /// </summary>

    [Fact]
    public void Should_Filter_StationMonitors_By_Customer()
    {
        var config = CreateTestConfiguration();

        var stations = new Dictionary<int, StationMonitor>
        {
            { 1, new StationMonitor { MachineId = 10000, PartNumber = "L823566" } },
            { 2, new StationMonitor { MachineId = 400, PartNumber = "L823581" } },
            { 3, new StationMonitor { MachineId = 500, PartNumber = "L422290" } },
        };

        var result = stations.FilterByModel();

        result.Count.ShouldBe(1);
        result.Values.ShouldAllBe(m => m.PartNumber == "L422290");
    }

    // TODO: Flagged for further analysis — possible behavior change or environment interaction.
    // Not confirmed as bug. Root causes may include:
    // - Changes in .NET 10 async scheduling or timing.
    // - Test relying on external state (e.g. temp files, shared mocks).
    // - Subtle concurrency or initialization differences.
    // → Review after remaining core features and infrastructure are finalized.
    /// <summary>
    /// Executes Should_Filter_Large_ControllerMonitor_Set operation.
    /// </summary>
    [Fact]
    public void Should_Filter_Large_ControllerMonitor_Set()
    {
        var config = new ApplicationConfiguration
        {
            MachineProductCompatibility = Enumerable.Range(1, 50)
                .Select(i => new MachineProductMap { MachineId = 10000 + i, PartNumber = $"P{i:D3}", CustomerId = 2 })
                .ToList()
        };

        var monitors = Enumerable.Range(1, 100)
            .ToDictionary(i => i, i => new ControllerMonitor
            {
                MachineId = 10000 + (i % 50),
                PartNumber = $"P{i % 50:D3}"
            });

        var result = monitors.FilterByModel();

        result.ShouldNotBeEmpty();
        result.Values.All(x => x.PartNumber.StartsWith("P")).ShouldBeTrue();
    }
}
