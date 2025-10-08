// <copyright file="GetRegistroDetailQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Queries.GetRegisterDetail;

/// <summary>
/// Represents the GetRegistroDetailQueryValidator.
/// </summary>
public class GetRegistroDetailQueryValidator : AbstractValidator<GetRegisterDetailQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetRegistroDetailQueryValidator"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetRegistroDetailQueryValidator()
    {
        // [Fix]
        // CLAUDE
        // Date: 20/08/2025
        // Reason: Enhanced ID validation with proper error message following railway-oriented pattern
        this.RuleFor(v => v.RegisterId)
            .GreaterThan(0)
            .WithMessage("Register ID must be greater than 0.");
    }
}
