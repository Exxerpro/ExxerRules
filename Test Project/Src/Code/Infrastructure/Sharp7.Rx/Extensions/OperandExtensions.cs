// <copyright file="OperandExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Extensions;

using Sharp7.Rx.Enums;

/// <summary>
/// Provides extension methods for the Operand enum.
/// </summary>
internal static class OperandExtensions
{
    /// <summary>
    /// Converts an Operand to its corresponding S7Area.
    /// </summary>
    /// <param name="operand">The operand to convert.</param>
    /// <returns>The corresponding S7Area value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the operand is not recognized.</exception>
    public static S7Area ToArea(this Operand operand) =>
        operand switch
        {
            Operand.Input => S7Area.PE,
            Operand.Output => S7Area.PA,
            Operand.Marker => S7Area.MK,
            Operand.Db => S7Area.DB,
            _ => throw new ArgumentOutOfRangeException(nameof(operand), operand, null),
        };
}
