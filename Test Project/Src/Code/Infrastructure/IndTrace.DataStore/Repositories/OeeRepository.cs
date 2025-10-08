using IndTrace.Application.Oee.Queries;
using IndTrace.Application.Repository;
using IndTrace.Domain.ValueObjects;

namespace IndTrace.DataStore.Repositories;

/// <summary>
/// Basic implementation of the OEE repository for data storage operations.
/// </summary>
public class OeeRepository : IOeeRepository
{
    private readonly ILogger<OeeRepository> logger;
    private readonly List<OeeStoredRecord> storage; // In-memory storage for now

    /// <summary>
    /// Initializes a new instance of the <see cref="OeeRepository"/> class.
    /// Initializes a new instance of the OeeRepository.
    /// </summary>
    /// <param name="logger">Logger for diagnostic information.</param>
    public OeeRepository(ILogger<OeeRepository> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.storage = [];
    }

    /// <summary>
    /// Retrieves OEE history records based on the specified criteria.
    /// </summary>
    /// <param name="machineId">Machine identifier (optional, null means all machines).</param>
    /// <param name="startDate">Start date for the history range.</param>
    /// <param name="endDate">End date for the history range.</param>
    /// <param name="minPerformanceLevel">Minimum performance level filter (optional).</param>
    /// <param name="pageNumber">Page number for pagination (1-based).</param>
    /// <param name="pageSize">Page size for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the OEE history records and total count.</returns>
    public Task<(IEnumerable<OeeHistoryRecord> Records, int TotalCount)> GetOeeHistoryAsync(
        int? machineId,
        DateTime startDate,
        DateTime endDate,
        OeePerformanceLevel? minPerformanceLevel,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        this.logger.LogDebug(
            "Getting OEE history for Machine {MachineId} from {StartDate} to {EndDate}",
            machineId, startDate, endDate);

        var query = this.storage.AsQueryable();

        // Apply filters
        query = query.Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate);

        if (machineId.HasValue)
        {
            query = query.Where(r => r.MachineId == machineId.Value);
        }

        if (minPerformanceLevel.HasValue)
        {
            query = query.Where(r => r.Metrics.PerformanceLevel >= minPerformanceLevel.Value);
        }

        var totalCount = query.Count();

        // Apply pagination
        var records = query
            .OrderByDescending(r => r.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new OeeHistoryRecord
            {
                MachineId = r.MachineId,
                MachineName = r.MachineName,
                Metrics = r.Metrics,
                Timestamp = r.Timestamp,
                Shift = r.Shift,
                Product = r.Product,
            })
            .ToList();

        this.logger.LogDebug(
            "Retrieved {RecordCount} OEE history records (total: {TotalCount})",
            records.Count, totalCount);

