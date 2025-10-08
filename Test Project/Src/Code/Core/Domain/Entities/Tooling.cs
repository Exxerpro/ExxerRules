// <copyright file="Tooling.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

// https://www.globalspec.com/reference/69812/203279/chapter-8-tooling-die-making-molds-jigs-and-fixtures
// Tooling. Tooling is a general term used to encompass many different processes
// involving the design and manufacture of special tools, dies, molds, jigs, and fixtures.
// The most common type of tooling consists of dies that are used to stamp or
// blank sheet metal parts for mass production.
// The use of a stamping or blanking die makes it possible to produce thousands of parts with consistent dimensional accuracy at a rapid pace. This ensures that the die-produced parts will fit correctly into their next assembly stage, such as in a complex mechanical device or mechanism, and have interchangeability.

/// <summary>
/// Represents a tooling entity, including tool identification and name.
/// Tooling refers to the design and manufacture of special tools, dies, molds, jigs, and fixtures used in mass production.
/// </summary>
public class Tooling : ILookupEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tooling"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public Tooling()
    {
        this.Name = null!;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the tool.
    /// </summary>
    public int ToolId { get; set; }

    /// <summary>
    /// Gets or sets the code for the tool.
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Gets or sets the name of the tool.
    /// </summary>
    public string Name { get; set; }
}
