// <copyright file="PlcConnectionSettings.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Settings;

/// <summary>
/// Represents the connection settings for a PLC.
/// </summary>
public class PlcConnectionSettings
{
    /// <summary>
    /// Gets or sets the CPU MPI address.
    /// </summary>
    public int CpuMpiAddress { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the PLC.
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the port number for the PLC connection.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the rack number of the PLC.
    /// </summary>
    public int RackNumber { get; set; }
}
