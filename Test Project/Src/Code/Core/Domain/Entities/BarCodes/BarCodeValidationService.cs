// <copyright file="BarCodeValidationService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities.BarCodes
{
    using IndTrace.Domain.Enum;
    using IndTrace.Domain.Models;

    /// <summary>
    /// Provides validation logic for barcode operations based on flow status, machine type, and cycle status.
    /// </summary>
    public class BarCodeValidationService : IBarCodeValidationService
    {
        /// <summary>
        /// Gets the result of the last validation operation.
        /// </summary>
        public Result? Result { get; private set; }

        /// <summary>
        /// Validates the barcode operation based on the provided statuses and machine information.
        /// </summary>
        /// <param name="flowStatus">The flow status of the part.</param>
        /// <param name="machineType">The type of machine involved.</param>
        /// <param name="cycleStatus">The status of the cycle.</param>
        /// <param name="partStatus">The status of the part.</param>
        /// <param name="machineId">The current machine ID.</param>
        /// <param name="nextMachineId">The expected next machine ID.</param>
        /// <returns>A <see cref="ResultValidation"/> indicating the validation outcome.</returns>
        public ResultValidation Validate(FlowStatus flowStatus, MachineType machineType, CycleStatus cycleStatus,
            PartStatus partStatus, int machineId, int nextMachineId)
        {
            ArgumentNullException.ThrowIfNull(flowStatus);
            ArgumentNullException.ThrowIfNull(machineType);
            ArgumentNullException.ThrowIfNull(cycleStatus);

            if (!Equals(partStatus, PartStatus.Ok) && !Equals(cycleStatus, CycleStatus.FinishedNok))
            {
                this.Result = Result.WithFailure($"Status Invalid {partStatus} cycle status {cycleStatus}");
                return ResultValidation.PartNotValid;
            }

            if (nextMachineId != machineId)
            {
                this.Result = Result.WithFailure($"Machine {machineId} is not the next machine {nextMachineId}");
                return ResultValidation.DestinationNotValid;
            }

            if (flowStatus.Value == FlowStatus.Rejected)
            {
                return ResultValidation.PartRejected;
            }

            var key = (FlowStatus: flowStatus, MachineType: machineType, CycleStatus: cycleStatus);

            var result = key switch
            {
                // Valid Cases
                // When the flow is created, the machine is a printer, and the cycle has started.
                var (fs, mt, cs) when Equals(fs, FlowStatus.Created) && Equals(mt, MachineType.Printer) &&
                                      Equals(cs, CycleStatus.Started) => ResultValidation.Valid,

                // When the flow is created, the machine is an initial printer, and the cycle has started.
                var (fs, mt, cs) when Equals(fs, FlowStatus.Created) && Equals(mt, MachineType.InitialPrinter) &&
                                      Equals(cs, CycleStatus.Started) => ResultValidation.Valid,

                // When the flow is created and the machine is set to initial.
                var (fs, mt, _) when Equals(fs, FlowStatus.Created) && Equals(mt, MachineType.Initial)
                                            => ResultValidation.Valid,

                // When the flow is created and the machine is set to initial.
                var (fs, mt, _) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.InitialPrinter)
                                            => ResultValidation.Valid,

                // When the flow is in process and the machine is in the process state.
                var (fs, mt, _) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.Process)
                                            => ResultValidation.Valid,

                // When the flow is in process, the machine is in the final state, and the cycle hasn't started.
                var (fs, mt, cs) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.Final) &&
                                      Equals(cs, CycleStatus.NotStarted) => ResultValidation.Valid,

                // When the flow is in process, the machine is in the final state, and the cycle finished successfully.
                var (fs, mt, cs) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.Final) &&
                                      Equals(cs, CycleStatus.FinishedOk) => ResultValidation.Valid,

                // When the flow is in process, the machine is in the final state, and the cycle finished nOK.
                var (fs, mt, cs) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.Final) &&
                                      Equals(cs, CycleStatus.FinishedNok) => ResultValidation.Valid,

                // When the flow is in process, the machine is a dashboard, and the cycle hasn't started.
                var (fs, mt, cs) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.DashBoard) &&
                                      Equals(cs, CycleStatus.NotStarted) => ResultValidation.Valid,

                // When the flow is in process, the machine is a dashboard, and the cycle finished successfully.
                var (fs, mt, cs) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.DashBoard) &&
                                      Equals(cs, CycleStatus.FinishedOk) => ResultValidation.Valid,

                // When the flow is in process, the machine is a printer, and the cycle has started.
                var (fs, mt, cs) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.Printer) &&
                                      Equals(cs, CycleStatus.Started) => ResultValidation.Valid,

                // When the flow is in process, the machine is a dashboard, and the cycle has started.
                var (fs, mt, cs) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.Final) &&
                                      Equals(cs, CycleStatus.Started) => ResultValidation.Valid,

                // Invalid Cases
                // When the flow is in process and the machine is in the final state (regardless of cycle status).
                var (fs, mt, _) when Equals(fs, FlowStatus.InProcess) && Equals(mt, MachineType.Final)
                                        => ResultValidation.WorkFlowNotValid,

                // When the flow has finished, the machine is a dashboard, and the cycle finished successfully.
                var (fs, mt, cs) when Equals(fs, FlowStatus.Finished) && Equals(mt, MachineType.DashBoard) &&
                                      Equals(cs, CycleStatus.FinishedOk) => ResultValidation.WorkFlowNotValid,

                // Default case when none of the above conditions are met.
                _ => ResultValidation.Invalid,
            };

            if (Equals(result, ResultValidation.Invalid))
            {
                this.Result = Result.WithFailure($"Status Invalid {flowStatus} machine type {machineType} cycle status {cycleStatus}");
            }

            return result;
        }
    }
}
