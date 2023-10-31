using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GyroShell.Library.Commands;
using GyroShell.Library.Models.Hardware;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI.Core;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using GyroShell.Library.Constants;
using System.Diagnostics;
using Windows.UI.Notifications.Management;
using Windows.UI.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GyroShell.Library.ViewModels
{
    public partial class DefaultTaskbarViewModel : ObservableObject, IDisposable
    {
        private readonly IEnvironmentInfoService m_envService;
        private readonly ISettingsService m_appSettings;
        private readonly IInternalLauncher m_internalLauncher;
        private readonly IDispatcherService m_dispatcherService;
        private readonly ITimeService m_timeService;

        private readonly IAppHelperService m_appHelper;
        private readonly IBitmapHelperService m_bmpHelper;

        private readonly ITaskbarManagerService m_tbManager;
        private readonly INotificationManager m_notifManager;

        private readonly INetworkService m_netService;
        private readonly IBatteryService m_powerService;
        private readonly ISoundService m_soundService;

        public StartFlyoutCommands StartFlyoutCommands { get; }

        public DefaultTaskbarViewModel(
            IEnvironmentInfoService envService,
            ISettingsService appSettings,
            IAppHelperService appHelper,
            IBitmapHelperService bmpHelper,
            ITaskbarManagerService tbManager,
            INotificationManager notifManager,
            INetworkService netService,
            IBatteryService powerService,
            ISoundService soundService,
            IInternalLauncher internalLauncher,
            IDispatcherService dispatcherService,
            ITimeService timeService)
        {
            m_envService = envService;
            m_appSettings = appSettings;
            m_appHelper = appHelper;
            m_bmpHelper = bmpHelper;
            m_tbManager = tbManager;
            m_netService = netService;
            m_powerService = powerService;
            m_soundService = soundService;
            m_internalLauncher = internalLauncher;
            m_dispatcherService = dispatcherService;
            m_notifManager = notifManager;
            m_timeService = timeService;

            StartFlyoutCommands = new StartFlyoutCommands(m_envService, m_internalLauncher);

            m_soundService.OnVolumeChanged += SoundService_OnVolumeChanged;
            AudioCheck();

            m_powerService.BatteryStatusChanged += BatteryService_BatteryStatusChanged;
            DetectBatteryPresence();

            m_netService.InternetStatusChanged += NetworkService_InternetStatusChanged;
            UpdateNetworkStatus();

            m_notifManager.NotifcationChanged += NotificationManager_NotificationChanged;
            Task.Run(UpdateNotifications).Wait();

            m_timeService.UpdateClockBinding += TimeService_UpdateClockBinding;
        }

        public FontFamily IconFontFamily => m_appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };

        public Thickness NetworkStatusMargin
        {
            get
            {
                switch (m_netService.InternetType)
                {
                    case InternetConnection.Wired:
                        return new Thickness(0, 2, 7, 0);
                    default:
                        return m_appSettings.IconStyle switch
                        {
                            0 => new Thickness(0, -2, 7, 0),
                            1 => new Thickness(0, 2, 7, 0),
                            _ => new Thickness(0, -2, 7, 0)
                        };
                }
            }
        }
        public Thickness BatteryStatusMargin
        {
            get
            {
                if (!m_powerService.IsBatteryInstalled) { return new Thickness(0); }
                return m_appSettings.IconStyle switch
                {
                    0 => new Thickness(0, 2, 12, 0),
                    1 => new Thickness(0, 3, 14, 0),
                    _ => new Thickness(0, 2, 12, 0)
                };
            }
        }
        public Thickness SoundStatusMargin => m_appSettings.IconStyle switch
        {
            0 => new Thickness(6, 1, 0, 0),
            1 => new Thickness(5, 0, 0, 0),
            _ => new Thickness(6, 1, 0, 0)
        };


        public HorizontalAlignment TaskbarIconAlignment => m_appSettings.TaskbarAlignment switch
        {
            0 => HorizontalAlignment.Left,
            1 => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Left
        };


        [RelayCommand]
        public void SystemControlsChecked()
        {
            if (m_envService.IsWindows11) { m_tbManager.ToggleControlCenter(); }
            else { m_tbManager.ToggleActionCenter(); }
        }

        [RelayCommand]
        public void StartButtonChecked()
        {
            m_tbManager.ToggleStartMenu();
        }

        [RelayCommand]
        public void ActionCenterChecked()
        {
            m_tbManager.ToggleActionCenter();
        }

        [RelayCommand]
        public void SystemTrayClicked()
        {
            throw new NotImplementedException("Systray not ready yet.");
            //await TaskbarManager.ShowSysTray(); /* Does nothing, no action */
        }


        #region Audio
        [ObservableProperty]
        private string soundStatusText;

        [ObservableProperty]
        private Visibility soundBackIconVisibility;

        private void SoundService_OnVolumeChanged(object sender, EventArgs e)
        {
            AudioCheck();
        }
        private void AudioCheck()
        {
            int currentVolume = m_soundService.Volume;
            string statusTextBuf = "\uEA85";

            if (currentVolume == 0 || currentVolume == -1)
            {
                statusTextBuf = "\uE992";
            }
            else
            {
                switch (currentVolume)
                {
                    case int volume when volume <= 33:
                        statusTextBuf = "\uE993";
                        break;
                    case int volume when volume <= 66:
                        statusTextBuf = "\uE994";
                        break;
                    case int volume when volume <= 100:
                        statusTextBuf = "\uE995";
                        break;
                }
            }

            if (m_soundService.IsMuted)
            {
                statusTextBuf = "\uE198";
            }

            m_dispatcherService.DispatcherQueue.TryEnqueue(() =>
            {
                SoundStatusText = statusTextBuf;
                SoundBackIconVisibility = (statusTextBuf == "\uE198") ? Visibility.Collapsed : Visibility.Visible;
            });
        }
        #endregion

        #region Battery
        [ObservableProperty]
        private string batteryStatusCharacter;

        [ObservableProperty]
        private Visibility battStatusVisibility;

        private void BatteryService_BatteryStatusChanged(object sender, EventArgs e)
        {
            m_dispatcherService.DispatcherQueue.TryEnqueue(() =>
            {
                AggregateBattery();
            });
        }
        private void DetectBatteryPresence()
        {
            BatteryReport report = m_powerService.GetStatusReport();
            BatteryPowerStatus status = report.PowerStatus;

            if (status == BatteryPowerStatus.NotInstalled)
            {
                BattStatusVisibility = Visibility.Collapsed;
            }
            else
            {
                BattStatusVisibility = Visibility.Visible;
                AggregateBattery();
            }
        }
        private void AggregateBattery()
        {
            BatteryReport report = m_powerService.GetStatusReport();
            double battLevel = report.ChargePercentage;
            BatteryPowerStatus status = report.PowerStatus;
            if (status == BatteryPowerStatus.Charging || status == BatteryPowerStatus.Idle)
            {
                int indexCharge = (int)Math.Floor(battLevel / 10);
                BatteryStatusCharacter = UIConstants.BatteryIconsCharge[indexCharge];
            }
            else
            {
                int indexDischarge = (int)Math.Floor(battLevel / 10);
                BatteryStatusCharacter = UIConstants.BatteryIcons[indexDischarge];
            }
        }
        #endregion

        #region Internet
        [ObservableProperty]
        private string networkStatusCharacter;

        private void NetworkService_InternetStatusChanged(object sender, EventArgs e)
        {
            UpdateNetworkStatus();
        }
        private void UpdateNetworkStatus()
        {
            string statusTextBuf = "\uE774";
            if (m_netService.IsInternetAvailable)
            {
                switch (m_netService.InternetType)
                {
                    case InternetConnection.Wired:
                        statusTextBuf = "\uE839";
                        OnPropertyChanged(nameof(NetworkStatusMargin));
                        break;
                    case InternetConnection.Wireless:
                        statusTextBuf = UIConstants.WiFiIcons[m_netService.SignalStrength];
                        break;
                    case InternetConnection.Data:
                        statusTextBuf = UIConstants.DataIcons[m_netService.SignalStrength];
                        break;
                    case InternetConnection.Unknown:
                    default:
                        statusTextBuf = "\uE774";
                        break;
                }
            }
            else
            {
                statusTextBuf = "\uEB55";
            }
            m_dispatcherService.DispatcherQueue.TryEnqueue(() =>
            {
                NetworkStatusCharacter = statusTextBuf;
            });
        }
        #endregion

        #region Notifications
        [ObservableProperty]
        private Visibility notifIndicatorVisibility;

        private void NotificationManager_NotificationChanged(object sender, EventArgs e)
        {
            UpdateNotifications();
        }
        private async Task UpdateNotifications()
        {
            IReadOnlyList<UserNotification> notifsToast = await m_notifManager.NotificationListener.GetNotificationsAsync(NotificationKinds.Toast);
            IReadOnlyList<UserNotification> notifsOther = await m_notifManager.NotificationListener.GetNotificationsAsync(NotificationKinds.Unknown);

            m_dispatcherService.DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
            {
                NotifIndicatorVisibility = notifsToast.Count > 0 || notifsOther.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        #endregion

        #region Clock
        [ObservableProperty]
        private string timeText;

        [ObservableProperty]
        private string dateText;

        private void TimeService_UpdateClockBinding(object sender, EventArgs e)
        {
            m_dispatcherService.DispatcherQueue.TryEnqueue(() =>
            {
                TimeText = DateTime.Now.ToString(m_timeService.ClockFormat);
                DateText = DateTime.Now.ToString(m_timeService.DateFormat);
            });
        }
        #endregion


        public void Dispose()
        {
            m_soundService.OnVolumeChanged -= SoundService_OnVolumeChanged;
            m_powerService.BatteryStatusChanged -= BatteryService_BatteryStatusChanged;
            m_netService.InternetStatusChanged -= NetworkService_InternetStatusChanged;
            m_timeService.UpdateClockBinding -= TimeService_UpdateClockBinding;
        }
    }
}