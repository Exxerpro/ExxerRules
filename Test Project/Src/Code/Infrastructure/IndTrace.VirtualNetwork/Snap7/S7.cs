using System.Diagnostics;
using System.Runtime.InteropServices;

using IndTrace.VirtualNetwork.Services;
using IndTrace.VirtualNetwork.Simulation;

namespace IndTrace.VirtualNetwork.Snap7;

/// <summary>
/// Provides static methods for reading and writing various data types to and from byte arrays, mimicking Siemens S7 PLC data formats.
/// </summary>
public static class S7
{
    private static long bias = 621355968000000000; // "decimicros" between 0001-01-01 00:00:00 and 1970-01-01 00:00:00

    private static int BcDtoByte(byte b)
    {
        return ((b >> 4) * 10) + (b & 0x0F);
    }

    private static byte ByteToBcd(int value)
    {
        return (byte)(((value / 10) << 4) | (value % 10));
    }

    private static byte[] CopyFrom(byte[] buffer, int pos, int size)
    {
        byte[] result = new byte[size];
        Array.Copy(buffer, pos, result, 0, size);
        return result;
    }


    /// <summary>
    /// Gets the boolean value of a specific bit at a given position in a byte array.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The byte position in the buffer.</param>
    /// <param name="bit">The bit position within the byte (0-7).</param>
    /// <returns><see langword="true"/> if the bit is set; otherwise, <see langword="false"/>.</returns>
    public static bool GetBitAt(byte[] buffer, int pos, int bit)
    {
        byte[] mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
        if (bit < 0) bit = 0;
        if (bit > 7) bit = 7;
        return (buffer[pos] & mask[bit]) != 0;
    }

    /// <summary>
    /// Sets the value of a specific bit at a given position in a byte array.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The byte position in the buffer.</param>
    /// <param name="bit">The bit position within the byte (0-7).</param>
    /// <param name="value">The boolean value to set the bit to.</param>
    public static void SetBitAt(ref byte[] buffer, int pos, int bit, bool value)
    {
        byte[] mask = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
        if (bit < 0) bit = 0;
        if (bit > 7) bit = 7;

        if (value)
            buffer[pos] = (byte)(buffer[pos] | mask[bit]);
        else
            buffer[pos] = (byte)(buffer[pos] & ~mask[bit]);
    }



    /// <summary>
    /// Gets an 8-bit signed integer (S7 SInt) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 8-bit signed integer value.</returns>
    public static int GetSIntAt(byte[] buffer, int pos)
    {
        int value = buffer[pos];
        if (value < 128)
            return value;
        else
            return (int)(value - 256);
    }

    /// <summary>
    /// Sets an 8-bit signed integer (S7 SInt) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 8-bit signed integer value to set.</param>
    public static void SetSIntAt(byte[] buffer, int pos, int value)
    {
        if (value < -128) value = -128;
        if (value > 127) value = 127;
        buffer[pos] = (byte)value;
    }



    /// <summary>
    /// Gets a 16-bit signed integer (S7 Int) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 16-bit signed integer value.</returns>
    public static int GetIntAt(byte[] buffer, int pos)
    {
        return (int)((buffer[pos] << 8) | buffer[pos + 1]);
    }

    /// <summary>
    /// Sets a 16-bit signed integer (S7 Int) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 16-bit signed integer value to set.</param>
    public static void SetIntAt(byte[] buffer, int pos, short value)
    {
        buffer[pos] = (byte)(value >> 8);
        buffer[pos + 1] = (byte)(value & 0x00FF);
    }



    /// <summary>
    /// Gets a 32-bit signed integer (S7 DInt) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 32-bit signed integer value.</returns>
    public static int GetDIntAt(byte[] buffer, int pos)
    {
        int result;
        result = buffer[pos]; result <<= 8;
        result += buffer[pos + 1]; result <<= 8;
        result += buffer[pos + 2]; result <<= 8;
        result += buffer[pos + 3];
        return result;
    }

    /// <summary>
    /// Sets a 32-bit signed integer (S7 DInt) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 32-bit signed integer value to set.</param>
    public static void SetDIntAt(byte[] buffer, int pos, int value)
    {
        buffer[pos + 3] = (byte)(value & 0xFF);
        buffer[pos + 2] = (byte)((value >> 8) & 0xFF);
        buffer[pos + 1] = (byte)((value >> 16) & 0xFF);
        buffer[pos] = (byte)((value >> 24) & 0xFF);
    }



