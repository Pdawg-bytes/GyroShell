#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Models.Plugins;

namespace GyroShell.Library.Interfaces
{
    public interface IPlugin
    {
        public IPluginInfo PluginInformation { get; }

        void Initialize(PluginServiceContext serviceContext);

        void Shutdown();
    }
}