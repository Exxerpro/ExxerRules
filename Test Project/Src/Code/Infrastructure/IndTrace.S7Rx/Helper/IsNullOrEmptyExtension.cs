// <copyright file="IsNullOrEmptyExtension.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.S7Rx.Helper;

/// <summary>
/// Provides extension methods to check if collections are null or empty.
/// </summary>
public static class IsNullOrEmptyExtension
{
    /// <summary>
    /// Determines whether the specified non-generic enumerable is null or contains no elements.
    /// </summary>
    /// <param name="source">The enumerable to check.</param>
    /// <returns>True if the enumerable is null or empty; otherwise, false.</returns>
    public static bool IsNullOrEmpty(this IEnumerable source)
    {
        if (source == null)
        {
            return true;
        }

        return !source.Cast<object>().Any();
    }

    /// <summary>
    /// Determines whether the specified generic enumerable is null or contains no elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="source">The enumerable to check.</param>
    /// <returns>True if the enumerable is null or empty; otherwise, false.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        if (source == null)
        {
            return true;
        }

        return !source.Any();
    }
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IsNullOrEmptyExtension logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
