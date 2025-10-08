// <copyright file="IBarCodeResult.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities.BarCodes;

using IndTrace.Domain.Enum;

/// <summary>
/// Defines the contract for barcode result operations and properties, including validation, cycle, and workflow information.
/// </summary>
public interface IBarCodeResult
{
    /// <summary>
    /// Gets the machine identifier associated with the barcode result.
    /// </summary>
    int MachineId { get; }

    /// <summary>
    /// Gets the barcode identifier.
    /// </summary>
    int BarCodeId { get; }

    /// <summary>
    /// Gets the cycle identifier.
    /// </summary>
    int CycleId { get; }

    /// <summary>
    /// Gets the number of successful cycles.
    /// </summary>
    int CyclesOk { get; }

    /// <summary>
    /// Gets the shift identifier.
    /// </summary>
    int ShiftId { get; }

    /// <summary>
    /// Gets the command identifier.
    /// </summary>
    public int CommandId { get; }

    /// <summary>
    /// Gets or sets the result validation status.
    /// </summary>
    ResultValidation ResultValidation { get; set; }

    /// <summary>
    /// Gets the error message, if any.
    /// </summary>
    string? Error { get; }

    /// <summary>
    /// Gets the barcode label.
    /// </summary>
    string? Label { get; }

    /// <summary>
    /// Gets the part number associated with the barcode.
    /// </summary>
    string? PartNumber { get; }

    /// <summary>
    /// Gets the product associated with the barcode result.
    /// Note: This property enables service access to product details that were previously available in handlers.
    /// </summary>
    Product Product { get; }

    /// <summary>
    /// Gets the description of the barcode result.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the last machine identifier.
    /// </summary>
    int LastMachineId { get; }

    /// <summary>
    /// Gets the next machine identifier.
    /// </summary>
    int NextMachineId { get; }

    /// <summary>
    /// Gets the number of registers saved.
    /// </summary>
    int RegistersSaved { get; }

    /// <summary>
    /// Gets the cycle status.
    /// </summary>
    CycleStatus CycleStatus { get; }

    /// <summary>
    /// Gets the flow status.
    /// </summary>
    FlowStatus FlowStatus { get; }

    /// <summary>
    /// Gets the part status.
    /// </summary>
    PartStatus PartStatus { get; }

    /// <summary>
    /// Gets the machine type.
    /// </summary>
    MachineType MachineType { get; }

    /// <summary>
    /// Gets the workflow type.
    /// </summary>
    WorkFlowType WorkFlowType { get; }

    /// <summary>
    /// Gets the recipe associated with the barcode result.
    /// </summary>
    Recipe Recipe { get; }

    /// <summary>
    /// Gets the cycle associated with the barcode result.
    /// </summary>
    Cycle Cycle { get; }

    /// <summary>
    /// Gets the collection of cycles associated with the barcode result.
    /// </summary>
    IEnumerable<Cycle> Cycles { get; }

    /// <summary>
    /// Gets the barcode entity.
    /// </summary>
    BarCode BarCode { get; }

    /// <summary>
    /// Gets the master label entity.
    /// </summary>
    MasterLabel MasterLabel { get; }

    /// <summary>
    /// Gets the dictionary of references associated with the barcode result.
    /// </summary>
    IDictionary<string, Register> References { get; }

    /// <summary>
    /// Asynchronously retrieves barcode details for the specified request.
    /// </summary>
    /// <param name="barCodeDetailsRequest">The barcode details request.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, with the barcode result.</returns>
    Task<IBarCodeResult> GetBarCodeDetails(BarCodeDetailsRequest barCodeDetailsRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Updates barcode information for the current cycle.
    /// </summary>
    /// <param name="flowStatus">The flow status.</param>
    /// <param name="partStatus">The part status.</param>
    /// <param name="cycleStatus">The cycle status.</param>
    void UpdateBarCodeInformationOnCycle(FlowStatus flowStatus, PartStatus partStatus, CycleStatus cycleStatus);

    /// <summary>
    /// Sets the cycle for the barcode result.
    /// </summary>
    /// <param name="cycle">The cycle to set.</param>
    void SetCycle(Cycle cycle);

    /// <summary>
    /// Sets the barcode entity for the result.
    /// </summary>
    /// <param name="barCode">The barcode to set.</param>
    void SetBarCode(BarCode barCode);

    /// <summary>
    /// Sets the number of successful cycles.
    /// </summary>
    /// <param name="cyclesOk">The number of successful cycles.</param>
    void SetCyclesOk(int cyclesOk);

    /// <summary>
    /// Sets the number of registers saved.
    /// </summary>
    /// <param name="registers">The number of registers saved.</param>
    void SetRegistersSaved(int registers);
}
