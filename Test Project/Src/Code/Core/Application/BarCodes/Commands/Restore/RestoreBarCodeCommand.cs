// <copyright file="RestoreBarCodeCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Restore;

/// <summary>
/// Represents the RestoreBarCodeCommand.
/// </summary>
public class RestoreBarCodeCommand : IMonitorRequest<BarCodeRestoredView>
{
    /// <summary>
    /// Gets or sets the Label.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RestoreBarCodeCommand"/> class.
    /// </summary>
    public RestoreBarCodeCommand()
    {
        this.Label = string.Empty;
    }
}
