using IndTrace.Dependencies.Simulations;
using IndTrace.VirtualNetwork.Simulation;

namespace IndTrace.Filters.Tests.SimulationTests
{
    /// <summary>
    /// Represents the SimulationTests.
    /// </summary>
    public class SimulationTests
    {
        private readonly string _settingText = """
                             {
                              "EnableSimulation": true,
                              "TimeStep": 1,
                              "SpeedSimulation": 1.0,
                              "ApplicationFlag": "Prod",
                              "RunningTime": "10:00:00",
                              "StoppedTime": "01:30:00",
                              "FaultedTime": "00:40:00",
                              "StatusFaultReason": "Overload",
                              "EventCounter": 100,
                              "StatusFaultReject": "LowPressure",
                              "StartTime": "2025-05-27T01:00:00",
                              "EndTime": "2025-05-27T01:00:00",
                              "CurrentTime": "2025-05-27T12:00:00",
                              "TotalProduction": 5000,
                              "ProductionOk": 4900,
                              "ProductionNoK": 100
                             }
                             """;

        /// <summary>
        /// Executes SimulateProgress_Should_GrowValuesAccordingToTime operation.
        /// </summary>
        /// <returns>The result of SimulateProgress_Should_GrowValuesAccordingToTime.</returns>

        [Fact]
        public async Task SimulateProgress_Should_GrowValuesAccordingToTime()
        {
            // Arrange: Load JSON

            var raw = JsonSerializer.Deserialize<SimulationSettings>(_settingText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            raw.ShouldNotBeNull();

            var settings = SimulationSettings.ConvertFrom(raw);

            var monitor = Substitute.For<IOptionsMonitor<SimulationSettings>>();
            monitor.CurrentValue.Returns(settings);

            var engine = new SimulationEngine(monitor);

            var result1 = engine.Simulate();
            await Task.Delay(1000, TestContext.Current.CancellationToken);
            var result2 = engine.Simulate();
            await Task.Delay(1000, TestContext.Current.CancellationToken);
            var result3 = engine.Simulate();

            await Task.Delay(1000, TestContext.Current.CancellationToken);
            var result4 = engine.Simulate();

            result2.TotalProduction.ShouldBeGreaterThanOrEqualTo(result1.TotalProduction);
            result2.ProductionOk.ShouldBeGreaterThanOrEqualTo(result1.ProductionOk);
            result2.ProductionNoK.ShouldBeGreaterThanOrEqualTo(result1.ProductionNoK);
            result2.EventCounter.ShouldBeGreaterThanOrEqualTo(result1.EventCounter);

            result2.RunningTime.ShouldBeGreaterThanOrEqualTo(result1.RunningTime);
            result2.StoppedTime.ShouldBeGreaterThanOrEqualTo(result1.StoppedTime);
            result2.FaultedTime.ShouldBeGreaterThanOrEqualTo(result1.FaultedTime);

            result3.ShouldBeGreaterThanOrEqualTo(result2);
            result4.ShouldBeGreaterThanOrEqualTo(result3);

            (result2 != result1).ShouldBeTrue();
            (result3 != result2).ShouldBeTrue();
            (result4 != result3).ShouldBeTrue();

            (result2 > result1).ShouldBeTrue();
            (result3 > result2).ShouldBeTrue();
            (result4 > result3).ShouldBeTrue();
        }
    }
}
