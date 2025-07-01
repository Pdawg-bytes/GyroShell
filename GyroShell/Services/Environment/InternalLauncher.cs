﻿#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
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
        private readonly IEnvironmentInfoService _envService;

        public InternalLauncher(IEnvironmentInfoService envService) 
        {
            _envService = envService;
        }

        public void LaunchShellSettings()
        {
            if (_envService.SettingsInstances > 1) return; 

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
