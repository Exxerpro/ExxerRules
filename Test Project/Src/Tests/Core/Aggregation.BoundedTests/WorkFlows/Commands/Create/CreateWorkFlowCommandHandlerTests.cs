namespace IndTrace.Aggregation.BoundedTests.WorkFlows.Commands.Create;

//[Fix]
//CLAUDE
//Date: 27/08/2025
//Reason: [Pattern 1] - Added ITestOutputHelper parameter to constructor and passed to base class
//[Fix]
//CLAUDE
//Date: 27/08/2025
//Reason: Refactored to use real repositories from DependenciesFactory instead of NSubstitute mocks

/// <summary>
/// Unit tests for the CreateWorkFlowCommandHandler using real repositories.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
public class CreateWorkFlowCommandHandlerTests(ITestOutputHelper outputHelper) : DependenciesFactory(outputHelper)
{
    /// <summary>
    /// Creates a handler instance with real dependencies.
    /// </summary>
    private async Task<CreateWorkFlowCommandHandler> CreateHandlerAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        if (cancellationToken.IsCancellationRequested)
        {
        }

        var logger = XUnitLogger.CreateLogger<CreateWorkFlowCommandHandler>();
        return new CreateWorkFlowCommandHandler(DpWorkFlowRepository, logger);
    }

    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public async Task Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        await Task.CompletedTask;

        var logger = XUnitLogger.CreateLogger<CreateWorkFlowCommandHandler>();

        // Act
        var handler = new CreateWorkFlowCommandHandler(DpWorkFlowRepository, logger);

        // Assert
        handler.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Process_WithValidCommand_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidCommand_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        //Create handler with real dependencies

        var handler = await CreateHandlerAsync(TestContext.Current.CancellationToken);

        // Ensure we have valid related data
        var product = await DpRoProductRepository.FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        product.IsSuccess.ShouldBeTrue();

        var machines = await DpRoMachineRepository.ListAsync(cancellationToken: TestContext.Current.CancellationToken);
        machines.IsSuccess.ShouldBeTrue();
        machines.Value.ShouldNotBeNull();
        machines.Value.Count().ShouldBeGreaterThan(1);

        var command = new CreateWorkFlowCommand
        {
            WorkFlowId = 0, // Will be auto-generated
            ProductId = product.Value!.ProductId,
            LastMachineId = machines.Value.ElementAt(0).MachineId,
            NextMachineId = machines.Value.ElementAt(1).MachineId
        };

        // Act
        var result = await handler.ProcessAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<WorkFlowCreatedEvent>();
        result.Value.ProductId.ShouldBe(command.ProductId);
        result.Value.LastMachineId.ShouldBe(command.NextMachineId); // Note: These are swapped in handler logic
        result.Value.NextMachineId.ShouldBe(command.LastMachineId); // Note: These are swapped in handler logic

        // Verify the workflow was created in the database
        var spec = new Specification<WorkFlow>(wf => wf.ProductId == command.ProductId);
        var createdWorkFlow = await DpRoWorkFlowRepository.FirstOrDefaultAsync(spec, TestContext.Current.CancellationToken);
        createdWorkFlow.IsSuccess.ShouldBeTrue();
        createdWorkFlow.Value.ShouldNotBeNull();
    }

    /// <summary>
    /// Tests creating workflow with invalid product ID.
    /// </summary>
    /// <returns>The result of the test.</returns>

    [Fact]
    public async Task Process_WithInvalidProductId_ShouldStillSucceed()
    {
        // Arrange

        var handler = await CreateHandlerAsync(TestContext.Current.CancellationToken);

        var command = new CreateWorkFlowCommand
        {
            WorkFlowId = 0,
            ProductId = 508, // Non-existent product
            LastMachineId = 100,
            NextMachineId = 200
        };

        // Act
        var result = await handler.ProcessAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - The handler doesn't validate product existence, so it should succeed
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ProductId.ShouldBe(command.ProductId);
    }

    /// <summary>
    /// Executes Process_ShouldSwapLastAndNextMachineIds operation.
    /// </summary>
    /// <returns>The result of Process_ShouldSwapLastAndNextMachineIds.</returns>

    [Fact]
    public async Task Process_ShouldSwapLastAndNextMachineIds()
    {
        // Arrange
        var handler = await CreateHandlerAsync(TestContext.Current.CancellationToken);

        var request = new CreateWorkFlowCommand
        {
            WorkFlowId = 0,
            ProductId = 508,
            LastMachineId = 500,
            NextMachineId = 600
        };

        // Act
        var result = await handler.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify the IDs are swapped in the result
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.LastMachineId.ShouldBe(600); // Original NextMachineId
        result.Value.NextMachineId.ShouldBe(500); // Original LastMachineId
    }

    /// <summary>
    /// Executes Process_WithMinimalData_ShouldStillSucceed operation.
    /// </summary>
    /// <returns>The result of Process_WithMinimalData_ShouldStillSucceed.</returns>

    [Fact]
    public async Task Process_WithMinimalData_ShouldStillSucceed()
    {
        // Arrange
        var handler = await CreateHandlerAsync(TestContext.Current.CancellationToken);

        var request = new CreateWorkFlowCommand
        {
            ProductId = 1,
            LastMachineId = 0,
            NextMachineId = 0
        };

        // Act
        var result = await handler.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ProductId.ShouldBe(1);
        result.Value.LastMachineId.ShouldBe(0);
        result.Value.NextMachineId.ShouldBe(0);
    }

    /// <summary>
    /// Tests creating multiple workflows in sequence.
    /// </summary>
    /// <returns>The result of the test.</returns>

    [Fact]
    public async Task Process_MultipleWorkflows_ShouldCreateSuccessfully()
    {
        // Arrange
        var handler = await CreateHandlerAsync(TestContext.Current.CancellationToken);

        var products = await DpRoProductRepository.ListAsync(cancellationToken: TestContext.Current.CancellationToken);
        products.IsSuccess.ShouldBeTrue();
        var productIds = products.Value!.Take(3).Select(p => p.ProductId).ToList();

        var results = new List<Result<WorkFlowCreatedEvent>>();

        // Act - Create multiple workflows
        foreach (var productId in productIds)
        {
            var command = new CreateWorkFlowCommand
            {
                ProductId = productId,
                LastMachineId = 100,
                NextMachineId = 200
            };

            var result = await handler.ProcessAsync(command, cancellationToken: TestContext.Current.CancellationToken);
            results.Add(result);
        }

        // Assert - All should succeed
        results.ShouldAllBe(r => r.IsSuccess);
        results.Select(r => r.Value!.ProductId).ShouldBe(productIds);

        // Verify all were created in database
        foreach (var productId in productIds)
        {
            var spec = new Specification<WorkFlow>(wf => wf.ProductId == productId && wf.LastMachineId == 200 && wf.NextMachineId == 100);
            var workflowResult = await DpRoWorkFlowRepository.FirstOrDefaultAsync(spec, TestContext.Current.CancellationToken);
            workflowResult.IsSuccess.ShouldBeTrue();
            workflowResult.Value.ShouldNotBeNull();
        }
    }

    /// <summary>
    /// Tests concurrent workflow creation.
    /// </summary>
    /// <returns>The result of the test.</returns>

    [Fact]
    public async Task Process_ConcurrentCreation_ShouldHandleCorrectly()
    {
        // Arrange
        var handler = await CreateHandlerAsync(TestContext.Current.CancellationToken);

        var tasks = new List<Task<Result<WorkFlowCreatedEvent>>>();

        // Act - Create workflows concurrently
        for (int i = 0; i < 5; i++)
        {
            var command = new CreateWorkFlowCommand
            {
                ProductId = 508,
                LastMachineId = 100,
                NextMachineId = 200
            };

            tasks.Add(handler.ProcessAsync(command, cancellationToken: TestContext.Current.CancellationToken));
        }

        var results = await Task.WhenAll(tasks);

        // Assert - All should succeed
        // The WorkFlowId will be unique due to database constrain
        // These test can create a race condition if run in parallel with other tests creating workflows for same product
        // The row dont hava a column for concurrent optimist access and even if they have one for creation it does not work just for update
        // The workflow creation is stored on many rows maybe a better strategy is store on json as a complete network, this can simplify the code
        // [TODO] [ABR] 8 SEPTIEMBER 2025 [REVIEW] [ARCHITECTURE]
        results.ShouldAllBe(r => r.IsSuccess);
        results.Select(r => r.Value!.ProductId).Distinct().Count().ShouldBe(1);
    }
}
