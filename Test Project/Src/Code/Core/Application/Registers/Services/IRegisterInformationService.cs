// <copyright file="IRegisterInformationService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Services
{
    /// <summary>
    /// Provides services for retrieving and managing register information within the IndTrace system.
    /// </summary>
    /// <remarks>
    /// This service interface defines methods for accessing register data and historical trends,
    /// supporting both current register information and time-series data analysis.
    /// </remarks>
    public interface IRegisterInformationService
    {
        /// <summary>
        /// Retrieves a list of all available registers in the system.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="Result{T}"/> with an enumerable of <see cref="RegistersRecords"/> if successful,
        /// or error information if the operation fails.
        /// </returns>
        /// <remarks>
        /// This method provides access to the complete catalog of registers available for monitoring and analysis.
        /// </remarks>
        public Task<Result<IEnumerable<RegistersRecords>>> GetListOfAvailableRegisters();

        /// <summary>
        /// Retrieves historical trend data for the specified registers.
        /// </summary>
        /// <param name="variables">The collection of register records for which to retrieve trend data.</param>
        /// <param name="maxItems">The maximum number of data points to return. Defaults to 100.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="Result{T}"/> with a dictionary where keys are tuples of (MachineId, Name)
        /// and values are collections of <see cref="TimeSeriesDataPoint"/> representing the trend data.
        /// </returns>
        /// <remarks>
        /// This method enables time-series analysis by providing historical data points for specified registers.
        /// The data is organized by machine ID and register name for efficient access and analysis.
        /// </remarks>
        public Task<Result<Dictionary<(int MachineId, string Name),
                    IEnumerable<TimeSeriesDataPoint>>>>
            GetListRegisterTrends(IEnumerable<RegistersRecords> variables, int maxItems = 100);
    }
}
