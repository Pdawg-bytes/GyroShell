#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GyroShell.Library.Commands;
using GyroShell.Library.Models.Hardware;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Managers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI.Core;
using GyroShell.Library.Constants;
using Windows.UI.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GyroShell.Library.Models.InternalData;

namespace GyroShell.Library.ViewModels
{
    public partial class DefaultTaskbarViewModel : ObservableObject, IDisposable
    {
        private readonly IEnvironmentInfoService _envService;
        private readonly ISettingsService _appSettings;
        private readonly IInternalLauncher _internalLauncher;
        private readonly IDispatcherService _dispatcherService;
        private readonly ITimeService _timeService;
        private readonly IShellHookService _shellHookService;

        private readonly IExplorerManagerService _explorerManager;
        private readonly INotificationManager _notifManager;

        private readonly INetworkService _netService;
        private readonly IBatteryService _powerService;
        private readonly ISoundService _soundService;

        public StartFlyoutCommands StartFlyoutCommands { get; }

        public DefaultTaskbarViewModel(
            IEnvironmentInfoService envService,
            ISettingsService appSettings,
            IExplorerManagerService explorerManager,
            INotificationManager notifManager,
            INetworkService netService,
            IBatteryService powerService,
            ISoundService soundService,
            IInternalLauncher internalLauncher,
            IDispatcherService dispatcherService,
            ITimeService timeService,
            IShellHookService shellHookService)
        {
            _envService = envService;
            _appSettings = appSettings;
            _explorerManager = explorerManager;
            _netService = netService;
            _powerService = powerService;
            _soundService = soundService;
            _internalLauncher = internalLauncher;
            _dispatcherService = dispatcherService;
            _notifManager = notifManager;
            _timeService = timeService;
            _shellHookService = shellHookService;

            StartFlyoutCommands = new StartFlyoutCommands(_envService, _internalLauncher);

            _soundService.OnVolumeChanged += SoundService_OnVolumeChanged;
            AudioCheck();

            _powerService.BatteryStatusChanged += BatteryService_BatteryStatusChanged;
            DetectBatteryPresence();

            _netService.InternetStatusChanged += NetworkService_InternetStatusChanged;
            UpdateNetworkStatus();

            _notifManager.NotifcationChanged += NotificationManager_NotificationChanged;
            //Task.Run(UpdateNotifications).Wait();

            _timeService.UpdateClockBinding += TimeService_UpdateClockBinding;

            _appSettings.SettingUpdated += AppSettings_SettingUpdated;
        }

        public ObservableCollection<WindowModel> CurrentWindows => _shellHookService.CurrentWindows;

        private void AppSettings_SettingUpdated(object sender, string key)
        {
            switch (key)
            {
                case "iconStyle": 
                    OnPropertyChanged(nameof(IconFontFamily)); 
                    OnPropertyChanged(nameof(NetworkStatusMargin));
                    OnPropertyChanged(nameof(BatteryStatusMargin));
                    OnPropertyChanged(nameof(SoundStatusMargin));
                    break;
                case "tbAlignment": OnPropertyChanged(nameof(TaskbarIconAlignment)); break;
            }
        }

        public FontFamily IconFontFamily => _appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };

