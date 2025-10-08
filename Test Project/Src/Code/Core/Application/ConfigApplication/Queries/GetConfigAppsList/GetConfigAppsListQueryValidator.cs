// <copyright file="GetConfigAppsListQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Queries.GetConfigAppsList;

/// <summary>
/// Represents the GetConfigAppsListQueryValidator.
/// </summary>
public class GetConfigAppsListQueryValidator : AbstractValidator<GetConfigAppsListQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetConfigAppsListQueryValidator"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetConfigAppsListQueryValidator()
    {
        // [Fix]
        // CLAUDE
        // Date: 20/08/2025
        // Reason: Added comprehensive validation - validator was too simple, only checking Length(5) but missing proper null/empty validation and business rules for ID format
        this.RuleFor(v => v.Id)
            .NotNull()
            .WithMessage("ID cannot be null.")
            .NotEmpty()
            .WithMessage("ID cannot be empty.")
            .Length(5)
            .WithMessage("ID must be exactly 5 characters.");
    }
}
