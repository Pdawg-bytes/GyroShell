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

using GyroShell.Helpers;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Graphics;
using Windows.UI;
using WinRT;

using static GyroShell.Library.Helpers.Win32.Win32Interop;
using static GyroShell.Library.Helpers.Win32.WindowChecks;

using AppWindow = Microsoft.UI.Windowing.AppWindow;

namespace GyroShell
{
    internal sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        private readonly IEnvironmentInfoService m_envService;
        private readonly ISettingsService m_appSettings;
        private readonly IExplorerManagerService m_explorerManager;

        internal static IntPtr hWnd;

        internal static int uCallBack;

        internal static bool fBarRegistered = false;

        internal MainWindow()
        {
            this.InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>();
            m_appSettings = App.ServiceProvider.GetRequiredService<ISettingsService>();
            m_explorerManager = App.ServiceProvider.GetRequiredService<IExplorerManagerService>();

            m_explorerManager.Initialize();

            // Presenter handling code
            OverlappedPresenter presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);
            m_AppWindow = GetAppWindowForCurrentWindow();
            m_AppWindow.SetPresenter(AppWindowPresenterKind.Default);

            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            m_envService.MainWindowHandle = hWnd;

            IShellHookService shHookService = App.ServiceProvider.GetRequiredService<IShellHookService>();
            shHookService.MainWindowHandle = hWnd;
            shHookService.Initialize();

            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            if (m_envService.IsWindows11)
            {
                DWMWINDOWATTRIBUTE attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                DWM_WINDOW_CORNER_PREFERENCE preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
                DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
            }

            // Hide in ALT+TAB view
            int exStyle = (int)GetWindowLongPtr(hWnd, -20);
            exStyle |= 128;
            SetWindowLongPtr(hWnd, -20, (IntPtr)exStyle);

