// <copyright file="TaskExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Behaviors;

/// <summary>
/// Provides extension methods for running fire-and-forget tasks with exception handling.
/// </summary>
public static class TaskExtensions
{
    private static ILogger? logger;

    /// <summary>
    /// Sets the logger for TaskExtensions. Call this during application startup.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging operations.</param>
    public static void SetLogger(ILogger logger)
    {
        TaskExtensions.logger = logger;
    }

    /// <summary>
    /// Runs a task in a fire-and-forget manner and logs any exceptions to the console.
    /// </summary>
    /// <param name="task">The task to run.</param>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure all awaited tasks are properly observed and exceptions are handled. Use ConfigureAwait(false) where appropriate for library code.
    public static void Forget(this Task task)
    {
        // note: this code is inspired by a tweet from Ben Adams: https://twitter.com/ben_a_adams/status/1045060828700037125
        // Only care about tasks that may fault (not completed) or are faulted,
        // so fast-path for SuccessfullyCompleted and Canceled tasks.
        if (!task.IsCompleted || task.IsFaulted)
        {
            // use "_" (Discard operation) to remove the warning IDE0058: Because this call is not awaited, execution of the current method continues before the call is completed
            // https://docs.microsoft.com/en-us/dotnet/csharp/discards#a-standalone-discard
            _ = ForgetAwaited(task);
        }

        // Allocate the async/await state machine only when needed for performance reason.
        // More info about the state machine: https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/
        static async Task ForgetAwaited(Task task)
        {
            try
            {
                // No need to resume on the original SynchronizationContext, so use ConfigureAwait(false)
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // 🔴 Fixed: Use proper logging instead of Console.WriteLine anti-pattern
                logger?.LogError(ex, "Error in task fire-and-forget operation");
            }
        }
    }
}
