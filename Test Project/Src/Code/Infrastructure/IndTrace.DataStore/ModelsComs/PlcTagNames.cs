namespace IndTrace.DataStore.ModelsComs;

/// <summary>
/// Provides constants and mappings for PLC tag names, types, and performance tag definitions.
/// </summary>
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

    /*
     *

    PerformanceTags

    PerformanceTags

           "ApplicationFlag",
           "EventCounter",
           "CurrentTime",
           "RunningTime",
           "StoppedTime",
           "FaultedTime",
           "StatusFaultReason",
           "TotalProduction",
           "ProductionOk",
           "ProductionNoK",
           "StatusFaultReject"
       };

     *
     */

    /// <summary>
    /// The application flag tag name.
    /// </summary>
    public const string ApplicationFlag = "ApplicationFlag";
    /// <summary>
    /// The event counter tag name.
    /// </summary>
    public const string EventCounter = "EventCounter";
    /// <summary>
    /// The current time tag name.
    /// </summary>
    public const string CurrentTime = "CurrentTime";
    /// <summary>
    /// The running time tag name.
    /// </summary>
    public const string RunningTime = "RunningTime";
    /// <summary>
    /// The stopped time tag name.
    /// </summary>
    public const string StoppedTime = "StoppedTime";
    /// <summary>
    /// The faulted time tag name.
    /// </summary>
    public const string FaultedTime = "FaultedTime";
    /// <summary>
    /// The status fault reason tag name.
    /// </summary>
    public const string StatusFaultReason = "StatusFaultReason";
    /// <summary>
    /// The total production tag name.
    /// </summary>
    public const string TotalProduction = "TotalProduction";
    /// <summary>
    /// The production OK tag name.
    /// </summary>
    public const string ProductionOk = "ProductionOk";
    /// <summary>
    /// The production NoK tag name.
    /// </summary>
    public const string ProductionNok = "ProductionNoK";
    /// <summary>
    /// The status fault reject tag name.
    /// </summary>
    public const string StatusFaultReject = "StatusFaultReject";
    /// <summary>
    /// The stop event counter tag name.
    /// </summary>
    public const string StopEventCounter = "StopEventCounter";
    /// <summary>
    /// The production NoK tag name (alternate).
    /// </summary>
    public const string ProductionNoK = "ProductionNoK";
    /// <summary>
    /// The reject event counter tag name.
    /// </summary>
    public const string RejectEventCounter = "RejectEventCounter";
    /// <summary>
    /// The reject quantity units tag name.
    /// </summary>
    public const string RejectQuantityUnits = "RejectQuantityUnits";
    /// <summary>
    /// The result validation tag name.
    /// </summary>
    public const string ResultValidation = "ResultValidation";
    /// <summary>
    /// The last machine ID tag name.
    /// </summary>
    public const string LastMachineId = "LastMachineId";
    /// <summary>
    /// The next machine ID tag name.
    /// </summary>
    public const string NextMachineId = "NextMachineId";
    /// <summary>
    /// The flow status tag name.
    /// </summary>
    public const string FlowStatus = "FlowStatus";
    /// <summary>
    /// The machine type tag name.
    /// </summary>
    public const string MachineType = "MachineType";
    /// <summary>
    /// The workflow type tag name.
    /// </summary>
    public const string WorkFlowType = "WorkFlowType";
    /// <summary>
    /// The actual station tag name.
    /// </summary>
    public const string ActualStation = "ActualStation";
    /// <summary>
    /// The barcode ID tag name.
    /// </summary>
    public const string BarCodeId = "BarCodeId";
    /// <summary>
    /// The cycle ID tag name.
    /// </summary>
    public const string CycleId = "CycleId";
    /// <summary>
    /// The PLC ID tag name.
    /// </summary>
    public const string PlcId = "PlcId";
    /// <summary>
    /// The heartbeat tag name.
    /// </summary>
    public const string HeartBeat = "HeartBeat";
    /// <summary>
    /// The part number tag name.
    /// </summary>
    public const string PartNumber = "PartNumber";
    /// <summary>
    /// The barcode tag name.
    /// </summary>
    public const string BarCode = "BarCode";
    /// <summary>
    /// The part status tag name.
    /// </summary>
    public const string PartStatus = "PartStatus";
    /// <summary>
    /// The cycle status tag name.
    /// </summary>
    public const string CycleStatus = "CycleStatus";
    /// <summary>
    /// The part status PLC tag name.
    /// </summary>
    public const string PartStatusPlc = "PartStatusPlc";
    /// <summary>
    /// The cycle status PLC tag name.
    /// </summary>
    public const string CycleStatusPlc = "CycleStatusPlc";
    /// <summary>
    /// The command tag name.
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
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public static readonly IReadOnlyList<string> AllStringReferenceTags = new List<string> { PartNumber, BarCode };

    /// <summary>
    /// Gets a dictionary mapping reference tag names to their types.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Type> AllReferenceTagTypes = new Dictionary<string, Type>
    {
        { ResultValidation, typeof(int) },
        { LastMachineId, typeof(int) },
        { NextMachineId, typeof(int) },

        { CycleStatus, typeof(int) },
        { CycleStatusPlc ,typeof(short)},
        { FlowStatus, typeof(int) },
        { PartStatus, typeof(int) },
        { PartStatusPlc ,typeof(short)},

        { MachineType, typeof(int) },
        { WorkFlowType, typeof(int) },
        { ActualStation, typeof(int) },
        { BarCodeId, typeof(int) },

        { PartNumber, typeof(string) },
        { BarCode, typeof(string) },

        { CycleId , typeof(int) },

        { PlcId ,typeof(short)},
        { HeartBeat  ,typeof(short)},

        { Command,typeof(short)},
    };

    /// <summary>
    /// Gets a dictionary mapping supported .NET types to their full names for S7 communication.
    /// </summary>
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

    /// <summary>
    /// Gets a dictionary mapping .NET types to expected PLC tag type names.
    /// </summary>
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

    /// <summary>
    /// Gets a list of performance tags with their names, addresses, and default values.
    /// </summary>
    public static readonly List<TagDataStore> Performance =
    [
        new("ApplicationFlag", "DB400.DINT00", "0"),
        new("CurrentTime", "DB400.DINT04", "0"),
        new("RunningTime", "DB400.DINT08", "0"),
        new("StoppedTime", "DB400.DINT12", "0"),
        new("StopEventCounter", "DB400.DINT16", "0"),
        new("StatusFaultReason", "DB400.DINT20", "0"),
        new("FaultedTime", "DB400.DINT24", "0"),
        new("RejectEventCounter", "DB400.DINT28", "0"),
        new("StatusFaultReject", "DB400.DINT32", "0"),
        new("RejectQuantityUnits", "DB400.REAL36", "0"),
        new("TotalProduction", "DB400.REAL40", "0"),
        new("ProductionOk", "DB400.REAL44", "0"),
        new("ProductionNoK", "DB400.REAL48", "0"),
    ];

    /// <summary>
    /// Gets a dictionary mapping performance real tag names to their addresses.
    /// </summary>
    public static readonly Dictionary<string, string> PerformanceReals = new()
    {
        {"RejectQuantityUnits", "DB400.REAL36"},
        {"TotalProduction", "DB400.REAL40"},
        {"ProductionOk", "DB400.REAL44"},
        {"ProductionNoK", "DB400.REAL48"},
    };

    /// <summary>
    /// Gets a dictionary mapping performance integer tag names to their addresses.
    /// </summary>
    public static readonly Dictionary<string, string> PerformanceInts = new()
    {
        {"ApplicationFlag", "DB400.DINT00"},
        {"CurrentTime", "DB400.DINT04"},
        {"RunningTime", "DB400.DINT08"},
        {"StoppedTime", "DB400.DINT12"},
        {"StopEventCounter", "DB400.DINT16"},
        {"StatusFaultReason", "DB400.DINT20"},
        {"FaultedTime", "DB400.DINT24"},
        {"RejectEventCounter", "DB400.DINT28"},
        {"StatusFaultReject", "DB400.DINT32"},
    };

    /// <summary>
    /// Gets a list of all performance tag names.
    /// </summary>
    public static readonly IReadOnlyList<string> AllPerformanceTags = new List<string>
    {
        ApplicationFlag,
        CurrentTime,
        RunningTime,
        StoppedTime,
        StopEventCounter,
        StatusFaultReason,
        FaultedTime,
        RejectEventCounter,
        StatusFaultReject,
        RejectQuantityUnits,
        TotalProduction,
        ProductionOk,
        ProductionNoK,
    };

    /// <summary>
    /// Gets a dictionary mapping all performance tag names to their types.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Type> AllPerformanceTagTypes = new Dictionary<string, Type>
    {
        { ApplicationFlag, typeof(int) },
        { CurrentTime, typeof(int) },
        { RunningTime, typeof(int) },
        { StoppedTime, typeof(int) },
        { StopEventCounter, typeof(int) },
        { StatusFaultReason, typeof(int) },
        { FaultedTime, typeof(int) },
        { RejectEventCounter, typeof(int) },
        { StatusFaultReject, typeof(int) },
        { RejectQuantityUnits, typeof(float) },
        { TotalProduction, typeof(float) },
        { ProductionOk, typeof(float) },
        { ProductionNoK, typeof(float) },
    };

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate PlcTagNames logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
