// <copyright file="ReportDetailMonitorVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;

/// <summary>
/// Represents the ReportDetailMonitorVm.
/// </summary>
public class ReportDetailMonitorVm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReportDetailMonitorVm"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public ReportDetailMonitorVm()
    {
        this.CycleStatus = CycleStatus.None;
        this.FlowStatus = FlowStatus.None;
        this.PartStatus = PartStatus.None;
        this.MachineType = MachineType.None;
        this.WorkFlowType = WorkFlowType.None;
        this.ResultValidation = ResultValidation.None;
        this.Label = string.Empty;
        this.Cycles = [];
        this.Registers = [];
        this.StatusMonitor = new StatusMonitor();
        this.Variables = [];
    }

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
    /// Gets or sets the ResultValidation.
    /// </summary>
    public ResultValidation ResultValidation { get; set; }

    /// <summary>
    /// Gets or sets the Label.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the Cycles.
    /// </summary>
    public List<CycleView> Cycles { get; set; }

    /// <summary>
    /// Gets or sets the Registers.
    /// </summary>
    public List<RegisterView> Registers { get; set; }

    /// <summary>
    /// Gets or sets the StatusMonitor.
    /// </summary>
    public StatusMonitor StatusMonitor { get; set; }

    /// <summary>
    /// Gets or sets the Variables.
    /// </summary>
    public List<Variable> Variables { get; set; }

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<ReportDetailMonitorVm> ToDto(BarCode src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<ReportDetailMonitorVm>.WithFailure("BarCode source cannot be null");
        }

        return IndQuestResults.Result<ReportDetailMonitorVm>.Success(new ReportDetailMonitorVm
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            FlowStatus = EnumModel.FromValue<FlowStatus>(src.FlowStatus),
            PartStatus = EnumModel.FromValue<PartStatus>(src.PartStatus),
            Registers = [],
            Cycles = [],
            StatusMonitor = new StatusMonitor(),
            Variables = [],

            // Only properties available in BarCode are mapped
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<BarCode> ToEntity(ReportDetailMonitorVm src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCode>.WithFailure("ReportDetailMonitorVm source cannot be null");
        }

        return IndQuestResults.Result<BarCode>.Success(new BarCode
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            FlowStatus = (int)src.FlowStatus,
            PartStatus = (int)src.PartStatus,

            // Only properties available in BarCode are mapped
        });
    }
}
