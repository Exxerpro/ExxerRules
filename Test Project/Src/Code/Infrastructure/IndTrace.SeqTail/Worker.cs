// <copyright file="Worker.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.SeqTail
{
    using Microsoft.Extensions.Options;
    using Seq.Api;

    /// <summary>
    /// Represents the Worker.
    /// </summary>
    public class Worker(ILogger<Worker> logger, IOptions<SeqApiOptions> seqOptions) : BackgroundService
    {
        private readonly ILogger<Worker> logger = logger;
        private readonly SeqApiOptions seqOptions = seqOptions.Value;

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [Security] - Use injected configuration instead of hardcoded values
            var connection = new SeqConnection(this.seqOptions.Server, this.seqOptions.ApiKey);

            var installedApps = await connection.Apps.ListAsync(stoppingToken);

            await this.ReadEvents(stoppingToken);
            Console.WriteLine("Good bye");
            await Task.Delay(5000, stoppingToken);
            Console.ReadLine();
        }

        /*
         *
         *
         *

           select @Message, Timestamp
           from stream
           where @Message like '%mismatch%' ci and @Message like '%QA45422290251313618%' ci
           limit 100



           select @Message, Timestamp
           from stream
           where @Message like '%QA45L823566251322903%' ci
           limit 100

         *
         */

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate worker logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

        /// <summary>
        /// Executes ReadEvents operation.
        /// </summary>
        /// <param name="stoppingToken">The stoppingToken.</param>
        /// <returns>The result of ReadEvents.</returns>
        public async Task ReadEvents(CancellationToken stoppingToken)
        {
            var barcode = "QA45422290251313607";

            var query = $"""
                        select @Message, @Timestamp
                        from stream
                        where @Message like '%{barcode}%'
                        limit 50000
                        """;

            var queryMismatch = $"""
                                 select @Message, Timestamp
                                 from stream
                                 where @Message like '%mismatch%' ci and @Message like '%{barcode}%' ci
                                 limit 100

                                 """;

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [Security] - Use injected configuration instead of creating new instance
            var connection = new SeqConnection(this.seqOptions.Server, this.seqOptions.ApiKey);

            var now = DateTime.UtcNow;
            var rangeStartUtc = now - TimeSpan.FromMinutes(600);
            DateTime? rangeEndUtc = now;

            try
            {
                var result = await connection.Data.QueryCsvAsync(query, rangeStartUtc, rangeEndUtc, cancellationToken: stoppingToken);

                // Define output file path
                var outputPath = Path.Combine(AppContext.BaseDirectory, $"SeqExport_{barcode}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt");

                // Write result to file
                await File.WriteAllTextAsync(outputPath, result, stoppingToken);

                Console.WriteLine($"✅ Events exported to: {outputPath}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("⚠️ Operation was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ An error occurred while reading events: {ex.Message}");
            }
        }
    }
}
