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

using GyroShell.Library.Services.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
