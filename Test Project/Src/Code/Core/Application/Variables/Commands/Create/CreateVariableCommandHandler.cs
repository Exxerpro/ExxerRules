// <copyright file="CreateVariableCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Variables.Commands.Create;

/// <summary>
/// Handles the creation of new variable entities in the system.
/// Variables represent data points that can be monitored and tracked in the industrial process.
/// </summary>
public class CreateVariableCommandHandler : IMonitorRequestHandler<CreateVariableCommand, VariableCreatedEvent>
{
    private readonly IRepository<Variable> repository;
    private readonly ILogger<CreateVariableCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateVariableCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository for accessing variable data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public CreateVariableCommandHandler(IRepository<Variable> repository, ILogger<CreateVariableCommandHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the variable creation command.
    /// </summary>
    /// <param name="request">The command containing variable data to create.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the created variable notification.</returns>
    public async Task<Result<VariableCreatedEvent>> ProcessAsync(CreateVariableCommand request, CancellationToken cancellationToken)
    {
        var entity = new Variable
        {
            MachineId = request.MachineId,
            Name = request.Name,
            Address = request.Address,
            NetType = request.Type,
            Length = request.Length,
            IsActive = request.Event,
            Direction = request.Direction,
            VariableGroupId = request.VariableGroupId,
            NativeType = request.Model,
            NativeAddress = request.Transaction,
        };

        if (cancellationToken.IsCancellationRequested)
        {
            this.logger.LogError("Task Canceled");
            return Result<VariableCreatedEvent>.WithFailure(["Task Canceled"]);
        }

        try
        {
            var addResult = await this.repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            if (addResult.IsFailure)
            {
                this.logger.LogError("Failed to add Variable: {Errors}", string.Join(", ", addResult.Errors ?? []));
                return Result<VariableCreatedEvent>.WithFailure(addResult.Errors);
            }

            var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
            if (commitResult.IsFailure)
            {
                this.logger.LogError("Failed to commit Variable: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                return Result<VariableCreatedEvent>.WithFailure(commitResult.Errors);
            }

            return Result<VariableCreatedEvent>.Success(new VariableCreatedEvent
            {
                MachineId = entity.MachineId,
                Name = entity.Name,
                Address = entity.Address,
                Type = entity.NetType,
                Length = entity.Length,
                Event = entity.IsActive,
                Direction = entity.Direction,
                VariableGroupId = entity.VariableGroupId,
                Model = entity.NativeType,
                Transaction = entity.NativeAddress,
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError("Eror ocurred while creating variable {Message} ", ex.Message);
            return Result<VariableCreatedEvent>.WithFailure([$"Eror ocurred while creating variable {ex.Message} "]);
        }
    }
}
