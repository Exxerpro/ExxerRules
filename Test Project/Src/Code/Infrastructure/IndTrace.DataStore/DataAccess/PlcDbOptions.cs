using System.Collections.Generic;
using System.Text;

namespace IndTrace.DataStore.DataAccess
{
    /// <summary>
    /// Represents options for configuring PLC database connections.
    /// </summary>
    public class PlcDbOptions
    {
        /// <summary>
        /// Gets or sets the connection string for the PLC database.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }
}
