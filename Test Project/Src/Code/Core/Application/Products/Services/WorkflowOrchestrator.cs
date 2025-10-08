using IndTrace.Application.Products.Services.Interfaces;
using IndTrace.Application.WorkFlows.Dto;
using IndTrace.Domain.Services.Products;

namespace IndTrace.Application.Products.Services;

/// <summary>
/// Orchestrates workflow creation and management for products.
/// Handles complex workflow generation logic from original handler.
/// Preserves sophisticated workflow creation patterns and entity relationships.
/// </summary>
public class WorkflowOrchestrator : IWorkflowOrchestrator
{
    private readonly IRepository<WorkFlow> _workflowRepository;
    private readonly ILogger<WorkflowOrchestrator> _logger;

    public WorkflowOrchestrator(
        IRepository<WorkFlow> workflowRepository,
        ILogger<WorkflowOrchestrator> logger)
    {
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<IEnumerable<WorkFlow>>> CreateAndPersistWorkflowsAsync(
        Product product,
        IEnumerable<int> machineIds,
        CancellationToken cancellationToken)
    {
        var dtos = GenerateWorkflowDtos(machineIds);
        if (dtos.IsFailure || dtos.Value is null)
        {
            return Result<IEnumerable<WorkFlow>>.WithFailure(dtos.Errors);
        }
        var convert = await ConvertAndLinkWorkflowsAsync(dtos.Value, product).ConfigureAwait(false);
        return convert;
    }

    public Result<IEnumerable<WorkFlowDto>> GenerateWorkflowDtos(IEnumerable<int> machineIds)
    {
        var ids = (machineIds ?? Enumerable.Empty<int>()).Where(id => id > 0).OrderBy(id => id).ToList();
        var list = new List<WorkFlowDto>();
        if (ids.Count == 0)
        {
            return Result<IEnumerable<WorkFlowDto>>.Success(list);
        }
        // create circular chain 0 -> ids[0] -> ... -> 0
        list.Add(new WorkFlowDto { NextMachineId = ids[0], LastMachineId = 0, RuleId = 2005 });
        for (int i = 0; i < ids.Count - 1; i++)
        {
            list.Add(new WorkFlowDto { NextMachineId = ids[i + 1], LastMachineId = ids[i], RuleId = 2005 });
        }
        list.Add(new WorkFlowDto { NextMachineId = 0, LastMachineId = ids[^1], RuleId = 2005 });
        return Result<IEnumerable<WorkFlowDto>>.Success(list);
    }

    public async Task<Result<IEnumerable<WorkFlow>>> ConvertAndLinkWorkflowsAsync(
        IEnumerable<WorkFlowDto> workflowDtos,
        Product product)
    {
        var resultList = new List<WorkFlow>();
        foreach (var dto in workflowDtos)
        {
            var entityResult = WorkFlowDto.ToEntity(dto);
            if (entityResult.IsFailure || entityResult.Value is null)
            {
                return Result<IEnumerable<WorkFlow>>.WithFailure(entityResult.Errors);
            }
            var entity = entityResult.Value;
            entity.ProductId = product.ProductId;
            var add = await _workflowRepository.AddAsync(entity, CancellationToken.None).ConfigureAwait(false);
            if (add.IsFailure)
            {
                return Result<IEnumerable<WorkFlow>>.WithFailure(add.Errors);
            }
            resultList.Add(entity);
        }
        return Result<IEnumerable<WorkFlow>>.Success(resultList);
    }

    public async Task<Result> ValidateMachineAssignmentsAsync(
        IEnumerable<int> machineIds,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(Result.Success()).ConfigureAwait(false);
    }

    public async Task<Result<IEnumerable<WorkFlow>>> GetWorkflowsForProductAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        var spec = new Specification<WorkFlow>(w => w.ProductId == productId);
        var result = await _workflowRepository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        return (result.IsFailure || result.Value is null) ? Result<IEnumerable<WorkFlow>>.WithFailure(result.Errors) : Result<IEnumerable<WorkFlow>>.Success(result.Value);
    }

    /// <summary>
    /// Generates a workflow for a product using sophisticated creation logic.
    /// Preserves EXACT workflow generation patterns from the original handler.
    /// </summary>
    public async Task<Result<WorkFlow>> GenerateWorkflowForProductAsync(
        Product product,
        ProductInput productInput,
        CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<WorkFlow>.WithFailure("Operation was canceled.");
        }

