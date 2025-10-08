// <copyright file="ToolingEnum.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

/// <summary>
/// Represents the types of tooling used in manufacturing or production environments.
/// </summary>
public enum ToolingEnum
{
    /// <summary>
    /// Represents general tools.
    /// </summary>
    Tools,

    /// <summary>
    /// Represents dies used in manufacturing.
    /// </summary>
    Dies,

    /// <summary>
    /// Represents molds used for shaping materials.
    /// </summary>
    Molds,

    /// <summary>
    /// Represents jigs used for guiding tools or workpieces.
    /// </summary>
    Jigs,

    /// <summary>
    /// Represents fixtures used to hold workpieces in place.
    /// </summary>
    Fixtures,
}
