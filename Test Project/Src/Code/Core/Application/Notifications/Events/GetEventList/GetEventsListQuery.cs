// <copyright file="GetEventsListQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Notifications.Events.GetEventList;

/// <summary>
/// Represents the GetEventsListQuery.
/// </summary>
public class GetEventsListQuery(int pageNumber, int pageSize) : IMonitorRequest<EventsListVm>
{
    /// <summary>
    /// Gets or sets the StartDate.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the EndDate.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the PageNumber.
    /// </summary>
    public int PageNumber { get; set; } = pageNumber;

    /// <summary>
    /// Gets or sets the PageSize.
    /// </summary>
    public int PageSize { get; set; } = pageSize;
}
