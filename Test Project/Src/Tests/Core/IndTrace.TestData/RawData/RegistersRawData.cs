using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for Register entities with O(1) lookup.
/// RELATIONAL: Generated from BarCode→Cycles relational cascade.
/// Generated with ImmutableDictionary for thread-safety and performance.
/// Implements lazy-loaded Dict for best of both worlds: O(1) lookups + List compatibility.
/// </summary>
internal static class RegistersRawData
{
    /// <summary>
    /// Relational test data for registers linked to test Cycles
    /// </summary>
    private static readonly ImmutableDictionary<int, Register> _registersDict =
        new Dictionary<int, Register>
        {
            [1] = new Register
            {
                RegisterId = 1,
                Name = "REG_0001",
                Description = "Test register 1",
                MachineId = 100,
                VariableId = 1,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [2] = new Register
            {
                RegisterId = 2,
                Name = "REG_0002",
                Description = "Test register 2",
                MachineId = 100,
                VariableId = 2,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [3] = new Register
            {
                RegisterId = 3,
                Name = "REG_0003",
                Description = "Test register 3",
                MachineId = 100,
                VariableId = 3,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [4] = new Register
            {
                RegisterId = 4,
                Name = "REG_0004",
                Description = "Test register 4",
                MachineId = 100,
                VariableId = 4,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [5] = new Register
            {
                RegisterId = 5,
                Name = "REG_0005",
                Description = "Test register 5",
                MachineId = 100,
                VariableId = 5,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [10] = new Register
            {
                RegisterId = 10,
                Name = "REG_0010",
                Description = "Test register 10",
                MachineId = 100,
                VariableId = 10,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [11] = new Register
            {
                RegisterId = 11,
                Name = "REG_0011",
                Description = "Test register 11",
                MachineId = 100,
                VariableId = 11,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [12] = new Register
            {
                RegisterId = 12,
                Name = "REG_0012",
                Description = "Test register 12",
                MachineId = 100,
                VariableId = 12,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [15] = new Register
            {
                RegisterId = 15,
                Name = "REG_0015",
                Description = "Test register 15",
                MachineId = 100,
                VariableId = 15,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [20] = new Register
            {
                RegisterId = 20,
                Name = "REG_0020",
                Description = "Test register 20",
                MachineId = 100,
                VariableId = 20,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },

            [25] = new Register
            {
                RegisterId = 25,
                Name = "REG_0025",
                Description = "Test register 25",
                MachineId = 100,
                VariableId = 25,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [30] = new Register
            {
                RegisterId = 30,
                Name = "REG_0030",
                Description = "Test register 30",
                MachineId = 100,
                VariableId = 30,
                CycleId = 1,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [100] = new Register
            {
                RegisterId = 100,
                Name = "Original Complex Entity",
                VariableId = 100,
                CycleId = 1000,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            },
            [480] = new Register
            {
                RegisterId = 480,
                Name = "REG_0480",
                Description = "Test register 480",
                MachineId = 300,
                VariableId = 480,
                CycleId = 92,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [481] = new Register
            {
                RegisterId = 481,
                Name = "REG_0481",
                Description = "Test register 481",
                MachineId = 300,
                VariableId = 481,
                CycleId = 92,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [482] = new Register
            {
                RegisterId = 482,
                Name = "REG_0482",
                Description = "Test register 482",
                MachineId = 300,
                VariableId = 482,
                CycleId = 92,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            // Bridge entries to align RawData with canonical JSON register names used by tests
            [20001] = new Register
            {
                RegisterId = 20001,
                Name = "PartStatusPlc",
                Description = "PartStatusPlc",
                MachineId = 100,
                VariableId = 1,
                CycleId = 1,
                Value = "1",
                DataType = "System.Int16",
                StatusValueId = 1,
                TimeStamp = new DateTime(2023, 8, 27, 0, 50, 34, DateTimeKind.Utc)
            },
            [20002] = new Register
            {
                RegisterId = 20002,
                Name = "CycleStatusPlc",
                Description = "CycleStatusPlc",
                MachineId = 100,
                VariableId = 2,
                CycleId = 1,
                Value = "2",
                DataType = "System.Int16",
                StatusValueId = 1,
                TimeStamp = new DateTime(2023, 8, 27, 0, 50, 34, DateTimeKind.Utc)
            },
            [20003] = new Register
            {
                RegisterId = 20003,
                Name = "PartStatusPlc",
                Description = "PartStatusPlc",
                MachineId = 400,
                VariableId = 1,
                CycleId = 1,
                Value = "1",
                DataType = "System.Int16",
                StatusValueId = 1,
                TimeStamp = new DateTime(2023, 8, 27, 0, 50, 34, DateTimeKind.Utc)
            },
            [20004] = new Register
            {
                RegisterId = 20004,
                Name = "CycleStatusPlc",
                Description = "CycleStatusPlc",
                MachineId = 400,
                VariableId = 2,
                CycleId = 1,
                Value = "2",
                DataType = "System.Int16",
                StatusValueId = 1,
                TimeStamp = new DateTime(2023, 8, 27, 0, 50, 34, DateTimeKind.Utc)
            },
            [555] = new Register
            {
                RegisterId = 555,
                Name = "Original Complex Entity",
                VariableId = 555,
                CycleId = 555,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            },
            [666] = new Register
            {
                RegisterId = 666,
                Name = "Original Complex Entity",
                VariableId = 666,
                CycleId = 666,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            },
            [777] = new Register
            {
                RegisterId = 777,
                Name = "Original Complex Entity",
                VariableId = 777,
                CycleId = 777,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            },
            [888] = new Register
            {
                RegisterId = 888,
                Name = "Exception Test",
                VariableId = 888,
                CycleId = 888,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            },
            [920] = new Register
            {
                RegisterId = 920,
                Name = "REG_0920",
                Description = "Test register 920",
                MachineId = 500,
                VariableId = 920,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [921] = new Register
            {
                RegisterId = 921,
                Name = "REG_0921",
                Description = "Test register 921",
                MachineId = 500,
                VariableId = 921,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [922] = new Register
            {
                RegisterId = 922,
                Name = "REG_0922",
                Description = "Test register 922",
                MachineId = 500,
                VariableId = 922,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1000] = new Register
            {
                RegisterId = 1000,
                Name = "Original Complex Entity",
                VariableId = 1000,
                CycleId = 1000,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            },
            [1090] = new Register
            {
                RegisterId = 1090,
                Name = "REG_1090",
                Description = "Test register 1090",
                MachineId = 500,
                VariableId = 1090,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1091] = new Register
            {
                RegisterId = 1091,
                Name = "REG_1091",
                Description = "Test register 1091",
                MachineId = 500,
                VariableId = 1091,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1092] = new Register
            {
                RegisterId = 1092,
                Name = "REG_1092",
                Description = "Test register 1092",
                MachineId = 500,
                VariableId = 1092,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1360] = new Register
            {
                RegisterId = 1360,
                Name = "REG_1360",
                Description = "Test register 1360",
                MachineId = 500,
                VariableId = 1360,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1361] = new Register
            {
                RegisterId = 1361,
                Name = "REG_1361",
                Description = "Test register 1361",
                MachineId = 500,
                VariableId = 1361,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1362] = new Register
            {
                RegisterId = 1362,
                Name = "REG_1362",
                Description = "Test register 1362",
                MachineId = 500,
                VariableId = 1362,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },

            [1550] = new Register
            {
                RegisterId = 1550,
                Name = "REG_1550",
                Description = "Test register 1550",
                MachineId = 500,
                VariableId = 1550,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1551] = new Register
            {
                RegisterId = 1551,
                Name = "REG_1551",
                Description = "Test register 1551",
                MachineId = 500,
                VariableId = 1551,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1552] = new Register
            {
                RegisterId = 1552,
                Name = "REG_1552",
                Description = "Test register 1552",
                MachineId = 500,
                VariableId = 1552,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },

            [1520] = new Register
            {
                RegisterId = 1520,
                Name = "REG_1520",
                Description = "Test register 1520",
                MachineId = 500,
                VariableId = 1520,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1521] = new Register
            {
                RegisterId = 1521,
                Name = "REG_1521",
                Description = "Test register 1521",
                MachineId = 500,
                VariableId = 1521,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [1522] = new Register
            {
                RegisterId = 1522,
                Name = "REG_1522",
                Description = "Test register 1522",
                MachineId = 500,
                VariableId = 1522,
                CycleId = 152,
                Value = "0",
                DataType = "INT",
                StatusValueId = 1,
                TimeStamp = DateTime.UtcNow
            },
            [999999] = new Register
            {
                RegisterId = 999999,
                Name = "Original Complex Entity",
                VariableId = 100,
                CycleId = 1000,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            },
            [2147483646] = new Register
            {
                RegisterId = 2147483646,
                Name = "Original Complex Entity",
                VariableId = 100,
                CycleId = 1000,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            },
        }.ToImmutableDictionary();

    /// <summary>
    /// Lazy-loaded cached list for maximum performance - best of both worlds
    /// </summary>
    private static readonly Lazy<IReadOnlyList<Register>> _fixtureCache =
        new(() => _registersDict.Values.ToList());

    /// <summary>
    /// Get all Register entities (cached List from dictionary for backward compatibility)
    /// </summary>
    public static IReadOnlyList<Register> Fixture => _fixtureCache.Value;

    /// <summary>
    /// Get a specific Register by ID - O(1) lookup (standardized pattern)
    /// </summary>
    public static Register? GetById(int id) =>
        _registersDict.TryGetValue(id, out var register) ? register : null;

    /// <summary>
    /// Get a specific Register by ID - O(1) lookup (backward compatibility)
    /// </summary>
    public static Register? GetRegister(int id) => GetById(id);

    /// <summary>
    /// Direct dictionary access for advanced scenarios (standardized pattern)
    /// </summary>
    public static IImmutableDictionary<int, Register> Dictionary => _registersDict;

    /// <summary>
    /// Direct dictionary access for advanced scenarios (backward compatibility)
    /// </summary>
    public static IImmutableDictionary<int, Register> Registers => _registersDict;

    /// <summary>
    /// Check if a Register exists by ID - O(1) lookup
    /// </summary>
    public static bool Contains(int id) => _registersDict.ContainsKey(id);

    /// <summary>
    /// Get count of Registers - O(1) operation
    /// </summary>
    public static int Count => _registersDict.Count;

    /// <summary>
    /// Get Register by MachineId - O(n) operation
    /// </summary>
    public static IEnumerable<Register> GetByMachineId(int machineId) =>
        _registersDict.Values.Where(r => r.MachineId == machineId);

    /// <summary>
    /// Get Register by VariableId - O(n) operation
    /// </summary>
    public static IEnumerable<Register> GetByVariableId(int variableId) =>
        _registersDict.Values.Where(r => r.VariableId == variableId);

    /// <summary>
    /// Get Register by CycleId - O(n) operation
    /// </summary>
    public static IEnumerable<Register> GetByCycleId(int cycleId) =>
        _registersDict.Values.Where(r => r.CycleId == cycleId);

    /// <summary>
    /// Get Register by Name - O(n) operation
    /// </summary>
    public static Register? GetByName(string name) =>
        _registersDict.Values.FirstOrDefault(r => r.Name == name);
}
