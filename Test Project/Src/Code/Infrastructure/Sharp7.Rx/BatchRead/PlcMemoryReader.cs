// <copyright file="PlcMemoryReader.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.BatchRead;

using Sharp7.Rx.Enums;

/// <summary>
/// Provides methods for reading PLC memory from a cached buffer.
/// </summary>
public static class PlcMemoryReader
{
    /// <summary>
    /// Reads a slice of PLC memory from a cached buffer instead of directly from the network.
    /// </summary>
    /// <param name="buffer">The full byte array previously read from the PLC.</param>
    /// <param name="operand">The type of memory area (e.g., DB, PE, etc.).</param>
    /// <param name="start">The start byte offset.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <param name="dbNo">The data block number (only relevant for DB area).</param>
    /// <returns>A byte array slice of the requested data.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when requested slice is out of buffer range.</exception>
    public static byte[] ReadFromMemoryBuffer(byte[] buffer, Operand operand, int start, int length, int dbNo = 0)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer), "PLC memory buffer cannot be null.");
        }

        if (start < 0 || length < 0 || start + length > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start), $"Requested slice [{start}..{start + length}] exceeds buffer length {buffer.Length}");
        }

        // Future extension: Validate operand type and dbNo relevance here
        return buffer.Skip(start).Take(length).ToArray();
    }
}
