using System.Collections;
using Microsoft.Extensions.Configuration;

namespace Integration.Tests.Data;

// Provides a list of DB profiles (connection string keys) discovered from test appsettings
public sealed class DbProfileTheoryData : IEnumerable<object[]>
{
    private readonly List<object[]> _data = new();

    public DbProfileTheoryData()
    {
        // Load test appsettings from bin directory during test discovery
        var baseDir = AppContext.BaseDirectory;
        var builder = new ConfigurationBuilder()
            .SetBasePath(baseDir)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables();

        var cfg = builder.Build();
        var keys = new[]
        {
            Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45,
            Integration.Tests.Utilities.DbProfiles.IndTraceDbContext46,
            Integration.Tests.Utilities.DbProfiles.IndTraceDbContext62,
        };

        foreach (var key in keys)
        {
            var value = cfg.GetConnectionString(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                _data.Add(new object[] { key });
            }
        }
        // Ensure at least one entry
        if (_data.Count == 0)
        {
            _data.Add(new object[] { Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45 });
        }
    }

    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
