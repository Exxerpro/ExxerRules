// <copyright file="RecipeDto.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repositories;

/// <summary>
/// Data transfer object for Recipe entity, used for transferring recipe data between layers.
/// </summary>
public class RecipeDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the recipe.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product identifier associated with the recipe.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the recipe.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the minimum cycle time for the recipe.
    /// </summary>
    public int CycleTimeMinimum { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum cycle time for the recipe.
    /// </summary>
    public int CycleTimeMaximum { get; set; } = 216000;

    /// <summary>
    /// Converts a <see cref="Recipe"/> entity to a <see cref="RecipeDto"/>.
    /// </summary>
    /// <param name="src">The source <see cref="Recipe"/> entity.</param>
    /// <returns>A <see cref="RecipeDto"/> representing the entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="src"/> is null.</exception>
    public static IndQuestResults.Result<RecipeDto> ToDto(Recipe src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<RecipeDto>.WithFailure("Recipe source cannot be null");
        }

        return IndQuestResults.Result<RecipeDto>.Success(new RecipeDto
        {
            Id = src.RecipeId,
            ProductId = src.ProductId,
            MachineId = src.MachineId,
            CycleTimeMinimum = src.CycleTimeMinimum,
            CycleTimeMaximum = src.CycleTimeMaximum,
        });
    }

    /// <summary>
    /// Converts a <see cref="RecipeDto"/> to a <see cref="Recipe"/> entity.
    /// </summary>
    /// <param name="src">The source <see cref="RecipeDto"/>.</param>
    /// <returns>A <see cref="Recipe"/> entity representing the DTO.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="src"/> is null.</exception>
    public static IndQuestResults.Result<Recipe> ToEntity(RecipeDto src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<Recipe>.WithFailure("RecipeDto source cannot be null");
        }

        return IndQuestResults.Result<Recipe>.Success(new Recipe
        {
            RecipeId = src.Id,
            ProductId = src.ProductId,
            MachineId = src.MachineId,
            CycleTimeMinimum = src.CycleTimeMinimum,
            CycleTimeMaximum = src.CycleTimeMaximum,
        });
    }
}
