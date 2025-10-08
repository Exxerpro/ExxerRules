using IndTrace.Agregation.Dependices.DependenciesFactoryTests;

namespace IndTrace.Aggregation.BoundedTests.BarCodes.Services;
/// <summary>
/// Represents the ValidationServiceTests.
/// </summary>

public class ValidationServiceTests
{
    private readonly IBarCodeValidationService _validationService = new BarCodeValidationService();
    /// <summary>
    /// Executes Validate_WithDifferentConditions_ReturnsExpectedResult operation.
    /// </summary>
    /// <param name="flowStatus">The flowStatus.</param>
    /// <param name="machineType">The machineType.</param>
    /// <param name="cycleStatus">The cycleStatus.</param>
    /// <param name="expectedResult">The expectedResult.</param>

    [Theory]
    [MemberData(nameof(ValidationTestData.GetValidationTestData), MemberType = typeof(ValidationTestData))]
    public void Validate_WithDifferentConditions_ReturnsExpectedResult(FlowStatus flowStatus, MachineType machineType, CycleStatus cycleStatus, ResultValidation expectedResult)
    {
        var logger = XUnitLogger.CreateLogger<DependenciesFactoryComprehensiveTests>();

        // Arrange
        var partStatus = PartStatus.Ok;
        var machineId = 1;
        var nextMachineId = 100;

        // Act
        var result = _validationService.Validate(flowStatus, machineType, cycleStatus, partStatus, machineId, nextMachineId);

        //log the parameters and result
        logger.LogInformation("Validate called with FlowStatus: {FlowStatus}, MachineType: {MachineType}, CycleStatus: {CycleStatus}, PartStatus: {PartStatus}, MachineId: {MachineId}, NextMachineId: {NextMachineId}. Result: {Result}",
            flowStatus, machineType, cycleStatus, partStatus, machineId, nextMachineId, result);

        if (result != expectedResult)
        {
            logger.LogInformation("Results unexpectect for");
            logger.LogInformation("Validate called with FlowStatus: {FlowStatus}, MachineType: {MachineType}, CycleStatus: {CycleStatus}, PartStatus: {PartStatus}, MachineId: {MachineId}, NextMachineId: {NextMachineId}. Result: {Result}",
                flowStatus, machineType, cycleStatus, partStatus, machineId, nextMachineId, result);
        }

        // Assert
        result.ShouldBe(expectedResult);
    }

    /// <summary>
    /// Executes Validate_WhenInvalidPartStatus_ReturnsPartNotValid operation.
    /// </summary>

    [Fact]
    public void Validate_WhenInvalidPartStatus_ReturnsPartNotValid()
    {
        // Arrange
        var flowStatus = FlowStatus.InProcess;
        var machineType = MachineType.Process;
        var cycleStatus = CycleStatus.FinishedOk;
        var partStatus = PartStatus.Invalid;
        var machineId = 100;
        var nextMachineId = 400;

        // Act
        var result = _validationService.Validate(flowStatus, machineType, cycleStatus, partStatus, machineId, nextMachineId);

        // Assert
        result.ShouldBe(ResultValidation.PartNotValid);
        _validationService.Result.ShouldNotBeNull();
        _validationService.Result.Errors.Count().ShouldBe(1);
        _validationService.Result.Errors.First().ShouldContain("Status Invalid");
    }

    /// <summary>
    /// Executes Validate_WhenMachineIdDoesNotMatchNextMachineId_ReturnsDestinationNotValid operation.
    /// </summary>

    [Fact]
    public void Validate_WhenMachineIdDoesNotMatchNextMachineId_ReturnsDestinationNotValid()
    {
        // Arrange
        var flowStatus = FlowStatus.InProcess;
        var machineType = MachineType.Process;
        var cycleStatus = CycleStatus.Started;
        var partStatus = PartStatus.Ok;
        var machineId = 1;
        var nextMachineId = 2;

        // Act
        var result = _validationService.Validate(flowStatus, machineType, cycleStatus, partStatus, machineId, nextMachineId);

        // Assert
        result.ShouldBe(ResultValidation.DestinationNotValid);
        _validationService.Result.ShouldNotBeNull();
        _validationService.Result.Errors.Count().ShouldBe(1);
        _validationService.Result.Errors.First().ShouldBe($"Machine {machineId} is not the next machine {nextMachineId}");
    }

    /// <summary>
    /// Executes Validate_WithDefaultCase_ReturnsInvalid operation.
    /// </summary>

    [Fact]
    public void Validate_WithDefaultCase_ReturnsInvalid()
    {
        //create logger to log the case

        // Arrange
        var flowStatus = FlowStatus.InProcess;
        var machineType = MachineType.None;
        var cycleStatus = CycleStatus.NotStarted;
        var partStatus = PartStatus.Ok;
        var machineId = 1;
        var nextMachineId = 100;

        // Act
        var result = _validationService.Validate(flowStatus, machineType, cycleStatus, partStatus, machineId, nextMachineId);

        // Assert
        result.ShouldBe(ResultValidation.DestinationNotValid);
        _validationService.Result.ShouldNotBeNull();
        _validationService.Result.Errors.Count().ShouldBe(1);
        _validationService.Result.Errors.First().ShouldContain($"Machine {machineId} is not the next machine {nextMachineId}");
    }
}//End of class
