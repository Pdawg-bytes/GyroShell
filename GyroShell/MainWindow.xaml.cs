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
        private IEnvironmentInfoService m_envService;
        private ISettingsService m_appSettings;
        private ITaskbarManagerService m_tbManager;
        private IAppHelperService m_appHelper;

        private IntPtr _oldWndProc;
        internal static IntPtr hWnd;

        internal static int uCallBack;

        internal static bool fBarRegistered = false;
        private bool finalOpt;

        internal MainWindow()
        {
            this.InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>();
            m_appSettings = App.ServiceProvider.GetRequiredService<ISettingsService>();
            m_appHelper = App.ServiceProvider.GetRequiredService<IAppHelperService>();
            m_tbManager = App.ServiceProvider.GetRequiredService<ITaskbarManagerService>();

            m_tbManager.Initialize();

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
            RegisterWinEventHook();
            _oldWndProc = SetWndProc(WindowProcess);
            MonitorSummon();
            TaskbarFrame.Navigate(typeof(Controls.DefaultTaskbar), null, new SuppressNavigationTransitionInfo());
            SetBackdrop();
            ShellDDEInit(true);

            // Show GyroShell when everything is ready
            m_AppWindow.Show();
        }

        #region Window Handling
        private void OnProcessExit(object sender, EventArgs e)
        {
            m_tbManager.ShowTaskbar();
            //UnhookWinEvent(foregroundHook);
            //UnhookWinEvent(cloakedHook);
            //UnhookWinEvent(nameChangeHook);
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
            bool? option = m_appSettings.EnableCustomTransparency;

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
                        TrySetMicaBackdrop(MicaKind.BaseAlt, alpha, red, green, blue, tintOpacity, luminOpacity);
                    }
                    else
                    {
                        TrySetAcrylicBackdrop(alpha, red, green, blue, tintOpacity, luminOpacity);
                    }
                    break;
                case 1:
                    if (m_envService.IsWindows11)
                    {
                        TrySetMicaBackdrop(MicaKind.Base, alpha, red, green, blue, tintOpacity, luminOpacity);
                    }
                    else
                    {
                        TrySetAcrylicBackdrop(alpha, red, green, blue, tintOpacity, luminOpacity);
                    }
                    break;
                case 2:
                    TrySetAcrylicBackdrop(alpha, red, green, blue, tintOpacity, luminOpacity);
                    break;
            }
        }

        bool TrySetMicaBackdrop(MicaKind micaKind, byte alpha, byte red, byte green, byte blue, float tintOpacity, float luminOpacity)
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

                if (finalOpt == true)
                {
                    micaController.TintColor = Color.FromArgb(alpha, red, green, blue);
                    micaController.TintOpacity = tintOpacity;
                }

                micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                micaController.SetSystemBackdropConfiguration(m_configurationSource);

                return true;
            }

            TrySetAcrylicBackdrop(alpha, red, green, blue, tintOpacity, luminOpacity);

            return false;
        }

        bool TrySetAcrylicBackdrop(byte alpha, byte red, byte green, byte blue, float tintOpacity, float luminOpacity)
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
        internal void RegisterBar()
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

                m_tbManager.ToggleAutoHideExplorer(true);
                ABSetPos();
                m_tbManager.ToggleAutoHideExplorer(false);
                m_tbManager.HideTaskbar();
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

        #region Callbacks

        #region SetWinEventHook Init
        private int WM_ShellHook;

        private void RegisterWinEventHook()
        {
            WM_ShellHook = RegisterWindowMessage("SHELLHOOK");

            RegisterShellHook(hWnd, 3);
        }
        #endregion

        #region WndProc Init
        private static WndProcDelegate _currDelegate = null;
        internal static IntPtr SetWndProc(WndProcDelegate newProc)
        {
            _currDelegate = newProc;

            IntPtr newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newProc);

            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr(hWnd, GWLP_WNDPROC, newWndProcPtr);
            }
            else
            {
                return SetWindowLong(hWnd, GWLP_WNDPROC, newWndProcPtr);
            }
        }
        #endregion

        // WNDPROC Callback
        private IntPtr WindowProcess(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            /*Debug.WriteLine("------------");
            Debug.WriteLine("MESSAGE: " + (WM_CODE)message);
            Debug.WriteLine(wParam);
            Debug.WriteLine(lParam);*/
            if (message == WM_ShellHook)
            {
                //return HandleShellHook(wParam.ToInt32(), lParam);
            }

            return CallWindowProc(_oldWndProc, hwnd, message, wParam, lParam);
        }

        // WM_ShellHook Callback
        private IntPtr HandleShellHook(int iCode, IntPtr hwnd)
        {
            switch (iCode)
            {
                case HSHELL_GETMINRECT: //HSHELL_GETMINRECT
                    return new IntPtr(1);
                case (4 | 0x8000): // HSHELL_RUDEAPPACTIVATED
                    break;
                case HSHELL_WINDOWACTIVATED:
                    break;
                case HSHELL_WINDOWCREATED:
                    if (isUserWindow(hwnd))
                    {
                        Debug.WriteLine("Window created: " + m_appHelper.GetWindowTitle(hwnd) + " | Handle: " + (hwnd));
                        //indexedWindows.Add(hwnd);
                    }
                    break;
                case HSHELL_WINDOWDESTROYED:
                    /*if (indexedWindows.Contains(hwnd))
                    {
                        Debug.WriteLine("Window destroyed: " + GetWindowTitle(hwnd) + " | Handle: " + (hwnd));
                        //indexedWindows.Remove(hwnd);
                    }*/
                    break;
                case HSHELL_APPCOMMAND:
                    int appCommand = ((short)((((uint)hwnd) >> 16) & ushort.MaxValue)) & ~FAPPCOMMAND_MASK;

                    Debug.WriteLine("App command: " + appCommand);

                    if (appCommand == 8)
                    {
                        Debug.WriteLine("Volume muted");
                    }
                    break;
                case 16:
                    return new IntPtr(1);
                case HSHELL_REDRAW:
                    //Debug.WriteLine("Window redraw: " + GetWindowTitle(hwnd) + " | Handle: " + (hwnd));
                    break;
                case HSHELL_FULLSCREEN_ENABLED:
                    //m_AppWindow.Hide();
                    break;
                case HSHELL_FULLSCREEN_DISABLED:
                    //m_AppWindow.Show();
                    break;
                default:
                    Debug.WriteLine("Unknown shhook code: " + iCode + " with window: " + m_appHelper.GetWindowTitle(hwnd));
                    break;
            }
            return IntPtr.Zero;
        }
        #endregion
    }
}