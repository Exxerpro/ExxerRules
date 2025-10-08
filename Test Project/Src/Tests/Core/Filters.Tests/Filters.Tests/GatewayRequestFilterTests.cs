namespace IndTrace.Filters.Tests
{
    /// <summary>
    /// Represents the GatewayRequestFilterTests.
    /// </summary>
    public class GatewayRequestFilterTests
    {
        private readonly ITestOutputHelper _output;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="output">The output.</param>

        public GatewayRequestFilterTests(ITestOutputHelper output)
        {
            _output = output;
        }
        /// <summary>
        /// Executes FilterByModel_ShouldKeepLatestWhenDuplicateNameExists operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldKeepLatestWhenDuplicateNameExists()
        {
            // Arrange
            var data = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422290", TimeStamp = ParseUtc("2025-05-03T05:18:09.2368162-06:00") } },
                { 400, new TaskGatewayRequest { Name = "WS400", PartNumber = "422290", TimeStamp = ParseUtc("2025-05-03T05:18:31.1073106-06:00") } },
                { 500, new TaskGatewayRequest { Name = "WS500", PartNumber = "422290", TimeStamp = ParseUtc("2025-05-03T05:18:37.884095-06:00") } },
                { 1100, new TaskGatewayRequest { Name = "WS100", PartNumber = "431610", TimeStamp = ParseUtc("2025-05-03T05:19:07.5308491-06:00") } },
            };

            // Act
            var result = data.FilterByModel();

            // Assert
            AssertFilteredResult(result, 3, "WS100", "WS400", "WS500");
            result.Keys.ShouldBe([1100, 400, 500], ignoreOrder: true);
        }
        /// <summary>
        /// Executes FilterByModel_ShouldKeepOnlyCanonicalPartNumberDuplicates operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldKeepOnlyCanonicalPartNumberDuplicates()
        {
            // Arrange
            var baseTime = ParseUtc("2025-05-02T19:30:00Z");

            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-60) } },
                { 400, new TaskGatewayRequest { Name = "WS400", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-48) } },
                { 500, new TaskGatewayRequest { Name = "WS500", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-46) } },
                {1100, new TaskGatewayRequest { Name = "WS100", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.5) } },
                {1200, new TaskGatewayRequest { Name = "WS200", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.3) } },
                {1300, new TaskGatewayRequest { Name = "WS300", PartNumber = "431610", TimeStamp = baseTime } }
            };
            // Act
            var result = source.FilterByModel();

            // Assert
            AssertFilteredResult(result, 3, "WS100", "WS200", "WS300");
        }
        /// <summary>
        /// Executes FilterByModel_ShouldInclude2OldModelEntries_WhenWithin45Minutes operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldInclude2OldModelEntries_WhenWithin45Minutes()
        {
            // Arrange
            var baseTime = ParseUtc("2025-05-02T19:30:00Z");

            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-46) } }, // expired
                { 400, new TaskGatewayRequest { Name = "WS400", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-40) } }, // valid
                { 500, new TaskGatewayRequest { Name = "WS500", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-42) } }, // valid
                {1100, new TaskGatewayRequest { Name = "WS100", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.5) } },
                {1200, new TaskGatewayRequest { Name = "WS200", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.3) } },
                {1300, new TaskGatewayRequest { Name = "WS300", PartNumber = "431610", TimeStamp = baseTime } }
            };

            // Act
            var result = source.FilterByModel();

            // Assert
            AssertFilteredResult(result, 5, "WS100", "WS200", "WS300", "WS400", "WS500");
        }
        /// <summary>
        /// Executes FilterByModel_ShouldInclude1OldModelEntry_WhenBarelyWithin45Minutes operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldInclude1OldModelEntry_WhenBarelyWithin45Minutes()
        {
            // Arrange
            var baseTime = ParseUtc("2025-05-02T19:30:00Z");

            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-50) } }, // expired
                { 400, new TaskGatewayRequest { Name = "WS400", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-46) } }, // expired
                { 500, new TaskGatewayRequest { Name = "WS500", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-44.5) } }, // valid
                {1100, new TaskGatewayRequest { Name = "WS100", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.5) } },
                {1200, new TaskGatewayRequest { Name = "WS200", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.3) } },
                {1300, new TaskGatewayRequest { Name = "WS300", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.1) } }
            };

            // Act
            var result = source.FilterByModel();

            // Assert
            AssertFilteredResult(result, 4, "WS100", "WS200", "WS300", "WS500");
        }
        /// <summary>
        /// Executes FilterByModel_ShouldIncludeEntryExactlyAt45Minutes operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldIncludeEntryExactlyAt45Minutes()
        {
            // Arrange
            var baseTime = ParseUtc("2025-05-02T19:30:00Z");

            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422290", TimeStamp = baseTime.AddMinutes(-46) } }, // exactly 45m
                {1100, new TaskGatewayRequest { Name = "WS100", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.5) } },
                {1200, new TaskGatewayRequest { Name = "WS200", PartNumber = "431610", TimeStamp = baseTime.AddMinutes(-0.3) } },
                {1300, new TaskGatewayRequest { Name = "WS300", PartNumber = "431610", TimeStamp = baseTime } }
            };

            // Act
            var result = source.FilterByModel();

            // Assert
            AssertFilteredResult(result, 3, "WS100", "WS200", "WS300"); // includes both WS100s
            result.ContainsKey(100).ShouldBeFalse();  // check inclusion of the 45-min old entry
        }
        /// <summary>
        /// Executes FilterByModel_ShouldReturnOriginalWhenNoDuplicates operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldReturnOriginalWhenNoDuplicates()
        {
            // Arrange
            var baseTime = DateTime.UtcNow;
            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "L823566", TimeStamp = baseTime.AddSeconds(-5) } },
                { 400, new TaskGatewayRequest { Name = "WS400", PartNumber = "L823581", TimeStamp = baseTime.AddSeconds(-4) } },
            };

            // Act
            var result = source.FilterByModel();

            // Assert
            result.ShouldBe(source, ignoreOrder: true);
        }
        /// <summary>
        /// Executes FilterByModel_ShouldReturnOriginalWhenThreeUniqueEntries operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldReturnOriginalWhenThreeUniqueEntries()
        {
            // Arrange
            var baseTime = DateTime.UtcNow;
            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-5) } },
                { 400, new TaskGatewayRequest { Name = "WS400", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-4) } },
                { 500, new TaskGatewayRequest { Name = "WS500", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-3) } },
            };

            // Act
            var result = source.FilterByModel();

            // Assert
            result.ShouldBe(source, ignoreOrder: true);
        }
        /// <summary>
        /// Executes FilterByModel_ShouldKeepLatestEntryAmongDuplicates operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldKeepLatestEntryAmongDuplicates()
        {
            // Arrange
            var baseTime = DateTime.UtcNow;
            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-5) } },
                { 200, new TaskGatewayRequest { Name = "WS400", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-4) } },
                { 300, new TaskGatewayRequest { Name = "WS500", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-3) } },
                { 1100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422330", TimeStamp = baseTime.AddSeconds(-55) } },
            };

            // Act
            var result = source.FilterByModel();

            // Assert
            AssertFilteredResult(result, 3, "WS100", "WS400", "WS500");
        }
        /// <summary>
        /// Executes FilterByModel_ShouldRemoveStaleEntriesIfExpired operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldRemoveStaleEntriesIfExpired()
        {
            // Arrange
            var baseTime = DateTime.UtcNow;
            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-50) } },
                { 200, new TaskGatewayRequest { Name = "WS400", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-40) } },
                { 300, new TaskGatewayRequest { Name = "WS500", PartNumber = "422290", TimeStamp = baseTime.AddSeconds(-35) } },
                { 1100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422330", TimeStamp = baseTime.AddSeconds(-5) } },
            };

            // Act
            var result = source.FilterByModel();

            // Assert
            AssertFilteredResult(result, 3, "WS100", "WS400", "WS500");
        }
        /// <summary>
        /// Executes FilterByModel_ShouldDeduplicateMultipleNamesAndKeepLatestByTimestamp operation.
        /// </summary>

        [Fact]
        public void FilterByModel_ShouldDeduplicateMultipleNamesAndKeepLatestByTimestamp()
        {
            // Arrange
            var baseTime = DateTime.UtcNow;
            var source = new Dictionary<int, TaskGatewayRequest>
            {
                { 100, new TaskGatewayRequest { Name = "WS100", PartNumber = "L823566", TimeStamp = baseTime.AddSeconds(-5) } },
                { 400, new TaskGatewayRequest { Name = "WS400", PartNumber = "L823581", TimeStamp = baseTime.AddSeconds(-4) } },
                { 500, new TaskGatewayRequest { Name = "WS500", PartNumber = "422290",   TimeStamp = baseTime.AddSeconds(-3) } },
                { 1100, new TaskGatewayRequest { Name = "WS100", PartNumber = "422300",   TimeStamp = baseTime.AddSeconds(5) } }, // duplicate
                { 200, new TaskGatewayRequest { Name = "WS200", PartNumber = "422310",   TimeStamp = baseTime.AddSeconds(1) } },
                { 300, new TaskGatewayRequest { Name = "WS300", PartNumber = "422330",   TimeStamp = baseTime.AddSeconds(2) } },
                { 1400, new TaskGatewayRequest { Name = "WS400", PartNumber = "431610",   TimeStamp = baseTime.AddSeconds(6) } }, // duplicate
                { 1500, new TaskGatewayRequest { Name = "WS500", PartNumber = "431850",   TimeStamp = baseTime.AddSeconds(7) } }, // duplicate
                { 600, new TaskGatewayRequest { Name = "WS600", PartNumber = "432060",   TimeStamp = baseTime.AddSeconds(3) } },
                { 700, new TaskGatewayRequest { Name = "WS700", PartNumber = "432070",  TimeStamp = baseTime.AddSeconds(4) } },
                { 800, new TaskGatewayRequest { Name = "WS800", PartNumber = "432080",  TimeStamp = baseTime.AddSeconds(8) } }
            };

            // Act
            var result = source.FilterByModel();

            // Assert
            AssertFilteredResult(result, 8, "WS100", "WS200", "WS300", "WS400", "WS500", "WS600", "WS700", "WS800");

            result.Values.First(r => r.Name == "WS100").TimeStamp.ShouldBe(baseTime.AddSeconds(5));
            result.Values.First(r => r.Name == "WS400").TimeStamp.ShouldBe(baseTime.AddSeconds(6));
            result.Values.First(r => r.Name == "WS500").TimeStamp.ShouldBe(baseTime.AddSeconds(7));
        }

        // Shared helper
        private void AssertFilteredResult(IReadOnlyDictionary<int, TaskGatewayRequest> result, int expectedCount, params string[] expectedNames)
        {
            result.ShouldNotBeNull();

            var actualNames = result.Values.Select(r => r.Name).ToList();

            _output.WriteLine("Filtered station names:");
            foreach (var name in actualNames)
            {
                _output.WriteLine($" - {name}");
            }

            result.Count.ShouldBe(expectedCount);
            actualNames.ShouldBe(expectedNames, ignoreOrder: true);
        }

        private static DateTime ParseUtc(string iso8601)
        {
            return DateTimeOffset.Parse(iso8601, CultureInfo.InvariantCulture).UtcDateTime;
        }
    }
}
