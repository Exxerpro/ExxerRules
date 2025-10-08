// <copyright file="GatewayExecutionHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Helpers;

public static class GatewayExecutionHelper
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate gateway execution helper logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static async Task<Result<T>> ExecuteWithTimeoutAndLogging<T>(
        Func<CancellationToken, Task<Result<T>>> action,
        TimeSpan timeout,
        CancellationToken parentToken,
        ILogger logger,
        string operationName = "Operation")
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(parentToken, timeoutCts.Token);

        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("Gateway Operation Starting {OperationName} with timeout {TimeoutSeconds} seconds.", operationName, timeout.TotalSeconds);

        try
        {
            var result = await action(linkedCts.Token).ConfigureAwait(false);
            stopwatch.Stop();
            if (result.IsSuccess)
            {
                logger.LogInformation("Gateway Operation {OperationName} completed successfully in {ElapsedMilliseconds} ms.", operationName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.LogWarning("Gateway Operation {OperationName} completed with failure in {ElapsedMilliseconds} ms.", operationName, stopwatch.ElapsedMilliseconds);
            }

            return result;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !parentToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            logger.LogWarning(
                "Gateway Operation {OperationName} timed out after {ElapsedMilliseconds} ms (timeout limit was {TimeoutSeconds} seconds).",
                operationName, stopwatch.ElapsedMilliseconds, timeout.TotalSeconds);

            return Result<T>.WithFailure($"{operationName} timed out. Please retry.");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "Gateway Operation {OperationName} failed after {ElapsedMilliseconds} ms due to unexpected error.",
                operationName, stopwatch.ElapsedMilliseconds);

            return Result<T>.WithFailure($"Gateway Operation {operationName} failed unexpectedly.");
        }
    }

    public static async Task<Result> ExecuteWithTimeoutAndLogging(
       Func<CancellationToken, Task<Result>> action,
       TimeSpan timeout,
       CancellationToken parentToken,
       ILogger logger,
       string operationName = "Operation")
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(parentToken, timeoutCts.Token);

        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("Gateway Operation Starting {OperationName} with timeout {TimeoutSeconds} seconds.", operationName, timeout.TotalSeconds);

        try
        {
            var result = await action(linkedCts.Token).ConfigureAwait(false);
            stopwatch.Stop();
            if (result.IsSuccess)
            {
                logger.LogInformation("Gateway Operation {OperationName} completed successfully in {ElapsedMilliseconds} ms.", operationName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.LogWarning("Gateway Operation {OperationName} completed with failure in {ElapsedMilliseconds} ms.", operationName, stopwatch.ElapsedMilliseconds);
            }

            return result;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !parentToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            logger.LogWarning(
                "Gateway Operation {OperationName} timed out after {ElapsedMilliseconds} ms (timeout limit was {TimeoutSeconds} seconds).",
                operationName, stopwatch.ElapsedMilliseconds, timeout.TotalSeconds);

            return Result.WithFailure($"{operationName} timed out. Please retry.");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "Gateway Operation {OperationName} failed after {ElapsedMilliseconds} ms due to unexpected error.",
                operationName, stopwatch.ElapsedMilliseconds);

            return Result.WithFailure($"Gateway Operation {operationName} failed unexpectedly.");
        }
    }
}
