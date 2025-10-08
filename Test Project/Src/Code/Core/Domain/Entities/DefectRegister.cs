// <copyright file="DefectRegister.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a record of a defect occurrence in the production process, including related barcode, machine, and defect details.
/// </summary>
public class DefectRegister : IEntityRoot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefectRegister"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public DefectRegister()
    {
        this.Description = string.Empty;
        this.Comment = string.Empty;
        this.TimeStamp = [];
    }

    /// <summary>
    /// Returns a string representation of the defect register.
    /// </summary>
    /// <returns>A string containing the defect register ID and description.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"Defect {this.DefectRegisterId}: {this.Description}";

    /// <summary>
    /// Gets or sets the unique identifier for the defect register entry.
    /// </summary>
    public int DefectRegisterId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated barcode.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the machine where the defect was registered.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the defect type.
    /// </summary>
    public int DefectId { get; set; }

    /// <summary>
    /// Gets or sets the description of the defect.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets additional comments about the defect.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of when the defect was registered.
    /// </summary>
    public byte[] TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time of the defect register entry.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the last modification date and time of the defect register entry.
    /// </summary>
    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// Gets or sets the quantity of parts affected by the defect.
    /// </summary>
    public decimal PartsQuantity { get; set; }

    // public virtual BarCodes BarCode { get; set; }
    // public virtual Defectos Defect { get; set; }
    // public virtual Maquinas Maquina { get; set; }
}
