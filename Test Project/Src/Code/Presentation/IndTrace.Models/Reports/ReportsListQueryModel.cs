// <copyright file="ReportsListQueryModel.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.Reports;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using IndTrace.Application.BarCodes.Queries.GetReportsList.FiltersInfo;
using IndTrace.Application.Models.RequestHandler;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a query model for filtering and retrieving reports with various filter options and date range validation.
/// </summary>
public class ReportsListQueryModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReportsListQueryModel"/> class with default values.
    /// </summary>
    public ReportsListQueryModel()
    {
        if (Debugger.IsAttached)
        {
            this.StartDate = DateTime.Now.ToLocalTime().AddDays(-150);
            this.EndDate = DateTime.Now.ToLocalTime();
        }
        else
        {
            this.StartDate = DateTime.Now.ToLocalTime().AddDays(-14);
            this.EndDate = DateTime.Now.ToLocalTime();
        }
    }

    private readonly TimeSpan maxRangeConsult = TimeSpan.FromDays(365);

    /// <summary>
    /// Gets or sets the start date for the reports query range.
    /// </summary>
    [Required]
    public DateTime? StartDate { get; set; } = DateTime.Now.ToLocalTime().AddDays(-120);

    /// <summary>
    /// Gets or sets the end date for the reports query range.
    /// </summary>
    [Required]
    public DateTime? EndDate { get; set; } = DateTime.Now.ToLocalTime();

    /// <summary>
    /// Gets or sets a value indicating whether to filter for master records only.
    /// </summary>
    [Required]
    public bool IsMaster { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to apply product filtering.
    /// </summary>
    [Required]
    public bool FilterByProduct { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to apply shift filtering.
    /// </summary>
    [Required]
    public bool FilterByShift { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to apply line filtering.
    /// </summary>
    [Required]
    public bool FilterByLine { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to apply state filtering.
    /// </summary>
    [Required]
    public bool FilterByState { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to apply register filtering.
    /// </summary>
    [Required]
    public bool FilterByRegister { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to apply customer filtering.
    /// </summary>
    [Required]
    public bool FilterByCustomer { get; set; }

    /// <summary>
    /// Gets or sets the selected product for filtering.
    /// </summary>
    public string SelectedProduct { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected state for filtering.
    /// </summary>
    public string SelectedState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected customer for filtering.
    /// </summary>
    public string SelectedCustomer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected line for filtering.
    /// </summary>
    public string SelectedLine { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected register for filtering.
    /// </summary>
    public string SelectedRegister { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected shift for filtering.
    /// </summary>
    public int SelectedShift { get; set; }

    /// <summary>
    /// Gets or sets the list of available products for filtering.
    /// </summary>
    public List<string> Products { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of available customers for filtering.
    /// </summary>
    public List<string> Customers { get; set; } = [];

    /// <summary>
    /// Gets the list of customer products for filtering.
    /// </summary>
    public List<CustomerProduct> CustomerProducts { get; private set; } = [];

    /// <summary>
    /// Gets or sets the list of available states for filtering.
    /// </summary>
    public List<string> StatesList { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of available shifts for filtering.
    /// </summary>
    public List<int> Shifts { get; set; } = [];

    private IMonitorRequestDispatcher? monitorRequestDispatcher;

    /// <summary>
    /// Gets or sets a value indicating whether the model has been initialized.
    /// </summary>
    public bool IsInitialized { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportsListQueryModel"/> class with a command dispatcher.
    /// </summary>
    /// <param name="guiCommandDispatcher">The GUI command dispatcher for executing queries.</param>
    public ReportsListQueryModel(IMonitorRequestDispatcher monitorRequestDispatcher)
    {
        this.monitorRequestDispatcher = monitorRequestDispatcher;
        this.StartDate = DateTime.Now.AddDays(-7);
        this.EndDate = DateTime.Now;
        this.SelectedProduct = string.Empty;
        this.SelectedState = string.Empty;
        this.SelectedShift = 0;
        this.Products = [];
        this.StatesList = [];
        this.Customers = [];
        this.Shifts = [];
    }

    private async Task<ReportsFilterInfoVm> GetInfoFilterReports(IMonitorRequestDispatcher monitorRequestDispatcher)
    {
        this.monitorRequestDispatcher = monitorRequestDispatcher;
        var request = new GetReportsFilterInfoQuery(false, DateTime.Now.AddDays(-1).AddTicks(-1), DateTime.Now);

        var result = await this.monitorRequestDispatcher.QueryAsync(request);
        return result.Value ?? new ReportsFilterInfoVm();
    }

    /// <summary>
    /// Initializes the model asynchronously by loading filter information.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InitializeAsync()
    {
        if (this.monitorRequestDispatcher is null)
        {
            // Not initialized with a dispatcher; leave defaults and mark not initialized.
            this.IsInitialized = false;
            return;
        }

        var result = await this.GetInfoFilterReports(this.monitorRequestDispatcher);
        this.Products = result.Products;
        this.StatesList = result.States;
        this.Shifts = result.Shifts;
        this.Customers = result.Customers;
        this.CustomerProducts = result.CustomerProducts;

        this.IsInitialized = true;
    }

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
        if (!this.IsRangeLesThanMaxRange)
        {
            if (this.EndDate is DateTime end)
            {
                this.StartDate = end.AddTicks(this.maxRangeConsult.Ticks);
            }
        }

        if (!this.IsRangeValidEndAfterInitialDate)
        {
            if (this.EndDate is DateTime end)
            {
                this.StartDate = end.AddDays(-1);
            }
        }
    }

    private void ValidateRangeWhenInitialDateChanged()
    {
        if (!this.IsRangeLesThanMaxRange)
        {
            if (this.StartDate is DateTime start)
            {
                this.EndDate = start.AddTicks(this.maxRangeConsult.Ticks);
            }
        }

        if (!this.IsRangeValidEndAfterInitialDate)
        {
            if (this.StartDate is DateTime start)
            {
                this.EndDate = start.AddDays(1);
            }
        }
    }

    private bool IsRangeValidEndAfterInitialDate => (this.EndDate ?? DateTime.MinValue) >= (this.StartDate ?? DateTime.MinValue);

    private bool IsRangeLesThanMaxRange => (this.StartDate ?? DateTime.MinValue).AddTicks(this.maxRangeConsult.Ticks) >= (this.EndDate ?? DateTime.MinValue);

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate ReportsListQueryModel logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
