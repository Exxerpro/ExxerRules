// <copyright file="UpdatePlcCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Plcs.Commands.Update;

/// <summary>
/// Represents the UpdatePlcCommand.
/// </summary>
public class UpdatePlcCommand : IMonitorRequest<PlcDto>
{
#nullable enable
    /// <summary>
    /// Gets or sets the PlcId.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the IpAddress.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the PlcType.
    /// </summary>
    public string PlcType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the PlcBrand.
    /// </summary>
    public string PlcBrand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the CommLibrary.
    /// </summary>
    public string CommLibrary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the BrandOwner.
    /// </summary>
    public string BrandOwner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the EnableSimulation.
    /// </summary>
    public bool EnableSimulation { get; set; }
}
