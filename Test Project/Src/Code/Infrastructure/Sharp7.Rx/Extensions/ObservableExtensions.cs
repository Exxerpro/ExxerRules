// <copyright file="ObservableExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Extensions;

using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides extension methods for <see cref="IObservable{T}"/>.
/// </summary>
internal static class ObservableExtensions
{
    /// <summary>
    /// Disposes of each item emitted by the source observable if it implements <see cref="IDisposable"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <param name="source">The source observable sequence.</param>
    /// <returns>An observable sequence that contains the elements of the source sequence, but disposes each element if it is disposable.</returns>
    public static IObservable<T> DisposeMany<T>(this IObservable<T> source)
    {
        return Observable.Create<T>(obs =>
        {
            var serialDisposable = new SerialDisposable();
            var subscription =
                source.Subscribe(
                    item =>
                    {
                        serialDisposable.Disposable = item as IDisposable;
                        obs.OnNext(item);
                    },
                    obs.OnError,
                    obs.OnCompleted);
            return new CompositeDisposable(serialDisposable, subscription);
        });
    }

    /// <summary>
    /// Logs any errors and retries the observable sequence indefinitely.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <param name="source">The source observable sequence.</param>
    /// <param name="logger">The logger instance to use for logging errors.</param>
    /// <param name="message">The message to log along with the error.</param>
    /// <returns>An observable sequence that retries indefinitely upon error.</returns>
    public static IObservable<T> LogAndRetry<T>(this IObservable<T> source, ILogger logger, string message)
    {
        return source
            .Do(
                _ => { },
                ex => logger?.LogError(ex, message))
            .Retry();
    }

    /// <summary>
    /// Logs any errors and retries the observable sequence after a specified delay.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <param name="source">The source observable sequence.</param>
    /// <param name="logger">The logger instance to use for logging errors.</param>
    /// <param name="retryDelay">The time span to wait before retrying.</param>
    /// <param name="message">The message to log along with the error.</param>
    /// <param name="retryCount">The maximum number of times to retry. Use -1 for indefinite retries.</param>
    /// <param name="scheduler">The scheduler to use for delaying retries. If null, <see cref="TaskPoolScheduler.Default"/> is used.</param>
    /// <returns>An observable sequence that retries after a delay upon error.</returns>
    public static IObservable<T> LogAndRetryAfterDelay<T>(
        this IObservable<T> source,
        ILogger logger,
        TimeSpan retryDelay,
        string message,
        int retryCount = -1,
        IScheduler scheduler = null)
    {
        var sourceLogged =
            source
                .Do(
                    _ => { },
                    ex => logger?.LogError(ex, message));

        return RetryAfterDelay(sourceLogged, retryDelay, retryCount, scheduler);
    }

    /// <summary>
    /// Repeats the observable sequence after a specified delay.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <param name="source">The source observable sequence.</param>
    /// <param name="retryDelay">The time span to wait before repeating.</param>
    /// <param name="repeatCount">The maximum number of times to repeat. Use -1 for indefinite repeats.</param>
    /// <param name="scheduler">The scheduler to use for delaying repeats. If null, <see cref="TaskPoolScheduler.Default"/> is used.</param>
    /// <returns>An observable sequence that repeats after a delay.</returns>
    public static IObservable<T> RepeatAfterDelay<T>(
        this IObservable<T> source,
        TimeSpan retryDelay,
        int repeatCount = -1,
        IScheduler scheduler = null)
    {
        return RedoAfterDelay(source, retryDelay, repeatCount, scheduler, Observable.Repeat, Observable.Repeat);
    }

    /// <summary>
    /// Retries the observable sequence after a specified delay.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source observable sequence.</typeparam>
    /// <param name="source">The source observable sequence.</param>
    /// <param name="retryDelay">The time span to wait before retrying.</param>
    /// <param name="retryCount">The maximum number of times to retry. Use -1 for indefinite retries.</param>
    /// <param name="scheduler">The scheduler to use for delaying retries. If null, <see cref="TaskPoolScheduler.Default"/> is used.</param>
    /// <returns>An observable sequence that retries after a delay.</returns>
    public static IObservable<T> RetryAfterDelay<T>(
        this IObservable<T> source,
        TimeSpan retryDelay,
        int retryCount = -1,
        IScheduler scheduler = null)
    {
        return RedoAfterDelay(source, retryDelay, retryCount, scheduler, Observable.Retry, Observable.Retry);
    }

    private static IObservable<T> RedoAfterDelay<T>(IObservable<T> source, TimeSpan retryDelay, int retryCount, IScheduler scheduler, Func<IObservable<T>, IObservable<T>> reDo,
        Func<IObservable<T>, int, IObservable<T>> reDoCount)
    {
        scheduler = scheduler ?? TaskPoolScheduler.Default;
        var attempt = 0;

        var deferedObs = Observable.Defer(() => ((++attempt == 1) ? source : source.DelaySubscription(retryDelay, scheduler)));
        return retryCount > 0 ? reDoCount(deferedObs, retryCount) : reDo(deferedObs);
    }
}
