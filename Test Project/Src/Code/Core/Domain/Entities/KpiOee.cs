// <copyright file="KpiOee.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents OEE (Overall Equipment Effectiveness) KPI values and their association with an OEE register.
/// </summary>
public class KpiOee : IEntityRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KpiOee"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public KpiOee()
    {
        this.OeeRegister = null!;
    }

    /// <summary>
    /// Returns a string representation of the KPI OEE entry.
    /// </summary>
    /// <returns>A string containing the KPI OEE ID, OEE percentage, and timestamp.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"KPI {this.KpiOeeId}: OEE {this.Oee:P1} at {this.TimeStamp:yyyy-MM-dd HH:mm}";

    /// <summary>
    /// Gets or sets the unique identifier for the KPI OEE entry.
    /// </summary>
    public int KpiOeeId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated OEE register.
    /// </summary>
    public int OeeRegisterId { get; set; }

    /// <summary>
    /// Gets or sets the OEE value.
    /// </summary>
    public double Oee { get; set; }

    /// <summary>
    /// Gets or sets the availability value.
    /// </summary>
    public double Availability { get; set; }

    /// <summary>
    /// Gets or sets the performance value.
    /// </summary>
    public double Performance { get; set; }

    /// <summary>
    /// Gets or sets the quality value.
    /// </summary>
    public double Quality { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the KPI OEE entry.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the associated OEE register entity.
    /// </summary>
    public OeeRegister OeeRegister { get; set; }
}
