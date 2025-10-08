using IndTrace.Persistence.Interfaces;
using Integration.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Integration.Tests.Infrastructure;

/// <summary>
/// Debug tests to diagnose database connectivity issues.
/// </summary>
public class DatabaseConnectivityDebugTests : IClassFixture<TestHostFixture>
{
    private readonly TestHostFixture _fixture;

    public DatabaseConnectivityDebugTests(TestHostFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(DbProfiles.IndTraceDbContext45)]
    [InlineData(DbProfiles.IndTraceDbContext46)]
    [InlineData(DbProfiles.IndTraceDbContext62)]
    public async Task Should_Connect_To_Database(string profile)
    {
        // Arrange
        var factory = _fixture.Services.GetRequiredKeyedService<IIndTraceDbContextFactory>(profile);
        var logger = _fixture.Services.GetRequiredService<ILogger<DatabaseConnectivityDebugTests>>();

        // Act
        using var context = await factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        context.ShouldNotBeNull();

        logger.LogInformation("Testing database connection for profile: {Profile}", profile);
        logger.LogInformation("Database Provider: {Provider}", context.Database.ProviderName);

        // Test actual connection
        var canConnect = await context.Database.CanConnectAsync(TestContext.Current.CancellationToken);
        logger.LogInformation("Can Connect: {CanConnect}", canConnect);

        // Assert
        canConnect.ShouldBeTrue($"Cannot connect to database {profile}. Check if SQL Server DESKTOP-FB2ES22 is running and accessible.");
    }
}
