// <copyright file="keyRegister.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Record representing a key registration that links a cycle to a variable for barcode processing.
/// </summary>
/// <remarks>
/// This record is used to establish relationships between manufacturing cycles and specific variables
/// in the barcode traceability system. It serves as a composite key for lookup operations.
/// </remarks>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate key registration logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the KeyRegister.
/// </summary>

public record KeyRegister
{
    /// <summary>
    /// Gets or sets the unique identifier of the manufacturing cycle.
    /// </summary>
    /// <value>The cycle ID as an integer.</value>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the variable associated with this cycle.
    /// </summary>
    /// <value>The variable ID as an integer.</value>
    public int VariableId { get; set; }
}
