// <copyright file="ControllerMonitor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.UI.Models
{
    using System.Timers;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// Represents a controller monitor for tracking PLC controller status and communication.
    /// </summary>
    public class ControllerMonitor : IMonitorFilter, IComparable<ControllerMonitor>, IEquatable<ControllerMonitor>
    {
        private readonly IDateTimeMachine dateTimeMachine;
        private readonly object @lock = new();
        private DateTime timeStamp;
        private bool isConnected;
        private const int TimeOut = 12; // Change this value to set the timeout from the database configuration
        private TimeSpan heartBeatInterval = TimeSpan.FromSeconds(TimeOut);

        /// <summary>
        /// Gets or sets the PlcId.
        /// </summary>
        public int PlcId { get; set; }

        /// <summary>
        /// Gets or sets the MachineId.
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        /// Gets or sets the PartNumber.
        /// </summary>
        public string PartNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Label.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the HeartBeat.
        /// </summary>
        public int HeartBeat { get; set; }

        /// <summary>
        /// Gets or sets the CyclesOk.
        /// </summary>
        public int CyclesOk { get; set; }

        /// <summary>
        /// Gets or sets the IP address for network communication with the controller.
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the descriptive name of the controller.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        public event EventHandler ConnectionLost = null!;

        public event EventHandler ConnectionRecovered = null!;

        public event EventHandler UpdateReceived = null!;

        /// <summary>
        /// Gets or sets the Parameters.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = [];

        /// <inheritdoc/>
        public DateTime TimeStamp
        {
            get => this.timeStamp;
            set
            {
                lock (this.@lock)
                {
                    if (this.IsConnectionAlive)
                    {
                        this.OnConnectionRecovered(EventArgs.Empty);
                    }

                    this.timeStamp = value;
                    this.RestartHeartBeatTimer();
                    this.isConnected = true;
                }

                this.OnUpdateReceived(EventArgs.Empty);  // Raise the UpdateReceived event
            }
        }

        public bool IsConnected
        {
            get
            {
                lock (this.@lock)
                {
                    return this.isConnected && this.IsConnectionAlive;  // Reflect actual desired state
                }
            }
        }

        /// <summary>
        /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
        /// </summary>
        public Timer HeartBeatTimer { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMonitor"/> class with optional date time machine dependency.
        /// </summary>
        /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
        public ControllerMonitor(IDateTimeMachine? dateTimeMachine = null)
        {
            this.dateTimeMachine = dateTimeMachine ?? new DateTimeMachine();

            // Initialize with default date 2020-01-01 when no dependency provided
            this.timeStamp = this.dateTimeMachine.Now;
            this.InitializeHeartBeatTimer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMonitor"/> class with a name and optional date time machine dependency.
        /// </summary>
        /// <param name="name">The name of the controller.</param>
        /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
        public ControllerMonitor(string name, IDateTimeMachine? dateTimeMachine = null)
            : this(dateTimeMachine)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMonitor"/> class with PLC and machine IDs and optional date time machine dependency.
        /// </summary>
        /// <param name="plcId">The PLC identifier.</param>
        /// <param name="machineId">The machine identifier.</param>
        /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
        public ControllerMonitor(int plcId, int machineId, IDateTimeMachine? dateTimeMachine = null)
            : this(dateTimeMachine)
        {
            this.PlcId = plcId;
            this.MachineId = machineId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMonitor"/> class with a name, timestamp, and optional date time machine dependency.
        /// </summary>
        /// <param name="name">The name of the controller.</param>
        /// <param name="timeStamp">The initial timestamp.</param>
        /// <param name="dateTimeMachine">The date time machine to use for timestamp operations. Defaults to new DateTimeMachine() if null.</param>
        public ControllerMonitor(string name, DateTime timeStamp, IDateTimeMachine? dateTimeMachine = null)
            : this(dateTimeMachine)
        {
            this.Name = name;
            this.timeStamp = timeStamp;
        }

        // Override GetHashCode to ensure that equal objects have the same hash code

        /// <summary>
        /// Executes GetHashCode operation.
        /// </summary>
        /// <returns>The result of GetHashCode.</returns>
        public override int GetHashCode()
        {
            return this.PlcId.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMonitor"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        public ControllerMonitor()
            : this(null)
        {
        }

        private void InitializeHeartBeatTimer()
        {
            this.HeartBeatTimer = new Timer(this.heartBeatInterval.TotalMilliseconds);
            this.HeartBeatTimer.Elapsed += this.OnHeartBeatTimerElapsed;
            this.HeartBeatTimer.AutoReset = false; // Timer should not restart automatically
        }

        private void RestartHeartBeatTimer()
        {
            this.HeartBeatTimer.Stop();
            this.HeartBeatTimer.Start();
        }

        private void OnHeartBeatTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            lock (this.@lock)
            {
                this.isConnected = false;
            }

            this.HeartBeatTimer.Stop();
            this.OnConnectionLost(EventArgs.Empty);
        }

        protected virtual void OnConnectionLost(EventArgs e)
        {
            this.ConnectionLost?.Invoke(this, e);
        }

        protected virtual void OnConnectionRecovered(EventArgs e)
        {
            this.ConnectionRecovered?.Invoke(this, e);
        }

        protected virtual void OnUpdateReceived(EventArgs e)
        {
            this.UpdateReceived?.Invoke(this, e);
        }

        /// <inheritdoc/>
        int IComparable<ControllerMonitor>.CompareTo(ControllerMonitor? other)
        {
            if (other is null)
            {
                return 1;
            }

            return this.PlcId.CompareTo(other.PlcId);
        }

        // Override the base Equals method

        /// <summary>
        /// Executes Equals operation.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The result of Equals.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is ControllerMonitor other)
            {
                // Call the explicit interface implementation directly to avoid recursion
                return this.PlcId == other.PlcId;
            }

            return false;
        }

        /// <inheritdoc/>
        bool IEquatable<ControllerMonitor>.Equals(ControllerMonitor? other)
        {
            return this.PlcId == other?.PlcId;
        }

        /// <summary>
        /// Executes RefreshConnection operation.
        /// </summary>
        public void RefreshConnection()
        {
            lock (this.@lock)
            {
                this.TimeStamp = this.dateTimeMachine.Now.AddSeconds(-TimeOut - 1);
                this.RestartHeartBeatTimer();
            }
        }

        public bool IsConnectionAlive => (this.dateTimeMachine.Now - this.timeStamp) < this.heartBeatInterval;

        /// <summary>
        /// Executes MapTo operation.
        /// </summary>
        /// <param name="src">The src.</param>
        /// <returns>The result of MapTo.</returns>
        public static IndQuestResults.Result<ControllerMonitor> MapTo(PlcDto src)
        {
            if (src == null)
            {
                return IndQuestResults.Result<ControllerMonitor>.WithFailure("PlcDto source cannot be null");
            }

            return IndQuestResults.Result<ControllerMonitor>.Success(new ControllerMonitor
            {
                PlcId = src.PlcId,
                MachineId = src.MachineId,
                Name = src.Name,
                IpAddress = src.IpAddress,
                Description = src.PlcType, // or src.Description if available

                // Map other properties as needed
            });
        }

        /// <summary>
        /// Executes ToEntity operation.
        /// </summary>
        /// <param name="src">The src.</param>
        /// <returns>The result of ToEntity.</returns>
        public static IndQuestResults.Result<PlcDto> ToEntity(ControllerMonitor src)
        {
            if (src == null)
            {
                return IndQuestResults.Result<PlcDto>.WithFailure("ControllerMonitor source cannot be null");
            }

            return IndQuestResults.Result<PlcDto>.Success(new PlcDto
            {
                PlcId = src.PlcId,
                MachineId = src.MachineId,
                Name = src.Name,
                IpAddress = src.IpAddress,
                PlcType = src.Description, // or src.PlcType if available

                // Map other properties as needed
            });
        }

        /// <summary>
        /// Executes ToString operation.
        /// </summary>
        /// <returns>The result of ToString.</returns>
        public override string ToString()
        {
            return $"PlcId: {this.PlcId}, MachineId: {this.MachineId}, IpAddress: {this.IpAddress}, Name: {this.Name}";
        }

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate controller monitor input and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
        // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated monitoring or logging logic. Refactor for maintainability if necessary.
        // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency monitoring operations, consider optimizing data collection and event handling.
    }
}
