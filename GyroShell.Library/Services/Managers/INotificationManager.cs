﻿using System;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace GyroShell.Library.Services.Managers
{
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
    }
}
