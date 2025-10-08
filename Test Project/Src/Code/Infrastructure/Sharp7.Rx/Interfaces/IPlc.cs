// <copyright file="IPlc.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Interfaces;

using Microsoft.Extensions.Logging;
using Sharp7.Rx.BatchRead;
using Sharp7.Rx.Enums;

/// <summary>
/// Represents an interface for interacting with a PLC (Programmable Logic Controller).
/// </summary>
public interface IPlc : IDisposable
{
    /// <summary>
    /// Asynchronously retrieves a batch of values from the PLC.
    /// </summary>
    /// <param name="tags">A collection of tuples, each containing the alias, type, and status value ID of a tag.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="PlcBatchReadResult"/>.</returns>
    public Task<IEnumerable<PlcBatchReadResult>> GetBatchValuesPlcAsync(
        IEnumerable<(string Alias, Type Type, int StatusValueId)> tags,
        CancellationToken token = default);

    /// <summary>
    /// Gets or sets the logger instance for the PLC.
    /// </summary>
    ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets the Sharp7 connector used for PLC communication.
    /// </summary>
    ISharp7Connector S7Connector { get; set; }

    /// <summary>
    /// Gets or sets the Sharp7 connector used for PLC communication (lowercase accessor for compatibility).
    /// </summary>
    ISharp7Connector s7Connector { get; set; }

    /// <summary>
    /// Gets an observable that provides the connection state of the PLC.
    /// </summary>
    IObservable<ConnectionState> ConnectionState { get; }

    /// <summary>
    /// Asynchronously sets the value of a PLC variable.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to set.</typeparam>
    /// <param name="variableName">The name of the PLC variable.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetValue<TValue>(string variableName, TValue value, CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets the value of a PLC variable.
    /// </summary>
    /// <typeparam name="TValue">The expected type of the value.</typeparam>
    /// <param name="variableName">The name of the PLC variable.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value of the PLC variable.</returns>
    Task<TValue> GetValue<TValue>(string variableName, CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets the value of a PLC variable as an object.
    /// The return type is automatically inferred from the variable name.
    /// </summary>
    /// <param name="variableName">The name of the PLC variable.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value of the PLC variable as an object.</returns>
    Task<object> GetValue(string variableName, CancellationToken token = default);

    /// <summary>
    /// Creates an observable that provides notifications for changes in a PLC variable.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to observe.</typeparam>
    /// <param name="variableName">The name of the PLC variable.</param>
    /// <param name="transmissionMode">The transmission mode for notifications (e.g., cyclic, on change).</param>
    /// <returns>An observable sequence that emits the value of the PLC variable when it changes or cyclically.</returns>
    IObservable<TValue> CreateNotification<TValue>(string variableName, TransmissionMode transmissionMode);

    /// <summary>
    /// Creates an observable that provides notifications for changes in a PLC variable as an object.
    /// The return type is automatically inferred from the variable name.
    /// </summary>
    /// <param name="variableName">The name of the PLC variable.</param>
    /// <param name="transmissionMode">The transmission mode for notifications (e.g., cyclic, on change).</param>
    /// <returns>An observable sequence that emits the value of the PLC variable as an object when it changes or cyclically.</returns>
    IObservable<object> CreateNotification(string variableName, TransmissionMode transmissionMode);

    /// <summary>
    /// Asynchronously reads a PLC variable using the S7 client.
    /// </summary>
    /// <typeparam name="TValue">The expected type of the value.</typeparam>
    /// <param name="variableName">The name of the PLC variable.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value of the PLC variable.</returns>
    Task<TValue> GetValueS7Client<TValue>(string variableName, CancellationToken token = default);

    /// <summary>
    /// Initializes the PLC connection and waits for the connection to be established.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InitializeConnection(CancellationToken token = default);
}
