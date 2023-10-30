using GyroShell.Library.Services.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace GyroShell.Services.Managers
{
    internal class NotificationManager : INotificationManager
    {
        public UserNotificationListener NotificationListener { get; init; }

        public NotificationManager() 
        {
            NotificationListener = UserNotificationListener.Current;
            Task.Run(InitializeEventAsync);
        }

        private async Task<bool> InitializeEventAsync()
        {
            UserNotificationListenerAccessStatus accessStatus = await NotificationListener.RequestAccessAsync();
            return accessStatus == UserNotificationListenerAccessStatus.Allowed;
        }

        public event EventHandler<UserNotificationChangedEventArgs> NotifcationChanged;

        private void NotificationManager_NotificationChanged(UserNotificationListener sender, UserNotificationChangedEventArgs args)
        {
            NotifcationChanged?.Invoke(this, args);
        }
    }
}
