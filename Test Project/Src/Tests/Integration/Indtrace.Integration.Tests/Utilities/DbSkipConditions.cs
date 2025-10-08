using Microsoft.Extensions.Configuration;

namespace Integration.Tests.Utilities;

public static class DbSkipConditions
{
    public static bool IsCiSkip =>
        IsTrue(Environment.GetEnvironmentVariable("SKIP_DB_TESTS"));

    public static bool SkipDb(string dbKey)
    {
        if (IsCiSkip)
            return true;

        var conn = ResolveConnectionString(dbKey);
        return string.IsNullOrWhiteSpace(conn);
    }

    public static bool SkipDb45 => SkipDb(Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45);
    public static bool SkipQA62 => SkipDb(Integration.Tests.Utilities.DbProfiles.IndTraceDbContext62);
    public static bool SkipQA46 => SkipDb(Integration.Tests.Utilities.DbProfiles.IndTraceDbContext46);

    private static string? ResolveConnectionString(string dbKey)
    {
        // Read settings from test output directory where appsettings are copied
        var baseDir = AppContext.BaseDirectory;
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Test";

        var builder = new ConfigurationBuilder();

        var appsettings = Path.Combine(baseDir, "appsettings.json");
        if (File.Exists(appsettings)) builder.AddJsonFile(appsettings, optional: true, reloadOnChange: false);

        var envAppsettings = Path.Combine(baseDir, $"appsettings.{env}.json");
        if (File.Exists(envAppsettings)) builder.AddJsonFile(envAppsettings, optional: true, reloadOnChange: false);

        var cfg = builder.Build();
        return cfg[$"ConnectionStrings:{dbKey}"];
    }

    private static bool IsTrue(string? value)
        => !string.IsNullOrWhiteSpace(value) &&
           (string.Equals(value, "1") || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase));
}
