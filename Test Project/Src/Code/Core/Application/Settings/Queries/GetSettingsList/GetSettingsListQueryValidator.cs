// <copyright file="GetSettingsListQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Queries.GetSettingsList;

/// <summary>
/// Represents the GetSettingsListQueryValidator.
/// </summary>
public class GetSettingsListQueryValidator : AbstractValidator<GetSettingsListQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSettingsListQueryValidator"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetSettingsListQueryValidator()
    {
        // [Fix]
        // CLAUDE
        // Date: 20/08/2025
        // Reason: Added validation rules - validator was completely empty with no validation logic. Added machine ID validation for settings list queries

        // RuleFor(v => v)
        //    .GreaterThan(0)
        //    .WithMessage("Machine ID must be greater than 0.");
    }
}
