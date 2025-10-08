// <copyright file="MachineUpdateCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Commands.Update;

using IndTrace.Application.Machines.Commands.Create;

/// <summary>
/// Represents the MachineUpdateCommandHandler.
/// </summary>
public class MachineUpdateCommandHandler(IRepository<Machine> repository, ILogger<MachineUpdateCommandHandler> logger, IMonitorRequestDispatcher monitorRequestDispatcher)
    : IMonitorRequestHandler<MachineUpdateCommand, MachineDto>
{
    private IMonitorRequestDispatcher monitorRequestDispatcher = monitorRequestDispatcher;

    /// <inheritdoc/>
    public async Task<Result<MachineDto>> ProcessAsync(MachineUpdateCommand request, CancellationToken cancellationToken)
    {
        var specification = new Specification<Machine>(m =>
            m.MachineId == request.MachineId || m.Name == request.Name);

        // Using AsTracking ensures the entity is tracked by the DbContext
        var resultMachine = await repository.FirstOrDefaultAsync(specification, cancellationToken);

        // [Fix]
        // CLAUDE
        // Date: 24/08/2025
        // Reason: [SAME PRAGMATIC PATTERN] - Apply database failure detection. Check for critical failures vs "entity not found".
        if (resultMachine.IsFailure)
        {
            var errorText = string.Join(" ", resultMachine.Errors ?? []).ToLower();
            if (!errorText.Contains("entity"))
            {
                // Real database error (connection, timeout, etc.) - fail fast
                logger.LogError("Repository failure during machine lookup: {Errors}", string.Join(", ", resultMachine.Errors ?? []));
                return Result<MachineDto>.WithFailure(resultMachine.Errors);
            }
        }

        if (resultMachine.Value is null)
        {
            logger.LogWarning("Machine {MachineId} does not exist", request.MachineId);
            return Result<MachineDto>.WithFailure($"Machine {request.MachineId} does not exist please provide a valid MachineId");
        }

        var machine = resultMachine.Value;

        machine.Name = request.Name ?? machine.Name;
        machine.Location = request.Location ?? machine.Location;
        machine.WorkFlowType = request.WorkFlowType ?? machine.WorkFlowType;
        machine.MachineType = request.MachineType ?? machine.MachineType;

        var resultUpdate = await repository.UpdateAsync(machine, cancellationToken);

        if (resultUpdate.IsFailure)
        {
            return Result<MachineDto>.WithFailure(new[] { $"Failure updating machine {request.MachineId} " }).Combine(resultUpdate);
        }

        var result = MachineDto.ToDto(machine);

        return result;
    }
}
