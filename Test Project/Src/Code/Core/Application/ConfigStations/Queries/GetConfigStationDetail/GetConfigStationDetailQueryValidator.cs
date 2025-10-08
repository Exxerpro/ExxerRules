// <copyright file="GetConfigStationDetailQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigStations.Queries.GetConfigStationDetail;

/// <summary>
/// Validator for <see cref="GetConfigStationDetailQuery"/>. Ensures that the part number meets length requirements.
/// </summary>
public class GetConfigStationDetailQueryValidator : AbstractValidator<GetConfigStationDetailQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetConfigStationDetailQueryValidator"/> class.
    /// </summary>
    public GetConfigStationDetailQueryValidator()
    {
        // [Fix]
        // CLAUDE
        // Date: 20/08/2025
        // Reason: Added NotNull and NotEmpty validation before length checks to prevent validation chain issues

        // [Fix]
        // CURSOR
        // Date: 23/08/2025
        // Reason: Pattern A Fix - PartNumber is optional field, should allow null values but validate when provided
        this.RuleFor(v => v.PartNumber)
            .NotEmpty()
            .WithMessage("Part number cannot be empty.")
            .Length(4, 9)
            .WithMessage("Part number must be between 4 and 9 characters.")
            .When(v => v.PartNumber != null);  // Only validate when PartNumber is provided
    }
}
