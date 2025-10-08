// <copyright file="IEnumModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Enum;

/// <summary>
/// Represents a base interface for enumerations with value, name, and display name.
/// </summary>
public interface IEnumModel
{
    /// <summary>
    /// Gets or sets the integer value of the enumeration.
    /// </summary>
    int Value { get; set; }

    /// <summary>
    /// Gets or sets the name of the enumeration.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the display name of the enumeration.
    /// </summary>
    string DisplayName { get; set; }

    /// <summary>
    /// Gets the invalid enumeration instance.
    /// </summary>
    public IEnumModel Invalid { get; }
}
