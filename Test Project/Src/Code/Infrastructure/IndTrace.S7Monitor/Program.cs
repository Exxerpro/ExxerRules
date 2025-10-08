// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

internal class Program
{
    private static readonly CancellationTokenSource Cts = new();

    /// <summary>
    /// The main entry point for the S7 Monitor application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The exit code for the application.</returns>
    public static async Task<int> Main(string[] args)
    {
        VerifyIfApplicationIsNotRunning("IndTrace.S7Monitor");

        Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

        Console.CancelKeyPress += OnCancelKeyPress;
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

        try
        {
            var app =
                new CommandApp<MultiPlcCommand>()
                    .WithData(Cts.Token)
                    .WithDescription("This program connects to a Siemens S7 PLC using RFC1006 and reads the variables specified.");

            app.Configure(config =>
            {
                config.AddExample("192.0.0.10 db100.int12");
                config.AddExample("192.0.0.10 --cpu 2 --rack 1 db100.int12");

                config.SetHelpProvider(new CustomHelpProvider(config.Settings));

                config.SetApplicationName(OperatingSystem.IsWindows() ? "s7mon.exe" : "s7mon");
            });

            return await app.RunAsync(args);
        }
        catch (OperationCanceledException)
        {
            Console.ReadLine();
            return 0;
        }
        finally
        {
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            Console.CancelKeyPress -= OnCancelKeyPress;
        }
    }

    /// <summary>
    /// Handles the cancel key press event to trigger cancellation.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        if (!Cts.IsCancellationRequested)
        {
            // NOTE: cancel event, don't terminate the process
            e.Cancel = true;
        }

        Cts.Cancel();
    }

    /// <summary>
    /// Handles the process exit event to trigger cancellation if needed.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnProcessExit(object? sender, EventArgs e)
    {
        if (Cts.IsCancellationRequested)
        {
            // NOTE: SIGINT (cancel key was pressed, this shouldn't ever actually hit however, as we remove the event handler upon cancellation of the `cancellationSource`)
            return;
        }

        Cts.Cancel();
    }

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetConsoleWindow();

    /// <summary>
    /// Verifies that another instance of the application is not already running.
    /// </summary>
    /// <param name="name">The name of the mutex to check for application instance.</param>
    private static void VerifyIfApplicationIsNotRunning(string name)
    {
        using var mutex = new Mutex(true, name, out var createdNew);
        if (createdNew)
        {
            return;
        }

        try
        {
            var hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
            }

            Console.WriteLine("An instance of the application is already running.");
            Console.ReadLine();
            Environment.Exit(0);
        }
        catch (Exception)
        {
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
