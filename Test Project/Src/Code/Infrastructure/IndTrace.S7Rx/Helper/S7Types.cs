// <copyright file="S7Types.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.S7Rx.Helper;

public static class S7Types
{
    public static Dictionary<string, AtomicTypes> Types()
    {
        var types = new Dictionary<string, AtomicTypes>
        {
            {
                "System.Boolean",
                new AtomicTypes
                {
                    Name = "Boolean",
                    FullName = "System.Boolean",
                    Type = typeof(bool),
                    AtomicName = "ATOMIC.BOOL",
                    AtomicNumber = 0,
                }
            },
            {
                "System.SByte",
                new AtomicTypes
                {
                    Name = "SByte",
                    FullName = "System.SByte",
                    Type = typeof(byte),
                    AtomicName = "ATOMIC.SBYTE",
                    AtomicNumber = 1,
                }
            },
            {
                "System.Byte",
                new AtomicTypes
                {
                    Name = "Byte",
                    FullName = "System.Byte",
                    Type = typeof(byte),
                    AtomicName = "ATOMIC.BYTE",
                    AtomicNumber = 2,
                }
            },
            {
                "System.Int16",
                new AtomicTypes
                {
                    Name = "Int16",
                    FullName = "System.Int16",
                    Type = typeof(short),
                    AtomicName = "ATOMIC.INT",
                    AtomicNumber = 3,
                }
            },
            {
                "System.UInt16",
                new AtomicTypes
                {
                    Name = "UInt16",
                    FullName = "System.UInt16",
                    Type = typeof(ushort),
                    AtomicName = "ATOMIC.WORD",
                    AtomicNumber = 4,
                }
            },
            {
                "System.Int32",
                new AtomicTypes
                {
                    Name = "Int32",
                    FullName = "System.Int32",
                    Type = typeof(int),
                    AtomicName = "ATOMIC.DINT",
                    AtomicNumber = 5,
                }
            },
            {
                "System.UInt32",
                new AtomicTypes
                {
                    Name = "UInt32",
                    FullName = "System.UInt32",
                    Type = typeof(uint),
                    AtomicName = "ATOMIC.DWORD",
                    AtomicNumber = 6,
                }
            },
            {
                "System.Single",
                new AtomicTypes
                {
                    Name = "Single",
                    FullName = "System.Single",
                    Type = typeof(float),
                    AtomicName = "ATOMIC.REAL",
                    AtomicNumber = 7,
                }
            },
            {
                "System.Int64",
                new AtomicTypes
                {
                    Name = "Int64",
                    FullName = "System.Int64",
                    Type = typeof(int),
                    AtomicName = "ATOMIC.LINT",
                    AtomicNumber = 8,
                }
            },
            {
                "System.UInt64",
                new AtomicTypes
                {
                    Name = "UInt64",
                    FullName = "System.UInt64",
                    Type = typeof(ulong),
                    AtomicName = "ATOMIC.ULINT",
                    AtomicNumber = 9,
                }
            },
            {
                "System.Double",
                new AtomicTypes
                {
                    Name = "Double",
                    FullName = "System.Double",
                    Type = typeof(double),
                    AtomicName = "ATOMIC.LREAL",
                    AtomicNumber = 10,
                }
            },

            // {
            //    "System.Int64",
            //    new AtomicTypes
            //    {
            //        Name = "Int64",
            //        FullName = "System.Int64",
            //        Type = typeof(Int64),
            //        AtomicName = "ATOMIC.LWORD",
            //        AtomicNumber = 11
            //    }
            // },
            {
                "System.String",
                new AtomicTypes
                {
                    Name = "String",
                    FullName = "System.String",
                    Type = typeof(string),
                    AtomicName = "ATOMIC.STRING",
                    AtomicNumber = 12,
                }
            },
        };

        return types;
    }

    /// <summary>
    /// Represents the AtomicTypes.
    /// </summary>
    public class AtomicTypes
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        public Type Type { get; set; } = typeof(object);

        /// <summary>
        /// Gets or sets the AtomicName.
        /// </summary>
        public string AtomicName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the AtomicNumber.
        /// </summary>
        public int AtomicNumber { get; set; }
    }

    /// <summary>
    /// Gets an array of .NET type names corresponding to PLC data types.
    /// </summary>
    public static readonly string[] NetTypes = new[]
    {
        "System.Boolean",   // = ATOMIC.BOOL  = 0
        "System.SByte",     // = ATOMIC.BYTE   = 1
        "System.Byte",      // = ATOMIC.SBYTE = 2
        "System.Int16",     // = ATOMIC.INT = 3
        "System.UInt16",    // = ATOMIC.WORD = 4
        "System.Int32",     // = ATOMIC.DINT = 5
        "System.UInt32",    // = ATOMIC.DWORD = 6
        "System.Single",    // = ATOMIC.REAL = 7
        "System.Int64",     // = ATOMIC.LINT = 8
        "System.UInt64",    // = ATOMIC.ULINT = 9
        "System.Double",    // = ATOMIC.LREAL = 10
        "System.Int64",     // = ATOMIC.LWORD = 11
        "System.String",     // = ATOMIC.STRING = 12
    };

    /// <summary>
    /// Represents the different types of Siemens S7 CPUs.
    /// </summary>
    public enum CPU
    {
        /// <summary>
        /// Represents a Siemens S7-200 CPU.
        /// </summary>
        S7200,

        /// <summary>
        /// Represents a Siemens S7-300 CPU.
        /// </summary>
        S7300,

        /// <summary>
        /// Represents a Siemens S7-400 CPU.
        /// </summary>
        S7400,

        /// <summary>
        /// Represents a Siemens S7-1200 CPU.
        /// </summary>
        S71200,
    }
}

// System.Boolean = ATOMIC.BOOL
// System.SByte = ATOMIC.BYTE
// System.Byte = ATOMIC.SBYTE
// System.Int16 = ATOMIC.INT
// System.UInt16 = ATOMIC.WORD
// System.Int32 = ATOMIC.DINT
// System.UInt32 = ATOMIC.DWORD
// System.Single = ATOMIC.REAL
// System.Int64 = ATOMIC.LINT
// System.UInt64 = ATOMIC.ULINT
// System.Double = ATOMIC.LREAL
// System.Int64 = ATOMIC.LWORD
// System.String = ATOMIC.STRING

// BOOL PLC Type(Bit/Discrete) .NET equivalent Boolean
// SBYTE   PLC Type(8-bit signed byte) .NET equivalent SByte
// BYTE    PLC Type(8-bit unsigned byte) .NET equivalent Byte
// INT PLC Type(16-bit signed integer) .NET equivalent Int16
// WORD    PLC Type(16-bit unsigned integer) .NET equivalent UInt16
// DINT    PLC Type(32-Bit signed integer) .NET equivalent Int32
// DWORD   PLC Type(32-Bit unsigned integer) .NET equivalent UInt32
// REAL    PLC Type(32-bit Single Precision Real) .NET equivalent Single
// LINT    PLC Type(64-Bit signed integer) .NET equivalent Int64
// ULINT   PLC Type(64-Bit unsigned integer) .NET equivalent UInt64
// LREAL   PLC Type(64-Bit double precision real) .NET equivalent Double
// LWORD   PLC Type(64-Bit signed integer) .NET equivalent Int64
// STRING  PLC Type(ASCII Character String) .NET equivalent String

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate S7Types logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
