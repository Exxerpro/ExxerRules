// <copyright file="ReadPlcCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Command for reading variables from a Siemens S7 PLC and displaying them in a live table.
/// </summary>
internal sealed class ReadPlcCommand : AsyncCommand<ReadPlcCommand.Settings>
{
    /// <summary>
    /// Executes the PLC reading command asynchronously.
    /// </summary>
    /// <param name="context">The command execution context.</param>
    /// <param name="settings">The command settings containing PLC connection and variable information.</param>
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
        AnsiConsole.MarkupLine($"Connecting to plc [green]{settings.PlcIp}[/], CPU [green]{settings.CpuMpiAddress}[/], rack [green]{settings.RackNumber}[/]. ");
        AnsiConsole.MarkupLine("[gray]Press Ctrl + C to cancel.[/]");

        using var plc = new Sharp7Plc(settings.PlcIp, settings.RackNumber, settings.CpuMpiAddress);

        await plc.TriggerConnection(token);

        // Connect
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBar)
            .StartAsync("Connecting...", async ctx =>
            {
                var lastState = ConnectionState.Initial;
                ctx.Status(lastState.ToString());

                while (!token.IsCancellationRequested)
                {
                    var state = await plc.ConnectionState.FirstAsync(s => s != lastState).ToTask(token);
                    ctx.Status(state.ToString());

                    if (state == ConnectionState.Connected)
                    {
                        return;
                    }
                }
            });

        token.ThrowIfCancellationRequested();

        using var variableContainer = VariableContainer.Initialize(plc, settings.Variables);

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

    /// <summary>
    /// Settings class for configuring PLC connection parameters and variables to read.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Gets the IP address of the S7 PLC to connect to.
        /// </summary>
        [Description("IP address of S7")]
        [CommandArgument(0, "<IP address>")]
        public string PlcIp { get; init; } = string.Empty;

        /// <summary>
        /// Gets the array of variable addresses to read from the PLC (e.g., "Db200.Int4").
        /// </summary>
        [CommandArgument(1, "[variables]")]
        [Description("Variables to read from S7, like Db200.Int4")]
        public string[] Variables { get; init; } = System.Array.Empty<string>();

        /// <summary>
        /// Gets the CPU MPI address of the S7 PLC instance.
        /// </summary>
        [CommandOption("-c|--cpu")]
        [Description("CPU MPI address of S7 instance")]
        [DefaultValue(0)]
        public int CpuMpiAddress { get; init; }

        /// <summary>
        /// Gets the rack number of the S7 PLC instance.
        /// </summary>
        [CommandOption("-r|--rack")]
        [Description("Rack number of S7 instance")]
        [DefaultValue(0)]
        public int RackNumber { get; init; }

        /// <summary>
        /// Validates the command settings to ensure proper PLC connection parameters.
        /// </summary>
        /// <returns>A validation result indicating success or failure with error message.</returns>
        public override ValidationResult Validate()
        {
            if (!StringHelper.IsValidIp4(this.PlcIp))
            {
                return ValidationResult.Error($"\"{this.PlcIp}\" is not a valid IP V4 address");
            }

            if (this.Variables == null || this.Variables.Length == 0)
            {
                return ValidationResult.Error("Please supply at least one variable to read");
            }

            return ValidationResult.Success();
        }
    }
}
