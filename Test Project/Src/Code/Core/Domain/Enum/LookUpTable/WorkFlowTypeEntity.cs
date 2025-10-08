// <copyright file="WorkFlowTypeEntity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum.LookUpTable;

using IndTrace.Domain.Enum.Attributes;

/// <summary>
/// Represents the WorkFlowTypeEntity.
/// </summary>
[EnumLookup]
public class WorkFlowTypeEntity : EnumLookUpTable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkFlowTypeEntity"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="displayName">The displayName.</param>
    public WorkFlowTypeEntity(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkFlowTypeEntity"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public WorkFlowTypeEntity()
    {
    }
}
