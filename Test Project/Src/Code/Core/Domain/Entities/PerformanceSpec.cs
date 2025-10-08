// <copyright file="PerformanceSpec.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a performance specification lookup entity for production targets and metrics.
/// </summary>
public class PerformanceSpec : ILookupEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the performance specification.
    /// </summary>
    public int Id { get; set; }
}
