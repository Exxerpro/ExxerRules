// <copyright file="VariableAddress.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx;

using Sharp7.Rx.Enums;

/// <summary>
/// Represents a variable address in the Sharp7 Rx library.
/// </summary>

public record VariableAddress(Operand Operand, ushort DbNo, DbType Type, ushort Start, ushort Length, byte? Bit = null)
{
    /// <summary>
    /// Gets the operand type.
    /// </summary>
    public Operand Operand { get; } = Operand;

    /// <summary>
    /// Gets the database number.
    /// </summary>
    public ushort DbNo { get; } = DbNo;

    /// <summary>
    /// Gets the start address.
    /// </summary>
    public ushort Start { get; } = Start;

    /// <summary>
    /// Gets the length of the variable.
    /// </summary>
    public ushort Length { get; } = Length;

    /// <summary>
    /// Gets the bit index, if applicable.
    /// </summary>
    public byte? Bit { get; } = Bit;

    /// <summary>
    /// Gets the database type.
    /// </summary>
    public DbType Type { get; } = Type;

    /// <summary>
    /// Gets the buffer length for the variable, depending on its type.
    /// </summary>
    public ushort BufferLength => this.Type switch
    {
        DbType.String => (ushort)(this.Length + 2),
        DbType.WString => (ushort)((this.Length * 2) + 4),
        _ => this.Length,
    };

    /// <summary>
    /// Returns a string representation of the variable address.
    /// </summary>
    /// <returns>A string describing the variable address.</returns>
    public override string ToString() =>
        this.Type switch
        {
            DbType.Bit => $"{this.Operand}{this.DbNo}.{this.Type}{this.Start}.{this.Bit}",
            DbType.String => $"{this.Operand}{this.DbNo}.{this.Type}{this.Start}.{this.Length}",
            DbType.WString => $"{this.Operand}{this.DbNo}.{this.Type}{this.Start}.{this.Length}",
            DbType.Byte => this.Length == 1 ? $"{this.Operand}{this.DbNo}.{this.Type}{this.Start}" : $"{this.Operand}{this.DbNo}.{this.Type}{this.Start}.{this.Length}",
            _ => $"{this.Operand}{this.DbNo}.{this.Type}{this.Start}",
        };
}
