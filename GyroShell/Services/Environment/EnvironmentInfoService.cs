#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using System;
using System.IO;
using Windows.ApplicationModel;
using GyroShell.Library.Services.Environment;
using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Services.Environment
{
    public class EnvironmentInfoService : IEnvironmentInfoService
    {
        public string SystemArchitecture { get; init; }
        public Version AppVersion { get; init; }
        public DateTime AppBuildDate { get; init; }


        private IntPtr _mainWindowHandle;
        public IntPtr MainWindowHandle
        {
            get => _mainWindowHandle;
            set => _mainWindowHandle = value;
        }

        private int _settingsInstances;
        public int SettingsInstances
        {
            get => _settingsInstances;
            set => _settingsInstances = value;
        }

        public bool IsSystemUsingDarkmode
        {
            get => ShouldSystemUseDarkMode(); 
        }

        public bool IsWindows11
        {
            get => System.Environment.OSVersion.Version.Build >= 22000;
        }

        public int MonitorWidth
        {
            get => GetSystemMetrics(SM_CXSCREEN);
        }

        public int MonitorHeight
        {
            get => GetSystemMetrics(SM_CYSCREEN);
        }

        public EnvironmentInfoService()
        {
            Package package = Package.Current;
            SystemArchitecture = package.Id.Architecture.ToString();

            PackageVersion version = package.Id.Version;
            AppVersion = new Version(version.Major, version.Minor, version.Build, version.Revision);

            string mainExecutablePath = Path.Combine(Package.Current.InstalledLocation.Path, "GyroShell.exe");
            AppBuildDate = File.GetLastWriteTime(mainExecutablePath);
        }
    }
}