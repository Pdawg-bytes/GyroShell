using GyroShell.Library.Models.InternalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GyroShell.Library.Services.Managers
{
    /// <summary>
    /// Defines the service that handles plugins.
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Loads and executes a plugin.
        /// </summary>
        public void LoadAndRunPlugin(string pluginName);

        /// <summary>
        /// Gets a list of plugins without executing them.
        /// </summary>
        public List<PluginUIModel> GetPlugins();

        /// <summary>
        /// Unloads a running plugin.
        /// </summary>
        public void UnloadPlugin(string pluginName);

        /// <summary>
        /// If any plugins are in the unload queue.
        /// </summary>
        public bool IsUnloadRestartPending { get; set; }
    }
}