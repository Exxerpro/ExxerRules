// <copyright file="MachineStateEvaluator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Validation;

using IndTrace.DataStore.DataAccess;
using IndTrace.DataStore.Interfaces;
using IndTrace.Simulator.Models.Constants;

/// <summary>
/// Evaluates the next expected machine state based on the current context and static snapshot.
/// </summary>
public class MachineStateEvaluator : IMachineStateEvaluator
{
    /// <summary>
    /// Builds the next expected state for a fixture task based on the current context and static workflow snapshot.
    /// </summary>
    /// <param name="context">The current fixture context.</param>
    /// <param name="staticSnapshot">The static workflow snapshot for the fixture.</param>
    /// <returns>The expected <see cref="FixtureTaskSnapshot"/> for the next operation.</returns>
    public FixtureTaskSnapshot BuildNextExpectedState(IFixtureContext context, FixtureStaticSnapshot staticSnapshot)
    {
        Enum.TryParse<GatewayTask>(context.TaskName, out var task);

        var machineId = context.MachineId;

        var isInitialPrinter = staticSnapshot.WorkFlows.Any(wf =>
            wf.ProductId == staticSnapshot.ProductId &&
            wf.LastMachineId == 0 &&
            wf.NextMachineId == machineId);

        // Attempt to resolve based on current MachineId's workflow context
        var previousStep = staticSnapshot.WorkFlows
            .FirstOrDefault(wf => wf.NextMachineId == context.MachineId);

        var nextStep = staticSnapshot.WorkFlows
            .FirstOrDefault(wf => wf.LastMachineId == context.MachineId);

        var lastMachineId = staticSnapshot.WorkFlows.FirstOrDefault(wf => wf.NextMachineId == machineId)?.LastMachineId ?? 0;
        var nextMachineId = staticSnapshot.WorkFlows.FirstOrDefault(wf => wf.LastMachineId == machineId)?.NextMachineId ?? 0;

        var initialMachine = previousStep?.LastMachineId == 0;
        var finalMachine = nextStep?.NextMachineId == 0;

        // var (initialMachine, finalMachine) = DetectInitialFinalMachine(context, lastMachineId, nextMachineId);
        return task switch
        {
            // Happy path - Create barcode and start cycle from initial printer
            GatewayTask.CreateBarCodeAsync when initialMachine => new FixtureTaskSnapshot
            {
                Source = "TASK",
                BarCodeId = 0,
                Barcode = context.Barcode,
                ProductId = staticSnapshot.ProductId,
                CycleId = 0,
                CycleStatus = (int)CycleStatusEnum.Started,
                PartStatus = (int)PartStatusEnum.Ok,
                FlowStatus = (int)FlowStatusEnum.Created,
                ResultValidation = (int)ResultValidationEnum.Valid,
                LastMachineId = machineId,
                NextMachineId = machineId,
                CycleMachineId = machineId,
                BarcodeMachineId = machineId,
                ActualStation = machineId,
            },

            // Happy path - read barcode on a downstream machine
            GatewayTask.ReadBarCodeAsync => new FixtureTaskSnapshot
            {
                Source = "TASK",
                BarCodeId = 0,
                Barcode = context.Barcode,
                ProductId = staticSnapshot.ProductId,
                CycleId = 0,
                CycleStatus = (int)CycleStatusEnum.FinishedOk,
                PartStatus = (int)PartStatusEnum.Ok,
                FlowStatus = (int)FlowStatusEnum.InProcess,
                ResultValidation = (int)ResultValidationEnum.Valid,
                LastMachineId = lastMachineId,
                NextMachineId = machineId,
                CycleMachineId = machineId,
                BarcodeMachineId = machineId,
                ActualStation = machineId,
            },

            // Happy path - Create a new cycle on a downstream machine
            GatewayTask.CreateCycleAsync => new FixtureTaskSnapshot
            {
                Source = "TASK",
                BarCodeId = 0,
                Barcode = context.Barcode,
                ProductId = staticSnapshot.ProductId,
                CycleId = 0,
                CycleStatus = (int)CycleStatusEnum.Started,
                PartStatus = (int)PartStatusEnum.Ok,
                FlowStatus = (int)FlowStatusEnum.InProcess,
                ResultValidation = (int)ResultValidationEnum.Valid,
                LastMachineId = machineId,
                NextMachineId = machineId,
                CycleMachineId = machineId,
                BarcodeMachineId = machineId,
                ActualStation = machineId,
            },

            // Happy path - Update cycle as OK
            GatewayTask.UpdateCycleOkAsync when finalMachine => new FixtureTaskSnapshot
            {
                Source = "TASK",
                BarCodeId = 0,
                Barcode = context.Barcode,
                ProductId = staticSnapshot.ProductId,
                CycleId = 0,
                CycleStatus = (int)CycleStatusEnum.FinishedOk,
                PartStatus = (int)PartStatusEnum.Ok,
                FlowStatus = (int)FlowStatusEnum.Finished,
                ResultValidation = (int)ResultValidationEnum.Valid,
                LastMachineId = machineId,
                NextMachineId = machineId,
                CycleMachineId = machineId,
                BarcodeMachineId = machineId,
                ActualStation = machineId,
            },

            // Happy path - Update cycle as OK
            GatewayTask.UpdateCycleOkAsync => new FixtureTaskSnapshot
            {
                Source = "TASK",
                BarCodeId = 0,
                Barcode = context.Barcode,
                ProductId = staticSnapshot.ProductId,
                CycleId = 0,
                CycleStatus = (int)CycleStatusEnum.FinishedOk,
                PartStatus = (int)PartStatusEnum.Ok,
                FlowStatus = (int)FlowStatusEnum.InProcess,
                ResultValidation = (int)ResultValidationEnum.Valid,
                LastMachineId = machineId,
                NextMachineId = machineId,
                CycleMachineId = machineId,
                BarcodeMachineId = machineId,
                ActualStation = machineId,
            },

            // Happy path - Update cycle as NOK
            GatewayTask.UpdateCycleNotOkAsync => new FixtureTaskSnapshot
            {
                Source = "TASK",
                BarCodeId = 0,
                Barcode = context.Barcode,
                ProductId = staticSnapshot.ProductId,
                CycleId = 0,
                CycleStatus = (int)CycleStatusEnum.FinishedNok,
                PartStatus = (int)PartStatusEnum.NOK,
                FlowStatus = (int)FlowStatusEnum.InProcess,
                ResultValidation = (int)ResultValidationEnum.Valid,
                LastMachineId = machineId,
                NextMachineId = machineId,
                CycleMachineId = machineId,
                BarcodeMachineId = machineId,
                ActualStation = machineId,
            },

            _ => new FixtureTaskSnapshot
            {
                Source = "TASK",
                ResultValidation = (int)ResultValidationEnum.Invalid,
                Barcode = context.Barcode,
                ProductId = staticSnapshot.ProductId,
                ActualStation = machineId,
            },
        };
    }
}
