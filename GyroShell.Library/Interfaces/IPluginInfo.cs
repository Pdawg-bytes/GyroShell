#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Enums;
using System;

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