// <copyright file="PlcBatchReadResult.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.BatchRead;

/// <summary>
/// Represents the result of a batch read operation from a PLC.
/// </summary>
/// <param name="Address">The address of the PLC tag.</param>
/// <param name="Alias">The alias of the PLC tag.</param>
/// <param name="ValueString">The string representation of the tag's value.</param>
/// <param name="ValueInt">The integer representation of the tag's value.</param>
/// <param name="Status">The status of the read operation (e.g., 1 for success, -1 for failure).</param>
public record PlcBatchReadResult(string Address, string Alias, string ValueString, int ValueInt, int Status);
