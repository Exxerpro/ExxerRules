// <copyright file="CreateShiftCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Commands.Create;

/// <summary>
/// Represents the CreateShiftCommandHandler.
/// </summary>
public class CreateShiftCommandHandler(IShiftService shiftService) : IMonitorRequestHandler<CreateShiftCommand, ShiftCreatedEvent>
{
    /// <inheritdoc/>
    public async Task<Result<ShiftCreatedEvent>> ProcessAsync(CreateShiftCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<ShiftCreatedEvent>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ShiftCreatedEvent>.WithFailure("Operation was canceled.");
        }

        try
        {
            return await shiftService.CreateOrRetrieveShiftAndCyclesOkAsync(request.MachineId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Result<ShiftCreatedEvent>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
