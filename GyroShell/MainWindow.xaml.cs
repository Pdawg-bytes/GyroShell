using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using WinRT;
using Windows.Foundation.Collections;
using Windows.UI.WindowManagement;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using Windows.Graphics;
using Windows.ApplicationModel.Activation;
using Microsoft.UI.Composition.SystemBackdrops;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using WindowsUdk.UI.Shell;
using GyroShell.Helpers;
using System.Linq.Expressions;
using Microsoft.Win32;
using Windows.UI.Core;
using static System.Net.Mime.MediaTypeNames;
using Windows.Devices.Power;

namespace GyroShell
{
    public sealed partial class MainWindow : Window
    {
        AppWindow m_appWindow;
        bool reportRequested = false;

        public MainWindow()
        {
            this.InitializeComponent();

            // Removes titlebar
            var presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = false;
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

            TaskbarManager.ShowTaskbar();

            // Init stuff
            TimeAndDate();
            DetectBatteryPresence();
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
        }

        private OverlappedPresenter GetAppWindowAndPresenter()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId WndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var _apw = AppWindow.GetFromWindowId(WndId);
            var handleObject = WinRT.Interop.WindowNative.GetWindowHandle(this);

            return _apw.Presenter as OverlappedPresenter;
        }
        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWndApp = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId WndIdApp = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWndApp);

            return AppWindow.GetFromWindowId(WndIdApp);
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
            var aggBattery = Battery.AggregateBattery;
            var report = aggBattery.GetReport();
            string ReportResult = report.Status.ToString();
            Debug.WriteLine(ReportResult);
            if (ReportResult == "NotPresent")
            {
                BattStatus.Visibility = Visibility.Collapsed;
            }
            else
            {
                BattStatus.Visibility = Visibility.Visible;
                reportRequested = true;
                double fullCharge = Convert.ToDouble(report.FullChargeCapacityInMilliwattHours);
                double currentCharge = Convert.ToDouble(report.RemainingCapacityInMilliwattHours);
                double battLevel = (currentCharge / fullCharge) * 100;
                Debug.WriteLine(battLevel);
            }
        }
        async private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            if (reportRequested)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    DetectBatteryPresence();
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
    } 
}
