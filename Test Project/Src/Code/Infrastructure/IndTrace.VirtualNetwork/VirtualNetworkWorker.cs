using IndTrace.VirtualNetwork.Extensions;
using IndTrace.VirtualNetwork.Services;
using IndTrace.VirtualNetwork.Simulation;
using System.Diagnostics;
using IndTrace.DataStore.ModelsComs;
using IndTrace.DataStore.DataAccess;

namespace IndTrace.VirtualNetwork;

/// <summary>
/// A background service that simulates a virtual network of PLCs.
/// </summary>
/// <param name="logger">The logger instance.</param>
/// <param name="idPlcDbRepository">The PLC DB tags repository.</param>
/// <param name="loggerFactory">The logger factory.</param>
/// <param name="commandSimulator">The command simulator.</param>
public class VirtualNetworkWorker(ILogger<VirtualNetworkWorker> logger, PlcDbTagsRepository idPlcDbRepository, LoggerFactory loggerFactory, CommandSimulator commandSimulator)
    : BackgroundService
{
    private readonly CommandSimulator commandSimulator = commandSimulator;

    /// <summary>
    /// Gets the dictionary of virtual PLCs in the network.
    /// </summary>
    public Dictionary<int, VirtualPlc> PlcNetwork { get; private set; } = new Dictionary<int, VirtualPlc>();

    /// <summary>
    /// Executes the background service logic.
    /// </summary>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this.InitializeAsync(stoppingToken);
        await Task.Delay(1_000, stoppingToken); // Wait for 1 seconds before fetching again

        var plc100 = this.PlcNetwork.FirstOrDefault(id => id.Key == 100).Value;

        if (plc100 != null)
        {
            await this.commandSimulator.WriteSimulatedPerformanceDataAsync(plc100, stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var commands = await this.commandSimulator.WaitForSimulationCommands(TimeSpan.FromMilliseconds(500), stoppingToken);

            var seconds = DateTime.Now.Second;

            if (seconds % 5 == 0)
            {
                foreach (var plc in this.PlcNetwork)
                {
                    await plc.Value.SetHeartBeat(seconds);
                }
            }

            if (seconds == 0)
            {
                foreach (var plc in this.PlcNetwork)
                {
                    var plcId = (short)plc.Key;
                    var result = await plc.Value.SetMachineIdToPlcAsync(plcId, stoppingToken);

                    if (plc.Key == 100)
                    {
                        await this.commandSimulator.WriteSimulatedPerformanceDataAsync(plc.Value, stoppingToken);
                    }

                    if (plcId != result)
                    {
                        logger.LogInformation("Problems with PLC {PlcId}", plcId);
                    }
                }
                logger.LogInformation("VirtualNetworkWorker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1_000, stoppingToken);
        }
    }

    /// <summary>
    /// Gets or sets the dictionary of PLCs.
    /// </summary>
    public Dictionary<int, PlcData> Plcs { get; set; } = new Dictionary<int, PlcData>();

    /// <summary>
    /// Initializes the virtual network worker, including loading PLC data and setting up network interfaces.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("VirtualNetworkWorker running at: {time}", DateTimeOffset.Now);

        var allplcs = await idPlcDbRepository.GetPlcsAsync(cancellationToken);

        var groupedTags = await idPlcDbRepository.GetTagsGroupedByMachineAsync(cancellationToken);

        var distinctDbInfosAsync = await idPlcDbRepository.GetDistinctDbInfosAsync(cancellationToken);

        var port = 102;
        //READ THE TAGS FOR EACH PLC
        foreach (var plc in allplcs)
        {
            groupedTags.TryGetValue(plc.MachineId, out var tagsForMachine);
            plc.Variables = tagsForMachine ?? [];
            logger.LogInformation("PLC Info: {Plc}, {IpAddres}", plc.PlcId, plc.IpAddress);
            plc.Port = port;
            logger.LogInformation("PLC: {PlcId}, {Name}, {IpAddress}, {PlcType}, {PlcBrand} ", plc.PlcId, plc.Name, plc.IpAddress, plc.PlcType, plc.PlcBrand);
        }
        //Convert the list of PLCs to a dictionary for easier access
        this.Plcs = allplcs.Where(p => p.Enabled == 1).ToDictionary(p => p.PlcId);

        Debug.Assert(this.Plcs is not null);
        Debug.Assert(distinctDbInfosAsync is not null);

        foreach (var db in distinctDbInfosAsync)
        {
            logger.LogInformation("DB Info: {Address}, {BlockNumber}", db.Name, db.Id);
        }

        foreach (var (key, plc) in this.Plcs)
        {
            //TODO 13 AGOSTO
            // ABR
            // SOLVE THE ISSUE THE SERVICE HANGS AFTER THE FIRST IP ADDED
            // AFTER REINITIALIZATION THE SERVICE WORKS FINE BECAUSE THE IP IS ALREADY ADDED,
            // THE ISSUE IS WHEN THE IP IS ADDED FOR THE FIRST TIME
            // SEARCH FOR THE ROOT CAUSE
            if (plc.IpAddressIsNotAssigned)
            {
                logger.LogInformation("Assigning IP:  {IpAddress}, for {PlcId}", plc.IpAddress, plc.PlcId);

                plc.AddIpAddressToNetwork(logger);

                await Task.Delay(2_500);

                logger.LogInformation("IP Address:  {IpAddress}, for {PlcId} exist ? {Exist}", plc.IpAddress, plc.PlcId, plc.IpAddressExist);
            }
            else
            {
                logger.LogInformation("IP Address:  {IpAddress}, for {PlcId} Already exist", plc.IpAddress, plc.PlcId);
            }

            foreach (var db in distinctDbInfosAsync)
            {
                //var size of the data blocks is 500 bytes
                var size = 500;

                var newDb = new Db(db.Id, size);

                plc.AddDb(newDb);
            }

            foreach (var (id, db) in plc.DataBlocks)
            {
                logger.LogInformation("Machine UserId {MachineInd}  DB Info:{Address}, {BlockNumber}", plc.MachineId, db.Name, db.Id);
            }

            var loggerVirtualPlc = loggerFactory.CreateLogger<Services.VirtualPlc>();

            var newPlc = new Services.VirtualPlc(plc, loggerVirtualPlc);

            plc.AddIpAddressToNetwork(logger);

            await Task.Delay(2_500);
            await newPlc.InitializeServerAsync();
            plc.AddIpAddressToNetwork(logger);

            await Task.Delay(2_500);
            await newPlc.InitializeClientAsync();
            plc.AddIpAddressToNetwork(logger);

            await Task.Delay(2_500);
            this.PlcNetwork.Add(plc.PlcId, newPlc);
        }
    }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate virtual network worker logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
