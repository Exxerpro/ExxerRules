// <copyright file="ResultValidationEnum.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models.Constants;

/// <summary>
/// Represents the validation result states for operations in the simulator.
/// </summary>
public enum ResultValidationEnum
{
    /// <summary>
    /// The operation is invalid.
    /// </summary>
    Invalid = -1,

    /// <summary>
    /// The operation is valid.
    /// </summary>
    Valid = 1,

    /// <summary>
    /// No validation result is available.
    /// </summary>
    None = 0,
}