        return Task.FromResult<(IEnumerable<OeeHistoryRecord>, int)>((records, totalCount));
    }

    /// <summary>
    /// Stores OEE calculation results.
    /// </summary>
    /// <param name="machineId">Machine identifier.</param>
    /// <param name="metrics">OEE metrics to store.</param>
    /// <param name="timestamp">Calculation timestamp.</param>
    /// <param name="shift">Shift information (optional).</param>
    /// <param name="product">Product information (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stored record identifier.</returns>
    public Task<long> StoreOeeCalculationAsync(
        int machineId,
        OeeMetrics metrics,
        DateTime timestamp,
        string? shift = null,
        string? product = null,
        CancellationToken cancellationToken = default)
    {
        this.logger.LogDebug("Storing OEE calculation for Machine {MachineId}", machineId);

        var record = new OeeStoredRecord
        {
            Id = this.storage.Count + 1,
            MachineId = machineId,
            MachineName = GetMachineName(machineId),
            Metrics = metrics,
            Timestamp = timestamp,
            Shift = shift,
            Product = product,
        };

        this.storage.Add(record);

        this.logger.LogDebug(
            "Successfully stored OEE calculation for Machine {MachineId} with ID {RecordId}",
            machineId, record.Id);

        return Task.FromResult(record.Id);
    }

    /// <summary>
    /// Retrieves the latest OEE metrics for a specific machine.
    /// </summary>
    /// <param name="machineId">Machine identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The latest OEE record for the machine, or null if not found.</returns>
    public Task<OeeHistoryRecord?> GetLatestOeeAsync(
        int machineId,
        CancellationToken cancellationToken = default)
    {
        this.logger.LogDebug("Getting latest OEE for Machine {MachineId}", machineId);

        var latestRecord = this.storage
            .Where(r => r.MachineId == machineId)
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefault();

        if (latestRecord == null)
        {
            this.logger.LogDebug("No OEE records found for Machine {MachineId}", machineId);
            return Task.FromResult<OeeHistoryRecord?>(null);
        }

        var result = new OeeHistoryRecord
        {
            MachineId = latestRecord.MachineId,
            MachineName = latestRecord.MachineName,
            Metrics = latestRecord.Metrics,
            Timestamp = latestRecord.Timestamp,
            Shift = latestRecord.Shift,
            Product = latestRecord.Product,
        };

        return Task.FromResult<OeeHistoryRecord?>(result);
    }

    /// <summary>
    /// Retrieves OEE summary statistics for machines within a date range.
    /// </summary>
    /// <param name="machineIds">Machine identifiers (optional, null means all machines).</param>
    /// <param name="startDate">Start date for the summary range.</param>
    /// <param name="endDate">End date for the summary range.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OEE summary statistics.</returns>
    public Task<IEnumerable<OeeSummaryRecord>> GetOeeSummaryAsync(
        IEnumerable<int>? machineIds,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        this.logger.LogDebug("Getting OEE summary from {StartDate} to {EndDate}", startDate, endDate);

        var query = this.storage
            .Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate);

        if (machineIds != null && machineIds.Any())
        {
            var machineIdSet = machineIds.ToHashSet();
            query = query.Where(r => machineIdSet.Contains(r.MachineId));
        }

        var summaries = query
            .GroupBy(r => new { r.MachineId, r.MachineName })
            .Select(g => new OeeSummaryRecord
            {
                MachineId = g.Key.MachineId,
                MachineName = g.Key.MachineName,
                AverageAvailability = g.Average(r => r.Metrics.Availability),
                AveragePerformance = g.Average(r => r.Metrics.Performance),
                AverageQuality = g.Average(r => r.Metrics.Quality),
                AverageOee = g.Average(r => r.Metrics.Oee),
                MinOee = g.Min(r => r.Metrics.Oee),
                MaxOee = g.Max(r => r.Metrics.Oee),
                CalculationCount = g.Count(),
                DominantPerformanceLevel = GetDominantPerformanceLevel(g.Select(r => r.Metrics.PerformanceLevel)),
                StartDate = startDate,
                EndDate = endDate,
            })
            .OrderBy(s => s.MachineId)
            .ToList();

        this.logger.LogDebug("Retrieved {SummaryCount} OEE summary records", summaries.Count);

        return Task.FromResult<IEnumerable<OeeSummaryRecord>>(summaries);
    }

    /// <summary>
    /// Deletes OEE records older than the specified date.
    /// </summary>
    /// <param name="cutoffDate">Date before which records should be deleted.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of records deleted.</returns>
    public Task<int> DeleteOldRecordsAsync(
        DateTime cutoffDate,
        CancellationToken cancellationToken = default)
    {
        this.logger.LogInformation("Deleting OEE records older than {CutoffDate}", cutoffDate);

        var recordsToDelete = this.storage.Where(r => r.Timestamp < cutoffDate).ToList();

        foreach (var record in recordsToDelete)
        {
            this.storage.Remove(record);
        }

        this.logger.LogInformation("Deleted {DeletedCount} old OEE records", recordsToDelete.Count);

        return Task.FromResult(recordsToDelete.Count);
    }

    /// <summary>
    /// Gets the machine name for a given machine ID.
    /// </summary>
    /// <param name="machineId">Machine identifier.</param>
    /// <returns>Machine name or a default name.</returns>
    private static string GetMachineName(int machineId)
    {
        // TODO: This should ideally lookup from a machines table
        // For now, return a default name pattern
        return $"Machine_{machineId}";
    }

    /// <summary>
    /// Determines the dominant performance level from a collection of levels.
    /// </summary>
    /// <param name="levels">Collection of performance levels.</param>
    /// <returns>The most frequently occurring performance level.</returns>
    private static OeePerformanceLevel GetDominantPerformanceLevel(IEnumerable<OeePerformanceLevel> levels)
    {
        return levels
            .GroupBy(l => l)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }
}

/// <summary>
/// Internal storage record for OEE data.
/// </summary>
internal record OeeStoredRecord
{
    /// <summary>
    /// Gets the record identifier.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Gets the machine identifier.
    /// </summary>
    public int MachineId { get; init; }

    /// <summary>
    /// Gets the machine name.
    /// </summary>
    public string MachineName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the OEE metrics.
    /// </summary>
    public OeeMetrics Metrics { get; init; } = new(0, 0, 0);

    /// <summary>
    /// Gets the calculation timestamp.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the shift information.
    /// </summary>
    public string? Shift { get; init; }

    /// <summary>
    /// Gets the product information.
    /// </summary>
    public string? Product { get; init; }
}
