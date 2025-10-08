// <copyright file="ICycleTimeValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Services.Interfaces;

using IndTrace.Domain.Entities;

/// <summary>
/// Validates cycle time against recipe constraints.
/// </summary>
public interface ICycleTimeValidator
{
    /// <summary>
    /// Validates if the cycle time is within acceptable bounds.
    /// </summary>
    /// <param name="cycleTime">The cycle time to validate.</param>
    /// <param name="recipe">The recipe containing constraints.</param>
    /// <returns>The validation result.</returns>
    CycleTimeValidationResult Validate(int cycleTime, Recipe? recipe);
}

/// <summary>
/// Represents the result of cycle time validation.
/// </summary>
/// <param name="IsValid">Indicates if the cycle time is valid.</param>
/// <param name="FailureReason">The reason for validation failure.</param>
/// <param name="ShouldForceNok">Indicates if NOK status should be forced.</param>
public record CycleTimeValidationResult(
    bool IsValid,
    string? FailureReason,
    bool ShouldForceNok);
