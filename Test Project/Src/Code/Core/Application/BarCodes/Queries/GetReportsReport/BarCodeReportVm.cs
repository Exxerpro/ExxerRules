// <copyright file="BarCodeReportVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetReportsReport;

/// <summary>
/// Represents the BarCodeReportVm.
/// </summary>
public class BarCodeReportVm
{
    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the BarCodeId.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the CycleId.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the Label.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the ProductId.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the LastMachineId.
    /// </summary>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the NextMachineId.
    /// </summary>
    public int NextMachineId { get; set; }

    /// <summary>
    /// Gets or sets the CycleStatus.
    /// </summary>
    public CycleStatus CycleStatus { get; set; }

    /// <summary>
    /// Gets or sets the FlowStatus.
    /// </summary>
    public FlowStatus FlowStatus { get; set; }

    /// <summary>
    /// Gets or sets the PartStatus.
    /// </summary>
    public PartStatus PartStatus { get; set; }

    /// <summary>
    /// Gets or sets the MachineType.
    /// </summary>
    public MachineType MachineType { get; set; }

    /// <summary>
    /// Gets or sets the WorkFlowType.
    /// </summary>
    public WorkFlowType WorkFlowType { get; set; }

    /// <summary>
    /// Gets or sets the Cycles.
    /// </summary>
    public List<CycleView> Cycles { get; set; }

    /// <summary>
    /// Gets or sets the Registers.
    /// </summary>
    public List<RegisterView> Registers { get; set; }

    /// <summary>
    /// Gets or sets the CreatedOn.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the ModifiedOn.
    /// </summary>
    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarCodeReportVm"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public BarCodeReportVm()
    {
        this.Label = string.Empty;
        this.CycleStatus = CycleStatus.None;
        this.FlowStatus = FlowStatus.None;
        this.PartStatus = PartStatus.None;
        this.MachineType = MachineType.None;
        this.WorkFlowType = WorkFlowType.None;
        this.Cycles = [];
        this.Registers = [];
    }

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<BarCodeReportVm> ToDto(BarCode src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCodeReportVm>.WithFailure("BarCode source cannot be null");
        }

        return IndQuestResults.Result<BarCodeReportVm>.Success(new BarCodeReportVm
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            FlowStatus = EnumModel.FromValue<FlowStatus>(src.FlowStatus),
            PartStatus = EnumModel.FromValue<PartStatus>(src.PartStatus),
            ProductId = src.ProductId,
            CreatedOn = src.CreatedOn,
            ModifiedOn = src.ModifiedOn,
        });
    }

    /// <summary>
    /// Executes ToDtoList operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDtoList.</returns>
    public static IndQuestResults.Result<List<BarCodeReportVm>> ToDtoList(IEnumerable<BarCode> src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<List<BarCodeReportVm>>.WithFailure("BarCode collection cannot be null");
        }

        var list = src.Select(s => new BarCodeReportVm
        {
            MachineId = s.MachineId,
            BarCodeId = s.BarCodeId,
            Label = s.Label ?? string.Empty,
            FlowStatus = EnumModel.FromValue<FlowStatus>(s.FlowStatus),
            PartStatus = EnumModel.FromValue<PartStatus>(s.PartStatus),
            ProductId = s.ProductId,
            CreatedOn = s.CreatedOn,
            ModifiedOn = s.ModifiedOn,
        }).ToList();
        return IndQuestResults.Result<List<BarCodeReportVm>>.Success(list);
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<BarCode> ToEntity(BarCodeReportVm src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCode>.WithFailure("BarCodeReportVm source cannot be null");
        }

        return IndQuestResults.Result<BarCode>.Success(new BarCode
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            FlowStatus = (int)src.FlowStatus,
            PartStatus = (int)src.PartStatus,
            ProductId = src.ProductId,
            CreatedOn = src.CreatedOn,
            ModifiedOn = src.ModifiedOn,
        });
    }
}
