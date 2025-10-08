// <copyright file="WorkFlowType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum;

/// <summary>
/// Represents the type of workflow in the system, such as Initial, Serial, Lateral, Diverter, Merger, and Final.
/// </summary>
public class WorkFlowType : EnumModel
{
    /// <summary>
    /// Represents an invalid workflow type.
    /// </summary>
    public static new readonly WorkFlowType Invalid
        = new(-1, "Invalid Value");

    /// <summary>
    /// Represents no workflow type.
    /// </summary>
    public static readonly WorkFlowType None
        = new(0, "None");

    /// <summary>
    /// Represents an initial workflow type.
    /// </summary>
    public static readonly WorkFlowType Initial
        = new(1, "Initial");

    /// <summary>
    /// Represents a serial (process) workflow type.
    /// </summary>
    public static readonly WorkFlowType Serial
        = new(2, "Serial");

    /// <summary>
    /// Represents a lateral workflow type.
    /// </summary>
    public static readonly WorkFlowType Lateral
        = new(4, "Lateral");

    /// <summary>
    /// Represents a diverter workflow type.
    /// </summary>
    public static readonly WorkFlowType Diverter
        = new(8, "Diverter");

    /// <summary>
    /// Represents a merger workflow type.
    /// </summary>
    public static readonly WorkFlowType Merger
        = new(16, "Merger");

    /// <summary>
    /// Represents a final workflow type.
    /// </summary>
    public static readonly WorkFlowType Final
        = new(32, "Final");

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkFlowType"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public WorkFlowType()
    {
    }

    private WorkFlowType(int value, string name, string? displayName = null)
        : base(value, name, displayName ?? string.Empty)
    {
    }

    public static implicit operator int(WorkFlowType enumerator) => enumerator.Value;

    public static implicit operator string(WorkFlowType enumerator) => enumerator.Value.ToString();

    public static implicit operator WorkFlowType(int value) => FromValue<WorkFlowType>(value);
}
