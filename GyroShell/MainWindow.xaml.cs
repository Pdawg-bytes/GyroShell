using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using Windows.Graphics;
using System.Diagnostics;
using WindowsUdk.UI.Shell;
using GyroShell.Helpers;
using Windows.UI.Core;
using Windows.Devices.Power;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace GyroShell
{
    public sealed partial class MainWindow : Window
    {
        AppWindow m_appWindow;
        bool reportRequested = false;

        [DllImport("User32.dll")]
        public static extern void GetSystemPowerStatus();

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);
        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        public MainWindow()
        {
            this.InitializeComponent();

            // Removes titlebar
            var presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = true;
            presenter.SetBorderAndTitleBar(true, false);
            m_appWindow = GetAppWindowForCurrentWindow();
            m_appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            m_appWindow.Show();

            // Resize Window
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            int ScreenWidth = (int)Bounds.Width;
            appWindow.Resize(new SizeInt32 { Width = ScreenWidth, Height = 50 });

            appWindow.MoveInZOrderAtTop();

            TaskbarManager.HideTaskbar();

            // Init stuff
            TimeAndDate();
            DetectBatteryPresence();
            MonitorSummonAsync();
            TrySetMicaBackdrop();
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
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

        private async void MonitorSummonAsync()
        {
            bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
            {
                return true;
            }

            [DllImport("user32.dll")]
            static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
        }

        private void TimeAndDate()
        {
            DispatcherTimer dateTimeUpdate = new DispatcherTimer();
            dateTimeUpdate.Tick += DTUpdateMethod;
            dateTimeUpdate.Interval = new TimeSpan(1000000);
            dateTimeUpdate.Start();
        }
        private void DTUpdateMethod(object sender, object e)
        {
            TimeText.Text = DateTime.Now.ToString("t");
            DateText.Text = DateTime.Now.ToString("M");
        }

        private void DetectBatteryPresence()
        {
            var aggDetectBattery = Battery.AggregateBattery;
            var report = aggDetectBattery.GetReport();
            string ReportResult = report.Status.ToString();
            if (ReportResult == "NotPresent")
            {
                BattStatus.Visibility = Visibility.Collapsed;
            }
            else
            {
                BattStatus.Visibility = Visibility.Visible;
                reportRequested = true;
                AggregateBattery();
            }
        }
        private void AggregateBattery()
        {
            var aggBattery = Battery.AggregateBattery;
            var report = aggBattery.GetReport();
            string charging = report.Status.ToString();
            double fullCharge = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
            double currentCharge = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
            double battLevel = (currentCharge / fullCharge) * 100;
            if (charging == "Charging" || charging == "Idle")
            {
                if (battLevel >= 100)
                {
                    BattStatus.Text = "\uEBB5";
                }
                else if (battLevel >= 90)
                {
                    BattStatus.Text = "\uEBB4";
                }
                else if (battLevel >= 80)
                {
                    BattStatus.Text = "\uEBB3";
                }
                else if (battLevel >= 70)
                {
                    BattStatus.Text = "\uEBB2";
                }
                else if (battLevel >= 60)
                {
                    BattStatus.Text = "\uEBB1";
                }
                else if (battLevel >= 50)
                {
                    BattStatus.Text = "\uEBB0";
                }
                else if (battLevel >= 40)
                {
                    BattStatus.Text = "\uEBAF";
                }
                else if (battLevel >= 30)
                {
                    BattStatus.Text = "\uEBAE";
                }
                else if (battLevel >= 20)
                {
                    BattStatus.Text = "\uEBAD";
                }
                else if (battLevel >= 10)
                {
                    BattStatus.Text = "\uEBAC";
                }
                else if (battLevel >= 0)
                {
                    BattStatus.Text = "\uEBAB";
                }
            }
            else
            {
                if (battLevel >= 100)
                {
                    BattStatus.Text = "\uEBAA";
                }
                else if (battLevel >= 90)
                {
                    BattStatus.Text = "\uEBA9";
                }
                else if (battLevel >= 80)
                {
                    BattStatus.Text = "\uEBA8";
                }
                else if (battLevel >= 70)
                {
                    BattStatus.Text = "\uEBA7";
                }
                else if (battLevel >= 60)
                {
                    BattStatus.Text = "\uEBA6";
                }
                else if (battLevel >= 50)
                {
                    BattStatus.Text = "\uEBA5";
                }
                else if (battLevel >= 40)
                {
                    BattStatus.Text = "\uEBA4";
                }
                else if (battLevel >= 30)
                {
                    BattStatus.Text = "\uEBA3";
                }
                else if (battLevel >= 20)
                {
                    BattStatus.Text = "\uEBA2";
                }
                else if (battLevel >= 10)
                {
                    BattStatus.Text = "\uEBA1";
                }
                else if (battLevel >= 0)
                {
                    BattStatus.Text = "\uEBA0";
                }
            }
        }

        private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            if (reportRequested)
            {
                DispatcherQueue.TryEnqueue((Microsoft.UI.Dispatching.DispatcherQueuePriority)CoreDispatcherPriority.Normal, () =>
                {
                   AggregateBattery();
                });
            }
        }

        private async void SystemControls_Click(object sender, RoutedEventArgs e)
        {
            if (SystemControls.IsChecked == true)
            {
                ShellViewCoordinator controlsC = new ShellViewCoordinator(ShellView.ControlCenter);
                await controlsC.TryShowAsync(new ShowShellViewOptions());
            }
            else
            {

            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartButton.IsChecked == true)
            {
                ShellViewCoordinator startC = new ShellViewCoordinator(ShellView.Start);
                await startC.TryShowAsync(new ShowShellViewOptions());
            }
            else
            {
                
            }
        }

        private async void ActionCenter_Click(object sender, RoutedEventArgs e)
        {
            if (ActionCenter.IsChecked == true)
            {
                ShellViewCoordinator actionC = new ShellViewCoordinator(ShellView.ActionCenter);
                await actionC.TryShowAsync(new ShowShellViewOptions());
            }
            else
            {

            }
        }

        WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See separate sample below for implementation
        MicaController micaController;
        DesktopAcrylicController acrylicController;
        SystemBackdropConfiguration m_configurationSource;
        bool TrySetMicaBackdrop()
        {
            if (MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                micaController.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt;

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }
            TrySetAcrylicBackdrop();
            return false; // Mica is not supported on this system
        }
        bool TrySetAcrylicBackdrop()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_configurationSource = new SystemBackdropConfiguration();
                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                acrylicController = new DesktopAcrylicController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Acrylic is not supported on this system
        }

        private void Window_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = true;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use this closed window.
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
            TaskbarManager.ShowTaskbar();
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
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }
    }
}
