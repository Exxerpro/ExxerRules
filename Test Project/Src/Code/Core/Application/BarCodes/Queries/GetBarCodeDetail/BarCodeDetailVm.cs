// <copyright file="BarCodeDetailVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeDetail;

/// <summary>
/// View model containing comprehensive barcode details including machine assignments, production data, and processing status.
/// </summary>
/// <remarks>
/// This view model aggregates all information related to a barcode's processing journey including:
/// - Machine routing information (current, last, next)
/// - Production cycle data and status information
/// - Manufacturing shift and timing data
/// - Quality validation results
/// - Associated registers and variables
/// It provides multiple mapping methods for different source types.
/// </remarks>
public class BarCodeDetailVm
{
    /// <summary>
    /// Gets or sets the identifier of the machine currently processing this barcode.
    /// </summary>
    /// <value>The current machine ID as an integer.</value>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the barcode being tracked.
    /// </summary>
    /// <value>The barcode ID as an integer.</value>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the current manufacturing cycle.
    /// </summary>
    /// <value>The cycle ID as an integer.</value>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the machine that previously processed this barcode.
    /// </summary>
    /// <value>The last machine ID as an integer.</value>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the machine that will next process this barcode.
    /// </summary>
    /// <value>The next machine ID as an integer.</value>
    public int NextMachineId { get; set; }

    /// <summary>
    /// Gets or sets any error message associated with barcode processing.
    /// </summary>
    /// <value>The error message as a string, or empty string if no error occurred.</value>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the production shift information when this barcode was processed.
    /// </summary>
    /// <value>A Shift object containing shift timing and scheduling data.</value>
    public Shift Shift { get; set; }

    /// <summary>
    /// Gets or sets the production metrics and performance data for this barcode.
    /// </summary>
    /// <value>A ProductionData value object containing performance metrics.</value>
    public ProductionData ProductionData { get; set; }

    /// <summary>
    /// Gets or sets the current status of the manufacturing cycle.
    /// </summary>
    /// <value>A CycleStatus enumeration indicating the cycle's progression state.</value>
    public CycleStatus CycleStatus { get; set; }

    /// <summary>
    /// Gets or sets the status of the barcode within the production workflow.
    /// </summary>
    /// <value>A FlowStatus enumeration indicating the workflow position.</value>
    public FlowStatus FlowStatus { get; set; }

    /// <summary>
    /// Gets or sets the quality status of the part associated with this barcode.
    /// </summary>
    /// <value>A PartStatus enumeration indicating quality validation results.</value>
    public PartStatus PartStatus { get; set; }

    /// <summary>
    /// Gets or sets the type of machine currently processing this barcode.
    /// </summary>
    /// <value>A MachineType enumeration indicating the functional category of the machine.</value>
    public MachineType MachineType { get; set; }

    /// <summary>
    /// Gets or sets the type of workflow this barcode is following.
    /// </summary>
    /// <value>A WorkFlowType enumeration indicating the processing workflow pattern.</value>
    public WorkFlowType WorkFlowType { get; set; }

    /// <summary>
    /// Gets or sets the validation result for this barcode's processing.
    /// </summary>
    /// <value>A ResultValidation enumeration indicating validation outcome.</value>
    public ResultValidation ResultValidation { get; set; }

    /// <summary>
    /// Gets or sets the human-readable label of the barcode.
    /// </summary>
    /// <value>The barcode label as a string.</value>
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the complete barcode entity with all its properties.
    /// </summary>
    /// <value>A BarCode entity containing full barcode information.</value>
    public BarCode BarCode { get; set; }

    /// <summary>
    /// Gets or sets the collection of manufacturing cycles associated with this barcode.
    /// </summary>
    /// <value>A list of Cycle entities representing the processing history.</value>
    public List<Cycle> Cycles { get; set; }

    /// <summary>
    /// Gets or sets the collection of register data captured during barcode processing.
    /// </summary>
    /// <value>A list of Register entities containing measurement and status data.</value>
    public List<Register> Registers { get; set; }

    /// <summary>
    /// Gets or sets the collection of register view models for UI display.
    /// </summary>
    /// <value>A list of RegisterVm objects formatted for user interface presentation.</value>
    public List<RegisterVm> RegistersVm { get; set; }

