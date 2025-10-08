// <copyright file="GetBarCodeQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCode;

/// <summary>
/// Defines a query to retrieve bar code details.
/// </summary>
public class GetBarCodeQuery(
    IReadOnlyRepository<MasterLabel> repositoryMasterLabel,
    IReadOnlyRepository<Cycle> repositoryCycles,
    IReadOnlyRepository<BarCode> repositoryBarcode,
    IReadOnlyRepository<WorkFlow> repositoryWorkFlow,
    IReadOnlyRepository<Machine> repositoryMachine)
{
    // Properties declaration

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
    /// Gets or sets the ResultValidation.
    /// </summary>
    public ResultValidation? ResultValidation { get; set; }

    /// <summary>
    /// Gets or sets the Label.
    /// </summary>
    public string? Label { get; set; }

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
    public CycleStatus? CycleStatus { get; set; }

    /// <summary>
    /// Gets or sets the FlowStatus.
    /// </summary>
    public FlowStatus? FlowStatus { get; set; }

    /// <summary>
    /// Gets or sets the PartStatus.
    /// </summary>
    public PartStatus? PartStatus { get; set; }

    /// <summary>
    /// Gets or sets the MachineType.
    /// </summary>
    public MachineType? MachineType { get; set; }

    /// <summary>
    /// Gets or sets the WorkFlowType.
    /// </summary>
    public WorkFlowType? WorkFlowType { get; set; }

    /// <summary>
    /// Gets or sets the Cycle.
    /// </summary>
    public Cycle? Cycle { get; set; }

    /// <summary>
    /// Gets or sets the BarCode.
    /// </summary>
    public BarCode? BarCode { get; set; }

    /// <summary>
    /// Gets or sets the MasterLabel.
    /// </summary>
    public MasterLabel? MasterLabel { get; set; }

    /// <summary>
    /// Gets or sets the References.
    /// </summary>
    public IDictionary<string, Register>? References { get; set; }

    /// <summary>
    /// Executes DetailVm operation.
    /// </summary>
    /// <returns>The result of DetailVm.</returns>
    public async Task<Result<GetBarCodeQuery>> DetailVm(
        string label,
        int machineId,
        CancellationToken cancellationToken)
    {
        // Validate input parameters
        var validationResult = ValidateConstructorParameters(label);
        if (validationResult.IsFailure)
        {
            return Result<GetBarCodeQuery>.WithFailure(validationResult.Errors!);
        }

        // Capture the local variables
        this.Label = label;
        this.MachineId = machineId;

        this.ResultValidation = ResultValidation.Invalid;

        var masterLabelsResult = await this.LoadMasterLabelAsync(cancellationToken);
        if (masterLabelsResult.IsSuccess && masterLabelsResult.Value is not null)
        {
            this.MasterLabel = masterLabelsResult.Value;
        }

        var barCodeResult = await this.LoadBarCodeAsync(cancellationToken);
        if (barCodeResult.IsSuccess && barCodeResult.Value is not null)
        {
            this.BarCode = barCodeResult.Value;
        }

        var cycleResult = await this.LoadCycleAsync(cancellationToken);
        if (cycleResult.IsFailure)
        {
            return Result<GetBarCodeQuery>.WithFailure(cycleResult.Errors!);
        }

        var workFlowResult = await this.LoadWorkFlowsAsync(cancellationToken);
        if (workFlowResult.IsFailure)
        {
            return Result<GetBarCodeQuery>.WithFailure(workFlowResult.Errors!);
        }

        if (workFlowResult.Value is null)
        {
            return Result<GetBarCodeQuery>.WithFailure("WorkFlow value cannot be null");
        }

        var machineDetailsResult = await this.ProcessMachineDetails(workFlowResult.Value, cancellationToken);
        if (machineDetailsResult.IsFailure)
        {
            return Result<GetBarCodeQuery>.WithFailure(machineDetailsResult.Errors!);
        }

        // TODO Check if the two following methods are equivalent
        // ValidatePiece();
        var evaluationResult = this.EvaluateMachineProcess();
        if (evaluationResult.IsFailure)
        {
            return Result<GetBarCodeQuery>.WithFailure(evaluationResult.Errors!);
        }

        var referencesResult = this.AssignReferences();
        if (referencesResult.IsFailure)
        {
            return Result<GetBarCodeQuery>.WithFailure(referencesResult.Errors!);
        }

        return Result<GetBarCodeQuery>.Success(this);
    }

    /// <summary>
    /// Asynchronously loads the MasterLabel based on the Label property.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task<Result<MasterLabel>> LoadMasterLabelAsync(CancellationToken cancellationToken)
    {
        var allMasterLabelsResult = await repositoryMasterLabel.ListAsync(cancellationToken);
        if (allMasterLabelsResult.IsFailure)
        {
            return Result<MasterLabel>.WithFailure(allMasterLabelsResult.Errors);
        }

        if (allMasterLabelsResult.Value is null)
        {
            return Result<MasterLabel>.WithFailure($"No MasterLabels found");
        }

        var matches = allMasterLabelsResult.Value
            .Where(b => b.MasterLabelCode == this.Label)
            .ToList();

        return matches.Count switch
        {
            0 => Result<MasterLabel>.WithFailure($"No MasterLabel found for code: {this.Label}"),
            1 => Result<MasterLabel>.Success(matches[0]),
            _ => Result<MasterLabel>.WithFailure($"Multiple MasterLabels found for code: {this.Label}", matches.First()),
        };
    }

    /// <summary>
    /// Asynchronously loads the BarCode based on the Label property.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task<Result<BarCode>> LoadBarCodeAsync(CancellationToken cancellationToken)
    {
        var allBarCodesResult = await repositoryBarcode.ListAsync(cancellationToken);
        if (allBarCodesResult.IsFailure)
        {
            return Result<BarCode>.WithFailure(allBarCodesResult.Errors);
        }

        if (allBarCodesResult.Value is null)
        {
            return Result<BarCode>.WithFailure($"No BarCodes found");
        }

        var matches = allBarCodesResult.Value
            .Where(b => b.Label == this.Label)
            .OrderByDescending(b => b.BarCodeId)
            .ToList();

        return matches.Count switch
        {
            0 => Result<BarCode>.WithFailure($"No BarCode found for label: {this.Label}"),
            1 => Result<BarCode>.Success(matches[0]),
            _ => Result<BarCode>.WithFailure($"Multiple BarCodes found for label: {this.Label}", matches[0]),
        };
    }

    /// <summary>
    /// Asynchronously loads the Cycle based on the BarCode property.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task<Result> LoadCycleAsync(CancellationToken cancellationToken)
    {
        if (this.BarCode is null)
        {
            return Result.WithFailure("BarCode must be non-null before loading cycle");
        }

        var allCyclesResult = await repositoryCycles.ListAsync(cancellationToken);
        if (allCyclesResult.IsFailure)
        {
            return Result.WithFailure(allCyclesResult.Errors);
        }

        if (allCyclesResult.Value is null)
        {
            return Result.WithFailure($"No Cycles found");
        }

        this.Cycle = allCyclesResult.Value
            .Where(c => c.BarCodeId == this.BarCode.BarCodeId)
            .OrderByDescending(c => c.CycleId)
            .FirstOrDefault();

        if (this.Cycle is null)
        {
            return Result.WithFailure("Cycle must be non-null");
        }

        return Result.Success();
    }

    /// <summary>
    /// Asynchronously loads the workflows related to the product in the BarCode.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The WorkFlows as a dictionary.</returns>
    private async Task<Result<IDictionary<int, WorkFlow>>> LoadWorkFlowsAsync(CancellationToken cancellationToken)
    {
        if (this.BarCode is null)
        {
            return Result<IDictionary<int, WorkFlow>>.WithFailure("BarCode must be non-null before loading workflows");
        }

        // TODO Implement a better structure for more complicated works flow
        // TODO Implement a better structure for more complicated workflows
        var allWorkFlowsResult = await repositoryWorkFlow.ListAsync(cancellationToken);
        if (allWorkFlowsResult.IsFailure)
        {
            return Result<IDictionary<int, WorkFlow>>.WithFailure(allWorkFlowsResult.Errors);
        }

        if (allWorkFlowsResult.Value is null)
        {
            return Result<IDictionary<int, WorkFlow>>.WithFailure($"No WorkFlows found");
        }

        var workflows = allWorkFlowsResult.Value
                   .Where(f => f.ProductId == this.BarCode.ProductId)
                   .OrderBy(f => f.NextMachineId)
                   .ToDictionary(f => f.LastMachineId);

        if (workflows is null || !workflows.Any())
        {
            return Result<IDictionary<int, WorkFlow>>.WithFailure($"No workflows found for label: {this.Label}");
        }

        return Result<IDictionary<int, WorkFlow>>.Success(workflows);
    }

    /// <summary>
    /// Processes machine details and assigns relevant properties.
    /// </summary>
    /// <param name="vmFt">Workflow dictionary.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task<Result> ProcessMachineDetails(IDictionary<int, WorkFlow> vmFt, CancellationToken cancellationToken)
    {
        if (this.BarCode is null)
        {
            return Result.WithFailure("BarCode must be non-null");
        }

        if (this.Cycle is null)
        {
            return Result.WithFailure("Cycle must be non-null");
        }

        this.LastMachineId = this.Cycle.MachineId;
        if (this.LastMachineId == 0)
        {
            return Result.WithFailure("Machine RecipeId Not Valid");
        }

        if (!vmFt.ContainsKey(this.LastMachineId))
        {
            return Result.WithFailure("workflow not valid");
        }

        var allMachinesResult = await repositoryMachine.ListAsync(cancellationToken);
        if (allMachinesResult.IsFailure)
        {
            return Result.WithFailure(allMachinesResult.Errors);
        }

        if (allMachinesResult.Value is null)
        {
            return Result.WithFailure($"No Machines found");
        }

        Machine? machineInfo = allMachinesResult.Value
                    .Where(m => m.MachineId == this.MachineId)
                    .FirstOrDefault();

        if (machineInfo is null)
        {
            return Result.WithFailure($"Machine not found with ID: {this.MachineId}");
        }

        if (this.Cycle!.CycleStatus == CycleStatus.FinishedOk.Value)
        {
            this.NextMachineId = vmFt[this.LastMachineId].NextMachineId;
        }
        else
        {
            this.NextMachineId = this.LastMachineId;
        }

        this.MachineType = machineInfo.MachineType;
        this.BarCodeId = this.BarCode!.BarCodeId;
        this.CycleId = this.Cycle!.CycleId;
        this.PartStatus = this.BarCode!.PartStatus;
        this.CycleStatus = this.Cycle!.CycleStatus;
        this.FlowStatus = this.BarCode!.FlowStatus;

        return Result.Success();
    }

    /// <summary>
    /// Contains the refactored validation logic.
    /// </summary>
    /// <returns>Result indicating validation success or failure.</returns>
    private Result<bool> Validate()
    {
        var validationResult = this.ValidateRequiredProperties();
        if (validationResult.IsFailure)
        {
            return Result<bool>.WithFailure(validationResult.Errors!);
        }

        if (!Equals(this.PartStatus, PartStatus.Ok))
        {
            return Result<bool>.Success(false);
        }

        if (this.NextMachineId != this.MachineId)
        {
            return Result<bool>.Success(false);
        }

        // TODO TWO CASES WHERE ELIMINATED, ASSESS THE OVERALL IMPACT
        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.Created) &&
            this.MachineType!.Equals(MachineType.Printer) &&
            this.CycleStatus!.Equals(CycleStatus.Started))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.Created) &&
            this.MachineType!.Equals(MachineType.Initial))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.Process))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.Final) &&
            this.CycleStatus!.Equals(CycleStatus.NotStarted))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.Final) &&
            this.CycleStatus!.Equals(CycleStatus.NotStarted))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.Final) &&
            this.CycleStatus!.Equals(CycleStatus.FinishedOk))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.Final))
        {
            return Result<bool>.Success(false);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.DashBoard) &&
            this.CycleStatus!.Equals(CycleStatus.NotStarted))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.DashBoard) &&
            this.CycleStatus!.Equals(CycleStatus.FinishedOk))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.DashBoard))
        {
            return Result<bool>.Success(false);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//
        if (this.FlowStatus!.Equals(FlowStatus.InProcess) &&
            this.MachineType!.Equals(MachineType.DashBoard) &&
            this.CycleStatus!.Equals(CycleStatus.FinishedOk))
        {
            return Result<bool>.Success(true);
        }

        //------------------------------------------------------------------------------------------------------------//
        //------------------------------------------------------------------------------------------------------------//

        // TODO RETURN TRUE IF ENABLED SEVERAL BAR CODES BY PIECE, NON STANDARD MANUFACTURING PRACTICE,
        // TODO MAYBE WITH SOMETHING WAS WRONG IN THE OTHER SYSTEM
        if (this.FlowStatus!.Equals(FlowStatus.Finished) && this.MachineType!.Equals(MachineType.DashBoard)
                                                   && Equals(this.CycleStatus, CycleStatus.FinishedOk))
        {
            return Result<bool>.Success(false);
        }

        return Result<bool>.Success(false);
    } // end of code for method Validate

    /// <summary>
    /// Evaluates the status of the machine process.
    /// </summary>
    /// <returns>Result indicating evaluation success or failure.</returns>
    public Result<bool> EvaluateMachineProcess()
    {
        var validationResult = this.ValidateRequiredProperties();
        if (validationResult.IsFailure)
        {
            return Result<bool>.WithFailure(validationResult.Errors!);
        }

        // Check if the part status is OK
        if (!this.PartStatus!.Equals(PartStatus.Ok))
        {
            return Result<bool>.Success(false);
        }

        // Ensure that the next machine ID matches the current machine ID
        if (this.NextMachineId != this.MachineId)
        {
            return Result<bool>.Success(false);
        }

        // Evaluate the flow status
        if (this.FlowStatus!.Equals(FlowStatus.Created))
        {
            var createdResult = this.HandleFlowStatusCreated();
            return createdResult.IsFailure ? createdResult : Result<bool>.Success(createdResult.Value);
        }

        if (this.FlowStatus!.Equals(FlowStatus.InProcess))
        {
            var inProcessResult = this.HandleFlowStatusInProcess();
            return inProcessResult.IsFailure ? inProcessResult : Result<bool>.Success(inProcessResult.Value);
        }

        // If the flow is finished and other conditions are met, return false; otherwise, return true
        bool result = this.FlowStatus!.Equals(FlowStatus.Finished)
            && this.MachineType!.Equals(MachineType.DashBoard)
            && Equals(this.CycleStatus, CycleStatus.FinishedOk)
            ? false
            : true; // Add your logic for multiple barcodes here

        return Result<bool>.Success(result);
    }

    /// <summary>
    /// Handles the Created flow status.
    /// </summary>
    /// <returns>Result indicating if the conditions are met.</returns>
    private Result<bool> HandleFlowStatusCreated()
    {
        if (this.MachineType is null)
        {
            return Result<bool>.WithFailure("MachineType must be non-null");
        }

        if (this.CycleStatus is null)
        {
            return Result<bool>.WithFailure("CycleStatus must be non-null");
        }

        // Evaluate the machine type for the Created flow status
        bool result = (this.MachineType.Equals(MachineType.Printer) && this.CycleStatus.Equals(CycleStatus.Started))
            || this.MachineType.Equals(MachineType.Initial);
        return Result<bool>.Success(result);
    }

    /// <summary>
    /// Handles the InProcess flow status.
    /// </summary>
    /// <returns>Result indicating if the conditions are met.</returns>
    private Result<bool> HandleFlowStatusInProcess()
    {
        if (this.MachineType is null)
        {
            return Result<bool>.WithFailure("MachineType must be non-null");
        }

        // Evaluate the machine type for the InProcess flow status
        if (this.MachineType.Equals(MachineType.Final))
        {
            var finalResult = this.HandleFinalMachineType();
            if (finalResult.IsFailure)
            {
                return finalResult;
            }

            return Result<bool>.Success(finalResult.Value);
        }

        if (this.MachineType.Equals(MachineType.DashBoard))
        {
            var dashboardResult = this.HandleDashBoardMachineType();
            if (dashboardResult.IsFailure)
            {
                return dashboardResult;
            }

            return Result<bool>.Success(dashboardResult.Value);
        }

        if (this.MachineType.Equals(MachineType.Process))
        {
            return Result<bool>.Success(true);
        }

        return Result<bool>.Success(false);
    }

    /// <summary>
    /// Handles the Final machine type.
    /// </summary>
    /// <returns>Result indicating if the conditions are met.</returns>
    private Result<bool> HandleFinalMachineType()
    {
        if (this.CycleStatus is null)
        {
            return Result<bool>.WithFailure("CycleStatus must be non-null");
        }

        // Evaluate the cycle status for the Final machine type
        bool result = this.CycleStatus.Equals(CycleStatus.NotStarted) || this.CycleStatus.Equals(CycleStatus.FinishedOk);
        return Result<bool>.Success(result);
    }

    /// <summary>
    /// Handles the DashBoard machine type.
    /// </summary>
    /// <returns>Result indicating if the conditions are met.</returns>
    private Result<bool> HandleDashBoardMachineType()
    {
        if (this.CycleStatus is null)
        {
            return Result<bool>.WithFailure("CycleStatus must be non-null");
        }

        // Evaluate the cycle status for the DashBoard machine type
        bool result = this.CycleStatus.Equals(CycleStatus.NotStarted) || this.CycleStatus.Equals(CycleStatus.FinishedOk); // Add your logic for specific handling here
        return Result<bool>.Success(result);
    }

    /// <summary>
    /// Assigns reference values related to the barcode.
    /// </summary>
    /// <returns></returns>
    public Result AssignReferences()
    {
        if (this.References == null || this.References.Count == 0)
        {
            return Result.Success();
        }

        var validationResult = this.ValidateAllRequiredProperties();
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        var assignmentResult = this.AssignReference("LastMachineId", this.LastMachineId.ToString());
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("NextMachineId", this.NextMachineId.ToString());
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("CycleStatus", this.CycleStatus);
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("FlowStatus", this.FlowStatus);
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("PartStatus", this.PartStatus);
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("MachineType", this.MachineType);
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("WorkFlowType", this.WorkFlowType);
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("BarCodeId", this.BarCodeId.ToString());
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("CycleId", this.CycleId.ToString());
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("Label", this.Label);
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        assignmentResult = this.AssignReference("ResultValidation", this.ResultValidation);
        if (assignmentResult.IsFailure)
        {
            return assignmentResult;
        }

        return Result.Success();
    }

    private Result AssignReference(string key, object? value)
    {
        if (this.References is null)
        {
            return Result.WithFailure($"Error argument must not be null {nameof(this.References)}");
        }

        if (this.References.Count == 0)
        {
            return Result.WithFailure($"Error argument must not be empty {nameof(this.References)}");
        }

        if (this.References.TryGetValue(key, out var reference))
        {
            reference.Value = value?.ToString() ?? string.Empty;
        }

        return Result.Success();
    }

    /// <summary>
    /// Validates constructor parameters.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="label">The label to validate.</param>
    /// <returns>Result indicating validation success or failure.</returns>
    private static Result ValidateConstructorParameters(string? label)
    {
        var errors = new List<string>();

        if (label is null)
        {
            errors.Add("label must be non-null");
        }

        // Context validation removed - using repository pattern
        return errors.Any() ? Result.WithFailure(errors) : Result.Success();
    }

    /// <summary>
    /// Validates required properties for business logic operations.
    /// </summary>
    /// <returns>Result indicating validation success or failure.</returns>
    private Result ValidateRequiredProperties()
    {
        var errors = new List<string>();

        if (this.FlowStatus is null)
        {
            errors.Add("FlowStatus must be non-null");
        }

        if (this.MachineType is null)
        {
            errors.Add("MachineType must be non-null");
        }

        if (this.CycleStatus is null)
        {
            errors.Add("CycleStatus must be non-null");
        }

        if (this.PartStatus is null)
        {
            errors.Add("PartStatus must be non-null");
        }

        return errors.Any() ? Result.WithFailure(errors) : Result.Success();
    }

    /// <summary>
    /// Validates all required properties for reference assignment operations.
    /// </summary>
    /// <returns>Result indicating validation success or failure.</returns>
    private Result ValidateAllRequiredProperties()
    {
        var errors = new List<string>();

        if (this.FlowStatus is null)
        {
            errors.Add("FlowStatus must be non-null");
        }

        if (this.PartStatus is null)
        {
            errors.Add("PartStatus must be non-null");
        }

        if (this.WorkFlowType is null)
        {
            errors.Add("WorkFlowType must be non-null");
        }

        if (this.Label is null)
        {
            errors.Add("Label must be non-null");
        }

        if (this.MachineType is null)
        {
            errors.Add("MachineType must be non-null");
        }

        if (this.CycleStatus is null)
        {
            errors.Add("CycleStatus must be non-null");
        }

        if (this.ResultValidation is null)
        {
            errors.Add("ResultValidation must be non-null");
        }

        return errors.Any() ? Result.WithFailure(errors) : Result.Success();
    }
}
