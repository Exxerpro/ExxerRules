// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Hub.Client;

/// <summary>
/// Represents the Program.
/// </summary>
public class Program
{
    /// <summary>
    /// Executes Main operation.
    /// </summary>
    /// <param name="args">The args.</param>
    public static void Main(string[] args)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<WorkerHubClient>();

            var host = builder.Build();
            host.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        Console.ReadLine();
    }
}
