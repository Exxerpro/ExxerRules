// <copyright file="CreateCyclesCommandHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.Services;
using IndTrace.Application.Cycles.Policies;
using IndTrace.Application.Cycles.Services;
using IndTrace.Application.Cycles.Validation;
using IndTrace.Application.Gateway.Auditing;

namespace Application.UnitTests.Features.Cycles;

/// <summary>
/// Unit tests for the <see cref="CreateCyclesCommandHandler"/> class.
/// </summary>
public class CreateCyclesCommandHandlerTests
{
    private readonly IDateTimeMachine _dateTimeMachineSub = null!;
    private readonly IBarCodeResult _barCodeResultSub = null!;
    private readonly IStationValidator _stationValidatorSub = null!;
    private readonly ICycleLimitPolicy _cycleLimitPolicySub = null!;
    private readonly ICycleCreator _cycleCreatorSub = null!;
    private readonly IBarCodeUpdater _barCodeUpdaterSub = null!;
    private readonly IGatewayAuditFactory _gatewayAuditFactorySub = null!;
    private readonly CreateCyclesCommandHandler _handler = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCyclesCommandHandlerTests"/> class.
    /// </summary>
    public CreateCyclesCommandHandlerTests()
    {
        // Mock all dependencies - updated for new 8-parameter constructor
        _dateTimeMachineSub = Substitute.For<IDateTimeMachine>();
        _barCodeResultSub = Substitute.For<IBarCodeResult>();
        _stationValidatorSub = Substitute.For<IStationValidator>();
        _cycleLimitPolicySub = Substitute.For<ICycleLimitPolicy>();
        _cycleCreatorSub = Substitute.For<ICycleCreator>();
        _barCodeUpdaterSub = Substitute.For<IBarCodeUpdater>();
        _gatewayAuditFactorySub = Substitute.For<IGatewayAuditFactory>();

        // Instantiate handler with mocked dependencies
        var logger = XUnitLogger.CreateLogger<CreateCyclesCommandHandler>();
        _handler = new CreateCyclesCommandHandler(
            logger,
            _dateTimeMachineSub,
            _barCodeResultSub,
            _stationValidatorSub,
            _cycleLimitPolicySub,
            _cycleCreatorSub,
            _barCodeUpdaterSub,
            _gatewayAuditFactorySub);
    }

    /// <summary>
    /// Test that the handler can be constructed successfully.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var logger = XUnitLogger.CreateLogger<CreateCyclesCommandHandler>();
        var handler = new CreateCyclesCommandHandler(
            logger,
            _dateTimeMachineSub,
            _barCodeResultSub,
            _stationValidatorSub,
            _cycleLimitPolicySub,
            _cycleCreatorSub,
            _barCodeUpdaterSub,
            _gatewayAuditFactorySub);

