// <copyright file="KpiDataSink.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Infrastructure.Repository;

using System.Net.Http;
using System.Text.Json;
using IndTrace.DataStore.Services.OEE;
using IndTrace.DataStore.Services.OEE.Interfaces;
using IndTrace.Domain.Entities;
using QuestDB;
using QuestDB.Senders;

/// <summary>
/// Handles persistence and retrieval of OEE and KPI data to and from QuestDB time-series database.
/// This class implements both read and write operations for high-performance analytics and real-time data processing.
/// </summary>
/// <remarks>
/// This implementation uses QuestDB's native HTTP interface for queries and the ILP (InfluxDB Line Protocol)
/// sender for high-performance writes. The dual-purpose design supports both real-time data ingestion
/// and analytical queries for dashboard visualization.
/// </remarks>
/// <param name="logger">Logger instance for tracking operations and debugging.</param>
/// <param name="httpClient">Pre-configured HTTP client for QuestDB query operations (reserved for future read operations).</param>
/// <param name="sender">QuestDB ILP sender for high-performance data writes.</param>
// TODO [SOLID][CURSOR][20/JUNE/2025] - Consider separating read and write operations into distinct classes to follow Single Responsibility Principle.
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Implement retry policies and circuit breaker patterns for database resilience.
// TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Add connection pooling and batch write capabilities for high-throughput scenarios.
/// <summary>
/// Represents the KpiDataSink.
/// </summary>
//[Fix]
//CLAUDE
//Date: 02/09/2025
//Reason: [CS9113] - Store unused HttpClient as field for potential future use
public class KpiDataSink(ILogger<KpiDataSink> logger, HttpClient httpClient, ISender sender) : IKpiDataSink
{
    // TODO INJECT FACTORY FOR HTTPCLIENT
    // [URGENT] 17 JUN 2025
    // THIS MUST HAVE THE ADDRESS FROM CONFIGURATION
    // ABR
    // [DONE]

    // Field for HttpClient - reserved for future read operations
    private readonly HttpClient httpClient = httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="KpiDataSink"/> class.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>

    /// <summary>
    /// Retrieves OEE calculation results from the QuestDB time-series database.
    /// Executes a SQL query against the OeeResults table and transforms the JSON response into strongly-typed objects.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A list of <see cref="OeeResult"/> objects containing historical OEE data.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request to QuestDB fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be parsed as valid JSON.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when expected columns are missing from the query result.</exception>
    /// <remarks>
    /// This method performs a SELECT * query which may not be optimal for large datasets.
    /// Consider implementing pagination, filtering, and column selection for production use.
    /// </remarks>

