using IndTrace.Application.Repositories;

namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Real IBarCodeResult implementation backed by the database via IRepository.
/// Provides minimal data required by handlers while using the real persistence layer.
/// </summary>
public class BarCodeResultService
{
    private readonly IRepository<BarCode> _barCodes;

    public BarCodeResultService(IRepository<BarCode> barCodes)
    {
        _barCodes = barCodes;
        // defaults
        ResultValidation = ResultValidation.Invalid;
        FlowStatus = FlowStatus.InProcess;
        PartStatus = PartStatus.Ok;
        CycleStatus = CycleStatus.Started;
        MachineType = MachineType.Process;
        WorkFlowType = WorkFlowType.None;
        Description = string.Empty;
        References = new Dictionary<string, Register>();
        Recipe = new Recipe();
        Cycle = new Cycle();
        BarCode = new BarCode();
        MasterLabel = new MasterLabel();
    }

    public int MachineId { get; private set; }
    public int BarCodeId { get; private set; }
    public int CycleId { get; private set; }
    public int CyclesOk { get; private set; }
    public int ShiftId { get; private set; }
    public int CommandId { get; private set; }
    public ResultValidation ResultValidation { get; set; }
    public string? Error { get; private set; }
    public string? Label { get; private set; }
    public string? PartNumber { get; private set; }
    public string Description { get; private set; }
    public int LastMachineId { get; private set; }
    public int NextMachineId { get; private set; }
    public int RegistersSaved { get; private set; }
    public CycleStatus CycleStatus { get; private set; }
    public FlowStatus FlowStatus { get; private set; }
    public PartStatus PartStatus { get; private set; }
    public MachineType MachineType { get; private set; }
    public WorkFlowType WorkFlowType { get; private set; }
    public Recipe Recipe { get; private set; }
    public Cycle Cycle { get; private set; }
    public IEnumerable<Cycle> Cycles { get; private set; } = Array.Empty<Cycle>();
    public BarCode BarCode { get; private set; }
    public MasterLabel MasterLabel { get; private set; }
    public IDictionary<string, Register> References { get; private set; }

    public async Task<BarCodeResultService> GetBarCodeDetails(BarCodeDetailsRequest barCodeDetailsRequest, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(barCodeDetailsRequest);

        MachineId = barCodeDetailsRequest.MachineId;
        Label = barCodeDetailsRequest.Label;
        PartNumber = barCodeDetailsRequest.PartNumber;

        // Query the real database for the barcode id
        var result = await _barCodes.GetBarCodeByLabelAsync(Label!, cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            Error = result.IsFailure ? string.Join("; ", result.Errors) : "Barcode not found";
            BarCodeId = 0;
            return this;
        }

        var entity = result.Value;
        BarCodeId = entity.BarCodeId;
        BarCode = entity;

        // Optionally fill other fields as needed later
        return this;
    }

    public void UpdateBarCodeInformationOnCycle(FlowStatus flowStatus, PartStatus partStatus, CycleStatus cycleStatus)
    {
        FlowStatus = flowStatus;
        PartStatus = partStatus;
        CycleStatus = cycleStatus;
    }

    public void SetCycle(Cycle cycle)
    {
        Cycle = cycle;
        CycleId = cycle?.CycleId ?? 0;
    }

    public void SetBarCode(BarCode barCode)
    {
        BarCode = barCode;
        BarCodeId = barCode?.BarCodeId ?? 0;
        Label = barCode?.Label ?? Label;
    }

    public void SetCyclesOk(int cyclesOk) => CyclesOk = cyclesOk;

    public void SetRegistersSaved(int registers) => RegistersSaved = registers;
}
