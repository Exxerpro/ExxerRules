using IndTrace.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Xunit;
using IndTrace.Domain.Entities;

namespace Integration.Tests.DatabaseConnectivity;

/// <summary>
/// Minimal test to verify database connectivity and basic operations.
/// </summary>
public class DatabaseConnectivityTest
{
    private const string QA46ConnectionString = "Server=DESKTOP-FB2ES22\\SQL2022;Database=IndTraceDataQA46;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

    [Fact]
    public async Task Should_Connect_To_QA46_Database_And_Retrieve_First_BarCode()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<IndTraceDbContext>()
            .UseSqlServer(QA46ConnectionString, actions =>
            {
                actions.MigrationsAssembly(typeof(IndTraceDbContext).Assembly.FullName)
                       .EnableRetryOnFailure(maxRetryCount: 2, maxRetryDelay: TimeSpan.FromSeconds(1), errorNumbersToAdd: []);
            })
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        // Act & Assert
        await using var context = new IndTraceDbContext(options);

        // Test 1: Can we connect?
        var canConnect = await context.Database.CanConnectAsync(TestContext.Current.CancellationToken);
        Assert.True(canConnect, "Should be able to connect to QA46 database");

        // Test 2: Is connection active?
        var isActive = context.Database.CanConnect();
        Assert.True(isActive, "Database connection should be active");

        // Test 3: Can we retrieve data?
        var firstBarCode = await context.Set<BarCode>()
            .AsNoTracking()
            .FirstOrDefaultAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(firstBarCode);
        Assert.True(firstBarCode.BarCodeId > 0);
        Assert.False(string.IsNullOrEmpty(firstBarCode.Label));

        // Test 4: Count total BarCodes
        var totalBarCodes = await context.Set<BarCode>().CountAsync(TestContext.Current.CancellationToken);
        Assert.True(totalBarCodes > 0, "Should have BarCodes in database");
    }

    [Fact]
    public async Task Should_Test_IndTraceDbContext_IsConnectionActive_Property()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<IndTraceDbContext>()
            .UseSqlServer(QA46ConnectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        // Act & Assert
        await using var context = new IndTraceDbContext(options);

        // This is the exact check that Repository.InvalidContext uses
        var isConnectionActive = context.IsConnectionActive;

        Assert.True(isConnectionActive, "IsConnectionActive should return true for valid database connection");
    }
}
