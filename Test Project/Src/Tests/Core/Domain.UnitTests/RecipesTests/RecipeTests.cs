namespace IndTrace.Domain.UnitTests.RecipesTests;

/// <summary>
/// Unit tests for Recipe domain entity
/// </summary>
public class RecipeTests
{
    /// <summary>
    /// Executes Recipe_Constructor_Default_ShouldCreateInstanceWithDefaultValues operation.
    /// </summary>
    [Fact]
    public void Recipe_Constructor_Default_ShouldCreateInstanceWithDefaultValues()
    {
        // Arrange & Act
        var recipe = new Recipe();

        // Assert
        recipe.ShouldNotBeNull();
        recipe.RecipeId.ShouldBe(0);
        recipe.ProductId.ShouldBe(0);
        recipe.MachineId.ShouldBe(0);
        recipe.CycleTimeMinimum.ShouldBe(0);
        recipe.CycleTimeMaximum.ShouldBe(216000);
        recipe.MaxCyclesOk.ShouldBe(3);
        recipe.MaxCyclesNOk.ShouldBe(5);
        recipe.Retry.ShouldBe(1);
    }
    /// <summary>
    /// Executes Recipe_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange
        var recipe = new Recipe();
        var recipeId = 100;
        var productId = 200;
        var machineId = 300;
        var cycleTimeMin = 10;
        var cycleTimeMax = 1000;
        var maxCyclesOk = 5;
        var maxCyclesNOk = 3;
        var retry = 2;

        // Act
        recipe.RecipeId = recipeId;
        recipe.ProductId = productId;
        recipe.MachineId = machineId;
        recipe.CycleTimeMinimum = cycleTimeMin;
        recipe.CycleTimeMaximum = cycleTimeMax;
        recipe.MaxCyclesOk = maxCyclesOk;
        recipe.MaxCyclesNOk = maxCyclesNOk;
        recipe.Retry = retry;

