// <copyright file="EventsListVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Notifications.Events.GetEventList;

/// <summary>
/// Represents the EventsListVm.
/// </summary>
public class EventsListVm
{
    /// <summary>
    /// Executes CreateEventsListVm operation.
    /// </summary>
    /// <param name="requests">The requests.</param>
    /// <param name="responses">The responses.</param>
    /// <returns>The result of CreateEventsListVm.</returns>
    public static Result<EventsListVm> CreateEventsListVm(Result<IEnumerable<TaskGatewayRequest>> requests, Result<IEnumerable<TaskGatewayResponse>> responses)
    {
        var result = new EventsListVm(requests.Value ?? [], responses.Value ?? []);

        if (requests.IsSuccess && responses.IsSuccess)
        {
            return Result<EventsListVm>.Success(result);
        }

        return Result<EventsListVm>.CombineErrors<EventsListVm>(requests.Errors, responses.Errors, result);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventsListVm"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="requests">The requests.</param>
    /// <param name="responses">The responses.</param>
    public EventsListVm(IEnumerable<TaskGatewayRequest> requests, IEnumerable<TaskGatewayResponse> responses)
    {
        this.Requests = requests;
        this.Responses = responses;
    }

    /// <summary>
    /// Gets the Requests.
    /// </summary>
    public IEnumerable<TaskGatewayRequest> Requests { get; private set; }

    /// <summary>
    /// Gets the Responses.
    /// </summary>
    public IEnumerable<TaskGatewayResponse> Responses { get; private set; }
}