        public Thickness NetworkStatusMargin
        {
            get
            {
                switch (_netService.InternetType)
                {
                    case InternetConnection.Wired:
                        return new Thickness(0, 2, 7, 0);
                    default:
                        return _appSettings.IconStyle switch
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
                if (!_powerService.IsBatteryInstalled) { return new Thickness(0); }
                return _appSettings.IconStyle switch
                {
                    0 => new Thickness(0, 2, 12, 0),
                    1 => new Thickness(0, 3, 14, 0),
                    _ => new Thickness(0, 2, 12, 0)
                };
            }
        }

        public Thickness SoundStatusMargin => _appSettings.IconStyle switch
        {
            0 => new Thickness(6, 1, 0, 0),
            1 => new Thickness(5, 0, 0, 0),
            _ => new Thickness(6, 1, 0, 0)
        };

        public HorizontalAlignment TaskbarIconAlignment => _appSettings.TaskbarAlignment switch
        {
            0 => HorizontalAlignment.Left,
            1 => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Left
        };


        [RelayCommand]
        public void SystemControlsChecked()
        {
            if (_envService.IsWindows11) { _explorerManager.ToggleControlCenter(); }
            else { _explorerManager.ToggleActionCenter(); }
        }

        [RelayCommand]
        public void StartButtonChecked()
        {
            _explorerManager.ToggleStartMenu();
        }

        [RelayCommand]
        public void ActionCenterChecked()
        {
            _explorerManager.ToggleActionCenter();
        }

        [RelayCommand]
        public void SystemTrayClicked()
        {
            throw new NotImplementedException("Systray not ready yet.");
            //await TaskbarManager.ShowSysTray(); /* Does nothing, no action */
        }

        [ObservableProperty]
        private bool isStartOpen;
        [ObservableProperty]
        private bool isActionCenterOpen;
        [ObservableProperty]
        private bool isSystemControlsOpen;


        #region Sound
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
            int currentVolume = _soundService.Volume;
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

            if (_soundService.IsMuted)
            {
                statusTextBuf = "\uE198";
            }

            _dispatcherService.DispatcherQueue.TryEnqueue(() =>
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
            _dispatcherService.DispatcherQueue.TryEnqueue(() =>
            {
                AggregateBattery();
            });
        }
        private void DetectBatteryPresence()
        {
            BatteryReport report = _powerService.GetStatusReport();
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
            BatteryReport report = _powerService.GetStatusReport();
            double battLevel = report.ChargePercentage;
            BatteryPowerStatus status = report.PowerStatus;
            if (status == BatteryPowerStatus.Charging || status == BatteryPowerStatus.Idle)
            {
                int indexCharge = (int)Math.Floor(battLevel / 10);
                BatteryStatusCharacter = IconConstants.BatteryIconsCharge[indexCharge];
            }
            else
            {
                int indexDischarge = (int)Math.Floor(battLevel / 10);
                BatteryStatusCharacter = IconConstants.BatteryIcons[indexDischarge];
            }
        }
        #endregion


        #region Network
        [ObservableProperty]
        private string networkStatusCharacter;

        [ObservableProperty]
        private string networkBackCharacter;

        private void NetworkService_InternetStatusChanged(object sender, EventArgs e)
        {
            UpdateNetworkStatus();
        }
        private void UpdateNetworkStatus()
        {
            string statusTextBuf = "\uE774";
            string backTextBuf = "\uE774";
            if (_netService.IsInternetAvailable)
            {
                switch (_netService.InternetType)
                {
                    case InternetConnection.Wired:
                        statusTextBuf = "\uE839";
                        backTextBuf = "\uE839";
                        OnPropertyChanged(nameof(NetworkStatusMargin));
                        break;
                    case InternetConnection.Wireless:
                        statusTextBuf = IconConstants.WiFiIcons[(int)_netService.SignalStrength];
                        backTextBuf = "\uE701";
                        break;
                    case InternetConnection.Data:
                        statusTextBuf = IconConstants.DataIcons[(int)_netService.SignalStrength];
                        backTextBuf = "\uEC3B";
                        break;
                    case InternetConnection.Unknown:
                    default:
                        statusTextBuf = "\uE774";
                        backTextBuf = "\uE774";
                        break;
                }
            }
            else
            {
                statusTextBuf = "\uEB55";
                backTextBuf = "\uEB55";
            }
            _dispatcherService.DispatcherQueue.TryEnqueue(() =>
            {
                NetworkStatusCharacter = statusTextBuf;
                NetworkBackCharacter = backTextBuf;
            });
        }
        #endregion


        #region Notifications
        [ObservableProperty]
        private Visibility notifIndicatorVisibility;

        private void NotificationManager_NotificationChanged(object sender, EventArgs e)
        {
            UpdateNotifications().Wait();
        }
        private async Task UpdateNotifications()
        {
            IReadOnlyList<UserNotification> notifsToast = await _notifManager.NotificationListener.GetNotificationsAsync(NotificationKinds.Toast);
            IReadOnlyList<UserNotification> notifsOther = await _notifManager.NotificationListener.GetNotificationsAsync(NotificationKinds.Unknown);

            _dispatcherService.DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
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

        [ObservableProperty]
        private bool openTest = true;

        private void TimeService_UpdateClockBinding(object sender, EventArgs e)
        {
            _dispatcherService.DispatcherQueue.TryEnqueue(() =>
            {
                DateTime now = DateTime.Now;
                TimeText = now.ToString(_timeService.ClockFormat);
                DateText = now.ToString(_timeService.DateFormat);
            });
        }
        #endregion


        public void Dispose()
        {
            _soundService.OnVolumeChanged -= SoundService_OnVolumeChanged;
            _powerService.BatteryStatusChanged -= BatteryService_BatteryStatusChanged;
            _netService.InternetStatusChanged -= NetworkService_InternetStatusChanged;
            _notifManager.NotifcationChanged -= NotificationManager_NotificationChanged;
            _timeService.UpdateClockBinding -= TimeService_UpdateClockBinding;
            _appSettings.SettingUpdated -= AppSettings_SettingUpdated;
        }
    }
}