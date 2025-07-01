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
using System.Timers;
using GyroShell.Views;
using System.Threading;
using System.Diagnostics;
using GyroShell.Library.Services.Managers;
using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;

using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Controls
{
    internal partial class StartupWindow : Library.Helpers.Window.ShellWindow
    {
        private IExplorerManagerService m_explorerManager;

        private System.Timers.Timer timer;

        private int appProcessId;
        private Process appProcess;
        private Thread timeThread;

        private const int WIDTH = 550;
        private const int HEIGHT = 300;

        internal StartupWindow() 
            : base(App.ServiceProvider.GetRequiredService<ISettingsService>(), 
                  width: WIDTH, height: HEIGHT, 
                  x: (GetSystemMetrics(SM_CXSCREEN) - WIDTH) / 2, y: (GetSystemMetrics(SM_CYSCREEN) - HEIGHT) / 2,
                  round: true,
                  customTransparency: false)
        {
            this.InitializeComponent();
            RootPageFrame.Navigate(typeof(StartupPage));

            base.Title = "GyroShell Startup Host";

            m_explorerManager = App.ServiceProvider.GetRequiredService<IExplorerManagerService>();

            appProcessId = Process.GetCurrentProcess().Id;
            appProcess = Process.GetProcessById(appProcessId);

            timer = new System.Timers.Timer(10000);
            timer.Elapsed += CloseTimer_Elapsed;
            timer.Start();

            base.Closed += (_, _) => timer.Stop();
        }


        private void CloseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeThread = new Thread(Kill);
            timeThread.Start();
            timer.Stop();
        }

        private void Kill()
        {
            MessageBox(base.WindowHandle, "If you keep seeing this message, please contact the developers.", "GyroShell was unable to start.", 0x00000000 | 0x00000030);
            m_explorerManager.ShowTaskbar();
            appProcess.Kill();
        }
    }
}