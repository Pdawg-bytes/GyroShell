using GyroShell.Controls;
using GyroShell.Helpers;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using WinRT;

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
            contentFrame.Navigate(typeof(Customization));

            int? iconStyle = App.localSettings.Values["iconStyle"] as int?;
            ExtendsContentIntoTitleBar = true;
            Title = "GyroShell Settings";
            SetTitleBar(AppTitleBar);
            TrySetMicaBackdrop();

            if (iconStyle != null)
            {
                switch (iconStyle)
                {
                    case 0:
                    default:
                        TopIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        break;
                    case 1:
                        TopIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                        break;
                }
            }
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
                m_configurationSource = new SystemBackdropConfiguration();

                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                micaController = new MicaController();

                micaController.Kind = MicaKind.Base;
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

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
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
                    case "Customization":
                    default:
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
