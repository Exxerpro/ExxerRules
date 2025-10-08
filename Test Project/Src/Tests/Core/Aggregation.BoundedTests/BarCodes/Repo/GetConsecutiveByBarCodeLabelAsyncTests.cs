namespace IndTrace.Aggregation.BoundedTests.BarCodes.Repo;
/// <summary>
/// Represents the GetConsecutiveByBarCodeLabelAsyncTests.
/// </summary>

public class GetConsecutiveByBarCodeLabelAsyncTests : DependenciesFactory
{
    /// <summary>
    /// Executes GetBarCodesListTest operation.
    /// </summary>
    /// <returns>The result of GetBarCodesListTest.</returns>
    [Fact]
    public async Task GetBarCodesListTest()
    {
        await Initialization;

        await Task.CompletedTask;
    }

    public GetConsecutiveByBarCodeLabelAsyncTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    /// <summary>
    /// Executes GetConsecutiveByBarCodeLabelAsync_MustRolBackToZeroAfter9999 operation.
    /// </summary>
    /// <returns>The result of GetConsecutiveByBarCodeLabelAsync_MustRolBackToZeroAfter9999.</returns>

    [Fact]
    public async Task GetConsecutiveByBarCodeLabelAsync_MustRolBackToZeroAfter9999()
    {
        await Initialization;

        // Arrange

        var repository = DpBarCodeRepository;
        var partNumber = "12345";
        var masterLabels = new List<string> { "L1AL90164629232370001" };

        // Set deterministic time for test
        DpDateTimeMachine.SetDateTimeNow(new DateTimeOffset(2020, 06, 06, 06, 06, 06, 6, TimeSpan.Zero));

        // Add test data with high BarCodeId to trigger rollback (using unique ID to avoid conflicts)
        var barcode = new BarCode
        {
            BarCodeId = 19999,
            Label = "L1AL90164629232370001",
            CreatedOn = DpDateTimeMachine.Now,
            ModifiedOn = DpDateTimeMachine.Now
        };

        await repository.AddAsync(barcode, TestContext.Current.CancellationToken);
        await repository.CommitAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetConsecutiveByBarCodeLabelAsync(partNumber, masterLabels, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Executes GetConsecutiveByBarCodeLabelAsync_NoMatchingLabels_ReturnsFailure operation.
    /// </summary>
    /// <returns>The result of GetConsecutiveByBarCodeLabelAsync_NoMatchingLabels_ReturnsFailure.</returns>

    [Fact]
    public async Task GetConsecutiveByBarCodeLabelAsync_NoMatchingLabels_ReturnsFailure()
    {
        await Initialization;

        // Arrange

        var repository = DpBarCodeRepository;
        var partNumber = "NonExistentPart";
        var masterLabels = new List<string> { "NonExistentLabel" };

        // Set deterministic time for test
        DpDateTimeMachine.SetDateTimeNow(new DateTimeOffset(2020, 06, 06, 06, 06, 06, 6, TimeSpan.Zero));

        // Act
        var result = await repository.GetConsecutiveByBarCodeLabelAsync(partNumber, masterLabels, TestContext.Current.CancellationToken);

        // Assert - when no matching labels found, should return 1 (first consecutive number)
        result.IsSuccess.ShouldBeTrue("When no BarCodes exist for label, should return success with consecutive = 1");
        result.Value.ShouldBe(0, "When no BarCodes exist for label, should return 1 as the first consecutive number");
    }

    /// <summary>
    /// Executes GetConsecutiveByBarCodeLabelAsync_MatchingLabelFound_ReturnsCorrectConsecutive operation.
    /// </summary>
    /// <returns>The result of GetConsecutiveByBarCodeLabelAsync_MatchingLabelFound_ReturnsCorrectConsecutive.</returns>

    [Fact]
    public async Task GetConsecutiveByBarCodeLabelAsync_MatchingLabelFound_ReturnsCorrectConsecutive()
    {
        await Initialization;

        // Arrange

        var repository = DpBarCodeRepository;
        var partNumber = "L90164629";
        var masterLabels = new List<string> { "L1AL90164629232370001" };

        // Set deterministic time for test
        DpDateTimeMachine.SetDateTimeNow(new DateTimeOffset(2020, 06, 06, 06, 06, 06, 6, TimeSpan.Zero));

        // Add test data - create barcodes with unique IDs that won't conflict with existing test data
        var barCodes = new List<BarCode>
        {
            new BarCode { BarCodeId = 9996, Label = "L1AL90164629232379996", CreatedOn = DpDateTimeMachine.Now, ModifiedOn = DpDateTimeMachine.Now },
            new BarCode { BarCodeId = 9997, Label = "L1AL90164629232379997", CreatedOn = DpDateTimeMachine.Now, ModifiedOn = DpDateTimeMachine.Now },
            new BarCode { BarCodeId = 9998, Label = "L1AL90164629232379998", CreatedOn = DpDateTimeMachine.Now, ModifiedOn = DpDateTimeMachine.Now }, // This should be the highest
            new BarCode { BarCodeId = 9999, Label = "L1AL90164629232379999", CreatedOn = DpDateTimeMachine.Now, ModifiedOn = DpDateTimeMachine.Now }
        };

        foreach (var barcode in barCodes)
        {
            await repository.AddAsync(barcode, TestContext.Current.CancellationToken);
        }
        await repository.CommitAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetConsecutiveByBarCodeLabelAsync(partNumber, masterLabels, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        result.Value.ShouldBe(0000); // Expected next consecutive after highest ID 20681
    }

    /// <summary>
    /// Executes GetConsecutiveByBarCodeLabelAsync_ConsecutiveExceedsLimit_ReturnsWrappedConsecutive operation.
    /// </summary>
    /// <returns>The result of GetConsecutiveByBarCodeLabelAsync_ConsecutiveExceedsLimit_ReturnsWrappedConsecutive.</returns>

    [Fact]
    public async Task GetConsecutiveByBarCodeLabelAsync_ConsecutiveExceedsLimit_ReturnsWrappedConsecutive()
    {
        await Initialization;

        // Arrange

        var repository = DpBarCodeRepository;
        var partNumber = "L90164629";
        var masterLabels = new List<string> { "L1AL90164629232370001" };

        // Set deterministic time for test
        DpDateTimeMachine.SetDateTimeNow(new DateTimeOffset(2020, 06, 06, 06, 06, 06, 6, TimeSpan.Zero));

        // Add test data - create barcodes with labels ending in 9999 to trigger rollover logic
        var barCodes = new List<BarCode>
        {
            new BarCode { BarCodeId = 9996, Label = "L1AL90164629232379996", CreatedOn = DpDateTimeMachine.Now, ModifiedOn = DpDateTimeMachine.Now },
            new BarCode { BarCodeId = 9997, Label = "L1AL90164629232379997", CreatedOn = DpDateTimeMachine.Now, ModifiedOn = DpDateTimeMachine.Now },
            new BarCode { BarCodeId = 9998, Label = "L1AL90164629232379998", CreatedOn = DpDateTimeMachine.Now, ModifiedOn = DpDateTimeMachine.Now },
            new BarCode { BarCodeId = 9999, Label = "L1AL90164629232379999", CreatedOn = DpDateTimeMachine.Now, ModifiedOn = DpDateTimeMachine.Now }, // This ends in 9999 - should trigger rollover
        };

        foreach (var barcode in barCodes)
        {
            var addResult = await repository.AddAsync(barcode, TestContext.Current.CancellationToken);
            if (!addResult.IsSuccess)
            {
                Logger.LogError("ERROR adding BarCode ID={BarCodeId}: {Errors}",
                    barcode.BarCodeId, string.Join(";", addResult.Errors ?? new List<string>()));
            }
        }

        var commitResult = await repository.CommitAsync(TestContext.Current.CancellationToken);
        if (!commitResult.IsSuccess)
        {
            Logger.LogError("ERROR in commit: {Errors}", string.Join(";", commitResult.Errors ?? new List<string>()));
        }

        // Act
        var result = await repository.GetConsecutiveByBarCodeLabelAsync(partNumber, masterLabels, TestContext.Current.CancellationToken);

        // Log only if there's an error - success cases don't need logging
        if (!result.IsSuccess)
        {
            Logger.LogError("GetConsecutiveByBarCodeLabelAsync failed for PartNumber={PartNumber}: {Errors}",
                partNumber, string.Join(";", result.Errors ?? new List<string>()));
        }

        // Assert - should rollover to exactly 0 when consecutive exceeds 9999 limit
        result.IsSuccess.ShouldBeTrue($"Expected success but got failure. Errors: [{string.Join(";", result.Errors ?? new List<string>())}]");
        result.Value.ShouldBe(0, $"Expected consecutive value to rollover to exactly 0 when exceeding 9999 limit, but got {result.Value}. Labels ending in 9999 should trigger rollover to 0.");
    }
}
