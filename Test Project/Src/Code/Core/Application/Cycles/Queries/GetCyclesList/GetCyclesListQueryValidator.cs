// <copyright file="GetCyclesListQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Queries.GetCyclesList;

/// <summary>
/// Represents the GetCyclesListQueryValidator.
/// </summary>
public class GetCyclesListQueryValidator : AbstractValidator<GetCyclesListQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCyclesListQueryValidator"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetCyclesListQueryValidator()
    {
        // [Fix]
        // CLAUDE
        // Date: 20/08/2025
        // Reason: Added comprehensive validation - validator was extremely incomplete, only checking NotEmpty() but missing proper ID validation constraints for cycle queries
        this.RuleFor(v => v.Id)
            .NotNull()
            .WithMessage("ID cannot be null.")
            .GreaterThan(0)
            .WithMessage("ID must be greater than 0.");
    }
}
