// <copyright file="ShiftDefinition.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities
{
    using System.Text;
    using IndTrace.Domain.Interfaces;
    using IndTrace.Domain.Models;

    /// <summary>
    /// Represents the definition of a shift, including catalog, plant, timing, and duration information.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 25/08/2025
    // Reason: EF Core entity interface fix - ShiftDefinition needs ILookupEntity for DbSet registration
    public class ShiftDefinition : AuditableEntity, ILookupEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftDefinition"/> class.
        /// Initializes a new instance of the class.
        /// </summary>
        public ShiftDefinition()
        {
            this.ShiftName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the shift catalog identifier.
        /// </summary>
        public int ShiftCatalogId { get; set; }

        /// <summary>
        /// Gets or sets the plant identifier associated with the shift.
        /// </summary>
        public int PlantId { get; set; }

        /// <summary>
        /// Gets or sets the name of the shift.
        /// </summary>
        public string ShiftName { get; set; }

        /// <summary>
        /// Gets or sets the start time of the shift.
        /// </summary>
        public TimeSpan StartBy { get; set; }

        /// <summary>
        /// Gets or sets the duration of the shift.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the end time of the shift.
        /// </summary>
        public TimeSpan EndTime { get; set; }
    }
}
