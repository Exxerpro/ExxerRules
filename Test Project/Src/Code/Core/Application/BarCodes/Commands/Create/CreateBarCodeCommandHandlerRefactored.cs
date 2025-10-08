using System.Diagnostics;
using IndTrace.Application.BarCodes.Services.Interfaces;
using IndTrace.Application.Common.Services.Interfaces;
using IndTrace.Domain.Enum;

namespace IndTrace.Application.BarCodes.Commands.Create;

/// <summary>
/// Refactored CreateBarCodeCommandHandler using SRP services and fluent pipeline.
/// Orchestrates barcode creation through focused services following Railway-oriented programming.
/// </summary>
public class CreateBarCodeCommandHandlerRefactored : IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>
{
    private readonly IMachineValidator _machineValidator;
    private readonly IProductLookupService _productLookupService;
    private readonly IRuleExecutionService _ruleExecutionService;
    private readonly IBarCodePersistenceOrchestrator _persistenceOrchestrator;
    private readonly IReferenceVariableService _referenceVariableService;
    private readonly IBarCodeResponseBuilder _responseBuilder;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<CreateBarCodeCommandHandlerRefactored> _logger;

    public CreateBarCodeCommandHandlerRefactored(
        IMachineValidator machineValidator,
        IProductLookupService productLookupService,
        IRuleExecutionService ruleExecutionService,
        IBarCodePersistenceOrchestrator persistenceOrchestrator,
        IReferenceVariableService referenceVariableService,
        IBarCodeResponseBuilder responseBuilder,
        IAuditLogger auditLogger,
        ILogger<CreateBarCodeCommandHandlerRefactored> logger)
    {
        _machineValidator = machineValidator ?? throw new ArgumentNullException(nameof(machineValidator));
        _productLookupService = productLookupService ?? throw new ArgumentNullException(nameof(productLookupService));
        _ruleExecutionService = ruleExecutionService ?? throw new ArgumentNullException(nameof(ruleExecutionService));
        _persistenceOrchestrator = persistenceOrchestrator ?? throw new ArgumentNullException(nameof(persistenceOrchestrator));
        _referenceVariableService = referenceVariableService ?? throw new ArgumentNullException(nameof(referenceVariableService));
        _responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));
        _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the create barcode command using orchestrated SRP services.
    /// Implements fluent Railway-oriented programming pipeline for clean error handling.
    /// </summary>
    /// <param name="cmd">The create barcode command containing the request details</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <returns>TaskGatewayResponse on success, or error information on failure</returns>
    public async Task<Result<TaskGatewayResponse>> ProcessAsync(CreateBarCodeCommand cmd, CancellationToken cancellationToken)
    {
        using var activity = Activity.Current?.Source.StartActivity("CreateBarCode.Process");
        var sw = Stopwatch.StartNew();

        // Validate inputs first
        var inputValidation = ValidateInputs(cmd, cancellationToken);
        if (inputValidation.IsFailure)
        {
            return Result<TaskGatewayResponse>.WithFailure(inputValidation.Errors);
        }

        var request = cmd.Command;

        // Add activity tags for observability
        activity?.SetTag("MachineId", request.MachineId);
        activity?.SetTag("PartNumber", request.PartNumber);

        _logger.LogInformation(LogEvents.Start, "Start CreateBarCode MachineId={MachineId} Part={Part}",
            request.MachineId, request.PartNumber);

        try
        {
            // Define a simple context object to avoid complex tuple chaining
            var context = new BarCodeCreationContext
            {
                MachineId = request.MachineId,
                PartNumber = request.PartNumber
            };

            // Sequential pipeline: Machine → Product → Rule → Persistence → References → Response
            var machineResult = await ValidateMachineStep(context, cancellationToken);
            if (machineResult.IsFailure || machineResult.Value is null)
            {
                await LogFailureAsync(machineResult.Errors, request, cancellationToken);
                return Result<TaskGatewayResponse>.WithFailure(machineResult.Errors);
            }

            var productResult = await ValidateProductStep(machineResult.Value, cancellationToken);
            if (productResult.IsFailure || productResult.Value is null)
            {
                await LogFailureAsync(productResult.Errors, request, cancellationToken);
                return Result<TaskGatewayResponse>.WithFailure(productResult.Errors);
            }

            var ruleResult = await ExecuteRuleStep(productResult.Value, cancellationToken);
            if (ruleResult.IsFailure || ruleResult.Value is null)
            {
                await LogFailureAsync(ruleResult.Errors, request, cancellationToken);
                return Result<TaskGatewayResponse>.WithFailure(ruleResult.Errors);
            }

            var persistResult = await PersistEntitiesStep(ruleResult.Value, cancellationToken);
            if (persistResult.IsFailure || persistResult.Value is null)
            {
                await LogFailureAsync(persistResult.Errors, request, cancellationToken);
                return Result<TaskGatewayResponse>.WithFailure(persistResult.Errors);
            }

            var referencesResult = await CollectReferencesStep(persistResult.Value, cancellationToken);
            if (referencesResult.IsFailure || referencesResult.Value is null)
            {
                await LogFailureAsync(referencesResult.Errors, request, cancellationToken);
                return Result<TaskGatewayResponse>.WithFailure(referencesResult.Errors);
            }

            var response = BuildResponseStep(referencesResult.Value);
            _logger.LogInformation(LogEvents.Success, "CreateBarCode success BarCode={BarCode}", response.BarCode);

            var result = Result<TaskGatewayResponse>.Success(response);

            // Record metrics and activity tags
            activity?.SetTag("Success", result.IsSuccess);
            activity?.SetTag("DurationMs", sw.ElapsedMilliseconds);
            RecordMetrics(sw, result.IsSuccess);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CreateBarCode pipeline for MachineId={MachineId}, PartNumber={PartNumber}",
                request.MachineId, request.PartNumber);
            var failureResult = Result<TaskGatewayResponse>.WithFailure([$"Unexpected error: {ex.Message}"]);

            // Record metrics and activity tags for exception case
            activity?.SetTag("Success", false);
            activity?.SetTag("DurationMs", sw.ElapsedMilliseconds);
            activity?.SetTag("Exception", ex.GetType().Name);
            RecordMetrics(sw, false);
            return failureResult;
        }
    }

    #region Pipeline Steps

    /// <summary>
    /// Step 1: Validate machine exists and is printer type
    /// </summary>
    private async Task<Result<BarCodeCreationContext>> ValidateMachineStep(BarCodeCreationContext context, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Pipeline Step 1: Validating machine {MachineId}", context.MachineId);

        var machineResult = await _machineValidator.ValidatePrinterMachineAsync(context.MachineId, cancellationToken);

        if (machineResult.IsFailure || machineResult.Value is null)
        {
            await _auditLogger.LogFailureAsync(context.MachineId, context.PartNumber, ResultValidation.InvalidMachine, cancellationToken);
            return Result<BarCodeCreationContext>.WithFailure(machineResult.Errors);
        }

        context.Machine = machineResult.Value;
        _logger.LogDebug("Machine validation successful: {MachineName} ({MachineType})",
            context.Machine.Name, context.Machine.MachineType);

        return Result<BarCodeCreationContext>.Success(context);
    }

    /// <summary>
    /// Step 2: Validate product exists by part number
    /// </summary>
    private async Task<Result<BarCodeCreationContext>> ValidateProductStep(BarCodeCreationContext context, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Pipeline Step 2: Looking up product {PartNumber}", context.PartNumber);

        var productResult = await _productLookupService.GetProductByPartNumberAsync(context.PartNumber, cancellationToken);

        if (productResult.IsFailure || productResult.Value is null)
        {
            await _auditLogger.LogFailureAsync(context.MachineId, context.PartNumber, ResultValidation.ProductNotFound, cancellationToken);
            return Result<BarCodeCreationContext>.WithFailure(productResult.Errors);
        }

        context.Product = productResult.Value;
        _logger.LogDebug("Product lookup successful: ProductId={ProductId}", context.Product.ProductId);

        return Result<BarCodeCreationContext>.Success(context);
    }

    /// <summary>
    /// Step 3: Execute rule to generate barcode label
    /// </summary>
    private async Task<Result<BarCodeCreationContext>> ExecuteRuleStep(BarCodeCreationContext context, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Pipeline Step 3: Executing rule for MachineId={MachineId}, ProductId={ProductId}",
            context.MachineId, context.Product!.ProductId);

        var labelResult = await _ruleExecutionService.GenerateBarCodeLabelAsync(
            context.MachineId, context.Product!.ProductId, context.PartNumber, cancellationToken);

        if (labelResult.IsFailure)
        {
            await _auditLogger.LogFailureAsync(context.MachineId, context.PartNumber, ResultValidation.RuleNotFound, cancellationToken);
            return Result<BarCodeCreationContext>.WithFailure(labelResult.Errors);
        }

        context.GeneratedLabel = labelResult.Value;
        _logger.LogDebug("Rule execution successful: Generated label={Label}", context.GeneratedLabel);

        return Result<BarCodeCreationContext>.Success(context);
    }

    /// <summary>
    /// Step 4: Persist BarCode and Cycle entities
    /// </summary>
    private async Task<Result<BarCodeCreationContext>> PersistEntitiesStep(BarCodeCreationContext context, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Pipeline Step 4: Persisting BarCode and Cycle for label={Label}", context.GeneratedLabel);

        var persistenceResult = await _persistenceOrchestrator.CreateBarCodeAndCycleAsync(
            context.GeneratedLabel!, context.Product!.ProductId, context.MachineId, cancellationToken);

        if (persistenceResult.IsFailure)
        {
            await _auditLogger.LogFailureAsync(context.MachineId, context.PartNumber, ResultValidation.Invalid, cancellationToken);
            return Result<BarCodeCreationContext>.WithFailure(persistenceResult.Errors);
        }

        var (barCode, cycle) = persistenceResult.Value;
        context.BarCode = barCode;
        context.Cycle = cycle;

        _logger.LogDebug("Persistence successful: BarCodeId={BarCodeId}, CycleId={CycleId}",
            barCode.BarCodeId, cycle.CycleId);

        return Result<BarCodeCreationContext>.Success(context);
    }

    /// <summary>
    /// Step 5: Collect reference variables
    /// </summary>
    private async Task<Result<BarCodeCreationContext>> CollectReferencesStep(BarCodeCreationContext context, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Pipeline Step 5: Collecting reference variables for MachineId={MachineId}, CycleId={CycleId}",
            context.MachineId, context.Cycle!.CycleId);

        var referencesResult = await _referenceVariableService.GetReferenceRegistersAsync(
            context.MachineId, context.Cycle!.CycleId, cancellationToken);

        if (referencesResult.IsFailure || referencesResult.Value is null)
        {
            await _auditLogger.LogFailureAsync(context.MachineId, context.PartNumber, ResultValidation.ReferencesNotFound, cancellationToken);
            return Result<BarCodeCreationContext>.WithFailure(referencesResult.Errors);
        }

        context.References = referencesResult.Value;
        _logger.LogDebug("Reference collection successful: {ReferenceCount} variables collected",
            context.References.Count);

        return Result<BarCodeCreationContext>.Success(context);
    }

    /// <summary>
    /// Step 6: Build final response
    /// </summary>
    private TaskGatewayResponse BuildResponseStep(BarCodeCreationContext context)
    {
        _logger.LogDebug("Pipeline Step 6: Building final response");

        return _responseBuilder.BuildResponse(
            context.BarCode!,
            context.Cycle!,
            context.Machine!,
            context.References!,
            context.PartNumber);
    }

    #endregion Pipeline Steps

    #region Helper Methods

    /// <summary>
    /// Validates command inputs before processing
    /// </summary>
    private Result ValidateInputs(CreateBarCodeCommand cmd, CancellationToken cancellationToken)
    {
        if (cmd is null)
        {
            return Result.WithFailure("cmd cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure("Operation was canceled.");
        }

        if (cmd.Command is null)
        {
            return Result.WithFailure("Command.Command cannot be null.");
        }

        return Result.Success();
    }

    /// <summary>
    /// Logs operation failure for debugging and monitoring
    /// </summary>
    private async Task LogFailureAsync(IEnumerable<string> errors, TaskGatewayRequest request, CancellationToken cancellationToken)
    {
        _logger.LogError(LogEvents.Failure, "CreateBarCode failed MachineId={MachineId} PartNumber={PartNumber} Errors={Errors}",
            request.MachineId, request.PartNumber, string.Join(", ", errors));

        // Additional failure telemetry could be added here
        await Task.CompletedTask;
    }

    #endregion Helper Methods

    #region Context Class

    /// <summary>
    /// Context object to pass data through the pipeline steps.
    /// Simplifies method signatures and improves readability vs complex tuples.
    /// </summary>
    private class BarCodeCreationContext
    {
        public int MachineId { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public Machine? Machine { get; set; }
        public Product? Product { get; set; }
        public string? GeneratedLabel { get; set; }
        public BarCode? BarCode { get; set; }
        public Cycle? Cycle { get; set; }
        public Dictionary<string, Register>? References { get; set; }
    }

    #endregion Context Class

    #region Metrics and Observability

    /// <summary>
    /// Records metrics for monitoring and alerting
    /// </summary>
    private void RecordMetrics(Stopwatch sw, bool isSuccess)
    {
        // Record performance metrics
        // TODO: Implement metrics collection when IMetrics abstraction is available
        _logger.LogDebug("CreateBarCode metrics: Duration={DurationMs}ms, Success={Success}",
            sw.ElapsedMilliseconds, isSuccess);
    }

    #endregion Metrics and Observability
}

/// <summary>
/// Structured logging event IDs following master plan observability standards
/// </summary>
public static class LogEvents
{
    public static readonly EventId Start = new(1000, "Start");
    public static readonly EventId Success = new(1001, "Success");
    public static readonly EventId Failure = new(1002, "Failure");
}
