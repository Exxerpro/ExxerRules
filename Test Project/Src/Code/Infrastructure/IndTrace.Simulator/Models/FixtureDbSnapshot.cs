// <copyright file="FixtureDbSnapshot.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

using IndTrace.DataStore.DataAccess;
using IndTrace.Simulator.Validation;

/// <summary>
/// Represents a snapshot of the fixture database state, used for validation and simulation.
/// </summary>
public class FixtureDbSnapshot : FixtureValidationResult
{
    /// <summary>
    /// Converts this snapshot to a <see cref="FixtureValidationResult"/> instance.
    /// </summary>
    /// <returns>A <see cref="FixtureValidationResult"/> representing the current snapshot.</returns>
    public FixtureValidationResult ToValidationResult()
    {
        return new FixtureValidationResult
        {
            Source = "DB",
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

    /// <summary>
    /// Creates a <see cref="FixtureDbSnapshot"/> from barcode, static snapshot, and context information.
    /// </summary>
    /// <param name="barcode">The barcode database record.</param>
    /// <param name="staticSnapshot">The static snapshot of the fixture.</param>
    /// <param name="context">The fixture context.</param>
    /// <returns>A new <see cref="FixtureDbSnapshot"/> instance.</returns>
    public static FixtureDbSnapshot FromBarcodeAndStatic(
        BarcodeDbRecord barcode,
        FixtureStaticSnapshot staticSnapshot,
        FixtureContext context)
    {
        var workflow = staticSnapshot.WorkFlows
            .FirstOrDefault(wf => wf.ProductId == staticSnapshot.ProductId);

        // Attempt to resolve based on current MachineId's workflow context
        var previousStep = staticSnapshot.WorkFlows
            .FirstOrDefault(wf => wf.NextMachineId == barcode.MachineId);

        var nextStep = staticSnapshot.WorkFlows
            .FirstOrDefault(wf => wf.LastMachineId == barcode.MachineId);

        var lastMachineId = staticSnapshot.WorkFlows.FirstOrDefault(wf => wf.NextMachineId == context.MachineId)?.LastMachineId ?? 0;
        var nextMachineId = staticSnapshot.WorkFlows.FirstOrDefault(wf => wf.LastMachineId == context.MachineId)?.NextMachineId ?? 0;

        var initialMachine = previousStep?.LastMachineId == 0;
        var finalMachine = nextStep?.NextMachineId == 0;

        // var (initialMachine, finalMachine) = DetectInitialFinalMachine(context, lastMachineId, nextMachineId);
        return context.TaskName switch
        {
            nameof(GatewayTask.CreateBarCodeAsync) when initialMachine => new FixtureDbSnapshot
            {
                Source = "DB",
                Barcode = barcode.Barcode,
                BarCodeId = barcode.BarCodeId,
                ProductId = staticSnapshot.ProductId,
                CycleStatus = barcode.CycleStatus,
                PartStatus = barcode.PartStatus,
                FlowStatus = barcode.FlowStatus,
                CycleId = barcode.CycleId,
                ResultValidation = barcode.ResultValidation,
                LastMachineId = previousStep?.LastMachineId ?? barcode.MachineId,
                NextMachineId = previousStep?.NextMachineId ?? barcode.MachineId,
                ActualStation = barcode.MachineId,
                CycleMachineId = barcode.CycleMachineId,
                BarcodeMachineId = barcode.MachineId,
            },

            nameof(GatewayTask.ReadBarCodeAsync) => new FixtureDbSnapshot
            {
                Source = "DB",
                Barcode = barcode.Barcode,
                BarCodeId = barcode.BarCodeId,
                ProductId = staticSnapshot.ProductId,
                CycleStatus = barcode.CycleStatus,
                PartStatus = barcode.PartStatus,
                FlowStatus = barcode.FlowStatus,
                CycleId = barcode.CycleId,
                ResultValidation = barcode.ResultValidation,
                LastMachineId = nextStep?.LastMachineId ?? barcode.MachineId,
                NextMachineId = nextStep?.NextMachineId ?? barcode.MachineId,
                ActualStation = context.MachineId,
                CycleMachineId = barcode.CycleMachineId,
                BarcodeMachineId = barcode.MachineId,
            },
            nameof(GatewayTask.CreateCycleAsync) => new FixtureDbSnapshot
            {
                Source = "DB",
                Barcode = barcode.Barcode,
                BarCodeId = barcode.BarCodeId,
                ProductId = staticSnapshot.ProductId,
                CycleStatus = barcode.CycleStatus,
                PartStatus = barcode.PartStatus,
                FlowStatus = barcode.FlowStatus,
                CycleId = barcode.CycleId,
                ResultValidation = barcode.ResultValidation,
                LastMachineId = previousStep?.LastMachineId ?? barcode.MachineId,
                NextMachineId = barcode.MachineId,
                ActualStation = barcode.MachineId,
                CycleMachineId = barcode.CycleMachineId,
                BarcodeMachineId = barcode.MachineId,
            },

            nameof(GatewayTask.UpdateCycleOkAsync) when finalMachine => new FixtureDbSnapshot
            {
                Source = "DB",
                Barcode = barcode.Barcode,
                BarCodeId = barcode.BarCodeId,
                ProductId = staticSnapshot.ProductId,
                CycleStatus = barcode.CycleStatus,
                PartStatus = barcode.PartStatus,
                FlowStatus = barcode.FlowStatus,
                CycleId = barcode.CycleId,
                ResultValidation = barcode.ResultValidation,
                LastMachineId = barcode.MachineId,
                NextMachineId = barcode.MachineId,
                ActualStation = barcode.MachineId,
                CycleMachineId = barcode.CycleMachineId,
                BarcodeMachineId = barcode.MachineId,
            },

            nameof(GatewayTask.UpdateCycleOkAsync) => new FixtureDbSnapshot
            {
                Source = "DB",
                Barcode = barcode.Barcode,
                BarCodeId = barcode.BarCodeId,
                ProductId = staticSnapshot.ProductId,
                CycleStatus = barcode.CycleStatus,
                PartStatus = barcode.PartStatus,
                FlowStatus = barcode.FlowStatus,
                CycleId = barcode.CycleId,
                ResultValidation = barcode.ResultValidation,
                LastMachineId = barcode.MachineId,
                NextMachineId = barcode.MachineId,
                ActualStation = barcode.MachineId,
                CycleMachineId = barcode.CycleMachineId,
                BarcodeMachineId = barcode.MachineId,
            },

            nameof(GatewayTask.UpdateCycleNotOkAsync) => new FixtureDbSnapshot
            {
                Source = "DB",
                Barcode = barcode.Barcode,
                BarCodeId = barcode.BarCodeId,
                ProductId = staticSnapshot.ProductId,
                CycleStatus = barcode.CycleStatus,
                PartStatus = barcode.PartStatus,
                FlowStatus = barcode.FlowStatus,
                CycleId = barcode.CycleId,
                ResultValidation = barcode.ResultValidation,
                LastMachineId = barcode.MachineId,
                NextMachineId = barcode.MachineId,
                ActualStation = barcode.MachineId,
                CycleMachineId = barcode.CycleMachineId,
                BarcodeMachineId = barcode.MachineId,
            },

            _ => new FixtureDbSnapshot
            {
                Source = "DB",
                Barcode = barcode.Barcode,
                BarCodeId = barcode.BarCodeId,
                ProductId = staticSnapshot.ProductId,
                CycleStatus = barcode.CycleStatus,
                PartStatus = barcode.PartStatus,
                FlowStatus = barcode.FlowStatus,
                CycleId = barcode.CycleId,
                ResultValidation = barcode.ResultValidation,
                LastMachineId = previousStep?.LastMachineId ?? barcode.MachineId,
                NextMachineId = nextStep?.NextMachineId ?? barcode.MachineId,
                ActualStation = barcode.MachineId,
                CycleMachineId = barcode.CycleMachineId,
                BarcodeMachineId = barcode.MachineId,
            },
        };
    }
}
