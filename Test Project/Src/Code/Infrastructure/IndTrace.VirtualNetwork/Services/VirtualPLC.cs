using IndTrace.VirtualNetwork.Exceptions;
using IndTrace.VirtualNetwork.Snap7;
using Sharp7.Rx;

namespace IndTrace.VirtualNetwork.Services
{
    /// <summary>
    /// Represents a virtual PLC that simulates the behavior of a real PLC, including server and client functionalities.
    /// </summary>
    public class VirtualPlc : IDisposable
    {
        /// <summary>
        /// Gets the Sharp7 PLC connection instance.
        /// </summary>
        public readonly Sharp7Plc S7Conn;

        private readonly PlcData virtualPlc;
        private readonly S7Server server;
        private readonly ILogger<VirtualPlc> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPlc"/> class.
        /// </summary>
        /// <param name="plcData">The PLC data containing configuration and tag information.</param>
        /// <param name="logger">The _logger instance for logging messages.</param>

        private VariableS7 heartBeat = new();

        private VariableS7 plcId = new();
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPlc"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="virtualPlc">The virtualPlc.</param>
        /// <param name="logger">The logger.</param>

        public VirtualPlc(PlcData virtualPlc, ILogger<VirtualPlc> logger)
        {
            if (virtualPlc is null)
            {
                throw new ArgumentNullException(nameof(virtualPlc));
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.virtualPlc = virtualPlc;
            this.logger = logger;
            logger.LogInformation("Initializing plc service...{Name} , {IpAddress}", this.virtualPlc.Name, this.virtualPlc.IpAddress);
            this.server = new S7Server();
            this.S7Conn = new Sharp7Plc(this.virtualPlc.IpAddress, 0, 2, port: this.virtualPlc.Port);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VirtualPlc"/> class.
        /// </summary>
        ~VirtualPlc()
        {
            this.logger.LogInformation("Stopping plc server...");
            this.server.Stop();
        }

        /// <summary>
        /// Gets or sets a dictionary of IndTrace tags associated with the PLC.
        /// </summary>
        public IDictionary<string, TagDataStore> IndTraceTags { get; set; } = new Dictionary<string, TagDataStore>();

        /// <summary>
        /// Disposes the <see cref="VirtualPlc"/> instance, stopping the server and releasing resources.
        /// </summary>
        public void Dispose()
        {
            this.logger.LogInformation("Stopping plc server...");
            this.server.Stop();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Retrieves the machine ID from the PLC.
        /// </summary>
        /// <returns>The machine ID read from the PLC, or 0 if an error occurs.</returns>
        public async Task<short> GetMachineIdFromPlc()
        {
            try
            {
                await this.S7Conn.InitializeConnection();

                var plcId = await this.S7Conn.GetValue<short>(this.plcId.Address);
                this.logger.LogInformation(
                    "MachineId: {MachineId}, PlcId: {PlcId}, IP Address: {IpAddress}, DBs: {@DBs}",
                    this.virtualPlc.MachineId,
                    plcId,
                    this.virtualPlc.IpAddress,
                    this.virtualPlc.DataBlocks.Values.Select(d => d.Name).ToArray()
                );
                return plcId;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
            }
            return 0;
        }

        /// <summary>
        /// Initializes the virtual PLC client, connecting to the server and setting initial PLC ID.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitializeClientAsync()
        {
            await this.S7Conn.InitializeConnection();
            await Task.Delay(250);

            try
            {
                if (this.virtualPlc.Variables.TryGetValue("PlcId", out var plcIdVar))
                {
                    this.plcId = plcIdVar;
                }
                else
                {
                    this.logger.LogInformation("PlcId not found in variables");
                }
                if (this.virtualPlc.Variables.TryGetValue("HeartBeat", out var heartBeatVar))
                {
                    this.heartBeat = heartBeatVar;
                }
                else
                {
                    this.logger.LogInformation("HeartBeat not found in variables");
                }

                this.IndTraceTags = this.virtualPlc.Variables.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new TagDataStore
                    {
                        Address = kvp.Value.Address,
                        Description = kvp.Key, // optional if Name == Key
                    }
                );

                await this.S7Conn.SetValue<short>(this.plcId.Address, (short)this.virtualPlc.MachineId);
                await Task.Delay(250);

                await this.S7Conn.InitializeConnection();
                await Task.Delay(250);

                var taskStatus = this.S7Conn.InitializeConnection().Status;
                this.logger.LogInformation("Connection Task status: {taskStatus}", taskStatus);
                await Task.Delay(50);

                var plcId = await this.S7Conn.GetValue<short>(this.plcId.Address);
                this.logger.LogInformation(
                    "MachineId: {MachineId}, PlcId: {PlcId}, IP Address: {IpAddress}, DBs: {@DBs}",
                    this.virtualPlc.MachineId,
                    plcId,
                    this.virtualPlc.IpAddress,
                    this.virtualPlc.DataBlocks.Values.Select(d => d.Name).ToArray()
                );
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
            }

            await Task.Delay(100);
        }

        /// <summary>
        /// Initializes the virtual PLC server, starting it and registering data areas.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitializeServerAsync()
        {
            var port = this.virtualPlc.Port;
            this.server.SetParam(S7Consts.PU16LocalPort, ref port);
            var result = this.server.StartTo(this.virtualPlc.IpAddress);

            this.logger.LogInformation(result != 0
                ? $"Error starting plc server: {result}"
                : $"VirtualNetworkPlcData server started successfully {this.virtualPlc.IpAddress} Port {this.virtualPlc.Port}");

            await Task.Delay(100);

            foreach (var (id, db) in this.virtualPlc.DataBlocks)
            {
                this.server.RegisterArea(S7Server.SrvAreaDb, id, ref this.virtualPlc.DataBlocks[id].Data, this.virtualPlc.DataBlocks[id].Data.Length);
                await Task.Delay(50);
            }
        }

        /// <summary>
        /// Sets the heartbeat value in the PLC.
        /// </summary>
        /// <param name="seconds">The heartbeat value in seconds.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetHeartBeat(int seconds)
        {
            try
            {
                await this.S7Conn.InitializeConnection();
                await this.S7Conn.SetValue<short>(this.heartBeat.Address, (short)seconds);
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
            }
        }

        /// <summary>
        /// Sets the machine ID to the PLC asynchronously.
        /// </summary>
        /// <param name="plcId">The PLC ID to set.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The PLC ID that was set, or -1 if an error occurs.</returns>
        public async Task<short> SetMachineIdToPlcAsync(short plcId, CancellationToken cancellationToken)
        {
            try
            {
                await this.S7Conn.InitializeConnection();

                await this.S7Conn.SetValue<short>(this.plcId.Address, plcId, cancellationToken);
                await Task.Delay(10, cancellationToken);
                var id = await this.S7Conn.GetValue<short>(this.plcId.Address, cancellationToken);

                this.logger.LogInformation(
                    "MachineId: {MachineId}, PlcId: {PlcId}, IP Address: {IpAddress}, DBs: {@DBs}",
                    this.virtualPlc.MachineId,
                    plcId,
                    this.virtualPlc.IpAddress,
                    this.virtualPlc.DataBlocks.Values.Select(d => d.Name).ToArray()
                );

                return id;
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Updates the data in a specific data block.
        /// </summary>
        /// <param name="id">The ID of the data block to update.</param>
        /// <param name="data">The new data to write to the data block.</param>
        /// <exception cref="DbOutOfRangeException">Thrown if the data block ID is invalid.</exception>
        /// <exception cref="DbNotFoundException">Thrown if the data block is not found.</exception>
        /// <exception cref="DateExceedsDbLengthException">Thrown if the provided data exceeds the data block's length.</exception>
        public void UpdateDataBlockData(int id, byte[] data)
        {
            DbOutOfRangeException.ThrowIfInvalid(id);

            if (!this.virtualPlc.DataBlocks.TryGetValue(id, out var db))
            {
                throw new DbNotFoundException(id);
            }

            if (data.Length > db.Data.Length)
            {
                throw new DateExceedsDbLengthException(id, db.Data.Length, data.Length);
            }

            Array.Copy(data, this.virtualPlc.DataBlocks[id].Data, data.Length);
        }

        /// <summary>
        /// Maps variable records to IndTrace tags.
        /// </summary>
        /// <param name="variables">The dictionary of variables to map.</param>
        private void MapVariableRecordsToIndTraceTags(Dictionary<string, VariableS7> variables)
        {
            this.IndTraceTags = variables.ToDictionary(
                kvp => kvp.Key,
                kvp => new TagDataStore
                {
                    Address = kvp.Value.Address,
                    Description = kvp.Key,
                }
            );
        }
    }
}
