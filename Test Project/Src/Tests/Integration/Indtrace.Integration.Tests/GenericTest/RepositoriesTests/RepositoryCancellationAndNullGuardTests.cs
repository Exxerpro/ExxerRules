using IndTrace.Application.Repository;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Models;
using Integration.Tests.Infrastructure;
using Integration.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Integration.Tests.GenericTest.RepositoriesTests;

public class RepositoryCancellationAndNullGuardTests : IClassFixture<TestHostFixture>
{
    private readonly TestHostFixture _fixture;
    private const string DbKey = DbProfiles.IndTraceDbContext45;

    public RepositoryCancellationAndNullGuardTests(TestHostFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Cancelled_token_ReturnsFailure_ListAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(DbKey);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var result = await repo.ListAsync(cts.Token);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Cancelled_token_ReturnsFailure_FirstOrDefaultAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(DbKey);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var result = await repo.FirstOrDefaultAsync(cts.Token);

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Cancelled_token_ReturnsFailure_Count_List_Add_Update_Delete()
    {
        using var scope = _fixture.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(DbKey);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Count with spec
        var spec = new Specification<Machine>(m => true);
        var count = await repo.CountAsync(spec, cts.Token);
        count.IsFailure.ShouldBeTrue();
        count.Errors.ShouldNotBeEmpty();

        // List (no spec)
        var list = await repo.ListAsync(cts.Token);
        list.IsFailure.ShouldBeTrue();
        list.Errors.ShouldNotBeEmpty();

        // Add/Update/Delete should early-cancel before DB
        var entity = new Machine();
        var add = await repo.AddAsync(entity, cts.Token);
        add.IsFailure.ShouldBeTrue();
        add.Errors.ShouldNotBeEmpty();

        var update = await repo.UpdateAsync(entity, cts.Token);
        update.IsFailure.ShouldBeTrue();
        update.Errors.ShouldNotBeEmpty();

        var del = await repo.DeleteAsync(entity, cts.Token);
        del.IsFailure.ShouldBeTrue();
        del.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Null_entity_ReturnsFailure_Add_Update_Delete_Detach()
    {
        using var scope = _fixture.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(DbKey);
        using var cts = new CancellationTokenSource();

        var add = await repo.AddAsync((Machine)null!, cts.Token);
        add.IsFailure.ShouldBeTrue();
        add.Errors.ShouldNotBeEmpty();

        var update = await repo.UpdateAsync((Machine)null!, cts.Token);
        update.IsFailure.ShouldBeTrue();
        update.Errors.ShouldNotBeEmpty();

        var del = await repo.DeleteAsync((Machine)null!, cts.Token);
        del.IsFailure.ShouldBeTrue();
        del.Errors.ShouldNotBeEmpty();

        var detach = await repo.DetachAsync((Machine)null!, cts.Token);
        detach.IsFailure.ShouldBeTrue();
        detach.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Null_spec_ReturnsFailure_List_First_Count()
    {
        using var scope = _fixture.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(DbKey);
        using var cts = new CancellationTokenSource();

        var list = await repo.ListAsync((ISpecification<Machine>)null!, cts.Token);
        list.IsFailure.ShouldBeTrue();
        list.Errors.ShouldNotBeEmpty();

        var first = await repo.FirstOrDefaultAsync((ISpecification<Machine>)null!, cts.Token);
        first.IsFailure.ShouldBeTrue();
        first.Errors.ShouldNotBeEmpty();

        var count = await repo.CountAsync((ISpecification<Machine>)null!, cts.Token);
        count.IsFailure.ShouldBeTrue();
        count.Errors.ShouldNotBeEmpty();
    }
}
