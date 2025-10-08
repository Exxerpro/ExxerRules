// <copyright file="PlcManager.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Infrastructure.Services;

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Linq;
using System.Runtime.ExceptionServices;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using IndTrace.DataStore.ModelsComs;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Models;
using Sharp7.Rx;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;

/// <summary>
/// Service for fetching performance data from S7 PLCs.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate PlcManager logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the PlcManager.
/// </summary>
public class PlcManager(ILogger logger) : IPlcManager
{
    /// <summary>
    /// Gets the PLCs data managed by this service.
    /// </summary>
    [Required]
    private ReadOnlyDictionary<int, PlcData> plcManagedData = null!;

    /// <summary>
    /// Gets the PLC processors managed by this service.
    /// </summary>
    [Required]
    private ReadOnlyDictionary<int, IPlcProcessor> plcProcessors = null!;

    /// <summary>
    /// Initializes the PlcManager with PLC data and performance tags.
    /// </summary>
    /// <param name="plcsData">
    /// The PLCs data.
    /// </param>
    /// <param name="performanceTags">
    /// The performance tags.
    /// </param>
    /// <returns>
    /// The initialization result.
    /// </returns>
    public async Task<Result> InitializeAsync(IReadOnlyDictionary<int, PlcData> plcsData, IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>> performanceTags)
    {
        var verifyPlcHavePerformanceTags = this.VerifyPlcHavePerformanceTags(plcsData, performanceTags);

        var dataIsValid = this.VerifyDataIsValid(plcsData);
        if (dataIsValid.IsFailure)
        {
            return dataIsValid;
        }

        this.CreatePlcCollections(plcsData, performanceTags);

        // Initialize each one of the PLCs
        foreach (var (id, processor) in this.plcProcessors)
        {
            await processor.InitializeAsync()
                      .ContinueWith(task =>
                      {
                          if (task.IsFaulted)
                          {
                              logger.LogError("Error initializing PLC {PlcId}: {Message}", id, task.Exception?.Message);

                              // HandleAsync initialization error
                          }
                          else
                          {
                              logger.LogInformation("PLC {PlcId} initialized successfully.", id);
                          }
                      });
        }

        /*
        *for code enhancer
        *
        *leave this space here to code missing functionality
        *
        *
        */
        return verifyPlcHavePerformanceTags > 0
            ? Result.WithFailure($"There are {verifyPlcHavePerformanceTags} PLCs without tags. Please check the configuration.")
            : Result.Success();
    }

    /// <summary>
    /// Executes ReadPerformanceDataAsync operation.
    /// </summary>
    /// <param name="plcId">The plcId.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ReadPerformanceDataAsync.</returns>
    public async Task<Result<PerformanceData>> ReadPerformanceDataAsync(int plcId, CancellationToken cancellationToken)
    {
        if (this.plcManagedData.TryGetValue(plcId, out var plc))
        {
            if (this.plcProcessors.TryGetValue(plcId, out var processor))
            {
                try
                {
                    var result = await processor.ReadPerformanceDataFromPlcAsync(plcId, cancellationToken);
                    return result;
                }
                catch (Exception e)
                {
                    logger.LogError("Error fetching performance data for PLC {PlcId}: {Message}", plcId, e.Message);
                    return Result<PerformanceData>.WithFailure("Fail to fetch performance data: " + e.Message);
                }
            }
            else
            {
                logger.LogError("No processor found for PLC {PlcId}.", plcId);
                return Result<PerformanceData>.WithFailure($"Fail plc not found {plcId})");
            }
        }

        logger.LogError("No PLC data found for PLC {PlcId}.", plcId);
        return Result<PerformanceData>.WithFailure($"Fail plc not found {plcId})");
    }

    private void CreatePlcCollections(IReadOnlyDictionary<int, PlcData> plcs, IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>> performanceTags)
    {
        var plcFilteredDatas = plcs
            .Where(p => p.Value.OeeEnabled)
            .ToDictionary(p => p.Key, p => p.Value);

        this.plcManagedData = new ReadOnlyDictionary<int, PlcData>(plcFilteredDatas);

        // First create the PLCs with the performance tags, then create the processors.
        var plcProcessorsClasses = this.plcManagedData.ToDictionary(
            kvp => kvp.Key,
            kvp =>
            {
                performanceTags.TryGetValue(kvp.Key, out var tags);

                return new PlcProcessor(kvp.Key, kvp.Value, tags, logger);
            });

        // Create ReadOnlyDictionary for PLC processors
        this.plcProcessors = new ReadOnlyDictionary<int, IPlcProcessor>(
            plcProcessorsClasses.ToDictionary(
                kvp => kvp.Key,
                kvp => (IPlcProcessor)kvp.Value));
    }

    private Result VerifyDataIsValid(IReadOnlyDictionary<int, PlcData> plcs)
    {
        if (!plcs.Values.Any(plc => plc.OeeEnabled))
        {
            logger.LogCritical("No PLCs with performance tags found. Please check the configuration.");
            // Railway flow control: return failure instead of throwing for control flow
            return Result.WithFailure("No PLCs with performance tags found. Please check the configuration.");
        }

        return Result.Success();
    }

    private int VerifyPlcHavePerformanceTags(IReadOnlyDictionary<int, PlcData> plcs, IReadOnlyDictionary<int, IReadOnlyDictionary<string, VariableS7>> performanceTags)
    {
        var result = 0;
        foreach (var plc in plcs.Values)
        {
            logger.LogInformation("PLC found: {PlcId}, Name: {Name}, IP: {IpAddress}", plc.PlcId, plc.Name, plc.IpAddress);

            if (performanceTags.TryGetValue(plc.PlcId, out var tags) && tags.Any())
            {
                foreach (var tag in tags.Values)
                {
                    logger.LogInformation("TagDataStore found for PLC {PlcId}: {TagName} at {Address}", plc.PlcId, tag.Name, tag.Address);
                }

                plc.OeeEnabled = true;
            }
            else
            {
                // This validation is redundant, but we made it to ensure that the PLC is disabled if no tags are found.
                // This is to ensure that the PLC is not enabled if no tags are found.
                // Just to be very defensive in the code.
                result++;
                plc.OeeEnabled = false;
                plc.Enabled = -1;
                logger.LogError("No tags found for PLC {PlcId}. PLC will be disabled", plc.PlcId);
                logger.LogWarning("No tags found for PLC {PlcId}.", plc.PlcId);
            }
        }

        return result;
    }
}
