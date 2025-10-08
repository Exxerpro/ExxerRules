namespace IndTrace.Dependencies.Interceptors
{
    /// <summary>
    /// Provides methods to load application configuration from JSON files and environment variables.
    /// </summary>
    public static class ConfigLoader
    {
        /// <summary>
        /// Loads the application configuration, including environment-specific overrides if present.
        /// </summary>
        /// <returns>The loaded <see cref="IConfiguration"/> instance.</returns>
        public static IConfiguration Load()
        {
            var baseDir = AppContext.BaseDirectory;
            var settingsPath = Path.Combine(baseDir, "..", "settings");

            if (!Directory.Exists(settingsPath))
            {
                // Fallback for Debug mode
                settingsPath = Directory.GetCurrentDirectory();
            }

            Console.WriteLine($"[ConfigLoader] Using settings directory: {settingsPath}");

            var preliminaryConfig = new ConfigurationBuilder()
                .SetBasePath(settingsPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var activeProfile = preliminaryConfig["AppSettings:ActiveConfig"];
            Console.WriteLine($"[ConfigLoader] Active profile: {activeProfile}");

            var finalConfig = new ConfigurationBuilder()
                .SetBasePath(settingsPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{activeProfile}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return finalConfig;
        }
    }
}
