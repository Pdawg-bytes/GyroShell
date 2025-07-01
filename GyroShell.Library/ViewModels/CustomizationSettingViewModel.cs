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
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Managers;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Notifications.Management;

namespace GyroShell.Library.ViewModels
{
    public partial class CustomizationSettingViewModel : ObservableObject
    {
        private readonly ISettingsService _appSettings;
        private readonly IInternalLauncher _internalLauncher;
        private readonly IEnvironmentInfoService _envService;
        private readonly INotificationManager _notifManager;

        public CustomizationSettingViewModel(ISettingsService appSettings, IInternalLauncher internalLauncher, IEnvironmentInfoService envService, INotificationManager notifManager)
        {
            _appSettings = appSettings;
            _internalLauncher = internalLauncher;
            _envService = envService;
            _notifManager = notifManager;

            IsRestartInfoOpen = false;
            IsNotifInfoOpen = _notifManager.NotificationAccessStatus != UserNotificationListenerAccessStatus.Allowed;

            _appSettings.SettingUpdated += AppSettings_SettingUpdated;
        }

        private void AppSettings_SettingUpdated(object sender, string key)
        {
            switch (key)
            {
                case "iconStyle": OnPropertyChanged(nameof(IconFontFamily)); break;
            }
        }

        public FontFamily IconFontFamily => _appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };


        public bool IsWindows11 => _envService.IsWindows11;


        public bool Is24HourToggleChecked
        {
            get => _appSettings.EnableMilitaryTime;
            set => _appSettings.EnableMilitaryTime = value;
        }

        public bool IsSecondToggleChecked
        {
            get => _appSettings.EnableSeconds;
            set => _appSettings.EnableSeconds = value;
        }


        [RelayCommand]
        public void IconStyleSelection(string iconStyle)
        {
            switch (iconStyle)
            {
                case "Icon10":
                default:
                    _appSettings.IconStyle = 0;
                    break;
                case "Icon11":
                    _appSettings.IconStyle = 1;
                    break;
            }
        }
        public bool Icon10Selected
        {
            get => _appSettings.IconStyle == 0;
        }
        public bool Icon11Selected
        {
            get => _appSettings.IconStyle == 1;
        }


        [ObservableProperty]
        private bool isRestartInfoOpen;

        [RelayCommand]
        public void RestartGyroShell()
        {
            _internalLauncher.LaunchNewShellInstance();
        }

        [RelayCommand]
        public void HideRestartInfo()
        {
            IsRestartInfoOpen = false;
        }


        [ObservableProperty]
        private bool isNotifInfoOpen;

        [RelayCommand]
        public async Task NotifInfoSettings()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
        }

        [RelayCommand]
        public void HideNotifInfo()
        {
            IsNotifInfoOpen = false;
        }


        public int CurrentTransparencyTypeIndex
        {
            get => _appSettings.TransparencyType;
            set
            {
                if (!_appSettings.EnableCustomTransparency) { DefaultTransparencySettings(); }

                _appSettings.TransparencyType = value;
                OnPropertyChanged(nameof(LuminositySliderEnabled));
                OnPropertyChanged(nameof(LuminosityOpacity));
            }
        }

        public bool LuminositySliderEnabled
        {
            get => _appSettings.TransparencyType == 2;
        }
        public int LuminosityOpacity
        {
            get => (int)Math.Round((decimal)_appSettings.LuminosityOpacity * 100, 1);
            set
            {
                _appSettings.LuminosityOpacity = (float)value / 100;
                _appSettings.EnableCustomTransparency = true;

                IsRestartInfoOpen = true;
            }
        }
        public int TintOpacity
        {
            get => (int)Math.Round((decimal)_appSettings.TintOpacity * 100, 1);
            set
            {
                _appSettings.TintOpacity = (float)value / 100;
                _appSettings.EnableCustomTransparency = true;

                IsRestartInfoOpen = true;
            }
        }

        public Color TransparencyColorPickerValue
        {
            get => Color.FromArgb(_appSettings.AlphaTint, _appSettings.RedTint, _appSettings.GreenTint, _appSettings.BlueTint);
            set
            {
                _appSettings.AlphaTint = value.A;
                _appSettings.RedTint = value.R;
                _appSettings.GreenTint = value.G;
                _appSettings.BlueTint = value.B;
                _appSettings.EnableCustomTransparency = true;

                IsRestartInfoOpen = true;
            }
        }

        [RelayCommand]
        public void DefaultTransparencySettings()
        {
            if (_envService.IsSystemUsingDarkmode)
            {
                TransparencyColorPickerValue = Color.FromArgb(255, 32, 32, 32);
            }
            else
            {
                TransparencyColorPickerValue = Color.FromArgb(255, 232, 232, 232);
            }
            TintOpacity = 50;
            LuminosityOpacity = 96;
            CurrentTransparencyTypeIndex = 2;

            OnPropertyChanged(nameof(TransparencyColorPickerValue));
            OnPropertyChanged(nameof(TintOpacity));
            OnPropertyChanged(nameof(LuminosityOpacity));
            OnPropertyChanged(nameof(CurrentTransparencyTypeIndex));

            _appSettings.EnableCustomTransparency = false;
        }


        public int CurrentAlignmentIndex
        {
            get => _appSettings.TaskbarAlignment;
            set => _appSettings.TaskbarAlignment = value;
        }
    }
}