            Thread.Sleep(20); //TODO: Stop the window message from moving our window into the wokring area

            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);
            int barHeight = 48;

            Title = "GyroShell";
            appWindow.Resize(new SizeInt32 { Width = screenWidth, Height = barHeight });
            appWindow.Move(new PointInt32 { X = 0, Y = screenHeight - barHeight });
            appWindow.MoveInZOrderAtTop();

            // Init stuff
            RegisterBar();
            MonitorSummon();
            TaskbarFrame.Navigate(typeof(Controls.DefaultTaskbar), null, new SuppressNavigationTransitionInfo());
            SetBackdrop();

            // Show GyroShell when everything is ready
            m_AppWindow.Show();
        }

        #region Window Handling
        private void OnProcessExit(object sender, EventArgs e)
        {
            m_explorerManager.ShowTaskbar();
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

        internal void MonitorSummon()
        {

            bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData)
            {
                return true;
            }

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
        }
        #endregion

        #region Backdrop Stuff
        WindowsSystemDispatcherQueueHelper m_wsdqHelper;
        MicaController micaController;
        DesktopAcrylicController acrylicController;
        SystemBackdropConfiguration m_configurationSource;

        private void SetBackdrop()
        {
            bool option = m_appSettings.EnableCustomTransparency;

            byte alpha = m_appSettings.AlphaTint;
            byte red = m_appSettings.RedTint;
            byte green = m_appSettings.GreenTint;
            byte blue = m_appSettings.BlueTint;

            float luminOpacity = m_appSettings.LuminosityOpacity;
            float tintOpacity = m_appSettings.TintOpacity;

            int transparencyType = m_appSettings.TransparencyType;

            switch (transparencyType)
            {
                case 0:
                default:
                    if (m_envService.IsWindows11)
                    {
                        TrySetMicaBackdrop(MicaKind.BaseAlt, alpha, red, green, blue, tintOpacity, luminOpacity, option);
                    }
                    else
                    {
                        TrySetAcrylicBackdrop(alpha, red, green, blue, tintOpacity, luminOpacity, option);
                    }
                    break;
                case 1:
                    if (m_envService.IsWindows11)
                    {
                        TrySetMicaBackdrop(MicaKind.Base, alpha, red, green, blue, tintOpacity, luminOpacity, option);
                    }
                    else
                    {
                        TrySetAcrylicBackdrop(alpha, red, green, blue, tintOpacity, luminOpacity, option);
                    }
                    break;
                case 2:
                    TrySetAcrylicBackdrop(alpha, red, green, blue, tintOpacity, luminOpacity, option);
                    break;
            }
        }

        bool TrySetMicaBackdrop(MicaKind micaKind, byte alpha, byte red, byte green, byte blue, float tintOpacity, float luminOpacity, bool customTransparency)
        {
            if (MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();
                m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();

                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                m_configurationSource.IsInputActive = true;

                SetConfigurationSourceTheme();

                micaController = new MicaController();
                micaController.Kind = micaKind;

                if (customTransparency)
                {
                    micaController.TintColor = Color.FromArgb(alpha, red, green, blue);
                    micaController.TintOpacity = tintOpacity;
                }

                micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                micaController.SetSystemBackdropConfiguration(m_configurationSource);

                return true;
            }

            TrySetAcrylicBackdrop(alpha, red, green, blue, tintOpacity, luminOpacity, customTransparency);

            return false;
        }

        bool TrySetAcrylicBackdrop(byte alpha, byte red, byte green, byte blue, float tintOpacity, float luminOpacity, bool customTransparency)
        {
            if (DesktopAcrylicController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();
                m_configurationSource = new SystemBackdropConfiguration();

                this.Activated += Window_Activated;
                this.Closed += Window_Closed;

                m_configurationSource.IsInputActive = true;

                SetConfigurationSourceTheme();

                acrylicController = new DesktopAcrylicController();

                acrylicController.TintColor = Color.FromArgb(alpha, red, green, blue);
                acrylicController.TintOpacity = tintOpacity;
                acrylicController.LuminosityOpacity = luminOpacity;

                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                acrylicController.SetSystemBackdropConfiguration(m_configurationSource);

                return true;
            }

            return false;
        }

        private void Window_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = true;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            RegisterBar();

            if (micaController != null)
            {
                micaController.Dispose();
                micaController = null;
            }
            if (acrylicController != null)
            {
                acrylicController.Dispose();
                acrylicController = null;
            }

            this.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark:
                    m_configurationSource.Theme = SystemBackdropTheme.Dark;

                    if (acrylicController != null)
                    {
                        acrylicController.TintColor = Color.FromArgb(255, 0, 0, 0);
                    }
                    break;
                case ElementTheme.Light:
                    m_configurationSource.Theme = SystemBackdropTheme.Light;

                    if (acrylicController != null)
                    {
                        acrylicController.TintColor = Color.FromArgb(255, 255, 255, 255);
                    }
                    break;
                case ElementTheme.Default:
                    m_configurationSource.Theme = SystemBackdropTheme.Default;

                    if (acrylicController != null)
                    {
                        acrylicController.TintColor = Color.FromArgb(255, 0, 0, 0);
                    }
                    break;
            }
        }
        #endregion

        #region AppBar
        private void RegisterBar()
        {
            APPBARDATA abd = new APPBARDATA();

            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hWnd;

            if (!fBarRegistered)
            {
                uCallBack = RegisterWindowMessage("AppBarMessage");
                abd.uCallbackMessage = uCallBack;

                uint ret = SHAppBarMessage((int)ABMsg.ABM_NEW, ref abd);
                bool regShellHook = RegisterShellHookWindow(hWnd);
                fBarRegistered = true;

                m_explorerManager.ToggleAutoHideExplorer(true);
                ABSetPos();
                m_explorerManager.ToggleAutoHideExplorer(false);
                m_explorerManager.HideTaskbar();
                SetWindowPos(hWnd, (IntPtr)WindowZOrder.HWND_TOPMOST, 0, 0, 0, 0, (int)SWPFlags.SWP_NOMOVE | (int)SWPFlags.SWP_NOSIZE | (int)SWPFlags.SWP_SHOWWINDOW);
            }
            else
            {
                SHAppBarMessage((int)ABMsg.ABM_REMOVE, ref abd);

                bool deRegShellHook = DeregisterShellHookWindow(hWnd);

                fBarRegistered = false;
            }
        }

        private void ABSetPos()
        {
            APPBARDATA abd = new APPBARDATA();

            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hWnd;
            abd.uEdge = (int)ABEdge.ABE_BOTTOM;

            abd.rc.left = 0;
            abd.rc.right = m_envService.MonitorWidth;

            if (abd.uEdge == (int)ABEdge.ABE_TOP)
            {
                abd.rc.top = 0;
                abd.rc.bottom = 48;
            }
            else
            {
                abd.rc.bottom = m_envService.MonitorHeight;
                abd.rc.top = abd.rc.bottom - 46;
            }

            SHAppBarMessage((int)ABMsg.ABM_QUERYPOS, ref abd);

            switch (abd.uEdge)
            {
                case (int)ABEdge.ABE_LEFT:
                    abd.rc.right = abd.rc.left + 48;
                    break;
                case (int)ABEdge.ABE_RIGHT:
                    abd.rc.left = abd.rc.right - 48;
                    break;
                case (int)ABEdge.ABE_TOP:
                    abd.rc.bottom = abd.rc.top + 48;
                    break;
                case (int)ABEdge.ABE_BOTTOM:
                    abd.rc.top = abd.rc.bottom - 48;
                    break;
            }

            SHAppBarMessage((int)ABMsg.ABM_SETPOS, ref abd);
            MoveWindow(abd.hWnd, abd.rc.left, abd.rc.top, abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top, true);
        }
        #endregion
    }
}