// <copyright file="ShiftDetectionRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Services;

using IndTrace.Domain.Enum;

/// <summary>
/// Concrete shift detection rule implementation
/// Domain entity representing a manufacturing shift pattern rule.
/// </summary>
public record ShiftDetectionRule(
    int StartHour,
    int EndHour,
    ShiftType ShiftType,
    int DurationHours,
    bool SpansMidnight = false) : IShiftDetectionRule
{
    /// <summary>
    /// Determines if this rule applies to the given hour with proper midnight boundary handling.
    /// </summary>
    /// <param name="hour">The hour to check (0-23).</param>
    /// <returns></returns>
    public bool AppliesTo(int hour)
    {
        // Input validation
        if (hour < 0 || hour > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hour), "Hour must be between 0 and 23");
        }

        if (!this.SpansMidnight)
        {
            // Normal range (e.g., 7-15, 15-23)
            return hour >= this.StartHour && hour < this.EndHour;
        }

        // Handle midnight-spanning shifts (e.g., 23-7)
        // This means: hour >= 23 OR hour < 7
        if (this.StartHour > this.EndHour)
        {
            return hour >= this.StartHour || hour < this.EndHour;
        }

        // Fallback to normal range if misconfigured
        return hour >= this.StartHour && hour < this.EndHour;
    }
}
