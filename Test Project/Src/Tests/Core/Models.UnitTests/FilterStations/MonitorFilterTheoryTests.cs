namespace IndTrace.Models.UnitTests.FilterStations;
/// <summary>
/// Represents the MonitorFilterTheoryTests.
/// </summary>

public class MonitorFilterTheoryTests
{
    public static IEnumerable<object[]> FilterTestData => new List<object[]>
    {
        new object[] { 2, 20, 100 },
        new object[] { 3, 15, 150 },
        new object[] { 1, 30, 50 }
    };
    /// <summary>
    /// Executes Should_Filter_ControllerMonitors_By_Customer_With_Large_Data operation.
    /// </summary>
    /// <param name="customerId">The customerId.</param>
    /// <param name="partsPerCustomer">The partsPerCustomer.</param>
    /// <param name="monitorCount">The monitorCount.</param>

    [Theory]
    [MemberData(nameof(FilterTestData))]
    public void Should_Filter_ControllerMonitors_By_Customer_With_Large_Data(int customerId, int partsPerCustomer, int monitorCount)
    {
        var config = MonitorDataFixture.GenerateConfiguration(5, partsPerCustomer);
        var monitors = MonitorDataFixture.GenerateControllerMonitors(monitorCount, customerId);

        var result = monitors.FilterByModel();

        result.ShouldNotBeEmpty();
        result.Values.All(m => m.PartNumber.StartsWith($"PN{customerId:D2}_")).ShouldBeTrue();
    }
    /// <summary>
    /// Executes Should_Filter_StationMonitors_By_Customer_With_Large_Data operation.
    /// </summary>
    /// <param name="customerId">The customerId.</param>
    /// <param name="partsPerCustomer">The partsPerCustomer.</param>
    /// <param name="monitorCount">The monitorCount.</param>

    [Theory]
    [MemberData(nameof(FilterTestData))]
    public void Should_Filter_StationMonitors_By_Customer_With_Large_Data(int customerId, int partsPerCustomer, int monitorCount)
    {
        var config = MonitorDataFixture.GenerateConfiguration(5, partsPerCustomer);
        var stations = MonitorDataFixture.GenerateStationMonitors(monitorCount, customerId);

        var result = stations.FilterByModel();

        result.ShouldNotBeEmpty();
        result.Values.All(m => m.PartNumber.StartsWith($"PN{customerId:D2}_")).ShouldBeTrue();
    }
}
