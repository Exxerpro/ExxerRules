// <copyright file="BarCodeResult.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Data transfer object for barcode result data, including machine, barcode, cycle, and validation information.
/// </summary>
public class BarCodeResultData
{
    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the barcode identifier.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the cycle identifier.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the number of cycles marked as OK.
    /// </summary>
    public int CyclesOk { get; set; }

    /// <summary>
    /// Gets or sets the shift identifier.
    /// </summary>
    public int ShiftId { get; set; }

    /// <summary>
    /// Gets or sets the command identifier.
    /// </summary>
    public int CommandId { get; set; }

    /// <summary>
    /// Gets or sets the result validation status.
    /// </summary>
    public ResultValidation ResultValidation { get; set; } = ResultValidation.None;

    /// <summary>
    /// Gets or sets the error message, if any.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the label associated with the barcode.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the part number.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the last machine.
    /// </summary>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the next machine.
    /// </summary>
    public int NextMachineId { get; set; }

    /// <summary>
    /// Gets or sets the number of registers saved.
    /// </summary>
    public int RegistersSaved { get; set; }

    /// <summary>
    /// Gets or sets the current shift information.
    /// </summary>
    public Shift Shift { get; set; } = new(new DateTimeMachine());

    /// <summary>
    /// Gets or sets the last shift information.
    /// </summary>
    public Shift LastShift { get; set; } = new(new DateTimeMachine());

    /// <summary>
    /// Gets or sets the product information.
    /// </summary>
    public Product Product { get; set; } = new();

    /// <summary>
    /// Gets or sets the command information.
    /// </summary>
    public TaskGatewayRequest Command { get; set; } = new();

    /// <summary>
    /// Gets or sets the cycle status.
    /// </summary>
    public CycleStatus CycleStatus { get; set; } = CycleStatus.None;

    /// <summary>
    /// Gets or sets the flow status.
    /// </summary>
    public FlowStatus FlowStatus { get; set; } = FlowStatus.None;

    /// <summary>
    /// Gets or sets the part status.
    /// </summary>
    public PartStatus PartStatus { get; set; } = PartStatus.None;

    /// <summary>
    /// Gets or sets the machine type.
    /// </summary>
    public MachineType MachineType { get; set; } = MachineType.None;

    /// <summary>
    /// Gets or sets the workflow type.
    /// </summary>
    public WorkFlowType WorkFlowType { get; set; } = WorkFlowType.None;

    /// <summary>
    /// Gets or sets the recipe information.
    /// </summary>
    public Recipe Recipe { get; set; } = new();

    /// <summary>
    /// Gets or sets the machine information.
    /// </summary>
    public Machine Machine { get; set; } = new();

    /// <summary>
    /// Gets or sets the cycle information.
    /// </summary>
    public Cycle Cycle { get; set; } = new();

    /// <summary>
    /// Gets or sets the collection of cycles.
    /// </summary>
    public IEnumerable<Cycle> Cycles { get; set; } = new List<Cycle>();

    /// <summary>
    /// Gets or sets the barcode information.
    /// </summary>
    public BarCode BarCode { get; set; } = new();

    /// <summary>
    /// Gets or sets the master label information.
    /// </summary>
    public MasterLabel MasterLabel { get; set; } = new();

    /// <summary>
    /// Gets or sets the references dictionary.
    /// </summary>
    public IDictionary<string, Register> References { get; set; } = new Dictionary<string, Register>();
}

