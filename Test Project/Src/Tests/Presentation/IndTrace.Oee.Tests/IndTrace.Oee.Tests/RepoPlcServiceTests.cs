using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.ModelsComs;
using IndTrace.OEE.Infrastructure.Services;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndTrace.Oee.Tests;
/// <summary>
/// Represents the RepoPlcServiceTests.
/// </summary>

public class RepoPlcServiceTests
{
    private readonly ITagsRepository _tagsRepo;
    private readonly ILogger<RepoPlcService> _logger;
    private readonly RepoPlcService _sut;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="output">The output.</param>

    public RepoPlcServiceTests(ITestOutputHelper output)
    {
        _tagsRepo = Substitute.For<ITagsRepository>();
        _logger = XUnitLogger.CreateLogger<RepoPlcService>(output);
        _sut = new RepoPlcService(_tagsRepo, _logger);
    }
    /// <summary>
    /// Executes LoadPlcsAsync_Should_Return_Success_When_PLCs_Found operation.
    /// </summary>
    /// <returns>The result of LoadPlcsAsync_Should_Return_Success_When_PLCs_Found.</returns>

    [Fact]
    public async Task LoadPlcsAsync_Should_Return_Success_When_PLCs_Found()
    {
        var plcs = new List<PlcData> { new PlcData { PlcId = 1, Name = "PLC1", IpAddress = "192.168.0.1" } };
        _tagsRepo.GetPlcsAsync(Arg.Any<CancellationToken>()).Returns(plcs);

        var result = await _sut.LoadPlcsDataAsync(TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
        if (!(result.IsSuccess && result.Value is { } plcsValue))
        {
            throw new Xunit.Sdk.XunitException("Expected success result with non-null Value");
        }
        plcsValue.ShouldContainKey(1);
        plcsValue.ShouldBeAssignableTo<IReadOnlyDictionary<int, PlcData>>();
    }
    /// <summary>
    /// Executes LoadPlcsAsync_Should_Return_Failure_When_Empty operation.
    /// </summary>
    /// <returns>The result of LoadPlcsAsync_Should_Return_Failure_When_Empty.</returns>

    [Fact]
    public async Task LoadPlcsAsync_Should_Return_Failure_When_Empty()
    {
        _tagsRepo.GetPlcsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Enumerable.Empty<PlcData>() as IEnumerable<PlcData>));

        var result = await _sut.LoadPlcsDataAsync(TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("No PLCs found in the database.");
    }
    /// <summary>
    /// Executes LoadTagsAsync_Should_Return_Success_When_Tags_Found operation.
    /// </summary>
    /// <returns>The result of LoadTagsAsync_Should_Return_Success_When_Tags_Found.</returns>

    [Fact]
    public async Task LoadTagsAsync_Should_Return_Success_When_Tags_Found()
    {
        var tags = new Dictionary<int, Dictionary<string, VariableS7>>
        {
            { 1, new Dictionary<string, VariableS7> { { "Tag1", new VariableS7 { Name = "Tag1", Address = "DB1.DBX0.0" } } } }
        };

        _tagsRepo.GetTagsGroupedByMachineAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(tags));

        var result = await _sut.LoadTagsDataAsync(new[] { 1 }, 1, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
        if (!(result.IsSuccess && result.Value is { } tagsValue))
        {
            throw new Xunit.Sdk.XunitException("Expected success result with non-null Value");
        }
        tagsValue.ShouldContainKey(1);
        tagsValue[1].ShouldBeAssignableTo<IReadOnlyDictionary<string, VariableS7>>();
    }
    /// <summary>
    /// Executes LoadTagsAsync_Should_Return_Failure_When_Empty operation.
    /// </summary>
    /// <returns>The result of LoadTagsAsync_Should_Return_Failure_When_Empty.</returns>

    [Fact]
    public async Task LoadTagsAsync_Should_Return_Failure_When_Empty()
    {
        _tagsRepo.GetTagsGroupedByMachineAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Dictionary<int, Dictionary<string, VariableS7>>()));

        var result = await _sut.LoadTagsDataAsync(new[] { 1 }, 1, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("No tags found for the given PLCs.");
    }
    /// <summary>
    /// Executes LogPlcsAndTags_Should_Log_Correct_Info operation.
    /// </summary>

    [Fact]
    public void LogPlcsAndTags_Should_Log_Correct_Info()
    {
        var plcs = new Dictionary<int, PlcData> {
            { 1, new PlcData { PlcId = 1, Name = "PLC1", IpAddress = "192.168.0.1" } }
        };

        var tags = new Dictionary<int, IReadOnlyDictionary<string, VariableS7>> {
            { 1, new Dictionary<string, VariableS7> { { "Tag1", new VariableS7 { Name = "Tag1", Address = "DB1.DBX0.0" } } } }
        };

        _sut.VerifyOeeIsEnabled(plcs, tags);
    }
}
