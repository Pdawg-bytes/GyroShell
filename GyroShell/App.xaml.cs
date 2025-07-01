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
using Microsoft.UI.Xaml;
using GyroShell.Controls;
using System.Threading.Tasks;
using GyroShell.Library.Helpers.Window;

namespace GyroShell
{
    public partial class App : Application
    {
        internal static StartupWindow startupScreen;
        private ShellWindow m_window;
        private IntPtr handle;

        public App()
        {
            this.InitializeComponent();
        }

        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ConfigureServices();
            PreloadServices();

            await LoadStartupScreenContentAsync();
            m_window = new MainWindow();
            m_window.Activate();
        }

        private async Task LoadStartupScreenContentAsync()
        {
            startupScreen = new StartupWindow();
            startupScreen.Activate();
            await Task.Delay(200);
        }
    }
}
