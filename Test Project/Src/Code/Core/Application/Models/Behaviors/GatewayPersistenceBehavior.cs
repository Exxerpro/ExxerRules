// <copyright file="GatewayPersistenceBehavior.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Behaviors;

/// <summary>
/// Pipeline behavior for persisting gateway requests and responses in the pipeline.
/// </summary>
/// <typeparam name="TRequest">Type of the request being processed.</typeparam>
/// <typeparam name="TResponse">Type of the response produced.</typeparam>
public class GatewayPersistenceBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger,
    IRepository<TaskGatewayRequest> requestRepo,
    IRepository<TaskGatewayResponse> resultRepo)
    : IPipelineBehavior<TRequest, TResponse>
{
    /// <summary>
    /// Handles the request, persists the command and result, and manages error handling for gateway operations.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response of the action.</returns>
    public async Task<TResponse> HandleAsync(TRequest request, RequestFunctionalHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        int commandId = 0;

        if (request is ICommandData commandData && commandData.Command.EnsureIsValidToRenderAndPersist())
        {
            try
            {
                await requestRepo.AddAsync(commandData.Command, cancellationToken).ConfigureAwait(false);
                commandId = commandData.Command.CommandId;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding request");
            }
        }

        TResponse response = await next().ConfigureAwait(false);

        if (response is not Result<TaskGatewayResponse> result)
        {
            return response;
        }

        try
        {
            await this.HandleResultPersistenceAsync(result, request, commandId, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while handling gateway persistence");
            throw;
        }

        return response;
    }

    private async Task HandleResultPersistenceAsync(Result<TaskGatewayResponse> result, TRequest request, int commandId, CancellationToken cancellationToken)
    {
        try
        {
            if (result is { IsSuccess: true, Value: not null })
            {
                await this.PersistValidResultAsync(request, result.Value, commandId, cancellationToken).ConfigureAwait(false);
            }
            else if (result is { IsFailure: true, Value: not null } && result.Value.EnsureIsValidToRenderAndPersist())
            {
                await this.PersistFailedResultAsync(request, result, commandId, cancellationToken).ConfigureAwait(false);
            }
            else if (result.Value is null && request is ICommandData command)
            {
                await this.PersistFallbackResultAsync(command, result.Errors.FirstOrDefault() ?? "No error message provided", commandId, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while handling gateway persistence");
        }
    }

    private async Task PersistValidResultAsync(TRequest request, TaskGatewayResponse value, int commandId, CancellationToken cancellationToken)
    {
        if (request is not ICommandData command)
        {
            return;
        }

        UpdateCommandWithSuccess(command.Command, commandId);
        value.CommandId = commandId;

        await Task.WhenAll(
            this.SafeExecuteAsync(() => requestRepo.UpdateAsync(command.Command, cancellationToken), "log 2"),
            this.SafeExecuteAsync(() => resultRepo.AddAsync(value, cancellationToken), "log 3"))
        .ConfigureAwait(false);
    }

    private async Task PersistFailedResultAsync(TRequest request, Result<TaskGatewayResponse> result, int commandId, CancellationToken cancellationToken)
    {
        if (request is not ICommandData command)
        {
            return;
        }

        var firstError = result.Errors.FirstOrDefault(e => !string.IsNullOrEmpty(e)) ?? "Invalid";
        var value = result.Value ?? new TaskGatewayResponse();
        UpdateCommandWithFailure(command.Command, commandId, firstError, value.ResultValidation);
        value.CommandId = commandId;
        value.Error = firstError;

        await Task.WhenAll(
            this.SafeExecuteAsync(() => requestRepo.UpdateAsync(command.Command, cancellationToken), "log 4"),
            this.SafeExecuteAsync(() => resultRepo.AddAsync(value, cancellationToken), "log 5"))
        .ConfigureAwait(false);
    }

    private async Task PersistFallbackResultAsync(ICommandData command, string error, int commandId, CancellationToken cancellationToken)
    {
        UpdateCommandWithFailure(command.Command, commandId, error ?? "Invalid", -1);

        await this.SafeExecuteAsync(() => requestRepo.UpdateAsync(command.Command, cancellationToken), "log 6").ConfigureAwait(false);

        // TODO : Check if this is the correct way to handle fallback result
        // It seems like the fallback result is being created from the command itself
        if (command is IBarCodeResult barCodeResult)
        {
            var fallbackResult = TaskGatewayResponse.ToDto(barCodeResult);
            fallbackResult.CommandId = commandId;

            if (fallbackResult.EnsureIsValidToRenderAndPersist())
            {
                await this.SafeExecuteAsync(() => resultRepo.AddAsync(fallbackResult, cancellationToken), "log 7").ConfigureAwait(false);
            }
        }
    }

    private static void UpdateCommandWithSuccess(TaskGatewayRequest command, int commandId)
    {
        command.CommandId = commandId;
        command.Comment = "Valid Request";
        command.ResultValidation = 1;
    }

    private static void UpdateCommandWithFailure(TaskGatewayRequest command, int commandId, string error, int validationResult)
    {
        command.CommandId = commandId;
        command.Comment = error;
        command.Error = error;
        command.ResultValidation = validationResult;
    }

    private async Task SafeExecuteAsync(Func<Task> action, string logMessage)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception)
        {
            logger.LogCritical(logMessage);
        }
    }
}
