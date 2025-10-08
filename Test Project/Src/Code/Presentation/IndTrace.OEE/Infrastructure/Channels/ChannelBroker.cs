// <copyright file="ChannelBroker.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Infrastructure.Channels;

using System; // Add this using directive to fix CS0246 for DateTime
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using IndTrace.Application.Models.Services;
using IndTrace.Domain.Entities;
using Microsoft.Extensions.Options;

using Questdb.Net;

// TODO: Replace direct ChannelBroker<T> references with IChannelBroker<T> for DI and abstraction.
// Context: ReactiveEventService, ViewModels, integration wiring.
// Date: 2025-06-17 - Authored after defensive refactor & test integration.
// ABR

/// <summary>
/// Provides a thread-safe channel broker for asynchronous message passing between producers and consumers.
/// </summary>
/// <typeparam name="T">The type of messages handled by the channel broker.</typeparam>
public class ChannelBroker<T> : IChannelBroker<T>
{
    private readonly Channel<T> channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelBroker{T}"/> class.
    /// </summary>
    public ChannelBroker()
    {
        this.channel = Channel.CreateUnbounded<T>(); // or CreateBounded<T>(...) if needed
    }

    /// <summary>
    /// Attempts to write to the channel safely, rejecting null or invalid inputs.
    /// </summary>
    /// <param name="message">The message to write to the channel.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, with a result indicating whether the write was successful.</returns>
    public async Task<bool> WriteAsync(T message, CancellationToken cancellationToken)
    {
        if (message == null)
        {
            return false;
        }

        // Domain-specific validation
        if (message is not PerformanceData pd || pd.TimeStamp >= new DateTime(1970, 1, 1))
        {
            try
            {
                await this.channel.Writer.WriteAsync(message, cancellationToken);
                return true;
            }
            catch (ChannelClosedException)
            {
                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the channel reader for receiving messages as an async stream.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>An async enumerable of messages.</returns>
    public async IAsyncEnumerable<T> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var item in this.channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Marks the channel as complete, preventing further writes.
    /// </summary>
    public void Complete()
    {
        this.channel.Writer.Complete();
    }
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate ChannelBroker logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
