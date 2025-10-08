// <copyright file="GetShiftoDetailQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Queries.GetShftDetail;

/// <summary>
/// Represents the GetShiftoDetailQueryValidator.
/// </summary>
public class GetShiftoDetailQueryValidator : AbstractValidator<GetShiftDetailQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetShiftoDetailQueryValidator"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetShiftoDetailQueryValidator()
    {
        this.RuleFor(v => v.ShiftId).GreaterThan(0).LessThan(100);
    }
}