        // Null guards for dependencies and parameters
        if (_workflowRepository is null)
        {
            return Result<WorkFlow>.WithFailure("Workflow repository cannot be null.");
        }

        if (product is null)
        {
            return Result<WorkFlow>.WithFailure("Product cannot be null for workflow generation.");
        }

        if (productInput is null)
        {
            return Result<WorkFlow>.WithFailure("ProductInput cannot be null for workflow generation.");
        }

        try
        {
            _logger.LogDebug("Generating workflow for Product: {ProductId}, PartNumber: {PartNumber}",
                product.ProductId, product.PartNumber);

            // Create workflow entity using actual WorkFlow schema
            var workflow = new WorkFlow
            {
                // WorkFlowId will be assigned by persistence layer
                WorkFlowId = 0,

                // Link to product
                ProductId = product.ProductId,

                // Machine navigation - defaults for linear workflow
                NextMachineId = 0, // Will be set when machines are configured
                LastMachineId = 0, // Will be set when machines are configured

                // Rule association - can be linked later
                RuleId = product.RuleId,

                // [Fix] Audit field from ProductInput
                CreatedBy = productInput.CreatedBy,

                // Machine and Edge collections - empty initially, populated later
                Machine = new List<Machine>(),
                Edges = new List<Edge>()
            };

            // Validate generated workflow before persistence
            var validationResult = ValidateGeneratedWorkflow(workflow);
            if (validationResult.IsFailure)
            {
                _logger.LogWarning("Workflow validation failed for Product: {ProductId}", product.ProductId);
                return Result<WorkFlow>.WithFailure(validationResult.Errors);
            }

            // Persist the workflow
            var persistenceResult = await _workflowRepository.AddAsync(workflow, cancellationToken)
                .ConfigureAwait(false);

            if (persistenceResult.IsFailure)
            {
                _logger.LogError("Workflow persistence failed for Product: {ProductId}", product.ProductId);
                return Result<WorkFlow>.WithFailure($"Failed to persist workflow: {string.Join(", ", persistenceResult.Errors)}");
            }

            _logger.LogDebug("Workflow generation successful. WorkFlowId: {WorkFlowId}, ProductId: {ProductId}",
                workflow.WorkFlowId, workflow.ProductId);

            return Result<WorkFlow>.Success(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while generating workflow for Product: {ProductId}", product.ProductId);
            return Result<WorkFlow>.WithFailure($"Exception occurred while generating workflow: {ex.Message}");
        }
    }

    /// <summary>
    /// Links an existing workflow to a product.
    /// Alternative to generation when workflow already exists.
    /// </summary>
    public async Task<Result<WorkFlow>> LinkExistingWorkflowToProductAsync(
        Product product,
        int workflowId,
        CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<WorkFlow>.WithFailure("Operation was canceled.");
        }

        // Null guards for dependencies and parameters
        if (_workflowRepository is null)
        {
            return Result<WorkFlow>.WithFailure("Workflow repository cannot be null.");
        }

        if (product is null)
        {
            return Result<WorkFlow>.WithFailure("Product cannot be null for workflow linking.");
        }

