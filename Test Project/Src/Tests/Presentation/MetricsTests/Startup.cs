using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndTrace.Application.UnitTests
{
    /// <summary>
    /// Provides startup configuration for unit test services.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configures services required for unit tests.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add any services that your tests require here.
            // For example, services.AddTransient<MyService>();
        }
    }
}
