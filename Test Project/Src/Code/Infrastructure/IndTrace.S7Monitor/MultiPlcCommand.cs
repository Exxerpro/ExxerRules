// <copyright file="MultiPlcCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Command for connecting to multiple Siemens S7 PLCs and monitoring variables in real-time.
/// </summary>
internal sealed class MultiPlcCommand : AsyncCommand<MultiPlcCommand.Settings>
{
    /// <summary>
    /// Executes the multi-PLC monitoring command asynchronously.
    /// </summary>
    /// <param name="context">The command execution context.</param>
    /// <param name="settings">The command settings containing configuration file information.</param>
    /// <returns>A task representing the execution result with exit code.</returns>
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var token = (CancellationToken)(context.Data ?? CancellationToken.None);

        try
        {
            await RunProgram(settings, token);
        }
        catch (TaskCanceledException)
        {
        }

        return 0;
    }

    private static async Task RunProgram(Settings settings, CancellationToken token)
    {
        DisplayFileName(settings);

        var data = DataInitializer.InitializePlcData();
        foreach (var (id, plcinfo) in data)
        {
            await ConnectToPlc(plcinfo, token);
        }

        AnsiConsole.MarkupLine("[gray]Press Ctrl + C to cancel.[/]");

        using var variableContainer = VariableContainer.Initialize(data);

        // Create a table
        var table = new Table
        {
            Border = TableBorder.Rounded,
            BorderStyle = CustomStyles.TableBorder,
        };

        table.AddColumn(new TableColumn(new Text("Variable", CustomStyles.Default)));
        table.AddColumn(new TableColumn(new Text("Description", CustomStyles.Default)));
        table.AddColumn(new TableColumn(new Text("ValueString", CustomStyles.Default)));

        foreach (var record in variableContainer.VariableRecords)
        {
            table.AddRow(
                new Text(record.Address, CustomStyles.Default),
                new Text(record.Description, CustomStyles.Data),
                new Text("init", CustomStyles.Note));
        }

        await AnsiConsole.Live(table)
            .StartAsync(async ctx =>
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var record in variableContainer.VariableRecords)
                    {
                        if (record.HasUpdate(out var value))
                        {
                            table.Rows.Update(
                                record.RowIdx, 2, RenderUtil.FormatCellData(value));
                        }
                    }

                    ctx.Refresh();

                    await Task.Delay(100, token);
                }
            });
    }

    private static void DisplayFileName(Settings settings)
    {
        AnsiConsole.MarkupLine(settings.FileName == "NoFile"
            ? "[red]No file name provided. Using DefaultData[/]"
            : $"Using file name [green]{settings.FileName}[/].");
    }

    private static async Task ConnectToPlc(PlcDataModels plcDataModels, CancellationToken token)
    {
        AnsiConsole.MarkupLine($"Connecting to plc [green]{plcDataModels.IpAddress}[/].");

        var newPlc = new Sharp7Plc(plcDataModels.IpAddress, 0, 2, 102);
        await newPlc.TriggerConnection(token);
        plcDataModels.Plc = newPlc;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBar)
            .StartAsync("Connecting...", async ctx =>
            {
                var lastState = ConnectionState.Initial;
                ctx.Status(lastState.ToString());

                while (!token.IsCancellationRequested)
                {
                    var state = await newPlc.ConnectionState.FirstAsync(s => s != lastState).ToTask(token);
                    ctx.Status(state.ToString());

                    if (state == ConnectionState.Connected)
                    {
                        return;
                    }
                }
            });

        token.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Settings class for configuring multi-PLC monitoring with optional configuration file.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Gets the configuration file name. If not provided, default data will be used.
        /// </summary>
        [Description("File Name if not used DefaultData")]
        [CommandArgument(0, "[FileName]")]
        public string FileName { get; init; } = "NoFile";

        /// <summary>
        /// Validates the command settings to ensure proper configuration.
        /// </summary>
        /// <returns>A validation result indicating success or failure with error message.</returns>
        public override ValidationResult Validate()
        {
            return this.FileName.Length < 2
                ? ValidationResult.Error("Names must be at least two characters long")
                : ValidationResult.Success();
        }
    }
}
