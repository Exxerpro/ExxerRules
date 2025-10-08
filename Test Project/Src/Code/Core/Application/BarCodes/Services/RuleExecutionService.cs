using IndTrace.Application.BarCodes.Services.Interfaces;

namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Manages rule lookup and execution for barcode generation.
/// Handles complex rule parsing and barcode label creation using the rule engine.
/// </summary>
public class RuleExecutionService : IRuleExecutionService
{
    private readonly IReadOnlyRepository<Rule> _ruleRepository;
    private readonly IMasterLabelService _masterLabelService;
    private readonly IBarCodeService _barCodeService;
    private readonly IDateTimeMachine _dateTimeMachine;
    private readonly ILogger<RuleExecutionService> _logger;

    public RuleExecutionService(
        IReadOnlyRepository<Rule> ruleRepository,
        IMasterLabelService masterLabelService,
        IBarCodeService barCodeService,
        IDateTimeMachine dateTimeMachine,
        ILogger<RuleExecutionService> logger)
    {
        _ruleRepository = ruleRepository ?? throw new ArgumentNullException(nameof(ruleRepository));
        _masterLabelService = masterLabelService ?? throw new ArgumentNullException(nameof(masterLabelService));
        _barCodeService = barCodeService ?? throw new ArgumentNullException(nameof(barCodeService));
        _dateTimeMachine = dateTimeMachine ?? throw new ArgumentNullException(nameof(dateTimeMachine));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a barcode label using the appropriate rule for the machine and product combination.
    /// Finds the latest active rule, parses the rule JSON, and executes the barcode creation logic.
    /// </summary>
    /// <param name="machineId">The machine identifier</param>
    /// <param name="productId">The product identifier</param>
    /// <param name="partNumber">The part number for the product</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Success with generated barcode label string, Failure if rule not found or execution fails</returns>
    public async Task<Result<string>> GenerateBarCodeLabelAsync(int machineId, int productId, string partNumber, CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<string>.WithFailure(["Operation was canceled."]);
        }

        // Null guards for dependencies
        if (_ruleRepository is null)
        {
            return Result<string>.WithFailure(["_ruleRepository cannot be null."]);
        }

        if (string.IsNullOrWhiteSpace(partNumber))
        {
            return Result<string>.WithFailure(["Part number cannot be null or empty."]);
        }

        try
        {
            _logger.LogDebug("Starting barcode label generation for MachineId={MachineId}, ProductId={ProductId}, PartNumber={PartNumber}",
                machineId, productId, partNumber);

            // Step 1: Find the latest active rule for the machine-product combination
            var ruleResult = await GetLatestActiveRuleAsync(machineId, productId, cancellationToken);
            if (ruleResult.IsFailure || ruleResult.Value is null)
            {
                return Result<string>.WithFailure(ruleResult.Errors);
            }

            var rule = ruleResult.Value;
            _logger.LogDebug("Found rule RuleId={RuleId}, Version={Version}", rule.RuleId, rule.Version);

            // Step 2: Get master labels for the part number
            var masterLabelResult = await GetMasterLabelsAsync(partNumber, cancellationToken);
            if (masterLabelResult.IsFailure || masterLabelResult.Value is null)
            {
                return Result<string>.WithFailure(masterLabelResult.Errors);
            }

            var masterLabels = masterLabelResult.Value;
            _logger.LogDebug("Retrieved {Count} master labels for PartNumber={PartNumber}", masterLabels.Count, partNumber);

            // Step 3: Get consecutive number for barcode generation
            var consecutiveResult = await GetConsecutiveNumberAsync(partNumber, masterLabels, cancellationToken);
            if (consecutiveResult.IsFailure)
            {
                return Result<string>.WithFailure(consecutiveResult.Errors);
            }

            var consecutiveNumber = consecutiveResult.Value;
            _logger.LogDebug("Got consecutive number {ConsecutiveNumber} for PartNumber={PartNumber}", consecutiveNumber, partNumber);

            // Step 4: Execute rule to generate barcode label
            var labelResult = ExecuteRuleForBarCodeLabel(rule, partNumber, consecutiveNumber);
            if (labelResult.IsFailure || labelResult.Value is null)
            {
                return Result<string>.WithFailure(labelResult.Errors);
            }

            var label = labelResult.Value;
            _logger.LogInformation("Successfully generated barcode label: {Label} for PartNumber={PartNumber}", label, partNumber);

            return Result<string>.Success(label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Barcode label generation error for MachineId={MachineId}, ProductId={ProductId}, PartNumber={PartNumber}",
                machineId, productId, partNumber);
            return Result<string>.WithFailure([$"Rule execution failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Retrieves the latest active rule for the machine-product combination.
    /// Business rule: Latest version rule takes precedence (OrderByDescending).
    /// </summary>
    private async Task<Result<Rule>> GetLatestActiveRuleAsync(int machineId, int productId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Looking up rule for MachineId={MachineId}, ProductId={ProductId}", machineId, productId);

        var specification = new Specification<Rule>(r =>
            r.MachineId == machineId &&
            r.ProductId == productId &&
            r.IsActive);

        // Critical business rule: Latest version takes precedence
        specification.AddOrderByDescending(r => r.Version);

        var ruleResult = await _ruleRepository.FirstOrDefaultAsync(specification, cancellationToken);

        if (ruleResult.IsFailure || ruleResult.Value is null)
        {
            _logger.LogWarning("Rule not found for MachineId={MachineId}, ProductId={ProductId}", machineId, productId);
            return Result<Rule>.WithFailure([$"Rule for Machine {machineId} does not exist"]);
        }

        return Result<Rule>.Success(ruleResult.Value);
    }

    /// <summary>
    /// Retrieves master labels for the given part number.
    /// </summary>
    private async Task<Result<List<string>>> GetMasterLabelsAsync(string partNumber, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting master labels for PartNumber={PartNumber}", partNumber);

        var masterLabelResult = await _masterLabelService.GetMasterLabelByPartNumberAsync(partNumber, cancellationToken);

        if (masterLabelResult is null)
        {
            _logger.LogWarning("Master label result is null for PartNumber={PartNumber}", partNumber);
            return Result<List<string>>.Success(new List<string>());
        }

        var masterLabels = masterLabelResult.IsSuccess ? (masterLabelResult.Value ?? new List<string>()) : new List<string>();
        return Result<List<string>>.Success(masterLabels);
    }

    /// <summary>
    /// Gets the consecutive number for barcode generation.
    /// </summary>
    private async Task<Result<int>> GetConsecutiveNumberAsync(string partNumber, List<string> masterLabels, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting consecutive number for PartNumber={PartNumber}", partNumber);

        var consecutiveResult = await _barCodeService.GetConsecutiveByBarCodeLabelAsync(partNumber, masterLabels, cancellationToken);

        if (consecutiveResult.IsFailure)
        {
            _logger.LogError("Failed to get consecutive number for PartNumber={PartNumber}", partNumber);
            return Result<int>.WithFailure(["Failed to get consecutive number"]);
        }

        return Result<int>.Success(consecutiveResult.Value);
    }

    /// <summary>
    /// Executes the rule to generate the barcode label.
    /// Uses the existing CreateBarCodeDictionaryExecutor rule engine.
    /// </summary>
    private Result<string> ExecuteRuleForBarCodeLabel(Rule rule, string partNumber, int consecutiveNumber)
    {
        try
        {
            _logger.LogDebug("Creating rule executor for RuleId={RuleId}", rule.RuleId);

            var ruleExecutor = new CreateBarCodeDictionaryExecutor(_dateTimeMachine);

            // Parse rule JSON
            ruleExecutor.ParseRuleFromJson(rule.RuleJson ?? string.Empty);

            // Initialize component actions
            ruleExecutor.InitializeComponentActions();

            // Apply rule to create barcode label
            _logger.LogDebug("Applying rule to create barcode label for PartNumber={PartNumber}, Consecutive={Consecutive}",
                partNumber, consecutiveNumber);

            var labelResult = ruleExecutor.ApplyRuleCreateBarCode(partNumber, consecutiveNumber);

            if (labelResult.IsFailure)
            {
                _logger.LogError("Rule application failed with errors: {Errors}", string.Join(", ", labelResult.Errors ?? []));
                return Result<string>.WithFailure(labelResult.Errors);
            }

            if (string.IsNullOrEmpty(labelResult.Value))
            {
                _logger.LogError("Rule application returned null or empty label");
                return Result<string>.WithFailure(["Label result value cannot be null or empty"]);
            }

            return Result<string>.Success(labelResult.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rule execution error for RuleId={RuleId}", rule.RuleId);
            return Result<string>.WithFailure([$"Rule execution failed: {ex.Message}"]);
        }
    }
}
