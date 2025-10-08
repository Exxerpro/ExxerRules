// <copyright file="MainLineConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Configuration.Services
{
    /// <summary>
    /// Represents the configuration for the main production line, including printers, clients, and product associations.
    /// </summary>
    public class MainLineConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether there are multiple initial printers.
        /// </summary>
        public bool HasMultipleInitialPrinters { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether there are multiple clients.
        /// </summary>
        public bool HasMultipleClients { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of initial printer IDs.
        /// </summary>
        public List<int> InitialPrinterIds { get; set; } = [];

        /// <summary>
        /// Gets or sets the dictionary mapping client names to lists of product names.
        /// </summary>
        public Dictionary<string, List<string>> DictClientsProducts { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of printer-product associations.
        /// </summary>
        public List<PrinterProductAssociation> Associations { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of active customers.
        /// </summary>
        public IEnumerable<CustomerDto> ActiveCustomer { get; set; } = [];
    }
}
