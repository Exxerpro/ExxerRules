using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.Models;
using IndTrace.DataStore.ModelsComs;

namespace IndTrace.DataStore.DataAccess;

/// <summary>
/// Provides database access methods for fixture-related data, including barcodes, PLC addresses, and static snapshots.
/// </summary>
public class FixtureDb(IDbConnection db, ILogger<FixtureDb> logger) : IFixtureDb
{
    /// <summary>
    /// Loads the state of a barcode asynchronously from the database.
    /// </summary>
    /// <param name="barcode">The barcode to load the state for.</param>
    /// <returns>A <see cref="BarcodeDbRecord"/> representing the barcode state.</returns>
    public async Task<BarcodeDbRecord> LoadBarcodeStateAsync(string barcode)
    {
        const string sqlSelectTags = @"
					SELECT TOP 1
					b.BarCodeId,
					b.ProductId,
					b.Label AS Barcode,
					b.MachineId,
					b.FlowStatus,
					c.PartStatus,
					c.CycleStatus,
					c.CycleId,
					c.MachineId AS CycleMachineId,

					COALESCE(c.FinishedOn, c.StartedOn, b.CreatedOn) AS LastModified
				FROM dbo.BarCodes b
				LEFT JOIN dbo.Cycles c ON b.BarCodeId = c.BarCodeId
				WHERE b.Label = @barcode
				ORDER BY
					c.FinishedOn DESC,
					c.StartedOn DESC
					";

        try
        {
            var result = await db.QueryFirstOrDefaultAsync<BarcodeDbRecord>(sqlSelectTags, new { barcode });
            return result ?? new BarcodeDbRecord { Barcode = barcode };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load barcode state for {Barcode}", barcode);
            return new BarcodeDbRecord { Barcode = barcode }; ;
        }
    }

    /// <summary>
    /// Loads PLC address records asynchronously from the database.
    /// </summary>
    /// <returns>A dictionary mapping machine IDs to PLC records.</returns>
    public async Task<Dictionary<int, PlcRecordDb>> LoadPlcAddressAsync()
    {
        const string sql = @"
		SELECT TOP 100
			MachineId,
			Name,
			IpAddress
		FROM Plcs";

        try
        {
            var result = await db.QueryAsync<PlcRecordDb>(sql);
            return result?.ToDictionary(plc => plc.MachineId) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while loading PLC records.");
            return [];
        }
    }

    /// <summary>
    /// Loads a static snapshot for a product by part number and machine ID asynchronously.
    /// </summary>
    /// <param name="partNumber">The part number of the product.</param>
    /// <param name="machineId">The machine ID to filter workflows.</param>
    /// <returns>A <see cref="FixtureStaticSnapshot"/> representing the static snapshot.</returns>
    public async Task<FixtureStaticSnapshot> LoadStaticSnapshotByPartNumberAsync(string partNumber, int machineId)
    {
        const string sql = @"
				SELECT TOP 1
					ProductId,
					PartNumber,
					ProductName,
					IsActive,
					Description
				FROM dbo.Products
				WHERE PartNumber = @PartNumber;
			SELECT
				 WorkFlowId ,
				 ProductId ,
				 NextMachineId ,
				 LastMachineId
				FROM dbo.WorkFlows
				WHERE ProductId = (SELECT TOP 1 ProductId FROM dbo.Products WHERE PartNumber = @PartNumber AND
				( NextMachineId = @MachineId OR LastMachineId = @MachineId ) );
			";

        try
        {
            //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture DB logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
            await using var multi = await db.QueryMultipleAsync(sql, new { PartNumber = partNumber, MachineId = machineId });

            var product = await multi.ReadSingleOrDefaultAsync<ProductDbRecord>();
            if (product == null)
                return new FixtureStaticSnapshot();

            var workFlows = (await multi.ReadAsync<WorkFlowDbRecord>()).ToList();

            return new FixtureStaticSnapshot
            {
                ProductId = product.ProductId,
                PartNumber = product.PartNumber,
                ProductName = product.ProductName,
                IsActive = product.IsActive,
                Description = product.Description,
                WorkFlows = workFlows,
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load product state for {PartNumber}", partNumber);
            return new FixtureStaticSnapshot();
        }
    }
}
