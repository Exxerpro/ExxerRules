// <copyright file="DbType.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Enums;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the data types supported for PLC communication.
/// </summary>
/// <remarks>
/// See https://support.industry.siemens.com/cs/mdm/109747174?c=88343664523&lc=de-DE for more details.
/// </remarks>
public enum DbType
{
    /// <summary>
    /// Boolean bit type.
    /// </summary>
    Bit,

    /// <summary>
    /// Represents an ASCII string.
    /// </summary>
    String,

    /// <summary>
    /// Represents a UTF16 string.
    /// </summary>
    WString,

    /// <summary>
    /// Represents an 8-bit unsigned integer.
    /// </summary>
    Byte,

    /// <summary>
    /// Represents a 16-bit signed integer (Int16).
    /// </summary>
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Required for PLC communication protocol")]
    Int,

    /// <summary>
    /// Represents a 16-bit unsigned integer (UInt16).
    /// </summary>
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Required for PLC communication protocol")]
    UInt,

    /// <summary>
    /// Represents a 32-bit signed integer (Int32).
    /// </summary>
    DInt,

    /// <summary>
    /// Represents a 32-bit unsigned integer (UInt32).
    /// </summary>
    UDInt,

    /// <summary>
    /// Represents a 64-bit signed integer (Int64).
    /// </summary>
    LInt,

    /// <summary>
    /// Represents a 64-bit unsigned integer (UInt64).
    /// </summary>
    ULInt,

    /// <summary>
    /// Represents a 32-bit floating-point number.
    /// </summary>
    Single,

    /// <summary>
    /// Represents a 64-bit floating-point number.
    /// </summary>
    Double,
}
