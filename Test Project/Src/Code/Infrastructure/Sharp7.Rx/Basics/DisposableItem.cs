// <copyright file="DisposableItem.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Basics;

/// <summary>
/// Represents a disposable item that encapsulates an observable and an action to be performed upon disposal.
/// </summary>
/// <typeparam name="TValue">The type of the observable's value.</typeparam>
internal class DisposableItem<TValue> : IDisposable
{
    private readonly Action disposeAction;

    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableItem{TValue}"/> class.
    /// </summary>
    /// <param name="observable">The observable encapsulated by this item.</param>
    /// <param name="disposeAction">The action to perform when this item is disposed.</param>
    public DisposableItem(IObservable<TValue> observable, Action disposeAction)
    {
        this.disposeAction = disposeAction;
        this.Observable = observable;
    }

    /// <summary>
    /// Gets the observable encapsulated by this item.
    /// </summary>
    public IObservable<TValue> Observable { get; }

    /// <summary>
    /// Disposes the item, executing the encapsulated dispose action.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="DisposableItem{TValue}"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            this.disposeAction();
        }

        this.disposed = true;
    }
}
