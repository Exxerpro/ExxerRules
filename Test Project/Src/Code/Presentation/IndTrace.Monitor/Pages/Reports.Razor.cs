using Google.Protobuf;
using IndTrace.Application.BarCodes.DTO;
using IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;
using IndTrace.Application.BarCodes.Queries.GetReportsList.GetList;
using IndTrace.Application.BarCodes.Queries.GetReportsReport;
using IndTrace.Application.ConfigStations.Queries.GetConfigStationList;
using IndTrace.Application.Configuration.Services;
using IndTrace.UI.Models.Reports;
using IndTrace.UI.Models.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace IndTrace.Monitor.Pages;

/// <summary>
/// Represents the Reports page component that handles report generation and bar code details.
/// </summary>
public partial class Reports
{
    private readonly int debounce = 300;
    private BarCodeDetailMonitorVm? barCodeDetails;
    private int countBarCodes;
    private System.Timers.Timer? debounceTimer1;
    private EditContext? reportsListContext;
    private ReportsListQueryModel? reportsListQueryModel;
    private string? selectedBarCode;

    /// <summary>
    /// Gets or sets the last error message.
    /// </summary>
    public string LastError { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the selected bar code.
    /// </summary>
    public string? SelectedBarCode { get; set; }

    private List<BarCodeDto>? BarCodesDtoList { get; set; } = new();

    private ReportDetailMonitorVm ReportDetails { get; set; } = new();

    private bool Visible { get; set; } = false;

    private ApplicationConfiguration? applicationConfiguration;

    private bool isJsReady;

    /// <summary>
    /// Called after the component has been rendered.
    /// </summary>
    /// <param name="firstRender">True if this is the first render of the component.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            for (int i = 0; i < 10; i++) // Retry for up to ~1s
            {
                var jsReady = await this.JS.InvokeAsync<bool>("isMudLoaded");
                if (jsReady)
                {
                    this.isJsReady = true;
                    this.StateHasChanged();
                    break;
                }

                await Task.Delay(100); // Wait 100ms before retry
            }
        }
    }

    /// <summary>
    /// Initializes the component asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        await this.InitializeApplicationAsync();

        this.reportsListQueryModel = new ReportsListQueryModel(this.MonitorRequestDispatcher);
        await this.reportsListQueryModel.InitializeAsync();
        this.reportsListContext = new EditContext(this.reportsListQueryModel);
        await this.GetReportCodeListsAsync();

        this.reportsListContext.OnFieldChanged += (sender, e) =>
        {
            this.reportsListQueryModel?.ValidateRangeOfDates(e.FieldIdentifier.FieldName);
            this.debounceTimer1?.Stop();
            this.debounceTimer1?.Start();
        };

        this.debounceTimer1 = new System.Timers.Timer
        {
            Interval = this.debounce,
            AutoReset = false,
        };

        this.debounceTimer1.Elapsed += async (o, args) => await this.GetReportCodeListsAsync();
    }

    /// <summary>
    /// Initializes the application configuration asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns true if initialization succeeded.</returns>
    private async Task<bool> InitializeApplicationAsync()
    {
        if (this.applicationConfiguration is not null) return true;
        var result = await this.IndTraceConfigurationService.GetConfigurationAsync();
        if (result.IsSuccess)
        {
            this.applicationConfiguration = result.Value;
        }
        else
        {
            // Error loading configuration details - handled by ApplicationConfiguration null check
        }

        return this.applicationConfiguration is not null;
    }

    /// <summary>
    /// Exports reports to an Excel file for download.
    /// </summary>
    /// <param name="MachineNames">The dictionary of machine names keyed by machine ID.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ExportReports(Dictionary<int, string> MachineNames)
    {
        if (this.BarCodesDtoList is null) return;
        if (MachineNames is null) return;

        var request = new GetBarCodeReportQuery
        {
            BarCodesIdList = this.BarCodesDtoList.Select(x => x.BarCodeId).ToList(),
        };

        foreach (var item in this.BarCodesDtoList)
        {
            this.Logger.LogInformation("BarCode to report {BarCode}", item.BarCodeId.ToString());
        }

        var result = await this.MonitorRequestDispatcher.QueryAsync(request);
        if (result.IsFailure)
        {
            this.Logger.LogError("Request completes with errors: {@Result}", result);
            this.Snackbar.Add("Request completes with errors", Severity.Error);

            return;
        }

        this.Snackbar.Add("Request Success generating report", Severity.Success);

        var base64Excel = result!.Value!.ExportToExcel(MachineNames);
        var fileBytes = Convert.FromBase64String(base64Excel);
        var date = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss");
        var fileName = $"report {date}.xlsx";

        var downloadedFileResult = await this.BlazorDownloadFileService.DownloadFile(fileName, fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        Console.WriteLine(downloadedFileResult.Succeeded);

        this.Logger.LogInformation("Downloaded File result {Result}", downloadedFileResult.Succeeded);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the component has been initialized.
    /// </summary>
    public bool IsInitialized { get; set; }

    /// <summary>
    /// Gets the list of report codes based on the current filter criteria.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task GetReportCodeListsAsync()
    {
        if (this.reportsListQueryModel is null)
        {
            return;
        }

        var model = this.reportsListQueryModel;
        if (model.StartDate is null || model.EndDate is null)
        {
            return;
        }

        var request = new GetReportsListQuery(model.IsMaster, model.StartDate.Value,
            model.EndDate.Value, model.FilterByProduct, model.FilterByShift,
            model.FilterByState, model.SelectedProduct, model.SelectedShift,
            model.SelectedState, model.FilterByLine, model.SelectedLine,
            model.SelectedRegister, model.FilterByRegister,
            model.SelectedCustomer, model.FilterByCustomer);

        var result = await this.MonitorRequestDispatcher.QueryAsync(request);

        if (result.Value is not null)
        {
            this.BarCodesDtoList = result.Value.BarCodes.ToList();
            this.countBarCodes = result.Value.Count;
        }

        this.IsInitialized = true;
        await this.InvokeAsync(this.StateHasChanged);
    }

    /// <summary>
    /// Handles the button click event for displaying bar code details.
    /// </summary>
    /// <param name="barcode">The bar code to display details for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task OnButtonClicked(string barcode)
    {
        try
        {
            this.LastError = string.Empty;
            if (string.IsNullOrWhiteSpace(barcode)) return;

            this.selectedBarCode = barcode;

            var request = new GetBarCodeDetailMonitorQuery
            {
                BarCode = barcode,
            };

            var result = await this.MonitorRequestDispatcher.QueryAsync(request);

            if (result.IsFailure)
            {
                this.Logger.LogError("Request completes with errors: {@Result}", result);
                this.Snackbar.Add("Request completes with errors", Severity.Error);
            }

            this.barCodeDetails = result.Value ?? new BarCodeDetailMonitorVm();

            if (result.IsSuccess)
            {
                this.Snackbar.Add("Request Success generating report", Severity.Success);
            }

            await this.InvokeAsync(this.StateHasChanged);
        }
        catch (Exception ex)
        {
            this.LastError = ex.Message;
        }

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            NoHeader = false,
            FullWidth = true,
            MaxWidth = MaxWidth.ExtraLarge,
            CloseButton = true,
        };

        var parameters = new DialogParameters
        {
            { "BarCodeDetails", this.barCodeDetails },
            { "SelectedBarCode", this.selectedBarCode },
            { "MachineNames" , this.applicationConfiguration?.MachineNames ?? new Dictionary<int, string>() },
        };

        await this.DialogService.ShowAsync<IndTrace.Components.Area.BarCode.BarCodeDetailsViewModal>(this.selectedBarCode, parameters, options);
    }
}

//TODO
//ABR
//DISPLAY QUALITY OF REGISTER ON UI AND REPORTS
/*
 *
  Stage 1 – Backlog Grooming
Tag your UI and report feature requests explicitly with ShowSignalQuality, with descriptions like:

"Display icon or color indicator for low-quality register data"

"Include quality score or flag in detailed CSV/Excel export"

🖼️ Stage 2 – UI Minimal Version
Add a color-coded badge (🟢🟡🔴) beside each register name in the UI.

Green: good

Yellow: degraded

Red: suspect/drop

Tooltips show quality value or explanation.

Optional: Sort/filter views by quality.

📊 Stage 3 – Reports and Exports
Include a column per register or row:

RegisterQuality, DataConfidence, or IsSuspect

CSV / Excel exports get a lightweight flag (e.g., 0, 1, 2 or "good", "warn", "bad").

🔁 Stage 4 – Feedback Loop
When quality drops repeatedly:

Show alerts in UI

Suggest signal source diagnosis

Add export filter: "exclude low quality rows"

 *
 */
