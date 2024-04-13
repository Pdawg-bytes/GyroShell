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

using GyroShell.Library.Services.Environment;
using System;
using System.IO;
using Windows.ApplicationModel;
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
            if (Package.Current.InstalledLocation != null)
            {
                Package package = Package.Current;
                SystemArchitecture = package.Id.Architecture.ToString();

                PackageVersion version = package.Id.Version;
                AppVersion = new Version(version.Major, version.Minor, version.Build, version.Revision);

                string mainExecutablePath = Path.Combine(Package.Current.InstalledLocation.Path, "GyroShell.exe");
                AppBuildDate = File.GetLastWriteTime(mainExecutablePath);
            }
            else
            {
                SYSTEM_INFO sysInfo = new SYSTEM_INFO();
                GetNativeSystemInfo(out sysInfo);

                ushort arch = sysInfo.wProcessorArchitecture;

                SystemArchitecture = arch switch
                {
                    0 => "X86",
                    5 => "ARM",
                    6 => "ARM64",
                    9 => "X64",
                    _ => "Unknown Processor",
                };

                AppVersion = new Version(0, 0, 0, 0);
                AppBuildDate = new DateTime(2023, 9, 28);
            }
        }
    }
}
