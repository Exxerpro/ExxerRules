// <copyright file="CyclesDto.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Queries.GetCyclesList;

/// <summary>
/// Represents the CyclesDto.
/// </summary>
public class CyclesDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CyclesDto"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public CyclesDto()
    {
        this.BarCode = new BarCode();
        this.Machine = new Machine();
        this.Product = new Product();
    }

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
    /// Gets or sets the CycleStatus.
    /// </summary>
    public int CycleStatus { get; set; }

    /// <summary>
    /// Gets or sets the CyclesOk.
    /// </summary>
    public int CyclesOk { get; set; }

    /// <summary>
    /// Gets or sets the PartStatus.
    /// </summary>
    public int PartStatus { get; set; }

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
    /// Gets or sets the StatusCicloId.
    /// </summary>
    public int StatusCicloId { get; set; }

    /// <summary>
    /// Gets or sets the BarCode.
    /// </summary>
    public virtual BarCode BarCode { get; set; }

    /// <summary>
    /// Gets or sets the Machine.
    /// </summary>
    public virtual Machine Machine { get; set; }

    /// <summary>
    /// Gets or sets the Product.
    /// </summary>
    public virtual Product Product { get; set; }

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<CyclesDto> ToDto(Cycle src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<CyclesDto>.WithFailure("Cycle source cannot be null");
        }

        return IndQuestResults.Result<CyclesDto>.Success(new CyclesDto
        {
            CycleId = src.CycleId,
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            CycleStatus = src.CycleStatus,
            CyclesOk = src.CyclesOk,
            PartStatus = src.PartStatus,
            CycleTime = src.CycleTime,
            TaktTime = src.TaktTime,
            StartedOn = src.StartedOn,
            FinishedOn = src.FinishedOn,

            // StatusCicloId, BarCode, Machine, Product are not mapped from Cycle directly
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<Cycle> ToEntity(CyclesDto src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<Cycle>.WithFailure("CyclesDto source cannot be null");
        }

        return IndQuestResults.Result<Cycle>.Success(new Cycle
        {
            CycleId = src.CycleId,
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            CycleStatus = src.CycleStatus,
            CyclesOk = src.CyclesOk,
            PartStatus = src.PartStatus,
            CycleTime = src.CycleTime,
            TaktTime = src.TaktTime,
            StartedOn = src.StartedOn,
            FinishedOn = src.FinishedOn,
        });
    }

    /// <summary>
    /// Executes ToDtoList operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDtoList.</returns>
    public static IndQuestResults.Result<List<CyclesDto>> ToDtoList(IEnumerable<Cycle> src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<List<CyclesDto>>.WithFailure("Cycle collection cannot be null");
        }

        var list = src.Select(s => new CyclesDto
        {
            CycleId = s.CycleId,
            MachineId = s.MachineId,
            BarCodeId = s.BarCodeId,
            CycleStatus = s.CycleStatus,
            CyclesOk = s.CyclesOk,
            PartStatus = s.PartStatus,
            CycleTime = s.CycleTime,
            TaktTime = s.TaktTime,
            StartedOn = s.StartedOn,
            FinishedOn = s.FinishedOn,
        }).ToList();
        return IndQuestResults.Result<List<CyclesDto>>.Success(list);
    }
}
