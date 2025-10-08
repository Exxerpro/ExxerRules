// <copyright file="DisposableExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Extensions;

using System.Reactive.Disposables;

/// <summary>
/// Provides extension methods for <see cref="IDisposable"/> objects.
/// </summary>
internal static class DisposableExtensions
{
    /// <summary>
    /// Adds a disposable to a <see cref="CompositeDisposable"/>.
    /// </summary>
    /// <param name="disposable">The disposable to add.</param>
    /// <param name="compositeDisposable">The composite disposable to add to.</param>
    public static void AddDisposableTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
    {
        compositeDisposable.Add(disposable);
    }
}
