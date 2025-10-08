// <copyright file="PlcExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Extensions;

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;

/// <summary>
/// Provides extension methods for <see cref="IPlc"/> to simplify common PLC operations.
/// </summary>
public static class PlcExtensions
{
    /// <summary>
    /// Creates an observable that performs data transfer with a handshake mechanism.
    /// </summary>
    /// <typeparam name="TReturn">The type of data to be returned.</typeparam>
    /// <param name="plc">The PLC instance.</param>
    /// <param name="triggerAddress">The address of the PLC tag that triggers the data transfer.</param>
    /// <param name="ackTriggerAddress">The address of the PLC tag used to acknowledge the data transfer.</param>
    /// <param name="readData">A function that reads data from the PLC asynchronously.</param>
    /// <param name="initialTransfer">A boolean indicating whether to perform an initial data transfer upon connection.</param>
    /// <returns>An observable sequence that emits the transferred data.</returns>
    public static IObservable<TReturn> CreateDatatransferWithHandshake<TReturn>(this IPlc plc, string triggerAddress, string ackTriggerAddress, Func<IPlc, Task<TReturn>> readData,
        bool initialTransfer)
    {
        return Observable.Create<TReturn>(async observer =>
        {
            var subscriptions = new CompositeDisposable();

            var notification = plc
                .CreateNotification<bool>(triggerAddress, TransmissionMode.OnChange)
                .Publish()
                .RefCount();

            if (initialTransfer)
            {
                await plc.ConnectionState.FirstAsync(state => state == ConnectionState.Connected).ToTask();
                var initialValue = await ReadData(plc, readData);
                observer.OnNext(initialValue);
            }

            notification
                .Where(trigger => trigger)
                .SelectMany(_ => ReadDataAndAcknowlodge(plc, readData, ackTriggerAddress))
                .Subscribe(observer)
                .AddDisposableTo(subscriptions);

            notification
                .Where(trigger => !trigger)
                .SelectMany(async _ =>
                {
                    await plc.SetValue(ackTriggerAddress, false);
                    return Unit.Default;
                })
                .Subscribe()
                .AddDisposableTo(subscriptions);

            return subscriptions;
        });
    }

    /// <summary>
    /// Creates an observable that performs data transfer with a handshake mechanism, without an initial transfer.
    /// </summary>
    /// <typeparam name="TReturn">The type of data to be returned.</typeparam>
    /// <param name="plc">The PLC instance.</param>
    /// <param name="triggerAddress">The address of the PLC tag that triggers the data transfer.</param>
    /// <param name="ackTriggerAddress">The address of the PLC tag used to acknowledge the data transfer.</param>
    /// <param name="readData">A function that reads data from the PLC asynchronously.</param>
    /// <returns>An observable sequence that emits the transferred data.</returns>
    public static IObservable<TReturn> CreateDatatransferWithHandshake<TReturn>(this IPlc plc, string triggerAddress, string ackTriggerAddress, Func<IPlc, Task<TReturn>> readData)
    {
        return CreateDatatransferWithHandshake(plc, triggerAddress, ackTriggerAddress, readData, false);
    }

    private static async Task<TReturn> ReadData<TReturn>(IPlc plc, Func<IPlc, Task<TReturn>> receiveData)
    {
        return await receiveData(plc);
    }

    private static async Task<TReturn> ReadDataAndAcknowlodge<TReturn>(IPlc plc, Func<IPlc, Task<TReturn>> readData, string ackTriggerAddress)
    {
        try
        {
            return await ReadData(plc, readData);
        }
        finally
        {
            await plc.SetValue(ackTriggerAddress, true);
        }
    }
}
