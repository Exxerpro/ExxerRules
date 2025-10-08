// <copyright file="BarCodesListQueryModel.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.BarCodes;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

/// <summary>
/// Represents a query model for filtering and retrieving lists of barcodes with date range validation.
/// </summary>
public class BarCodesListQueryModel
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate BarCodesListQueryModel logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

    /// <summary>
    /// Initializes a new instance of the <see cref="BarCodesListQueryModel"/> class.
    /// Sets default date ranges based on whether a debugger is attached.
    /// </summary>
    public BarCodesListQueryModel()
    {
        if (Debugger.IsAttached)
        {
            this.InitialDate = DateTime.Now.ToLocalTime().AddDays(-150);
            this.EndDate = DateTime.Now;
        }
        else
        {
            this.InitialDate = DateTime.Now.ToLocalTime().AddDays(-14);
            this.EndDate = DateTime.Now;
        }
    }

    /// <summary>
    /// Gets or sets the initial date for the query range.
    /// </summary>
    [Required]
    public DateTime? InitialDate { get; set; } = DateTime.Now.ToLocalTime().AddDays(-120);

    /// <summary>
    /// Gets or sets the end date for the query range.
    /// </summary>
    [Required]
    public DateTime? EndDate { get; set; } = DateTime.Now.ToLocalTime();

    /// <summary>
    /// Gets or sets a value indicating whether this query is for master records.
    /// </summary>
    [Required]
    public bool IsMaster { get; set; }

    private readonly TimeSpan maxRangeConsult = TimeSpan.FromDays(365);

    /// <summary>
    /// Validates the date range based on which field was changed.
    /// </summary>
    /// <param name="fieldChanged">The name of the field that was changed ("StartDate" or "EndDate").</param>
    public void ValidateRangeOfDates(string fieldChanged)
    {
        if (fieldChanged == "StartDate")
        {
            this.ValidateRangeWhenInitialDateChanged();
        }
        else
        {
            this.ValidateRangeWhenEndDateChanged();
        }
    }

    private void ValidateRangeWhenEndDateChanged()
    {
        if (this.EndDate is null)
        {
            this.EndDate = DateTime.Now.ToLocalTime();
        }
        if (this.InitialDate is null)
        {
            this.InitialDate = this.EndDate.Value.AddDays(-1);
        }
        if (!this.IsRangeLesThanMaxRange)
        {
            this.InitialDate = this.EndDate.Value.AddTicks(this.maxRangeConsult.Ticks);
        }

        if (!this.IsRangeValidEndAfterInitialDate)
        {
            this.InitialDate = this.EndDate.Value.AddDays(-1);
        }
    }

    private void ValidateRangeWhenInitialDateChanged()
    {
        if (this.InitialDate is null)
        {
            this.InitialDate = DateTime.Now.ToLocalTime().AddDays(-1);
        }
        if (this.EndDate is null)
        {
            this.EndDate = this.InitialDate.Value.AddDays(1);
        }
        if (!this.IsRangeLesThanMaxRange)
        {
            this.EndDate = this.InitialDate.Value.AddTicks(this.maxRangeConsult.Ticks);
        }

        if (!this.IsRangeValidEndAfterInitialDate)
        {
            this.EndDate = this.InitialDate.Value.AddDays(1);
        }
    }

    private bool IsRangeValidEndAfterInitialDate
    {
        get
        {
            if (this.InitialDate is null || this.EndDate is null)
            {
                return false;
            }
            return this.EndDate.Value >= this.InitialDate.Value;
        }
    }

    private bool IsRangeLesThanMaxRange
    {
        get
        {
            if (this.InitialDate is null || this.EndDate is null)
            {
                return false;
            }
            return this.InitialDate.Value.AddTicks(this.maxRangeConsult.Ticks) >= this.EndDate.Value;
        }
    }
}
