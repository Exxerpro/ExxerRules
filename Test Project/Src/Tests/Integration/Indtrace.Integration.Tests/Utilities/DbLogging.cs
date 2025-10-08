namespace Integration.Tests.Utilities;

internal sealed class DbLogCategory { }

public static class DbLogging
{
    public static void LogConnectionString(IServiceProvider sp, string dbKey, ITestOutputHelper output, string source)
    {
        var cfg = sp.GetService<Microsoft.Extensions.Configuration.IConfiguration>();
        var conn = cfg?["ConnectionStrings:" + dbKey] ?? "<null>";
        var logger = XUnitLogger.CreateLogger<DbLogCategory>(output);
        logger.LogInformation("DB connection [{Source}] key {DbKey} -> {Conn}", source, dbKey, conn);
    }
}
