// <copyright file="UpdateVariableCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Variables.Commands.Update;

using IndTrace.Application.Variables.Queries.GetVariableDetail;

/// <summary>
/// Handles the updating of existing variable entities in the system.
/// Variables represent data points that can be monitored and tracked in the industrial process.
/// </summary>
public class UpdateVariableCommandHandler : IMonitorRequestHandler<UpdateVariableCommand, VariableDetailVm>
{
    private readonly IRepository<Variable> repository;
    private readonly ILogger<UpdateVariableCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateVariableCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository for accessing variable data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public UpdateVariableCommandHandler(IRepository<Variable> repository, ILogger<UpdateVariableCommandHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the variable update command.
    /// </summary>
    /// <param name="request">The command containing updated variable data.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the updated variable details view model.</returns>
    public async Task<Result<VariableDetailVm>> ProcessAsync(UpdateVariableCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<VariableDetailVm>.WithFailure("Process was cancelled");
        }

        try
        {
            var getResult = await this.repository.GetByIdAsync(request.VariableId ?? 0, cancellationToken).ConfigureAwait(false);
            if (!getResult.IsSuccess || getResult.Value == null)
            {
                this.logger.LogError("Variable not found: {EntitieId}", request.VariableId);
                return Result<VariableDetailVm>.WithFailure("Variable not found");
            }

            var entity = getResult.Value;
            entity.Address = request.Address ?? entity.Address;
            entity.Direction = request.Direction ?? entity.Direction;
            entity.IsActive = request.Event ?? entity.IsActive;
            entity.Length = request.Length ?? entity.Length;
            entity.MachineId = request.MachineId ?? entity.MachineId;
            entity.NativeType = request.Model ?? entity.NativeType;
            entity.Name = request.Name ?? entity.Name;
            entity.NativeAddress = request.Transaction ?? entity.NativeAddress;
            entity.NetType = request.Type ?? entity.NetType;
            entity.VariableGroupId = request.VariableGroupId ?? entity.VariableGroupId;

            var updateResult = await this.repository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            if (!updateResult.IsSuccess)
            {
                this.logger.LogError("Failed to update Variable: {Errors}", string.Join(", ", updateResult.Errors ?? []));
                return Result<VariableDetailVm>.WithFailure(updateResult.Errors);
            }

            var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
            if (!commitResult.IsSuccess)
            {
                this.logger.LogError("Failed to commit Variable update: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                return Result<VariableDetailVm>.WithFailure(commitResult.Errors);
            }

            var dtoResult = VariableDetailVm.ToDto(entity);
            if (dtoResult.IsFailure)
            {
                this.logger.LogError("Failed to convert Variable to DTO: {Errors}", string.Join(", ", dtoResult.Errors ?? []));
                return Result<VariableDetailVm>.WithFailure(dtoResult.Errors);
            }

            if (dtoResult.Value is null)
            {
                this.logger.LogError("DTO conversion returned null value");
                return Result<VariableDetailVm>.WithFailure("DTO conversion returned null value");
            }

            return Result<VariableDetailVm>.Success(dtoResult.Value);
        }
        catch (Exception ex)
        {
            return Result<VariableDetailVm>.WithFailure($"Process resulted on exception {ex.Message}");
        }
    }
}
