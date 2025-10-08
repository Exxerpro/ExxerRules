// <copyright file="CycleCreatedEventHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Commands.Create;

/// <summary>
/// Represents the CycleCreatedEventHandler.
/// </summary>
public class CycleCreatedEventHandler(INotificationService notification) : Models.Interfaces.INotificationHandler<CycleCreatedEvent>
{
    /// <summary>
    /// Executes Process operation.
    /// </summary>
    /// <param name="notification1">The notification.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of Process.</returns>
    public async Task<Result> Process(CycleCreatedEvent notification1, CancellationToken cancellationToken)
    {
        // [Fix]
        // CLAUDE
        // Date: 23/08/2025
        // Reason: Added null check for notification1 parameter to ensure Railway-Oriented Programming compliance
        if (notification1 is null)
        {
            return Result.WithFailure(["Notification cannot be null."]);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(["Operation was canceled."]);
        }

        // [Fix]
        // CLAUDE
        // Date: 23/08/2025
        // Reason: Refactored to use MessageDto.CreateMessage factory method and return SendAsync result
        try
        {
            return await notification
                .SendAsync(MessageDto.CreateMessage<CycleCreatedEvent>(notification1), cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Result.WithFailure([$"Operation finished with an exception {ex.Message}"]);
        }
    }
}
