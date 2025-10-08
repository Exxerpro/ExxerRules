// <copyright file="StringHelper.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Provides helper methods for string operations.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Determines whether the specified string is a valid IPv4 address.
    /// </summary>
    /// <param name="ipString">The string to validate as an IPv4 address.</param>
    /// <returns><c>true</c> if the string is a valid IPv4 address; otherwise, <c>false</c>.</returns>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate string helper logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static bool IsValidIp4(string? ipString)
    {
        if (string.IsNullOrWhiteSpace(ipString))
        {
            return false;
        }

        var splitValues = ipString.Split('.');
        return splitValues.Length == 4 && splitValues.All(r => byte.TryParse(r, out _));
    }
}