        // Assert
        handler.ShouldNotBeNull();
    }

    /// <summary>
    /// Test that the handler fails when machine type cannot start cycles.
    /// </summary>
    [Theory]
    [InlineData("None")] // MachineType.None
    [InlineData("DashBoard")] // MachineType.DashBoard
    public async Task Handle_ShouldFail_WhenMachineTypeCannotStartCycles(string machineTypeName)
    {
        // Using parameters: machineTypeName
        _ = machineTypeName; // xUnit1026 fix
        // Using parameters: machineTypeName
        _ = machineTypeName; // xUnit1026 fix
        // Using parameters: machineTypeName
        _ = machineTypeName; // xUnit1026 fix
        // Using parameters: machineTypeName
        _ = machineTypeName; // xUnit1026 fix
        // Using parameters: machineTypeName
        _ = machineTypeName; // xUnit1026 fix
        var machineType = EnumModel.FromName<MachineType>(machineTypeName);

        // Arrange
        var command = new CreateCyclesCommand { Command = new TaskGatewayRequest() };

        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [CONSTRUCTOR UPDATE] - Updated to mock new station validator service for refactored handler
        var barCodeResultMock = Substitute.For<IBarCodeResult>();
        barCodeResultMock.MachineType.Returns(machineType);
        barCodeResultMock.Cycles.Returns(new List<IndTrace.Domain.Entities.Cycle>());
        barCodeResultMock.Recipe.Returns(new Recipe { MaxCyclesOk = 10, MaxCyclesNOk = 3 });

        _barCodeResultSub.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(barCodeResultMock));

        // Mock station validator to reject non-process stations
        _stationValidatorSub.ValidateCanStartCycles(Arg.Any<IBarCodeResult>())
            .Returns(Result.WithFailure("Just process station can invoke Update cycles."));

        var logger = XUnitLogger.CreateLogger<CreateCyclesCommandHandler>();
        logger.LogInformation(machineType.Name);
        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Just process station can invoke Update cycles.");
    }

    /// <summary>
    /// Test that the handler calls the correct repository methods when processing a valid command.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallRepositoryMethods_WhenProcessingValidCommand()
    {
        // Arrange
        var command = new CreateCyclesCommand
        {
            Command = new TaskGatewayRequest
            {
                BarCode = "TEST123",
                MachineId = 100,
                TimeStamp = DateTime.UtcNow,
            },
        };

        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [CONSTRUCTOR UPDATE] - Updated to mock all new services for refactored handler architecture
        var barCodeResultMock = Substitute.For<IBarCodeResult>();
        barCodeResultMock.MachineId.Returns(1);
        barCodeResultMock.NextMachineId.Returns(2);
        barCodeResultMock.CycleStatus.Returns(CycleStatus.NotStarted);
        barCodeResultMock.FlowStatus.Returns(FlowStatus.Created);
        barCodeResultMock.PartStatus.Returns(PartStatus.Ok);
        barCodeResultMock.MachineType.Returns(MachineType.Process);
        barCodeResultMock.WorkFlowType.Returns(WorkFlowType.Initial);
        barCodeResultMock.BarCodeId.Returns(1);
        barCodeResultMock.CycleId.Returns(0);
        barCodeResultMock.Label.Returns("TEST123");
        barCodeResultMock.PartNumber.Returns("PartNumberExample");
        barCodeResultMock.CyclesOk.Returns(0);
        barCodeResultMock.ShiftId.Returns(1);
        barCodeResultMock.ResultValidation.Returns(ResultValidation.Valid);

        // Set up collections to prevent null reference exceptions in validation methods
        barCodeResultMock.Cycles.Returns(new List<IndTrace.Domain.Entities.Cycle>());
        barCodeResultMock.Recipe.Returns(new Recipe { MaxCyclesOk = 10, MaxCyclesNOk = 3 });

        _barCodeResultSub.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(barCodeResultMock));

        // Mock all new services for successful flow
        _stationValidatorSub.ValidateCanStartCycles(Arg.Any<IBarCodeResult>())
            .Returns(Result.Success());

        _cycleLimitPolicySub.EvaluateCycleLimits(Arg.Any<IBarCodeResult>(), Arg.Any<CreateCyclesCommand>())
            .Returns(Result<CycleLimitDecision>.Success(new CycleLimitDecision(true, "Allowed", ResultValidation.Valid)));

        var createdCycle = new IndTrace.Domain.Entities.Cycle { CycleId = 123 };
        _cycleCreatorSub.CreateAsync(Arg.Any<CycleCreateRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<IndTrace.Domain.Entities.Cycle>.Success(createdCycle)));

        _barCodeUpdaterSub.UpdateAsync(Arg.Any<BarCodeUpdateRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        _gatewayAuditFactorySub.CreateAuditEntryAsync(Arg.Any<GatewayAuditRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<TaskGatewayRequest>.Success(new TaskGatewayRequest())));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await _cycleCreatorSub.Received(1).CreateAsync(Arg.Any<CycleCreateRequest>(), Arg.Any<CancellationToken>());
        await _barCodeUpdaterSub.Received(1).UpdateAsync(Arg.Any<BarCodeUpdateRequest>(), Arg.Any<CancellationToken>());
        await _gatewayAuditFactorySub.Received(1).CreateAuditEntryAsync(Arg.Any<GatewayAuditRequest>(), Arg.Any<CancellationToken>());
    }
}
