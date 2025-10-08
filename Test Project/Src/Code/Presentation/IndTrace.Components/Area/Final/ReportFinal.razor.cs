using IndTrace.Application.BarCodes.Queries.GetBarCodeDetail;
using IndTrace.Application.Cycles;
using IndTrace.Application.Models.BarCodesUtilities;
using IndTrace.CognexComm;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Interfaces;
using IndTrace.UI.Models.BarCodes;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace IndTrace.Components.Area.Final;

/// <summary>
/// Component for displaying the final report and barcode details, including QR code generation and cycle status.
/// </summary>

/// <summary>
/// Initializes a new instance of the <see cref="ReportFinal"/> class.
/// </summary>
/// <param name="dataMan">The DataMan barcode reader instance.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="monitorRequestDispatcher">The Monitor command dispatcher.</param>
public partial class ReportFinal(
    DataMan dataMan,
    ILogger<ReportFinal> logger,
    IMonitorRequestDispatcher monitorRequestDispatcher)
{
    /// <summary>
    /// Gets or sets the visual display mode for the report.
    /// </summary>
    public string Visual = "Visual";

    /// <summary>
    /// Gets the barcode model instance for managing barcode data.
    /// </summary>
    public readonly BarCodeModel BarCodeModel = new BarCodeModel();

    /// <summary>
    /// Generates a QR code from the current barcode label and returns it as a base64 data URI.
    /// </summary>
    /// <returns>A base64 encoded data URI representing the QR code image.</returns>
    public string GetQrCode()
    {
        //N40299054210082
        var qroCodeGenerator = new QRCodeGenerator();
        var barCodeImage = qroCodeGenerator.CreateQrCode(this.BarCodeModel.Label, QRCodeGenerator.ECCLevel.H, true);
        return barCodeImage.ToBase64DataUri();
    }

    /// <summary>
    /// Gets or sets the barcode detail result from the query operation.
    /// </summary>
    public BarCodeDetailVm? BarCodeDetailResult { get; set; }

    private Cycle? Cycle { get; set; }

    private CycleViewModel CycleView { get; set; } = new CycleViewModel();

    /// <summary>
    /// Gets or sets the result barcode string after processing.
    /// </summary>
    public string ResultBarCode { get; set; } = string.Empty;

    private string ScannedBarCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status message for the report operation.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Handles asynchronous barcode reading and processing from the DataMan scanner.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleBarCodeReadAsync()
    {
        if (string.IsNullOrWhiteSpace(dataMan.Result)) return;
        if (this.ScannedBarCode == dataMan.Result) return;
        this.ScannedBarCode = dataMan.Result;
        if (await this.FillQueryAsync(this.ScannedBarCode))
            await this.SendQueryAsync();
    }

    /// <summary>
    /// Handles the valid form submission for barcode processing.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnValidSubmit()
    {
        logger.LogInformation("OnValidSubmit {Label}", this.BarCodeModel.Label);

        if (string.IsNullOrWhiteSpace(this.BarCodeModel.Label)) return;

        if (await this.FillQueryAsync(this.BarCodeModel.Label))
            await this.SendQueryAsync();
    }

    /// <summary>
    /// Gets or sets a value indicating whether to display detailed values in the UI.
    /// </summary>
    public bool ShowValues { get; set; }

    /// <summary>
    /// Updates the heading display based on mouse click events.
    /// </summary>
    /// <param name="e">The mouse event arguments.</param>
    public void UpdateHeading(MouseEventArgs e)
    {
        this.ShowValues = !this.ShowValues;
    }

    /// <summary>
    /// Fills the query parameters with the provided barcode label and validates the input.
    /// </summary>
    /// <param name="label">The barcode label to process.</param>
    /// <returns>A task that returns true if the query was successfully filled, false otherwise.</returns>
    public async Task<bool> FillQueryAsync(string? label)
    {
        var query = new GetBarCodeDetailQuery();
        this.ScannedBarCode = label ?? string.Empty;

        this.Status = string.Empty;
        this.ResultBarCode = string.Empty;
        this.BarCodeDetailResult = null;
        this.Cycle = null;

        if (!dataMan.ValidateLabel(this.ScannedBarCode))
        {
            this.Status = "BarCodeInvalid";
            await this.InvokeAsync(this.StateHasChanged);
            return false;
        }

        query.BarCode = this.ScannedBarCode;
        query.MachineId = 500;  //TODO [CRITICAL] WHY 500?, THIS IS THE NUMBER OF A FINAL STATION, WHY IS HARDOCODE

        return true;
    }

    /// <summary>
    /// Sends the barcode query to the backend and processes the response.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendQueryAsync()
    {
        try
        {
            var query = new GetBarCodeDetailQuery
            {
                BarCode = this.ScannedBarCode,
            };
            var vm = await monitorRequestDispatcher.QueryAsync(query);
            this.BarCodeDetailResult = vm.Value;
            if (this.BarCodeDetailResult is null)
            {
                this.Status = "No data returned";
                await this.InvokeAsync(this.StateHasChanged);
                return;
            }
            this.ResultBarCode = this.BarCodeDetailResult is not null && Equals(this.BarCodeDetailResult.ResultValidation, ResultValidation.Valid) ? this.ScannedBarCode : string.Empty;

            if (this.BarCodeDetailResult is not null && Equals(this.BarCodeDetailResult.ResultValidation, ResultValidation.Valid))
            {
                await this.InvokeAsync(this.StateHasChanged);
                return;
            }

            if (this.BarCodeDetailResult?.Cycles is null || this.BarCodeDetailResult!.Cycles.Count == 0)
            {
                this.Status = "No cycles available";
                await this.InvokeAsync(this.StateHasChanged);
                return;
            }

            var lastCycle = this.BarCodeDetailResult.Cycles.Aggregate((curMax, x) => x.MachineId > curMax.MachineId ? x : curMax);

            if (lastCycle.CycleStatus != CycleStatus.FinishedOk && lastCycle.PartStatus == PartStatus.Ok)
            {
                lastCycle.MachineId = this.BarCodeDetailResult.NextMachineId;
                lastCycle.CycleStatus = CycleStatus.NotStarted;
                lastCycle.CycleId = 0;
            }

            this.BarCodeDetailResult.Cycles.Clear();
            this.BarCodeDetailResult.Cycles.Add(lastCycle);
            this.CycleView.CycleStatus = lastCycle.CycleStatus;
            this.CycleView.PartStatus = lastCycle.PartStatus;
            this.Cycle = lastCycle;
            await this.InvokeAsync(this.StateHasChanged);
        }
        catch (Exception ex)
        {
            // 🔴 Fixed: Use proper logging instead of Console.WriteLine anti-pattern
            logger.LogError(ex, "Error during report final processing");
            this.Status = ex.Message;
            this.BarCodeDetailResult = null;
            this.ResultBarCode = string.Empty;
            await this.InvokeAsync(this.StateHasChanged);
        }
    }

    /// <summary>
    /// Handles component initialization logic.
    /// </summary>
    protected override void OnInitialized()
    {
        dataMan.BarCode.Subscribe(e => _ = this.HandleBarCodeReadAsync());
    }

    private int currentSelectedItem;

    /// <summary>
    /// Handles the selection changed event for cycle rows.
    /// </summary>
    /// <param name="row">The selected row object.</param>
    public void SelectionChangedEvent(object row)
    {
        this.currentSelectedItem = ((Cycle)row)?.CycleId ?? 0;
        this.StateHasChanged();
    }
}
