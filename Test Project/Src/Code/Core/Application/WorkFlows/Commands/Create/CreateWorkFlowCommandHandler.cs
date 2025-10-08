// <copyright file="CreateWorkFlowCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.WorkFlows.Commands.Create;

/// <summary>
/// Represents the CreateWorkFlowCommandHandler.
/// </summary>
public class CreateWorkFlowCommandHandler : IMonitorRequestHandler<CreateWorkFlowCommand, WorkFlowCreatedEvent>
{
    private readonly IRepository<WorkFlow> repository;
    private readonly ILogger<CreateWorkFlowCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWorkFlowCommandHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public CreateWorkFlowCommandHandler(IRepository<WorkFlow> repository, ILogger<CreateWorkFlowCommandHandler> logger)
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
    public async Task<Result<WorkFlowCreatedEvent>> ProcessAsync(CreateWorkFlowCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<WorkFlowCreatedEvent>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<WorkFlowCreatedEvent>.WithFailure("Operation was canceled.");
        }

        try
        {
            var entity = new WorkFlow
            {
                ProductId = request.ProductId,
                NextMachineId = request.LastMachineId,
                LastMachineId = request.NextMachineId,
            };

            var addResult = await this.repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            if (!addResult.IsSuccess)
            {
                this.logger.LogError("Failed to add WorkFlow: {Errors}", string.Join(", ", addResult.Errors ?? []));
                return Result<WorkFlowCreatedEvent>.WithFailure(addResult.Errors);
            }

            var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
            if (!commitResult.IsSuccess)
            {
                this.logger.LogError("Failed to commit WorkFlow creation: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                return Result<WorkFlowCreatedEvent>.WithFailure(commitResult.Errors);
            }

            var response = new WorkFlowCreatedEvent
            {
                ProductId = entity.ProductId,
                NextMachineId = entity.NextMachineId,
                LastMachineId = entity.LastMachineId,
            };

            return Result<WorkFlowCreatedEvent>.Success(response);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in CreateWorkFlowCommandHandler");
            return Result<WorkFlowCreatedEvent>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
