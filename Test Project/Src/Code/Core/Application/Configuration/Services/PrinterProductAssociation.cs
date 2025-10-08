// <copyright file="PrinterProductAssociation.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Configuration.Services;

/// <summary>
/// Represents an association between a printer and a product, including machine and product details.
/// </summary>
public class PrinterProductAssociation
{
    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the name of the machine.
    /// </summary>
    public string MachineName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the machine.
    /// </summary>
    public MachineType MachineType { get; set; } = MachineType.None;

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the part number of the product.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate printer-product association logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
