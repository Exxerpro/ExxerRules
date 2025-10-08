// <copyright file="Recipe.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a recipe entity with product configuration and processing instructions.
/// </summary>
public class Recipe : IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the recipe.
    /// </summary>
    public int RecipeId { get; set; }

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
    /// Gets or sets the maximum number of successful cycles allowed.
    /// </summary>
    public int MaxCyclesOk { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum number of unsuccessful cycles allowed.
    /// </summary>
    public int MaxCyclesNOk { get; set; } = 5;

    /// <summary>
    /// Gets or sets the retry count for the recipe.
    /// </summary>
    public int Retry { get; set; } = 1;

    /// <summary>
    /// Returns a string representation of the Recipe.
    /// </summary>
    /// <returns>A string containing the recipe ID, product ID, and machine ID.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"Recipe {this.RecipeId} (Product {this.ProductId}, Machine {this.MachineId})";
}
