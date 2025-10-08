// <copyright file="CycleStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum;

/// <summary>
/// Represents the status of a cycle in the system, such as NotStarted, Started, FinishedOk, FinishedNok, etc.
/// </summary>
public class CycleStatus : EnumModel
{
    /// <summary>
    /// Represents an invalid cycle status.
    /// </summary>
    public static new readonly CycleStatus Invalid =
        new(-1, "Invalid Value");

    /// <summary>
    /// Represents no cycle status.
    /// </summary>
    public static readonly CycleStatus None
        = new(0, "None");

    /// <summary>
    /// Represents a cycle that has not started.
    /// </summary>
    public static readonly CycleStatus NotStarted
        = new(1, "NotStarted");

    /// <summary>
    /// Represents a cycle that has started.
    /// </summary>
    public static readonly CycleStatus Started
        = new(2, "Started");

    /// <summary>
    /// Represents a cycle that finished successfully.
    /// </summary>
    public static readonly CycleStatus FinishedOk
        = new(4, "FinishedOk");

    /// <summary>
    /// Represents a cycle that finished with a failure.
    /// </summary>
    public static readonly CycleStatus FinishedNok
        = new(8, "FinishedNok");

    /// <summary>
    /// Represents the end of a process cycle.
    /// </summary>
    public static readonly CycleStatus EndOfProcess
        = new(16, "EndOfProcess");

    /// <summary>
    /// Represents a cycle that was rejected.
    /// </summary>
    public static readonly CycleStatus Rejected
        = new(32, "Rejected");

    /// <summary>
    /// Represents a cycle that was canceled.
    /// </summary>
    public static readonly CycleStatus Canceled
        = new(64, "Canceled");

    /// <summary>
    /// Initializes a new instance of the <see cref="CycleStatus"/> class.
    /// </summary>
    public CycleStatus()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CycleStatus"/> class with specified values.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <param name="name">The name.</param>
    /// <param name="displayName">The display name.</param>
    private CycleStatus(int value, string name, string? displayName = null)
        : base(value, name, displayName ?? string.Empty)
    {
    }

    /// <summary>
    /// Implicitly converts a CycleStatus to its string representation.
    /// </summary>
    /// <param name="enumerator">The enumerator to convert.</param>
    public static implicit operator string(CycleStatus enumerator) => enumerator.Value.ToString();

    /// <summary>
    /// Implicitly converts an integer value to a CycleStatus.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator CycleStatus(int value) => FromValue<CycleStatus>(value);

    /// <summary>
    /// Implicitly converts a nullable integer value to a CycleStatus.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator CycleStatus(int? value) => FromValue<CycleStatus>(value ?? 0);

    /// <summary>
    /// Retrieves a <see cref="CycleStatus"/> instance from an integer value.
    /// </summary>
    /// <param name="value">The integer value representing the status.</param>
    /// <returns>A <see cref="CycleStatus"/> instance corresponding to the specified value.</returns>
    public static CycleStatus FromValue(int value) => FromValue<CycleStatus>(value);
}
