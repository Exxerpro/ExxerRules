// <copyright file="RenderUtil.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Provides utility methods for formatting and rendering data values for console display.
/// </summary>
internal static class RenderUtil
{
    /// <summary>
    /// Formats the given value as an <see cref="IRenderable"/> for display in a table cell.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>An <see cref="IRenderable"/> representing the formatted value.</returns>
    public static IRenderable FormatCellData(object value)
    {
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate render utility logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
        return value switch
        {
            IRenderable renderable => renderable,
            Exception ex => new Text(ex.Message, CustomStyles.Error),
            byte[] byteArray => FormatByteArray(byteArray),
            byte => FormatNo(),
            short => FormatNo(),
            ushort => FormatNo(),
            int => FormatNo(),
            uint => FormatNo(),
            long => FormatNo(),
            ulong => FormatNo(),

            _ => new Text(value.ToString() ?? string.Empty, CustomStyles.Default),
        };

        IRenderable FormatNo() => new Paragraph()
            .Append($"0x{value:X2}", CustomStyles.Hex)
            .Append("   ")
            .Append($"{value}", CustomStyles.Default)
        ;

        IRenderable FormatByteArray(byte[] byteArray) =>
            new Paragraph()
                .Append("0x " + string.Join(" ", byteArray.Select(b => $"{b:X2}")), CustomStyles.Hex)
                .Append(Environment.NewLine)
                .Append(new string(
                            byteArray
                                .Select(b => (char)b)
                                .Select(c => char.IsControl(c) ? '·' : c)
                                .ToArray()));
    }
}
