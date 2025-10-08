// <copyright file="FixturePlcSnapshot.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

using IndTrace.DataStore.ModelsComs;

/// <summary>
/// Represents a snapshot of PLC data for fixture validation, containing tag values and context information.
/// </summary>
public class FixturePlcSnapshot : FixtureValidationResult
{
    /// <summary>
    /// Creates a new fixture PLC snapshot from tag values and context.
    /// </summary>
    /// <param name="tags">The dictionary of PLC tags and their values.</param>
    /// <param name="context">The fixture context containing barcode and machine information.</param>
    /// <returns>A new fixture PLC snapshot instance.</returns>
    public static FixturePlcSnapshot FromTagsValues(Dictionary<string, VariableS7> tags, FixtureContext context)
    {
        return new FixturePlcSnapshot
        {
            Source = "PLC",
            Barcode = context.Barcode,
            BarCodeId = tags.GetValueOrDefault("BarCodeId")?.ValueInt ?? 0,
            CycleId = tags.GetValueOrDefault("CycleId")?.ValueInt ?? 0,
            CycleStatus = tags.GetValueOrDefault("CycleStatus")?.ValueInt ?? 0,
            PartStatus = tags.GetValueOrDefault("PartStatus")?.ValueInt ?? 0,
            FlowStatus = tags.GetValueOrDefault("FlowStatus")?.ValueInt ?? 0,
            ResultValidation = tags.GetValueOrDefault("ResultValidation")?.ValueInt ?? 0,
            LastMachineId = tags.GetValueOrDefault("LastMachineId")?.ValueInt ?? 0,
            NextMachineId = tags.GetValueOrDefault("NextMachineId")?.ValueInt ?? 0,
            ActualStation = context.MachineId,
        };
    }

    /// <summary>
    /// Converts this PLC snapshot to a validation result.
    /// </summary>
    /// <returns>A fixture validation result containing the PLC snapshot data.</returns>
    public FixtureValidationResult ToValidationResult()
    {
        return new FixtureValidationResult
        {
            Source = "PLC",
            Barcode = this.Barcode,
            BarCodeId = this.BarCodeId,
            ProductId = this.ProductId,
            CycleStatus = this.CycleStatus,
            PartStatus = this.PartStatus,
            FlowStatus = this.FlowStatus,
            CycleId = this.CycleId,
            ResultValidation = this.ResultValidation,
            LastMachineId = this.LastMachineId,
            NextMachineId = this.NextMachineId,
            ActualStation = this.ActualStation,
            CycleMachineId = this.CycleMachineId,
            BarcodeMachineId = this.BarcodeMachineId,
        };
    }
}
