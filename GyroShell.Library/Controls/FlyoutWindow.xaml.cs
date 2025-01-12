using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using GyroShell.Library.Helpers.Composition;

using AppWindow = Microsoft.UI.Windowing.AppWindow;
using static GyroShell.Library.Helpers.Win32.Win32Interop;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
using Windows.UI;
using System.Runtime.InteropServices;

namespace GyroShell.Library.Controls
{
    internal partial class FlyoutWindow : Window
    {
        AppWindow appWindow;

        internal FlyoutWindow()
        {
            this.InitializeComponent();

            this.SystemBackdrop = new TransparentTintBackdrop(new Windows.UI.Composition.Compositor(), new Windows.UI.Color { A = 0, R = 0, G = 0, B = 0 });

            OverlappedPresenter presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);
            appWindow.SetPresenter(AppWindowPresenterKind.Default);

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            int exStyle = (int)GetWindowLongPtr(hWnd, -20);
            exStyle |= 128 | WS_EX_LAYERED;
            SetWindowLongPtr(hWnd, GWL_EXSTYLE, (IntPtr)exStyle);

            DWMWINDOWATTRIBUTE cornerAttrib = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            int cornerPref = (int)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
            DwmSetWindowAttribute(hWnd, cornerAttrib, ref cornerPref, sizeof(uint));

            DWMWINDOWATTRIBUTE shadowAttrib = DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY;
            int shadowPref = (int)DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;
            DwmSetWindowAttribute(hWnd, shadowAttrib, ref shadowPref, sizeof(uint));

            appWindow.Resize(new SizeInt32 { Width = 300, Height = 300 });
            appWindow.Move(new PointInt32 { X = 200, Y = 300 });
            appWindow.MoveInZOrderAtTop();

            appWindow.Hide();
        }

        private OverlappedPresenter GetAppWindowAndPresenter()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            appWindow = AppWindow.GetFromWindowId(wndId);

            return appWindow.Presenter as OverlappedPresenter;
        }

        internal void ShowWindow() { if (appWindow != null) appWindow.Show(); }
        internal void HideWindow() { if (appWindow != null) appWindow.Hide(); }
    }
}