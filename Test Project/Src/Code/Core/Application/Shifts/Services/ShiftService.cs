// <copyright file="ShiftService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Services;

using IndTrace.Application.Shifts.Commands.Create;

/// <summary>
/// Provides shift management functionality including creation, retrieval, and cycle tracking for production shifts.
/// </summary>
public class ShiftService(
    IRepository<Shift> shiftRepository,
    IRepository<Cycle> cycleRepository,
    IShiftDetectionRuleExecutor shiftDetectionRuleExecutor,
    ILogger<ShiftService> logger,
    IDateTimeMachine dateTimeMachine) : IShiftService
{
    /// <summary>
    /// Creates a new shift or retrieves an existing shift for the specified machine, and updates cycle information.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the shift creation or retrieval.</returns>
    public async Task<Result<ShiftCreatedEvent>> CreateOrRetrieveShiftAndCyclesOkAsync(int machineId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new CreateShiftCommand(dateTimeMachine, shiftDetectionRuleExecutor, machineId);

            // Check if a shift already exists for the provided date
            var shiftByDate = await shiftRepository.GetShiftByDateAsync(dateTimeMachine, cancellationToken).ConfigureAwait(false);

            if (shiftByDate.IsSuccess && shiftByDate.Value is not null)
            {
                logger.LogInformation("Shift already exists with ID: {ShiftID}", shiftByDate.Value.ShiftId);

                var dtoResult = ShiftCreatedEvent.ToDto(shiftByDate.Value);
                if (!dtoResult.IsSuccess || dtoResult.Value is null)
                {
                    logger.LogError("Failed to convert shift to DTO: {ShiftId}", shiftByDate.Value.ShiftId);
                    return Result<ShiftCreatedEvent>.WithFailure(dtoResult.Error ?? "Failed to convert shift to DTO");
                }

                await this.GetCyclesOkUpdateShiftDuration(dtoResult.Value, request, cancellationToken).ConfigureAwait(false);

                return dtoResult;
            }

            // Create a new shift
            var newShift = await this.CreateNewShiftAsync(request, cancellationToken).ConfigureAwait(false);

            var entitiesSaved = await shiftRepository.AddAsync(newShift, cancellationToken).ConfigureAwait(false);

            if (entitiesSaved.Value == 0)
            {
                logger.LogError("Shift not created at start time {startTime}", request.StartBy);
                return Result<ShiftCreatedEvent>.WithFailure($"Shift not created at start time {request.StartBy}");
            }

            var resultNewShift = ShiftCreatedEvent.ToDto(newShift);
            if (!resultNewShift.IsSuccess || resultNewShift.Value is null)
            {
                logger.LogError("Failed to convert new shift to DTO");
                return Result<ShiftCreatedEvent>.WithFailure(resultNewShift.Error ?? "Failed to convert new shift to DTO");
            }

            await this.GetCyclesOkUpdateShiftDuration(resultNewShift.Value, request, cancellationToken).ConfigureAwait(false);
            return resultNewShift;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the shift");
            return Result<ShiftCreatedEvent>.WithFailure($"An error occurred {ex}");
        }
    }

    private async Task<Shift> CreateNewShiftAsync(CreateShiftCommand request, CancellationToken cancellationToken)
    {
        var cyclesOk = await cycleRepository.GetProductionByShiftAsync(
            request.StartBy,
            request.StartBy + request.Duration,
            request.MachineId,
            cancellationToken).ConfigureAwait(false);

        return new Shift(new DateTimeMachine())
        {
            StartBy = request.StartBy,
            Duration = request.Duration,
            EndTime = request.StartBy + request.Duration,
            ShiftType = "Normal",
            CyclesOk = cyclesOk.Value,
        };
    }

    private async Task GetCyclesOkUpdateShiftDuration(ShiftCreatedEvent result, CreateShiftCommand request, CancellationToken cancellationToken)
    {
        var resultCycles = await cycleRepository.GetProductionByShiftAsync(
            result.StartBy,
            result.EndTime,
            request.MachineId,
            cancellationToken).ConfigureAwait(false);

        result.CyclesOk = resultCycles.Value;
        result.Duration = request.Duration;
    }
}
