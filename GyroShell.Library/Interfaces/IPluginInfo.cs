using GyroShell.Library.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Interfaces
{
    public interface IPluginInfo
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A brief description of the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The publisher name of the plugin.
        /// </summary>
        string Publisher { get; }

        /// <summary>
        /// The version of the plugin.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// The Guid associated with the plugin.
        /// </summary>
        Guid PluginId { get; }

        /// <summary>
        /// The required services required for the plugin to function.
        /// </summary>
        ServiceType[] RequiredServices { get; }
    }
}