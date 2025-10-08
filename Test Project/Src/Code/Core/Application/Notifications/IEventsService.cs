// <copyright file="IEventsService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Notifications;

using IndTrace.Application.Notifications.Events.GetEventList;

public interface IEventsService
{
    int ActualPage { get; set; }

    int PageSize { get; set; }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate events service interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    Task<Result<EventsListVm>> GetNextEventsAsync(CancellationToken cancellationToken = default);

    Task<Result<EventsListVm>> GetPreviousEventsAsync(CancellationToken cancellationToken = default);
}
