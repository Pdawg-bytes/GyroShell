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

using GyroShell.Library.Models.Hardware;
using System;

namespace GyroShell.Library.Services.Hardware
{
    /// <summary>
    /// Defines a platform-agnostic service interface to get battery information.
    /// </summary>
    public interface IBatteryService
    {
        /// <summary>
        /// An event raised when the battery's status changes.
        /// </summary>
        public event EventHandler BatteryStatusChanged;

        /// <summary>
        /// Gets a status report of the system's battery.
        /// </summary>
        /// <returns>The new <see cref="BatteryReport"/> object.</returns>
        public BatteryReport GetStatusReport();

        /// <summary>
        /// Checks if there is a battery installed in the system.
        /// </summary>
        public bool IsBatteryInstalled { get; }
    }
}