    /// <summary>
    /// Gets a 64-bit signed integer (S7 LInt) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 64-bit signed integer value.</returns>
    public static long GetLIntAt(byte[] buffer, int pos)
    {
        long result;
        result = buffer[pos]; result <<= 8;
        result += buffer[pos + 1]; result <<= 8;
        result += buffer[pos + 2]; result <<= 8;
        result += buffer[pos + 3]; result <<= 8;
        result += buffer[pos + 4]; result <<= 8;
        result += buffer[pos + 5]; result <<= 8;
        result += buffer[pos + 6]; result <<= 8;
        result += buffer[pos + 7];
        return result;
    }

    /// <summary>
    /// Sets a 64-bit signed integer (S7 LInt) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 64-bit signed integer value to set.</param>
    public static void SetLIntAt(byte[] buffer, int pos, long value)
    {
        buffer[pos + 7] = (byte)(value & 0xFF);
        buffer[pos + 6] = (byte)((value >> 8) & 0xFF);
        buffer[pos + 5] = (byte)((value >> 16) & 0xFF);
        buffer[pos + 4] = (byte)((value >> 24) & 0xFF);
        buffer[pos + 3] = (byte)((value >> 32) & 0xFF);
        buffer[pos + 2] = (byte)((value >> 40) & 0xFF);
        buffer[pos + 1] = (byte)((value >> 48) & 0xFF);
        buffer[pos] = (byte)((value >> 56) & 0xFF);
    }



    /// <summary>
    /// Gets an 8-bit unsigned integer (S7 USInt) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 8-bit unsigned integer value.</returns>
    public static byte GetUsIntAt(byte[] buffer, int pos)
    {
        return buffer[pos];
    }

    /// <summary>
    /// Sets an 8-bit unsigned integer (S7 USInt) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 8-bit unsigned integer value to set.</param>
    public static void SetUsIntAt(byte[] buffer, int pos, byte value)
    {
        buffer[pos] = value;
    }



    /// <summary>
    /// Gets a 16-bit unsigned integer (S7 UInt) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 16-bit unsigned integer value.</returns>
    public static ushort GetUIntAt(byte[] buffer, int pos)
    {
        return (ushort)((buffer[pos] << 8) | buffer[pos + 1]);
    }

    /// <summary>
    /// Sets a 16-bit unsigned integer (S7 UInt) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 16-bit unsigned integer value to set.</param>
    public static void SetUIntAt(byte[] buffer, int pos, ushort value)
    {
        buffer[pos] = (byte)(value >> 8);
        buffer[pos + 1] = (byte)(value & 0x00FF);
    }



    /// <summary>
    /// Gets a 32-bit unsigned integer (S7 UDInt) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 32-bit unsigned integer value.</returns>
    public static uint GetUdIntAt(byte[] buffer, int pos)
    {
        uint result;
        result = buffer[pos]; result <<= 8;
        result |= buffer[pos + 1]; result <<= 8;
        result |= buffer[pos + 2]; result <<= 8;
        result |= buffer[pos + 3];
        return result;
    }

    /// <summary>
    /// Sets a 32-bit unsigned integer (S7 UDInt) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 32-bit unsigned integer value to set.</param>
    public static void SetUdIntAt(byte[] buffer, int pos, uint value)
    {
        buffer[pos + 3] = (byte)(value & 0xFF);
        buffer[pos + 2] = (byte)((value >> 8) & 0xFF);
        buffer[pos + 1] = (byte)((value >> 16) & 0xFF);
        buffer[pos] = (byte)((value >> 24) & 0xFF);
    }



    /// <summary>
    /// Gets a 64-bit unsigned integer (S7 ULInt) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 64-bit unsigned integer value.</returns>
    public static ulong GetUlIntAt(byte[] buffer, int pos)
    {
        ulong result;
        result = buffer[pos]; result <<= 8;
        result |= buffer[pos + 1]; result <<= 8;
        result |= buffer[pos + 2]; result <<= 8;
        result |= buffer[pos + 3]; result <<= 8;
        result |= buffer[pos + 4]; result <<= 8;
        result |= buffer[pos + 5]; result <<= 8;
        result |= buffer[pos + 6]; result <<= 8;
        result |= buffer[pos + 7];
        return result;
    }

