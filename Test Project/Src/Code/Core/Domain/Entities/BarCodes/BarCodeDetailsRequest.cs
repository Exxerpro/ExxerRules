// <copyright file="BarCodeDetailsRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities.BarCodes;

using IndTrace.Domain.Models;

/// <summary>
/// Represents a request for barcode details, including machine ID, label, and part number.
/// </summary>
public class BarCodeDetailsRequest(int machineId, string label, string partNumber)
{
    /// <summary>
    /// Gets the identifier of the machine associated with the barcode request.
    /// </summary>
    public int MachineId { get; } = machineId;

    /// <summary>
    /// Gets the label value for the barcode request.
    /// </summary>
    public string Label { get; } = label ?? string.Empty; // Functional style: avoid exceptions in domain

    /// <summary>
    /// Gets the part number associated with the barcode request.
    /// </summary>
    public string PartNumber { get; } = partNumber ?? string.Empty; // Functional style: avoid exceptions in domain

    /// <summary>
    /// Builds a <see cref="BarCodeDetailsRequest"/> validating inputs using the functional Result pattern.
    /// </summary>
    /// <param name="machineId">Machine identifier (must be greater than 0).</param>
    /// <param name="label">Barcode label (required).</param>
    /// <param name="partNumber">Part number (required).</param>
    /// <returns>A <see cref="Result{T}"/> with the created <see cref="BarCodeDetailsRequest"/> or validation errors.</returns>
    public static Result<BarCodeDetailsRequest> Build(int machineId, string? label, string? partNumber)
    {
        var errors = new List<string>();

        if (machineId <= 0)
        {
            errors.Add("machineId must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(label))
        {
            errors.Add("label must be provided");
        }

        if (string.IsNullOrWhiteSpace(partNumber))
        {
            errors.Add("partNumber must be provided");
        }

        if (errors.Count > 0)
        {
            return Result<BarCodeDetailsRequest>.WithFailure(errors);
        }

        return Result<BarCodeDetailsRequest>.Success(new BarCodeDetailsRequest(machineId, label!, partNumber!));
    }
}
