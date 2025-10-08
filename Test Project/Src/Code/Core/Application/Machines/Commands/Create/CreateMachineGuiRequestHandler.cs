// <copyright file="CreateMachineGuiRequestHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Commands.Create;

/// <summary>
/// Handles the creation of new machines within the IndTrace manufacturing system.
/// </summary>
/// <remarks>
/// This handler manages the creation of production machines including printers, assembly stations,
/// test stations, and other manufacturing equipment. It validates machine uniqueness,
/// creates the machine entity, and returns a creation confirmation.
/// </remarks>
public class CreateMachineMonitorRequestHandler(
    IRepository<Machine> repository,
    ILogger<CreateMachineMonitorRequestHandler> logger)
    : IMonitorRequestHandler<CreateMachineMonitorRequest, MachineCreated>
{
    /// <summary>
    /// Processes the create machine command asynchronously.
    /// </summary>
    /// <param name="request">The create machine request containing machine details.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{T}"/> with a <see cref="MachineCreated"/> if successful,
    /// or error information if the operation fails.
    /// </returns>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Validates that no machine exists with the same ID or name
    /// 2. Creates a new machine entity with the provided configuration
    /// 3. Persists the machine to the repository
    /// 4. Returns a creation confirmation with the machine details.
    /// </remarks>
    public async Task<Result<MachineCreated>> ProcessAsync(CreateMachineMonitorRequest request, CancellationToken cancellationToken)
    {
        var specification = new Specification<Machine>(m =>
            m.MachineId == request.MachineId || m.Name == request.Name);

        var existingMachineResult = await repository.FirstOrDefaultAsync(specification, cancellationToken);

        if (existingMachineResult.IsSuccess && existingMachineResult.Value is not null)
        {
            logger.LogWarning("Machine creation failed: duplicate found for MachineId: {MachineId}, Name: {Name}", request.MachineId, request.Name);
            return Result<MachineCreated>.WithFailure(new[] { "A machine with the same ID or name already exists." });
        }

        // [Fix]
        // CLAUDE
        // Date: 24/08/2025
        // Reason: [SIMPLE PRAGMATIC FIX] - Check for critical database failures vs "entity not found". Only proceed if entity not found, fail fast on real database errors.
        if (existingMachineResult.IsFailure)
        {
            var errorText = string.Join(" ", existingMachineResult.Errors ?? []).ToLower();
            if (!errorText.Contains("entity"))
            {
                // Real database error (connection, timeout, etc.) - fail fast
                logger.LogError("Repository failure during machine validation: {Errors}", string.Join(", ", existingMachineResult.Errors ?? []));
                return Result<MachineCreated>.WithFailure(existingMachineResult.Errors);
            }
        }

        // Either Success with null OR "entity not found" error → safe to create machine
        var newMachine = new Machine
        {
            MachineId = request.MachineId,
            Name = request.Name,
            Location = request.Location,
            MachineType = request.MachineType,
            EnableAppTraceability = request.EnableAppTraceability,
            EnableBypassTraceability = request.EnableBypassTraceability,
        };

        logger.LogInformation(newMachine.ToString());

        // [Fix]
        // CLAUDE
        // Date: 24/08/2025
        // Reason: [RAILWAY-ORIENTED PROGRAMMING BUG] - Handler was missing IsFailure check after AddAsync, causing test to expect failure but get success
        var createResult = await repository.AddAsync(newMachine, cancellationToken);
        if (createResult.IsFailure)
        {
            logger.LogError("Failed to add machine to database: {Errors}", string.Join(", ", createResult.Errors ?? []));
            return Result<MachineCreated>.WithFailure(createResult.Errors);
        }

        await repository.DetachAsync(newMachine, cancellationToken);

        var resultDto = MachineCreated.ToDto(newMachine);
        if (resultDto.IsSuccess)
        {
            if (resultDto.Value is null)
            {
                logger.LogError("DTO conversion returned null value");
                return Result<MachineCreated>.WithFailure("DTO conversion returned null value");
            }

            return Result<MachineCreated>.Success(resultDto.Value);
        }
        else
        {
            logger.LogError("Failed to create MachineCreated DTO: {Errors}", string.Join(", ", resultDto.Errors ?? []));
            return Result<MachineCreated>.WithFailure(resultDto.Errors);
        }
    }
}
