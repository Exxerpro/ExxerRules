using IndTrace.Application.Repository;
using IndTrace.Domain.Entities;
using IndTrace.Persistence.Interfaces;
using Integration.Tests.Adapters;
using Integration.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Integration.Tests.Infrastructure;

/// <summary>
/// Simple DI verification tests using the existing TestHostFixture.
/// </summary>
public class SimpleDiVerificationTests : IClassFixture<TestHostFixture>
{
    private readonly TestHostFixture _fixture;

    public SimpleDiVerificationTests(TestHostFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Should_Resolve_DbContextFactory_From_TestHostFixture()
    {
        // Arrange & Act
        var factory = _fixture.Services.GetRequiredKeyedService<IIndTraceDbContextFactory>(DbProfiles.IndTraceDbContext45);

        // Assert
        factory.ShouldNotBeNull();
        factory.ShouldBeAssignableTo<IIndTraceDbContextFactory>();
    }

    [Fact]
    public void Should_Resolve_Machine_Repository_From_TestHostFixture()
    {
        // Arrange & Act
        var repository = _fixture.Services.GetRequiredKeyedService<IRepository<Machine>>(DbProfiles.IndTraceDbContext45);

        // Assert
        repository.ShouldNotBeNull();
        repository.ShouldBeAssignableTo<IRepository<Machine>>();
    }

    [Fact]
    public async Task Should_Create_DbContext_From_TestHostFixture()
    {
        // Arrange
        var factory = _fixture.Services.GetRequiredKeyedService<IIndTraceDbContextFactory>(DbProfiles.IndTraceDbContext45);

        // Act
        using var context = await factory.CreateDbContextAsync(CancellationToken.None);

        // Assert
        context.ShouldNotBeNull();
    }
}
