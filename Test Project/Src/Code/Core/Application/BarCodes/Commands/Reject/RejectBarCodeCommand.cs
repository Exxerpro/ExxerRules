// <copyright file="RejectBarCodeCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Reject;

/// <summary>
/// Command to reject a barcode by label.
/// </summary>
public class RejectBarCodeCommand : IMonitorRequest<BarCodeRejectedView>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RejectBarCodeCommand"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public RejectBarCodeCommand()
    {
        this.Label = string.Empty;
    }

    /// <summary>
    /// Gets or sets the label of the barcode to reject.
    /// </summary>
    public string Label { get; set; }
}
