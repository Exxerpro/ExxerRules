// <copyright file="IGatewayRequestHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

using IndTrace.Domain.Models;

/// <summary>
/// Gateway command handler for instructions that return no specific result.
/// </summary>
public interface IGatewayRequestHandler<TCommand>
    where TCommand : IGatewayRequest
{
    Task<Result> ProcessAsync(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Gateway command or query handler that expects a typed result.
/// </summary>
public interface IGatewayRequestHandler<TCommand, TResponse>
    where TCommand : IGatewayRequest<TResponse>
{
    Task<Result<TResponse>> ProcessAsync(TCommand command, CancellationToken cancellationToken);
}
