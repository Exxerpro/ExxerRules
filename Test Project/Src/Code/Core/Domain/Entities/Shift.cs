// <copyright file="Shift.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities
{
    using IndTrace.Domain.Enum;
    using IndTrace.Domain.Interfaces;
    using IndTrace.Domain.Models;

    /// <summary>
    /// Represents a work shift, including timing, type, and duration constraints.
    /// </summary>
    public class Shift : AuditableEntity, IEntityRoot
    {
        private readonly IDateTimeMachine dateTimeMachine;

        // [Fix]
        // CLAUDE
        // Date: 25/08/2025
        // Reason: EF Core fix - Add private parameterless constructor for entity materialization

        /// <summary>
        /// Initializes a new instance of the <see cref="Shift"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="dateTimeMachine">The dateTimeMachine.</param>
        public Shift(IDateTimeMachine dateTimeMachine)
        {
            this.dateTimeMachine = dateTimeMachine;
            this.ShiftType = string.Empty;
            this.Type = Enum.ShiftType.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shift"/> class.
        /// Private parameterless constructor for EF Core entity materialization.
        /// </summary>
        private Shift()
        {
            this.dateTimeMachine = new DateTimeMachine(); // Default implementation for EF Core
            this.ShiftType = string.Empty;
            this.Type = Enum.ShiftType.None;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the shift.
        /// </summary>
        public int ShiftId { get; set; }

        /// <summary>
        /// Gets or sets the start time of the shift.
        /// </summary>
        public DateTime StartBy { get; set; }

        /// <summary>
        /// Gets or sets the duration of the shift.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the end time of the shift.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the type of the shift (e.g., Morning, Evening, Night).
        /// </summary>
        public string ShiftType { get; set; } = null!; // Morning, Evening, Night, etc.

        /// <summary>
        /// Gets or sets the maximum allowed duration for the shift.
        /// </summary>
        public TimeSpan MaxDuration { get; set; } = new TimeSpan(16, 0, 0);

        /// <summary>
        /// Gets or sets the minimum allowed duration for the shift.
        /// </summary>
        public TimeSpan MinDuration { get; set; } = new TimeSpan(16, 0, 0);

        /// <summary>
        /// Gets or sets the normal duration for the shift.
        /// </summary>
        public TimeSpan NormalDuration { get; set; } = new TimeSpan(8, 30, 0);

        /// <summary>
        /// Gets a value indicating whether the shift is currently running.
        /// </summary>
        public bool IsRunningNow => this.StartBy <= this.dateTimeMachine.Now && this.dateTimeMachine.Now <= this.StartBy + this.Duration;

        /// <summary>
        /// Gets or sets the number of successful cycles during the shift.
        /// </summary>
        public int CyclesOk { get; set; }

        /// <summary>
        /// Gets or sets the type enumeration for the shift.
        /// </summary>
        public ShiftType Type { get; set; }
    }
}
