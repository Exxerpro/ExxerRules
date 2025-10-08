// <copyright file="UpdateWorkFlowCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.WorkFlows.Commands.Update;

using IndTrace.Application.WorkFlows.Queries.GetDetail;

/// <summary>
/// Represents the UpdateWorkFlowCommandHandler.
/// </summary>
public class UpdateWorkFlowCommandHandler : IMonitorRequestHandler<UpdateWorkFlowCommand, WorkFlowDetailVm>
{
    private readonly IRepository<WorkFlow> repository;
    private readonly ILogger<UpdateWorkFlowCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWorkFlowCommandHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public UpdateWorkFlowCommandHandler(IRepository<WorkFlow> repository, ILogger<UpdateWorkFlowCommandHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Executes ProcessAsync operation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ProcessAsync.</returns>
    public async Task<Result<WorkFlowDetailVm>> ProcessAsync(UpdateWorkFlowCommand request, CancellationToken cancellationToken)
    {
        var getResult = await this.repository.GetByIdAsync(request.WorkFlowId ?? 0, cancellationToken).ConfigureAwait(false);
        if (!getResult.IsSuccess || getResult.Value == null)
        {
            this.logger.LogError("WorkFlow not found: {WorkFlowId}", request.WorkFlowId);
            return Result<WorkFlowDetailVm>.WithFailure($"WorkFlowId {request.WorkFlowId} does not exist");
        }

        var entity = getResult.Value;
        entity.WorkFlowId = request.WorkFlowId ?? entity.WorkFlowId;
        entity.ProductId = request.ProductId ?? entity.ProductId;
        entity.NextMachineId = request.NextMachineId ?? entity.NextMachineId;
        entity.LastMachineId = request.LastMachineId ?? entity.LastMachineId;

        var updateResult = await this.repository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        if (!updateResult.IsSuccess)
        {
            this.logger.LogError("Failed to update WorkFlow: {Errors}", string.Join(", ", updateResult.Errors ?? []));
            return Result<WorkFlowDetailVm>.WithFailure(updateResult.Errors);
        }

        var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
        if (!commitResult.IsSuccess)
        {
            this.logger.LogError("Failed to commit WorkFlow update: {Errors}", string.Join(", ", commitResult.Errors ?? []));
            return Result<WorkFlowDetailVm>.WithFailure(commitResult.Errors);
        }

        var dtoResult = WorkFlowDetailVm.ToDto(entity);
        if (dtoResult.IsFailure || dtoResult.Value is null)
        {
            this.logger.LogError("Failed to convert WorkFlow to DTO: {Errors}", string.Join(", ", dtoResult.Errors ?? []));
            return Result<WorkFlowDetailVm>.WithFailure(dtoResult.Errors);
        }

        return Result<WorkFlowDetailVm>.Success(dtoResult.Value);
    }
}
