// <copyright file="IBarCodeScanned.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.CognexComm;

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IBarCodeScanned logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IBarCodeScanned
{
    string? Label { get; set; }
}
