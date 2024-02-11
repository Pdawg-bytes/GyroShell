using GyroShell.Controls;
using GyroShell.Views;
using GyroShell.Helpers;
using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using WinRT;
using GyroShell.Library.ViewModels;
using Windows.UI.Core;
using Windows.System;

namespace GyroShell.Controls
{
    public sealed partial class SettingsWindow : Window
    {
        private readonly ISettingsService m_appSettings;
        private readonly IEnvironmentInfoService m_envService;

        private bool _isDebugMenuOpen;

        public SettingsWindow()
        {
            this.InitializeComponent();

            RootGrid.DataContext = App.ServiceProvider.GetService<SettingsWindowViewModel>();

            m_appSettings = App.ServiceProvider.GetRequiredService<ISettingsService>();
            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>();

            // Window Handling
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.MoveInZOrderAtTop();
            contentFrame.Navigate(typeof(CustomizationSettingView));

            ExtendsContentIntoTitleBar = true;
            Title = "GyroShell Settings";
            SetTitleBar(AppTitleBar);

            TrySetMicaBackdrop();

            _isDebugMenuOpen = false;
            RootGrid.ProcessKeyboardAccelerators += RootGrid_ProcessKeyboardAccelerators;
        }

        private async void RootGrid_ProcessKeyboardAccelerators(UIElement sender, Microsoft.UI.Xaml.Input.ProcessKeyboardAcceleratorEventArgs args)
        {
            switch (args.Key)
            {
                case VirtualKey.Insert:
                    if (!_isDebugMenuOpen)
                    {
                        _isDebugMenuOpen = true;
                        DebugDialog dialog = new();
                        dialog.XamlRoot = this.Content.XamlRoot;
                        await dialog.ShowAsync();
                        _isDebugMenuOpen = false;
                    }
                    break;
            }
        }

        public SettingsWindowViewModel ViewModel => (SettingsWindowViewModel)RootGrid.DataContext;

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

                acrylicController.TintColor = Color.FromArgb(255, 32, 32, 32);
                acrylicController.TintOpacity = 0;
                acrylicController.LuminosityOpacity = 0.95f;

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

            m_envService.SettingsInstances = 0;
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
                        acrylicController.TintColor = Color.FromArgb(255, 50, 50, 50); 
                    } 
                    break;
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
                        contentFrame.Navigate(typeof(CustomizationSettingView));
                        break;
                    case "Modules":
                        contentFrame.Navigate(typeof(ModulesSettingView));
                        break;
                    case "AboutPage":
                        contentFrame.Navigate(typeof(AboutSettingView));
                        break;
                }
            }
        }
    }
}
