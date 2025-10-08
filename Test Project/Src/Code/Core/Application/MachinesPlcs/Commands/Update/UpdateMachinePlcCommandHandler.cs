// <copyright file="UpdateMachinePlcCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.MachinesPlcs.Commands.Update;

using IndTrace.Application.MachinesPlcs.Queries.GetDetail;

/// <summary>
/// Handles the updating of MachinePLC entities.
/// </summary>
public class UpdateMachinePlcCommandHandler : IMonitorRequestHandler<UpdateMachinePlcCommand, MachinePlcDetailVm>
{
    private readonly IRepository<MachinePlc> repository;
    private readonly ILogger<UpdateMachinePlcCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateMachinePlcCommandHandler"/> class.
    /// Constructs a new instance of <see cref="UpdateMachinePlcCommandHandler"/>.
    /// </summary>
    /// <param name="repository">The repository for MachinePlc entities.</param>
    /// <param name="logger">The logger instance.</param>
    public UpdateMachinePlcCommandHandler(IRepository<MachinePlc> repository, ILogger<UpdateMachinePlcCommandHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the incoming request.
    /// </summary>
    /// <param name="request">The request to update a MachinePLC entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A MachinePlcDetailVm object representing the updated MachinePLC entity.</returns>
    public async Task<Result<MachinePlcDetailVm>> ProcessAsync(UpdateMachinePlcCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<MachinePlcDetailVm>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<MachinePlcDetailVm>.WithFailure("Operation was canceled.");
        }

        try
        {
            // Fetch the MachinePLC entity from the database
            var getAllResult = await this.repository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (!getAllResult.IsSuccess)
            {
                this.logger.LogError("Failed to retrieve MachinePlcs: {Errors}", string.Join(", ", getAllResult.Errors ?? []));
                return Result<MachinePlcDetailVm>.WithFailure(getAllResult.Errors);
            }

            var machinePlc = getAllResult.Value?.FirstOrDefault(mp => mp.MachineId == request.MachineId && mp.PlcId == request.PlcId);
            if (machinePlc == null)
            {
                this.logger.LogError("MachinePLC not found: MachineId={MachineId}, PlcId={PlcId}", request.MachineId, request.PlcId);
                return Result<MachinePlcDetailVm>.WithFailure($"MachinePLC with MachineId: {request.MachineId} and PlcId: {request.PlcId} cannot be found");
            }

            // This table has a composite primary key and that is the only one information in the table
            // In order to update, the row has to be deleted, we are going to add another columns with the data
            // Is Active, and make it an auditable entity

            // So now we have two cases

            // If the upgrade is in IsActive column, is a normal update
            // in the other hand if the request change one of the columns in the composite primary key, we must to delete (set to inactive the row)
            // And after that insert a new entry,
            // but what if the entity is already in the table, in this cases we update the existing entry ?

            // If the update only involves 'IsActive', perform a normal update
            if (ShouldUpdateIsActiveOnly(request, machinePlc))
            {
                // If the upgrade is in IsActive column, is a normal update
                machinePlc.IsActive = request.IsActive ?? machinePlc.IsActive;

                var updateResult = await this.repository.UpdateAsync(machinePlc, cancellationToken).ConfigureAwait(false);
                if (!updateResult.IsSuccess)
                {
                    this.logger.LogError("Failed to update MachinePlc IsActive: {Errors}", string.Join(", ", updateResult.Errors ?? []));
                    return Result<MachinePlcDetailVm>.WithFailure(updateResult.Errors);
                }

                var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
                if (!commitResult.IsSuccess)
                {
                    this.logger.LogError("Failed to commit MachinePlc IsActive update: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                    return Result<MachinePlcDetailVm>.WithFailure(commitResult.Errors);
                }

                var dto1 = MachinePlcDetailVm.ToDto(machinePlc);
                if (!dto1.IsSuccess || dto1.Value is null)
                {
                    return Result<MachinePlcDetailVm>.WithFailure(dto1.Errors);
                }

                return Result<MachinePlcDetailVm>.Success(dto1.Value);
            }

            // If the update involves changes to the MachineId or PlcId, mark the existing row as inactive, and create a new active row
            if (ShouldUpdateKeyAndIsActive(request, machinePlc))
            {
                // in this case, first delete (Update is Active to Inactive) the requested row
                machinePlc.IsActive = -1;

                var updateResult = await this.repository.UpdateAsync(machinePlc, cancellationToken);
                if (!updateResult.IsSuccess)
                {
                    this.logger.LogError("Failed to deactivate existing MachinePlc: {Errors}", string.Join(", ", updateResult.Errors ?? []));
                    return Result<MachinePlcDetailVm>.WithFailure(updateResult.Errors);
                }

                // Now we must to add a new row
                var newMachinePlc = new MachinePlc(
                    request.NewMachineId ?? machinePlc.MachineId,
                    request.NewPlcId ?? machinePlc.PlcId, (int)ActiveStatus.Active);

                var addResult = await this.repository.AddAsync(newMachinePlc, cancellationToken).ConfigureAwait(false);
                if (!addResult.IsSuccess)
                {
                    this.logger.LogError("Failed to add new MachinePlc: {Errors}", string.Join(", ", addResult.Errors ?? []));
                    return Result<MachinePlcDetailVm>.WithFailure(addResult.Errors);
                }

                var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
                if (!commitResult.IsSuccess)
                {
                    this.logger.LogError("Failed to commit MachinePlc key update: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                    return Result<MachinePlcDetailVm>.WithFailure(commitResult.Errors);
                }

                var dto2 = MachinePlcDetailVm.ToDto(newMachinePlc);
                if (!dto2.IsSuccess || dto2.Value is null)
                {
                    return Result<MachinePlcDetailVm>.WithFailure(dto2.Errors);
                }

                return Result<MachinePlcDetailVm>.Success(dto2.Value);
            }

            // If there's no provided Active status and there's no new Machine or PLC ID provided.
            if (this.IsActiveNullAndKeyIsNull(request))
            {
                var dto3 = MachinePlcDetailVm.ToDto(machinePlc);
                if (!dto3.IsSuccess || dto3.Value is null)
                {
                    return Result<MachinePlcDetailVm>.WithFailure(dto3.Errors);
                }

                return Result<MachinePlcDetailVm>.Success(dto3.Value);
            }

            // If the Active status is the same and there's no new Machine or PLC ID provided.
            if (this.IsActiveSameAndKeyIsNull(request, machinePlc))
            {
                var dto4 = MachinePlcDetailVm.ToDto(machinePlc);
                if (!dto4.IsSuccess || dto4.Value is null)
                {
                    return Result<MachinePlcDetailVm>.WithFailure(dto4.Errors);
                }

                return Result<MachinePlcDetailVm>.Success(dto4.Value);
            }

            // in any other case this is a invalid request
            this.logger.LogError("Invalid update request for MachinePLC: MachineId={MachineId}, PlcId={PlcId}", request.MachineId, request.PlcId);
            return Result<MachinePlcDetailVm>.WithFailure($"MachinePLC with MachineId: {request.MachineId} and PlcId: {request.PlcId} cannot be updated");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in UpdateMachinePlcCommandHandler");
            return Result<MachinePlcDetailVm>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if the request involves updating 'IsActive' only.
    /// </summary>
    private static bool ShouldUpdateIsActiveOnly(UpdateMachinePlcCommand request, MachinePlc machinePlc)
    {
        var result =
            request.IsActive != null
               && request.IsActive != machinePlc.IsActive
               && request.NewMachineId == null
               && request.NewPlcId == null;

        return result;
    }

    /// <summary>
    /// Checks if the request involves updating the keys and 'IsActive'.
    /// </summary>
    private static bool ShouldUpdateKeyAndIsActive(UpdateMachinePlcCommand request, MachinePlc machinePlc)
    {
        var result =
         (request.NewMachineId != null || request.NewPlcId != null)
               && machinePlc.IsActive == (int)ActiveStatus.Active;

        return result;
    }

    /// <summary>
    /// Checks if the request involves no changes to the active status and keys.
    /// </summary>
    private bool IsActiveSameAndKeyIsNull(UpdateMachinePlcCommand request, MachinePlc machinePlc)
    {
        return request.IsActive == machinePlc.IsActive && request.NewMachineId == null && request.NewPlcId == null;
    }

    /// <summary>
    /// Checks if the request does not provide an active status or new keys.
    /// </summary>
    private bool IsActiveNullAndKeyIsNull(UpdateMachinePlcCommand request)
    {
        return request.IsActive == null && request.NewMachineId == null && request.NewPlcId == null;
    }
}
