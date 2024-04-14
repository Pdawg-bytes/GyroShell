﻿#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
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
