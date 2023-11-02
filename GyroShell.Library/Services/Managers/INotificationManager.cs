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
