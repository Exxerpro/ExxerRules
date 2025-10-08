// <copyright file="FileName.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.S7Rx.Helper;

/// <summary>
/// Represents the S7ClientLocker.
/// </summary>
public class S7ClientLocker(S7Client client)
{
    private readonly Lock @lock = new Lock();
    private readonly S7Client client = client ?? throw new ArgumentNullException(nameof(client));

    // Safe Sync Access
    public TResult ExecuteLocked<TResult>(Func<S7Client, TResult> operation)
    {
        lock (this.@lock)
        {
            return operation(this.client);
        }
    }

    // Safe Async Access
    public Task<TResult> ExecuteLockedAsync<TResult>(Func<S7Client, Task<TResult>> operation)
    {
        lock (this.@lock)
        {
            return operation(this.client);
        }
    }

    // Safe Sync Action (void)

    /// <summary>
    /// Executes ExecuteLocked operation.
    /// </summary>
    /// <param name="operation">The operation.</param>
    public void ExecuteLocked(Action<S7Client> operation)
    {
        lock (this.@lock)
        {
            operation(this.client);
        }
    }
}
