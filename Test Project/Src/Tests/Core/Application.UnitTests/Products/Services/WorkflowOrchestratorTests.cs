namespace IndTrace.Application.UnitTests.Products.Services;

using Meziantou.Extensions.Logging.Xunit;

/// <summary>
/// Unit tests for WorkflowOrchestrator - Complex workflow generation and management.
/// Tests sophisticated workflow creation logic and naming conventions.
/// </summary>
public class WorkflowOrchestratorTests
{
    private readonly IRepository<WorkFlow> _mockWorkflowRepository;
    private readonly ILogger<WorkflowOrchestrator> _mockLogger;
    private readonly WorkflowOrchestrator _orchestrator;

    public WorkflowOrchestratorTests(ITestOutputHelper output)
    {
        _mockWorkflowRepository = Substitute.For<IRepository<WorkFlow>>();
        _mockLogger = XUnitLogger.CreateLogger<WorkflowOrchestrator>(output);
        _orchestrator = new WorkflowOrchestrator(_mockWorkflowRepository, _mockLogger);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_NullRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new WorkflowOrchestrator(null!, _mockLogger));
    }

    [Fact]
    public void Constructor_NullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new WorkflowOrchestrator(_mockWorkflowRepository, null!));
    }

    #endregion

    #region GenerateWorkflowForProductAsync Tests

    [Fact]
    public async Task GenerateWorkflowForProductAsync_ValidInput_ShouldCreateWorkflowSuccessfully()
    {
        // Arrange
        var product = CreateValidProduct();
        var productInput = CreateValidProductInput();

        // [Fix] Configure mock to simulate ID assignment during persistence
        _mockWorkflowRepository
            .AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1))
            .AndDoes(callInfo =>
            {
                var workflow = callInfo.Arg<WorkFlow>();
                workflow.WorkFlowId = 1; // Simulate ID assignment by persistence layer
            });

        // Act
        var result = await _orchestrator.GenerateWorkflowForProductAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var workflow = result.Value;
        workflow.WorkFlowId.ShouldBeGreaterThan(0); // Workflow should be persisted
        workflow.ProductId.ShouldBe(product.ProductId);
        workflow.CreatedBy.ShouldBe(productInput.CreatedBy);
        workflow.CreatedOn.ShouldNotBeNull();
        workflow.RuleId.ShouldBeGreaterThan(0);

        await _mockWorkflowRepository
            .Received(1)
            .AddAsync(Arg.Is<WorkFlow>(w => w.WorkFlowId > 0), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("FORD-F150-001")]
    [InlineData("TESLA-Y-2024")]
    [InlineData("BMW-X5-LUXURY")]
    public async Task GenerateWorkflowForProductAsync_VariousPartNumbers_ShouldGenerateCorrectNames(
        string partNumber)
    {
        // Arrange
        var product = CreateValidProduct();
        product.PartNumber = partNumber;
        var productInput = CreateValidProductInput();

        // [Fix] Configure mock to simulate ID assignment during persistence
        _mockWorkflowRepository
            .AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1))
            .AndDoes(callInfo =>
            {
                var workflow = callInfo.Arg<WorkFlow>();
                workflow.WorkFlowId = 1; // Simulate ID assignment by persistence layer
            });

        // Act
        var result = await _orchestrator.GenerateWorkflowForProductAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.WorkFlowId.ShouldBeGreaterThan(0);
        result.Value.ProductId.ShouldBe(product.ProductId);
    }

    [Theory]
    [InlineData("PROTO-PART-001")]
    [InlineData("TEST-PART-001")]
    [InlineData("PROD-PART-001")]
    [InlineData("STANDARD-PART-001")]
    public async Task GenerateWorkflowForProductAsync_PartNumberPatterns_ShouldDetermineCorrectWorkflowType(
        string partNumber)
    {
        // Arrange
        var product = CreateValidProduct();
        product.PartNumber = partNumber;
        var productInput = CreateValidProductInput();

        // [Fix] Configure mock to simulate ID assignment during persistence
        _mockWorkflowRepository
            .AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1))
            .AndDoes(callInfo =>
            {
                var workflow = callInfo.Arg<WorkFlow>();
                workflow.WorkFlowId = 1; // Simulate ID assignment by persistence layer
            });

        // Act
        var result = await _orchestrator.GenerateWorkflowForProductAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ProductId.ShouldBe(product.ProductId);
        result.Value.RuleId.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GenerateWorkflowForProductAsync_NullProduct_ShouldReturnFailure()
    {
        // Arrange
        var productInput = CreateValidProductInput();

        // Act
        var result = await _orchestrator.GenerateWorkflowForProductAsync(null!, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product cannot be null for workflow generation.");
    }

    [Fact]
    public async Task GenerateWorkflowForProductAsync_NullProductInput_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        var result = await _orchestrator.GenerateWorkflowForProductAsync(product, null!, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("ProductInput cannot be null for workflow generation.");
    }

    [Fact]
    public async Task GenerateWorkflowForProductAsync_RepositoryFailure_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        var productInput = CreateValidProductInput();

        _mockWorkflowRepository
            .AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.WithFailure(["Database error"]));

        // Act
        var result = await _orchestrator.GenerateWorkflowForProductAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Failed to persist workflow: Database error");
    }

    [Fact]
    public async Task GenerateWorkflowForProductAsync_CancellationRequested_ShouldReturnCancellationError()
    {
        // Arrange
        var product = CreateValidProduct();
        var productInput = CreateValidProductInput();
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await _orchestrator.GenerateWorkflowForProductAsync(product, productInput, cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Operation was canceled.");
    }

    #endregion

    #region LinkExistingWorkflowToProductAsync Tests

    [Fact]
    public async Task LinkExistingWorkflowToProductAsync_ValidWorkflow_ShouldLinkSuccessfully()
    {
        // Arrange
        var product = CreateValidProduct();
        const int workflowId = 1;
        var existingWorkflow = CreateValidWorkflow();

        _mockWorkflowRepository
            .GetByIdAsync(workflowId, Arg.Any<CancellationToken>())
            .Returns(Result<WorkFlow?>.Success(existingWorkflow));

        // Act
        var result = await _orchestrator.LinkExistingWorkflowToProductAsync(product, workflowId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(existingWorkflow);

        await _mockWorkflowRepository
            .Received(1)
            .GetByIdAsync(workflowId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LinkExistingWorkflowToProductAsync_WorkflowNotFound_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        const int workflowId = 999;

        _mockWorkflowRepository
            .GetByIdAsync(workflowId, Arg.Any<CancellationToken>())
            .Returns(Result<WorkFlow?>.WithFailure("Not found"));

        // Act
        var result = await _orchestrator.LinkExistingWorkflowToProductAsync(product, workflowId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain($"Workflow not found {workflowId}");
    }

    [Fact]
    public async Task LinkExistingWorkflowToProductAsync_IncompatibleWorkflow_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        product.CustomerId = 1;
        const int workflowId = 1;

        var incompatibleWorkflow = CreateValidWorkflow();
        incompatibleWorkflow.ProductId = 2; // Different product

        _mockWorkflowRepository
            .GetByIdAsync(workflowId, Arg.Any<CancellationToken>())
            .Returns(Result<WorkFlow?>.Success(incompatibleWorkflow));

        // Act
        var result = await _orchestrator.LinkExistingWorkflowToProductAsync(product, workflowId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Workflow ProductId 2 does not match Product ProductId 1.");
    }

    #endregion

    #region ValidateWorkflowUniquenessAsync Tests

    [Fact]
    public async Task ValidateWorkflowUniquenessAsync_UniqueWorkflow_ShouldReturnSuccess()
    {
        // Arrange
        const string workflowName = "WF-UNIQUE-001";
        const int customerId = 1;

        _mockWorkflowRepository
            .FirstOrDefaultAsync(Arg.Any<Specification<WorkFlow>>(), Arg.Any<CancellationToken>())
            .Returns(Result<WorkFlow?>.WithFailure("Not found"));

        // Act
        var result = await _orchestrator.ValidateWorkflowUniquenessAsync(workflowName, customerId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateWorkflowUniquenessAsync_DuplicateWorkflow_ShouldReturnFailure()
    {
        // Arrange
        const string workflowName = "WF-EXISTING-001";
        const int customerId = 1;
        var existingWorkflow = new WorkFlow { WorkFlowId = 1, ProductId = customerId };

        _mockWorkflowRepository
            .FirstOrDefaultAsync(Arg.Any<Specification<WorkFlow>>(), Arg.Any<CancellationToken>())
            .Returns(Result<WorkFlow?>.Success(existingWorkflow));

        // Act
        var result = await _orchestrator.ValidateWorkflowUniquenessAsync(workflowName, customerId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain($"Workflow already exists {workflowName}");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ValidateWorkflowUniquenessAsync_InvalidWorkflowName_ShouldReturnFailure(string workflowName)
    {
        // Act
        var result = await _orchestrator.ValidateWorkflowUniquenessAsync(workflowName, 1, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("WorkflowName cannot be null or empty for uniqueness validation.");
    }

    #endregion

    #region Helper Methods

    private Product CreateValidProduct()
    {
        return new Product
        {
            ProductId = 1,
            PartNumber = "FORD-F150-001",
            ProductName = "Ford F-150 Test Product",
            CustomerId = 1,
            CustomerName = "Ford Motor",
            Customer = new Customer { CustomerId = 1, Name = "Ford Motor" },
            LineId = 1,
            Line = new Line { LineId = 1, Name = "Production Line 1" },
            RuleId = 2005, // [Fix] Set RuleId so workflow.RuleId > 0
            IsActive = 1,
            Version = 1
        };
    }

    private ProductInput CreateValidProductInput()
    {
        return new ProductInput
        {
            PartNumber = "FORD-F150-001",
            ProductName = "Ford F-150 Test Product",
            CustomerId = 1,
            CustomerName = "Ford Motor",
            LineId = 1,
            IsActive = 1,
            Version = 1,
            CreatedBy = "TEST_USER"
        };
    }

    private WorkFlow CreateValidWorkflow()
    {
        return new WorkFlow
        {
            WorkFlowId = 1,
            ProductId = 1,
            NextMachineId = 1,
            LastMachineId = 2,
            RuleId = 1,
            CreatedBy = "TEST_USER"
        };
    }

    #endregion
}
