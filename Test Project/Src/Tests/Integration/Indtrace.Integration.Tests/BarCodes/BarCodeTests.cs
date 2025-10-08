namespace Integration.Tests.BarCodes
{
    /// <summary>
    /// Represents the GetBarCodeDetailGatewayQueryTests.
    /// </summary>
    public class GetBarCodeDetailGatewayQueryTests : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly IServiceProvider _services;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fixture">The test host fixture.</param>
        /// <param name="output">The output.</param>

        public GetBarCodeDetailGatewayQueryTests(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _services = fixture.Services;
        }

        public static IEnumerable<object[]> BarCodeDetailCases => Integration.Tests.Data.BarCodeCases_Db45.Cases;
        /// <summary>
        /// Executes Should_Return_BarCodeDetail_For_Known_Label operation.
        /// </summary>
        /// <param name="barCode">The barCode.</param>
        /// <param name="machineId">The machineId.</param>
        /// <param name="barCodeId">The barCodeId.</param>
        /// <returns>The result of Should_Return_BarCodeDetail_For_Known_Label.</returns>

        public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;

        [Theory(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
        [Trait("Db", "DB45")]
        [MemberData(nameof(BarCodeDetailCases))]
        public async Task Should_Return_BarCodeDetail_For_Known_Label(string barCode, int machineId, int barCodeId)
        {
            const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45; // DB45 explicit key

            using var scope = _services.CreateScope();
            DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(GetBarCodeDetailGatewayQueryTests));
            var barCodeRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<BarCode>>(dbKey);
            var barCodeService = scope.ServiceProvider.GetRequiredKeyedService<IBarCodeResult>(dbKey);

            var partNumber = ExtractPartNumber(barCode);

            var request = new BarCodeDetailsRequest(machineId, barCode, partNumber);

            var result = await barCodeService.GetBarCodeDetails(request, TestContext.Current.CancellationToken);

            result.ShouldNotBeNull();

            result.Label.ShouldBe(barCode);
            result.BarCodeId.ShouldBe(barCodeId);
        }

        public static bool ShouldSkipQA62 => TestDbGuards.ShouldSkipQA62;

        [Fact(Skip = "Missing QA62 DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipQA62))]
        public async Task Should_Return_BarCodeDetail_For_Known_Label_On_QA62()
        {
            const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext62; // secondary DB example
            var barCode = "QA4500t349251303242";
            var machineId = 100;

            using var scope = _services.CreateScope();
            DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(GetBarCodeDetailGatewayQueryTests));
            var barCodeRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<BarCode>>(dbKey);

            var barCodeService = scope.ServiceProvider.GetRequiredKeyedService<IBarCodeResult>(dbKey);

            var partNumber = ExtractPartNumber(barCode);
            var request = new BarCodeDetailsRequest(machineId, barCode, partNumber);
            var barCodeResult = await barCodeService.GetBarCodeDetails(request, TestContext.Current.CancellationToken);

            var sut = new GetBarCodeDetailGatewayQueryHandler(barCodeResult);
            var command = new ReadBarCodeQuery();
            command.WithData(TaskGatewayRequest.CreateWithLabel(machineId, barCode));

            var result = await sut.ProcessAsync(command, TestContext.Current.CancellationToken);
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<TaskGatewayResponse>();
            result.Value.Label.ShouldBe(barCode);
        }

        private static string ExtractPartNumber(string barCode)
        {
            // Business Rule: {LINENAME}{PARTNUMBER}{YEAR(2)}{DAYOFYEAR(3)}{CONSECUTIVE(3-4)}
            // Hard Rule: Always 9 characters (2+3+4) after part number end

            // Determine line name length
            int lineNameLength;
            if (barCode.StartsWith("QA45") || barCode.StartsWith("QA46") || barCode.StartsWith("QA62"))
            {
                lineNameLength = 4;
            }
            else if (barCode.StartsWith("L687508"))
            {
                lineNameLength = 7;
            }
            else if (barCode.StartsWith("PAL1") || barCode.StartsWith("PAL2"))
            {
                lineNameLength = 4;
            }
            else if (barCode.StartsWith("L1") || barCode.StartsWith("L2"))
            {
                lineNameLength = 2;
            }
            else
            {
                // Fallback: assume 4 character line name
                lineNameLength = 4;
            }

            // Part number starts after line name
            // Hard rule: 9 characters at the end (2 year + 3 day + 4 consecutive)
            var partNumberStart = lineNameLength;
            var partNumberEnd = barCode.Length - 9;

            if (partNumberEnd <= partNumberStart)
            {
                throw new ArgumentException($"Invalid barcode format: {barCode}");
            }

            return barCode.Substring(partNumberStart, partNumberEnd - partNumberStart);
        }
    }
}