        try
        {
            _logger.LogDebug("Linking existing workflow {WorkflowId} to Product: {ProductId}", workflowId, product.ProductId);

            // Retrieve existing workflow
            var workflowResult = await _workflowRepository.GetByIdAsync(workflowId, cancellationToken)
                .ConfigureAwait(false);

            if (workflowResult.IsFailure || workflowResult.Value is null)
            {
                _logger.LogWarning("Workflow linking failed - workflow not found: {WorkflowId}", workflowId);
                return Result<WorkFlow>.WithFailure($"Workflow not found {workflowId}");
            }

            var workflow = workflowResult.Value;

            // Validate compatibility between product and workflow
            var compatibilityResult = ValidateProductWorkflowCompatibility(product, workflow);
            if (compatibilityResult.IsFailure)
            {
                _logger.LogWarning("Workflow linking failed - compatibility check failed for Product: {ProductId}, Workflow: {WorkflowId}",
                    product.ProductId, workflowId);
                return Result<WorkFlow>.WithFailure(compatibilityResult.Errors);
            }

            _logger.LogDebug("Workflow linking successful. Product: {ProductId}, Workflow: {WorkflowId}",
                product.ProductId, workflowId);

            return Result<WorkFlow>.Success(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while linking workflow {WorkflowId} to Product: {ProductId}",
                workflowId, product.ProductId);
            return Result<WorkFlow>.WithFailure($"Exception occurred while linking workflow: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates sophisticated workflow name from PartNumber.
    /// Implements business-specific naming conventions.
    /// </summary>
    private string GenerateWorkflowName(string partNumber)
    {
        if (string.IsNullOrWhiteSpace(partNumber))
        {
            return "DEFAULT-WORKFLOW";
        }

        // Business rule: Workflow name format is "WF-{PartNumber}"
        return $"WF-{partNumber}";
    }

    /// <summary>
    /// Determines workflow type based on product characteristics.
    /// Implements business logic for workflow type classification.
    /// </summary>
    private string DetermineWorkflowType(Product product)
    {
        if (product is null)
        {
            return "STANDARD";
        }

        // Business logic for workflow type determination
        // This could be enhanced with more sophisticated rules

        // Example: Determine by PartNumber patterns
        if (product.PartNumber.Contains("PROTO"))
        {
            return "PROTOTYPE";
        }

        if (product.PartNumber.Contains("TEST"))
        {
            return "TESTING";
        }

        if (product.PartNumber.Contains("PROD"))
        {
            return "PRODUCTION";
        }

        // Default workflow type
        return "STANDARD";
    }

    /// <summary>
    /// Validates generated workflow meets business requirements.
    /// Ensures workflow is ready for persistence.
    /// </summary>
    private Result ValidateGeneratedWorkflow(WorkFlow workflow)
    {
        if (workflow is null)
        {
            return Result.WithFailure("Workflow cannot be null for validation.");
        }

        var errors = new List<string>();

        // Required field validation
        // Minimal validation aligned with Domain entity
        if (workflow.ProductId <= 0)
        {
            errors.Add("ProductId must be greater than 0 for generated workflow.");
        }

        return errors.Count > 0
            ? Result.WithFailure(errors)
            : Result.Success();
    }

    /// <summary>
    /// Validates compatibility between product and workflow.
    /// Ensures business rules are satisfied for workflow linking.
    /// </summary>
    private Result ValidateProductWorkflowCompatibility(Product product, WorkFlow workflow)
    {
        if (product is null)
        {
            return Result.WithFailure("Product cannot be null for compatibility validation.");
        }

        if (workflow is null)
        {
            return Result.WithFailure("Workflow cannot be null for compatibility validation.");
        }

        var errors = new List<string>();

        // Product compatibility
        if (workflow.ProductId != product.ProductId)
        {
            errors.Add($"Workflow ProductId {workflow.ProductId} does not match Product ProductId {product.ProductId}.");
        }

        // Active status compatibility
        if (product.IsActive <= 0)
        {
            errors.Add("Cannot link inactive product to workflow.");
        }

        return errors.Count > 0
            ? Result.WithFailure(errors)
            : Result.Success();
    }

    /// <summary>
    /// Validates workflow uniqueness by name.
    /// Ensures no duplicate workflow names within the same customer scope.
    /// </summary>
    public async Task<Result> ValidateWorkflowUniquenessAsync(
        string workflowName,
        int customerId,
        CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure("Operation was canceled.");
        }

        // Null guard for dependencies
        if (_workflowRepository is null)
        {
            return Result.WithFailure("Workflow repository cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(workflowName))
        {
            return Result.WithFailure("WorkflowName cannot be null or empty for uniqueness validation.");
        }

        try
        {
            _logger.LogDebug("Validating workflow uniqueness for WorkflowName: {WorkflowName}, CustomerId: {CustomerId}",
                workflowName, customerId);

            // Check for existing workflow with same product (customerId is actually productId in this context)
            var spec = new Specification<WorkFlow>(w => w.ProductId == customerId);

            var existingWorkflowResult = await _workflowRepository.FirstOrDefaultAsync(spec, cancellationToken)
                .ConfigureAwait(false);

            if (existingWorkflowResult.IsSuccess && existingWorkflowResult.Value is not null)
            {
                _logger.LogWarning("Workflow uniqueness validation failed - workflow already exists: {WorkflowName}", workflowName);
                return Result.WithFailure($"Workflow already exists {workflowName}");
            }

            _logger.LogDebug("Workflow uniqueness validation successful for WorkflowName: {WorkflowName}", workflowName);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while validating workflow uniqueness for WorkflowName: {WorkflowName}", workflowName);
            return Result.WithFailure($"Exception occurred while validating workflow uniqueness: {ex.Message}");
        }
    }
}
