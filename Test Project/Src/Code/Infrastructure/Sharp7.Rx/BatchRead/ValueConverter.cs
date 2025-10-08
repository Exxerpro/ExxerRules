// <copyright file="ValueConverter.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.BatchRead;

using System.Buffers.Binary;
using System.Text;
using Sharp7.Rx.Enums;

/// <summary>
/// Provides methods for converting values to and from byte buffers for batch read operations.
/// </summary>
internal static class ValueConverter
{
    private static readonly Dictionary<Type, ReadFunc> ReadFunctions = new()
    {
        { typeof(bool), (buffer, address) => (buffer[0] >> address.Bit & 1) > 0 },
        { typeof(byte), (buffer, _) => buffer[0] },
        { typeof(byte[]), (buffer, _) => buffer.ToArray() },
        { typeof(short), (buffer, _) => BinaryPrimitives.ReadInt16BigEndian(buffer) },
        { typeof(ushort), (buffer, _) => BinaryPrimitives.ReadUInt16BigEndian(buffer) },
        { typeof(int), (buffer, _) => BinaryPrimitives.ReadInt32BigEndian(buffer) },
        { typeof(uint), (buffer, _) => BinaryPrimitives.ReadUInt32BigEndian(buffer) },
        { typeof(long), (buffer, _) => BinaryPrimitives.ReadInt64BigEndian(buffer) },
        { typeof(ulong), (buffer, _) => BinaryPrimitives.ReadUInt64BigEndian(buffer) },
        { typeof(float), (buffer, _) => BinaryPrimitives.ReadSingleBigEndian(buffer) },
        { typeof(double), (buffer, _) => BinaryPrimitives.ReadDoubleBigEndian(buffer) },
        {
            typeof(string), (buffer, address) =>
            {
                return address.Type switch
                {
                    DbType.String => ParseString(buffer, address),
                    DbType.WString => ParseWString(buffer, address),
                    DbType.Byte => Encoding.ASCII.GetString(buffer),
                    _ => throw new DataTypeMissmatchException($"Cannot read string from {address.Type}", typeof(string), address),
                };
            }
        },
    };

    private delegate object ReadFunc(Span<byte> data, VariableAddress address);

    /// <summary>
    /// Reads a value from a byte buffer and returns it as a dynamic type along with its string representation.
    /// </summary>
    /// <param name="buffer">The byte buffer containing the data.</param>
    /// <param name="address">The variable address information.</param>
    /// <param name="type">The target type to convert the value to.</param>
    /// <returns>A tuple containing the dynamic result and its string representation.</returns>
    /// <exception cref="ArgumentException">Thrown if the buffer length is less than the required buffer length for the address.</exception>
    /// <exception cref="UnsupportedS7TypeException">Thrown if the specified type is not supported.</exception>
    public static (dynamic Result, string StringValue) ReadFromBuffer(Span<byte> buffer, VariableAddress address, Type type)
    {
        if (buffer.Length < address.BufferLength)
        {
            throw new ArgumentException($"Buffer must be at least {address.BufferLength} bytes long for {address}", nameof(buffer));
        }

        if (!ReadFunctions.TryGetValue(type, out var readFunc))
        {
            throw new UnsupportedS7TypeException($"{type.Name} is not supported. {address}", type, address);
        }

        var result = readFunc(buffer, address);

        string stringRepresentation = result switch
        {
            null => string.Empty,
            byte[] arr => BitConverter.ToString(arr),
            _ => result.ToString() ?? string.Empty,
        };

        return (result, stringRepresentation);
    }

    /// <summary>
    /// Reads a value from a byte buffer and returns it as a specified type along with its string representation.
    /// </summary>
    /// <typeparam name="T">The target type to convert the value to.</typeparam>
    /// <param name="buffer">The byte buffer containing the data.</param>
    /// <param name="address">The variable address information.</param>
    /// <param name="type">The target type to convert the value to.</param>
    /// <returns>A tuple containing the result of type <typeparamref name="T"/> and its string representation.</returns>
    /// <exception cref="ArgumentException">Thrown if the buffer length is less than the required buffer length for the address.</exception>
    /// <exception cref="UnsupportedS7TypeException">Thrown if the specified type is not supported.</exception>
    /// <exception cref="InvalidCastException">Thrown if the type is not supported or did not match the expected result type.</exception>
    public static (T Result, string StringValue) ReadFromBuffer<T>(Span<byte> buffer, VariableAddress address, Type type)
    {
        if (buffer.Length < address.BufferLength)
        {
            throw new ArgumentException($"Buffer must be at least {address.BufferLength} bytes long for {address}", nameof(buffer));
        }

        if (!ReadFunctions.TryGetValue(type, out var readFunc))
        {
            throw new UnsupportedS7TypeException($"{type.Name} is not supported. {address}", type, address);
        }

        object result = readFunc(buffer, address);

        string stringRepresentation = result switch
        {
            null => string.Empty,
            byte[] arr => BitConverter.ToString(arr),
            _ => result.ToString() ?? string.Empty,
        };

        return type switch
        {
            var t when t == typeof(string) && result is string str => ((T)(object)str, str),
            var t when t == typeof(int) && result is int i => ((T)(object)i, stringRepresentation),
            var t when t == typeof(short) && result is short s => ((T)(object)s, stringRepresentation),
            _ => throw new InvalidCastException($"Type '{type}' is not supported or did not match the expected result type."),
        };
    }

    private static string ParseString(Span<byte> buffer, VariableAddress address)
    {
        var length = Math.Min(address.Length, buffer[1]);
        return Encoding.ASCII.GetString(buffer.Slice(2, length));
    }

    private static string ParseWString(Span<byte> buffer, VariableAddress address)
    {
        var statedStringLength = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(2, 2));
        var length = Math.Min(address.Length, statedStringLength) * 2;
        return Encoding.BigEndianUnicode.GetString(buffer.Slice(4, length));
    }
}
