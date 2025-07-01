#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;

namespace GyroShell.Controls
{
    public sealed partial class StartWindow : Window
    {
        private AppWindow _appWindow;

        public StartWindow()
        {
            this.InitializeComponent();

            // Removes titlebar
            OverlappedPresenter presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = true;
            presenter.SetBorderAndTitleBar(false, false);
            _appWindow = GetAppWindowForCurrentWindow();
            _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            _appWindow.Show();

            // Resize Window
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            Title = "GyroShell Start Host";
            appWindow.MoveInZOrderAtTop();
        }

        private OverlappedPresenter GetAppWindowAndPresenter()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId WndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow _apw = AppWindow.GetFromWindowId(WndId);

            return _apw.Presenter as OverlappedPresenter;
        }
        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWndApp = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId WndIdApp = Win32Interop.GetWindowIdFromWindow(hWndApp);

            return AppWindow.GetFromWindowId(WndIdApp);
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {

        }
    }
}
