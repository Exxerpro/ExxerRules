using IndTrace.Application.Repository;
using IndTrace.Domain.Entities;
using Integration.Tests.Infrastructure;
using Integration.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Integration.Tests.GenericTest.RepositoriesTests;

public class ReadOnlyRepositoryCancellationAndNullGuardTests : IClassFixture<TestHostFixture>
{
    private readonly TestHostFixture _fixture;
    private const string DbKey = DbProfiles.IndTraceDbContext45;

    public ReadOnlyRepositoryCancellationAndNullGuardTests(TestHostFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Cancelled_token_ReturnsFailure_List_First()
    {
        using var scope = _fixture.Services.CreateScope();
        var roRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<Machine>>(DbKey);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var list = await roRepo.ListAsync(cts.Token);
        list.IsFailure.ShouldBeTrue();
        list.Errors.ShouldNotBeEmpty();

        var first = await roRepo.FirstOrDefaultAsync(cts.Token);
        first.IsFailure.ShouldBeTrue();
        first.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Cancelled_token_ReturnsFailure_Count_WithSpec()
    {
        using var scope = _fixture.Services.CreateScope();
        var roRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<Machine>>(DbKey);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var spec = new Specification<Machine>(m => true);
        var count = await roRepo.CountAsync(spec, cts.Token);

        count.IsFailure.ShouldBeTrue();
        count.Errors.ShouldNotBeEmpty();
    }
    [Fact]
    public async Task Null_spec_ReturnsFailure_List_First_Count()
    {
        using var scope = _fixture.Services.CreateScope();
        var roRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<Machine>>(DbKey);
        using var cts = new CancellationTokenSource();

        var list = await roRepo.ListAsync((ISpecification<Machine>)null!, cts.Token);
        list.IsFailure.ShouldBeTrue();
        list.Errors.ShouldNotBeEmpty();

        var first = await roRepo.FirstOrDefaultAsync((ISpecification<Machine>)null!, cts.Token);
        first.IsFailure.ShouldBeTrue();
        first.Errors.ShouldNotBeEmpty();

        var count = await roRepo.CountAsync((ISpecification<Machine>)null!, cts.Token);
        count.IsFailure.ShouldBeTrue();
        count.Errors.ShouldNotBeEmpty();
    }
}
