// <copyright file="IGatewayCommandDispatcher.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

using IndTrace.Domain.Models;

/// <summary>
/// Defines a contract for dispatching gateway commands and queries in the application.
/// </summary>
public interface IGatewayCommandDispatcher
{
    ///// <summary>
    ///// Dispatches a command that does not expect a response.
    ///// </summary>
    // Task<Result> ProcessAsync(IGatewayRequest command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a command that expects a typed response.
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, containing the result with the response.</returns>
    Task<Result<TResponse>> ProcessAsync<TResponse>(IGatewayRequest<TResponse> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a gateway query and returns a response of the specified type.
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected.</typeparam>
    /// <param name="request">The query to execute.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, containing the result with the response.</returns>
    Task<Result<TResponse>> QueryAsync<TResponse>(IGatewayRequest<TResponse> request, CancellationToken cancellationToken = default);
}
