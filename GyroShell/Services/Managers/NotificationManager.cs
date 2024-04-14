#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Services.Managers;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace GyroShell.Services.Managers
{
    internal class NotificationManager : INotificationManager, IDisposable
    {
        public UserNotificationListener NotificationListener { get; init; }

        public UserNotificationListenerAccessStatus NotificationAccessStatus { get; set; }

        public NotificationManager() 
        {
            if (!ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                NotificationAccessStatus = UserNotificationListenerAccessStatus.Unspecified;
                return;
            }

            NotificationListener = UserNotificationListener.Current;
            if (Task.Run(async () => await InitializeEventAsync()).Result)
            {
                NotificationListener.NotificationChanged += NotificationManager_NotificationChanged;
            }
            else
            {
                Task.Run(RequestNotificationAccess).Wait();
            }
        }

        private async Task<bool> InitializeEventAsync()
        {
            UserNotificationListenerAccessStatus accessStatus = await NotificationListener.RequestAccessAsync();
            NotificationAccessStatus = accessStatus;
            return accessStatus == UserNotificationListenerAccessStatus.Allowed;
        }

        private async Task RequestNotificationAccess()
        {
            NotificationAccessStatus = await NotificationListener.RequestAccessAsync();
        }

        public event EventHandler NotifcationChanged;

        private void NotificationManager_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
        {
            NotifcationChanged?.Invoke(this, EventArgs.Empty);
        }


        public void Dispose()
        {
            NotificationListener.NotificationChanged -= NotificationManager_NotificationChanged;
        }
    }
}
