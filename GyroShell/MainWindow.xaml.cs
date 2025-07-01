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
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Media.Animation;
using GyroShell.Library.Services.Managers;
using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;

using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell
{
    internal sealed partial class MainWindow : Library.Helpers.Window.ShellWindow
    {
        private readonly IEnvironmentInfoService m_envService;
        private readonly IExplorerManagerService m_explorerManager;

        internal static int uCallBack;

        internal static bool _appBarRegistered = false;

        internal MainWindow()
            : base(App.ServiceProvider.GetRequiredService<ISettingsService>(), width: GetSystemMetrics(SM_CXSCREEN), height: 48, dockBottom: true)
        {
            this.InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += (_, _) => m_explorerManager.ShowTaskbar();

            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>();
            m_explorerManager = App.ServiceProvider.GetRequiredService<IExplorerManagerService>();

            m_explorerManager.Initialize();

            base.Title = "GyroShell";

            m_envService.MainWindowHandle = base.WindowHandle;

            IShellHookService shHookService = App.ServiceProvider.GetRequiredService<IShellHookService>();
            shHookService.MainWindowHandle = base.WindowHandle;
            shHookService.Initialize();

            RegisterBar();
            MonitorSummon();
            TaskbarFrame.Navigate(typeof(Controls.DefaultTaskbar), null, new SuppressNavigationTransitionInfo());
        }

        internal void MonitorSummon()
        {
            bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData)
            {
                return true;
            }

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
        }


        private void RegisterBar()
        {
            APPBARDATA abd = CreateAppBarData();

            if (!_appBarRegistered)
            {
                uCallBack = RegisterWindowMessage("AppBarMessage");
                abd.uCallbackMessage = uCallBack;

                SHAppBarMessage((int)ABMsg.ABM_NEW, ref abd);
                RegisterShellHookWindow(base.WindowHandle);

                _appBarRegistered = true;

                m_explorerManager.ToggleAutoHideExplorer(true);
                SetAppBarPosition();
                m_explorerManager.ToggleAutoHideExplorer(false);
                m_explorerManager.HideTaskbar();

                SetWindowPos(base.WindowHandle, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 0, 0, 0,
                    (int)(SWPFlags.SWP_NOMOVE | SWPFlags.SWP_NOSIZE | SWPFlags.SWP_SHOWWINDOW));
            }
            else
            {
                SHAppBarMessage((int)ABMsg.ABM_REMOVE, ref abd);
                DeregisterShellHookWindow(base.WindowHandle);
                _appBarRegistered = false;
            }
        }

        private void SetAppBarPosition()
        {
            APPBARDATA abd = CreateAppBarData();
            abd.uEdge = (int)ABEdge.ABE_BOTTOM;

            abd.rc.left = 0;
            abd.rc.right = m_envService.MonitorWidth;

            if (abd.uEdge == (int)ABEdge.ABE_TOP)
            {
                abd.rc.top = 0;
                abd.rc.bottom = base.Height;
            }
            else
            {
                abd.rc.bottom = m_envService.MonitorHeight;
                abd.rc.top = abd.rc.bottom - base.Height;
            }

            SHAppBarMessage((int)ABMsg.ABM_QUERYPOS, ref abd);
            AdjustAppBarBounds(ref abd);
            SHAppBarMessage((int)ABMsg.ABM_SETPOS, ref abd);

            MoveWindow(
                abd.hWnd,
                abd.rc.left,
                abd.rc.top,
                abd.rc.right - abd.rc.left,
                abd.rc.bottom - abd.rc.top,
                true
            );
        }

        private APPBARDATA CreateAppBarData()
        {
            return new APPBARDATA
            {
                cbSize = Marshal.SizeOf<APPBARDATA>(),
                hWnd = base.WindowHandle
            };
        }

        private void AdjustAppBarBounds(ref APPBARDATA abd)
        {
            switch ((ABEdge)abd.uEdge)
            {
                case ABEdge.ABE_LEFT:
                    abd.rc.right = abd.rc.left + base.Width;
                    break;
                case ABEdge.ABE_RIGHT:
                    abd.rc.left = abd.rc.right - base.Width;
                    break;
                case ABEdge.ABE_TOP:
                    abd.rc.bottom = abd.rc.top + base.Height;
                    break;
                case ABEdge.ABE_BOTTOM:
                    abd.rc.top = abd.rc.bottom - base.Height;
                    break;
            }
        }
    }
}