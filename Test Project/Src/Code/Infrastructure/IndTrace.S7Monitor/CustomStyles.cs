// <copyright file="CustomStyles.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Provides custom styling options for console output formatting using Spectre.Console.
/// </summary>
public static class CustomStyles
{
    /// <summary>
    /// Gets the default style with a black background.
    /// </summary>
    public static Style Default { get; } = new(background: Color.Black);

    /// <summary>
    /// Gets the data style with a dark slate gray foreground for displaying data values.
    /// </summary>
    public static Style Data { get; } = Default.Foreground(Color.DarkSlateGray2);

    /// <summary>
    /// Gets the error style with a red foreground for displaying error messages.
    /// </summary>
    public static Style Error { get; } = Default.Foreground(Color.Red);

    /// <summary>
    /// Gets the hex style with a light goldenrod foreground for displaying hexadecimal values.
    /// </summary>
    public static Style Hex { get; } = Default.Foreground(Color.LightGoldenrod2_1);

    /// <summary>
    /// Gets the note style with a dark slate gray foreground for displaying notes and annotations.
    /// </summary>
    public static Style Note { get; } = Default.Foreground(Color.DarkSlateGray1);

    /// <summary>
    /// Gets the table border style with a dark green foreground for displaying table borders.
    /// </summary>
    public static Style TableBorder { get; } = Default.Foreground(Color.DarkGreen);
}
