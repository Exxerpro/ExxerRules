// <copyright file="CreateConfigAppCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigStations.Commands.Create;

/// <summary>
/// Represents the CreateConfigAppCommand.
/// </summary>
public class CreateConfigStationCommand : IMonitorRequest<ConfigStationCreated>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateConfigStationCommand"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public CreateConfigStationCommand()
    {
        this.ConfigId = string.Empty;
        this.Client = string.Empty;
        this.Factorie = string.Empty;
        this.Line = string.Empty;
        this.Machine = string.Empty;
        this.Project = string.Empty;
        this.Version = string.Empty;
        this.Pc = 1;
    }

    /// <summary>
    /// Gets or sets the Pc.
    /// </summary>
    public int Pc { get; set; }

    /// <summary>
    /// Gets or sets the ConfigId.
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the Client.
    /// </summary>
    public string Client { get; set; }

    /// <summary>
    /// Gets or sets the Factorie.
    /// </summary>
    public string Factorie { get; set; }

    /// <summary>
    /// Gets or sets the Line.
    /// </summary>
    public string Line { get; set; }

    /// <summary>
    /// Gets or sets the Machine.
    /// </summary>
    public string Machine { get; set; }

    /// <summary>
    /// Gets or sets the Project.
    /// </summary>
    public string Project { get; set; }

    /// <summary>
    /// Gets or sets the Version.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the VersionDate.
    /// </summary>
    public DateTime VersionDate { get; set; }

    /// <summary>
    /// Gets or sets the ModifiedDate.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public int PlcId { get; internal set; }
}
