namespace Integration.Tests.Data;

using System.Collections.Concurrent;

/// <summary>
/// Provides random, non-repeating barcode test cases for stress testing.
/// Each client gets unique random order with no duplicates.
/// </summary>
public static class RandomDistinctBarCodeGenerator
{
    // Thread-safe tracking of used indices per client
    private static readonly ConcurrentDictionary<string, ClientTracker> _clientTrackers = new();

    // Your 9,999 test cases loaded once and shared
    private static readonly List<object[]> _allTestCases = LoadAllTestCases();

    /// <summary>
    /// Creates a random, non-repeating enumerator for a specific client.
    /// Yields objects in random order but each object only once per client.
    /// </summary>
    /// <param name="clientId">Unique identifier for the client/worker</param>
    /// <param name="maxItems">Maximum items to yield (optional, defaults to all)</param>
    public static IEnumerable<object[]> GetRandomDistinctCases(string clientId, int? maxItems = null)
    {
        var tracker = _clientTrackers.GetOrAdd(clientId, _ => new ClientTracker());
        var totalCases = _allTestCases.Count;
        var targetCount = maxItems ?? totalCases;

        // Use client-specific random seed for different sequences per client
        var random = new Random(clientId.GetHashCode() + tracker.Iteration);

        // Fisher-Yates shuffle for true randomness
        var indices = Enumerable.Range(0, totalCases).ToList();

        lock (tracker)
        {
            // Reset if we've exhausted all cases
            if (tracker.UsedCount >= totalCases)
            {
                tracker.Reset();
            }

            // Shuffle remaining indices
            var remainingIndices = indices.Where(i => !tracker.UsedIndices.Contains(i)).ToList();
            ShuffleList(remainingIndices, random);

            // Yield up to targetCount items
            var yielded = 0;
            foreach (var index in remainingIndices)
            {
                if (yielded >= targetCount)
                    break;

                tracker.UsedIndices.Add(index);
                tracker.UsedCount++;
                yielded++;

                yield return _allTestCases[index];
            }

            // Track iteration for different random sequences
            tracker.Iteration++;
        }
    }

    /// <summary>
    /// Get a repeatable random sequence for deterministic tests
    /// </summary>
    public static IEnumerable<object[]> GetDeterministicRandomCases(int seed, int count)
    {
        var random = new Random(seed);
        var indices = Enumerable.Range(0, _allTestCases.Count).ToList();
        ShuffleList(indices, random);

        return indices.Take(count).Select(i => _allTestCases[i]);
    }

    /// <summary>
    /// Reset tracking for a specific client (useful for test cleanup)
    /// </summary>
    public static void ResetClient(string clientId)
    {
        _clientTrackers.TryRemove(clientId, out _);
    }

    /// <summary>
    /// Get statistics about case usage across all clients
    /// </summary>
    public static Dictionary<string, ClientStats> GetUsageStats()
    {
        return _clientTrackers.ToDictionary(
            kvp => kvp.Key,
            kvp => new ClientStats
            {
                UsedCount = kvp.Value.UsedCount,
                TotalAvailable = _allTestCases.Count,
                PercentageUsed = (kvp.Value.UsedCount * 100.0) / _allTestCases.Count,
                Iterations = kvp.Value.Iteration
            }
        );
    }

    /// <summary>
    /// Fisher-Yates shuffle algorithm for true randomness
    /// </summary>
    private static void ShuffleList<T>(IList<T> list, Random random)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    private static List<object[]> LoadAllTestCases()
    {
        var allCases = new List<object[]>();

        // Load from your existing data sources
        allCases.AddRange(BarCodeCases_Db45.Cases);
        allCases.AddRange(BarCodeCases_QA46.Cases);
        allCases.AddRange(BarCodeCases_QA62.Cases);

        // Add provenance if not already present
        var casesWithProvenance = allCases.Select(testCase =>
        {
            if (testCase.Length >= 4) return testCase; // Already has provenance

            // Add provenance based on label pattern
            var label = (string)testCase[0];
            var provenance = ExtractDatabaseFromLabel(label);

            return new object[]
            {
                testCase[0], // Label
                testCase[1], // MachineId
                testCase[2], // BarCodeId
                provenance   // Database
            };
        }).ToList();

        return casesWithProvenance;
    }

    private static string ExtractDatabaseFromLabel(string label)
    {
        if (label.StartsWith("QA45") || label.Contains("QA45")) return "QA45";
        if (label.StartsWith("QA46") || label.Contains("QA46")) return "QA46";
        if (label.StartsWith("QA62") || label.Contains("QA62")) return "QA62";
        return "QA45"; // Default
    }

    private class ClientTracker
    {
        public HashSet<int> UsedIndices { get; } = new();
        public int UsedCount { get; set; }
        public int Iteration { get; set; }

        public void Reset()
        {
            UsedIndices.Clear();
            UsedCount = 0;
        }
    }

    public class ClientStats
    {
        public int UsedCount { get; init; }
        public int TotalAvailable { get; init; }
        public double PercentageUsed { get; init; }
        public int Iterations { get; init; }
    }
}
