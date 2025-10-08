// <copyright file="TransmissionMode.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Enums;

/// <summary>
/// Specifies the transmission mode for PLC data.
/// </summary>
public enum TransmissionMode
{
    /// <summary>
    /// Data is transmitted cyclically at regular intervals.
    /// </summary>
    Cyclic = 3,

    /// <summary>
    /// Data is transmitted only when its value changes.
    /// </summary>
    OnChange = 4,
}
