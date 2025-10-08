// <copyright file="EnumToString.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum;

/// <summary>
/// Provides extension methods for converting enumeration values to strings.
/// </summary>
public static partial class EnumToString
{
    /// <summary>
    /// Converts an integer value to a CycleStatus enumeration.
    /// </summary>
    /// <param name="enumeration">The enumeration instance.</param>
    /// <param name="value">The integer value to convert.</param>
    /// <returns>The corresponding CycleStatus enumeration.</returns>
    public static CycleStatus ConvertEnumToString(this CycleStatus enumeration, int value)
    {
        return value;
    }

    /// <summary>
    /// Converts an integer value to a FlowStatus enumeration.
    /// </summary>
    /// <param name="enumeration">The enumeration instance.</param>
    /// <param name="value">The integer value to convert.</param>
    /// <returns>The corresponding FlowStatus enumeration.</returns>
    public static FlowStatus ConvertEnumToString(this FlowStatus enumeration, int value)
    {
        return value;
    }

    /// <summary>
    /// Converts an integer value to a PartStatus enumeration.
    /// </summary>
    /// <param name="enumeration">The enumeration instance.</param>
    /// <param name="value">The integer value to convert.</param>
    /// <returns>The corresponding PartStatus enumeration.</returns>
    public static PartStatus ConvertEnumToString(this PartStatus enumeration, int value)
    {
        return value;
    }
}
