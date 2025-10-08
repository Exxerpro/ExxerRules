// <copyright file="TimeSeriesDataPoint.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Services;

/// <summary>
/// Represents the TimeSeriesDataPoint.
/// </summary>
public class TimeSeriesDataPoint
{
    private string name = string.Empty;
    private string value = string.Empty;
    private string valueType = string.Empty;

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the name of the data point. Null values are automatically converted to empty string for runtime safety.
    /// </summary>
    public string Name
    {
        get => this.name;
        set => this.name = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the value of the data point. Null values are automatically converted to empty string for runtime safety.
    /// </summary>
    public string Value
    {
        get => this.value;
        set => this.value = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the value type of the data point. Null values are automatically converted to empty string for runtime safety.
    /// </summary>
    public string ValueType
    {
        get => this.valueType;
        set => this.valueType = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the TimeStamp.
    /// </summary>
    public DateTime TimeStamp { get; set; }
}