        // Assert
        recipe.RecipeId.ShouldBe(recipeId);
        recipe.ProductId.ShouldBe(productId);
        recipe.MachineId.ShouldBe(machineId);
        recipe.CycleTimeMinimum.ShouldBe(cycleTimeMin);
        recipe.CycleTimeMaximum.ShouldBe(cycleTimeMax);
        recipe.MaxCyclesOk.ShouldBe(maxCyclesOk);
        recipe.MaxCyclesNOk.ShouldBe(maxCyclesNOk);
        recipe.Retry.ShouldBe(retry);
    }
    /// <summary>
    /// Executes RecipeProperties_WhenSetToZero_ShouldAcceptZero operation.
    /// </summary>

    [Fact]
    public void RecipeProperties_WhenSetToZero_ShouldAcceptZero()
    {
        // Arrange
        var recipe = new Recipe();

        // Act
        recipe.RecipeId = 0;
        recipe.ProductId = 0;
        recipe.MachineId = 0;
        recipe.CycleTimeMinimum = 0;
        recipe.CycleTimeMaximum = 0;
        recipe.MaxCyclesOk = 0;
        recipe.MaxCyclesNOk = 0;
        recipe.Retry = 0;

        // Assert
        recipe.RecipeId.ShouldBe(0);
        recipe.ProductId.ShouldBe(0);
        recipe.MachineId.ShouldBe(0);
        recipe.CycleTimeMinimum.ShouldBe(0);
        recipe.CycleTimeMaximum.ShouldBe(0);
        recipe.MaxCyclesOk.ShouldBe(0);
        recipe.MaxCyclesNOk.ShouldBe(0);
        recipe.Retry.ShouldBe(0);
    }
    /// <summary>
    /// Executes RecipeProperties_WhenSetToNegative_ShouldAcceptNegative operation.
    /// </summary>

    [Fact]
    public void RecipeProperties_WhenSetToNegative_ShouldAcceptNegative()
    {
        // Arrange
        var recipe = new Recipe();

        // Act
        recipe.RecipeId = -1;
        recipe.ProductId = -1;
        recipe.MachineId = -1;
        recipe.CycleTimeMinimum = -1;
        recipe.CycleTimeMaximum = -1;
        recipe.MaxCyclesOk = -1;
        recipe.MaxCyclesNOk = -1;
        recipe.Retry = -1;

        // Assert
        recipe.RecipeId.ShouldBe(-1);
        recipe.ProductId.ShouldBe(-1);
        recipe.MachineId.ShouldBe(-1);
        recipe.CycleTimeMinimum.ShouldBe(-1);
        recipe.CycleTimeMaximum.ShouldBe(-1);
        recipe.MaxCyclesOk.ShouldBe(-1);
        recipe.MaxCyclesNOk.ShouldBe(-1);
        recipe.Retry.ShouldBe(-1);
    }
    /// <summary>
    /// Executes RecipeProperties_WhenSetToLargeValues_ShouldAcceptLargeValues operation.
    /// </summary>

    [Fact]
    public void RecipeProperties_WhenSetToLargeValues_ShouldAcceptLargeValues()
    {
        // Arrange
        var recipe = new Recipe();
        var largeValue = int.MaxValue;

        // Act
        recipe.RecipeId = largeValue;
        recipe.ProductId = largeValue;
        recipe.MachineId = largeValue;
        recipe.CycleTimeMinimum = largeValue;
        recipe.CycleTimeMaximum = largeValue;
        recipe.MaxCyclesOk = largeValue;
        recipe.MaxCyclesNOk = largeValue;
        recipe.Retry = largeValue;

        // Assert
        recipe.RecipeId.ShouldBe(largeValue);
        recipe.ProductId.ShouldBe(largeValue);
        recipe.MachineId.ShouldBe(largeValue);
        recipe.CycleTimeMinimum.ShouldBe(largeValue);
        recipe.CycleTimeMaximum.ShouldBe(largeValue);
        recipe.MaxCyclesOk.ShouldBe(largeValue);
        recipe.MaxCyclesNOk.ShouldBe(largeValue);
        recipe.Retry.ShouldBe(largeValue);
    }
    /// <summary>
    /// Executes Recipe_WhenRecipeIsCreated_ShouldHaveReasonableDefaults operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenRecipeIsCreated_ShouldHaveReasonableDefaults()
    {
        // Arrange & Act
        var recipe = new Recipe();

        // Assert - Verify business logic defaults
        recipe.CycleTimeMinimum.ShouldBe(0, "Minimum cycle time should default to 0");
        recipe.CycleTimeMaximum.ShouldBe(216000, "Maximum cycle time should default to 216000 (60 hours in seconds)");
        recipe.MaxCyclesOk.ShouldBe(3, "Default max successful cycles should be 3");
        recipe.MaxCyclesNOk.ShouldBe(5, "Default max unsuccessful cycles should be 5");
        recipe.Retry.ShouldBe(1, "Default retry count should be 1");
    }
    /// <summary>
    /// Executes Recipe_WhenRecipeIsConfigured_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenRecipeIsConfigured_ShouldBeValid()
    {
        // Arrange
        var recipe = new Recipe
        {
            RecipeId = 1,
            ProductId = 5080,
            MachineId = 200,
            CycleTimeMinimum = 30,
            CycleTimeMaximum = 120,
            MaxCyclesOk = 10,
            MaxCyclesNOk = 2,
            Retry = 3
        };

        // Act & Assert
        recipe.ShouldNotBeNull();
        recipe.RecipeId.ShouldBe(1);
        recipe.ProductId.ShouldBe(5080);
        recipe.MachineId.ShouldBe(200);
        recipe.CycleTimeMinimum.ShouldBe(30);
        recipe.CycleTimeMaximum.ShouldBe(120);
        recipe.MaxCyclesOk.ShouldBe(10);
        recipe.MaxCyclesNOk.ShouldBe(2);
        recipe.Retry.ShouldBe(3);
    }
    /// <summary>
    /// Executes Recipe_WhenCycleTimeRangeIsValid_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenCycleTimeRangeIsValid_ShouldBeValid()
    {
        // Arrange
        var recipe = new Recipe
        {
            CycleTimeMinimum = 10,
            CycleTimeMaximum = 100
        };

        // Act & Assert
        recipe.CycleTimeMinimum.ShouldBeLessThanOrEqualTo(recipe.CycleTimeMaximum);
    }
    /// <summary>
    /// Executes Recipe_WhenCycleTimeRangeIsReversed_ShouldStillBeValid operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenCycleTimeRangeIsReversed_ShouldStillBeValid()
    {
        // Arrange
        var recipe = new Recipe
        {
            CycleTimeMinimum = 100,
            CycleTimeMaximum = 10
        };

        // Act & Assert
        // Note: The domain doesn't enforce minimum <= maximum, so this should be valid
        recipe.CycleTimeMinimum.ShouldBe(100);
        recipe.CycleTimeMaximum.ShouldBe(10);
    }
    /// <summary>
    /// Executes Recipe_WhenRetryCountIsZero_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenRetryCountIsZero_ShouldBeValid()
    {
        // Arrange
        var recipe = new Recipe
        {
            Retry = 0
        };

        // Act & Assert
        recipe.Retry.ShouldBe(0);
    }
    /// <summary>
    /// Executes Recipe_WhenMaxCyclesAreZero_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenMaxCyclesAreZero_ShouldBeValid()
    {
        // Arrange
        var recipe = new Recipe
        {
            MaxCyclesOk = 0,
            MaxCyclesNOk = 0
        };

        // Act & Assert
        recipe.MaxCyclesOk.ShouldBe(0);
        recipe.MaxCyclesNOk.ShouldBe(0);
    }
    /// <summary>
    /// Executes Recipe_WhenRecipeHasLargeCycleTimes_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenRecipeHasLargeCycleTimes_ShouldBeValid()
    {
        // Arrange
        var recipe = new Recipe
        {
            CycleTimeMinimum = 3600, // 1 hour
            CycleTimeMaximum = 86400 // 24 hours
        };

        // Act & Assert
        recipe.CycleTimeMinimum.ShouldBe(3600);
        recipe.CycleTimeMaximum.ShouldBe(86400);
    }
    /// <summary>
    /// Executes Recipe_WhenRecipeHasHighRetryCount_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenRecipeHasHighRetryCount_ShouldBeValid()
    {
        // Arrange
        var recipe = new Recipe
        {
            Retry = 10
        };

        // Act & Assert
        recipe.Retry.ShouldBe(10);
    }
    /// <summary>
    /// Executes Recipe_WhenRecipeHasHighCycleLimits_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void Recipe_WhenRecipeHasHighCycleLimits_ShouldBeValid()
    {
        // Arrange
        var recipe = new Recipe
        {
            MaxCyclesOk = 100,
            MaxCyclesNOk = 50
        };

        // Act & Assert
        recipe.MaxCyclesOk.ShouldBe(100);
        recipe.MaxCyclesNOk.ShouldBe(50);
    }
}
