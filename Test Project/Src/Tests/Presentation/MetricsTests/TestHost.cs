using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndTrace.Application.UnitTests
{
    /// <summary>
    /// Provides a test host for configuring services in unit tests.
    /// </summary>
    public class TestHost
    {
        /// <summary>
        /// Configures the host with the specified services.
        /// </summary>
        /// <param name="hostContext">The host builder context.</param>
        /// <param name="services">The service collection to configure.</param>
        public static void ConfigureHost(HostBuilderContext hostContext, IServiceCollection services)
        {
            var startup = new Startup();
            startup.ConfigureServices(services);
        }
    }
}
