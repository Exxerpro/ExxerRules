namespace Integration.Tests.Utilities;

public static class TestDbGuards
{
    // Preferred: use these boolean helpers from a static property on the test class
    // and reference that property via [Fact/Theory(SkipWhen = nameof(YourProperty))].
    public static bool ShouldSkipDb(string dbKey) => DbSkipConditions.SkipDb(dbKey);
    public static bool ShouldSkipDb45 => DbSkipConditions.SkipDb45;
    public static bool ShouldSkipQA62 => DbSkipConditions.SkipQA62;
    public static bool ShouldSkipQA46 => DbSkipConditions.SkipQA46;

    public static void SkipIfDbMissing(IServiceProvider sp, string dbKey)
    {
        var cfg = sp.GetService<Microsoft.Extensions.Configuration.IConfiguration>();
        var conn = cfg?["ConnectionStrings:" + dbKey];
        // Deprecated: attribute-based [SkipWhen] is preferred; no-op here to avoid hard failures.
        _ = conn; // keep method signature; do nothing at runtime
    }
}