    /// <summary>
    /// Sets a 64-bit unsigned integer (S7 ULInt) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 64-bit unsigned integer value to set.</param>
    public static void SetULintAt(byte[] buffer, int pos, ulong value)
    {
        buffer[pos + 7] = (byte)(value & 0xFF);
        buffer[pos + 6] = (byte)((value >> 8) & 0xFF);
        buffer[pos + 5] = (byte)((value >> 16) & 0xFF);
        buffer[pos + 4] = (byte)((value >> 24) & 0xFF);
        buffer[pos + 3] = (byte)((value >> 32) & 0xFF);
        buffer[pos + 2] = (byte)((value >> 40) & 0xFF);
        buffer[pos + 1] = (byte)((value >> 48) & 0xFF);
        buffer[pos] = (byte)((value >> 56) & 0xFF);
    }



    /// <summary>
    /// Gets an 8-bit word (S7 Byte) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 8-bit word value.</returns>
    public static byte GetByteAt(byte[] buffer, int pos)
    {
        return buffer[pos];
    }

    /// <summary>
    /// Sets an 8-bit word (S7 Byte) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 8-bit word value to set.</param>
    public static void SetByteAt(byte[] buffer, int pos, byte value)
    {
        buffer[pos] = value;
    }



    /// <summary>
    /// Gets a 16-bit word (S7 Word) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 16-bit word value.</returns>
    public static ushort GetWordAt(byte[] buffer, int pos)
    {
        return GetUIntAt(buffer, pos);
    }

    /// <summary>
    /// Sets a 16-bit word (S7 Word) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 16-bit word value to set.</param>
    public static void SetWordAt(byte[] buffer, int pos, ushort value)
    {
        SetUIntAt(buffer, pos, value);
    }



    /// <summary>
    /// Gets a 32-bit word (S7 DWord) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 32-bit word value.</returns>
    public static uint GetDWordAt(byte[] buffer, int pos)
    {
        return GetUdIntAt(buffer, pos);
    }

    /// <summary>
    /// Sets a 32-bit word (S7 DWord) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 32-bit word value to set.</param>
    public static void SetDWordAt(byte[] buffer, int pos, uint value)
    {
        SetUdIntAt(buffer, pos, value);
    }



    /// <summary>
    /// Gets a 64-bit word (S7 LWord) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 64-bit word value.</returns>
    public static ulong GetLWordAt(byte[] buffer, int pos)
    {
        return GetUlIntAt(buffer, pos);
    }

    /// <summary>
    /// Sets a 64-bit word (S7 LWord) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 64-bit word value to set.</param>
    public static void SetLWordAt(byte[] buffer, int pos, ulong value)
    {
        SetULintAt(buffer, pos, value);
    }



    /// <summary>
    /// Gets a 32-bit floating-point number (S7 Real) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 32-bit floating-point number value.</returns>
    public static float GetRealAt(byte[] buffer, int pos)
    {
        uint value = GetUdIntAt(buffer, pos);
        byte[] bytes = BitConverter.GetBytes(value);
        return BitConverter.ToSingle(bytes, 0);
    }

    /// <summary>
    /// Sets a 32-bit floating-point number (S7 Real) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 32-bit floating-point number value to set.</param>
    public static void SetRealAt(byte[] buffer, int pos, float value)
    {
        byte[] floatArray = BitConverter.GetBytes(value);
        buffer[pos] = floatArray[3];
        buffer[pos + 1] = floatArray[2];
        buffer[pos + 2] = floatArray[1];
        buffer[pos + 3] = floatArray[0];
    }



    /// <summary>
    /// Gets a 64-bit floating-point number (S7 LReal) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The 64-bit floating-point number value.</returns>
    public static double GetLRealAt(byte[] buffer, int pos)
    {
        ulong value = GetUlIntAt(buffer, pos);
        byte[] bytes = BitConverter.GetBytes(value);
        return BitConverter.ToDouble(bytes, 0);
    }

    /// <summary>
    /// Sets a 64-bit floating-point number (S7 LReal) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The 64-bit floating-point number value to set.</param>
    public static void SetLRealAt(byte[] buffer, int pos, double value)
    {
        byte[] floatArray = BitConverter.GetBytes(value);
        buffer[pos] = floatArray[7];
        buffer[pos + 1] = floatArray[6];
        buffer[pos + 2] = floatArray[5];
        buffer[pos + 3] = floatArray[4];
        buffer[pos + 4] = floatArray[3];
        buffer[pos + 5] = floatArray[2];
        buffer[pos + 6] = floatArray[1];
        buffer[pos + 7] = floatArray[0];
    }



