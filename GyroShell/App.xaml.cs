#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

namespace GyroShell
{
    public partial class App : Application
    {
        internal static StartupWindow startupScreen;
        private Window m_window;
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
