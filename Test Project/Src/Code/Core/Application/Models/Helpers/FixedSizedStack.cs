// <copyright file="FixedSizedStack.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Helpers;

using System.Collections.Concurrent;

/// <summary>
/// Represents a fixed-size stack with FIFO behavior and thread safety.
/// </summary>
public class FixedSizedStack<T>(int size)
{
    private readonly ConcurrentQueue<T> queue = new(); // Use ConcurrentQueue for FIFO behavior
    private readonly object syncLock = new(); // Use an object for locking

    /// <summary>
    /// The default size of the stack.
    /// </summary>
    public const int DefaultSize = 80;

    /// <summary>
    /// Gets or sets the maximum number of items allowed in the stack.
    /// </summary>
    public int Limit { get; set; } = size;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedSizedStack{T}"/> class with the default size.
    /// </summary>
    public FixedSizedStack()
        : this(DefaultSize)
    {
    }

    /// <summary>
    /// Adds an item to the stack if it does not already exist.
    /// </summary>
    /// <param name="item">The item to add.</param>


    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Using .Contains on ConcurrentQueue in Push may be O(n) and inefficient for large collections. Consider using a HashSet for faster lookups if performance is a concern.
    /// <summary>
    /// Executes Push operation.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Push(T item)
    {
        if (item is null)
        {
            return;
        }

        if (item is string str && string.IsNullOrEmpty(str))
        {
            return;
        }

        lock (this.syncLock) // Lock to ensure thread safety
        {
            // Check if the item already exists in the queue
            if (this.queue.Contains(item))
            {
                return; // Do not push the item if it already exists in the queue
            }

            this.queue.Enqueue(item); // Add the new item to the queue
            while (this.queue.Count > this.Limit) // If the limit is exceeded
            {
                this.queue.TryDequeue(out _); // Remove the oldest item
            }
        }
    }

    /// <summary>
    /// Returns the oldest item in the stack without removing it.
    /// </summary>
    /// <returns>The oldest item in the stack.</returns>
    public T? Peek()
    {
        if (this.queue.TryPeek(out T? result) && result is not null)
        {
            return result; // Return the oldest item without removing it
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Returns the items in the stack in LIFO order as an enumerable.
    /// </summary>
    /// <returns>An enumerable of items in LIFO order.</returns>
    public IEnumerable<T> ToEnumerable()
    {
        return this.queue.Reverse();
    }

    /// <summary>
    /// Converts the stack to an <see cref="IReadOnlyCollection{T}"/> in LIFO order.
    /// </summary>
    /// <returns>An <see cref="IReadOnlyCollection{T}"/> containing items in LIFO order.</returns>
    public IReadOnlyCollection<T> ToReadOnlyCollection()
    {
        return this.queue.Reverse().ToArray(); // Materialize the IEnumerable to an array, which implements IReadOnlyCollection
    }

    /// <summary>
    /// Gets the number of items in the stack.
    /// </summary>
    public int Count => this.queue.Count;

    /// <summary>
    /// Converts the stack to an array.
    /// </summary>
    /// <returns>An array containing the items in the stack.</returns>
    public T[] ToArray() => this.queue.ToArray();
}
