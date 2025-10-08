// <copyright file="CreateBarCodeCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Create;

using IndQuestResults.Operations;
using Machine = IndTrace.Domain.Entities.Machine;

/// <summary>
/// Handles the creation of new bar codes by processing CreateBarCodeCommand requests.
/// Validates machine existence, creates bar code with consecutive numbering, and initializes associated cycle.
/// </summary>
// [Fix]
// CLAUDE
// Date: 25/08/2025
// Reason: [ARCHITECTURAL CLEANUP] - Add IBarCodeService to complete extension method migration
public class CreateBarCodeCommandHandler(
    IReadOnlyRepository<Rule> ruleRepository,
    IRepository<Cycle> cycleRepository,
    IReadOnlyRepository<Machine> machineRepository,
    IRepository<BarCode> barCodeRepository,
    IReadOnlyRepository<Product> productRepository,
    IReadOnlyRepository<Variable> variableRepository,
    IRepository<TaskGatewayRequest> requestRepository,
    IShiftService shiftService,
    IDateTimeMachine dateTimeMachine,
    IMasterLabelService masterLabelService,
    IBarCodeService barCodeService,
    ILogger<CreateBarCodeCommandHandler> logger) : IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>, IResettable
{
    /// <summary>
    /// Processes the create barcode command asynchronously.
    /// </summary>
    /// <param name="cmd">The create barcode command containing the request details.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{T}"/> with a <see cref="TaskGatewayResponse"/> if successful,
    /// or error information if the operation fails.
    /// </returns>
    /// <remarks>
    /// This method performs the following steps:
    /// 1. Validates the machine ID and ensures it's a printer type
    /// 2. Looks up the product by part number
    /// 3. Retrieves the applicable rule for the machine-product combination
    /// 4. Generates a unique barcode label using the rule engine
    /// 5. Creates a new barcode entity
    /// 6. Creates an associated cycle for tracking
    /// 7. Manages shift information
    /// 8. Collects reference variables for the response
    /// 9. Returns a comprehensive response with all related data.
    /// </remarks>
    private record CreateBarcodeContext(TaskGatewayRequest Request, Machine Machine, Product Product, Rule Rule, int Consecutive, string Label, BarCode Barcode, Cycle Cycle, Shift Shift, Dictionary<string, Register> References);

    public async Task<Result<TaskGatewayResponse>> ProcessAsync(CreateBarCodeCommand cmd, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TaskGatewayResponse>.WithFailure("Operation was canceled.");
        }

        var result = await Result.Success(cmd.Command)
            .ValidateNotNull(req => (req, nameof(req)))
            .Ensure(req => req.MachineId > 0, $"Machine {cmd.Command.MachineId} number invalid")
            .ThenAsync(req => GetMachineAsync(req, cancellationToken))
            .ThenAsync(context => GetProductAsync(context, cancellationToken))
            .ThenAsync(context => GetRuleAsync(context, cancellationToken))
            .ThenAsync(context => GetMasterLabelAndConsecutiveAsync(context, cancellationToken))
            .Then(context => GenerateLabel(context))
            .ThenAsync(context => CreateBarcodeEntityAsync(context, cancellationToken))
            .ThenAsync(context => CreateCycleEntityAsync(context, cancellationToken))
            .ThenAsync(context => GetShiftInfoAsync(context, cancellationToken))
            .ThenAsync(context => GetVariablesAndCreateReferencesAsync(context, cancellationToken))
            .ThenMap(context => BuildFinalResponse(context));

        await result.TapError(async error =>
        {
            var command = new TaskGatewayRequest
            {
                MachineId = cmd.Command.MachineId,
                ResultValidation = ResultValidation.Invalid,
                Comment = string.Join("; ", error),
                GatewayTask = GatewayTask.CreateBarCodeAsync,
                TimeStamp = dateTimeMachine.Now.ToLocalTime(),
            };
            await requestRepository.AddAsync(command, cancellationToken);
        });

        return result;
    }

    private async Task<Result<CreateBarcodeContext>> GetMachineAsync(TaskGatewayRequest request, CancellationToken cancellationToken)
    {
        var spec = new Specification<Machine>(m => m.MachineId == request.MachineId && (m.MachineType == MachineType.Printer || m.MachineType == MachineType.InitialPrinter));
        return await machineRepository.FirstOrDefaultAsync(spec, cancellationToken)
            .ToResult($"Machine {request.MachineId} does not exist or cannot create labels.")
            .ThenMap(machine => new CreateBarcodeContext(request, machine, null!, null!, 0, null!, null!, null!, null!, null!));
    }

    private async Task<Result<CreateBarcodeContext>> GetProductAsync(CreateBarcodeContext context, CancellationToken cancellationToken)
    {
        var spec = new Specification<Product>(p => p.PartNumber == context.Request.PartNumber);
        return await productRepository.FirstOrDefaultAsync(spec, cancellationToken)
            .ToResult($"Product for {context.Request.PartNumber} does not exist.")
            .ThenMap(product => context with { Product = product });
    }

    private async Task<Result<CreateBarcodeContext>> GetRuleAsync(CreateBarcodeContext context, CancellationToken cancellationToken)
    {
        var spec = new Specification<Rule>(r => r.MachineId == context.Machine.MachineId && r.ProductId == context.Product.ProductId && r.IsActive);
        spec.AddOrderByDescending(r => r.Version);
        return await ruleRepository.FirstOrDefaultAsync(spec, cancellationToken)
            .ToResult($"Rule for Machine {context.Machine.MachineId} does not exist.")
            .ThenMap(rule => context with { Rule = rule });
    }

    private async Task<Result<CreateBarcodeContext>> GetMasterLabelAndConsecutiveAsync(CreateBarcodeContext context, CancellationToken cancellationToken)
    {
        var masterLabelResult = await masterLabelService.GetMasterLabelByPartNumberAsync(context.Request.PartNumber, cancellationToken);
        if (masterLabelResult is null || masterLabelResult.IsFailure)
        {
            return Result<CreateBarcodeContext>.WithFailure("Failed to retrieve master labels.");
        }

        return await barCodeService.GetConsecutiveByBarCodeLabelAsync(context.Request.PartNumber, masterLabelResult.Value ?? [], cancellationToken)
            .ThenMap(consecutive => context with { Consecutive = consecutive });
    }

    private Result<CreateBarcodeContext> GenerateLabel(CreateBarcodeContext context)
    {
        var ruleExecutor = new CreateBarCodeDictionaryExecutor(dateTimeMachine);
        ruleExecutor.ParseRuleFromJson(context.Rule.RuleJson ?? string.Empty);
        ruleExecutor.InitializeComponentActions();
        return ruleExecutor.ApplyRuleCreateBarCode(context.Request.PartNumber, context.Consecutive)
            .ThenMap(label => context with { Label = label });
    }

    private async Task<Result<CreateBarcodeContext>> CreateBarcodeEntityAsync(CreateBarcodeContext context, CancellationToken cancellationToken)
    {
        var barcode = new BarCode
        {
            Label = context.Label,
            ProductId = context.Product.ProductId,
            MachineId = context.Request.MachineId,
            CreatedOn = dateTimeMachine.Now.ToLocalTime(),
            ModifiedOn = dateTimeMachine.Now.ToLocalTime(),
            FlowStatus = FlowStatus.Created,
            PartStatus = PartStatus.Ok,
        };
        return await barCodeRepository.AddAsync(barcode, cancellationToken)
            .ThenMap(id => context with { Barcode = barcode with { BarCodeId = id } });
    }

    private async Task<Result<CreateBarcodeContext>> CreateCycleEntityAsync(CreateBarcodeContext context, CancellationToken cancellationToken)
    {
        var cycle = new Cycle
        {
            MachineId = context.Request.MachineId,
            BarCodeId = context.Barcode.BarCodeId,
            CycleStatus = CycleStatus.Started,
            PartStatus = PartStatus.Ok,
            StartedOn = dateTimeMachine.Now.ToLocalTime(),
            FinishedOn = dateTimeMachine.Now.ToLocalTime(),
        };
        return await cycleRepository.AddAsync(cycle, cancellationToken)
            .ThenMap(id => context with { Cycle = cycle with { CycleId = id } });
    }

    private async Task<Result<CreateBarcodeContext>> GetShiftInfoAsync(CreateBarcodeContext context, CancellationToken cancellationToken)
    {
        return await shiftService.CreateOrRetrieveShiftAndCyclesOkAsync(context.Request.MachineId, cancellationToken)
            .ThenMap(shift => context with { Shift = shift, Cycle = context.Cycle with { CyclesOk = shift.CyclesOk } });
    }

    private async Task<Result<CreateBarcodeContext>> GetVariablesAndCreateReferencesAsync(CreateBarcodeContext context, CancellationToken cancellationToken)
    {
        var spec = new Specification<Variable>(r => r.MachineId == context.Machine.MachineId && r.IsActive == 1 && r.VariableGroupId == TagsGroups.ReferenceTags.Value);
        return await variableRepository.ListAsync(spec, cancellationToken)
            .Ensure(variables => variables.Any(), $"References for {context.Request.PartNumber} not found.")
            .ThenMap(variables =>
            {
                var references = variables.ToDictionary(
                    v => v.Name,
                    v => new Register
                    {
                        Name = v.Name,
                        VariableId = v.VariableId,
                        CycleId = context.Cycle.CycleId,
                        Value = v.Value,
                        DataType = v.NativeType,
                        StatusValueId = 1,
                    });
                return context with { References = references };
            });
    }

    private Result<TaskGatewayResponse> BuildFinalResponse(CreateBarcodeContext context)
    {
        var response = new TaskGatewayResponse()
             .WithMachineId(context.Request.MachineId)
             .WithDescription(context.Machine.Name ?? "Unknown Machine")
             .WithName(context.Machine.Name ?? "Unknown Machine")
             .WithBarCodeId(context.Barcode.BarCodeId)
             .WithCycleId(context.Cycle.CycleId)
             .WithCyclesOk(context.Cycle.CyclesOk)
             .WithResultValidation(ResultValidation.Valid)
             .WithPartNumber(context.Request.PartNumber)
             .WithLastMachineId(context.Request.MachineId)
             .WithNextMachineId(context.Request.MachineId)
             .WithCycleStatus(context.Cycle.CycleStatus)
             .WithFlowStatus(context.Barcode.FlowStatus)
             .WithPartStatus(context.Barcode.PartStatus)
             .WithMachineType(context.Machine.MachineType)
             .WithWorkFlowType(context.Machine.WorkFlowType)
             .WithCycle(context.Cycle)
             .WithBarCode(context.Barcode)
             .WithReferences(context.References);

        return response.ApplyReferencesValuesResult().Then(_ => Result.Success(response));
    }

    /// <summary>
    /// Resets the command handler to its initial state.
    /// </summary>
    /// <returns>True if the reset operation was successful.</returns>
    public bool TryReset()
    {
        return true;
    }
}
