// <copyright file="ICsvFileBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

/// <summary>
/// Defines a contract for building CSV files from collections of records.
/// </summary>
/// <typeparam name="T">The type of records to include in the CSV file.</typeparam>
public interface ICsvFileBuilder<T>
{
    /// <summary>
    /// Builds a CSV file from a collection of records.
    /// </summary>
    /// <param name="records">The collection of records to include in the CSV file.</param>
    /// <returns>A byte array containing the CSV file data.</returns>
    byte[] BuildProductsFile(IEnumerable<T> records);
}
