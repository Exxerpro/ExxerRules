// <copyright file="IPerformanceQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

using IndTrace.Domain.Models;

/// <summary>
/// Defines the contract for query handlers that process queries and return results.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResult">The type of result to return.</typeparam>
public interface IPerformanceQueryHandler<in TQuery, TResult>
{
    /// <summary>
    /// Handles the specified query and returns a result.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a result.</returns>
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
