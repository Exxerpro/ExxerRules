// <copyright file="PlcTagNames.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models.Constants;

public static class PlcTagNames
{
    /*
     *
     *               new TagDataStore { Description = "PartStatusPlc", Address = "DB257.W0" },
                     new TagDataStore { Description = "CycleStatusPlc", Address = "DB257.W2" },
                     new TagDataStore { Description = "PartNumber", Address = "DB249.S0.32" },
                     new TagDataStore { Description = "BarCode", Address = "DB249.S100.32" },

                     new TagDataStore { Description = "PlcId", Address = "DB254.DBW0" },
                     new TagDataStore { Description = "HeartBeat", Address = "DB254.DBW2" },
                     new TagDataStore { Description = "Command", Address = "DB254.DBW4" },
                     new TagDataStore { Description = "CommandFeedback", Address = "DB254.DBW6" },

                     new TagDataStore { Description = "ResultValidation", Address = "DB256.DINT0" },
                     new TagDataStore { Description = "LastMachineId", Address = "DB256.DINT4" },
                     new TagDataStore { Description = "NextMachineId", Address = "DB256.DINT8" },
                     new TagDataStore { Description = "CycleStatus", Address = "DB256.DINT12" },
                     new TagDataStore { Description = "FlowStatus", Address = "DB256.DINT16" },
                     new TagDataStore { Description = "CycleStatus", Address = "DB256.DINT12" },
                     new TagDataStore { Description = "FlowStatus", Address = "DB256.DINT16" },
                     new TagDataStore { Description = "PartStatus", Address = "DB256.DINT20" },
                     new TagDataStore { Description = "MachineType", Address = "DB256.DINT24" },
                     new TagDataStore { Description = "WorkFlowType", Address = "DB256.DINT28" },
                     new TagDataStore { Description = "CyclesOk", Address = "DB256.DINT48" }
     *
     *
     *
     *
     */

    /// <summary>
    /// Gets the result validation tag name.
    /// </summary>
    public const string ResultValidation = "ResultValidation";

    /// <summary>
    /// Gets the last machine ID tag name.
    /// </summary>
    public const string LastMachineId = "LastMachineId";

    /// <summary>
    /// Gets the next machine ID tag name.
    /// </summary>
    public const string NextMachineId = "NextMachineId";

    /// <summary>
    /// Gets the flow status tag name.
    /// </summary>
    public const string FlowStatus = "FlowStatus";

    /// <summary>
    /// Gets the machine type tag name.
    /// </summary>
    public const string MachineType = "MachineType";

    /// <summary>
    /// Gets the workflow type tag name.
    /// </summary>
    public const string WorkFlowType = "WorkFlowType";

    /// <summary>
    /// Gets the actual station tag name.
    /// </summary>
    public const string ActualStation = "ActualStation";

    /// <summary>
    /// Gets the barcode ID tag name.
    /// </summary>
    public const string BarCodeId = "BarCodeId";

    /// <summary>
    /// Gets the cycle ID tag name.
    /// </summary>
    public const string CycleId = "CycleId";

    /// <summary>
    /// Gets the PLC ID tag name.
    /// </summary>
    public const string PlcId = "PlcId";

    /// <summary>
    /// Gets the heartbeat tag name.
    /// </summary>
    public const string HeartBeat = "HeartBeat";

    /// <summary>
    /// Gets the part number tag name.
    /// </summary>
    public const string PartNumber = "PartNumber";

    /// <summary>
    /// Gets the barcode tag name.
    /// </summary>
    public const string BarCode = "BarCode";

    /// <summary>
    /// Gets the part status tag name.
    /// </summary>
    public const string PartStatus = "PartStatus";

    /// <summary>
    /// Gets the cycle status tag name.
    /// </summary>
    public const string CycleStatus = "CycleStatus";

    /// <summary>
    /// Gets the part status PLC tag name.
    /// </summary>
    public const string PartStatusPlc = "PartStatusPlc";

    /// <summary>
    /// Gets the cycle status PLC tag name.
    /// </summary>
    public const string CycleStatusPlc = "CycleStatusPlc";

    /// <summary>
    /// Gets the command tag name.
    /// </summary>
    public const string Command = "Command";

    /// <summary>
    /// Gets a list of all integer reference tag names.
    /// </summary>
    public static readonly IReadOnlyList<string> AllIntReferenceTags = new List<string>
    {
        ResultValidation,
        LastMachineId,
        NextMachineId,
        CycleStatus,
        FlowStatus,
        PartStatus,
        MachineType,
        WorkFlowType,
        ActualStation,
        BarCodeId,
        CycleId,
    };

    /// <summary>
    /// Gets a list of all string reference tag names.
    /// </summary>
    public static readonly IReadOnlyList<string> AllStringReferenceTags = new List<string>
    {
        PartNumber,
        BarCode,
    };

    /// <summary>
    /// Gets a dictionary mapping reference tag names to their types.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Type> AllReferenceTagTypes = new Dictionary<string, Type>
    {
        { ResultValidation, typeof(int) },
        { LastMachineId, typeof(int) },
        { NextMachineId, typeof(int) },
        { CycleStatus, typeof(int) },
        { CycleStatusPlc, typeof(short) },
        { FlowStatus, typeof(int) },
        { PartStatus, typeof(int) },
        { PartStatusPlc, typeof(short) },
        { MachineType, typeof(int) },
        { WorkFlowType, typeof(int) },
        { ActualStation, typeof(int) },
        { BarCodeId, typeof(int) },
        { PartNumber, typeof(string) },
        { BarCode, typeof(string) },
        { CycleId, typeof(int) },
        { PlcId, typeof(short) },
        { HeartBeat, typeof(short) },
        { Command, typeof(short) },
    };

    public static readonly Dictionary<Type, string> SupportedS7DotNetTypes = new()
    {
        { typeof(bool), typeof(bool).FullName! },     // System.Boolean
        { typeof(byte), typeof(byte).FullName! },     // System.Byte (BYTE/USINT)
        { typeof(short), typeof(short).FullName! },   // System.Int16 (INT)
        { typeof(int), typeof(int).FullName! },       // System.Int32 (DINT)
        { typeof(float), typeof(float).FullName! },   // System.Single (REAL)
        { typeof(string), typeof(string).FullName! }, // System.String (STRING)
        { typeof(long), typeof(long).FullName! },     // System.Int64 (LINT)
        { typeof(ulong), typeof(ulong).FullName! },    // System.UInt64 (ULINT)
    };

    public static readonly Dictionary<Type, string> ExpectedTagTypeMap = new()
    {
        { typeof(bool), "BOOL" },
        { typeof(byte), "BYTE" },
        { typeof(ushort), "WORD" },
        { typeof(uint), "DWORD" },
        { typeof(sbyte), "SINT" },
        { typeof(short), "INT" },
        { typeof(int), "DINT" },
        { typeof(long), "LINT" },
        { typeof(float), "REAL" },
        { typeof(double), "LREAL" },
        { typeof(char), "CHAR" },
        { typeof(string), "STRING" },
        { typeof(TimeSpan), "TIME" },
        { typeof(DateTime), "DATE" },

        // Add other mappings as necessary
    };
}
