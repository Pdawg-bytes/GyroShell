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

using GyroShell.Controls;
using GyroShell.Helpers;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;

using GyroShell.Library.Services.Environment;

namespace GyroShell.Services.Environment
{
    public class InternalLauncher : IInternalLauncher
    {
        private readonly IEnvironmentInfoService m_envService;

        public InternalLauncher(IEnvironmentInfoService envService) 
        {
            m_envService = envService;
        }

        public void LaunchShellSettings()
        {
            if (m_envService.SettingsInstances > 1) return; 

            SettingsWindow _settingsWindow = new SettingsWindow();
            _settingsWindow.Activate();
        }

        public void ExitGyroShell()
        {
            App.Current.Exit();
        }

        public void LaunchNewShellInstance()
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = Process.GetCurrentProcess().MainModule.FileName, UseShellExecute = true });
                Application.Current.Exit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void LaunchProcess(string procName, bool createNoWindow, bool useShellEx)
        {
            Process.Start(ProcessStart.ProcessStartEx(procName, createNoWindow, useShellEx));
        }
    }
}
