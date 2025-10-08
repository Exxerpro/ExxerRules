// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.SeqTail
{
    /// <summary>
    /// Represents the Program.
    /// </summary>
    public class Program
    {
        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate program startup and SeqTail logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

        /// <summary>
        /// Executes Main operation.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            //[Fix]
            //CLAUDE
            //Date: 27/09/2025
            //Reason: [Security] - Configure SeqApiOptions from appsettings.json instead of hardcoded values
            builder.Services.Configure<SeqApiOptions>(
                builder.Configuration.GetSection("SeqApi"));

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}
