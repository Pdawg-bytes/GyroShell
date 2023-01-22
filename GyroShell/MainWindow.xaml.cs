using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using Windows.Graphics;
using WindowsUdk.UI.Shell;
using GyroShell.Helpers;
using Windows.UI.Core;
using Windows.Devices.Power;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Controls;
using WinRT;
using Windows.UI;
using Microsoft.UI.Xaml.Input;
using CommunityToolkit.WinUI.Connectivity;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media.Animation;
using System.Reflection;
using System.Threading;
using Windows.Perception.Spatial.Preview;
using static GyroShell.Helpers.Win32Interop;
using System.Diagnostics;

namespace GyroShell
{
    public sealed partial class MainWindow : Window
    {
        AppWindow m_AppWindow;
 
        public MainWindow()
        {
            this.InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            TaskbarManager.Init();

            // Removes titlebar
            var presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);
            m_AppWindow = GetAppWindowForCurrentWindow();
            m_AppWindow.SetPresenter(AppWindowPresenterKind.Default);

            // Resize Window
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            if (OSVersion.IsWin11())
            {   
                var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
                DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
            }

            // Hide in ALT+TAB view
            int exStyle = (int)GetWindowLongPtr(hWnd, -20);
            exStyle |= 128;
            SetWindowLongPtr(hWnd, -20, (IntPtr)exStyle);

            TaskbarManager.SetHeight(0, 0, 0, 48);
            Thread.Sleep(1000); //TODO: Stop the window message from moving our window into the wokring area

            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);
            int barHeight = 50;

            Title = "GyroShell";
            appWindow.Resize(new SizeInt32 { Width = screenWidth + 2, Height = barHeight });
            appWindow.Move(new PointInt32 { X = -1, Y = screenHeight - 49 });
            appWindow.MoveInZOrderAtTop();

            // Init stuff
            MonitorSummon();
            TrySetMicaBackdrop(MicaKind.BaseAlt);
            TaskbarFrame.Navigate(typeof(Controls.DefaultTaskbar), null, new SuppressNavigationTransitionInfo());
           
            //Show GyroShell when everything is ready
            m_AppWindow.Show();
            TaskbarManager.HideTaskbar();
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            TaskbarManager.ShowTaskbar();
        }

        private OverlappedPresenter GetAppWindowAndPresenter()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId WndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var _apw = AppWindow.GetFromWindowId(WndId);

            return _apw.Presenter as OverlappedPresenter;
        }
        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWndApp = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId WndIdApp = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWndApp);

            return AppWindow.GetFromWindowId(WndIdApp);
        }

        public void MonitorSummon()
        {

            bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData)
            {
                return true;
            }

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
        }

        #region Backdrop Stuff
        WindowsSystemDispatcherQueueHelper m_wsdqHelper;
        MicaController micaController;
        DesktopAcrylicController acrylicController;
        SystemBackdropConfiguration m_configurationSource;

        bool TrySetMicaBackdrop(MicaKind micaKind)
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
                micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();
                micaController.Kind = micaKind;
                micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true;
            }
            TrySetAcrylicBackdrop();
            return false;
        }
        bool TrySetAcrylicBackdrop()
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
                acrylicController.TintColor = Color.FromArgb(255, 52, 52, 52);
                acrylicController.TintOpacity = 0.1f;
                acrylicController.LuminosityOpacity = 1;
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
                case ElementTheme.Dark: m_configurationSource.Theme = SystemBackdropTheme.Dark; if (acrylicController != null) { acrylicController.TintColor = Color.FromArgb(255, 0, 0, 0); } break;
                case ElementTheme.Light: m_configurationSource.Theme = SystemBackdropTheme.Light; if (acrylicController != null) { acrylicController.TintColor = Color.FromArgb(255, 255, 255, 255); } break;
                case ElementTheme.Default: m_configurationSource.Theme = SystemBackdropTheme.Default; if (acrylicController != null) { acrylicController.TintColor = Color.FromArgb(255, 50, 50, 50); } break;
            }
        }
        #endregion
    }
}