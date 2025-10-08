// <copyright file="IMonitorFilter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces
{
    /// <summary>
    /// Represents a filter for monitoring operations in the system.
    /// </summary>
    public interface IMonitorFilter
    {
        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the part number associated with the filter.
        /// </summary>
        string PartNumber { get; }

        /// <summary>
        /// Gets the timestamp of the filter.
        /// </summary>
        DateTime TimeStamp { get; }
    }
}
