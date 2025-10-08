// <copyright file="GetBarCodeDetailGatewayQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeGatewayDetail;

/// <summary>
/// Represents the GetBarCodeDetailGatewayQueryHandler.
/// </summary>
public class GetBarCodeDetailGatewayQueryHandler(IBarCodeResult barCodeResult) :
    IGatewayRequestHandler<ReadBarCodeQuery, TaskGatewayResponse>, IResettable
{
    /// <inheritdoc/>
    public async Task<Result<TaskGatewayResponse>> ProcessAsync(ReadBarCodeQuery cmd, CancellationToken cancellationToken)
    {
        var request = cmd.Command;

        BarCodeDetailsRequest barCodeDetailsRequest =
            new BarCodeDetailsRequest(
                request.MachineId,
                request.BarCode,
                request.PartNumber);

        var barCodeInfo = await barCodeResult.GetBarCodeDetails(
            barCodeDetailsRequest,
            cancellationToken);

        var result = TaskGatewayResponse.ToDto(barCodeInfo);
        _ = result.ApplyReferencesValuesResult();

        if (barCodeInfo.Error is not null && barCodeInfo.Error.Length > 0)
        {
            return Result<TaskGatewayResponse>.WithFailure(barCodeInfo.Error, result);
        }

        // [Fix]
        // CLAUDE
        // Date: 22/08/2025
        // Reason: [CLUSTER A - MASSIVE FIX] - Return Result<T>.Success() for Railway-Oriented Programming pattern
        return Result<TaskGatewayResponse>.Success(result);
    }

    /// <inheritdoc/>
    public bool TryReset()
    {
        return true;
    }
}
