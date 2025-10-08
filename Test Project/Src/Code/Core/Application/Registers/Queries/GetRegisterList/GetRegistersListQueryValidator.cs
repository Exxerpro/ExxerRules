// <copyright file="GetRegistersListQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Queries.GetRegisterList;

/// <summary>
/// Represents the GetRegistersListQueryValidator.
/// </summary>
public class GetRegistersListQueryValidator : AbstractValidator<GetRegistersListQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetRegistersListQueryValidator"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetRegistersListQueryValidator()
    {
        this.RuleFor(x => x.RegistersName)
            .NotNull()
            .WithMessage("RegistersName must not be null.")
            .NotEmpty()
            .WithMessage("RegistersName must not be empty.")
            .When(x => x.VariablesId == null || !x.VariablesId.Any())
            .WithMessage("Either RegistersName or VariablesId must be specified.");

        this.RuleFor(x => x.VariablesId)
            .NotNull()
            .WithMessage("VariablesId must not be null.")
            .NotEmpty()
            .WithMessage("VariablesId must not be empty.")
            .When(x => x.RegistersName == null || !x.RegistersName.Any())
            .WithMessage("Either RegistersName or VariablesId must be specified.");

        this.RuleFor(x => x.MachineId)
            .NotNull()
            .WithMessage("MachineId must not be null.")
            .NotEmpty()
            .WithMessage("At least one MachineId must be specified.");

        this.RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("StartDate must be earlier than or equal to EndDate.");

        // [NEW RULE] EndDate must be before today
        this.RuleFor(x => x.EndDate)
            .LessThan(DateTime.Now)
            .WithMessage("EndDate must be before the present day.");
    }
}