    /// <summary>
    /// Gets a <see cref="DateTime"/> value (S7 DATE_AND_TIME) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The <see cref="DateTime"/> value.</returns>
    public static DateTime GetDateTimeAt(byte[] buffer, int pos)
    {
        int year, month, day, hour, min, sec, mSec;

        year = BcDtoByte(buffer[pos]);
        if (year < 90)
            year += 2000;
        else
            year += 1900;

        month = BcDtoByte(buffer[pos + 1]);
        day = BcDtoByte(buffer[pos + 2]);
        hour = BcDtoByte(buffer[pos + 3]);
        min = BcDtoByte(buffer[pos + 4]);
        sec = BcDtoByte(buffer[pos + 5]);
        mSec = (BcDtoByte(buffer[pos + 6]) * 10) + (BcDtoByte(buffer[pos + 7]) / 10);
        try
        {
            return new DateTime(year, month, day, hour, min, sec, mSec);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return new DateTime(0);
        }
    }

    /// <summary>
    /// Sets a <see cref="DateTime"/> value (S7 DATE_AND_TIME) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The <see cref="DateTime"/> value to set.</param>
    public static void SetDateTimeAt(byte[] buffer, int pos, DateTime value)
    {
        int year = value.Year;
        int month = value.Month;
        int day = value.Day;
        int hour = value.Hour;
        int min = value.Minute;
        int sec = value.Second;
        int dow = (int)value.DayOfWeek + 1;
        // MSecH = First two digits of miliseconds
        int msecH = value.Millisecond / 10;
        // MSecL = Last digit of miliseconds
        int msecL = value.Millisecond % 10;
        if (year > 1999)
            year -= 2000;

        buffer[pos] = ByteToBcd(year);
        buffer[pos + 1] = ByteToBcd(month);
        buffer[pos + 2] = ByteToBcd(day);
        buffer[pos + 3] = ByteToBcd(hour);
        buffer[pos + 4] = ByteToBcd(min);
        buffer[pos + 5] = ByteToBcd(sec);
        buffer[pos + 6] = ByteToBcd(msecH);
        buffer[pos + 7] = ByteToBcd((msecL * 10) + dow);
    }



    /// <summary>
    /// Gets a <see cref="DateTime"/> value (S7 DATE) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The <see cref="DateTime"/> value.</returns>
    public static DateTime GetDateAt(byte[] buffer, int pos)
    {
        try
        {
            return new DateTime(1990, 1, 1).AddDays(GetIntAt(buffer, pos));
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return new DateTime(0);
        }
    }

    /// <summary>
    /// Sets a <see cref="DateTime"/> value (S7 DATE) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The <see cref="DateTime"/> value to set.</param>
    public static void SetDateAt(byte[] buffer, int pos, DateTime value)
    {
        SetIntAt(buffer, pos, (short)(value - new DateTime(1990, 1, 1)).Days);
    }



    /// <summary>
    /// Gets a <see cref="DateTime"/> value (S7 TIME_OF_DAY) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The <see cref="DateTime"/> value.</returns>
    public static DateTime GetTodAt(byte[] buffer, int pos)
    {
        try
        {
            return new DateTime(0).AddMilliseconds(S7.GetDIntAt(buffer, pos));
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return new DateTime(0);
        }
    }

    /// <summary>
    /// Sets a <see cref="DateTime"/> value (S7 TIME_OF_DAY) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The <see cref="DateTime"/> value to set.</param>
    public static void SetTodAt(byte[] buffer, int pos, DateTime value)
    {
        TimeSpan time = value.TimeOfDay;
        SetDIntAt(buffer, pos, (int)Math.Round(time.TotalMilliseconds));
    }



    /// <summary>
    /// Gets a <see cref="DateTime"/> value (S7 1500 LONG TIME_OF_DAY) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The <see cref="DateTime"/> value.</returns>
    public static DateTime GetLtodAt(byte[] buffer, int pos)
    {
        // .NET Tick = 100 ns, S71500 Tick = 1 ns
        try
        {
            return new DateTime(Math.Abs(GetLIntAt(buffer, pos) / 100));
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return new DateTime(0);
        }
    }

    /// <summary>
    /// Sets a <see cref="DateTime"/> value (S7 1500 LONG TIME_OF_DAY) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The <see cref="DateTime"/> value to set.</param>
    public static void SetLtodAt(byte[] buffer, int pos, DateTime value)
    {
        TimeSpan time = value.TimeOfDay;
        SetLIntAt(buffer, pos, (long)time.Ticks * 100);
    }



