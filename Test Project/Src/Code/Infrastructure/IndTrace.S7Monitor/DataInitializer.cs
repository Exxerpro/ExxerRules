// <copyright file="DataInitializer.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Provides methods for initializing PLC data with predefined configuration and tag definitions.
/// </summary>
public static class DataInitializer
{
    /// <summary>
    /// Initializes and returns a dictionary of PLC data models populated with predefined data.
    /// </summary>
    /// <returns>A dictionary of PLC IDs and their corresponding <see cref="PlcDataModels"/> objects.</returns>
    public static Dictionary<int, PlcDataModels> InitializePlcData()
    {
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate data initializer logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

        // PLC 100
        var plc100 = new PlcDataModels
        {
            PlcId = 100,
            IpAddress = "192.168.0.100",
            Tags =
            [
                new TagMonitor { Description = "PartStatusPlc", Address = "DB257.W0" },
                new TagMonitor { Description = "CycleStatusPlc", Address = "DB257.W2" },
                new TagMonitor { Description = "PartNumber", Address = "DB249.S0.32" },
                new TagMonitor { Description = "BarCode", Address = "DB249.S100.32" },
                new TagMonitor { Description = "PlcId", Address = "DB254.DBW0" },
                new TagMonitor { Description = "HeartBeat", Address = "DB254.DBW2" },
                new TagMonitor { Description = "Command", Address = "DB254.DBW4" },
                new TagMonitor { Description = "CommandFeedback", Address = "DB254.DBW6" },
                new TagMonitor { Description = "ResultValidation", Address = "DB256.DINT0" },
                new TagMonitor { Description = "LastMachineId", Address = "DB256.DINT4" },
                new TagMonitor { Description = "NextMachineId", Address = "DB256.DINT8" },
                new TagMonitor { Description = "CycleStatus", Address = "DB256.DINT12" },
                new TagMonitor { Description = "FlowStatus", Address = "DB256.DINT16" },
                new TagMonitor { Description = "PartStatus", Address = "DB256.DINT20" },
                new TagMonitor { Description = "MachineType", Address = "DB256.DINT24" },
                new TagMonitor { Description = "WorkFlowType", Address = "DB256.DINT28" },
                new TagMonitor { Description = "CyclesOk", Address = "DB256.DINT48" },

                // Manually added Performance tags
                new TagMonitor { Description = "TotalProduction", Address = "DB400.REAL280" },
                new TagMonitor { Description = "ProductionOk", Address = "DB400.REAL320" },
                new TagMonitor { Description = "ProductionNoK", Address = "DB400.REAL360" },
                new TagMonitor { Description = "ApplicationFlag", Address = "DB400.DINT00" },
                new TagMonitor { Description = "RunningTime", Address = "DB400.DINT120" },
                new TagMonitor { Description = "StoppedTime", Address = "DB400.DINT160" },
                new TagMonitor { Description = "FaultedTime", Address = "DB400.DINT200" },
                new TagMonitor { Description = "StatusFaultReason", Address = "DB400.DINT240" },
                new TagMonitor { Description = "EventCounter", Address = "DB400.DINT40" },
                new TagMonitor { Description = "StatusFaultReject", Address = "DB400.DINT400" },
                new TagMonitor { Description = "CurrentTime", Address = "DB400.DINT80" }

            ],
        };

        // PLC 400
        var plc400 = new PlcDataModels
        {
            PlcId = 400,
            IpAddress = "192.168.10.10",
            Tags =
            [
                new TagMonitor { Description = "PartStatusPlc", Address = "DB257.W0" },
                new TagMonitor { Description = "CycleStatusPlc", Address = "DB257.W2" },
                new TagMonitor { Description = "PartNumber", Address = "DB249.S0.32" },
                new TagMonitor { Description = "BarCode", Address = "DB249.S100.32" },
                new TagMonitor { Description = "PlcId", Address = "DB254.DBW0" },
                new TagMonitor { Description = "HeartBeat", Address = "DB254.DBW2" },
                new TagMonitor { Description = "Command", Address = "DB254.DBW4" },
                new TagMonitor { Description = "CommandFeedback", Address = "DB254.DBW6" },
                new TagMonitor { Description = "ResultValidation", Address = "DB256.DINT0" },
                new TagMonitor { Description = "LastMachineId", Address = "DB256.DINT4" },
                new TagMonitor { Description = "NextMachineId", Address = "DB256.DINT8" },
                new TagMonitor { Description = "CycleStatus", Address = "DB256.DINT12" },
                new TagMonitor { Description = "FlowStatus", Address = "DB256.DINT16" },
                new TagMonitor { Description = "PartStatus", Address = "DB256.DINT20" },
                new TagMonitor { Description = "MachineType", Address = "DB256.DINT24" },
                new TagMonitor { Description = "WorkFlowType", Address = "DB256.DINT28" },
                new TagMonitor { Description = "CyclesOk", Address = "DB256.DINT48" }
            ],
        };

        // PLC 500
        var plc500 = new PlcDataModels
        {
            PlcId = 500,
            IpAddress = "192.168.0.30",
            Tags =
            [
                new TagMonitor { Description = "PartStatusPlc", Address = "DB257.W0" },
                new TagMonitor { Description = "CycleStatusPlc", Address = "DB257.W2" },
                new TagMonitor { Description = "PartNumber", Address = "DB249.S0.32" },
                new TagMonitor { Description = "BarCode", Address = "DB249.S100.32" },
                new TagMonitor { Description = "PlcId", Address = "DB254.DBW0" },
                new TagMonitor { Description = "HeartBeat", Address = "DB254.DBW2" },
                new TagMonitor { Description = "Command", Address = "DB254.DBW4" },
                new TagMonitor { Description = "CommandFeedback", Address = "DB254.DBW6" },
                new TagMonitor { Description = "ResultValidation", Address = "DB256.DINT0" },
                new TagMonitor { Description = "LastMachineId", Address = "DB256.DINT4" },
                new TagMonitor { Description = "NextMachineId", Address = "DB256.DINT8" },
                new TagMonitor { Description = "CycleStatus", Address = "DB256.DINT12" },
                new TagMonitor { Description = "FlowStatus", Address = "DB256.DINT16" },
                new TagMonitor { Description = "PartStatus", Address = "DB256.DINT20" },
                new TagMonitor { Description = "MachineType", Address = "DB256.DINT24" },
                new TagMonitor { Description = "WorkFlowType", Address = "DB256.DINT28" },
                new TagMonitor { Description = "CyclesOk", Address = "DB256.DINT48" },
                new TagMonitor { Description = "ValueString Test Current CHMSL", Address = "DB257.D76" },
                new TagMonitor { Description = "ValueTest Current SPOILER", Address = "DB257.D80" },
                new TagMonitor { Description = "Test Current CHMSL", Address = "DB257.X10.2" },
                new TagMonitor { Description = "Test Current SPOILER", Address = "DB257.X10.3" },
                new TagMonitor { Description = "CyclesOk", Address = "DB256.DINT48" },
                new TagMonitor { Description = "ShiftOk", Address = "DB256.DINT52" },
                new TagMonitor { Description = "Leak Test", Address = "DB257.X10.4" },
                new TagMonitor { Description = "Test Cam Ventpatch", Address = "DB257.X10.5" },
                new TagMonitor { Description = "ValueString LED CHMSL A 1", Address = "DB257.DBW300" },
                new TagMonitor { Description = "ValueString LED CHMSL A 2", Address = "DB257.DBW302" },
                new TagMonitor { Description = "ValueString LED CHMSL A 3", Address = "DB257.DBW304" },
                new TagMonitor { Description = "ValueString LED CHMSL A 4", Address = "DB257.DBW306" },
                new TagMonitor { Description = "ValueString LED CHMSL A 5", Address = "DB257.DBW308" },
                new TagMonitor { Description = "ValueString LED CHMSL A 6", Address = "DB257.DBW310" },
                new TagMonitor { Description = "ValueString LED CHMSL A 7", Address = "DB257.DBW312" },
                new TagMonitor { Description = "ValueString LED CHMSL A 8", Address = "DB257.DBW314" },
                new TagMonitor { Description = "ValueString LED CHMSL A 9", Address = "DB257.DBW316" },
                new TagMonitor { Description = "ValueString LED CHMSL B 10", Address = "DB257.DBW318" },
                new TagMonitor { Description = "ValueString LED CHMSL B 11", Address = "DB257.DBW320" },
                new TagMonitor { Description = "ValueString LED CHMSL B 12  ", Address = "DB257.DBW322" },
                new TagMonitor { Description = "ValueString LED CHMSL B 13  ", Address = "DB257.DBW324" },
                new TagMonitor { Description = "ValueString LED CHMSL B 14  ", Address = "DB257.DBW326" },
                new TagMonitor { Description = "ValueString LED CHMSL B 15  ", Address = "DB257.DBW328" },
                new TagMonitor { Description = "ValueString LED CHMSL B 16  ", Address = "DB257.DBW330" },
                new TagMonitor { Description = "ValueString LED CHMSL B 17  ", Address = "DB257.DBW332" },
                new TagMonitor { Description = "ValueString LED SPOILER A 1 ", Address = "DB257.DBW336" },
                new TagMonitor { Description = "ValueString LED SPOILER A 2 ", Address = "DB257.DBW338" },
                new TagMonitor { Description = "ValueString LED SPOILER A 3 ", Address = "DB257.DBW340" },
                new TagMonitor { Description = " ValueString LED SPOILER A 4", Address = "DB257.DBW342" },
                new TagMonitor { Description = " ValueString LED SPOILER A 5", Address = "DB257.DBW344" },
                new TagMonitor { Description = " ValueString LED SPOILER A 6", Address = "DB257.DBW346" },
                new TagMonitor { Description = " ValueString LED SPOILER A 7", Address = "DB257.DBW348" },
                new TagMonitor { Description = " ValueString LED SPOILER A 8", Address = "DB257.DBW350" },
                new TagMonitor { Description = "ValueString LED SPOILER A 9 ", Address = "DB257.DBW352" },
                new TagMonitor { Description = "ValueString LED SPOILER B 10", Address = "DB257.DBW354" },
                new TagMonitor { Description = "ValueString LED SPOILER B 11", Address = "DB257.DBW356" },
                new TagMonitor { Description = "ValueString LED SPOILER B 12", Address = "DB257.DBW358" },
                new TagMonitor { Description = "ValueString LED SPOILER B 13", Address = "DB257.DBW360" },
                new TagMonitor { Description = "ValueString LED SPOILER B 14", Address = "DB257.DBW362" },
                new TagMonitor { Description = "ValueString LED SPOILER B 15", Address = "DB257.DBW364" },
                new TagMonitor { Description = "ValueString LED SPOILER B 16", Address = "DB257.DBW366" },
                new TagMonitor { Description = "ValueString LED SPOILER B 17", Address = "DB257.DBW368" },
                new TagMonitor { Description = "ValueString LED SPOILER B 18", Address = "DB257.DBW370" },
                new TagMonitor { Description = "ValueString LED SPOILER B 19", Address = "DB257.DBW372" },
                new TagMonitor { Description = "ValueString LED SPOILER C 20", Address = "DB257.DBW374" },
                new TagMonitor { Description = "ValueString LED SPOILER C 21", Address = "DB257.DBW376" },
                new TagMonitor { Description = "ValueString LED SPOILER C 22", Address = "DB257.DBW378" },
                new TagMonitor { Description = "ValueString LED SPOILER C 23", Address = "DB257.DBW380" },
                new TagMonitor { Description = "ValueString LED SPOILER C 24", Address = "DB257.DBW382" },
                new TagMonitor { Description = "ValueString LED SPOILER C 25", Address = "DB257.DBW384" },
                new TagMonitor { Description = "ValueString LED SPOILER C 26", Address = "DB257.DBW386" },
                new TagMonitor { Description = "ValueString LED SPOILER C 27", Address = "DB257.DBW388" },
                new TagMonitor { Description = "ValueString LED SPOILER C 28", Address = "DB257.DBW390" },
                new TagMonitor { Description = "ValueString LED SPOILER C 29", Address = "DB257.DBW392" },
                new TagMonitor { Description = "ValueString LED SPOILER D 30", Address = "DB257.DBW394" },
                new TagMonitor { Description = "ValueString LED SPOILER D 31", Address = "DB257.DBW396" },
                new TagMonitor { Description = "ValueString LED SPOILER D 32", Address = "DB257.DBW398" },
                new TagMonitor { Description = "ValueString LED SPOILER D 33", Address = "DB257.DBW400" },
                new TagMonitor { Description = "ValueString LED SPOILER D 34", Address = "DB257.DBW402" },
                new TagMonitor { Description = "ValueString LED SPOILER D 35", Address = "DB257.DBW404" },
                new TagMonitor { Description = "ValueString LED SPOILER D 36", Address = "DB257.DBW406" },
                new TagMonitor { Description = "ValueString LED SPOILER D 37", Address = "DB257.DBW408" },
                new TagMonitor { Description = "ValueString LED SPOILER D 38", Address = "DB257.DBW410" },
                new TagMonitor { Description = "ValueString LED SPOILER D 39", Address = "DB257.DBW412" },
                new TagMonitor { Description = "ValueString LED SPOILER E 40", Address = "DB257.DBW414" },

            ],
        };

        // List of PLCs
        var plcs = new Dictionary<int, PlcDataModels>
        {
            { plc100.PlcId, plc100 },
        };

        // plcs.Add(plc400.PlcId, plc400);
        // plcs.Add(plc500.PlcId, plc500);
        return plcs;
    }
}
