using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using GyroShell.Library.Helpers.Composition;

using AppWindow = Microsoft.UI.Windowing.AppWindow;

namespace GyroShell.Library.Controls
{
    public partial class FlyoutWindow : Window
    {
        public FlyoutWindow()
        {
            this.InitializeComponent();

            this.SystemBackdrop = new TransparentTintBackdrop(new Windows.UI.Composition.Compositor(), new Windows.UI.Color { A = 0, R = 0, G = 0, B = 0 });

            OverlappedPresenter presenter = GetAppWindowAndPresenter();
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);
            AppWindow appWindow = GetAppWindowForCurrentWindow();

            appWindow.SetPresenter(AppWindowPresenterKind.Default);
            appWindow.Resize(new SizeInt32 { Width = 300, Height = 300 });
            appWindow.Move(new PointInt32 { X = 200, Y = 300 });
            appWindow.MoveInZOrderAtTop();
        }

        #region Window Handling
        private OverlappedPresenter GetAppWindowAndPresenter()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId WndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow _apw = AppWindow.GetFromWindowId(WndId);

            return _apw.Presenter as OverlappedPresenter;
        }
        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWndApp = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId WndIdApp = Win32Interop.GetWindowIdFromWindow(hWndApp);

            return AppWindow.GetFromWindowId(WndIdApp);
        }
        #endregion
    }
}
