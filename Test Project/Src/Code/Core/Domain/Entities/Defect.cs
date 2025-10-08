// <copyright file="Defect.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a defect lookup entity for categorizing production quality issues.
/// </summary>
public class Defect : ILookupEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Defect"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public Defect()
    {
        this.Name = string.Empty;
        this.Description = string.Empty;
        this.ShortName = string.Empty;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the defect.
    /// </summary>
    public int DefectId { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the defect type.
    /// </summary>
    public int DefectTypeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the defect.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the defect.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the short name of the defect.
    /// </summary>
    public string ShortName { get; set; }
}
