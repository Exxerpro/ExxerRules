// <copyright file="IRule.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.RulesEngine;

/// <summary>
/// Defines a contract for applying a rule to a target entity asynchronously.
/// </summary>
/// <typeparam name="T">The type of the target entity.</typeparam>
public interface IRule<T>
{
    /// <summary>
    /// Applies the rule to the specified target asynchronously.
    /// </summary>
    /// <param name="target">The target entity to apply the rule to.</param>
    /// <returns>A Result indicating success or failure of the rule application.</returns>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate rule interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    Task<Result> ApplyAsync(T target);
}
