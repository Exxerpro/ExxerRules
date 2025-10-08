using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities;
using IndTrace.TestData.Attributes;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for Variable PLC100 entities with O(1) lookup.
/// IMPORTED: Contains all 42 entities from VariablesPlc100.json
/// Generated on: 2025-09-03 06:03:00
/// </summary>
[Release("V2 loader infrastructure")]
internal static class VariablePlc100RawData
{
    private static readonly ImmutableDictionary<int, Variable> _variablesDict =
        new Dictionary<int, Variable>
        {
            [40] = new Variable
            {
                VariableId = 40,
                MachineId = 100,
                PlcId = 100,
                Name = "PartStatusPlc",
                Description = "PartStatusPlc",
                Alias = "DB40.W0",
                Address = "DB40.W0",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [41] = new Variable
            {
                VariableId = 41,
                MachineId = 100,
                PlcId = 100,
                Name = "CycleStatusPlc",
                Description = "CycleStatusPlc",
                Alias = "DB40.W2",
                Address = "DB40.W2",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [42] = new Variable
            {
                VariableId = 42,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorInt1\n",
                Description = "ValorInt1\n",
                Alias = "DB40.W12",
                Address = "DB40.W12",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [43] = new Variable
            {
                VariableId = 43,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorInt2",
                Description = "ValorInt2",
                Alias = "DB40.W14",
                Address = "DB40.W14",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [44] = new Variable
            {
                VariableId = 44,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorInt3",
                Description = "ValorInt3",
                Alias = "DB40.W16",
                Address = "DB40.W16",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [45] = new Variable
            {
                VariableId = 45,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal1\n",
                Description = "ValorReal1\n",
                Alias = "DB40.D60",
                Address = "DB40.D60",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [46] = new Variable
            {
                VariableId = 46,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal2\n",
                Description = "ValorReal2\n",
                Alias = "DB40.D64",
                Address = "DB40.D64",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [47] = new Variable
            {
                VariableId = 47,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal3\n",
                Description = "ValorReal3\n",
                Alias = "DB40.D68",
                Address = "DB40.D68",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [48] = new Variable
            {
                VariableId = 48,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal4\n",
                Description = "ValorReal4\n",
                Alias = "DB40.D72",
                Address = "DB40.D72",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [49] = new Variable
            {
                VariableId = 49,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal5\n",
                Description = "ValorReal5\n",
                Alias = "DB40.D76",
                Address = "DB40.D76",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [50] = new Variable
            {
                VariableId = 50,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal6\n",
                Description = "ValorReal6\n",
                Alias = "DB40.D80",
                Address = "DB40.D80",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [51] = new Variable
            {
                VariableId = 51,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal6\n",
                Description = "ValorReal6\n",
                Alias = "DB40.D80",
                Address = "DB40.D80",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [52] = new Variable
            {
                VariableId = 52,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal7\n",
                Description = "ValorReal7\n",
                Alias = "DB40.D84",
                Address = "DB40.D84",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [53] = new Variable
            {
                VariableId = 53,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal8\n",
                Description = "ValorReal8\n",
                Alias = "DB40.D88",
                Address = "DB40.D88",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [54] = new Variable
            {
                VariableId = 54,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal8\n",
                Description = "ValorReal8\n",
                Alias = "DB40.D88",
                Address = "DB40.D88",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [55] = new Variable
            {
                VariableId = 55,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal9\n",
                Description = "ValorReal9\n",
                Alias = "DB40.D92",
                Address = "DB40.D92",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [56] = new Variable
            {
                VariableId = 56,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorReal10",
                Description = "ValorReal10",
                Alias = "DB40.D96",
                Address = "DB40.D96",
                NetType = "System.Single",
                NativeType = "Production",
            },
            [57] = new Variable
            {
                VariableId = 57,
                MachineId = 100,
                PlcId = 100,
                Name = "StatusPrueba1\n",
                Description = "StatusPrueba1\n",
                Alias = "DB40.X10.0",
                Address = "DB40.X10.0",
                NetType = "System.Boolean",
                NativeType = "Production",
            },
            [58] = new Variable
            {
                VariableId = 58,
                MachineId = 100,
                PlcId = 100,
                Name = "Component Id BarCode\n",
                Description = "Component Id BarCode\n",
                Alias = "DB40.S100",
                Address = "DB40.S100",
                NetType = "System.String",
                NativeType = "Production",
            },
            [59] = new Variable
            {
                VariableId = 59,
                MachineId = 100,
                PlcId = 100,
                Name = "StatusPrueba2\n",
                Description = "StatusPrueba2\n",
                Alias = "DB40.X10.1",
                Address = "DB40.X10.1",
                NetType = "System.Boolean",
                NativeType = "Production",
            },
            [60] = new Variable
            {
                VariableId = 60,
                MachineId = 100,
                PlcId = 100,
                Name = "StatusPrueba3\n",
                Description = "StatusPrueba3\n",
                Alias = "DB40.X10.2",
                Address = "DB40.X10.2",
                NetType = "System.Boolean",
                NativeType = "Production",
            },
            [61] = new Variable
            {
                VariableId = 61,
                MachineId = 100,
                PlcId = 100,
                Name = "StatusPrueba4\n",
                Description = "StatusPrueba4\n",
                Alias = "DB40.X10.3",
                Address = "DB40.X10.3",
                NetType = "System.Boolean",
                NativeType = "Production",
            },
            [62] = new Variable
            {
                VariableId = 62,
                MachineId = 100,
                PlcId = 100,
                Name = "StatusPrueba5\n",
                Description = "StatusPrueba5\n",
                Alias = "DB40.X10.4",
                Address = "DB40.X10.4",
                NetType = "System.Boolean",
                NativeType = "Production",
            },
            [63] = new Variable
            {
                VariableId = 63,
                MachineId = 100,
                PlcId = 100,
                Name = "ValorString2\n",
                Description = "ValorString2\n",
                Alias = "DB40.S150",
                Address = "DB40.S150",
                NetType = "System.String",
                NativeType = "Production",
            },
            [64] = new Variable
            {
                VariableId = 64,
                MachineId = 100,
                PlcId = 100,
                Name = "PartNumber",
                Description = "PartNumber ",
                Alias = "DB41.S0",
                Address = "DB41.S0",
                NetType = "System.String",
                NativeType = "Production",
            },
            [65] = new Variable
            {
                VariableId = 65,
                MachineId = 100,
                PlcId = 100,
                Name = "BarCode",
                Description = "BarCode",
                Alias = "DB41.S100",
                Address = "DB41.S100",
                NetType = "System.String",
                NativeType = "Production",
            },
            [66] = new Variable
            {
                VariableId = 66,
                MachineId = 100,
                PlcId = 100,
                Name = "PlcId",
                Description = "PlcId",
                Alias = "DB42.DBW0",
                Address = "DB42.DBW0",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [67] = new Variable
            {
                VariableId = 67,
                MachineId = 100,
                PlcId = 100,
                Name = "HeartBeat",
                Description = "HeartBeat",
                Alias = "DB42.DBW2",
                Address = "DB42.DBW2",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [68] = new Variable
            {
                VariableId = 68,
                MachineId = 100,
                PlcId = 100,
                Name = "Command",
                Description = "Command",
                Alias = "DB42.DBW4",
                Address = "DB42.DBW4",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [69] = new Variable
            {
                VariableId = 69,
                MachineId = 100,
                PlcId = 100,
                Name = "CommandFeedback",
                Description = "CommandFeedback",
                Alias = "DB42.DBW6",
                Address = "DB42.DBW6",
                NetType = "System.Int16",
                NativeType = "Production",
            },
            [70] = new Variable
            {
                VariableId = 70,
                MachineId = 100,
                PlcId = 100,
                Name = "ResultValidation",
                Description = "ResultValidation",
                Alias = "DB43.DINT0",
                Address = "DB43.DINT0",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [71] = new Variable
            {
                VariableId = 71,
                MachineId = 100,
                PlcId = 100,
                Name = "LastMachineId",
                Description = "LastMachineId",
                Alias = "DB43.DINT4",
                Address = "DB43.DINT4",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [72] = new Variable
            {
                VariableId = 72,
                MachineId = 100,
                PlcId = 100,
                Name = "NextMachineId",
                Description = "NextMachineId",
                Alias = "DB43.DINT8",
                Address = "DB43.DINT8",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [73] = new Variable
            {
                VariableId = 73,
                MachineId = 100,
                PlcId = 100,
                Name = "CycleStatus",
                Description = "CycleStatus",
                Alias = "DB43.DINT12",
                Address = "DB43.DINT12",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [74] = new Variable
            {
                VariableId = 74,
                MachineId = 100,
                PlcId = 100,
                Name = "FlowStatus",
                Description = "FlowStatus",
                Alias = "DB43.DINT16",
                Address = "DB43.DINT16",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [75] = new Variable
            {
                VariableId = 75,
                MachineId = 100,
                PlcId = 100,
                Name = "PartStatus",
                Description = "PartStatus",
                Alias = "DB43.DINT20",
                Address = "DB43.DINT20",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [76] = new Variable
            {
                VariableId = 76,
                MachineId = 100,
                PlcId = 100,
                Name = "MachineType",
                Description = "MachineType",
                Alias = "DB43.DINT24",
                Address = "DB43.DINT24",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [77] = new Variable
            {
                VariableId = 77,
                MachineId = 100,
                PlcId = 100,
                Name = "WorkFlowType",
                Description = "WorkFlowType",
                Alias = "DB43.DINT28",
                Address = "DB43.DINT28",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [78] = new Variable
            {
                VariableId = 78,
                MachineId = 100,
                PlcId = 100,
                Name = "ActualStation",
                Description = "ActualStation",
                Alias = "DB43.DINT32",
                Address = "DB43.DINT32",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [79] = new Variable
            {
                VariableId = 79,
                MachineId = 100,
                PlcId = 100,
                Name = "BarCodeId",
                Description = "BarCodeId",
                Alias = "DB43.DINT36",
                Address = "DB43.DINT36",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [80] = new Variable
            {
                VariableId = 80,
                MachineId = 100,
                PlcId = 100,
                Name = "CycleId",
                Description = "CycleId",
                Alias = "DB43.DINT40",
                Address = "DB43.DINT40",
                NetType = "System.Int32",
                NativeType = "Production",
            },
            [81] = new Variable
            {
                VariableId = 81,
                MachineId = 100,
                PlcId = 100,
                Name = "RegistersSaved",
                Description = "RegistersSaved",
                Alias = "DB43.DINT44",
                Address = "DB43.DINT44",
                NetType = "System.Int32",
                NativeType = "Production",
            }
        }.ToImmutableDictionary();

    private static readonly Lazy<IReadOnlyList<Variable>> _fixtureCache =
        new(() => _variablesDict.Values.ToList());

    public static IReadOnlyList<Variable> Fixture => _fixtureCache.Value;

    public static Variable? GetById(int id) => _variablesDict.TryGetValue(id, out var variable) ? variable : null;

    public static Variable? GetVariable(int id) => GetById(id);

    public static IImmutableDictionary<int, Variable> Dictionary => _variablesDict;

    public static bool Contains(int id) => _variablesDict.ContainsKey(id);

    public static int Count => _variablesDict.Count;
}
