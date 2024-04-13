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