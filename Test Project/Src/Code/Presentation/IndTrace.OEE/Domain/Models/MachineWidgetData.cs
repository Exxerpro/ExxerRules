// <copyright file="MachineWidgetData.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Domain.Models
{
    /// <summary>
    /// Represents widget data for a machine, including OEE and related KPIs for dashboard display.
    /// </summary>
    public class MachineWidgetData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the machine.
        /// </summary>
        public string MachineId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current operational status of the machine (e.g., RUNNING, STOPPED).
        /// </summary>
        public string Status { get; set; } = string.Empty; // RUNNING, STOPPED, etc.

        /// <summary>
        /// Gets or sets the Overall Equipment Effectiveness (OEE) percentage.
        /// </summary>
        public double OEE { get; set; }

        /// <summary>
        /// Gets or sets the availability percentage of the machine.
        /// </summary>
        public double Availability { get; set; }

        /// <summary>
        /// Gets or sets the performance percentage of the machine.
        /// </summary>
        public double Performance { get; set; }

        /// <summary>
        /// Gets or sets the quality percentage of the machine output.
        /// </summary>
        public double Quality { get; set; }

        /// <summary>
        /// Gets or sets the OEE trend data points for sparkline visualization.
        /// </summary>
        public List<double> OeeTrend { get; set; } = new(); // Simplified trend for sparkline
    }
}
