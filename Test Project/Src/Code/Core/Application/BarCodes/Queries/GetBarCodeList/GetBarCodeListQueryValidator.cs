// <copyright file="GetBarCodeListQueryValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeList;

/// <summary>
/// Validator for <see cref="GetBarCodesListQuery"/> that ensures barcode list queries have valid date range parameters.
/// </summary>
/// <remarks>
/// This validator enforces that barcode queries must specify both start and end dates, which is essential
/// for performance optimization and preventing unbounded queries against the production tracking database.
/// Date-based filtering is critical for managing large datasets in manufacturing environments.
/// </remarks>
public class GetBarCodeListQueryValidator : AbstractValidator<GetBarCodesListQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetBarCodeListQueryValidator"/> class
    /// and configures validation rules for barcode list queries.
    /// </summary>
    public GetBarCodeListQueryValidator()
    {
        // [Fix]
        // CLAUDE
        // Date: 20/08/2025
        // Reason: Added comprehensive date validation - validator was too simple, only checking NotNull but missing business rules for date ranges and valid date constraints
        this.RuleFor(v => v.StartDate)
            .NotNull()
            .WithMessage("Start date cannot be null.")
            .Must(date => date != default(DateTime))
            .WithMessage("Start date must be a valid date.")
            .LessThan(DateTime.Now.AddDays(1))
            .WithMessage("Start date cannot be in the future.");

        this.RuleFor(v => v.EndDate)
            .NotNull()
            .WithMessage("End date cannot be null.")
            .Must(date => date != default(DateTime))
            .WithMessage("End date must be a valid date.")
            .LessThan(DateTime.Now.AddDays(1))
            .WithMessage("End date cannot be in the future.");

        this.RuleFor(v => v)
            .Must(query => query.EndDate >= query.StartDate)
            .WithMessage("End date must be greater than or equal to start date.")
            .WithName("DateRange");

        this.RuleFor(v => v)
            .Must(query => (query.EndDate - query.StartDate).TotalDays <= 365)
            .WithMessage("Date range cannot exceed 365 days.")
            .WithName("DateRangeLimit");
    }
}
