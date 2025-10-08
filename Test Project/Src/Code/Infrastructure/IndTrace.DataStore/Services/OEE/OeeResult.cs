namespace IndTrace.DataStore.Services.OEE;

/// <summary>
/// Represents the result of an OEE (Overall Equipment Effectiveness) calculation, including individual KPI metrics and timestamp.
/// This class serves as a data transfer object for OEE calculation results from QuestDB queries and real-time calculations.
/// </summary>
/// <remarks>
/// OEE is calculated as the product of three key performance indicators:
/// - Availability: Percentage of scheduled time that the equipment is available to operate
/// - Performance: Speed at which the equipment runs compared to its designed speed
/// - Quality: Percentage of good parts produced compared to total parts produced
///
/// The overall OEE value represents the overall effectiveness of the manufacturing process,
/// with values closer to 1.0 (100%) indicating better performance.
/// </remarks>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider implementing IValidatableObject to ensure OEE metrics are within valid ranges (0.0-1.0).
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add factory methods for creating OeeResult from different data sources (PerformanceData, KpiOee entities).
//TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider using record type for immutability and better performance in high-throughput scenarios.
/// <summary>
/// Represents the OeeResult.
/// </summary>
public class OeeResult
{
    /// <summary>
    /// Gets or sets the unique identifier for the OEE data record.
    /// This identifier correlates with the source data in the time-series database.
    /// </summary>
    /// <value>A unique long integer identifying this OEE calculation result.</value>
    public long OEEDataId { get; set; }

    /// <summary>
    /// Gets or sets the availability KPI value as a decimal between 0.0 and 1.0.
    /// Availability measures the percentage of scheduled time that the equipment is available to operate.
    /// </summary>
    /// <value>
    /// A double value representing availability where:
    /// - 0.0 = 0% availability (equipment never available)
    /// - 1.0 = 100% availability (equipment always available when scheduled).
    /// </value>
    /// <remarks>
    /// Calculated as: Running Time / Planned Production Time
    /// Low availability typically indicates equipment failures, setup issues, or unplanned maintenance.
    /// </remarks>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add range validation to ensure value is between 0.0 and 1.0.
    /// <summary>
    /// Gets or sets the Availability.
    /// </summary>
    public double Availability { get; set; }

    /// <summary>
    /// Gets or sets the performance KPI value as a decimal between 0.0 and typically 1.0 (can exceed 1.0 for over-performance).
    /// Performance measures the speed at which the equipment runs compared to its designed speed.
    /// </summary>
    /// <value>
    /// A double value representing performance where:
    /// - 0.0 = 0% performance (no production)
    /// - 1.0 = 100% performance (ideal speed achieved)
    /// - >1.0 = Over-performance (faster than designed speed).
    /// </value>
    /// <remarks>
    /// Calculated as: (Standard Cycle Time × Total Production) / Running Time
    /// Low performance indicates equipment running slower than designed capacity due to wear,
    /// material issues, or operator efficiency factors.
    /// </remarks>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add validation to ensure value is non-negative and document acceptable range for over-performance.
    /// <summary>
    /// Gets or sets the Performance.
    /// </summary>
    public double Performance { get; set; }

    /// <summary>
    /// Gets or sets the quality KPI value as a decimal between 0.0 and 1.0.
    /// Quality measures the percentage of good parts produced compared to total parts produced.
    /// </summary>
    /// <value>
    /// A double value representing quality where:
    /// - 0.0 = 0% quality (all parts defective)
    /// - 1.0 = 100% quality (no defective parts).
    /// </value>
    /// <remarks>
    /// Calculated as: Good Parts / Total Parts Produced
    /// Low quality indicates process issues, material problems, or equipment calibration issues.
    /// </remarks>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add range validation to ensure value is between 0.0 and 1.0.
    /// <summary>
    /// Gets or sets the Quality.
    /// </summary>
    public double Quality { get; set; }

    /// <summary>
    /// Gets or sets the overall OEE value as a decimal between 0.0 and 1.0.
    /// OEE represents the overall effectiveness of the manufacturing process.
    /// </summary>
    /// <value>
    /// A double value representing OEE where:
    /// - 0.0 = 0% effectiveness (no productive output)
    /// - 1.0 = 100% effectiveness (perfect performance)
    /// - Typical world-class OEE is 0.85 (85%) or higher.
    /// </value>
    /// <remarks>
    /// Calculated as: Availability × Performance × Quality
    /// Industry benchmarks:
    /// - World Class: 85%+
    /// - Good: 65-85%
    /// - Fair: 40-65%
    /// - Poor: &lt;40%.
    /// </remarks>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add computed property to categorize OEE performance level (WorldClass, Good, Fair, Poor).
    /// <summary>
    /// Gets or sets the OEE.
    /// </summary>
    public double OEE { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for when this OEE result was calculated or recorded.
    /// This timestamp is used for time-series analysis and historical trending.
    /// </summary>
    /// <value>A DateTime representing when this OEE calculation was performed.</value>
    /// <remarks>
    /// The timestamp should typically represent the end of the measurement period for the OEE calculation.
    /// For real-time calculations, this would be the current time when the calculation was performed.
    /// </remarks>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider using DateTimeOffset for timezone-aware timestamps in multi-location deployments.
    /// <summary>
    /// Gets or sets the Timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate OeeResult logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    //TODO [SOLID][CURSOR][20/JUNE/2025] - Consider adding equality comparison methods (IEquatable<OeeResult>) for better testability and comparison operations.
    //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Add ToString() override with formatted output for logging and debugging purposes.
}
