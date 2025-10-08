// <copyright file="CognexBackGroundWorker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.CognexComm;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndTrace.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Background service for managing Cognex DataMan barcode reader operations.
/// </summary>
public class CognexBackGroundWorker : BackgroundService
{
    private readonly ILogger<CognexBackGroundWorker>? logger;

    private readonly DataMan dataMan;

    private readonly IBarCodeScanned barCodeScanned;

    /// <summary>
    /// Gets or sets the current result from the barcode reader.
    /// </summary>
    public string? Result;

    private readonly IMonitorRequestDispatcher monitorRequestDispatcher;

    private bool isDataManInitialized;
    private DateTime lasTimeCodeRead;
    private readonly DataManSettings dataManSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="CognexBackGroundWorker"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="dataMan">The DataMan instance.</param>
    /// <param name="barCodeScanned">The barcode scanned notification.</param>
    /// <param name="monitorRequestDispatcher">The Monitor command dispatcher.</param>
    /// <param name="dataManSettings">The DataMan configuration settings.</param>
    public CognexBackGroundWorker(
        DataMan dataMan,
        IBarCodeScanned barCodeScanned,
        IMonitorRequestDispatcher monitorRequestDispatcher,
        IOptions<DataManSettings> dataManSettings,
        ILogger<CognexBackGroundWorker>? logger = null)
    {
        this.logger = logger;
        this.monitorRequestDispatcher = monitorRequestDispatcher ?? throw new ArgumentNullException(nameof(monitorRequestDispatcher));

        this.barCodeScanned = barCodeScanned;
        this.dataMan = dataMan;

        this.dataManSettings = dataManSettings.Value;

        if (dataMan != null)
        {
            this.SetupDataMan(this.dataManSettings);
        }

        this.logger?.LogInformation("CognexBackGroundWorker started at : {time}", DateTimeOffset.Now.ToLocalTime());
    }

    /// <summary>
    /// Executes the background service operation.
    /// </summary>
    /// <param name="stoppingToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTime.Now.Minute % 5 == 0 && DateTime.Now.Second == 0)
            {
                this.logger?.LogInformation("DataMan Worker running at: {time}", DateTimeOffset.Now.ToLocalTime());
                this.logger?.LogInformation("Last Code read was {Result} at {_lasTimeCodeRead}", this.Result,
                    this.lasTimeCodeRead);
            }

            if (this.dataMan != null && this.isDataManInitialized == false)
            {
                this.SetupDataMan(this.dataManSettings);
            }

            if (this.dataMan != null && !this.dataMan.IsConnected)
            {
                this.dataMan.Connect();
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    /// <summary>
    /// Sets up the DataMan barcode reader with the specified settings.
    /// </summary>
    /// <param name="dataManSettings">The DataMan configuration settings.</param>
    public void SetupDataMan(DataManSettings dataManSettings)
    {
        try
        {
            // TODO get parameters from configuration o from database
            this.dataMan.Connect();
            this.isDataManInitialized = true;
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Error in CognexBackGroundWorker operation");
            throw;
        }

        try
        {
            this.dataMan.BarCode.Subscribe(_ => Console.WriteLine(this.dataMan.Result));
            this.dataMan.BarCode.Subscribe(_ => this.logger?.LogInformation(this.dataMan.Result));
            this.dataMan.BarCode.Subscribe(async _ => await this.PublishEventBarCodeScanned(this.dataMan.Result));
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Error in CognexBackGroundWorker secondary operation");
            throw;
        }
    }

    /// <summary>
    /// Publishes an event when a barcode is scanned.
    /// </summary>
    /// <param name="barCode">The scanned barcode value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task PublishEventBarCodeScanned(string? barCode)
    {
        // TODO get some valid samples BAR CODES and move this after the validation
        this.lasTimeCodeRead = DateTime.Now;

        this.Result = barCode;
        this.barCodeScanned.Label = barCode;

        if (this.dataMan.ValidateLabel(barCode))
        {
            return Task.CompletedTask;
        }

        if (this.monitorRequestDispatcher is null)
        {
            this.logger?.LogError("_monitorRequestDispatcher is null in task RequestBarCodeDetailAsync ");
        }

        if (this.barCodeScanned is null)
        {
            this.logger?.LogError("_barCodeScanned is null in task RequestBarCodeDetailAsync ");
        }

        // TODO DEFINE HOW IS TO BE PUBLISHED
        return Task.CompletedTask;
    }
}

/// <summary>
/// Provides helper methods for DataMan operations.
/// </summary>
public static class DataManHelper
{
    /// <summary>
    /// Validates a barcode label to ensure it meets the required format.
    /// </summary>
    /// <param name="dataMan">The DataMan instance.</param>
    /// <param name="barCode">The barcode to validate.</param>
    /// <returns>True if the barcode is valid; otherwise, false.</returns>
    public static bool ValidateLabel(this DataMan dataMan, string? barCode)
    {
        // [Fix]
        // CLAUDE
        // Date: 02/09/2025
        // Reason: [Nullable Reference] - Add null check before trimming to prevent CS8602 warning
        if (barCode is null)
        {
            return false;
        }

        // Trim the barcode to remove any leading or trailing whitespace or non-visible characters
        barCode = barCode.Trim();

        // Check if all characters are alphanumeric
        if (barCode.Any(c => !char.IsLetterOrDigit(c)))
        {
            return false;
        }

        return true;
    }
}
