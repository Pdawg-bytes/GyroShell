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

using System;

namespace GyroShell.Library.Services.Hardware
{
    public enum InternetConnection
    {
        /// <summary>
        /// A wired connection, usually Ethernet.
        /// </summary>
        Wired,

        /// <summary>
        /// A wireless connection, usually Wi-Fi.
        /// </summary>
        Wireless,

        /// <summary>
        /// A satellite or mobile data connection.
        /// </summary>
        Data,

        /// <summary>
        /// An unknown type of connection, or no connection at all.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Defines a platform-agnostic service interface to get network information.
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// The type of internet connection in use.
        /// </summary>
        public InternetConnection InternetType { get; }

        /// <summary>
        /// Checks if an internet connection is available.
        /// </summary>
        public bool IsInternetAvailable { get; }

        /// <summary>
        /// Gets the signal strength of the connection in case of a wireless one.
        /// </summary>
        /// <remarks>Returns 0 on wired or unknown connections.</remarks>
        public byte SignalStrength { get; }

        /// <summary>
        /// An event raised when the network's status changes.
        /// </summary>
        public event EventHandler InternetStatusChanged;
    }
}
