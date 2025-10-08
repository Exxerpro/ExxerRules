using System.Text.RegularExpressions;
using IndTrace.Dependencies.Simulations;

namespace IndTrace.VirtualNetwork.Simulation;

/// <summary>
/// Simulates commands and events for virtual PLCs.
/// </summary>
public class CommandSimulator
{
    private ILogger<CommandSimulator> logger = null!;

    private readonly IOptionsMonitor<SimulationSettings> optionsMonitor = null!;
    private readonly SimulationEngine engine = null!;

    //TODO ABSTRACT THIS CLASS TO SOME LIBRARY
    // IS THE SAME ON COMMUNICATION
    // ABR 18 MAU 2025
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandSimulator"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="optionsMonitor">The options monitor for simulation settings.</param>
    /// <param name="engine">The simulation engine.</param>
    public CommandSimulator(ILogger<CommandSimulator> logger, IOptionsMonitor<SimulationSettings> optionsMonitor, SimulationEngine engine)
    {
        this.logger = logger;
        this.optionsMonitor = optionsMonitor;
        this.engine = engine;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandSimulator"/> class.
    /// </summary>
    public CommandSimulator()
    {
        this.optionsMonitor = null!;
        this.engine = null!;
    }

    /// <summary>
    /// Gets or sets the machine ID.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the part number.
    /// </summary>
    public required string PartNumber { get; set; }

    /// <summary>
    /// Gets or sets the barcode.
    /// </summary>
    public required string BarCode { get; set; }

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    public short Command { get; set; }

    /// <summary>
    /// Gets or sets the cycle status.
    /// </summary>
    public int CycleStatus { get; set; }

    /// <summary>
    /// Gets or sets the part status.
    /// </summary>
    public int PartStatus { get; set; }
    /// <summary>
    /// Gets or sets the CycleStatusPlc.
    /// </summary>

    public short CycleStatusPlc { get; set; }
    /// <summary>
    /// Gets or sets the PartStatusPlc.
    /// </summary>
    public short PartStatusPlc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to simulate reference.
    /// </summary>
    public bool SimulateReference { get; set; }

    /// <summary>
    /// Gets or sets the watchdog value.
    /// </summary>
    public int WatchDog { get; set; }

    // Precompiled regex for performance
    private static readonly Regex CommandRegex = new(
        @"m\s+(\d+)\s+pn\s+(\S+)\s+bc\s+(\S+)\s+cmd\s+(\d+)\s+ps\s+(\d+)\s+cs\s+(\d+)(?:\s+sr\s+true)?(?:\s+wd\s+disable)?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    /// <summary>
    /// Parses simulated commands from a list of input lines.
    /// </summary>
    /// <param name="lines">The input lines containing command data.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    /// <returns>List of parsed simulated commands.</returns>
    public static List<CommandSimulator> ParseCommands(IEnumerable<string> lines, ILogger? logger = null)
    {
        var commands = new List<CommandSimulator>();
        var seenMachineIds = new HashSet<int>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var match = CommandRegex.Match(line);
            if (!match.Success)
            {
                logger?.LogWarning("Skipping invalid request line: {Line}", line);
                continue;
            }

            try
            {
                var machineId = int.Parse(match.Groups[1].Value);

                if (seenMachineIds.Contains(machineId))
                {
                    logger?.LogInformation("Skipping duplicate request for MachineId {MachineId}", machineId);
                    continue;
                }

                var commandValue = short.Parse(match.Groups[4].Value);

                var simulateReference = commandValue is 32 or 64 && line.Contains("sr true", StringComparison.OrdinalIgnoreCase);
                // 1987 — symbolic override for watchdog, chosen in honor of resilience and family
                var disableTimeoutMagic = 1987;
                var watchDog = line.Contains("wd disable", StringComparison.OrdinalIgnoreCase) ? disableTimeoutMagic : 0;

                var command = new CommandSimulator
                {
                    MachineId = machineId,
                    PartNumber = match.Groups[2].Value,
                    BarCode = match.Groups[3].Value,
                    Command = commandValue,
                    PartStatus = int.Parse(match.Groups[5].Value),
                    CycleStatus = int.Parse(match.Groups[6].Value),
                    PartStatusPlc = short.Parse(match.Groups[5].Value),
                    CycleStatusPlc = short.Parse(match.Groups[6].Value),
                    SimulateReference = simulateReference,
                    WatchDog = watchDog,
                };

                commands.Add(command);
                seenMachineIds.Add(machineId);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error parsing command line: {Line}", line);
            }
        }

        return commands;
    }

    /// <summary>
    /// Simulates events based on the provided input lines.
    /// </summary>
    /// <param name="plcNetwork">The dictionary of virtual PLCs.</param>
    /// <param name="inputLines">The input lines containing command data.</param>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SimulateEvents(Dictionary<int, VirtualPlc> plcNetwork, List<string> inputLines, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested) return;

        // Step 1: Parse all valid request lines

        var commands = CommandSimulator.ParseCommands(inputLines, this.logger);

        if (commands.Count == 0)
        {
            this.logger.LogTrace("No valid commands found.");
            return;
        }

        // Step 2: Dispatch each valid command to its worker in parallel
        var simulationTasks = new List<Task>();
        foreach (var command in commands)
        {
            if (plcNetwork.TryGetValue(command.MachineId, out VirtualPlc? plc))
            {
                simulationTasks.Add(this.SimulateCommandAsync(plc, command, stoppingToken));
            }
            else
            {
                this.logger.LogWarning("No worker found for MachineId {MachineId}", command.MachineId);
            }
        }

        await Task.WhenAll(simulationTasks);
    }

