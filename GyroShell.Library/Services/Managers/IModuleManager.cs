using GyroShell.Library.Models.InternalData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Services.Managers
{
    /// <summary>
    /// Defines the service that handles modules.
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// Initializes the module list.
        /// </summary>
        public void InitializeModuleList(string directory);

        /// <summary>
        /// Loads and execute modules in the list.
        /// </summary>
        public void LoadAndRunModules();

        /// <summary>
        /// Gets a list of modules without executing them.
        /// </summary>
        public List<ModuleModel> GetModules();

        /// <summary>
        /// Unloads all running modules.
        /// </summary>
        public void UnloadModules();
    }
}
