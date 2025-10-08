// <copyright file="IUserManagerResult.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Interfaces;

/// <summary>
/// Represents the result of a user management operation.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure interface is open for extension but closed for modification (OCP - SOLID). Consider using default interface methods or extension methods for future-proofing.
public interface IUserManagerResult
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    bool Succeeded { get; }

    /// <summary>
    /// Gets the collection of error messages, if any.
    /// </summary>
    IEnumerable<string> Errors { get; }
}
