using IndTrace.Domain.Entities;
using IndTrace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace IndTrace.TestData.RawData;

/// <summary>
/// Static test data for KpiOee entities with O(1) lookup.
/// Generated with ImmutableDictionary for thread-safety and performance.
/// Implements lazy-loaded Dict for best of both worlds: O(1) lookups + List compatibility.
/// </summary>
internal static class KpiOeeRawData
{
    /// <summary>
    /// KpiOee test data - Manufacturing KPI OEE metrics
    /// </summary>
    private static readonly ImmutableDictionary<int, KpiOee> _kpiOeesDict =
        new Dictionary<int, KpiOee>
        {
            [1] = new KpiOee
            {
                KpiOeeId = 1,
                OeeRegisterId = 1,
                TimeStamp = DateTime.UtcNow.AddHours(-1),
                Oee = Math.Round(0.85, 6), // Sanitized to 6 decimal places
                Availability = Math.Round(0.90, 6),
                Performance = Math.Round(0.94, 6),
                Quality = Math.Round(0.95, 6),
                // Navigation property will be set by EF Core
                OeeRegister = null!
            },
            [2] = new KpiOee
            {
                KpiOeeId = 2,
                OeeRegisterId = 2,
                TimeStamp = DateTime.UtcNow.AddHours(-2),
                Oee = Math.Round(0.82, 6),
                Availability = Math.Round(0.90, 6),
                Performance = Math.Round(1.0, 6), // Clamped to max 1.0 for KPI
                Quality = Math.Round(0.94, 6),
                OeeRegister = null!
            },
            [3] = new KpiOee
            {
                KpiOeeId = 3,
                OeeRegisterId = 3,
                TimeStamp = DateTime.UtcNow.AddMinutes(-30),
                Oee = Math.Round(0.88, 6),
                Availability = Math.Round(0.90, 6),
                Performance = Math.Round(0.99, 6),
                Quality = Math.Round(0.96, 6),
                OeeRegister = null!
            },
            [4] = new KpiOee
            {
                KpiOeeId = 4,
                OeeRegisterId = 4,
                TimeStamp = DateTime.UtcNow.AddMinutes(-15),
                Oee = Math.Round(0.92, 6),
                Availability = Math.Round(0.90, 6),
                Performance = Math.Round(1.0, 6), // Clamped to max 1.0 for KPI
                Quality = Math.Round(0.96, 6),
                OeeRegister = null!
            },
            [5] = new KpiOee
            {
                KpiOeeId = 5,
                OeeRegisterId = 5,
                TimeStamp = DateTime.UtcNow.AddMinutes(-5),
                Oee = Math.Round(0.95, 6),
                Availability = Math.Round(0.90, 6),
                Performance = Math.Round(1.0, 6), // Clamped to max 1.0 for KPI
                Quality = Math.Round(1.0, 6),
                OeeRegister = null!
            },
            [6] = new KpiOee
            {
                KpiOeeId = 6,
                OeeRegisterId = 1, // Multiple KPIs can reference same register
                TimeStamp = DateTime.UtcNow.AddMinutes(-45),
                Oee = Math.Round(0.83, 6),
                Availability = Math.Round(0.88, 6),
                Performance = Math.Round(0.95, 6),
                Quality = Math.Round(0.99, 6),
                OeeRegister = null!
            },
            [7] = new KpiOee
            {
                KpiOeeId = 7,
                OeeRegisterId = 2,
                TimeStamp = DateTime.UtcNow.AddMinutes(-75),
                Oee = Math.Round(0.79, 6),
                Availability = Math.Round(0.85, 6),
                Performance = Math.Round(0.98, 6),
                Quality = Math.Round(0.95, 6),
                OeeRegister = null!
            }
        }.ToImmutableDictionary();

    /// <summary>
    /// Lazy-loaded cached list for maximum performance - best of both worlds
    /// </summary>
    private static readonly Lazy<IReadOnlyList<KpiOee>> _fixtureCache =
        new(() => _kpiOeesDict.Values.ToList());

    /// <summary>
    /// Get all KpiOee entities (cached List from dictionary for backward compatibility)
    /// </summary>
    public static IReadOnlyList<KpiOee> Fixture => _fixtureCache.Value;

    /// <summary>
    /// Get a specific KpiOee by ID - O(1) lookup (standardized pattern)
    /// </summary>
    public static KpiOee? GetById(int id) =>
        _kpiOeesDict.TryGetValue(id, out var kpiOee) ? kpiOee : null;

    /// <summary>
    /// Direct dictionary access for advanced scenarios (standardized pattern)
    /// </summary>
    public static IImmutableDictionary<int, KpiOee> Dictionary => _kpiOeesDict;

    /// <summary>
    /// Check if a KpiOee exists by ID - O(1) lookup
    /// </summary>
    public static bool Contains(int id) => _kpiOeesDict.ContainsKey(id);

    /// <summary>
    /// Get count of KpiOees - O(1) operation
    /// </summary>
    public static int Count => _kpiOeesDict.Count;

    /// <summary>
    /// Get KpiOee entries by OeeRegister ID - business query
    /// </summary>
    public static IEnumerable<KpiOee> GetByOeeRegisterId(int oeeRegisterId) =>
        _kpiOeesDict.Values.Where(k => k.OeeRegisterId == oeeRegisterId);

    /// <summary>
    /// Get recent KpiOee entries within time range
    /// </summary>
    public static IEnumerable<KpiOee> GetRecent(TimeSpan timeRange) =>
        _kpiOeesDict.Values.Where(k => DateTime.UtcNow - k.TimeStamp <= timeRange);

    /// <summary>
    /// Get KpiOee entries with OEE above threshold (high performance filter)
    /// </summary>
    public static IEnumerable<KpiOee> GetHighPerformance(double oeeThreshold = 0.85) =>
        _kpiOeesDict.Values.Where(k => k.Oee >= oeeThreshold);

    /// <summary>
    /// Get KpiOee entries with OEE below threshold (needs attention filter)
    /// </summary>
    public static IEnumerable<KpiOee> GetLowPerformance(double oeeThreshold = 0.80) =>
        _kpiOeesDict.Values.Where(k => k.Oee < oeeThreshold);
}
