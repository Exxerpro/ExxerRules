namespace IndTrace.LogAnalyzer.Data
{
    /// <summary>
    /// Service that provides weather forecast data for demonstration purposes.
    /// </summary>
    /// <remarks>
    /// This service generates random weather forecast data using predefined temperature ranges
    /// and weather condition summaries. It's primarily used for testing and UI development.
    /// </remarks>
    public class WeatherForecastService
    {
        /// <summary>
        /// Array of weather condition summaries used for generating forecast data.
        /// </summary>
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        /// <summary>
        /// Gets weather forecast data asynchronously for a specified date range.
        /// </summary>
        /// <param name="startDate">The starting date for the forecast period.</param>
        /// <returns>A task containing an array of weather forecast data for 5 days.</returns>
        /// <remarks>
        /// Generates random temperature values between -20°C and 55°C, and randomly selects
        /// weather condition summaries from the predefined array.
        /// </remarks>
        //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate WeatherForecastService logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
        /// <summary>
        /// Executes GetForecastAsync operation.
        /// </summary>
        /// <param name="startDate">The startDate.</param>
        /// <returns>The result of GetForecastAsync.</returns>
        public Task<WeatherForecast[]> GetForecastAsync(DateOnly startDate)
        {
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray());
        }
    }
}
