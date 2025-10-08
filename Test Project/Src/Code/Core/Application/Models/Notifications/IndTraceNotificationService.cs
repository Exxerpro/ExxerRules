// <copyright file="IndTraceNotificationService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Notifications;

/// <summary>
/// Provides a notification service implementation for IndTrace notifications.
/// </summary>
public class IndTraceNotificationService : INotificationService
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate notification logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [IMPROVEMENT][ABEL][22/AUG/2025]
    // Implement actual notification sending logic
    // (e.g., email, SMS, push notification)
    // We need to inject a real notification service
    // What service we are going to use for sending notifications?
    // SignalR is already on the tech stack

    //[Fix]
    //CLAUDE
    //Date: 27/09/2025
    //Reason: [Pattern Null Safety] - Eliminated CS8618 pragma by properly initializing required field
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndTraceNotificationService"/> class.
    /// </summary>
    /// <param name="notificationService">The notification service implementation.</param>
    public IndTraceNotificationService(INotificationService notificationService)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    /// <summary>
    /// Executes SendAsync operation.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of SendAsync.</returns>
    public async Task<Result> SendAsync(MessageDto message, CancellationToken cancellationToken = default)
    {
        if (message == null)
        {
            return Result.WithFailure("Message cannot be null");
        }

        try
        {
            return await _notificationService.SendAsync(message, cancellationToken);
        }
        catch (Exception e)
        {
            return Result.WithFailure(e.Message);
        }
    }
}
