// <copyright file="CycleTimeValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Services;

using IndTrace.Domain.Services.Interfaces;
using IndTrace.Domain.Entities;

/// <summary>
/// Validates cycle time against recipe constraints.
/// </summary>
public class CycleTimeValidator : ICycleTimeValidator
{
    /// <inheritdoc/>
    public CycleTimeValidationResult Validate(int cycleTime, Recipe? recipe)
    {
        // Handle null recipe case directly to maintain expected error message
        if (recipe is null)
        {
            return new CycleTimeValidationResult(false, "Recipe is null - cannot validate cycle time", true);
        }

        var validationResult = ValidateCycleTimeRange(cycleTime, recipe);

        return validationResult.IsSuccess
            ? new CycleTimeValidationResult(true, null, false)
            : new CycleTimeValidationResult(false, validationResult.Errors.First(), true);
    }

    private static Result<Recipe> ValidateCycleTimeRange(int cycleTime, Recipe recipe)
    {
        return Result<Recipe>
            .Success(recipe)
            .Ensure(r => cycleTime >= 0,
                $"Cycle time cannot be negative: {cycleTime}s")
            .Ensure(r => cycleTime > r.CycleTimeMinimum,
                $"Cycle time {cycleTime}s is below minimum {recipe.CycleTimeMinimum}s")
            .Ensure(r => cycleTime < r.CycleTimeMaximum,
                $"Cycle time {cycleTime}s exceeds maximum {recipe.CycleTimeMaximum}s");
    }
}
