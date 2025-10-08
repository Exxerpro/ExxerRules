// <copyright file="EventsService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Notifications;

using IndTrace.Application.Notifications.Events.GetEventList;

/// <summary>
/// Represents the EventsService.
/// </summary>
public class EventsService(IMonitorRequestDispatcher monitorRequestDispatcher, ICacheService cache) : IEventsService
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate events service logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

    /// <summary>
    /// Gets or sets the ActualPage.
    /// </summary>
    public int ActualPage { get; set; } = 1;

    /// <summary>
    /// Gets or sets the PageSize.
    /// </summary>
    public int PageSize { get; set; } = 100;

    // Method to get new events

    /// <summary>
    /// Executes GetNextEventsAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of GetNextEventsAsync.</returns>
    public async Task<Result<EventsListVm>> GetNextEventsAsync(CancellationToken cancellationToken = default)
    {
        string cacheKey = $"Events_Page_{this.ActualPage}";

        // Cache the EventsListVm directly, not the Result wrapper
        var cachedResult = await cache.GetOrSetAsync<EventsListVm>(
            cacheKey,
            async token =>
            {
                var eventsList = await this.FetchEventsFromSourceAsync(this.ActualPage, this.PageSize, token).ConfigureAwait(false);
                return eventsList;
            },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Increment the page for the next call
        this.ActualPage++;

        return cachedResult != null
            ? Result<EventsListVm>.Success(cachedResult)
            : Result<EventsListVm>.WithFailure("Failed to retrieve events from cache");
    }

    // Method to get the previous page of events

    /// <summary>
    /// Executes GetPreviousEventsAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of GetPreviousEventsAsync.</returns>
    public async Task<Result<EventsListVm>> GetPreviousEventsAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<EventsListVm>.WithFailure("Operation cancelled.");
        }

        // Ensure that we do not navigate before the first page
        if (this.ActualPage <= 1)
        {
            return Result<EventsListVm>.WithFailure("Already on the first page.");
        }

        // Decrement the page number
        this.ActualPage--;
        string cacheKey = $"Events_Page_{this.ActualPage}";

        // Cache the EventsListVm directly, not the Result wrapper
        var cachedResult = await cache.GetOrSetAsync<EventsListVm>(
            cacheKey,
            async token =>
            {
                var eventsList = await this.FetchEventsFromSourceAsync(this.ActualPage, this.PageSize, token).ConfigureAwait(false);
                return eventsList;
            },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return cachedResult != null
            ? Result<EventsListVm>.Success(cachedResult)
            : Result<EventsListVm>.WithFailure("Failed to retrieve events from cache");
    }

    private async Task<EventsListVm> FetchEventsFromSourceAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetEventsListQuery(page, pageSize);
        var result = await monitorRequestDispatcher.QueryAsync(query, cancellationToken).ConfigureAwait(false);
        return result.Value ?? new EventsListVm(new List<TaskGatewayRequest>(), new List<TaskGatewayResponse>());
    }
}
