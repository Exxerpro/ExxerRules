using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IndTrace.Domain.Entities;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for Plc entities with O(1) lookup.
/// IMPORTED: Contains all 1 entities from Plc100.json
/// Generated on: 2025-09-03 06:03:46
/// </summary>
internal static class PlcRawData
{
    private static readonly ImmutableDictionary<int, Plc> _plcsDict =
        new Dictionary<int, Plc>
        {
            [100] = new Plc
            {
                PlcId = 100,
                Name = "S7-1200",
                IpAddress = "192.168.0.100",
                PlcType = "S7-1200",
                PlcBrand = "Siemens",
                Options = " [\n  {\n  \"Rack\": 0,\n  \"Slot\": 1,\n  \"TSAP\" : \"FD.01\"\n  }\n  ] ",
                CommLibrary = "S7-Link",
                BrandOwner = "Siemens"
            }
        }.ToImmutableDictionary();

    private static readonly Lazy<IReadOnlyList<Plc>> _fixtureCache =
        new(() => _plcsDict.Values.ToList());

    public static IReadOnlyList<Plc> Fixture => _fixtureCache.Value;
    public static Plc? GetById(int id) => _plcsDict.TryGetValue(id, out var plc) ? plc : null;
    public static Plc? GetPlc(int id) => GetById(id);
    public static IImmutableDictionary<int, Plc> Dictionary => _plcsDict;
    public static bool Contains(int id) => _plcsDict.ContainsKey(id);
    public static int Count => _plcsDict.Count;
}
