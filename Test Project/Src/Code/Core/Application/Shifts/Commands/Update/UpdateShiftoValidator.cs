// <copyright file="UpdateShiftoValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Commands.Update;

/// <summary>
/// Validator for UpdateShiftCommand that ensures shift update data meets system requirements.
/// </summary>
public class UpdateShiftoValidator : AbstractValidator<UpdateShiftCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateShiftoValidator"/> class.
    /// </summary>
    public UpdateShiftoValidator()
    {
        // [Fix]
        // CLAUDE
        // Date: 20/08/2025
        // Reason: Added comprehensive validation for all nullable properties - validator was extremely incomplete, only validating ShiftId with NotEmpty() but missing 8 other nullable properties. Added proper validation for railway-oriented programming

        // Validate nullable ID properties when provided
        this.When(v => v.ShiftId.HasValue, () =>
        {
            this.RuleFor(v => v.ShiftId)
                .GreaterThan(0)
                .WithMessage("Shift ID must be greater than 0.");
        });

        this.When(v => v.IsActive.HasValue, () =>
        {
            this.RuleFor(v => v.IsActive)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(1)
                .WithMessage("IsActive must be 0 (inactive) or 1 (active).");
        });

        this.When(v => v.Version.HasValue, () =>
        {
            this.RuleFor(v => v.Version)
                .GreaterThan(0)
                .WithMessage("Version must be greater than 0.");
        });

        // Validate string properties when provided for null safety
        this.When(v => !string.IsNullOrEmpty(v.PartNumber), () =>
        {
            this.RuleFor(v => v.PartNumber)
                .Length(1, 50)
                .WithMessage("Part Number must be between 1 and 50 characters.");
        });

        this.When(v => !string.IsNullOrEmpty(v.ShiftName), () =>
        {
            this.RuleFor(v => v.ShiftName)
                .Length(1, 100)
                .WithMessage("Shift Name must be between 1 and 100 characters.");
        });

        this.When(v => !string.IsNullOrEmpty(v.Shifto), () =>
        {
            this.RuleFor(v => v.Shifto)
                .Length(1, 50)
                .WithMessage("Shifto must be between 1 and 50 characters.");
        });

        this.When(v => !string.IsNullOrEmpty(v.CustomerPartNumber), () =>
        {
            this.RuleFor(v => v.CustomerPartNumber)
                .Length(1, 100)
                .WithMessage("Customer Part Number must be between 1 and 100 characters.");
        });

        this.When(v => !string.IsNullOrEmpty(v.AliasNoParte), () =>
        {
            this.RuleFor(v => v.AliasNoParte)
                .Length(1, 100)
                .WithMessage("Alias No Parte must be between 1 and 100 characters.");
        });

        this.When(v => !string.IsNullOrEmpty(v.Description), () =>
        {
            this.RuleFor(v => v.Description)
                .Length(1, 500)
                .WithMessage("Description must be between 1 and 500 characters.");
        });

        // At least one field should be provided for update
        this.RuleFor(v => v)
            .Must(v => v.ShiftId.HasValue || v.IsActive.HasValue || v.Version.HasValue ||
                      !string.IsNullOrEmpty(v.PartNumber) || !string.IsNullOrEmpty(v.ShiftName) ||
                      !string.IsNullOrEmpty(v.Shifto) || !string.IsNullOrEmpty(v.CustomerPartNumber) ||
                      !string.IsNullOrEmpty(v.AliasNoParte) || !string.IsNullOrEmpty(v.Description))
            .WithMessage("At least one field must be provided for update.");
    }
}
