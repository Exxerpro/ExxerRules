// <copyright file="FlowStatusEntity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum.LookUpTable;

using IndTrace.Domain.Enum.Attributes;
using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents the FlowStatusEntity.
/// </summary>
// [Fix]
// CLAUDE
// Date: 25/08/2025
// Reason: EF Core entity interface fix - FlowStatusEntity needs ILookupEntity for DbSet registration
[EnumLookup]
public class FlowStatusEntity : EnumLookUpTable, ILookupEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlowStatusEntity"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public FlowStatusEntity()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlowStatusEntity"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="displayName">The display name.</param>
    public FlowStatusEntity(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }
}
