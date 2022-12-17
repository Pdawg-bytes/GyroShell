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
using ShowAndHideTaskbar;
using System.Threading;
using System.Timers;
using System.Linq.Expressions;
using Microsoft.Win32;
using Windows.UI.Core;
using static System.Net.Mime.MediaTypeNames;

namespace GyroShell
{
    public sealed partial class MainWindow : Window
    {
        AppWindow m_appWindow;

        [DllImport("User32.dll")]
        public static extern int keybd_event(Byte bVk, Byte bScan, long dwFlags, long dwExtraInfo);
        public const byte UP = 2;
        public const byte CTRL = 17;
        public const byte ESC = 27;
        public const byte WIN = 91;
        public const byte A = 65;

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

            // TaskbarManager.ShowTaskbar();

            // Init stuff
            TimeAndDate();
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
            dateTimeUpdate.Interval = new TimeSpan(1000);
            dateTimeUpdate.Tick += UpdateMethod;
            dateTimeUpdate.Start();
        }
        private void UpdateMethod(object sender, object e)
        {
            TimeText.Text = DateTime.Now.ToString("t");
            DateText.Text = DateTime.Now.ToString("M");
        }

        private void SystemControls_Click(object sender, RoutedEventArgs e)
        {
            if (SystemControls.IsChecked == true)
            {
                keybd_event(WIN, 0, 0, 0);
                keybd_event(A, 0, 0, 0);

                keybd_event(WIN, 0, UP, 0);
                keybd_event(A, 0, UP, 0);
            }
            else
            {
                keybd_event(ESC, 0, 0, 0);
                keybd_event(ESC, 0, UP, 0);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

            if (StartButton.IsChecked == true)
            {
                keybd_event(CTRL, 0, 0, 0);
                keybd_event(ESC, 0, 0, 0);

                keybd_event(CTRL, 0, UP, 0);
                keybd_event(ESC, 0, UP, 0);
            }
            else
            {
                keybd_event(ESC, 0, 0, 0);
                keybd_event(ESC, 0, UP, 0);
            }
        }

        private async void ActionCenter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = new Uri("ms-actioncenter://");
                var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            catch (Exception NotifURI)
            {
                Debug.WriteLine(NotifURI.ToString());
            }
        }
    } 
}
