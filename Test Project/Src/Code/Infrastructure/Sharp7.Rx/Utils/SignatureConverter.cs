// <copyright file="SignatureConverter.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Utils;

using System.Reactive.Linq;
using System.Reflection;

internal static class SignatureConverter
{
    private static readonly MethodInfo ConvertToObjectObservableMethod =
        typeof(SignatureConverter)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == nameof(ConvertToObjectObservable) && m.GetGenericArguments().Length == 1);

    public static IObservable<object> ConvertToObjectObservable<T>(IObservable<T> obs) => obs.Select(o => (object)o);

    public static IObservable<object> ConvertToObjectObservable(object observable, Type sourceType)
    {
        var convertGeneric = ConvertToObjectObservableMethod.MakeGenericMethod(sourceType);

        return convertGeneric.Invoke(null, [observable]) as IObservable<object>;
    }
}
