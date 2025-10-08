// <copyright file="AsyncCallers.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Helpers;

public static class AsyncCallers
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate async callers logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static async Task ExecuteAsync(Func<Task> action, ILogger logger, string errorMessage, IIndTraceControllerRx controller)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "{ErrorMessage}. Controller info: {@Controller}", errorMessage, controller);
        }
    }

    public static async Task<T?> ExecuteAsync<T>(Func<Task<T>> action, ILogger logger, string errorMessage, IIndTraceControllerRx controller)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "{ErrorMessage}. Controller info: {@Controller}", errorMessage, controller);
        }

        return default;
    }

    public static async Task<T?> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, ILogger logger, string errorMessage, IIndTraceControllerRx controller, CancellationToken cancellationToken)
    {
        try
        {
            return await action(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "{ErrorMessage}. Controller info: {@Controller}", errorMessage, controller);
        }

        return default;
    }
}