/// <summary>
/// Provides barcode result operations, including fetching, validation, and mapping between entities and DTOs.
/// </summary>
public partial class BarCodeResult(
    ILogger<BarCodeResult> logger,
    IRepository<BarCode> barCodeRepository,
    IRepository<Cycle> cycleRepository,
    IReadOnlyRepository<Machine> machineRepository,
    IReadOnlyRepository<Recipe> recipeRepository,
    IReadOnlyRepository<MasterLabel> masterLabelRepository,
    IRepository<Shift> shiftRepository,
    IReadOnlyRepository<WorkFlow> workFlowRepository,
    IReadOnlyRepository<Variable> variablesRepository,
    IReadOnlyRepository<Product> productRepository,
    IDateTimeMachine dateTimeMachine,
    IBarCodeValidationService validationService) : IBarCodeResult, IResettable
{
    /// <summary>
    /// Test/builder factory to create a pre-populated BarCodeResult without external dependencies.
    /// </summary>
    public static BarCodeResult Create(
        int lastMachineId,
        int nextMachineId,
        CycleStatus cycleStatus,
        FlowStatus flowStatus,
        PartStatus partStatus,
        MachineType machineType,
        WorkFlowType workFlowType,
        int barCodeId,
        int cycleId,
        string label,
        string partNumber,
        int cyclesOk,
        int shiftId,
        ResultValidation resultValidation)
    {
        // You must provide valid arguments for all dependencies here.
        // For demonstration, using null/defaults. Replace with actual dependencies in your codebase.
        var instance = new BarCodeResult(
            logger: null!,
            barCodeRepository: null!,
            cycleRepository: null!,
            machineRepository: null!,
            recipeRepository: null!,
            masterLabelRepository: null!,
            shiftRepository: null!,
            workFlowRepository: null!,
            variablesRepository: null!,
            productRepository: null!,
            dateTimeMachine: null!,
            validationService: null!
        );

        instance.LastMachineId = lastMachineId;
        instance.NextMachineId = nextMachineId;
        instance.CycleStatus = cycleStatus;
        instance.FlowStatus = flowStatus;
        instance.PartStatus = partStatus;
        instance.MachineType = machineType;
        instance.WorkFlowType = workFlowType;
        instance.BarCodeId = barCodeId;
        instance.CycleId = cycleId;
        instance.Label = label;
        instance.PartNumber = partNumber;
        instance.CyclesOk = cyclesOk;
        instance.ShiftId = shiftId;
        instance.ResultValidation = resultValidation;
        return instance;
    }

    // Dependencies injected via constructor
    // ...constructor and injected fields...

    // State properties

    /// <summary>
    /// Gets the machine identifier.
    /// </summary>
    public int MachineId { get; private set; }

    /// <summary>
    /// Gets the barcode identifier.
    /// </summary>
    public int BarCodeId { get; private set; }

    /// <summary>
    /// Gets the cycle identifier.
    /// </summary>
    public int CycleId { get; private set; }

    /// <summary>
    /// Gets the number of cycles marked as OK.
    /// </summary>
    public int CyclesOk { get; private set; }

    /// <summary>
    /// Gets the shift identifier.
    /// </summary>
    public int ShiftId { get; private set; }

    /// <summary>
    /// Gets the command identifier.
    /// </summary>
    public int CommandId { get; private set; }

    /// <summary>
    /// Gets or sets the result validation status.
    /// </summary>
    public ResultValidation ResultValidation { get; set; } = new();

    /// <summary>
    /// Gets or sets the error message, if any.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets the label associated with the barcode.
    /// </summary>
    public string Label { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the part number.
    /// </summary>
    public string PartNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets the identifier of the last machine.
    /// </summary>
    public int LastMachineId { get; private set; }

    /// <summary>
    /// Gets the identifier of the next machine.
    /// </summary>
    public int NextMachineId { get; private set; }

    /// <summary>
    /// Gets the number of registers saved.
    /// </summary>
    public int RegistersSaved { get; private set; }

    /// <summary>
    /// Gets the current shift information.
    /// </summary>
    public Shift Shift { get; private set; } = new(new DateTimeMachine());

    /// <summary>
    /// Gets the last shift information.
    /// </summary>
    public Shift LastShift { get; private set; } = new(new DateTimeMachine());

    /// <summary>
    /// Gets the product information.
    /// </summary>
    public Product Product { get; private set; } = new();

    /// <summary>
    /// Gets the command information.
    /// </summary>
    public TaskGatewayRequest Command { get; private set; } = new();

    /// <summary>
    /// Gets the cycle status.
    /// </summary>
    public CycleStatus CycleStatus { get; private set; } = new();

    /// <summary>
    /// Gets the flow status.
    /// </summary>
    public FlowStatus FlowStatus { get; private set; } = new();

    /// <summary>
    /// Gets the part status.
    /// </summary>
    public PartStatus PartStatus { get; private set; } = new();

    /// <summary>
    /// Gets the machine type.
    /// </summary>
    public MachineType MachineType { get; private set; } = new();

    /// <summary>
    /// Gets the workflow type.
    /// </summary>
    public WorkFlowType WorkFlowType { get; private set; } = new();

    /// <summary>
    /// Gets the recipe information.
    /// </summary>
    public Recipe Recipe { get; private set; } = new();

    /// <summary>
    /// Gets the machine information.
    /// </summary>
    public Machine Machine { get; private set; } = new();

    /// <summary>
    /// Gets the cycle information.
    /// </summary>
    public Cycle Cycle { get; private set; } = new();

    /// <summary>
    /// Gets the collection of cycles.
    /// </summary>
    public IEnumerable<Cycle> Cycles { get; private set; } = new List<Cycle>();

    /// <summary>
    /// Gets the barcode information.
    /// </summary>
    public BarCode BarCode { get; private set; } = new();

    /// <summary>
    /// Gets the master label information.
    /// </summary>
    public MasterLabel MasterLabel { get; private set; } = new();

    /// <summary>
    /// Gets the references dictionary.
    /// </summary>
    public IDictionary<string, Register> References { get; private set; } = new Dictionary<string, Register>();

    private void InitResults(BarCodeDetailsRequest barCodeDetailsRequest)
    {
        // Reset the error message
        // and all the properties to their default values
        logger.LogInformation("Reset the error message");
        logger.LogInformation("and all the properties to their default values");
        this.Error = string.Empty;

        this.Label = barCodeDetailsRequest.Label;
        this.MachineId = barCodeDetailsRequest.MachineId;
        this.PartNumber = barCodeDetailsRequest.PartNumber;

        this.NextMachineId = 0;
        this.LastMachineId = 0;
        this.BarCodeId = 0;
        this.CycleId = 0;
        this.CyclesOk = 0;
        this.ShiftId = 0;

        this.MasterLabel = new MasterLabel();
        this.ResultValidation = ResultValidation.None;
        this.PartStatus = PartStatus.None;
        this.CycleStatus = CycleStatus.None;
        this.FlowStatus = FlowStatus.None;
        this.WorkFlowType = WorkFlowType.None;
        this.MachineType = MachineType.None;

        logger.LogInformation("Finish Starting Validation");
    }

    /// <inheritdoc/>
    public bool TryReset()
    {
        // Reset all mutable state to default
        this.MachineId = 0;
        this.BarCodeId = 0;
        this.CycleId = 0;
        this.CyclesOk = 0;
        this.ShiftId = 0;
        this.CommandId = 0;
        this.ResultValidation = ResultValidation.None;
        this.Error = string.Empty;
        this.Label = string.Empty;
        this.PartNumber = string.Empty;
        this.Description = string.Empty;
        this.LastMachineId = 0;
        this.NextMachineId = 0;
        this.RegistersSaved = 0;

        this.Shift = new Shift(new DateTimeMachine());
        this.LastShift = new Shift(new DateTimeMachine());
        this.Product = new Product();
        this.Command = new TaskGatewayRequest();
        this.CycleStatus = CycleStatus.None;
        this.FlowStatus = FlowStatus.None;
        this.PartStatus = PartStatus.None;
        this.MachineType = MachineType.None;
        this.WorkFlowType = WorkFlowType.None;
        this.Recipe = new Recipe();
        this.Machine = new Machine();
        this.Cycle = new Cycle();
        this.Cycles = new List<Cycle>();
        this.BarCode = new BarCode();
        this.MasterLabel = new MasterLabel();
        this.References = new Dictionary<string, Register>();

        return true;
    }

    private async Task<Result<Machine?>> FetchMachineByIdAsync(int machineId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Repository Coordination: Fetching Machine by ID: {MachineId}", machineId);

        var result = await machineRepository.FirstOrDefaultAsync(new Specification<Machine>(f => f.MachineId == machineId), cancellationToken);

        logger.LogInformation("Repository Coordination: Fetch Machine Result: {Result}", result.IsSuccess ? "Success" : "Failure");
        logger.LogInformation("Repository Coordination: Fetched Machine: {Machine}", result.Value);
        return result;
    }

    private async Task<Result<IDictionary<string, Register>?>> FetchReferencesByMachineIdAsync(int machineId, int cycleId, CancellationToken cancellationToken)
    {
        var spec = new Specification<Variable>(r => r.MachineId == machineId && r.IsActive == 1 &&
                                                    r.VariableGroupId == TagsGroups.ReferenceTags.Value);
        var variablesResult = await variablesRepository.ListAsync(spec, cancellationToken).ConfigureAwait(false);

        if (variablesResult.IsFailure)
        {
            logger.LogError(variablesResult.Error);
            return Result<IDictionary<string, Register>?>.WithFailure("References not found");
        }

        if (variablesResult.Value is null)
        {
            return Result<IDictionary<string, Register>?>.WithFailure("Variables collection is null");
        }

        var references = variablesResult.Value
            .GroupBy(v => v.Name)
            .Where(g => g.FirstOrDefault() != null)
            .ToDictionary(
                g => g.Key,  // Group key (v.Name)
                g => new Register
                {
                    RegisterId = g.First().VariableId,
                    Name = g.First().Name,
                    VariableId = g.First().VariableId,
                    CycleId = cycleId,
                    Value = g.First().Value,
                    DataType = g.First().NativeType,
                    StatusValueId = 1, // Provide a default value or map from Variable
                });

        return Result<IDictionary<string, Register>?>.Success(references);
    }

    private Task<Result<BarCode?>> FetchBarCodeByLabelAsync(string label, CancellationToken cancellationToken)
    {
        return barCodeRepository.FirstOrDefaultAsync(new Specification<BarCode>(b => b.Label == label), cancellationToken);
    }

    private async Task<Result<IEnumerable<Cycle>>> FetchCyclesByBarCodeIdAsync(int barCodeId, CancellationToken cancellationToken)
    {
        var cycles = await cycleRepository.ListAsync(new Specification<Cycle>(c => c.BarCodeId == barCodeId), cancellationToken).ConfigureAwait(false);
        return cycles;
    }

    private async Task<Result<Dictionary<int, WorkFlow>>> FetchWorkflowsByProductIdAsync(int productId, CancellationToken cancellationToken)
    {
        var specWorkFlow = new Specification<WorkFlow>(f => f.ProductId == productId).AddOrderBy(f => f.NextMachineId);
        var workflows = await workFlowRepository.ListAsync(specWorkFlow, cancellationToken).ConfigureAwait(false);

        if (workflows.IsSuccess && workflows.Value is not null)
        {
            return workflows.Value.ToDictionary(f => f.LastMachineId);
        }

        return Result<Dictionary<int, WorkFlow>>.WithFailure("workflows not found");
    }

    private async Task<Result<Recipe?>> FetchRecipeAsync(int productId, int machineId, CancellationToken cancellationToken)
    {
        var specRecipe = new Specification<Recipe>(r => r.ProductId == productId && r.MachineId == machineId);
        return await recipeRepository.FirstOrDefaultAsync(specRecipe, cancellationToken).ConfigureAwait(false);
    }

    private Task<Result<Product?>> FetchProductAsync(int productId, int machineId, CancellationToken cancellationToken)
    {
        var specProduct = new Specification<Product>(r => r.ProductId == productId);
        return productRepository.FirstOrDefaultAsync(specProduct, cancellationToken);
    }

    private async Task<Result<Shift?>> FetchCurrentShiftAsync(CancellationToken cancellationToken)
    {
        // Get the current time
        var now = dateTimeMachine.Now.ToLocalTime();

        // Retrieve the top 3 shifts that started before or at the current time
        // Create a specification to filter shifts and apply ordering and paging
        var shiftSpec = new Specification<Shift>(s =>
                s.StartBy <= now)
            .AddOrderByDescending(s => s.StartBy)
            .ApplyPaging(0, 3);

        var shifts = await shiftRepository.ListAsync(shiftSpec, cancellationToken).ConfigureAwait(false);

        if (shifts.IsFailure)
        {
            return Result<Shift?>.WithFailure("Shifts not found");
        }

        if (shifts.Value is null)
        {
            return Result<Shift?>.WithFailure("Shifts collection is null");
        }

        // Perform the time addition check on the client side
        var currentShift = shifts.Value
            .FirstOrDefault(s => now >= s.StartBy && now <= s.StartBy.Add(s.Duration));

        // Return null if no shift is found that includes the current time within its duration
        return currentShift ?? null;
    }

    private async Task<Result<MasterLabel?>> FetchMasterLabelAsync(string label, CancellationToken cancellationToken)
    {
        return await masterLabelRepository.FirstOrDefaultAsync(new Specification<MasterLabel>(ml => ml.MasterLabelCode == label), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously retrieves barcode details for the specified request.
    /// </summary>
    /// <param name="barCodeDetailsRequest">The barcode details request.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous operation, with the barcode result.</returns>
    public async Task<IBarCodeResult> GetBarCodeDetails(BarCodeDetailsRequest barCodeDetailsRequest, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("GetBarCodeDetails:: cancellation requested. Exiting early.");
            return this.SetFailureAndReturn("Operation cancelled", ResultValidation.OperationCancelled);
        }

        try
        {
            this.InitResults(barCodeDetailsRequest);

            var machineResult = await this.FetchMachineByIdAsync(this.MachineId, cancellationToken).ConfigureAwait(false);
            if (machineResult.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: Machine not found. MachineId={MachineId}, Validation={Validation}", this.MachineId, ResultValidation.MachineNotFound);
                return this.SetFailureAndReturn("Machine not found", ResultValidation.MachineNotFound);
            }

            if (machineResult.Value is null)
            {
                logger.LogWarning("GetBarCodeDetails:: Machine not found. MachineId={MachineId}, Validation={Validation}", this.MachineId, ResultValidation.MachineNotFound);
                return this.SetFailureAndReturn("Machine not found", ResultValidation.MachineNotFound);
            }
            this.Machine = machineResult.Value;
            this.Description = this.Machine.Name;
            this.MachineType = this.Machine.MachineType;
            this.Name = this.Machine.Name;

            logger.LogInformation("GetBarCodeDetails:: Machine found  Name: {Name} Type: {MachineType} :: {Machine}", this.Machine.Name, this.Machine.MachineType, this.Machine);

            var referencesResult = await this.FetchReferencesByMachineIdAsync(this.MachineId, this.CycleId, cancellationToken).ConfigureAwait(false);
            if (referencesResult.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: References not found. MachineId={MachineId}, CycleId={CycleId}, Validation={Validation}", this.MachineId, this.CycleId, ResultValidation.ReferencesNotFound);
                return this.SetFailureAndReturn("References not found", ResultValidation.ReferencesNotFound);
            }

            if (referencesResult.Value is null)
            {
                logger.LogWarning("GetBarCodeDetails:: References is null. MachineId={MachineId}, CycleId={CycleId}, Validation={Validation}", this.MachineId, this.CycleId, ResultValidation.ReferencesNotFound);
                return this.SetFailureAndReturn("References is null", ResultValidation.ReferencesNotFound);
            }

            this.References = referencesResult.Value;

            if (string.IsNullOrEmpty(this.Label))
            {
                logger.LogWarning("GetBarCodeDetails:: Label is null or empty. MachineId={MachineId}, Validation={Validation}", this.MachineId, ResultValidation.BarCodeNotFound);
                return this.SetFailureAndReturn("Label cannot be null or empty", ResultValidation.BarCodeNotFound);
            }

            logger.LogInformation("GetBarCodeDetails::Searching BarCode for Label={Label}", this.Label);
            var barCodeResult = await this.FetchBarCodeByLabelAsync(this.Label!, cancellationToken).ConfigureAwait(false);
            if (barCodeResult.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: BarCode label not found. Label={Label}, Validation={Validation}", this.Label, ResultValidation.BarCodeNotFound);
                return this.SetFailureAndReturn("BarCode label not found", ResultValidation.BarCodeNotFound);
            }

            if (barCodeResult.Value is null)
            {
                logger.LogWarning("GetBarCodeDetails:: BarCode label not found (null value). Label={Label}, Validation={Validation}", this.Label, ResultValidation.BarCodeNotFound);
                return this.SetFailureAndReturn("BarCode label not found", ResultValidation.BarCodeNotFound);
            }

            logger.LogInformation("BarCode found {Name} , label {Label}", this.Name, this.BarCode.Label);
            this.BarCode = barCodeResult.Value;

            if (string.IsNullOrEmpty(this.PartNumber))
            {
                logger.LogWarning("GetBarCodeDetails:: PartNumber is null or empty. Label={Label}, Validation={Validation}", this.Label, ResultValidation.PartNumberNotValid);
                return this.SetFailureAndReturn("PartNumber cannot be null or empty", ResultValidation.PartNumberNotValid);
            }

            if (!this.ValidatePartNumber(this.Label!, this.PartNumber!))
            {
                logger.LogWarning("GetBarCodeDetails:: Invalid part number. Label={Label}, PartNumber={PartNumber}, Validation={Validation}", this.Label, this.PartNumber, ResultValidation.PartNumberNotValid);
                return this.SetFailureAndReturn("Invalid part number", ResultValidation.PartNumberNotValid);
            }

            var productResult = await this.FetchProductAsync(this.BarCode.ProductId, this.MachineId, cancellationToken).ConfigureAwait(false);
            if (productResult.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: Product not found. PartNumber={PartNumber}, MachineId={MachineId}, Validation={Validation}", this.PartNumber, this.MachineId, ResultValidation.PartNumberNotValid);
                return this.SetFailureAndReturn($"Product not found for part number {this.PartNumber} and machine {this.MachineId}", ResultValidation.PartNumberNotValid);
            }

            if (productResult.Value is not null)
            {
                this.Product = productResult.Value;
            }

            logger.LogInformation("GetBarCodeDetails::Searching Cycles for BarCodeId={BarCodeId}", this.BarCode.BarCodeId);
            var resultCycles = await this.FetchCyclesByBarCodeIdAsync(this.BarCode.BarCodeId, cancellationToken).ConfigureAwait(false);
            if (resultCycles.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: Cycles not found. BarCodeId={BarCodeId}, Validation={Validation}", this.BarCode.BarCodeId, ResultValidation.CycleNotFound);
                return this.SetFailureAndReturn("Cycles not found", ResultValidation.CycleNotFound);
            }

            if (resultCycles.Value is not null)
            {
                this.Cycles = resultCycles.Value;
            }

            var resultCycle = this.FetchLastCycleCycleAsync(resultCycles);

            if (resultCycle.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: Cycle not found. BarCodeId={BarCodeId}, Validation={Validation}", this.BarCode.BarCodeId, ResultValidation.CycleNotFound);
                return this.SetFailureAndReturn("Cycle not found", ResultValidation.CycleNotFound);
            }

            if (resultCycle.Value is null)
            {
                logger.LogWarning("GetBarCodeDetails:: Cycle is null. BarCodeId={BarCodeId}, Validation={Validation}", this.BarCode.BarCodeId, ResultValidation.CycleNotFound);
                return this.SetFailureAndReturn("Cycle cannot be null", ResultValidation.CycleNotFound);
            }

            this.Cycle = resultCycle.Value;
            if (this.Cycle is null)
            {
                logger.LogWarning("GetBarCodeDetails:: Cycle is null (redundant check). BarCodeId={BarCodeId}, Validation={Validation}", this.BarCode.BarCodeId, ResultValidation.CycleNotFound);
                return this.SetFailureAndReturn("Cycle cannot be null", ResultValidation.CycleNotFound);
            }

            this.CyclesOk = this.Cycle.CyclesOk;

            logger.LogInformation("GetBarCodeDetails::Searching workflows for ProductId={ProductId}", this.BarCode.ProductId);

            var workflowsResult = await this.FetchWorkflowsByProductIdAsync(this.BarCode.ProductId, cancellationToken).ConfigureAwait(false);

            if (workflowsResult.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: WorkFlow not found. ProductId={ProductId}, Validation={Validation}", this.BarCode.ProductId, ResultValidation.WorkFlowNotFound);
                return this.SetFailureAndReturn("WorkFlow not found", ResultValidation.WorkFlowNotFound);
            }

            if (workflowsResult.Value is null)
            {
                logger.LogWarning("GetBarCodeDetails:: WorkFlow is null. ProductId={ProductId}, Validation={Validation}", this.BarCode.ProductId, ResultValidation.WorkFlowNotFound);
                return this.SetFailureAndReturn("WorkFlow is null", ResultValidation.WorkFlowNotFound);
            }

            var workflows = new Dictionary<int, WorkFlow>();

            if (workflowsResult is not null)
            {
                workflows = workflowsResult.Value;
            }
            this.LastMachineId = this.DetermineLastMachineId(this.Cycle, this.BarCode);

            if (this.IsBarCodeRestored())
            {
                logger.LogInformation("GetBarCodeDetails:: BarCode restored. Label={Label}, Validation={Validation}", this.Label, ResultValidation.Valid);
                this.HandleRestoredBarCode();
                return this;
            }

            // Check if the Piece has already been processed in the station,
            // has to have FlowStatus == FlowStatus.Restored
            if (this.HasMaxAllowedCyclesOnStation(barCodeDetailsRequest))
            {
                logger.LogWarning("GetBarCodeDetails:: Max allowed cycles (OK) reached. MachineId={MachineId}, Validation={Validation}", this.MachineId, ResultValidation.WorkFlowNotValid);
                this.ResultValidation = ResultValidation.WorkFlowNotValid;
                return this;
            }

            // Check if the Piece has already been processed in the station
            // has to have FlowStatus == FlowStatus.Restored
            if (this.HasMaxAllowedCyclesNotOkOnStation(barCodeDetailsRequest))
            {
                logger.LogWarning("GetBarCodeDetails:: Max allowed cycles (NOK) reached. MachineId={MachineId}, Validation={Validation}", this.MachineId, ResultValidation.WorkFlowNotValid);
                this.ResultValidation = ResultValidation.WorkFlowNotValid;
                return this;
            }

            if (!workflows.ContainsKey(this.LastMachineId))
            {
                logger.LogWarning("GetBarCodeDetails:: WorkFlow key missing. LastMachineId={LastMachineId}, Validation={Validation}", this.LastMachineId, ResultValidation.WorkFlowNotFound);
                return this.SetFailureAndReturn("WorkFlow key missing", ResultValidation.WorkFlowNotFound);
            }

            this.NextMachineId = this.DetermineNextMachineId(workflows, this.LastMachineId, this.Cycle);

            if (this.Machine.MachineType == MachineType.Process)
            {
                await this.HandleProcessMachineTypeAsync(cancellationToken, workflows).ConfigureAwait(false);
            }

            var recipeResult = await this.FetchRecipeAsync(this.BarCode.ProductId, this.MachineId, cancellationToken).ConfigureAwait(false);

            if (recipeResult.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: Recipe not found. PartNumber={PartNumber}, MachineId={MachineId}, Validation={Validation}", this.PartNumber, this.MachineId, ResultValidation.RecipeNotFound);
                return this.SetFailureAndReturn($"Recipe not found for part number {this.PartNumber} and machine {this.MachineId}", ResultValidation.RecipeNotFound);
            }

            if (recipeResult.Value is not null)
            {
                this.Recipe = recipeResult.Value;
            }

            var shiftResult = await this.FetchCurrentShiftAsync(cancellationToken).ConfigureAwait(false);

            if (shiftResult.IsFailure)
            {
                logger.LogWarning("GetBarCodeDetails:: Shift not found. Validation={Validation}", ResultValidation.None);

                // Create new shift and set shiftID and result on Shift ?? (no etBarCodeDetails:)
            }

            if (shiftResult.Value is not null)
            {
                this.Shift = shiftResult.Value;
            }

            this.AssignWorkflowAndMachineDetails(this.Machine, this.BarCode, this.Cycle);

            logger.LogInformation("GetBarCodeDetails:: Assigned workflow and machine details. WorkFlowType={WorkFlowType}, MachineType={MachineType}, BarCodeId={BarCodeId}, CycleId={CycleId}, CycleStatus={CycleStatus}, PartStatus={PartStatus}, FlowStatus={FlowStatus}",
                this.WorkFlowType, this.MachineType, this.BarCodeId, this.CycleId, this.CycleStatus, this.PartStatus, this.FlowStatus);

            this.ResultValidation = validationService.Validate(this.FlowStatus, this.MachineType, this.CycleStatus, this.PartStatus, this.MachineId, this.NextMachineId);

            if (!Equals(this.ResultValidation, ResultValidation.Valid))
            {
                logger.LogWarning("GetBarCodeDetails:: Failed Validation for: Barcode:{BarCode}, MachineId={MachineId}, Validation={Validation}", barCodeDetailsRequest.Label, this.MachineId, this.ResultValidation);
                return this.LogAndReturnValidationFailure();
            }

            var masterLabelResult = await this.FetchMasterLabelAsync(this.Label, cancellationToken).ConfigureAwait(false);

            if (masterLabelResult.IsSuccess)
            {
                this.MasterLabel = masterLabelResult.Value!;
                this.UpdateStatusForMasterLabel();
            }

            this.ResultValidation = ResultValidation.Valid;

            logger.LogInformation("GetBarCodeDetails:: Successful Validation for: Barcode:{BarCode}, MachineId={MachineId}, Validation={Validation}", barCodeDetailsRequest.Label, this.MachineId, this.ResultValidation);
            logger.LogWarning("GetBarCodeDetails:: Completed processing. Label={Label}, MachineId={MachineId}, Validation={Validation}", this.Label, this.MachineId, this.ResultValidation);

            return this;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetBarCodeDetails:: Exception Occurer.");
            return this.SetFailureAndReturn("Operation cancelled", ex, ResultValidation.ExceptionResultValidation);
        }
    }

    private bool HasMaxAllowedCyclesOnStation(BarCodeDetailsRequest request)
    {
        return this.Cycles
            .Count(c => c.MachineId == request.MachineId && c.CycleStatus == CycleStatus.FinishedOk) >= this.Recipe.MaxCyclesOk;
    }

    private bool HasMaxAllowedCyclesNotOkOnStation(BarCodeDetailsRequest request)
    {
        return this.Cycles
            .Count(c => c.MachineId == request.MachineId && c.CycleStatus == CycleStatus.FinishedNok) >= this.Recipe.MaxCyclesNOk;
    }

    private bool ValidatePartNumber(string label, string partNumber)
    {
        if (label.Contains(partNumber))
        {
            return true;
        }

        logger.LogError("BarCode {Label} doesn't match the part number {PartNumber}", label, partNumber);
        return false;
    }

    private Result<Cycle?> FetchLastCycleCycleAsync(Result<IEnumerable<Cycle>> cycles)
    {
        if (cycles.Value is null)
        {
            return Result<Cycle?>.WithFailure("Cycles collection is null");
        }

        var result = cycles.Value.OrderByDescending(c => c.CycleId).FirstOrDefault();
        if (result is null)
        {
            return Result<Cycle?>.WithFailure("Cycle not found");
        }

        return Result<Cycle?>.Success(result);
    }

    private int DetermineNextMachineId(Dictionary<int, WorkFlow> vmFt, int lastMachineId, Cycle? cycle)
    {
        if (cycle != null && cycle.CycleStatus == CycleStatus.FinishedOk.Value)
        {
            return vmFt[lastMachineId].NextMachineId;
        }

        return lastMachineId;
    }

    private int DetermineLastMachineId(Cycle? cycle, BarCode barCode)
    {
        return cycle?.MachineId ?? barCode.MachineId;
    }

    private async Task HandleProcessMachineTypeAsync(
        CancellationToken cancellationToken,
        Dictionary<int, WorkFlow> vmFt)
    {
        var specNextMachine = new Specification<Domain.Entities.Machine>(p => p.MachineId == this.NextMachineId);
        var nextMachineResult = await machineRepository.FirstOrDefaultAsync(specNextMachine, cancellationToken).ConfigureAwait(false);

        if (nextMachineResult.IsFailure)
        {
            this.NextMachineId = 0;
        }
        else
        {
            if (nextMachineResult.Value is not null)
            {
                this.UpdateNextMachineIdIfDisabled(nextMachineResult.Value, vmFt);
            }
        }
    }

    private void UpdateNextMachineIdIfDisabled(Machine nextMachine, Dictionary<int, WorkFlow> vmFt)
    {
        if (nextMachine.MachineType == MachineType.Process && !nextMachine.IsEnabled)
        {
            this.NextMachineId = vmFt[this.NextMachineId].NextMachineId;
        }
    }

    private void UpdateStatusForMasterLabel()
    {
        if (this.MasterLabel?.MasterLabelId == 0)
        {
            return;
        }

        this.PartStatus = PartStatus.Ok;
        this.CycleStatus = CycleStatus.FinishedOk;
        this.FlowStatus = FlowStatus.InProcess;
        this.WorkFlowType = WorkFlowType.Serial;
        this.MachineType = MachineType.DashBoard;

        this.ResultValidation = ResultValidation.Valid;
    }

    private void HandleRestoredBarCode()
    {
        this.ResultValidation = ResultValidation.Valid;

        logger.LogInformation("WorkFlow for this BarCode {Label} was restored", this.Label);

        this.MachineId = this.MachineId;
        this.NextMachineId = this.MachineId;

        this.PartStatus = PartStatus.Ok;
        this.CycleStatus = CycleStatus.NotStarted;
        this.FlowStatus = FlowStatus.Restored;
        this.WorkFlowType = WorkFlowType.None;
        this.MachineType = MachineType.None;
        this.CyclesOk = 0;
    }

    private void AssignWorkflowAndMachineDetails(Machine machineInfo, BarCode barCode, Cycle? cycle)
    {
        this.WorkFlowType = machineInfo.WorkFlowType;
        this.MachineType = machineInfo.MachineType;
        this.BarCodeId = barCode.BarCodeId;
        this.CycleId = cycle?.CycleId ?? 0;

        // Fix for CS9135: A constant value of type 'CycleStatus' is expected
        this.CycleStatus = cycle?.CycleStatus is null || cycle.CycleStatus.Equals(CycleStatus.None) ? CycleStatus.NotStarted : cycle.CycleStatus;
        this.PartStatus = barCode.PartStatus;
        this.FlowStatus = barCode.FlowStatus;
    }

    private bool IsBarCodeRestored()
    {
        return this.BarCode.FlowStatus == FlowStatus.Restored.Value && this.BarCode.PartStatus == PartStatus.Restored.Value;
    }

    /// <summary>
    /// Maps the source barcode result to a destination DTO.
    /// </summary>
    /// <typeparam name="TDest">The type of the destination DTO.</typeparam>
    /// <param name="src">The source barcode result.</param>
    /// <param name="dest">The destination DTO.</param>
    /// <returns>The mapped destination DTO.</returns>
    /// <summary>
    /// Maps the source barcode result to a destination DTO.
    /// </summary>
    /// <typeparam name="TDest">The type of the destination DTO.</typeparam>
    /// <param name="src">The source barcode result.</param>
    /// <param name="dest">The destination DTO.</param>
    /// <returns>The mapped destination DTO.</returns>
    public static Result<TDest> ToDto<TDest>(BarCodeResult src, TDest dest)
        where TDest : new()
    {
        if (src == null)
        {
            return Result<TDest>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        if (dest == null)
        {
            dest = new TDest();
        }

        if (dest is TaskGatewayResponse response)
        {
            response.BarCodeId = src.BarCodeId;
            response.BarCode = src.BarCode;
            response.Cycle = src.Cycle;
            response.Recipe = src.Recipe;
            response.Name = src.Name;

            // Map other properties as needed
            return Result<TDest>.Success((TDest)(object)response);
        }

        if (dest is TaskGatewayRequest request)
        {
            request.BarCodeId = src.BarCodeId;

            // Map other properties as needed
            return Result<TDest>.Success((TDest)(object)request);
        }

        if (dest is BarCodeResultData data)
        {
            data.MachineId = src.MachineId;
            data.BarCodeId = src.BarCodeId;
            data.CycleId = src.CycleId;
            data.CyclesOk = src.CyclesOk;
            data.ShiftId = src.ShiftId;
            data.CommandId = src.CommandId;
            data.ResultValidation = src.ResultValidation;
            data.Error = src.Error;
            data.Label = src.Label;
            data.PartNumber = src.PartNumber;
            data.Description = src.Description;
            data.LastMachineId = src.LastMachineId;
            data.NextMachineId = src.NextMachineId;
            data.RegistersSaved = src.RegistersSaved;
            data.Shift = src.Shift;
            data.LastShift = src.LastShift;
            data.Product = src.Product;
            data.Command = src.Command;
            data.CycleStatus = src.CycleStatus;
            data.FlowStatus = src.FlowStatus;
            data.PartStatus = src.PartStatus;
            data.MachineType = src.MachineType;
            data.WorkFlowType = src.WorkFlowType;
            data.Recipe = src.Recipe;
            data.Machine = src.Machine;
            data.Cycle = src.Cycle;
            data.Cycles = src.Cycles;
            data.BarCode = src.BarCode;
            data.MasterLabel = src.MasterLabel;
            data.References = src.References;
            return Result<TDest>.Success((TDest)(object)data);
        }

        return Result<TDest>.Success(dest);
    }

    /// <summary>
    /// Maps the source DTO to a destination barcode result entity.
    /// </summary>
    /// <typeparam name="T">The type of the source DTO.</typeparam>
    /// <param name="src">The source DTO.</param>
    /// <param name="dest">The destination barcode result entity.</param>
    /// <returns>The mapped barcode result entity.</returns>
    /// <summary>
    /// Maps the source DTO to a destination barcode result entity.
    /// </summary>
    /// <typeparam name="T">The type of the source DTO.</typeparam>
    /// <param name="src">The source DTO.</param>
    /// <param name="dest">The destination barcode result entity.</param>
    /// <returns>The mapped barcode result entity.</returns>
    public static Result<BarCodeResult> ToEntity<T>(T src, BarCodeResult dest)
    {
        if (src == null)
        {
            return Result<BarCodeResult>.WithFailure($"Parameter '{nameof(src)}' cannot be null");
        }

        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest), "BarCodeResult destination cannot be null - dependency injection required");
        }

        if (src is TaskGatewayResponse response)
        {
            dest.BarCodeId = response.BarCodeId;
            dest.BarCode = response.BarCode;
            dest.Cycle = response.Cycle;
            dest.Recipe = response.Recipe;
            dest.Name = response.Name;

            // Map other properties as needed
            return Result<BarCodeResult>.Success(dest);
        }

        if (src is TaskGatewayRequest request)
        {
            dest.BarCodeId = request.BarCodeId;

            // Map other properties as needed
            return Result<BarCodeResult>.Success(dest);
        }

        if (src is BarCodeResultData data)
        {
            dest.MachineId = data.MachineId;
            dest.BarCodeId = data.BarCodeId;
            dest.CycleId = data.CycleId;
            dest.CyclesOk = data.CyclesOk;
            dest.ShiftId = data.ShiftId;
            dest.CommandId = data.CommandId;
            dest.ResultValidation = data.ResultValidation;
            dest.Error = data.Error;
            dest.Label = data.Label;
            dest.PartNumber = data.PartNumber;
            dest.Description = data.Description;
            dest.LastMachineId = data.LastMachineId;
            dest.NextMachineId = data.NextMachineId;
            dest.RegistersSaved = data.RegistersSaved;
            dest.Shift = data.Shift;
            dest.LastShift = data.LastShift;
            dest.Product = data.Product;
            dest.Command = data.Command;
            dest.CycleStatus = data.CycleStatus;
            dest.FlowStatus = data.FlowStatus;
            dest.PartStatus = data.PartStatus;
            dest.MachineType = data.MachineType;
            dest.WorkFlowType = data.WorkFlowType;
            dest.Recipe = data.Recipe;
            dest.Machine = data.Machine;
            dest.Cycle = data.Cycle;
            dest.Cycles = data.Cycles;
            dest.BarCode = data.BarCode;
            dest.MasterLabel = data.MasterLabel;
            dest.References = data.References;
            return Result<BarCodeResult>.Success(dest);
        }

        return Result<BarCodeResult>.Success(dest);
    }

    private IBarCodeResult SetFailureAndReturn(string errorMessage, ResultValidation validation)
    {
        this.SetFailureResult(errorMessage, validation);
        return this;
    }

    private IBarCodeResult SetFailureAndReturn(string errorMessage, Exception ex, ResultValidation validation)
    {
        this.SetFailureResult(errorMessage + " " + ex.Message, validation);
        return this;
    }

    private IBarCodeResult LogAndReturnValidationFailure()
    {
        logger.LogError("Validation failed {ResultValidation}", this.ResultValidation);
        this.Error = "Validation failed " + this.ResultValidation;
        return this;
    }

    private void SetFailureResult(string errorMessage, ResultValidation validation)
    {
        this.Error = errorMessage;
        this.ResultValidation = validation;
        this.PartStatus = PartStatus.NOk;
        this.CycleStatus = CycleStatus.NotStarted;
        this.FlowStatus = FlowStatus.Invalid;
        this.WorkFlowType = WorkFlowType.None;
        this.MachineType = MachineType.None;

        logger.LogError("BarCode validation failed: {ErrorMessage}", errorMessage);
    }

    /// <summary>
    /// Updates barcode information on the cycle.
    /// </summary>
    /// <param name="flowStatus">The flow status.</param>
    /// <param name="partStatus">The part status.</param>
    /// <param name="cycleStatus">The cycle status.</param>
    public void UpdateBarCodeInformationOnCycle(FlowStatus flowStatus, PartStatus partStatus, CycleStatus cycleStatus)
    {
        this.FlowStatus = flowStatus;
        this.PartStatus = partStatus;
        this.CycleStatus = cycleStatus;
    }

    /// <summary>
    /// Sets the barcode information.
    /// </summary>
    /// <param name="barCode">The barcode.</param>
    public void SetBarCode(BarCode barCode)
    {
        if (barCode is not null)
        {
            this.BarCode = barCode;
        }
    }

    /// <summary>
    /// Sets the cycle information.
    /// </summary>
    /// <param name="cycle">The cycle.</param>
    public void SetCycle(Cycle cycle)
    {
        if (cycle is not null)
        {
            this.Cycle = cycle;
        }
    }

    /// <summary>
    /// Sets the number of cycles marked as OK.
    /// </summary>
    /// <param name="cyclesOk">The number of cycles marked as OK.</param>
    public void SetCyclesOk(int cyclesOk)
    {
        if (cyclesOk > 0)
        {
            this.CyclesOk = cyclesOk;
        }
    }

    /// <summary>
    /// Sets the number of registers saved.
    /// </summary>
    /// <param name="registers">The number of registers saved.</param>
    public void SetRegistersSaved(int registers)
    {
        if (registers > 0)
        {
            this.RegistersSaved = registers;
        }
    }

    /// <summary>
    /// Initializes the references dictionary.
    /// </summary>
    /// <param name="references">The references dictionary.</param>
    public void InitReferences(IDictionary<string, Register> references)
    {
        this.References = references;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"BarCodeResult: " +
               $"Label='{this.Label}', " +
               $"MachineId={this.MachineId}, " +
               $"PartNumber='{this.PartNumber}', " +
               $"NextMachineId={this.NextMachineId}, " +
               $"LastMachineId={this.LastMachineId}, " +
               $"BarCodeId={this.BarCodeId}, " +
               $"CycleId={this.CycleId}, " +
               $"CyclesOk={this.CyclesOk}, " +
               $"ShiftId={this.ShiftId}";
    }
}