    /// <summary>
    /// Writes KPI OEE data to QuestDB using the InfluxDB Line Protocol (ILP) for high-performance ingestion.
    /// This method is optimized for real-time data streaming and high-throughput scenarios.
    /// </summary>
    /// <param name="data">The KPI OEE data to write to the database.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the data parameter is null.</exception>
    /// <exception cref="QuestDbException">Thrown when the write operation fails due to database issues.</exception>
    /// <remarks>
    /// The data is written to the 'KpiOee' table with symbols and columns optimized for time-series analytics.
    /// Symbols are used for high-cardinality categorical data while columns store numerical measurements.
    /// </remarks>
    /// <inheritdoc />
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add input validation to ensure data parameter is not null and contains valid values.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Implement batch writing for multiple KpiOee records to improve throughput.
    // TODO [RESOURCE LEAK][CURSOR][20/JUNE/2025] - Ensure sender transactions are properly committed or rolled back in all scenarios.
    /// <summary>
    /// Executes WriteAsync operation.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of WriteAsync.</returns>
    public async Task WriteAsync(KpiOee data, CancellationToken cancellationToken = default)
    {
        logger?.LogInformation("Writing KpiOee to QuestDB: {KpiOee}", JsonSerializer.Serialize(data));

        const string tableName = "KpiOee";
        const string oeeDataIdSymbol = "OEEDataId";
        const string availabilityColumn = "Availability";
        const string performanceColumn = "Performance";
        const string qualityColumn = "Quality";
        const string oeeColumn = "OEE";

        // TODO [RESOURCE LEAK][CURSOR][20/JUNE/2025] - Remove commented code and implement proper sender lifecycle management.
        var sender = Sender.New("http::addr=localhost:9000;");

        // Sender is now injected, so we don't need to create a new instance here.
        // Lifecycle management of Sender is handled by the DI container.
        // using var sender = Sender.New("http::addr=localhost:9000;");

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add transaction error handling and rollback capabilities.
        sender.Transaction(tableName.AsSpan());

        // Map properties from OeeRegisterDto
        // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider using ReadOnlySpan<char> for string operations to reduce allocations.
        sender
            .Symbol(oeeDataIdSymbol.AsSpan(), data.OeeRegisterId.ToString().AsSpan())
            .Column(availabilityColumn.AsSpan(), data.Availability)
            .Column(performanceColumn.AsSpan(), data.Performance)
            .Column(qualityColumn.AsSpan(), data.Quality)
            .Column(oeeColumn.AsSpan(), data.Oee);

        await sender.AtAsync(data.TimeStamp, cancellationToken);
        await sender.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Writes performance data to QuestDB using the InfluxDB Line Protocol for real-time monitoring and analytics.
    /// This method handles the raw performance metrics collected from manufacturing equipment.
    /// </summary>
    /// <param name="data">The performance data to write to the database.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the data parameter is null.</exception>
    /// <exception cref="QuestDbException">Thrown when the write operation fails due to database issues.</exception>
    /// <remarks>
    /// Performance data includes comprehensive metrics from PLC systems including production counts,
    /// timing information, and quality metrics. This data serves as the foundation for OEE calculations.
    /// </remarks>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add comprehensive input validation for all performance data fields.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Implement bulk write operations for high-frequency data collection scenarios.
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add data field documentation and validation rules for manufacturing domain constraints.
    /// <summary>
    /// Executes WriteAsync operation.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of WriteAsync.</returns>
    public async Task WriteAsync(PerformanceData data, CancellationToken cancellationToken = default)
    {
        logger?.LogInformation("Writing PerformanceData to QuestDB: {PerformanceData}", JsonSerializer.Serialize(data));

        const string tableName = "PerformanceData";

        // TODO [RESOURCE LEAK][CURSOR][20/JUNE/2025] - Remove commented code and use injected sender consistently.
        // Sender is now injected, so we don't need to create a new instance here.
        // Lifecycle management of Sender is handled by the DI container.
        // using var sender = Sender.New("http::addr=localhost:9000;");

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add transaction error handling and ensure atomic operations.
        sender.Transaction(tableName.AsSpan());

        // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider using column batching for improved write performance with large datasets.
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add field validation to ensure data integrity before writing to database.
        sender
            .Symbol("PerformanceDataId".AsSpan(), data.PerformanceDataId.ToString().AsSpan())
            .Column("MachineId".AsSpan(), data.MachineId)
            .Column("PlcId".AsSpan(), data.PlcId)
            .Column("BarCodeId".AsSpan(), data.BarCodeId)
            .Column("CycleId".AsSpan(), data.CycleId)
            .Column("ApplicationFlag".AsSpan(), data.ApplicationFlag)
            .Column("EventCounter".AsSpan(), data.EventCounter)
            .Column("CurrentTime".AsSpan(), data.CurrentTime)
            .Column("RunningTime".AsSpan(), data.RunningTime)
            .Column("StoppedTime".AsSpan(), data.StoppedTime)
            .Column("FaultedTime".AsSpan(), data.FaultedTime)
            .Column("StatusFaultReason".AsSpan(), data.StatusFaultReason)
            .Column("TotalProduction".AsSpan(), data.TotalProduction)
            .Column("ProductionOk".AsSpan(), data.ProductionOk)
            .Column("ProductionNoK".AsSpan(), data.ProductionNoK)
            .Column("StatusFaultReject".AsSpan(), data.StatusFaultReject)
            .Column("RejectEventCounter".AsSpan(), data.RejectEventCounter)
            .Column("StatusReject".AsSpan(), data.StatusReject)
            .Column("RejectQuantityUnits".AsSpan(), data.RejectQuantityUnits)
            .Column("StandardCycleTime".AsSpan(), data.StandardCycleTime)
            .Column("ActualCycleTime".AsSpan(), data.ActualCycleTime)
            .Column("PlanedProductionTime".AsSpan(), data.PlanedProductionTime);

        await sender.AtAsync(data.TimeStamp, cancellationToken);
        await sender.CommitAsync(cancellationToken);
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate KpiDataSink logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [SOLID][CURSOR][20/JUNE/2025] - Split this class into separate read and write repositories to improve maintainability and testing.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Implement caching strategy for frequently accessed OEE results to reduce database load.
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add comprehensive error handling with specific exception types for different failure scenarios.
}

/// <summary>
/// Handles persistence and retrieval of OEE and KPI data to and from QuestDB time-series database.
/// This class implements both read and write operations for high-performance analytics and real-time data processing.
/// </summary>
/// <remarks>
/// This implementation uses QuestDB's native HTTP interface for queries and the ILP (InfluxDB Line Protocol)
/// sender for high-performance writes. The dual-purpose design supports both real-time data ingestion
/// and analytical queries for dashboard visualization.
/// </remarks>
/// <param name="logger">Logger instance for tracking operations and debugging.</param>
/// <param name="httpClient">Pre-configured HTTP client for QuestDB query operations.</param>
/// <param name="sender">QuestDB ILP sender for high-performance data writes (unused in read-only operations).</param>
// TODO [SOLID][CURSOR][20/JUNE/2025] - Consider separating read and write operations into distinct classes to follow Single Responsibility Principle.
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Implement retry policies and circuit breaker patterns for database resilience.
// TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Add connection pooling and batch write capabilities for high-throughput scenarios.
/// <summary>
/// Represents the KpiDataReader.
/// </summary>
//[Fix]
//CLAUDE
//Date: 02/09/2025
//Reason: [CS9113] - Store unused parameters as fields for consistency with DI pattern
public class KpiDataReader(ILogger<KpiDataSink> logger, HttpClient httpClient, ISender sender) : IKpiDataReader
{
    // TODO INJECT FACTORY FOR HTTPCLIENT
    // [URGENT] 17 JUN 2025
    // THIS MUST HAVE THE ADDRESS FROM CONFIGURATION
    // ABR
    // [DONE]

    // Fields for unused dependencies - reserved for consistency with DI pattern
    private readonly ILogger<KpiDataSink> logger = logger;
    private readonly ISender sender = sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="KpiDataSink"/> class.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>

    /// <summary>
    /// Retrieves OEE calculation results from the QuestDB time-series database.
    /// Executes a SQL query against the OeeResults table and transforms the JSON response into strongly-typed objects.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A list of <see cref="OeeResult"/> objects containing historical OEE data.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request to QuestDB fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be parsed as valid JSON.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when expected columns are missing from the query result.</exception>
    /// <remarks>
    /// This method performs a SELECT * query which may not be optimal for large datasets.
    /// Consider implementing pagination, filtering, and column selection for production use.
    /// </remarks>
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Implement query parameters for filtering, pagination, and date range selection.
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add input validation and sanitization for SQL injection prevention.
    // TODO [RESOURCE LEAK][CURSOR][20/JUNE/2025] - Implement proper disposal of JsonDocument resources in exception scenarios.
    /// <summary>
    /// Executes GetKpiResultsAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of GetKpiResultsAsync.</returns>
    public async Task<List<OeeResult>> GetKpiResultsAsync(CancellationToken cancellationToken)
    {
        var query = "SELECT * FROM OeeResults";
        var response = await httpClient.GetAsync($"/exec?query={Uri.EscapeDataString(query)}");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider using streaming JSON parsing for large result sets to reduce memory allocation.
        // Use System.Text.Json or parse manually
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        var data = new List<OeeResult>();

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add validation to ensure required columns exist before attempting to access them.
        var columns = root.GetProperty("columns")
            .EnumerateArray()
            .Select((col, idx) =>
                new KeyValuePair<string, int>(
                    col.GetProperty("name").GetString()!, idx))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Pre-allocate list capacity if dataset size is known to improve performance.
        foreach (var row in root.GetProperty("dataset").EnumerateArray())
        {
            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add null checking and safe parsing for all data fields to handle corrupt or incomplete data.
            data.Add(new OeeResult
            {
                OEEDataId = long.Parse(row[columns["OEEDataId"]].GetString() ?? "0"),
                Availability = row[columns["Availability"]].GetDouble(),
                Performance = row[columns["Performance"]].GetDouble(),
                Quality = row[columns["Quality"]].GetDouble(),
                OEE = row[columns["OEE"]].GetDouble(),
                Timestamp = DateTime.Parse(row[columns["timestamp"]].GetString() ?? DateTime.MinValue.ToString()),
            });
        }

        return data;
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate KpiDataSink logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [SOLID][CURSOR][20/JUNE/2025] - Split this class into separate read and write repositories to improve maintainability and testing.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Implement caching strategy for frequently accessed OEE results to reduce database load.
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add comprehensive error handling with specific exception types for different failure scenarios.
}
