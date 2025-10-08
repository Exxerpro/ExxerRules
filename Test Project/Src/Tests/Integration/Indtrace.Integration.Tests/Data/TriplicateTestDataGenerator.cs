using Xunit.Sdk;

namespace Integration.Tests.Data;

/// <summary>
/// Generates test data for all three databases with inline key specification
/// </summary>
public static class TriplicateTestDataGenerator
{
    /// <summary>
    /// Generates test cases for all three databases from a single source
    /// Usage: [MemberData(nameof(TriplicateTestDataGenerator.GenerateForAllDatabases), "TestMethodName", MemberType = typeof(TriplicateTestDataGenerator))]
    /// </summary>
    public static IEnumerable<object[]> GenerateForAllDatabases(string testMethodName)
    {
        // Define test data once, then triplicate with inline keys
        var baseTestCases = testMethodName switch
        {
            "CycleTests" => GetCycleTestData(),
            "MachineTests" => GetMachineTestData(),
            _ => throw new NotSupportedException($"No test data defined for {testMethodName}")
        };

        // Triplicate for each database
        foreach (var testCase in baseTestCases)
        {
            yield return PrependDatabaseKey(testCase, "IndTraceDbContext45");
            yield return PrependDatabaseKey(testCase, "IndTraceDbContext46");
            yield return PrependDatabaseKey(testCase, "IndTraceDbContext62");
        }
    }

    /// <summary>
    /// Inline database key attribute for xUnit theories - COMMENTED OUT FOR NOW
    /// Usage: [Theory, TriplicateInlineData("QA45422290241800874", 400, ...)]
    /// </summary>
    /*
    public class TriplicateInlineDataAttribute : CompositeDataAttribute
    {
        public TriplicateInlineDataAttribute(params object[] values)
            : base(CreateAttributes(values))
        {
        }

        private static DataAttribute[] CreateAttributes(object[] values)
        {
            return new[]
            {
                new InlineDataAttribute(PrependDatabaseKey(values, "IndTraceDbContext45")),
                new InlineDataAttribute(PrependDatabaseKey(values, "IndTraceDbContext46")),
                new InlineDataAttribute(PrependDatabaseKey(values, "IndTraceDbContext62"))
            };
        }
    }
    */

    private static object[] PrependDatabaseKey(object[] testCase, string dbKey)
    {
        var result = new object[testCase.Length + 1];
        result[0] = dbKey;
        Array.Copy(testCase, 0, result, 1, testCase.Length);
        return result;
    }

    private static IEnumerable<object[]> GetCycleTestData()
    {
        // Based on your failing tests, these need to exist in the database
        return new[]
        {
            new object[] { "QA45422290241800874", 400, 400, 400, 6, "Ok", "InProcess", "Started", 874, 1889, "Process", "2024-06-28T11:12:05.5532502" },
            new object[] { "QA45431610251233180", 1200, 1200, 1200, 6, "Ok", "InProcess", "Started", 13180, 28491, "Process", "2025-05-03T01:57:54.0154087" },
            new object[] { "QA45422310243382779", 100, 100, 100, 6, "Ok", "Created", "Started", 12779, 27869, "InitialPrinter", "2024-12-03T17:11:20.1029427" },
            new object[] { "QA45422290242491818", 100, 100, 100, 6, "Ok", "Created", "Started", 1818, 3933, "InitialPrinter", "2024-09-05T10:52:18.7509385" },
            new object[] { "QA45431610251233168", 1400, 1400, 1400, 6, "Ok", "InProcess", "Started", 13168, 28473, "Process", "2025-05-02T19:33:24.5847734" },
            new object[] { "QA45422300242361624", 400, 400, 400, 6, "Ok", "InProcess", "Started", 1624, 3571, "Process", "2024-08-23T17:32:14.8445767" },
            new object[] { "QA4500t349251303262", 100, 100, 100, 6, "Ok", "InProcess", "Started", 13262, 28670, "InitialPrinter", "2025-05-10T10:04:09.1933400" },
            new object[] { "QA45422330243118157", 400, 400, 400, 6, "Ok", "InProcess", "Started", 8157, 18846, "Process", "2024-11-06T13:35:08.2971758" },
            new object[] { "QA45422330242965567", 500, 500, 500, 6, "Ok", "InProcess", "Started", 5567, 12325, "Final", "2024-10-22T14:21:39.9461991" },
            new object[] { "QA45422310243199388", 500, 500, 500, 6, "Ok", "InProcess", "Started", 9388, 22261, "Final", "2024-11-14T13:15:10.2839409" },
            new object[] { "QA45422310242612050", 400, 400, 400, 6, "Ok", "InProcess", "Started", 2050, 4349, "Process", "2024-09-17T12:51:20.5356798" }
        };
    }

    private static IEnumerable<object[]> GetMachineTestData()
    {
        return new[]
        {
            new object[] { 100, "InitialPrinter", "Assembly Line A" },
            new object[] { 400, "Process", "Production Line B" },
            new object[] { 500, "Final", "Quality Control" },
            new object[] { 1200, "Process", "Special Line" },
            new object[] { 1400, "Process", "Custom Line" }
        };
    }
}

/// <summary>
/// Extension method for inline database key injection
/// </summary>
public static class DatabaseKeyExtensions
{
    public static IEnumerable<object[]> WithDatabaseKeys(this IEnumerable<object[]> testData)
    {
        foreach (var data in testData)
        {
            yield return new[] { "IndTraceDbContext45" }.Concat(data).ToArray();
            yield return new[] { "IndTraceDbContext46" }.Concat(data).ToArray();
            yield return new[] { "IndTraceDbContext62" }.Concat(data).ToArray();
        }
    }
}
