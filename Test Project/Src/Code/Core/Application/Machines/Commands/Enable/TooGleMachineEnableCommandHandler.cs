// <copyright file="TooGleMachineEnableCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Commands.Enable;

/// <summary>
/// Represents the TooGleMachineEnableCommandHandler.
/// </summary>
public class TooGleMachineEnableCommandHandler(IRepository<Machine> repository) : IMonitorRequestHandler<ToggleEnableMachineCommand, MachineDto>
{
    /// <summary>
    /// Executes ProcessAsync operation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ProcessAsync.</returns>
    public async Task<Result<MachineDto>> ProcessAsync(ToggleEnableMachineCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<MachineDto>.WithFailure(["Operation was canceled"]);
        }

        var specification = new Specification<Machine>(machine =>
             machine.MachineId == request.MachineId);

        var machineResult = await repository.FirstOrDefaultAsync(specification, cancellationToken);

        // [Fix]
        // CLAUDE
        // Date: 24/08/2025
        // Reason: [SAME PRAGMATIC PATTERN] - Apply database failure detection. Check for critical failures vs "entity not found".
        if (machineResult.IsFailure)
        {
            var errorText = string.Join(" ", machineResult.Errors ?? []).ToLower();
            if (!errorText.Contains("entity"))
            {
                // Real database error (connection, timeout, etc.) - fail fast
                return Result<MachineDto>.WithFailure(machineResult.Errors);
            }

            // Entity not found error - continue to null check below
        }

        var machine = machineResult.Value;
        if (machine is null)
        {
            return Result<MachineDto>.WithFailure([$"Machine {request.MachineId} does not exist please provide a valid RecipeId"]);
        }

        if (request.Enable)
        {
            machine.Enable();
        }

        if (request.Disable)
        {
            machine.Disable();
        }

        var updateResult = await repository.UpdateAsync(machine, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result<MachineDto>.WithFailure(updateResult.Errors);
        }

        // DetachAsync the entity after updating to avoid tracking issues
        try
        {
            await repository.DetachAsync(machine, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<MachineDto>.WithFailure([$"Failed to detach machine: {ex.Message}"]);
        }

        var result = MachineDto.ToDto(machine);

        return result;
    }
}
