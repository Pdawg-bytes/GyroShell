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
using GyroShell.Controls;

namespace GyroShell
{
    public sealed partial class MainWindow : Window
    {
        AppWindow m_appWindow;
        bool reportRequested = false;
        public static int SettingInstances = 0;
        public static MicaKind micaKind;
        public static bool useAcrylic = false;

        #region Win32 Stuff
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);
        #endregion

        public MainWindow()
        {
            this.InitializeComponent();

            // Removes titlebar
            var presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = true;
            presenter.SetBorderAndTitleBar(false, false);
            m_appWindow = GetAppWindowForCurrentWindow();
            m_appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            m_appWindow.Show();

            // Resize Window
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);



            int ScreenWidth = (int)Bounds.Width;
            appWindow.Resize(new SizeInt32 { Width = ScreenWidth, Height = 50 });
            Title = "GyroShell";
            appWindow.MoveInZOrderAtTop();

            TaskbarManager.ShowTaskbar();

            // Init stuff
            TimeAndDate();
            DetectBatteryPresence();
            MonitorSummon();
            MoveWindow();
            micaKind = MicaKind.BaseAlt;
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

        private void MoveWindow()
        {
            /*Window window = this;
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);

            var placement = new WINDOWPLACEMENT
            {
                length = Marshal.SizeOf<WINDOWPLACEMENT>(),
                flags = 0,
                ptMinPosition = new POINT { x = 0, y = 0 },
                ptMaxPosition = new POINT { x = 0, y = 0 },
                rcNormalPosition = new RECT
                {
                    Left = (int)window.Bounds.Width,
                    Top = (int)window.Bounds.Height,
                    Right = (int)window.Bounds.Width,
                    Bottom = (int)window.Bounds.Height,
                }
            };

            window.SetWindowPlacement(windowHandle, ref placement);*/
        }

        private void MonitorSummon()
        {

            bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
            {
                return true;
            }

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
        string[] batteryIconsCharge = { "\uEBAE", "\uEBAC", "\uEBAD", "\uEBAE", "\uEBAF", "\uEBB0", "\uEBB1", "\uEBB2", "\uEBB3", "\uEBB4", "\uEBB5" };
        string[] batteryIcons = { "\uEBA0", "\uEBA1", "\uEBA2", "\uEBA3", "\uEBA4", "\uEBA5", "\uEBA6", "\uEBA7", "\uEBA8", "\uEBA9", "\uEBAA" };
        private void AggregateBattery()
        {
            var aggBattery = Battery.AggregateBattery;
            var report = aggBattery.GetReport();
            string charging = report.Status.ToString();
            double fullCharge = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
            double currentCharge = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
            double battLevel = Math.Ceiling((currentCharge / fullCharge) * 100);
            if (charging == "Charging" || charging == "Idle")
            {
                int indexCharge = (int)Math.Floor(battLevel / 10);
                BattStatus.Text = batteryIconsCharge[indexCharge];
            }
            else
            {
                int indexDischarge = (int)Math.Floor(battLevel / 10);
                BattStatus.Text = batteryIcons[indexDischarge];
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

        #region Backdrop Stuff
        WindowsSystemDispatcherQueueHelper m_wsdqHelper;
        MicaController micaController;
        DesktopAcrylicController acrylicController;
        SystemBackdropConfiguration m_configurationSource;
        bool TrySetMicaBackdrop()
        {
            if (MicaController.IsSupported() && useAcrylic == false)
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
                acrylicController.TintColor = Color.FromArgb(255,0,0,0);
                acrylicController.TintOpacity = 0;
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
                case ElementTheme.Dark: m_configurationSource.Theme = SystemBackdropTheme.Dark; if (acrylicController != null) { acrylicController.TintColor = Color.FromArgb(255, 0, 0, 0); } break;
                case ElementTheme.Light: m_configurationSource.Theme = SystemBackdropTheme.Light; if (acrylicController != null) { acrylicController.TintColor = Color.FromArgb(255, 255, 255, 255); } break;
                case ElementTheme.Default: m_configurationSource.Theme = SystemBackdropTheme.Default; if (acrylicController != null) { acrylicController.TintColor = Color.FromArgb(255, 50, 50, 50); } break;
            }
        }
        #endregion

        private void StartButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            StartFlyout.ShowAt(StartButton);
        }

        private void StartFlyout_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem selectedItem)
            {
                string shellOption = selectedItem.Tag.ToString();
                switch (shellOption)
                {
                    case "ShellSettings":
                        SettingInstances++;
                        if (SettingInstances <= 1)
                        {
                            Settings.SettingsWindow settingsWindow = new Settings.SettingsWindow();
                            settingsWindow.Activate();
                        }
                        break;
                }
            }
            else
            {
                throw new Exception("MenuFlyout sender variable error");
            }
        }
    }
}