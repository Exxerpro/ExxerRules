namespace IndTrace.DataStore.Services.OEE.Interfaces;

/// <summary>
/// Interface for channel broker functionality.
/// </summary>
/// <typeparam name="T">The type of messages handled by the channel broker.</typeparam>
public interface IChannelBroker<T>
{
    /// <summary>
    /// Writes a message to the channel asynchronously.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the write operation result.</returns>
    Task<bool> WriteAsync(T message, CancellationToken cancellationToken);

    /// <summary>
    /// Reads all messages from the channel as an async enumerable.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>An async enumerable of messages.</returns>
    IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken);
}
