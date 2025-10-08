using IndTrace.Application.Machines.Commands.Enable;

namespace Integration.Tests.Machines.Tests.Commands;
/// <summary>
/// Represents the ToggleEnableMachineCommandValidatorTests.
/// </summary>

public class ToggleEnableMachineCommandValidatorTests
{
    private readonly ToggleEnableMachineCommandValidator _validator = new();

    private ToggleEnableMachineCommand CreateValidCommand()
    {
        return new ToggleEnableMachineCommand
        {
            MachineId = 100,
            Enable = true
        };
    }
    /// <summary>
    /// Executes Should_Have_Error_When_MachineId_Is_Zero operation.
    /// </summary>

    [Fact]
    public void Should_Have_Error_When_MachineId_Is_Zero()
    {
        var command = CreateValidCommand();
        command.MachineId = 0;
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MachineId);
    }
    /// <summary>
    /// Executes Should_Have_Error_When_MachineId_Is_Negative operation.
    /// </summary>

    [Fact]
    public void Should_Have_Error_When_MachineId_Is_Negative()
    {
        var command = CreateValidCommand();
        command.MachineId = -1;
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.MachineId);
    }
    /// <summary>
    /// Executes Should_Not_Have_Error_When_MachineId_Is_Positive operation.
    /// </summary>

    [Fact]
    public void Should_Not_Have_Error_When_MachineId_Is_Positive()
    {
        var command = CreateValidCommand();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
