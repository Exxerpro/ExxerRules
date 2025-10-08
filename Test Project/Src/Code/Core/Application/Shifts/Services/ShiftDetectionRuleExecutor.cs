// <copyright file="ShiftDetectionRuleExecutor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Services;

public interface IShiftDetectionRuleExecutor
{
    /// <summary>
    /// Detects shift type and calculates shift start time for given datetime
    /// Uses current 3-shift rules (future: configurable per facility).
    /// </summary>
    /// <returns></returns>
    (ShiftType shiftType, DateTime startTime, TimeSpan duration) DetectShift(DateTime currentTime);

    /// <summary>
    /// Generic shift detection with custom rules (for future database configuration).
    /// </summary>
    /// <returns></returns>
    (ShiftType shiftType, DateTime startTime, TimeSpan duration) DetectShiftWithRules(
        DateTime currentTime,
        IShiftDetectionRule[] rules);
}

/// <summary>
/// Rule executor for shift detection - supports configurable shift patterns
/// Handles 2-shift, 3-shift, 4-shift, and custom manufacturing patterns
/// Future: Load rules from database per facility/line configuration.
/// </summary>
public class ShiftDetectionRuleExecutor : IShiftDetectionRuleExecutor
{
    // Standard 3-shift pattern constants (current default)
    public const int FirstShiftStart = 7;

    public const int SecondShiftStart = 15;
    public const int ThirdShiftStart = 23;

    /// <summary>
    /// Current 3-shift manufacturing pattern (Ford, Tesla, BMW, etc.)
    /// Future: Replace with database-loaded rules per facility.
    /// </summary>
    private readonly IShiftDetectionRule[] current3ShiftRules =
    [
        new ShiftDetectionRule(FirstShiftStart, SecondShiftStart, ShiftType.First, 8),    // 7-14: First (8h)
        new ShiftDetectionRule(SecondShiftStart, ThirdShiftStart, ShiftType.Second, 8),   // 15-22: Second (8h)
        new ShiftDetectionRule(ThirdShiftStart, FirstShiftStart, ShiftType.Third, 8, SpansMidnight: true) // 23-6: Third (8h, spans midnight)
    ];

    /// <summary>
    /// Example 2-shift pattern (for facilities with day/night operations).
    /// </summary>
    private readonly IShiftDetectionRule[] example2ShiftRules =
    [
        new ShiftDetectionRule(6, 18, ShiftType.First, 12),   // 6-17: Day (12h)
        new ShiftDetectionRule(18, 6, ShiftType.Second, 12, SpansMidnight: true) // 18-5: Night (12h, spans midnight)
    ];

    /// <summary>
    /// Detects shift type and calculates shift start time for given datetime
    /// Uses current 3-shift rules (future: configurable per facility).
    /// </summary>
    /// <returns></returns>
    public (ShiftType shiftType, DateTime startTime, TimeSpan duration) DetectShift(DateTime currentTime)
    {
        return this.DetectShiftWithRules(currentTime, this.current3ShiftRules);
    }

    /// <summary>
    /// Generic shift detection with custom rules (for future database configuration).
    /// </summary>
    /// <returns></returns>
    public (ShiftType shiftType, DateTime startTime, TimeSpan duration) DetectShiftWithRules(
        DateTime currentTime,
        IShiftDetectionRule[] rules)
    {
        var hour = currentTime.Hour;
        var rule = rules.First(r => r.AppliesTo(hour));

        // Calculate proper shift start time with midnight boundary handling
        DateTime shiftStartTime;
        if (rule.SpansMidnight && hour < rule.StartHour)
        {
            // Early morning hours belong to shift that started previous day
            shiftStartTime = currentTime.Date.AddDays(-1).AddHours(rule.StartHour);
        }
        else
        {
            // Normal case: shift start on same day
            shiftStartTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, rule.StartHour, 0, 0);
        }

        return (rule.ShiftType, shiftStartTime, TimeSpan.FromHours(rule.DurationHours));
    }
}
