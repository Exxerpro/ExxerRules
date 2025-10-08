// <copyright file="StateStatusEnum.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

/// <summary>
/// Represents the status states for fixture operations in the simulator.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate state status enum logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public enum StateStatusEnum
{
    /// <summary>
    /// The operation has been created but not yet started.
    /// </summary>
    Created,

    /// <summary>
    /// The operation has been started and is currently running.
    /// </summary>
    Started,

    /// <summary>
    /// The operation has not been started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// The operation has finished successfully.
    /// </summary>
    FinishedOk,

    /// <summary>
    /// The operation has finished with errors or failures.
    /// </summary>
    FinishedNotOk,
}
