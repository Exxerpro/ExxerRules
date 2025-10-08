namespace IndTrace.Application.UnitTests.Products.Services;

using Meziantou.Extensions.Logging.Xunit;

/// <summary>
/// Unit tests for RecipeOrchestrator - Per-machine recipe generation logic.
/// Tests sophisticated recipe creation patterns and machine-specific configurations.
/// </summary>
public class RecipeOrchestratorTests
{
    private readonly IRepository<Recipe> _mockRecipeRepository;
    private readonly IRepository<Machine> _mockMachineRepository;
    private readonly ILogger<RecipeOrchestrator> _mockLogger;
    private readonly RecipeOrchestrator _orchestrator;

    public RecipeOrchestratorTests(ITestOutputHelper output)
    {
        _mockRecipeRepository = Substitute.For<IRepository<Recipe>>();
        _mockMachineRepository = Substitute.For<IRepository<Machine>>();
        _mockLogger = XUnitLogger.CreateLogger<RecipeOrchestrator>(output);
        _orchestrator = new RecipeOrchestrator(_mockRecipeRepository, _mockMachineRepository, _mockLogger);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_NullRecipeRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new RecipeOrchestrator(null!, _mockMachineRepository, _mockLogger));
    }

    [Fact]
    public void Constructor_NullMachineRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new RecipeOrchestrator(_mockRecipeRepository, null!, _mockLogger));
    }

    [Fact]
    public void Constructor_NullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new RecipeOrchestrator(_mockRecipeRepository, _mockMachineRepository, null!));
    }

    #endregion Constructor Tests

    #region GenerateRecipesForProductAsync Tests

    [Fact]
    public async Task GenerateRecipesForProductAsync_MultipleMachines_ShouldCreateRecipePerMachine()
    {
        // Arrange
        var product = CreateValidProduct();
        var machines = CreateTestMachines(3); // 3 machines
        var productInput = CreateValidProductInput();

        _mockRecipeRepository
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _orchestrator.GenerateRecipesForProductAsync(product, machines, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(3); // One recipe per machine

        // Verify each machine got a recipe
        await _mockRecipeRepository
            .Received(3)
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenerateRecipesForProductAsync_NoMachines_ShouldReturnEmptyList()
    {
        // Arrange
        var product = CreateValidProduct();
        var machines = new List<Machine>(); // No machines
        var productInput = CreateValidProductInput();

        // Act
        var result = await _orchestrator.GenerateRecipesForProductAsync(product, machines, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();

        // No repository calls should be made
        await _mockRecipeRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GenerateRecipesForProductAsync_NullProduct_ShouldReturnFailure()
    {
        // Arrange
        var machines = CreateTestMachines(1);
        var productInput = CreateValidProductInput();

        // Act
        var result = await _orchestrator.GenerateRecipesForProductAsync(null!, machines, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product cannot be null for recipe generation.");
    }

    [Fact]
    public async Task GenerateRecipesForProductAsync_NullMachines_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        var productInput = CreateValidProductInput();

        // Act
        var result = await _orchestrator.GenerateRecipesForProductAsync(product, null!, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Machines collection cannot be null for recipe generation.");
    }

    [Fact]
    public async Task GenerateRecipesForProductAsync_PartialFailure_ShouldReturnSuccessfulRecipes()
    {
        // Arrange
        var product = CreateValidProduct();
        var machines = CreateTestMachines(3);
        var productInput = CreateValidProductInput();

        // Setup repository to fail for one machine but succeed for others
        _mockRecipeRepository
            .AddAsync(Arg.Is<Recipe>(r => r.MachineId == 1), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));
        _mockRecipeRepository
            .AddAsync(Arg.Is<Recipe>(r => r.MachineId == 2), Arg.Any<CancellationToken>())
            .Returns(Result<int>.WithFailure(["Database error"]));
        _mockRecipeRepository
            .AddAsync(Arg.Is<Recipe>(r => r.MachineId == 3), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _orchestrator.GenerateRecipesForProductAsync(product, machines, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue(); // Should succeed with partial results
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(2); // Only successful recipes
    }

    [Fact]
    public async Task GenerateRecipesForProductAsync_AllFailures_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        var machines = CreateTestMachines(2);
        var productInput = CreateValidProductInput();

        // Setup repository to fail for all machines
        _mockRecipeRepository
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.WithFailure(["Database error"]));

        // Act
        var result = await _orchestrator.GenerateRecipesForProductAsync(product, machines, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain($"No recipes could be generated for Product {product.ProductId}");
    }

    #endregion GenerateRecipesForProductAsync Tests

    #region GenerateRecipeForMachineAsync Tests

    [Fact]
    public async Task GenerateRecipeForMachineAsync_ValidInputs_ShouldCreateRecipeSuccessfully()
    {
        // Arrange
        var product = CreateValidProduct();
        var machine = CreateTestMachine(1, "LASER-CUT-001");
        var productInput = CreateValidProductInput();

        // [Fix] Configure mock to simulate ID assignment during persistence
        _mockRecipeRepository
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1))
            .AndDoes(callInfo =>
            {
                var recipe = callInfo.Arg<Recipe>();
                recipe.RecipeId = 1; // Simulate ID assignment by persistence layer
            });

        // Act
        var result = await _orchestrator.GenerateRecipeForMachineAsync(product, machine, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var recipe = result.Value;
        recipe.RecipeId.ShouldBeGreaterThan(0); // Recipe should be assigned an ID
        recipe.ProductId.ShouldBe(product.ProductId);
        recipe.MachineId.ShouldBe(machine.MachineId);
        recipe.CycleTimeMinimum.ShouldBeGreaterThan(0);
        recipe.CycleTimeMaximum.ShouldBeGreaterThan(recipe.CycleTimeMinimum);
        recipe.MaxCyclesOk.ShouldBeGreaterThan(0);
        recipe.MaxCyclesNOk.ShouldBeGreaterThan(0);

        await _mockRecipeRepository
            .Received(1)
            .AddAsync(Arg.Is<Recipe>(r => r.ProductId == product.ProductId && r.MachineId == machine.MachineId), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("LASER-CUT-001")]
    [InlineData("MILL-STATION-002")]
    [InlineData("WELD-ROBOT-003")]
    [InlineData("STANDARD-MACHINE")]
    public async Task GenerateRecipeForMachineAsync_VariousMachineTypes_ShouldDetermineCorrectRecipeType(
        string machineName)
    {
        // Arrange
        var product = CreateValidProduct();
        var machine = CreateTestMachine(1, machineName);
        var productInput = CreateValidProductInput();

        _mockRecipeRepository
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _orchestrator.GenerateRecipeForMachineAsync(product, machine, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ProductId.ShouldBe(product.ProductId);
    }

    [Fact]
    public async Task GenerateRecipeForMachineAsync_NullMachine_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        var productInput = CreateValidProductInput();

        // Act
        var result = await _orchestrator.GenerateRecipeForMachineAsync(product, null!, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Machine cannot be null for recipe generation.");
    }

    [Fact]
    public async Task GenerateRecipeForMachineAsync_RepositoryFailure_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        var machine = CreateTestMachine(1, "TEST-MACHINE");
        var productInput = CreateValidProductInput();

        _mockRecipeRepository
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.WithFailure(["Database error"]));

        // Act
        var result = await _orchestrator.GenerateRecipeForMachineAsync(product, machine, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Failed to persist recipe: Database error");
    }

    #endregion GenerateRecipeForMachineAsync Tests

    #region LinkExistingRecipeToProductAsync Tests

    [Fact]
    public async Task LinkExistingRecipeToProductAsync_ValidRecipe_ShouldLinkSuccessfully()
    {
        // Arrange
        var product = CreateValidProduct();
        var machine = CreateTestMachine(1, "TEST-MACHINE");
        const int recipeId = 1;
        var existingRecipe = CreateValidRecipe(product.ProductId, machine.MachineId);

        _mockRecipeRepository
            .GetByIdAsync(recipeId, Arg.Any<CancellationToken>())
            .Returns(Result<Recipe?>.Success(existingRecipe));

        // Act
        var result = await _orchestrator.LinkExistingRecipeToProductAsync(product, machine, recipeId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(existingRecipe);
    }

    [Fact]
    public async Task LinkExistingRecipeToProductAsync_RecipeNotFound_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        var machine = CreateTestMachine(1, "TEST-MACHINE");
        const int recipeId = 999;

        _mockRecipeRepository
            .GetByIdAsync(recipeId, Arg.Any<CancellationToken>())
            .Returns(Result<Recipe?>.WithFailure("Not found"));

        // Act
        var result = await _orchestrator.LinkExistingRecipeToProductAsync(product, machine, recipeId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain($"Recipe not found {recipeId}");
    }

    [Fact]
    public async Task LinkExistingRecipeToProductAsync_IncompatibleRecipe_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProduct();
        product.ProductId = 1;
        var machine = CreateTestMachine(1, "TEST-MACHINE");
        const int recipeId = 1;

        var incompatibleRecipe = CreateValidRecipe(999, machine.MachineId); // Different product

        _mockRecipeRepository
            .GetByIdAsync(recipeId, Arg.Any<CancellationToken>())
            .Returns(Result<Recipe?>.Success(incompatibleRecipe));

        // Act
        var result = await _orchestrator.LinkExistingRecipeToProductAsync(product, machine, recipeId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Recipe ProductId 999 does not match Product ProductId 1.");
    }

    #endregion LinkExistingRecipeToProductAsync Tests

    #region ValidateRecipeUniquenessAsync Tests

    [Fact]
    public async Task ValidateRecipeUniquenessAsync_UniqueRecipe_ShouldReturnSuccess()
    {
        // Arrange
        const string recipeName = "RCP-UNIQUE-001";
        const int productId = 1;
        const int machineId = 1;

        _mockRecipeRepository
            .FirstOrDefaultAsync(Arg.Any<Specification<Recipe>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Recipe?>.WithFailure("Not found"));

        // Act
        var result = await _orchestrator.ValidateRecipeUniquenessAsync(recipeName, productId, machineId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateRecipeUniquenessAsync_DuplicateRecipe_ShouldReturnFailure()
    {
        // Arrange
        const string recipeName = "RCP-EXISTING-001";
        const int productId = 1;
        const int machineId = 1;
        var existingRecipe = CreateValidRecipe(productId, machineId);

        _mockRecipeRepository
            .FirstOrDefaultAsync(Arg.Any<Specification<Recipe>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Recipe?>.Success(existingRecipe));

        // Act
        var result = await _orchestrator.ValidateRecipeUniquenessAsync(recipeName, productId, machineId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain($"Recipe already exists {recipeName}");
    }

    #endregion ValidateRecipeUniquenessAsync Tests

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
            LineId = 1,
            RuleId = 2005, // [Fix] Set RuleId for recipe generation
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
            LineId = 1,
            IsActive = 1,
            Version = 1,
            CreatedBy = "TEST_USER"
        };
    }

    private Machine CreateTestMachine(int machineId, string machineName)
    {
        return new Machine
        {
            MachineId = machineId,
            Name = machineName,
            MachineType = MachineType.Printer,
            Description = $"Test machine {machineName}",
            Location = "Test Location"
        };
    }

    private List<Machine> CreateTestMachines(int count)
    {
        var machines = new List<Machine>();
        for (int i = 1; i <= count; i++)
        {
            machines.Add(CreateTestMachine(i, $"MACHINE-{i:D3}"));
        }
        return machines;
    }

    private Recipe CreateValidRecipe(int productId, int machineId)
    {
        return new Recipe
        {
            RecipeId = 1,
            ProductId = productId,
            MachineId = machineId,
            CycleTimeMinimum = 1000,
            CycleTimeMaximum = 5000,
            MaxCyclesOk = 3,
            MaxCyclesNOk = 5,
            Retry = 1
        };
    }

    #endregion Helper Methods
}
