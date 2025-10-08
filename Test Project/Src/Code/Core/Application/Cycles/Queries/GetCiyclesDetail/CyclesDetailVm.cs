// <copyright file="CyclesDetailVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Queries.GetCiyclesDetail;

/// <summary>
/// Represents the CyclesDetailVm.
/// </summary>
public class CyclesDetailVm
{
    /// <summary>
    /// Gets or sets the CycleId.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the BarCodeId.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the PartStatus.
    /// </summary>
    public int PartStatus { get; set; }

    /// <summary>
    /// Gets or sets the CycleStatus.
    /// </summary>
    public int CycleStatus { get; set; }

    /// <summary>
    /// Gets or sets the CycleTime.
    /// </summary>
    public int CycleTime { get; set; }

    /// <summary>
    /// Gets or sets the TaktTime.
    /// </summary>
    public int TaktTime { get; set; }

    /// <summary>
    /// Gets or sets the StartedOn.
    /// </summary>
    public DateTime StartedOn { get; set; }

    /// <summary>
    /// Gets or sets the FinishedOn.
    /// </summary>
    public DateTime FinishedOn { get; set; }

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<CyclesDetailVm> ToDto(Cycle src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<CyclesDetailVm>.WithFailure("Cycle source cannot be null");
        }

        return IndQuestResults.Result<CyclesDetailVm>.Success(new CyclesDetailVm
        {
            CycleId = src.CycleId,
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            CycleStatus = src.CycleStatus,
            PartStatus = src.PartStatus,
            CycleTime = src.CycleTime,
            TaktTime = src.TaktTime,
            StartedOn = src.StartedOn,
            FinishedOn = src.FinishedOn,
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<Cycle> ToEntity(CyclesDetailVm src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<Cycle>.WithFailure("CyclesDetailVm source cannot be null");
        }

        return IndQuestResults.Result<Cycle>.Success(new Cycle
        {
            CycleId = src.CycleId,
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            CycleStatus = src.CycleStatus,
            PartStatus = src.PartStatus,
            CycleTime = src.CycleTime,
            TaktTime = src.TaktTime,
            StartedOn = src.StartedOn,
            FinishedOn = src.FinishedOn,
        });
    }
}
