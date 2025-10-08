// <copyright file="IRuleEngine.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.RulesEngine;

/// <summary>
/// Defines the contract for a rule engine that applies business rules to target objects asynchronously.
/// </summary>
/// <typeparam name="T">The type of object that rules will be applied to.</typeparam>
/// <remarks>
/// The rule engine provides a flexible mechanism for applying business logic through configurable rules.
/// This allows for dynamic business rule processing without hardcoding logic in application services.
/// </remarks>
public interface IRuleEngine<T>
{
    /// <summary>
    /// Applies a collection of business rules to the specified target object asynchronously.
    /// </summary>
    /// <param name="target">The target object to which rules will be applied.</param>
    /// <param name="rules">The collection of rules to apply to the target object.</param>
    /// <returns>A task representing the asynchronous rule application operation.</returns>
    /// <remarks>
    /// Rules are applied in the order they appear in the collection. If any rule fails or throws an exception,
    /// the behavior depends on the specific implementation of the rule engine.
    /// </remarks>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate rule engine interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    Task<Result> ApplyRulesAsync(T target, IEnumerable<IRule<T>> rules);
}
