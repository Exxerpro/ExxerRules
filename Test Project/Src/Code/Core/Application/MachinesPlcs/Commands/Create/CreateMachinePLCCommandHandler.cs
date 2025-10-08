// <copyright file="CreateMachinePLCCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.MachinesPlcs.Commands.Create;

/// <summary>
/// Handles the creation of new machine-PLC relationship entities in the system.
/// These relationships define which PLCs are associated with specific machines in the industrial setup.
/// </summary>
public class CreateMachinePlcCommandHandler : IMonitorRequestHandler<CreateMachinePlcCommand, MachinePlcCreated>
{
    private readonly IRepository<MachinePlc> repository;
    private readonly ILogger<CreateMachinePlcCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateMachinePlcCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository for accessing machine-PLC relationship data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public CreateMachinePlcCommandHandler(IRepository<MachinePlc> repository, ILogger<CreateMachinePlcCommandHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the machine-PLC relationship creation command.
    /// </summary>
    /// <param name="request">The command containing machine-PLC relationship data to create.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the created machine-PLC relationship notification.</returns>
    public async Task<Result<MachinePlcCreated>> ProcessAsync(CreateMachinePlcCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<MachinePlcCreated>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<MachinePlcCreated>.WithFailure("Operation was canceled.");
        }

        try
        {
            var entity = new MachinePlc(request.MachineId, request.PlCsId, 1);

            var addResult = await this.repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            if (!addResult.IsSuccess)
            {
                this.logger.LogError("Failed to add MachinePlc: {Errors}", string.Join(", ", addResult.Errors ?? []));
                return Result<MachinePlcCreated>.WithFailure(addResult.Errors);
            }

            var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
            if (!commitResult.IsSuccess)
            {
                this.logger.LogError("Failed to commit MachinePlc creation: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                return Result<MachinePlcCreated>.WithFailure(commitResult.Errors);
            }

            var response = new MachinePlcCreated
            {
                MachineId = entity.MachineId,
                PlCsId = entity.PlcId,
            };

            return Result<MachinePlcCreated>.Success(response);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in CreateMachinePlcCommandHandler");
            return Result<MachinePlcCreated>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
