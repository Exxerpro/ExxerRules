// <copyright file="IIndTraceTagRx.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.S7Rx.Interfaces;

/// <summary>
/// Defines a contract for reactive tag operations in the IndTrace S7 communication system.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IIndTraceTagRx logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IIndTraceTagRx
{
    /// <summary>
    /// Gets or sets the .NET type associated with the tag.
    /// </summary>
    Type NetType { get; set; }

    /// <summary>
    /// Gets the variable entity associated with this tag.
    /// </summary>
    Variable Variable { get; }

    /// <summary>
    /// Downloads the current value from the PLC controller asynchronously.
    /// </summary>
    /// <param name="controller">The PLC controller to download from.</param>
    /// <returns>A task representing the asynchronous operation, returning true if successful.</returns>
    Task<bool> DownloadValueAsync(IPlc controller);

    /// <summary>
    /// Uploads the current value to the PLC controller asynchronously.
    /// </summary>
    /// <param name="controller">The PLC controller to upload to.</param>
    /// <returns>A task representing the asynchronous operation, returning the uploaded value as a string.</returns>
    Task<string> UploadValueAsync(IPlc controller);

    /// <summary>
    /// Gets or sets the current value of the tag.
    /// </summary>
    object Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether simulation mode is enabled for this tag.
    /// </summary>
    bool EnableSimulation { get; set; }
}
