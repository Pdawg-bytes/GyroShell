#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using System;
using Windows.UI.Notifications.Management;

namespace GyroShell.Library.Services.Managers
{
    /// <summary>
    /// Defines a WinRT based service to handle the user's notifcations.
    /// </summary>
    public interface INotificationManager
    {
        /// <summary>
        /// The current Notification Listener.
        /// </summary>
        public UserNotificationListener NotificationListener { get; init; }

        /// <summary>
        /// Event that is fired when any notification is changed.
        /// </summary>
        public event EventHandler NotifcationChanged;

        /// <summary>
        /// The accessibility of the user's notifications.
        /// </summary>
        public UserNotificationListenerAccessStatus NotificationAccessStatus { get; set; }
    }
}
