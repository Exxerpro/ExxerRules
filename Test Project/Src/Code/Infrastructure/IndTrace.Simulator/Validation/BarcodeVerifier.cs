// <copyright file="BarcodeVerifier.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Validation;

using Dapper;

/// <summary>
/// Provides methods to verify barcode existence and validate cycle state transitions in the database.
/// </summary>
public class BarcodeVerifier(IDbConnection db)
{
    /// <summary>
    /// Determines whether the specified barcode is recorded in the database.
    /// </summary>
    /// <param name="barcode">The barcode to check.</param>
    /// <returns>True if the barcode is recorded; otherwise, false.</returns>
    public async Task<bool> IsBarcodeRecordedAsync(string barcode)
    {
        const string sql = "SELECT COUNT(1) FROM BarCodes WHERE Label = @Label";
        var count = await db.ExecuteScalarAsync<int>(sql, new { Label = barcode });
        return count > 0;
    }

    /// <summary>
    /// Checks if the barcode has the expected cycle state.
    /// </summary>
    /// <param name="barcode">The barcode to check.</param>
    /// <param name="expectedState">The expected cycle state.</param>
    /// <returns>True if the barcode has the expected state; otherwise, false.</returns>
    public async Task<bool> HasExpectedCycleStateAsync(string barcode, string expectedState)
    {
        const string sql = @"SELECT TOP 1 c.CycleStatus FROM Cycles c
                             JOIN BarCodes b ON b.BarCodeId = c.BarCodeId
                             WHERE b.Label = @Label
                             ORDER BY c.FinishedOn DESC";

        var cycleStatus = await db.ExecuteScalarAsync<int?>(sql, new { Label = barcode });
        return expectedState switch
        {
            "FinishedOk" => cycleStatus == 4,
            "FinishedNotOk" => cycleStatus == 8,
            "Started" => cycleStatus == 2,
            _ => false,
        };
    }

    /// <summary>
    /// Validates the transition between two cycle states for a barcode.
    /// </summary>
    /// <param name="barcode">The barcode to check.</param>
    /// <param name="previousState">The previous state.</param>
    /// <param name="expectedNewState">The expected new state.</param>
    /// <returns>True if the transition is valid; otherwise, false.</returns>
    public async Task<bool> ValidateCycleTransitionAsync(string barcode, string previousState, string expectedNewState)
    {
        const string sql = @"SELECT TOP 2 c.CycleStatus FROM Cycles c
                             JOIN BarCodes b ON b.BarCodeId = c.BarCodeId
                             WHERE b.Label = @Label
                             ORDER BY c.FinishedOn DESC";

        var statusList = (await db.QueryAsync<int>(sql, new { Label = barcode })).ToList();
        if (statusList.Count < 2)
        {
            return false;
        }

        var prev = statusList[1];
        var next = statusList[0];

        return previousState switch
        {
            "Started" => expectedNewState == "FinishedOk" && next == 4 && prev == 2,
            "StartedFail" => expectedNewState == "FinishedNotOk" && next == 8 && prev == 2,
            _ => false,
        };
    }

    /// <summary>
    /// Validates the post-execution state of a barcode and part number.
    /// </summary>
    /// <param name="barcode">The barcode to check.</param>
    /// <param name="partNumber">The part number associated with the barcode.</param>
    /// <param name="expectedState">The expected state after execution.</param>
    /// <returns>True if the post-execution state is valid; otherwise, false.</returns>
    public async Task<bool> ValidatePostExecutionStateAsync(string barcode, string partNumber, string expectedState)
    {
        var recorded = await this.IsBarcodeRecordedAsync(barcode);
        if (!recorded)
        {
            return false;
        }

        var validFormat = BarcodeValidator.IsValidBarcode(barcode, partNumber);
        if (!validFormat)
        {
            return false;
        }

        return await this.HasExpectedCycleStateAsync(barcode, expectedState);
    }
}
