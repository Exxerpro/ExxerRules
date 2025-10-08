using IndTrace.DataStore.DataAccess;
using IndTrace.DataStore.ModelsComs;

namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines methods for accessing fixture-related data from the database.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IFixtureDb logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IFixtureDb
{
    /// <summary>
    /// Loads the state of a barcode asynchronously from the database.
    /// </summary>
    /// <param name="barcode">The barcode to load the state for.</param>
    /// <returns>A <see cref="BarcodeDbRecord"/> representing the barcode state.</returns>
    Task<BarcodeDbRecord> LoadBarcodeStateAsync(string barcode);

    /// <summary>
    /// Loads PLC address records asynchronously from the database.
    /// </summary>
    /// <returns>A dictionary mapping machine IDs to PLC records.</returns>
    Task<Dictionary<int, PlcRecordDb>> LoadPlcAddressAsync();

    /// <summary>
    /// Loads a static snapshot for a product by part number and machine ID asynchronously.
    /// </summary>
    /// <param name="partNumber">The part number of the product.</param>
    /// <param name="machineId">The machine ID to filter workflows.</param>
    /// <returns>A <see cref="FixtureStaticSnapshot"/> representing the static snapshot.</returns>
    Task<FixtureStaticSnapshot> LoadStaticSnapshotByPartNumberAsync(string partNumber, int machineId);
}
