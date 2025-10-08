using Microsoft.EntityFrameworkCore;
using IndTrace.Persistence.DBContext;

namespace Integration.Tests.BarCodes;

public class BarCodeTests_QA62_Debug : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services;
    private readonly ITestOutputHelper _output;

    public BarCodeTests_QA62_Debug(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
    {
        _services = fixture.Services;
        _output = output;
    }

    public static bool ShouldSkipQA62 => TestDbGuards.ShouldSkipQA62;

    [Fact(Skip = "Missing QA62 DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipQA62))]
    [Trait("Db", "QA62")]
    public async Task Debug_Check_QA62_Database_Connection_And_Data()
    {
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext62;

        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(BarCodeTests_QA62_Debug));

        var factory = scope.ServiceProvider.GetRequiredKeyedService<IIndTraceDbContextFactory>(dbKey);
        await using var context = (IndTraceDbContext)factory.CreateDbContext();

        // Test database connectivity
        var canConnect = await context.Database.CanConnectAsync(TestContext.Current.CancellationToken);
        _output.WriteLine($"Can connect to QA62: {canConnect}");
        canConnect.ShouldBeTrue("Database should be accessible");

        // Count total barcodes
        var totalBarcodes = await context.BarCodes.CountAsync(TestContext.Current.CancellationToken);
        _output.WriteLine($"Total barcodes in QA62: {totalBarcodes}");

        // Test specific barcode
        var testLabel = "QA62422190250596006";
        var barcode = await context.BarCodes
            .FirstOrDefaultAsync(b => b.Label == testLabel, TestContext.Current.CancellationToken);

        if (barcode != null)
        {
            _output.WriteLine($"Found barcode: {barcode.Label} with ID: {barcode.BarCodeId}");
        }
        else
        {
            _output.WriteLine($"Barcode '{testLabel}' NOT FOUND in database");

            // Try case-insensitive search
            var barcodeCI = await context.BarCodes
                .FirstOrDefaultAsync(b => b.Label.ToLower() == testLabel.ToLower(), TestContext.Current.CancellationToken);

            if (barcodeCI != null)
            {
                _output.WriteLine($"Found with case-insensitive search: {barcodeCI.Label} (actual case)");
            }

            // List first 10 barcodes to see what's actually in the database
            var sampleBarcodes = await context.BarCodes
                .Take(10)
                .Select(b => new { b.BarCodeId, b.Label })
                .ToListAsync(TestContext.Current.CancellationToken);

            _output.WriteLine("Sample barcodes from database:");
            foreach (var sample in sampleBarcodes)
            {
                _output.WriteLine($"  ID: {sample.BarCodeId}, Label: {sample.Label}");
            }
        }
    }
}
