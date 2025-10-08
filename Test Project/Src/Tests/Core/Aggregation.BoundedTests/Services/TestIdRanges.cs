namespace IndTrace.Aggregation.BoundedTests.Services;

/// <summary>
/// Centralized ID ranges for new, manually created test records.
/// Ensures no collisions with canonical TestData JSON datasets.
/// </summary>
public static class TestIdRanges
{
    public static class Registers
    {
        public const int Min = 1_800_000_000;
        public const int Max = 1_899_999_999;
    }

    public static class Cycles
    {
        public const int Min = 1_900_000_000;
        public const int Max = 1_904_999_999;
    }

    public static class BarCodes
    {
        public const int Min = 1_905_000_000;
        public const int Max = 1_909_999_999;
    }

    public static bool IsInRange(int id, int min, int max) => id >= min && id <= max;
}
