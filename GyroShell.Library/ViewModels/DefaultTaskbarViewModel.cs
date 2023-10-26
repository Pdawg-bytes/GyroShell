using CommunityToolkit.Mvvm.Input;
using GyroShell.Library.Commands;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.ViewModels
{
    public partial class DefaultTaskbarViewModel
    {
        private readonly IEnvironmentInfoService m_envService;
        private readonly ISettingsService m_appSettings;
        private readonly IInternalLauncher m_internalLauncher;

        private readonly IAppHelperService m_appHelper;
        private readonly IBitmapHelperService m_bmpHelper;

        private readonly ITaskbarManagerService m_tbManager;

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
            INetworkService netService,
            IBatteryService powerService,
            ISoundService soundService,
            IInternalLauncher internalLauncher)
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

            StartFlyoutCommands = new StartFlyoutCommands(m_envService, m_internalLauncher);
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
        public Thickness BatteryStatusMargin => m_appSettings.IconStyle switch
        {
            0 => new Thickness(0, 2, 12, 0),
            1 => new Thickness(0, 3, 14, 0),
            _ => new Thickness(0, 2, 12, 0)
        };
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
    }
}
