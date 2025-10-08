namespace Integration.Tests.BarCodes;

public class BarCodeTests_QA62 : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services;
    private readonly ITestOutputHelper _output;

    public BarCodeTests_QA62(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
    {
        _services = fixture.Services;
        _output = output;
    }

    public static IEnumerable<object[]> Cases => Integration.Tests.Data.BarCodeCases_QA62.Cases;

    public static bool ShouldSkipQA62 => TestDbGuards.ShouldSkipQA62;

    [Theory(Skip = "Missing QA62 DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipQA62))]
    [MemberData(nameof(Cases))]
    [Trait("Db", "QA62")]
    public async Task Should_Find_Label_On_QA62(string barCode, int machineId, int expectedBarCodeId)
    {
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext62;

        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(BarCodeTests_QA62));
        var barCodeRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<BarCode>>(dbKey);
        var barCodeService = scope.ServiceProvider.GetRequiredKeyedService<IBarCodeResult>(dbKey);
        var partNumber = ExtractPartNumber(barCode);
        var request = new BarCodeDetailsRequest(machineId, barCode, partNumber);
        var result = await barCodeService.GetBarCodeDetails(request, TestContext.Current.CancellationToken);

        //var sut = new GetBarCodeDetailGatewayQueryHandler(barCodeResult);
        //var command = new ReadBarCodeQuery();
        //command.WithData(TaskGatewayRequest.CreateWithLabel(machineId, barCode));

        //var result = await sut.ProcessAsync(command, TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        //result.Value.ShouldBeOfType<TaskGatewayResponse>();
        result.Label.ShouldBe(barCode);

        result.BarCodeId.ShouldBeGreaterThan(0, $"Label should exist in {dbKey} dataset");
        result.BarCodeId.ShouldBe(expectedBarCodeId, $"Label should exist in {dbKey} dataset");
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
