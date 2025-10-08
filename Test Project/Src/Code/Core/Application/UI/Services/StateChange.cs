// <copyright file="StateChange.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.UI.Services;

/// <summary>
/// Represents a state change event containing information about property changes and old/new state values.
/// </summary>
public class StateChange
{
    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string PropertyName { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public object State { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public object OldState { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public object Data { get; set; } = null!;

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate state change logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
