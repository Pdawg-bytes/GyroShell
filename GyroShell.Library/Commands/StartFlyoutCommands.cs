#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using CommunityToolkit.Mvvm.Input;
using GyroShell.Library.Services.Environment;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;

using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Library.Commands
{
    public partial class StartFlyoutCommands
    {
        private readonly IEnvironmentInfoService _envService;
        private readonly IInternalLauncher _internalLauncher;

        public StartFlyoutCommands(IEnvironmentInfoService envService, IInternalLauncher internalLauncher)
        {
            _envService = envService;
            _internalLauncher = internalLauncher;
        }

        [RelayCommand]
        public void OpenShellSettings()
        {
            _envService.SettingsInstances++;
            _internalLauncher.LaunchShellSettings();
        }

        [RelayCommand]
        public void RestartGyroShell()
        {
            _internalLauncher.LaunchNewShellInstance();
        }

        [RelayCommand]
        public void ExitGyroShell()
        {
            _internalLauncher.ExitGyroShell();
        }

        [RelayCommand]
        public void LaunchTaskManager()
        {
            _internalLauncher.LaunchProcess("taskmgr.exe", false, true);
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
            ExitWindowsEx(EWX_LOGOFF, 0);
        }

        [RelayCommand]
        public void SleepWindows()
        {
            SetSuspendState(false, false, false);
        }

        [RelayCommand]
        public void ShutdownWindows()
        {
            _internalLauncher.LaunchProcess("shutdown /s /t 00", false, true);
        }

        [RelayCommand]
        public void RestartWindows()
        {
            _internalLauncher.LaunchProcess("shutdown /r /t 00", false, true);
        }

        [RelayCommand]
        public void ShowDesktop()
        {
            // TODO: Implement.
        }
    }
}
