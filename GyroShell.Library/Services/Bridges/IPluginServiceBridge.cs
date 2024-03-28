using GyroShell.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Bridges
{
    public interface IPluginServiceBridge
    {
        /// <summary>
        /// This method generates a new ServiceProvider instance to initialize a plugin with.
        /// </summary>
        /// <param name="requestedServices">An array of the requested services needed for the plugin to function.</param>
        /// <returns>A new instance of the App's main <see cref="IServiceProvider"/> with the required services.</returns>
        public IServiceProvider CreatePluginServiceProvider(ServiceType[] requestedServices);
    }
}