    /// <summary>
    /// Gets or sets the status monitoring information for this barcode.
    /// </summary>
    /// <value>A StatusMonitor object containing real-time status tracking data.</value>
    public StatusMonitor StatusMonitor { get; set; }

    /// <summary>
    /// Gets or sets the collection of variables associated with barcode processing.
    /// </summary>
    /// <value>A list of Variable entities containing process parameters and measurements.</value>
    public List<Variable> Variables { get; set; }

    /// <summary>
    /// Converts a BarCode entity to a BarCodeDetailVm.
    /// </summary>
    /// <param name="src">The source BarCode entity to convert.</param>
    /// <returns>A BarCodeDetailVm containing the converted barcode data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when src is null.</exception>
    /// <remarks>
    /// This overload maps basic barcode properties and converts enumeration values using EnumModel.FromValue.
    /// Additional properties may require separate population.
    /// </remarks>
    public static IndQuestResults.Result<BarCodeDetailVm> ToDto(BarCode src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCodeDetailVm>.WithFailure("BarCode source cannot be null");
        }

        return IndQuestResults.Result<BarCodeDetailVm>.Success(new BarCodeDetailVm
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            FlowStatus = EnumModel.FromValue<FlowStatus>(src.FlowStatus),
            PartStatus = EnumModel.FromValue<PartStatus>(src.PartStatus),
            BarCode = src,

            // Map other properties as needed
        });
    }

    /// <summary>
    /// Converts an IBarCodeResult to a BarCodeDetailVm.
    /// </summary>
    /// <param name="src">The source IBarCodeResult to convert.</param>
    /// <returns>A BarCodeDetailVm containing the comprehensive barcode processing data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when src is null.</exception>
    /// <remarks>
    /// This overload provides more comprehensive mapping including machine routing,
    /// status information, cycle data, and error handling. It initializes collections safely.
    /// </remarks>
    public static IndQuestResults.Result<BarCodeDetailVm> ToDto(IBarCodeResult src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCodeDetailVm>.WithFailure("BarCodeResult source cannot be null");
        }

        return IndQuestResults.Result<BarCodeDetailVm>.Success(new BarCodeDetailVm
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            CycleId = src.CycleId,
            LastMachineId = src.LastMachineId,
            NextMachineId = src.NextMachineId,
            Error = src.Error ?? string.Empty,
            ResultValidation = src.ResultValidation,
            FlowStatus = src.FlowStatus,
            PartStatus = src.PartStatus,
            CycleStatus = src.CycleStatus,
            MachineType = src.MachineType,
            WorkFlowType = src.WorkFlowType,
            Label = src.Label ?? string.Empty,
            BarCode = src.BarCode,
            Cycles = src.Cycles?.ToList() ?? [],
            RegistersVm = [],

            // Add more mappings as needed
        });
    }

    /// <summary>
    /// Converts a BarCodeDetailVm back to a BarCode entity.
    /// </summary>
    /// <param name="src">The source BarCodeDetailVm to convert.</param>
    /// <returns>A BarCode entity containing the core barcode data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when src is null.</exception>
    /// <remarks>
    /// This method extracts the essential barcode properties for persistence.
    /// Complex view model data is not included in the entity conversion.
    /// </remarks>
    public static IndQuestResults.Result<BarCode> ToEntity(BarCodeDetailVm src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCode>.WithFailure("BarCodeDetailVm source cannot be null");
        }

        return IndQuestResults.Result<BarCode>.Success(new BarCode
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            FlowStatus = src.FlowStatus.Value,
            PartStatus = src.PartStatus.Value,

            // Map other properties as needed
        });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarCodeDetailVm"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public BarCodeDetailVm()
    {
        this.CycleStatus = CycleStatus.None;
        this.FlowStatus = FlowStatus.None;
        this.PartStatus = PartStatus.None;
        this.MachineType = MachineType.None;
        this.WorkFlowType = WorkFlowType.None;
        this.ResultValidation = ResultValidation.None;
        this.Shift = new Shift(new DateTimeMachine());
        this.ProductionData = new ProductionData();
        this.Label = string.Empty;
        this.BarCode = new BarCode();
        this.Cycles = new List<Cycle>();
        this.Registers = new List<Register>();
        this.RegistersVm = new List<RegisterVm>();
        this.StatusMonitor = new StatusMonitor();
        this.Variables = new List<Variable>();
    }
}
