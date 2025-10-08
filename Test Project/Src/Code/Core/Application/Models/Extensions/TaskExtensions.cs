// <copyright file="TaskExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Extensions;

/// <summary>
/// Provides extension methods for safely running fire-and-forget tasks with optional error logging.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Runs a task in a fire-and-forget manner and logs any exceptions that occur.
    /// </summary>
    /// <param name="task">The task to run.</param>
    /// <param name="logger">The logger to use for error logging (optional).</param>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure all awaited tasks are properly observed and exceptions are handled. Use ConfigureAwait(false) where appropriate for library code.
    public static void FireAndForgetSafeAsync(this Task task, ILogger? logger = null)
    {
        task.ContinueWith(
            t =>
        {
            if (t.IsFaulted && t.Exception != null)
            {
                // Log the exception
                logger?.LogError(t.Exception, "An exception occurred in a fire-and-forget task.");
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}
