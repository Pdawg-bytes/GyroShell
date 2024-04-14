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