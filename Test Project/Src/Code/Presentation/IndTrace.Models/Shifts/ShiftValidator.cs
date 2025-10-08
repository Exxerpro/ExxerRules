// <copyright file="ShiftValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.Shifts;

using FluentValidation;

/// <summary>
/// Validator for shift models that ensures shift data integrity and business rule compliance.
/// </summary>
/// <remarks>
/// This validator ensures that shift start times are provided and durations fall within acceptable ranges.
/// It validates both required fields and business constraints for shift scheduling.
/// </remarks>
public class ShiftValidator : AbstractValidator<ShiftModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftValidator"/> class and configures validation rules.
    /// </summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="ShiftValidator"/> class.
    /// </summary>
    /// <remarks>
    /// Sets up validation rules for shift properties including start time requirements
    /// and duration constraints based on minimum and maximum allowed values.
    /// </remarks>
    public ShiftValidator()
    {
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate ShiftValidator logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
        this.RuleFor(shift => shift.StartBy).NotNull().WithMessage("Start time is required.");

        this.RuleFor(shift => shift.Duration)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Duration is required.")
            .Must((shift, duration) => duration >= shift.MinDuration && duration <= shift.MaxDuration)
            .WithMessage(shift => $"Duration must be between {shift.MinDuration} and {shift.MaxDuration}.");

        // Add any additional complex rules you might need.
    }
}
