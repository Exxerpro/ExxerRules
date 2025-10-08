namespace Integration.Tests.Data;

/// <summary>
/// Real working test data extracted from production database copies
/// </summary>
public static class WorkingTestData
{
    /// <summary>
    /// Working barcode test cases that exist in all databases
    /// Format: [dbKey, label, machineId, barCodeId]
    /// </summary>
    public static IEnumerable<object[]> BarCodeTestCases()
    {
        // QA45 Database working data
        yield return new object[] { "IndTraceDbContext45", "QA45422290251700925", 100, 50925 };
        yield return new object[] { "IndTraceDbContext45", "QA45422290251700924", 100, 50924 };
        yield return new object[] { "IndTraceDbContext45", "QA45422310251420922", 100, 50922 };
        yield return new object[] { "IndTraceDbContext45", "QA45422310251420921", 100, 50921 };
        yield return new object[] { "IndTraceDbContext45", "QA45422310251420920", 500, 50920 };

        // QA46 Database equivalent (update labels to QA46 prefix)
        yield return new object[] { "IndTraceDbContext46", "QA46422290251700925", 100, 50925 };
        yield return new object[] { "IndTraceDbContext46", "QA46422290251700924", 100, 50924 };
        yield return new object[] { "IndTraceDbContext46", "QA46422310251420922", 100, 50922 };
        yield return new object[] { "IndTraceDbContext46", "QA46422310251420921", 100, 50921 };
        yield return new object[] { "IndTraceDbContext46", "QA46422310251420920", 500, 50920 };

        // QA62 Database equivalent (update labels to QA62 prefix)
        yield return new object[] { "IndTraceDbContext62", "QA62422290251700925", 100, 50925 };
        yield return new object[] { "IndTraceDbContext62", "QA62422290251700924", 100, 50924 };
        yield return new object[] { "IndTraceDbContext62", "QA62422310251420922", 100, 50922 };
        yield return new object[] { "IndTraceDbContext62", "QA62422310251420921", 100, 50921 };
        yield return new object[] { "IndTraceDbContext62", "QA62422310251420920", 500, 50920 };
    }

    /// <summary>
    /// Working cycle test cases with complete parameter sets
    /// Format: [dbKey, label, machineId, lastMachineId, nextMachineId, lenPartNumber, partStatus, flowStatus, cycleStatus, barCodeId, cycleId, machineType, startCycle]
    /// </summary>
    public static IEnumerable<object[]> CycleTestCases()
    {
        // QA45 Database working cycle data
        yield return new object[] { "IndTraceDbContext45", "QA45422290251700925", 100, 100, 100, 6, "Ok", "InProcess", "Started", 50925, 123414, "Process", "2025-06-18T18:45:29.822" };
        yield return new object[] { "IndTraceDbContext45", "QA45422290251700924", 100, 100, 100, 6, "Ok", "InProcess", "Started", 50924, 123413, "Process", "2025-06-18T18:43:03.977" };
        yield return new object[] { "IndTraceDbContext45", "QA45422310251420779", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50779, 123411, "Process", "2025-05-22T08:42:38.566" };
        yield return new object[] { "IndTraceDbContext45", "QA45422310251420864", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50864, 123410, "Process", "2025-05-22T08:41:09.223" };
        yield return new object[] { "IndTraceDbContext45", "QA45422310251420842", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50842, 123409, "Process", "2025-05-22T08:40:30.622" };

        // QA46 Database equivalent
        yield return new object[] { "IndTraceDbContext46", "QA46422290251700925", 100, 100, 100, 6, "Ok", "InProcess", "Started", 50925, 123414, "Process", "2025-06-18T18:45:29.822" };
        yield return new object[] { "IndTraceDbContext46", "QA46422290251700924", 100, 100, 100, 6, "Ok", "InProcess", "Started", 50924, 123413, "Process", "2025-06-18T18:43:03.977" };
        yield return new object[] { "IndTraceDbContext46", "QA46422310251420779", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50779, 123411, "Process", "2025-05-22T08:42:38.566" };
        yield return new object[] { "IndTraceDbContext46", "QA46422310251420864", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50864, 123410, "Process", "2025-05-22T08:41:09.223" };
        yield return new object[] { "IndTraceDbContext46", "QA46422310251420842", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50842, 123409, "Process", "2025-05-22T08:40:30.622" };

        // QA62 Database equivalent
        yield return new object[] { "IndTraceDbContext62", "QA62422290251700925", 100, 100, 100, 6, "Ok", "InProcess", "Started", 50925, 123414, "Process", "2025-06-18T18:45:29.822" };
        yield return new object[] { "IndTraceDbContext62", "QA62422290251700924", 100, 100, 100, 6, "Ok", "InProcess", "Started", 50924, 123413, "Process", "2025-06-18T18:43:03.977" };
        yield return new object[] { "IndTraceDbContext62", "QA62422310251420779", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50779, 123411, "Process", "2025-05-22T08:42:38.566" };
        yield return new object[] { "IndTraceDbContext62", "QA62422310251420864", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50864, 123410, "Process", "2025-05-22T08:41:09.223" };
        yield return new object[] { "IndTraceDbContext62", "QA62422310251420842", 500, 500, 500, 6, "Ok", "InProcess", "Started", 50842, 123409, "Process", "2025-05-22T08:40:30.622" };
    }

    /// <summary>
    /// Working machine test cases
    /// Format: [machineId, machineType, machineName]
    /// </summary>
    public static IEnumerable<object[]> MachineTestCases()
    {
        yield return new object[] { 100, 4, "WS100/AUDI" };
        yield return new object[] { 400, 8, "WS400/AUDI" };
        yield return new object[] { 500, 16, "WS500/AUDI" };
        yield return new object[] { 1200, 8, "WS200/RAM" };
        yield return new object[] { 1400, 8, "WS400/RAM" };
    }
}
