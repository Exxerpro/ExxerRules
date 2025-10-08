// <copyright file="GatewayWorkerManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Gateway.Gateway;

using IndTrace.Gateway.Exceptions;

/// <summary>
/// Represents the GatewayWorkerManager.
/// </summary>
public class GatewayWorkerManager(ILogger<GatewayWorkerManager> logger,
    DateTimeMachine dateTimeMachine,
    IndTraceConfigurationService configService, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<GatewayWorkerManager> logger = logger;
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private IndTraceConfigurationService configService = configService;
    private readonly DateTimeMachine dateTimeMachine = dateTimeMachine;
    private readonly Dictionary<int, GatewayWorker> gatewayWorkers = []; // Keeps track of workers
    private readonly Dictionary<int, Task> runningWorkers = []; // Keeps track of workers

    private readonly CancellationTokenSource cts = new(); // Global cancellation for cleanup

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Gateway Worker Manager started.");

        List<int> plcs = [];

        try
        {
            this.logger.LogInformation("Communications Worker GetConfigAppDetails at : {time}", this.dateTimeMachine.Now.ToLocalTime());
            var resultConfiguration = await this.configService.GetConfigurationAsync(true, stoppingToken).ConfigureAwait(false);
            if (resultConfiguration.Value is null)
            {
                this.logger.LogCritical("Plc list empty");
                this.logger.LogCritical("exiting application");
                return;
            }

            plcs = resultConfiguration.Value.Plcs.Select(e => e.PlcId).ToList();
            if (plcs.Count == 0)
            {
                this.logger.LogCritical("Plc list empty");
                this.logger.LogCritical("exiting application");
                return;
            }

            // Validate the PLC list have no tags on the same PLC with the same Address
            // For this purpose, we will use the IndTraceConfigurationService to get the configuration
            // and check if there are any duplicate addresses in the PLC list
            // Using Command, Barcode, PartNumber to discover duplicates
            this.DiscoverDuplicatedTags(resultConfiguration.Value);
            this.logger.LogInformation("Communications Worker GetConfigAppDetails at : {time}", this.dateTimeMachine.Now.ToLocalTime());
        }
        catch (Exception ex)
        {
            this.logger.LogError("Communications Worker GetConfigAppDetails at {ex}", ex);
            this.logger.LogCritical("exiting application");
            throw;
        }

        // Start a worker for each client
        foreach (var plc in plcs)
        {
            this.StartWorker(plc, stoppingToken);
        }

        this.logger.LogInformation("All {Count} workers initialized successfully: [{Ids}]", this.gatewayWorkers.Count,
            string.Join(", ", this.gatewayWorkers.Keys));

        // Keep running, listen for additional clients (if needed)
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken).ConfigureAwait(false);

            var commands = await this.WaitForSimulationCommands(TimeSpan.FromMilliseconds(500), stoppingToken).ConfigureAwait(false);
            await this.SimulateEventFromKeyboard(commands, stoppingToken).ConfigureAwait(false);
        }

        this.logger.LogInformation("Gateway Worker Manager stopping.");
    }

    private void StartWorker(int plcId, CancellationToken cancellationToken)
    {
        if (this.runningWorkers.ContainsKey(plcId))
        {
            this.logger.LogWarning("Worker for {PlcId} is already running", plcId);
            return;
        }

        this.logger.LogInformation("Starting worker for {PlcId}", plcId);

        using var scope = this.serviceProvider.CreateScope();
        var worker = new GatewayWorker(
            plcId,
            scope.ServiceProvider.GetRequiredService<ILogger<GatewayWorker>>(),
            scope.ServiceProvider.GetRequiredService<IGatewayCommandDispatcher>(),
            scope.ServiceProvider.GetRequiredService<IndTraceConfigurationService>(),
            scope.ServiceProvider.GetRequiredService<IHubConnectionFactory>(),
            scope.ServiceProvider.GetRequiredService<DateTimeMachine>(),
            // Synchronously create a connection for constructor; lifetime owned by worker
            scope.ServiceProvider.GetRequiredService<IHubConnectionFactory>().CreateAsync(cancellationToken).GetAwaiter().GetResult())
        {
            PlcData = new PlcDto { PlcId = plcId }
        };

        // Subscribe to the event (bind it to the existing SimulateCommand method)
        worker.OnCommandReceived += async (command) => await worker.SimulateCommand(command, cancellationToken).ConfigureAwait(false);

        // Fire-and-forget with logging
        var workerTask = Task.Run(
            async () =>
        {
            try
            {
                this.logger.LogInformation("Worker task for PLC {PlcId} started.", plcId);

                await worker.StartAsync(this.cts.Token).ConfigureAwait(false);

                this.logger.LogInformation("Worker for PLC {PlcId} started Ok.", plcId);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "Worker for PLC {PlcId} crashed.", plcId);
                throw; // Triggers shutdown via unobserved exception
            }
        }, cancellationToken);

        this.gatewayWorkers[plcId] = worker;
        this.runningWorkers[plcId] = workerTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Stopping all workers...");
        await this.cts.CancelAsync().ConfigureAwait(false);

        try
        {
            await Task.WhenAll(this.runningWorkers.Values).ConfigureAwait(false);
            this.logger.LogInformation("All workers stopped cleanly.");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error while stopping one or more workers.");
        }

        this.runningWorkers.Clear();

        this.logger.LogInformation("All workers stopped.");
    }

    private async Task<List<string>> WaitForSimulationCommands(TimeSpan timeout, CancellationToken cancellationToken)
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
                    // _logger.LogInformation("Empty input received — ending input collection.");
                    break;
                }

                collectedLines.Add(line);
            }

            await Task.Delay(100, cancellationToken).ConfigureAwait(false); // Polling delay
        }

        return collectedLines;
    }

    private async Task SimulateEventFromKeyboard(List<string> inputLines, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            return;
        }

        // Step 1: Parse all valid Request lines
        var commands = SimulatedCommand.ParseCommands(inputLines, this.logger);

        if (commands.Count == 0)
        {
            this.logger.LogTrace("No valid commands found.");
            return;
        }

        // Step 2: ProcessAsync each valid Request to its worker in parallel
        var simulationTasks = new List<Task>();

        foreach (var command in commands)
        {
            if (this.gatewayWorkers.TryGetValue(command.MachineId, out GatewayWorker? worker))
            {
                simulationTasks.Add(worker.TriggerCommand(command));
            }
            else
            {
                this.logger.LogWarning("No worker found for MachineId {MachineId}", command.MachineId);
            }
        }

        await Task.WhenAll(simulationTasks).ConfigureAwait(false);
    }

    private void DiscoverDuplicatedTags(ApplicationConfiguration configuration)
    {
        if (configuration?.Plcs == null)
        {
            throw new ArgumentNullException(nameof(configuration), "PLC configuration is missing.");
        }

        var criticalKeys = new[] { "PartNumber", "BarCode", "PlcId", "HeartBeat", "Command" };
        var errors = new List<PlcConfigurationException>();

        var plcGroupsByIp = configuration.Plcs
            .GroupBy(p => p.IpAddress)
            .Where(g => g.Count() > 1);

        foreach (var group in plcGroupsByIp)
        {
            foreach (var key in criticalKeys)
            {
                var addressGroups = group
                    .Where(plc => plc.Variables.ContainsKey(key))
                    .GroupBy(plc => plc.Variables[key].Address);

                foreach (var addressGroup in addressGroups)
                {
                    if (addressGroup.Count() > 1)
                    {
                        var ip = group.Key;
                        var duplicateAddress = addressGroup.Key;
                        var affectedPlcs = string.Join(", ", addressGroup.Select(p => p.PlcId));

                        var message = $"FATAL CONFIGURATION ERROR: PLCs with IP '{ip}' share address '{duplicateAddress}' for key '{key}'. Affected PLCs: {affectedPlcs}.";

                        this.logger.LogCritical("{Message} Application will terminate.", message);

                        errors.Add(new PlcConfigurationException(ip, key, duplicateAddress, message));
                    }
                }
            }
        }

        if (errors.Any())
        {
            var combinedMessage = string.Join(Environment.NewLine, errors.Select(e => e.Message));
            throw new AggregateException("PLC configuration errors found:\n" + combinedMessage, errors);
        }
    }
}
