// <copyright file="IBarCodeValidationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities.BarCodes;

using IndTrace.Domain.Enum;
using IndTrace.Domain.Models;

/// <summary>
/// Defines the contract for barcode validation services.
/// </summary>
public interface IBarCodeValidationService
{
    /// <summary>
    /// Gets the result of the last validation operation.
    /// </summary>
    Result? Result { get; }

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
    ResultValidation Validate(FlowStatus flowStatus, MachineType machineType, CycleStatus cycleStatus,
        PartStatus partStatus, int machineId, int nextMachineId);
}
