using GyroShell.Library.Models.InternalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Managers
{
    /// <summary>
    /// Defines the service that handles plugins.
    /// </summary>
    public interface IPluginManager
    {
        /// <summary>
        /// Initializes the plugin list.
        /// </summary>
        public void InitializePluginList(string directory);

        /// <summary>
        /// Loads and execute plugins in the list.
        /// </summary>
        public void LoadAndRunPlugins();

        /// <summary>
        /// Gets a list of plugins without executing them.
        /// </summary>
        public List<PluginUIModel> GetPlugins();

        /// <summary>
        /// Unloads all running plugins.
        /// </summary>
        public void UnloadPlugins();
    }
}
