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
        }


        public FontFamily IconFontFamily => m_appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };


        public bool IsWindows11 => m_envService.IsWindows11;

        #region Clock settings
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
        #endregion


        #region Icon style
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
        #endregion


        #region Restart Information
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
        #endregion


        #region Notification Information
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
        #endregion


        #region Transparency
        public int CurrentTransparencyTypeIndex
        {
            get => m_appSettings.TransparencyType;
            set
            {
                switch (value)
                {
                    case 0:
                    case 1:
                        LuminosityOpacity = 0; 
                        LuminositySliderEnabled = false;
                        if (!m_appSettings.EnableCustomTransparency) { DefaultTransparencySettings(); }
                        break;
                    case 2:
                        LuminositySliderEnabled = true;
                        if (!m_appSettings.EnableCustomTransparency) { DefaultTransparencySettings(); }
                        break;
                }
                m_appSettings.TransparencyType = value;
            }
        }

        [ObservableProperty]
        private bool luminositySliderEnabled;
        public int LuminosityOpacity
        {
            get => (int)Math.Round((decimal)m_appSettings.LuminosityOpacity * 100, 1);
            set
            {
                m_appSettings.LuminosityOpacity = value / 100;
                m_appSettings.EnableCustomTransparency = true;

                IsRestartInfoOpen = true;
            }
        }
        public int TintOpacity
        {
            get => (int)Math.Round((decimal)m_appSettings.TintOpacity * 100, 1);
            set
            {
                m_appSettings.TintOpacity = value / 100;
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
            m_appSettings.AlphaTint = 255;
            m_appSettings.RedTint = 32;
            m_appSettings.BlueTint = 32;
            m_appSettings.GreenTint = 32;
            m_appSettings.LuminosityOpacity = 0.95f;
            m_appSettings.TintOpacity = 0.0f;

            TintOpacity = 0;
            LuminosityOpacity = 95;

            TransparencyColorPickerValue = new Color { A = 255, R = 32, B = 32, G = 32 };

            m_appSettings.EnableCustomTransparency = false;
        }
        #endregion


        #region Taskbar Alignment
        public int CurrentAlignmentIndex
        {
            get => m_appSettings.TaskbarAlignment;
            set => m_appSettings.TaskbarAlignment = value;
        }
        #endregion
    }
}
