// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GyroShell.Controls
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartWindow : Window
    {
        AppWindow m_appWindow;
        public StartWindow()
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

            Title = "GyroShell Start Host";
            appWindow.MoveInZOrderAtTop();
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

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {

        }
    }
}
