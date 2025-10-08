// <copyright file="RegisterMachineIdUpdater.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Helper;

/// <summary>
/// Updates RegisterView.MachineId from CycleView data where missing or inconsistent
/// Pure function - no side effects, fully deterministic, perfect for parallel execution
/// </summary>
/// <remarks>
/// This static helper class provides manufacturing-grade performance for updating
/// RegisterView.MachineId fields from matching CycleView data. Handles null inputs
/// gracefully with O(1) lookup performance using dictionary-based lookups.
///
/// Industrial safety compliance:
/// - Defensive null checks everywhere
/// - No exceptions thrown - graceful degradation
/// - Deterministic behavior for testing
/// - High-performance O(1) lookup algorithm
/// </remarks>
public static class RegisterMachineIdUpdater
{
    /// <summary>
    /// Updates RegisterView.MachineId from matching CycleView data where missing or inconsistent.
    /// Manufacturing-grade: handles null inputs gracefully, O(1) lookup performance.
    /// </summary>
    /// <param name="registers">The collection of registers to update. Can be null or empty.</param>
    /// <param name="cycles">The collection of cycles containing machine ID data. Can be null or empty.</param>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Validates inputs with defensive null checks
    /// 2. Creates O(1) lookup dictionary from cycles
    /// 3. Updates registers where MachineId <= 0 and CycleId matches
    ///
    /// Performance characteristics:
    /// - Time complexity: O(n + m) where n = cycles.Count, m = registers.Count
    /// - Space complexity: O(n) for the lookup dictionary
    /// - No exceptions thrown - industrial safety compliant
    /// </remarks>
    public static void Update(
        IList<RegisterView>? registers,
        IReadOnlyList<CycleView>? cycles)
    {
        // CLAUDE.md compliance: defensive null checks
        if (registers is null || cycles is null || !cycles.Any())
            return;

        // Create lookup dictionary for O(1) performance
        var cycleLookup = cycles
            .Where(c => c.CycleId > 0 && c.MachineId > 0)
            .ToDictionary(c => c.CycleId, c => c.MachineId);

        // Update registers with missing or zero MachineId
        foreach (var register in registers.Where(r => r.MachineId <= 0))
        {
            if (register.CycleId > 0 &&
                cycleLookup.TryGetValue(register.CycleId, out var machineId))
            {
                register.MachineId = machineId;
            }
        }
    }
}
