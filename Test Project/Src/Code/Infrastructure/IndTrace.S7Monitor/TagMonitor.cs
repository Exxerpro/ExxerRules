// <copyright file="TagMonitor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Represents a tag associated with a PLC, containing an address and a description.
/// </summary>
public class TagMonitor
{
    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Description { get; set; } = null!;

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate tag logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