    /// <summary>
    /// Waits for simulation commands from the console input.
    /// </summary>
    /// <param name="timeout">The maximum time to wait for input.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A list of collected command lines.</returns>
    public async Task<List<string>> WaitForSimulationCommands(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var collectedLines = new List<string>();
        var startTime = DateTime.UtcNow;

        while (!cancellationToken.IsCancellationRequested)
        {
            if ((DateTime.UtcNow - startTime) > timeout)
            {
                break;
            }

            if (Console.KeyAvailable)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    this.logger.LogTrace("Empty input received — ending input collection.");
                    break;
                }

                collectedLines.Add(line);
            }

            await Task.Delay(100, cancellationToken); // Polling delay
        }

        return collectedLines;
    }

    /// <summary>
    /// Simulates a single command for a given PLC.
    /// </summary>
    /// <param name="plc">The virtual PLC instance.</param>
    /// <param name="command">The command to simulate.</param>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string> SimulateCommandAsync(VirtualPlc plc, CommandSimulator command, CancellationToken stoppingToken)
    {
        try
        {
            await this.WriteSimulatedCommandAsync(plc, command, stoppingToken);

            if (command is { MachineId: 100, Command: 32 })
            {
                await this.WriteSimulatedPerformanceDataAsync(plc, stoppingToken);
            }

            await Task.Delay(100);
            await plc.S7Conn.SetValue<short>(plc.IndTraceTags["Command"].Address, (short)command.Command);

            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            this.logger.LogError("Exception occurred while simulation {ex}", ex);
            return string.Empty;
        }

        return string.Empty;
    }

    /// <summary>
    /// Writes simulated command data to the PLC.
    /// </summary>
    /// <param name="plcDataData">The virtual PLC instance.</param>
    /// <param name="command">The command containing data to write.</param>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task WriteSimulatedCommandAsync(VirtualPlc plcDataData, CommandSimulator command, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;
        var settings = this.optionsMonitor.CurrentValue;

        if (!settings.EnableSimulation) return;

        // Logic using _settings properties

        await plcDataData.S7Conn.SetValue<string>(plcDataData.IndTraceTags["PartNumber"].Address, command.PartNumber, stoppingToken);
        await plcDataData.S7Conn.SetValue<string>(plcDataData.IndTraceTags["BarCode"].Address, command.BarCode, stoppingToken);
        await plcDataData.S7Conn.SetValue<int>(plcDataData.IndTraceTags["PartStatus"].Address, command.PartStatus, stoppingToken);
        await plcDataData.S7Conn.SetValue<short>(plcDataData.IndTraceTags["PartStatusPlc"].Address, (short)command.PartStatus, stoppingToken);
        await plcDataData.S7Conn.SetValue<int>(plcDataData.IndTraceTags["CycleStatus"].Address, command.CycleStatus, stoppingToken);
        await plcDataData.S7Conn.SetValue<short>(plcDataData.IndTraceTags["CycleStatusPlc"].Address, (short)command.CycleStatus, stoppingToken);
    }

    /// <summary>
    /// Writes simulated performance data to the PLC.
    /// </summary>
    /// <param name="plcDataData">The virtual PLC instance.</param>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task WriteSimulatedPerformanceDataAsync(VirtualPlc plcDataData, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        var snapshot = this.engine.Simulate();

        foreach (var (name, address) in PlcTagNames.PerformanceReals)
        {
            var value = name switch
            {
                "TotalProduction" => snapshot.TotalProduction,
                "ProductionOk" => snapshot.ProductionOk,
                "ProductionNoK" => snapshot.ProductionNoK,
                _ => 0.0f,
            };
            await plcDataData.S7Conn.SetValue<float>(address, value, stoppingToken);
        }

        foreach (var (name, address) in PlcTagNames.PerformanceInts)
        {
            var value = name switch
            {
                "RunningTime" => snapshot.RunningTime,
                "StoppedTime" => snapshot.StoppedTime,
                "FaultedTime" => snapshot.FaultedTime,
                "EventCounter" => snapshot.EventCounter,
                _ => 0,
            };
            await plcDataData.S7Conn.SetValue<int>(address, value, stoppingToken);
        }
    }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate request simulator logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    //TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated request simulation or execution logic. Refactor for maintainability if necessary.
    //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency request simulation, consider optimizing execution and memory usage.
}
