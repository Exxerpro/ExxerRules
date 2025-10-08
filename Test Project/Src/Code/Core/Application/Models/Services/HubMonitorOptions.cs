// <copyright file="HubMonitorOptions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Services;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents options for configuring the Hub monitor connection.
/// </summary>
public class HubMonitorOptions
{
    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    [Required]
    public string Url { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether to accept any server certificate.
    /// </summary>
    public bool AcceptAnyServerCertificate { get; set; }

    /// <summary>
    /// Gets or sets the retry time in seconds.
    /// </summary>
    [Range(1, 6000)]
    public int RetryTime { get; set; }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate options and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
