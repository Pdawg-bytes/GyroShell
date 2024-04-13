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
        private readonly ISettingsService m_appSettings;
        private readonly IInternalLauncher m_internalLauncher;
        private readonly IEnvironmentInfoService m_envService;
        private readonly INotificationManager m_notifManager;

        public CustomizationSettingViewModel(ISettingsService appSettings, IInternalLauncher internalLauncher, IEnvironmentInfoService envService, INotificationManager notifManager)
        {
            m_appSettings = appSettings;
            m_internalLauncher = internalLauncher;
            m_envService = envService;
            m_notifManager = notifManager;

            IsRestartInfoOpen = false;
            IsNotifInfoOpen = m_notifManager.NotificationAccessStatus != UserNotificationListenerAccessStatus.Allowed;

            m_appSettings.SettingUpdated += AppSettings_SettingUpdated;
        }

        private void AppSettings_SettingUpdated(object sender, string key)
        {
            switch (key)
            {
                case "iconStyle": OnPropertyChanged(nameof(IconFontFamily)); break;
            }
        }

        public FontFamily IconFontFamily => m_appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };


        public bool IsWindows11 => m_envService.IsWindows11;


        public bool Is24HourToggleChecked
        {
            get => m_appSettings.EnableMilitaryTime;
            set => m_appSettings.EnableMilitaryTime = value;
        }

        public bool IsSecondToggleChecked
        {
            get => m_appSettings.EnableSeconds;
            set => m_appSettings.EnableSeconds = value;
        }


        [RelayCommand]
        public void IconStyleSelection(string iconStyle)
        {
            switch (iconStyle)
            {
                case "Icon10":
                default:
                    m_appSettings.IconStyle = 0;
                    break;
                case "Icon11":
                    m_appSettings.IconStyle = 1;
                    break;
            }
        }
        public bool Icon10Selected
        {
            get => m_appSettings.IconStyle == 0;
        }
        public bool Icon11Selected
        {
            get => m_appSettings.IconStyle == 1;
        }


        [ObservableProperty]
        private bool isRestartInfoOpen;

        [RelayCommand]
        public void RestartGyroShell()
        {
            m_internalLauncher.LaunchNewShellInstance();
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
            get => m_appSettings.TransparencyType;
            set
            {
                if (!m_appSettings.EnableCustomTransparency) { DefaultTransparencySettings(); }

                m_appSettings.TransparencyType = value;
                OnPropertyChanged(nameof(LuminositySliderEnabled));
                OnPropertyChanged(nameof(LuminosityOpacity));
            }
        }

        public bool LuminositySliderEnabled
        {
            get => m_appSettings.TransparencyType == 2;
        }
        public int LuminosityOpacity
        {
            get => (int)Math.Round((decimal)m_appSettings.LuminosityOpacity * 100, 1);
            set
            {
                m_appSettings.LuminosityOpacity = (float)value / 100;
                m_appSettings.EnableCustomTransparency = true;

                IsRestartInfoOpen = true;
            }
        }
        public int TintOpacity
        {
            get => (int)Math.Round((decimal)m_appSettings.TintOpacity * 100, 1);
            set
            {
                m_appSettings.TintOpacity = (float)value / 100;
                m_appSettings.EnableCustomTransparency = true;

                IsRestartInfoOpen = true;
            }
        }

        public Color TransparencyColorPickerValue
        {
            get => Color.FromArgb(m_appSettings.AlphaTint, m_appSettings.RedTint, m_appSettings.GreenTint, m_appSettings.BlueTint);
            set
            {
                m_appSettings.AlphaTint = value.A;
                m_appSettings.RedTint = value.R;
                m_appSettings.GreenTint = value.G;
                m_appSettings.BlueTint = value.B;
                m_appSettings.EnableCustomTransparency = true;

                IsRestartInfoOpen = true;
            }
        }

        [RelayCommand]
        public void DefaultTransparencySettings()
        {
            if (m_envService.IsSystemUsingDarkmode)
            {
                TransparencyColorPickerValue = Color.FromArgb(255, 32, 32, 32);
            }
            else
            {
                TransparencyColorPickerValue = Color.FromArgb(255, 232, 232, 232);
            }
            TintOpacity = 0;
            LuminosityOpacity = 95;

            OnPropertyChanged(nameof(TransparencyColorPickerValue));
            OnPropertyChanged(nameof(TintOpacity));
            OnPropertyChanged(nameof(LuminosityOpacity));

            m_appSettings.EnableCustomTransparency = false;
        }


        public int CurrentAlignmentIndex
        {
            get => m_appSettings.TaskbarAlignment;
            set => m_appSettings.TaskbarAlignment = value;
        }
    }
}
