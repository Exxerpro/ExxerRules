// <copyright file="GetAppDetailsGuiRequest.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Configuration.Services;

/// <summary>
/// Represents a request to get application details for the GUI, with an option to refresh.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate request logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the GetAppDetailsMonitorRequest.
/// </summary>
public class GetAppDetailsMonitorRequest(bool refresh = false) : IMonitorRequest<ApplicationConfiguration>
{
    /// <summary>
    /// Gets a value indicating whether to refresh the application details.
    /// </summary>
    public bool Refresh { get; } = refresh;
}
