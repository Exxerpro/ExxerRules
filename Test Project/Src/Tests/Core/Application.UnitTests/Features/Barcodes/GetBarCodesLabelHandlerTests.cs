namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Unit tests for GetBarCodesLabelHandler
/// </summary>
public class GetBarCodesLabelHandlerTests
{
    private readonly IRepository<BarCode> _repository = Substitute.For<IRepository<BarCode>>();
    private readonly IMonitorRequestDispatcher _dispatcher = Substitute.For<IMonitorRequestDispatcher>();
    private readonly IDateTimeMachine _dateTimeMachine = Substitute.For<IDateTimeMachine>();
    private readonly ILogger<GetBarCodesLabelHandler> _logger = XUnitLogger.CreateLogger<GetBarCodesLabelHandler>();
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new GetBarCodesLabelHandler(_repository, _dispatcher, _dateTimeMachine, _logger);

        // Assert
        handler.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes Constructor_WithNullRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullRepository_ShouldThrowException()
    //     {
    //         // Arrange
    //         IRepository<BarCode>? nullRepository = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetBarCodesLabelHandler(nullRepository!, _dispatcher, _dateTimeMachine, _logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         ILogger<GetBarCodesLabelHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetBarCodesLabelHandler(_repository, _dispatcher, _dateTimeMachine, nullLogger!));
    //     }
    /// <summary>
    /// Executes Process_WithValidQuery_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidQuery_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidQuery_ShouldReturnSuccess()
    {
        // Arrange
        var handler = new GetBarCodesLabelHandler(_repository, _dispatcher, _dateTimeMachine, _logger);
        var query = new GetBarCodesLabelQuery("TEST-LABEL");

        query.Label.ShouldBe("TEST-LABEL");

        var barCodes = new List<BarCode>
        {
            new BarCode { BarCodeId = 1, Label = "TEST-LABEL" },
            new BarCode { BarCodeId = 2, Label = "OTHER-LABEL" }
        };

        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<BarCode>>.Success(barCodes));

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodes.Count.ShouldBe(1);
        result.Value.BarCodes.First().Label.ShouldBe(query.Label);
    }
    /// <summary>
    /// Executes Process_WhenRepositoryFails_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenRepositoryFails_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenRepositoryFails_ShouldReturnFailure()
    {
        // Arrange
        var handler = new GetBarCodesLabelHandler(_repository, _dispatcher, _dateTimeMachine, _logger);
        var query = new GetBarCodesLabelQuery("TEST-LABEL");

        query.Label.ShouldBe("TEST-LABEL");

        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<BarCode>>.WithFailure("Repository error"));

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Repository error");
    }
    /// <summary>
    /// Executes Process_WithEmptyResults_ShouldReturnEmptyList operation.
    /// </summary>
    /// <returns>The result of Process_WithEmptyResults_ShouldReturnEmptyList.</returns>

    [Fact]
    public async Task Process_WithEmptyResults_ShouldReturnEmptyList()
    {
        // Arrange
        var handler = new GetBarCodesLabelHandler(_repository, _dispatcher, _dateTimeMachine, _logger);
        var query = new GetBarCodesLabelQuery("NONEXISTENT-LABEL");

        query.Label.ShouldBe("NONEXISTENT-LABEL");

        var barCodes = new List<BarCode>();

        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<BarCode>>.Success(barCodes));

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodes.Count.ShouldBe(0);
    }
}
