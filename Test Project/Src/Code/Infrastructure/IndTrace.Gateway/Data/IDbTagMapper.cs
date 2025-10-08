// <copyright file="IDbTagMapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Data;

/// <summary>
/// Provides a contract for mapping and retrieving PLC tag variables from a database.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate DB tag mapper interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IDbTagMapper
{
    /// <summary>
    /// Asynchronously retrieves the tags for a given machine identifier.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <returns>A dictionary mapping tag names to <see cref="VariableS7"/> objects.</returns>
    Task<Dictionary<string, VariableS7>> GetTagsAsync(int machineId);
}
