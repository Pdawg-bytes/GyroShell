using Microsoft.UI.Windowing;
using Microsoft.UI;
using System;
using Windows.Graphics;

using static GyroShell.Helpers.Win32.Win32Interop;

using AppWindow = Microsoft.UI.Windowing.AppWindow;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using GyroShell.Helpers;
using Microsoft.UI.Composition.SystemBackdrops;
using Windows.UI;
using WinRT;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using GyroShell.Settings;
using Microsoft.UI.Xaml.Media.Animation;
using System.Diagnostics;

namespace GyroShell.Controls
{
    internal partial class StartupScreen : Window
    {
        private AppWindow m_AppWindow;

        private IntPtr hWnd;

        private Stopwatch sw;

        internal StartupScreen()
        {
            this.InitializeComponent();
            TrySetAcrylicBackdrop();

            OverlappedPresenter presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);
            m_AppWindow = GetAppWindowForCurrentWindow();
            m_AppWindow.SetPresenter(AppWindowPresenterKind.Default);

            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            // Hide in ALT+TAB view
            int exStyle = (int)GetWindowLongPtr(hWnd, -20);
            exStyle |= 128;
            SetWindowLongPtr(hWnd, -20, (IntPtr)exStyle);

            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

            Title = "GyroShell Startup Host";
            PackageVersion version = Package.Current.Id.Version;
            VersionText.Text = "Version: " + string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            int windowWidth = 550;
            int windowHeight = 300;
            int windowX = (screenWidth - windowWidth) / 2;
            int windowY = (screenHeight - windowHeight) / 2;

            appWindow.Resize(new SizeInt32 { Width = windowWidth, Height = windowHeight });
            appWindow.Move(new PointInt32 { X = windowX, Y = windowY });
            appWindow.MoveInZOrderAtTop();

            sw = new Stopwatch();
            sw.Start();
        }

        #region Window Handling
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
        #endregion

        #region Backdrop Stuff
        WindowsSystemDispatcherQueueHelper m_wsdqHelper;
        DesktopAcrylicController acrylicController;
        SystemBackdropConfiguration m_configurationSource;
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

                acrylicController.TintColor = Color.FromArgb(255, 0, 0, 0);
                acrylicController.TintOpacity = 0.3f;
                acrylicController.LuminosityOpacity = 0.2f;

                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                acrylicController.SetSystemBackdropConfiguration(m_configurationSource);

                return true;
            }

            return false;
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = true;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
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
    }
}
