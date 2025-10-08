using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for MachinePlc entities with O(1) lookup.
/// Generated with ImmutableDictionary for thread-safety and performance.
/// Implements lazy-loaded Dict for best of both worlds: O(1) lookups + List compatibility.
/// Maps machines to PLCs for communication configuration.
/// </summary>
internal static class MachinePlcRawData
{
    /// <summary>
    /// MachinePlc test data mapping machines to their PLCs
    /// </summary>
    private static readonly ImmutableDictionary<int, MachinePlc> _machinePlcsDict =
        new Dictionary<int, MachinePlc>
        {
            // MachineId 100 -> PlcId 100
            [100] = new MachinePlc
            {
                MachineId = 100,
                PlcId = 100,
                IsActive = 1,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 13),
                ModifiedBy = "0",
                ModifiedOn = new DateTime(2020, 6, 26, 12, 9, 37)
            },

            // MachineId 200 -> PlcId 200
            [200] = new MachinePlc
            {
                MachineId = 200,
                PlcId = 200,
                IsActive = 1,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 28, 17, 2, 13),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 8, 28, 17, 2, 13)
            },

            // MachineId 300 -> PlcId 300
            [300] = new MachinePlc
            {
                MachineId = 300,
                PlcId = 300,
                IsActive = 1,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 28, 17, 2, 13),
                ModifiedBy = "Admin",
                ModifiedOn = new DateTime(2024, 8, 28, 17, 2, 13)
            },

            // MachineId 400 -> PlcId 400
            [400] = new MachinePlc
            {
                MachineId = 400,
                PlcId = 400,
                IsActive = 1,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 13),
                ModifiedBy = "0",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 9, 37)
            },

            // MachineId 500 -> PlcId 500
            [500] = new MachinePlc
            {
                MachineId = 500,
                PlcId = 500,
                IsActive = 1,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2023, 8, 28, 17, 2, 13),
                ModifiedBy = "0",
                ModifiedOn = new DateTime(2023, 8, 25, 12, 9, 37)
            },

            // MachineId 600 -> PlcId 600
            [600] = new MachinePlc
            {
                MachineId = 600,
                PlcId = 600,
                IsActive = 1,
                CreatedBy = "Exxerpro",
                CreatedOn = new DateTime(2024, 7, 22, 16, 24, 6),
                ModifiedBy = "Exxerpro",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 24, 6)
            },

            // MachineId 700 -> PlcId 700
            [700] = new MachinePlc
            {
                MachineId = 700,
                PlcId = 700,
                IsActive = 1,
                CreatedBy = "Exxerpro",
                CreatedOn = new DateTime(2024, 7, 22, 16, 24, 6),
                ModifiedBy = "Exxerpro",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 24, 6)
            },

            // MachineId 800 -> PlcId 800
            [800] = new MachinePlc
            {
                MachineId = 800,
                PlcId = 800,
                IsActive = 1,
                CreatedBy = "Exxerpro",
                CreatedOn = new DateTime(2024, 7, 22, 16, 24, 6),
                ModifiedBy = "Exxerpro",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 24, 6)
            },

            // MachineId 900 -> PlcId 900
            [900] = new MachinePlc
            {
                MachineId = 900,
                PlcId = 900,
                IsActive = 1,
                CreatedBy = "Exxerpro",
                CreatedOn = new DateTime(2024, 7, 22, 16, 24, 6),
                ModifiedBy = "Exxerpro",
                ModifiedOn = new DateTime(2024, 7, 22, 16, 24, 6)
            }
        }.ToImmutableDictionary();

    /// <summary>
    /// Lazy-loaded cached list for maximum performance - best of both worlds
    /// </summary>
    private static readonly Lazy<IReadOnlyList<MachinePlc>> _fixtureCache =
        new(() => _machinePlcsDict.Values.ToList());

    /// <summary>
    /// Get all MachinePlc entities (cached List from dictionary for backward compatibility)
    /// </summary>
    public static IReadOnlyList<MachinePlc> Fixture => _fixtureCache.Value;

    /// <summary>
    /// Get a specific MachinePlc by MachineId - O(1) lookup (standardized pattern)
    /// </summary>
    public static MachinePlc? GetByMachineId(int machineId) =>
        _machinePlcsDict.TryGetValue(machineId, out var machinePlc) ? machinePlc : null;

    /// <summary>
    /// Direct dictionary access for advanced scenarios (standardized pattern)
    /// </summary>
    public static IImmutableDictionary<int, MachinePlc> Dictionary => _machinePlcsDict;

    /// <summary>
    /// Check if a MachinePlc exists by MachineId - O(1) lookup
    /// </summary>
    public static bool Contains(int machineId) => _machinePlcsDict.ContainsKey(machineId);

    /// <summary>
    /// Get count of MachinePlcs - O(1) operation
    /// </summary>
    public static int Count => _machinePlcsDict.Count;

    /// <summary>
    /// Get MachinePlc by PlcId - O(n) operation
    /// </summary>
    public static MachinePlc? GetByPlcId(int plcId) =>
        _machinePlcsDict.Values.FirstOrDefault(mp => mp.PlcId == plcId);

    /// <summary>
    /// Get all active MachinePlcs - O(n) operation
    /// </summary>
    public static IEnumerable<MachinePlc> GetActive() =>
        _machinePlcsDict.Values.Where(mp => mp.IsActive == 1);
}
