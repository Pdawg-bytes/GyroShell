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
using Windows.UI.WindowManagement;
using Windows.ApplicationModel.Core;
using GyroShell.Controls;

namespace GyroShell.Settings
{
    public sealed partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            this.InitializeComponent();

            // Window Handling
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.MoveInZOrderAtTop();
            appWindow.Resize(new SizeInt32 { Width = 1300, Height = 700});
            appWindow.Move(new PointInt32 { X = 200, Y = 200 });
            contentFrame.Navigate(typeof(BarSettings));

            ExtendsContentIntoTitleBar = true;
            Title = "GyroShell Settings";
            SetTitleBar(AppTitleBar);
            TrySetMicaBackdrop();
        }

        #region Backdrop Stuff
        WindowsSystemDispatcherQueueHelper m_wsdqHelper;
        MicaController micaController;
        DesktopAcrylicController acrylicController;
        SystemBackdropConfiguration m_configurationSource;
        bool TrySetMicaBackdrop()
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
                micaController.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
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
                acrylicController.TintColor = Color.FromArgb(255, 0, 0, 0);
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
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            App.SerializeSettings();
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
            DefaultTaskbar.SettingInstances = 0;
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

        private void SettingNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            if (item != null)
            {
                switch (item.Tag.ToString())
                {
                    case "BarSettings":
                    default:
                        contentFrame.Navigate(typeof(BarSettings));
                        break;
                    case "Customization":
                        contentFrame.Navigate(typeof(Customization));
                        break;
                    case "AboutPage":
                        contentFrame.Navigate(typeof(AboutPage));
                        break;
                }
            }
        }
    }
}
