// <copyright file="DuplicateTagException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.S7Rx.Exceptions;

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate DuplicateTagException logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

/// <summary>
/// Represents the DuplicateTagException.
/// </summary>
public class DuplicateTagException(int variableId, string variableName, string variableAddress)
    : Exception($"Duplicate tag detected: RecipeId={variableId}, Name={variableName}, Address={variableAddress}")
{
    public int VariableId { get; } = variableId;

    public string VariableName { get; } = variableName;

    public string VariableAddress { get; } = variableAddress;
}
