// <copyright file="UpdateConfigAppCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Commands.Update;

/// <summary>
/// Represents the UpdateConfigAppCommand.
/// </summary>
public class UpdateConfigAppCommand : IMonitorRequest<ConfigAppDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateConfigAppCommand"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public UpdateConfigAppCommand()
    {
        this.ConfigAppId = string.Empty;
        this.Client = string.Empty;
        this.Factory = string.Empty;
        this.Line = string.Empty;
        this.Project = string.Empty;
        this.Version = string.Empty;
    }

    /// <summary>
    /// Gets or sets the ConfigAppId.
    /// </summary>
    public string ConfigAppId { get; set; }

    /// <summary>
    /// Gets or sets the AppId.
    /// </summary>
    public int? AppId { get; set; }

    /// <summary>
    /// Gets or sets the Client.
    /// </summary>
    public string Client { get; set; }

    /// <summary>
    /// Gets or sets the Factory.
    /// </summary>
    public string Factory { get; set; }

    /// <summary>
    /// Gets or sets the Line.
    /// </summary>
    public string Line { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the Project.
    /// </summary>
    public string Project { get; set; }

    /// <summary>
    /// Gets or sets the Version.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the CreatedOn.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the ModifiedOn.
    /// </summary>
    public DateTime ModifiedOn { get; set; }
}
