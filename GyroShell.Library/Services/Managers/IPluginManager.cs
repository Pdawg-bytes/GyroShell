#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
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