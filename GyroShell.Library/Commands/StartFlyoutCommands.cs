using CommunityToolkit.Mvvm.Input;
using GyroShell.Library.Services.Environment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace GyroShell.Library.Commands
{
    public partial class StartFlyoutCommands
    {
        private readonly IEnvironmentInfoService m_envService;
        private readonly IpublicLauncher m_publicLauncher;

        public StartFlyoutCommands(IEnvironmentInfoService envService, IpublicLauncher publicLauncher)
        {
            m_envService = envService;
            m_publicLauncher = publicLauncher;
        }

        [RelayCommand]
        public void OpenShellSettings()
        {
            m_envService.SettingsInstances++;

            if (m_envService.SettingsInstances <= 1)
            {
                m_publicLauncher.LaunchShellSettings();
            }
        }

        [RelayCommand]
        public void RestartGyroShell()
        {
            m_publicLauncher.LaunchNewShellInstance();
        }

        [RelayCommand]
        public void LaunchTaskManager()
        {
            m_publicLauncher.LaunchProcess("taskmgr.exe", false, true);
        }

        [RelayCommand]
        public async Task LaunchWindowsSettings()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:"));
        }

        [RelayCommand]
        public void LaunchWindowsExplorer()
        {
            Process.Start("explorer.exe");
        }

        [RelayCommand]
        public void LaunchRunDialog()
        {
            Process.Start("explorer.exe", "shell:::{2559a1f3-21d7-11d4-bdaf-00c04f60b9f0}");
        }

        [RelayCommand]
        public void SignOutWindows()
        {
            // TODO: Implement
        }

        [RelayCommand]
        public void SleepWindows()
        {
            // TODO: Implement
        }

        [RelayCommand]
        public void ShutdownWindows()
        {
            m_publicLauncher.LaunchProcess("shutdown /s /t 00", false, true);
        }

        [RelayCommand]
        public void RestartWindows()
        {
            m_publicLauncher.LaunchProcess("shutdown /r /t 00", false, true);
        }

        [RelayCommand]
        public void ShowDesktop()
        {
            // TODO: Implement
        }
    }
}