    /// <summary>
    /// Gets a <see cref="DateTime"/> value (S7 1500 Long Date and Time) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The <see cref="DateTime"/> value.</returns>
    public static DateTime GetLdtAt(byte[] buffer, int pos)
    {
        try
        {
            return new DateTime((GetLIntAt(buffer, pos) / 100) + bias);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return new DateTime(0);
        }
    }

    /// <summary>
    /// Sets a <see cref="DateTime"/> value (S7 1500 Long Date and Time) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The <see cref="DateTime"/> value to set.</param>
    public static void SetLdtAt(byte[] buffer, int pos, DateTime value)
    {
        SetLIntAt(buffer, pos, (value.Ticks - bias) * 100);
    }



    /// <summary>
    /// Gets a <see cref="DateTime"/> value (S71200/1500 Date and Time) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The <see cref="DateTime"/> value.</returns>
    public static DateTime GetDtlAt(byte[] buffer, int pos)
    {
        int year, month, day, hour, min, sec, mSec;

        year = (buffer[pos] * 256) + buffer[pos + 1];
        month = buffer[pos + 2];
        day = buffer[pos + 3];
        hour = buffer[pos + 5];
        min = buffer[pos + 6];
        sec = buffer[pos + 7];
        mSec = (int)GetUdIntAt(buffer, pos + 8) / 1000000;

        try
        {
            return new DateTime(year, month, day, hour, min, sec, mSec);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return new DateTime(0);
        }
    }

    /// <summary>
    /// Sets a <see cref="DateTime"/> value (S71200/1500 Date and Time) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The <see cref="DateTime"/> value to set.</param>
    public static void SetDtlAt(byte[] buffer, int pos, DateTime value)
    {
        short year = (short)value.Year;
        byte month = (byte)value.Month;
        byte day = (byte)value.Day;
        byte hour = (byte)value.Hour;
        byte min = (byte)value.Minute;
        byte sec = (byte)value.Second;
        byte dow = (byte)(value.DayOfWeek + 1);

        int nanoSecs = value.Millisecond * 1000000;

        var bytesShort = BitConverter.GetBytes(year);

        buffer[pos] = bytesShort[1];
        buffer[pos + 1] = bytesShort[0];
        buffer[pos + 2] = month;
        buffer[pos + 3] = day;
        buffer[pos + 4] = dow;
        buffer[pos + 5] = hour;
        buffer[pos + 6] = min;
        buffer[pos + 7] = sec;
        SetDIntAt(buffer, pos + 8, nanoSecs);
    }



    /// <summary>
    /// Gets a string (S7 String) from a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <returns>The string value.</returns>
    public static string GetStringAt(byte[] buffer, int pos)
    {
        int size = (int)buffer[pos + 1];
        return Encoding.ASCII.GetString(buffer, pos + 2, size);
    }

    /// <summary>
    /// Sets a string (S7 String) in a byte array at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="maxLen">The maximum length of the string.</param>
    /// <param name="value">The string value to set.</param>
    public static void SetStringAt(byte[] buffer, int pos, int maxLen, string value)
    {
        int size = value.Length;
        buffer[pos] = (byte)maxLen;
        buffer[pos + 1] = (byte)size;
        Encoding.ASCII.GetBytes(value, 0, size, buffer, pos + 2);
    }



    /// <summary>
    /// Gets a string from a byte array representing an S7 ARRAY OF CHARS at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array containing the data.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="size">The number of characters to read.</param>
    /// <returns>The string value.</returns>
    public static string GetCharsAt(byte[] buffer, int pos, int size)
    {
        return Encoding.ASCII.GetString(buffer, pos, size);
    }

    /// <summary>
    /// Sets a string in a byte array representing an S7 ARRAY OF CHARS at a specified position.
    /// </summary>
    /// <param name="buffer">The byte array to write to.</param>
    /// <param name="pos">The starting position in the buffer.</param>
    /// <param name="value">The string value to set.</param>
    public static void SetCharsAt(byte[] buffer, int pos, string value)
    {
        int maxLen = buffer.Length - pos;
        // Truncs the string if there's no room enough
        if (maxLen > value.Length) maxLen = value.Length;
        Encoding.ASCII.GetBytes(value, 0, maxLen, buffer, pos);
    }



    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate S7 logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    //TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated S7 or communication logic. Refactor for maintainability if necessary.
    //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency S7 operations, consider optimizing communication and memory usage.
}
