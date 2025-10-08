namespace Integration.Tests.Utilities;

/// <summary>
/// Defines the allowed named database profiles for Integration.Tests and centralizes the keys.
/// </summary>
public static class DbProfiles
{
    public const string IndTraceDbContext45 = "IndTraceDbContext45";
    public const string IndTraceDbContext46 = "IndTraceDbContext46";
    public const string IndTraceDbContext62 = "IndTraceDbContext62";

    /// <summary>
    /// Allowed keys set. Only these keys are registered in DI from ConnectionStrings.
    /// </summary>
    public static readonly HashSet<string> Allowed = new(StringComparer.Ordinal)
    {
        IndTraceDbContext45, IndTraceDbContext46, IndTraceDbContext62
    };
}
