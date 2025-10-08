// <copyright file="OeeChannelProcessor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Performance.Request.Command.Create;

namespace IndTrace.OEE.Infrastructure.Channels;

using System.Threading.Channels;
using IndTrace.Domain.Entities;
using MudBlazor;

/// <summary>
/// Processes OEE register data from an input channel, performs OEE calculations, and writes results to an output channel.
/// This class implements a high-performance, asynchronous data transformation pipeline for real-time OEE monitoring.
/// </summary>
/// <remarks>
/// The processor operates as an intermediary between raw performance data collection and OEE result distribution.
/// It uses .NET channels for efficient async enumeration and backpressure handling, ensuring the system can
/// handle high-frequency data streams without blocking or losing data.
///
/// Processing Flow:
/// 1. Reads PerformanceData from input channel
/// 2. Creates OeeRegister with calculated metrics
/// 3. Performs OEE calculations using domain logic
/// 4. Converts results to DTOs for serialization
/// 5. Writes OeeRegisterDto to output channel for consumption.
/// </remarks>
/// <param name="inputChannel">Channel containing raw performance data from PLC systems.</param>
/// <param name="outputChannel">Channel for processed OEE register DTOs ready for persistence or distribution.</param>
/// <param name="logger">Logger instance for tracking processing operations and errors.</param>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Implement error handling strategy with dead letter queue for failed calculations.
// TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Add metrics collection for processing throughput and latency monitoring.
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider implementing batch processing for improved performance with high data volumes.
/// <summary>
/// Represents the OeeChannelProcessor.
/// </summary>
public class OeeChannelProcessor(Channel<PerformanceData> inputChannel, Channel<OeeRegisterDto> outputChannel, ILogger<OeeChannelProcessor> logger) : IOeeChannelProcessor
{
    /// <summary>
    /// Processes OEE register data asynchronously by reading from the input channel, calculating OEE metrics,
    /// and writing transformed DTOs to the output channel for downstream consumption.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests during processing.</param>
    /// <returns>A task representing the asynchronous processing operation that continues until cancellation is requested.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="InvalidOperationException">Thrown when channel operations fail due to channel closure or configuration issues.</exception>
    /// <remarks>
    /// This method runs continuously until cancellation is requested, processing each incoming PerformanceData item.
    ///
    /// Processing Steps:
    /// 1. Reads PerformanceData from input channel using async enumeration
    /// 2. Creates OeeRegister entity with performance metrics
    /// 3. Invokes OEE calculation using domain business logic
    /// 4. Handles calculation results, warnings, and errors appropriately
    /// 5. Converts successful results to DTOs and writes to output channel
    /// 6. Logs warnings and errors for monitoring and debugging
    ///
    /// The processor handles partial failures gracefully - calculation errors for individual items
    /// do not stop the overall processing pipeline, ensuring system resilience.
    /// </remarks>
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add comprehensive error handling with specific exception types for different failure scenarios.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Implement parallel processing for CPU-intensive OEE calculations when applicable.
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add input validation to ensure PerformanceData contains required fields before processing.
    /// <summary>
    /// Executes ProcessOeeRegisterAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ProcessOeeRegisterAsync.</returns>
    public async Task ProcessOeeRegisterAsync(CancellationToken cancellationToken = default)
    {
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate OeeChannelProcessor logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

        // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider using ConfigureAwait(false) for library code to avoid deadlocks.
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add structured logging with correlation IDs for tracking individual data items through the pipeline.
        await foreach (var data in inputChannel.Reader.ReadAllAsync(cancellationToken))
        {
            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add null checking and validation for incoming performance data.
            // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider object pooling for OeeRegister instances to reduce GC pressure in high-throughput scenarios.
            var register = new OeeRegister
            {
                MachineId = data.MachineId,
                PlcId = data.PlcId,
                TimeStamp = data.TimeStamp,
                ActualCycleTime = data.ActualCycleTime,
                StandardCycleTime = data.StandardCycleTime,
                PlanedProductionTime = data.PlanedProductionTime,
                ProductId = data.BarCodeId,
            };

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add try-catch around OEE calculation to handle domain logic exceptions gracefully.
            // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Monitor calculation performance and add metrics for slow calculations that may indicate data quality issues.
            var result = OeeRegister.CalculateOee(register, data);

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Improve result handling logic - consider separate handling for warnings vs errors.
            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add structured logging with machine/PLC context for better troubleshooting.
            if ((result.IsSuccess || result.HasWarnings) && result.Value is not null)
            {
                // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider async DTO conversion if it involves heavy computation or I/O operations.
                // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add validation of DTO conversion to ensure data integrity before writing to output channel.
                var dto = OeeRegisterDto.ToDto(result.Value);

                // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add error handling for channel write operations in case output channel is closed or full.
                // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Consider using TryWrite with timeout for better backpressure handling.
                if (dto.Value is not null)
                {
                    await outputChannel.Writer.WriteAsync(dto.Value, cancellationToken);
                }
                else
                {
                    logger.LogWarning(
                        "OEE DTO conversion resulted in null value for Machine {MachineId}, Plc {PlcId}",
                        data.MachineId, data.PlcId);
                }
            }

            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Enhance error logging with more context about the specific data that caused issues.
            // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider implementing different log levels based on error severity (warnings vs critical errors).
            if (result.Errors is not null)
            {
                logger.LogWarning(
                    "OEE calculation returned warnings or errors for Machine {MachineId}, Plc {PlcId}. Messages: {Messages}",
                    data.MachineId, data.PlcId, string.Join(" | ", result.Errors));
            }
        }

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Add cleanup logic and final logging when processing completes or is cancelled.
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Consider implementing graceful shutdown with proper channel completion signaling.
    }
}
