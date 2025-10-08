// <copyright file="IOeeChannelProcessor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Infrastructure.Channels;

/// <summary>
/// Defines the contract for processing OEE data through channels.
/// </summary>
public interface IOeeChannelProcessor
{
    /// <summary>
    /// Processes OEE register data asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ProcessOeeRegisterAsync(CancellationToken cancellationToken = default);
}
