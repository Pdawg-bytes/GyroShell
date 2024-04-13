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