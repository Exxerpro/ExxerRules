using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for Register entities with O(1) lookup.
/// IMPORTED: Contains all 2 entities from RegisterPlc100.json
/// Generated on: 2025-09-03 06:01:53
/// </summary>
internal static class RegisterPlc100RawData
{
    private static readonly ImmutableDictionary<int, Register> _registersDict =
        new Dictionary<int, Register>
        {
            [5512] = new Register
            {
                RegisterId = 5512,
                Name = "PartStatusPlc",
                VariableId = 121,
                CycleId = 1184,
                Value = "1",
                DataType = "System.Int16",
                StatusValueId = 1
            },
            [5554] = new Register
            {
                RegisterId = 5554,
                Name = "CycleStatusPlc",
                VariableId = 122,
                CycleId = 1195,
                Value = "4",
                DataType = "System.Int16",
                StatusValueId = 1
            }
        }.ToImmutableDictionary();

    private static readonly Lazy<IReadOnlyList<Register>> _fixtureCache =
        new(() => _registersDict.Values.ToList());

    public static IReadOnlyList<Register> Fixture => _fixtureCache.Value;

    public static Register? GetById(int id) => _registersDict.TryGetValue(id, out var register) ? register : null;

    public static Register? GetRegister(int id) => GetById(id);

    public static IImmutableDictionary<int, Register> Dictionary => _registersDict;

    public static bool Contains(int id) => _registersDict.ContainsKey(id);

    public static int Count => _registersDict.Count;
}
