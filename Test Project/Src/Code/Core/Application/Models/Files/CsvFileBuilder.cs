// <copyright file="CsvFileBuilder.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>
// namespace IndTrace.Application.Common.Files;

namespace IndTrace.Application.Models.Files;

// public class CsvFileBuilder<Barcodes> : ICsvFileBuilder<Barcodes>
// {
//    public byte[] BuildProductsFile(IEnumerable<Barcodes> records)
//    {
//        using var memoryStream = new MemoryStream();
//        using (var streamWriter = new StreamWriter(memoryStream))

// {
//            using var csvWriter = new CsvWriter(streamWriter, System.Globalization.CultureInfo.CurrentCulture);
//            csvWriter.Configuration.RegisterClassMap<ProductFileRecordMap>();
//            csvWriter.WriteRecords(records);
//        }
// return memoryStream.ToArray();
//    }
// }
using CsvHelper;

/// <summary>
/// Provides functionality to build CSV files from a collection of barcode records.
/// </summary>
public class CsvFileBuilder<TBarcodes> : ICsvFileBuilder<TBarcodes>
{
    /// <summary>
    /// Builds a CSV file from the provided collection of barcode records.
    /// </summary>
    /// <param name="records">The collection of barcode records.</param>
    /// <returns>A byte array containing the CSV file data.</returns>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate CSV input data defensively to avoid malformed files or injection. See .NET best practices for file handling.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For large CSV files, consider streaming or batching to avoid high memory usage.
    /// <summary>
    /// Executes BuildProductsFile operation.
    /// </summary>
    /// <param name="records">The records.</param>
    /// <returns>The result of BuildProductsFile.</returns>
    public byte[] BuildProductsFile(IEnumerable<TBarcodes> records)
    {
        // [Fix]
        // CLAUDE
        // Date: 21/08/2025
        // Reason: Fixed generic CSV builder - removed hardcoded ProductFileRecordMap registration to work with any type
        using var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream))
        {
            using var csvWriter = new CsvWriter(streamWriter, System.Globalization.CultureInfo.CurrentCulture);

            // Don't register specific map - let CsvHelper auto-map the type
            csvWriter.WriteRecords(records);
        }

        return memoryStream.ToArray();
    }
}
