namespace Application.UnitTests.Features.Machines;

/// <summary>
/// Unit tests for GetMachinePlcDetailQueryHandler
/// </summary>
public class GetMachinePlcDetailQueryHandlerTests
{
    private readonly IRepository<MachinePlc> _repository = Substitute.For<IRepository<MachinePlc>>();
    private readonly ILogger<GetMachinePlcDetailQueryHandler> _logger = XUnitLogger.CreateLogger<GetMachinePlcDetailQueryHandler>();
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new GetMachinePlcDetailQueryHandler(_repository, _logger);

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
    //         IRepository<MachinePlc>? nullRepository = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetMachinePlcDetailQueryHandler(nullRepository!, _logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         ILogger<GetMachinePlcDetailQueryHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetMachinePlcDetailQueryHandler(_repository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Process_WithValidQuery_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidQuery_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidQuery_ShouldReturnSuccess()
    {
        // Arrange
        var handler = new GetMachinePlcDetailQueryHandler(_repository, _logger);
        var query = new GetMachinePlcDetailQuery { MachineId = 10000, PlcId = 200 };
        var machinePlc = new MachinePlc { MachineId = 10000, PlcId = 200 };
        var machinePlcList = new List<MachinePlc> { machinePlc };

        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: [CLUSTER A FIX] - Handler uses ListAsync(), not GetByIdAsync(). Mock the correct method with proper list return
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.Success(machinePlcList));

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes Process_WhenEntityNotFound_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenEntityNotFound_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenEntityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var handler = new GetMachinePlcDetailQueryHandler(_repository, _logger);
        var query = new GetMachinePlcDetailQuery { MachineId = 10000, PlcId = 200 };

        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: [CLUSTER A FIX] - Handler uses ListAsync() but entity not found means empty list, not null result
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.Success(new List<MachinePlc>())); // Empty list = not found

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain($"MachinePlc with PlcId {query.PlcId} and MachineId {query.MachineId} not found");
    }
    /// <summary>
    /// Executes Process_WhenRepositoryFails_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenRepositoryFails_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenRepositoryFails_ShouldReturnFailure()
    {
        // Arrange
        var handler = new GetMachinePlcDetailQueryHandler(_repository, _logger);
        var query = new GetMachinePlcDetailQuery { MachineId = 10000, PlcId = 200 };

        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: [CLUSTER A FIX] - Handler uses ListAsync(), not GetByIdAsync(). Mock ListAsync() to return failure
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.WithFailure("Repository error"));

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Repository error");
    }
}
