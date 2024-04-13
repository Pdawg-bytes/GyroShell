#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using GyroShell.Library.Models.InternalData;
using System.Collections.Generic;

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