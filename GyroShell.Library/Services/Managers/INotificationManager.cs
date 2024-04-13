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
using Windows.UI.Notifications;
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
