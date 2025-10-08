// <copyright file="CacheVariableNameParser.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx;

using System.Collections.Concurrent;
using Sharp7.Rx.Interfaces;

/// <summary>
/// A decorator for <see cref="IVariableNameParser"/> that caches parsed variable addresses.
/// </summary>
internal class CacheVariableNameParser : IVariableNameParser
{
    private static readonly ConcurrentDictionary<string, VariableAddress> AddressCache = new ConcurrentDictionary<string, VariableAddress>(StringComparer.OrdinalIgnoreCase);

    private readonly IVariableNameParser inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheVariableNameParser"/> class.
    /// </summary>
    /// <param name="inner">The inner variable name parser to decorate.</param>
    public CacheVariableNameParser(IVariableNameParser inner)
    {
        this.inner = inner;
    }

    /// <summary>
    /// Parses a variable name and returns its address, using a cache to store previously parsed addresses.
    /// </summary>
    /// <param name="input">The variable name to parse.</param>
    /// <returns>The parsed <see cref="VariableAddress"/>.</returns>
    public VariableAddress Parse(string input) => AddressCache.GetOrAdd(input, this.inner.Parse);
}
