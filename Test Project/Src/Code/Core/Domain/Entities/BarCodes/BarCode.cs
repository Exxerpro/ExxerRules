// <copyright file="BarCode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities.BarCodes;

using IndTrace.Domain.Enum;
using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a barcode entity associated with a product and machine, including label and status information.
/// </summary>
public class BarCode : IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the barcode.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated machine.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the label value for the barcode.
    /// </summary>
    public virtual string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the part status for the barcode.
    /// </summary>
    public PartStatus PartStatus { get; set; } = PartStatus.None;

    /// <summary>
    /// Gets or sets the flow status for the barcode.
    /// </summary>
    public FlowStatus FlowStatus { get; set; } = FlowStatus.None;

    /// <summary>
    /// Gets or sets the creation date and time of the barcode entry.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the last modification date and time of the barcode entry.
    /// </summary>
    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// Returns the label of the barcode or an empty string if not set.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.Label;
}
