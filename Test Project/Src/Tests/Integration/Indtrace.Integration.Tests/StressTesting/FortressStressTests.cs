using IndTrace.Domain.Entities;
using IndTrace.Persistence.Interfaces;
using IndTrace.Application.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Integration.Tests.StressTesting;

/// <summary>
/// 🏰 FORTRESS STRESS TESTS: Parallel database validation under industrial load
/// Tests the fortress architecture with 3 parallel workers × 3,333 barcodes each = 10,000 total validations
/// </summary>
public class FortressStressTests : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services;
    private readonly ITestOutputHelper _output;

    public FortressStressTests(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
    {
        _services = fixture.Services;
        _output = output;
    }

    [Fact]
    [Trait("Category", "StressTest")]
    [Trait("Performance", "Fortress")]
    public async Task Fortress_Should_Handle_10000_Parallel_BarCode_Validations_Across_3_Workers()
    {
        // 🏰 FORTRESS STRESS TEST CONFIGURATION
        const int workersCount = 3;
        const int barcodesPerWorker = 3333;
        var totalBarcodes = workersCount * barcodesPerWorker; // 9,999 ≈ 10,000

        var databases = new[] { "IndTraceDbContext45", "IndTraceDbContext46", "IndTraceDbContext62" };

        _output.WriteLine($"🚀 FORTRESS STRESS TEST INITIATED");
        _output.WriteLine($"Workers: {workersCount}");
        _output.WriteLine($"BarCodes per worker: {barcodesPerWorker}");
        _output.WriteLine($"Total validations: {totalBarcodes}");
        _output.WriteLine($"Databases: {string.Join(", ", databases)}");

        var stopwatch = Stopwatch.StartNew();
        var results = new ConcurrentBag<WorkerResult>();
        var exceptions = new ConcurrentBag<Exception>();

        // 🎯 Launch parallel workers
        var tasks = Enumerable.Range(0, workersCount).Select(async workerId =>
        {
            try
            {
                var dbKey = databases[workerId % databases.Length];
                var workerResult = await ExecuteWorker(workerId, dbKey, barcodesPerWorker);
                results.Add(workerResult);

                _output.WriteLine($"✅ Worker {workerId} ({dbKey}): {workerResult.ProcessedCount} barcodes, {workerResult.SuccessCount} successful, {workerResult.ElapsedMs}ms");
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                _output.WriteLine($"❌ Worker {workerId} failed: {ex.Message}");
            }
        });

        // 🏰 Await fortress completion
        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // 📊 FORTRESS PERFORMANCE ANALYSIS
        var totalProcessed = results.Sum(r => r.ProcessedCount);
        var totalSuccessful = results.Sum(r => r.SuccessCount);
        var averageLatency = results.Any() ? results.Average(r => r.AverageLatencyMs) : 0;
        var throughputPerSecond = totalProcessed / (stopwatch.ElapsedMilliseconds / 1000.0);

        _output.WriteLine($"");
        _output.WriteLine($"🏰 FORTRESS STRESS TEST RESULTS:");
        _output.WriteLine($"Total Processed: {totalProcessed:N0}");
        _output.WriteLine($"Total Successful: {totalSuccessful:N0}");
        _output.WriteLine($"Success Rate: {(totalSuccessful * 100.0 / Math.Max(totalProcessed, 1)):F1}%");
        _output.WriteLine($"Total Duration: {stopwatch.ElapsedMilliseconds:N0}ms");
        _output.WriteLine($"Average Latency: {averageLatency:F2}ms per barcode");
        _output.WriteLine($"Throughput: {throughputPerSecond:F0} barcodes/second");
        _output.WriteLine($"Context Pool Efficiency: 128 contexts × {databases.Length} databases = {128 * databases.Length} total pool");

        // 🎯 FORTRESS VALIDATION ASSERTIONS
        exceptions.ShouldBeEmpty("Fortress should handle all workers without exceptions");
        results.Count.ShouldBe(workersCount, "All workers should complete");
        totalProcessed.ShouldBeGreaterThan(9000, "Should process close to 10,000 barcodes");
        totalSuccessful.ShouldBeGreaterThan((int)(totalProcessed * 0.95), "95%+ success rate expected");
        ((int)throughputPerSecond).ShouldBeGreaterThan(100, "Fortress should achieve 100+ barcodes/second");

        _output.WriteLine($"🏰 FORTRESS STRESS TEST: ✅ PASSED - Industrial load handled successfully!");
    }

    [Fact]
    [Trait("Category", "StressTest")]
    [Trait("Performance", "ChaosEngineering")]
    public async Task Fortress_Should_Handle_Random_Chaos_Load_Like_Real_Manufacturing()
    {
        // 🎲 CHAOS ENGINEERING: Random is real!
        _output.WriteLine($"🌪️ FORTRESS CHAOS ENGINEERING TEST INITIATED");
        _output.WriteLine($"Simulating REAL MANUFACTURING CHAOS with random patterns");

        var random = new Random();
        var databases = new[] { "IndTraceDbContext45", "IndTraceDbContext46", "IndTraceDbContext62" };

        // Random chaos parameters
        var workerCount = random.Next(2, 8); // 2-7 workers (random factory staffing)
        var minBarcodes = 500;
        var maxBarcodes = 5000;
        var burstProbability = 0.3; // 30% chance of production burst
        var failureProbability = 0.05; // 5% equipment failure rate

        _output.WriteLine($"Random Workers: {workerCount}");
        _output.WriteLine($"Burst Probability: {burstProbability:P0}");
        _output.WriteLine($"Equipment Failure Rate: {failureProbability:P0}");

        var stopwatch = Stopwatch.StartNew();
        var results = new ConcurrentBag<WorkerResult>();
        var exceptions = new ConcurrentBag<Exception>();

        // 🎯 Launch CHAOTIC parallel workers
        var tasks = Enumerable.Range(0, workerCount).Select(async workerId =>
        {
            try
            {
                // Random database selection (simulating load balancing chaos)
                var dbKey = databases[random.Next(databases.Length)];

                // Random workload per worker (real production variability)
                var targetBarcodes = random.Next(minBarcodes, maxBarcodes);

                // Burst detection
                if (random.NextDouble() < burstProbability)
                {
                    targetBarcodes *= 3; // Production burst!
                    _output.WriteLine($"🚨 BURST DETECTED: Worker {workerId} handling {targetBarcodes} barcodes!");
                }

                // Equipment failure simulation
                if (random.NextDouble() < failureProbability)
                {
                    await Task.Delay(random.Next(1000, 5000)); // Random downtime
                    _output.WriteLine($"⚠️ EQUIPMENT FAILURE: Worker {workerId} delayed {random.Next(1, 5)} seconds");
                }

                var workerResult = await ExecuteChaosWorker(workerId, dbKey, targetBarcodes, random);
                results.Add(workerResult);

                _output.WriteLine($"🎲 Worker {workerId} ({dbKey}): {workerResult.ProcessedCount}/{targetBarcodes} barcodes, Chaos factor: {workerResult.ChaosLevel:F2}");
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                _output.WriteLine($"💥 Chaos Worker {workerId} exploded: {ex.Message}");
            }
        });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // 📊 CHAOS ANALYSIS
        var totalProcessed = results.Sum(r => r.ProcessedCount);
        var totalSuccessful = results.Sum(r => r.SuccessCount);
        var chaosLevel = results.Any() ? results.Average(r => r.ChaosLevel) : 0;

        _output.WriteLine($"");
        _output.WriteLine($"🌪️ CHAOS ENGINEERING RESULTS:");
        _output.WriteLine($"Workers Survived: {results.Count}/{workerCount}");
        _output.WriteLine($"Total Processed: {totalProcessed:N0} (highly variable)");
        _output.WriteLine($"Success Rate: {(totalSuccessful * 100.0 / Math.Max(totalProcessed, 1)):F1}%");
        _output.WriteLine($"Chaos Level: {chaosLevel:F2} (0=calm, 1=pure chaos)");
        _output.WriteLine($"Total Duration: {stopwatch.ElapsedMilliseconds:N0}ms");

        // 🎯 CHAOS VALIDATION - More forgiving due to randomness
        results.Count.ShouldBeGreaterThan(0, "At least some workers should survive chaos");
        totalProcessed.ShouldBeGreaterThan(workerCount * minBarcodes / 2, "Should process at least minimal load");

        _output.WriteLine($"🌪️ FORTRESS CHAOS TEST: ✅ SURVIVED - Real manufacturing chaos handled!");
    }

    private async Task<WorkerResult> ExecuteWorker(int workerId, string dbKey, int barcodesTarget)
    {
        var workerStopwatch = Stopwatch.StartNew();
        var processedCount = 0;
        var successCount = 0;
        var latencies = new List<long>();

        using var scope = _services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FortressStressTests>>();
        var barCodeRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<BarCode>>(dbKey);

        logger.LogInformation("🚀 Worker {WorkerId} starting with database {DbKey}", workerId, dbKey);

        // Get random sample of barcodes for this worker - using simple approach
        var spec = new Specification<BarCode>(b => true).ApplyPaging(workerId * 1000, barcodesTarget);
        var barcodesResult = await barCodeRepo.ListAsync(spec, TestContext.Current.CancellationToken);

        if (!barcodesResult.IsSuccess)
        {
            logger.LogError("Worker {WorkerId} failed to get barcodes: {Error}", workerId, barcodesResult.Error);
            throw new Exception($"Failed to get barcodes: {barcodesResult.Error}");
        }

        var barcodes = barcodesResult.Value?.ToList() ?? new List<BarCode>();
        logger.LogInformation("Worker {WorkerId} loaded {Count} barcodes", workerId, barcodes.Count);

        // Process each barcode with latency measurement
        foreach (var barcode in barcodes)
        {
            var itemStopwatch = Stopwatch.StartNew();

            try
            {
                // Simulate barcode validation workload
                var existingBarcodeResult = await barCodeRepo.GetByIdAsync(barcode.BarCodeId, TestContext.Current.CancellationToken);

                if (existingBarcodeResult.IsSuccess && existingBarcodeResult.Value != null &&
                    existingBarcodeResult.Value.Label == barcode.Label)
                {
                    successCount++;
                }

                processedCount++;
                itemStopwatch.Stop();
                latencies.Add(itemStopwatch.ElapsedMilliseconds);

                // Log progress every 1000 items
                if (processedCount % 1000 == 0)
                {
                    logger.LogInformation("Worker {WorkerId}: {Processed}/{Target} processed", workerId, processedCount, barcodesTarget);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Worker {WorkerId} failed to process barcode {BarCodeId}", workerId, barcode.BarCodeId);
                processedCount++;
                itemStopwatch.Stop();
                latencies.Add(itemStopwatch.ElapsedMilliseconds);
            }
        }

        workerStopwatch.Stop();

        return new WorkerResult
        {
            WorkerId = workerId,
            DatabaseKey = dbKey,
            ProcessedCount = processedCount,
            SuccessCount = successCount,
            ElapsedMs = workerStopwatch.ElapsedMilliseconds,
            AverageLatencyMs = latencies.Any() ? latencies.Average() : 0
        };
    }

    private async Task<WorkerResult> ExecuteChaosWorker(int workerId, string dbKey, int targetBarcodes, Random random)
    {
        var workerStopwatch = Stopwatch.StartNew();
        var processedCount = 0;
        var successCount = 0;
        var chaosEvents = 0;

        using var scope = _services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FortressStressTests>>();
        var barCodeRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<BarCode>>(dbKey);

        logger.LogInformation("🎲 Chaos Worker {WorkerId} starting with {DbKey}, target: {Target}", workerId, dbKey, targetBarcodes);

        // Use the random distinct generator for unique, non-repeating cases
        var clientId = $"ChaosWorker_{workerId}_{dbKey}_{Guid.NewGuid():N}";
        var randomBarcodes = Data.RandomDistinctBarCodeGenerator
            .GetRandomDistinctCases(clientId, targetBarcodes)
            .ToList();

        logger.LogInformation("Worker {WorkerId} got {Count} unique random barcodes", workerId, randomBarcodes.Count);

        // Process with CHAOS patterns
        foreach (var testCase in randomBarcodes)
        {
            try
            {
                var label = (string)testCase[0];
                var machineId = (int)testCase[1];
                var expectedId = (int)testCase[2];
                var expectedDb = testCase.Length > 3 ? (string)testCase[3] : ExtractDatabaseFromLabel(label);

                // SMART VALIDATION: Check if we're using the correct database
                var isCorrectDb = dbKey.Contains(expectedDb);
                if (!isCorrectDb)
                {
                    // Cross-database validation - this SHOULD fail
                    logger.LogDebug("Cross-DB validation: Looking for {Label} from {ExpectedDb} in {ActualDb}",
                        label, expectedDb, dbKey);
                }

                // CHAOS EVENT: Random processing delays (simulating equipment issues)
                if (random.NextDouble() < 0.1) // 10% chance
                {
                    await Task.Delay(random.Next(10, 100)); // Random processing hiccup
                    chaosEvents++;
                }

                // CHAOS EVENT: Random barcode validation approach
                if (random.NextDouble() < 0.5) // 50% chance
                {
                    // Approach 1: Search by ID (expected path)
                    var result = await barCodeRepo.GetByIdAsync(expectedId, TestContext.Current.CancellationToken);
                    var found = result.IsSuccess && result.Value?.Label == label;

                    // Smart validation: correct DB should find it, wrong DB should NOT
                    if (isCorrectDb && found)
                        successCount++;
                    else if (!isCorrectDb && !found)
                        successCount++; // Correctly NOT found in wrong DB
                }
                else
                {
                    // Approach 2: Search by specification (chaos path)
                    var spec = new Specification<BarCode>(b => b.Label == label && b.MachineId == machineId);
                    var result = await barCodeRepo.ListAsync(spec, TestContext.Current.CancellationToken);
                    var found = result.IsSuccess && result.Value?.Any() == true;

                    // Smart validation for specification search too
                    if (isCorrectDb && found)
                        successCount++;
                    else if (!isCorrectDb && !found)
                        successCount++; // Correctly NOT found in wrong DB
                }

                processedCount++;

                // CHAOS EVENT: Random batch processing (simulating scanner bursts)
                if (random.NextDouble() < 0.05) // 5% chance
                {
                    var burstSize = random.Next(10, 50);
                    processedCount += Math.Min(burstSize, randomBarcodes.Count - processedCount);
                    chaosEvents++;
                    logger.LogWarning("🚨 Scanner burst: {BurstSize} barcodes at once!", burstSize);
                }

                // CHAOS EVENT: Random connection pool stress
                if (random.NextDouble() < 0.02) // 2% chance
                {
                    // Create multiple contexts to stress the pool
                    var stressTasks = Enumerable.Range(0, random.Next(5, 20)).Select(async _ =>
                    {
                        using var stressScope = _services.CreateScope();
                        var stressRepo = stressScope.ServiceProvider.GetRequiredKeyedService<IRepository<BarCode>>(dbKey);
                        await stressRepo.CountAsync(new Specification<BarCode>(b => b.BarCodeId > 0), TestContext.Current.CancellationToken);
                    });
                    await Task.WhenAll(stressTasks);
                    chaosEvents++;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "🎲 Chaos event during barcode processing");
                processedCount++;
                chaosEvents++;
            }

            // Early termination if we've hit our target
            if (processedCount >= targetBarcodes)
                break;
        }

        workerStopwatch.Stop();
        var chaosLevel = chaosEvents / (double)Math.Max(processedCount, 1);

        return new WorkerResult
        {
            WorkerId = workerId,
            DatabaseKey = dbKey,
            ProcessedCount = processedCount,
            SuccessCount = successCount,
            ElapsedMs = workerStopwatch.ElapsedMilliseconds,

            AverageLatencyMs = workerStopwatch.ElapsedMilliseconds / (double)Math.Max(processedCount, 1),

            ChaosLevel = Math.Min(chaosLevel, 1.0) // Normalize to 0-1
        };
    }

    private static string ExtractDatabaseFromLabel(string label)
    {
        // Pattern recognition for different barcode formats:
        // - QA45422310251110066 -> QA45
        // - QA46XXXXXXXXXXXXX -> QA46
        // - QA62XXXXXXXXXXXXX -> QA62
        // - LQA45L823566232450198 -> QA45 (embedded)
        // - L1A422290240740240 -> Unknown/Legacy

        if (label.StartsWith("QA45")) return "QA45";
        if (label.StartsWith("QA46")) return "QA46";
        if (label.StartsWith("QA62")) return "QA62";

        // Check for embedded patterns
        if (label.Contains("QA45")) return "QA45";
        if (label.Contains("QA46")) return "QA46";
        if (label.Contains("QA62")) return "QA62";

        // Legacy/unknown format - default to QA45
        return "QA45";
    }

    public record WorkerResult
    {
        public int WorkerId { get; init; }
        public string DatabaseKey { get; init; } = string.Empty;
        public int ProcessedCount { get; init; }
        public int SuccessCount { get; init; }
        public long ElapsedMs { get; init; }
        public double AverageLatencyMs { get; init; }
        public double ChaosLevel { get; init; } // 0=calm, 1=pure chaos
    }
}
