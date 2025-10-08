using IndTrace.Persistence.Repositories;
using IndTrace.Domain.Entities;
using Shouldly;
using Xunit;

namespace IndTrace.Persistence.Tests.Repositories;

/// <summary>
/// Unit tests for RepositoryCacheKeyBuilder to ensure unique cache key generation.
/// CRITICAL BUG: Current implementation hardcodes "RecipeId" for all entity types,
/// causing cache key collisions between different entities with the same ID.
/// </summary>
public class RepositoryCacheKeyBuilderTests
{
    [Theory]
    [InlineData("GetById", 300)]
    [InlineData("GetById", 500)]
    [InlineData("FirstOrDefault", 100)]
    public void BuildKey_WithEntityTypeAndId_ShouldCreateUniqueKeysForDifferentEntityTypes(string operation, int id)
    {
        // Arrange & Act
        var machineKey = RepositoryCacheKeyBuilder.BuildKey<Machine>(operation, id);
        var recipeKey = RepositoryCacheKeyBuilder.BuildKey<Recipe>(operation, id);
        var productKey = RepositoryCacheKeyBuilder.BuildKey<Product>(operation, id);

        // Assert - Keys should be unique for different entity types with same ID
        machineKey.ShouldNotBe(recipeKey, "Machine and Recipe with same ID should have different cache keys");
        machineKey.ShouldNotBe(productKey, "Machine and Product with same ID should have different cache keys");
        recipeKey.ShouldNotBe(productKey, "Recipe and Product with same ID should have different cache keys");

        // Keys should contain entity type name
        machineKey.ShouldContain("Machine");
        recipeKey.ShouldContain("Recipe");
        productKey.ShouldContain("Product");
    }

    [Theory]
    [InlineData("GetById", 300)]
    [InlineData("GetById", 500)]
    public void BuildKey_ForMachine_ShouldUseMachineIdNotRecipeId(string operation, int machineId)
    {
        // Arrange & Act
        var machineKey = RepositoryCacheKeyBuilder.BuildKey<Machine>(operation, machineId);

        // Assert - BUG DETECTION: Should use "MachineId" not "RecipeId"
        machineKey.ShouldNotContain("RecipeId", "Machine cache key should not contain 'RecipeId'");
        machineKey.ShouldContain($"MachineId:{machineId}", "Machine cache key should contain 'MachineId:' + actual ID");
        machineKey.ShouldContain("Type:Machine");
        machineKey.ShouldStartWith(operation);
    }

    [Theory]
    [InlineData("GetById", 100)]
    [InlineData("GetById", 200)]
    public void BuildKey_ForRecipe_ShouldUseRecipeId(string operation, int recipeId)
    {
        // Arrange & Act
        var recipeKey = RepositoryCacheKeyBuilder.BuildKey<Recipe>(operation, recipeId);

        // Assert
        recipeKey.ShouldContain($"RecipeId:{recipeId}");
        recipeKey.ShouldContain("Type:Recipe");
        recipeKey.ShouldStartWith(operation);
    }

    [Fact]
    public void BuildKey_SameEntityTypeWithDifferentIds_ShouldCreateDifferentKeys()
    {
        // Arrange & Act
        var machine100 = RepositoryCacheKeyBuilder.BuildKey<Machine>("GetById", 100);
        var machine300 = RepositoryCacheKeyBuilder.BuildKey<Machine>("GetById", 300);
        var machine500 = RepositoryCacheKeyBuilder.BuildKey<Machine>("GetById", 500);

        // Assert - Each ID should create unique cache key
        machine100.ShouldNotBe(machine300);
        machine100.ShouldNotBe(machine500);
        machine300.ShouldNotBe(machine500);

        // All should be Machine type
        machine100.ShouldContain("Type:Machine");
        machine300.ShouldContain("Type:Machine");
        machine500.ShouldContain("Type:Machine");
    }

    [Fact]
    public void BuildKey_WithSpecification_ShouldUseSpecificationKey()
    {
        // Arrange
        var spec = new Specification<Machine>(m => m.MachineId == 300);

        // Act
        var key = RepositoryCacheKeyBuilder.BuildKey("FirstOrDefault", spec);

        // Assert
        key.ShouldStartWith("FirstOrDefault|");
        key.ShouldContain(spec.Key);
    }

    [Fact]
    public void BuildKey_WithoutId_ShouldUseEntityTypeOnly()
    {
        // Arrange & Act
        var machineKey = RepositoryCacheKeyBuilder.BuildKey<Machine>("ListAsync");
        var recipeKey = RepositoryCacheKeyBuilder.BuildKey<Recipe>("ListAsync");

        // Assert
        machineKey.ShouldNotBe(recipeKey);
        machineKey.ShouldContain("Type:Machine");
        recipeKey.ShouldContain("Type:Recipe");
        machineKey.ShouldStartWith("ListAsync|");
        recipeKey.ShouldStartWith("ListAsync|");
    }

    /// <summary>
    /// CRITICAL BUG REPRODUCTION TEST
    /// This test demonstrates the cache collision bug where Machine 300 and Machine 500
    /// both return cached data for Machine 100 due to flawed cache key generation.
    /// </summary>
    [Fact]
    public void BugReproduction_CacheKeyCollision_MachinesShouldHaveUniqueKeys()
    {
        // Arrange - The problematic scenario from ComprehensiveEnumRepositoryTests
        var machine100Key = RepositoryCacheKeyBuilder.BuildKey<Machine>("GetById", 100);
        var machine300Key = RepositoryCacheKeyBuilder.BuildKey<Machine>("GetById", 300);
        var machine500Key = RepositoryCacheKeyBuilder.BuildKey<Machine>("GetById", 500);

        // Act - This test will FAIL with current buggy implementation
        // Current bug: All keys contain "RecipeId" instead of proper entity ID field

        // Assert - Each machine should have a unique cache key
        machine100Key.ShouldNotBe(machine300Key,
            "Machine 100 and 300 should have different cache keys to prevent data collision");
        machine300Key.ShouldNotBe(machine500Key,
            "Machine 300 and 500 should have different cache keys to prevent data collision");
        machine100Key.ShouldNotBe(machine500Key,
            "Machine 100 and 500 should have different cache keys to prevent data collision");

        // BUG DETECTION: Current implementation incorrectly uses "RecipeId" for ALL entities
        machine100Key.ShouldNotContain("RecipeId",
            "CRITICAL BUG: Machine cache key should not contain 'RecipeId' - this causes cache collisions!");
        machine300Key.ShouldNotContain("RecipeId",
            "CRITICAL BUG: Machine cache key should not contain 'RecipeId' - this causes cache collisions!");
        machine500Key.ShouldNotContain("RecipeId",
            "CRITICAL BUG: Machine cache key should not contain 'RecipeId' - this causes cache collisions!");
    }
}
