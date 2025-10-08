// <copyright file="PartStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum;

/// <summary>
/// Represents the status of a part in the system.
/// </summary>
public class PartStatus : EnumModel
{
    /// <summary>
    /// Gets the invalid part status.
    /// </summary>
    public static new readonly PartStatus Invalid
        = new(-1, "Invalid Value");

    /// <summary>
    /// Gets the none part status.
    /// </summary>
    public static readonly PartStatus None
        = new(0, "None");

    /// <summary>
    /// Gets the OK part status.
    /// </summary>
    public static readonly PartStatus Ok
        = new(1, "Ok");

    /// <summary>
    /// Gets the NOK part status.
    /// </summary>
    public static readonly PartStatus NOk
        = new(2, "nOK");

    /// <summary>
    /// Gets the restored part status.
    /// </summary>
    public static readonly PartStatus Restored
        = new(4, "Restored");

    /// <summary>
    /// Gets the rejected part status.
    /// </summary>
    public static readonly PartStatus Rejected
        = new(8, "Rejected");

    /// <summary>
    /// Gets the scrap part status.
    /// </summary>
    public static readonly PartStatus Scrap
        = new(512, "Scrap");

    public static implicit operator int(PartStatus enumerator) => enumerator.Value;

    public static implicit operator string(PartStatus enumerator) => enumerator.Value.ToString();

    public static implicit operator PartStatus(int value) => FromValue<PartStatus>(value);

    /// <summary>
    /// Initializes a new instance of the <see cref="PartStatus"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public PartStatus()
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether this status has a value.
    /// </summary>
    public bool HasValue { get; set; }

    /// <summary>
    /// Retrieves a <see cref="PartStatus"/> instance from an integer value.
    /// </summary>
    /// <param name="value">The integer value representing the status.</param>
    /// <returns>A <see cref="PartStatus"/> instance corresponding to the specified value.</returns>
    public static PartStatus FromValue(int value) => FromValue<PartStatus>(value);

    private PartStatus(int value, string name, string displayName = "")
        : base(value, name, displayName)
    {
    }
}
