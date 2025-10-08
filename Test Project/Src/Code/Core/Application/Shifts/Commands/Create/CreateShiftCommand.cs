// <copyright file="CreateShiftCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Commands.Create;

/// <summary>
/// Represents the CreateShiftCommand.
/// </summary>
public class CreateShiftCommand : IMonitorRequest<ShiftCreatedEvent>
{
    public const int FirstShiftStart = 7;

    public const int SecondShiftStart = 15;

    public const int ThirdShiftStart = 23;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateShiftCommand"/> class.
    ///
    /// 3 turnos:
    /// 1.- 7am a 3pm
    /// 2.- 3pm a 11pm
    /// 3.- 11pm a 7am.
    ///  </summary>
    /// <param name="dateTimeMachine"></param>
    /// <param name="dateTimeMachine">The dateTimeMachine.</param>
    /// <param name="shiftDetector"></param>
    /// <param name="machineId"></param>
    /// <param name="machineId">The machineId.</param>
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public CreateShiftCommand(IDateTimeMachine dateTimeMachine, IShiftDetectionRuleExecutor shiftDetector, int machineId = 100)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(machineId);
        var dateTime = dateTimeMachine.Now.ToLocalTime();

        // [Fix]
        // CLAUDE
        // Date: 23/08/2025
        // Reason: Initialize all properties to ensure validation passes
        (this.ShiftType, this.StartBy, this.Duration) = shiftDetector.DetectShift(dateTimeMachine.Now);

        this.MachineId = machineId;

        // Initialize production metrics to default values
        this.ShiftProduction = 0;
        this.CyclesOk = 0;
    }

    private CreateShiftCommand()
    {
        this.ShiftType = ShiftType.None;
    }

    /// <summary>
    /// Gets or sets the CyclesOk.
    /// </summary>
    public int CyclesOk { get; set; }

    /// <summary>
    /// Gets or sets the Duration.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the MaxDuration.
    /// </summary>
    public TimeSpan MaxDuration { get; set; } = new(16, 0, 0);

    /// <summary>
    /// Gets or sets the MinDuration.
    /// </summary>
    public TimeSpan MinDuration { get; set; } = new(2, 0, 0);

    /// <summary>
    /// Gets or sets the NormalDuration.
    /// </summary>
    public TimeSpan NormalDuration { get; set; } = new(8, 0, 0);

    /// <summary>
    /// Gets or sets the ShiftProduction.
    /// </summary>
    public int ShiftProduction { get; set; }

    /// <summary>
    /// Gets or sets the ShiftType.
    /// </summary>
    public ShiftType ShiftType { get; set; }

    /// <summary>
    /// Gets or sets the StartBy.
    /// </summary>
    public DateTime StartBy { get; set; }

    /// <summary>
    /// Creates a CreateShiftCommand from a ShiftCreatedEvent.
    /// </summary>
    /// <param name="shiftCreatedEvent"></param>
    /// <returns></returns>
    public static CreateShiftCommand FromShiftCreatedEvent(ShiftCreatedEvent shiftCreatedEvent)
    {
        var shiftCommand = new CreateShiftCommand();

        shiftCommand.StartBy = shiftCreatedEvent.StartBy;
        shiftCommand.Duration = shiftCreatedEvent.Duration;
        shiftCommand.CyclesOk = shiftCreatedEvent.CyclesOk;

        return shiftCommand;
    }

    /// <summary>
    /// Executes Equals operation.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>The result of Equals.</returns>
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            null => false,
            Shift shift => this.StartBy == shift.StartBy && this.Duration == shift.Duration,
            CreateShiftCommand shiftCmd => this.StartBy == shiftCmd.StartBy && this.Duration == shiftCmd.Duration,
            _ => false,
        };
    }

    /// <summary>
    /// Executes GetHashCode operation.
    /// </summary>
    /// <returns>The result of GetHashCode.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.StartBy, this.Duration);
    }
}
