#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Views;
using GyroShell.Helpers;
using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;
using WinRT;
using GyroShell.Library.ViewModels;
using Windows.System;
using GyroShell.Library.Services.Managers;

namespace GyroShell.Controls
{
    public sealed partial class SettingsWindow : Window
    {
        private readonly IPluginManager _pluginManager;
        private readonly IEnvironmentInfoService _envService;
        private readonly IInternalLauncher _internalLauncher;

        private bool _isDebugMenuOpen;

        public SettingsWindow()
        {
            this.InitializeComponent();

            RootGrid.DataContext = App.ServiceProvider.GetService<SettingsWindowViewModel>();

            _pluginManager = App.ServiceProvider.GetRequiredService<IPluginManager>();
            _envService = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>();
            _internalLauncher = App.ServiceProvider.GetRequiredService<IInternalLauncher>();

            // Window Handling
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
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
        WindowsSystemDispatcherQueueHelper _wsdqHelper;
        MicaController micaController;
        DesktopAcrylicController acrylicController;
        SystemBackdropConfiguration _configurationSource;
        bool TrySetMicaBackdrop()
        {
            if (MicaController.IsSupported())
            {
                _wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                _wsdqHelper.EnsureWindowsSystemDispatcherQueueController();
                _configurationSource = new SystemBackdropConfiguration();

                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                _configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                micaController = new MicaController();

                micaController.Kind = MicaKind.Base;
                micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                micaController.SetSystemBackdropConfiguration(_configurationSource);

                return true;
            }

            TrySetAcrylicBackdrop();

            return false;
        }
        bool TrySetAcrylicBackdrop()
        {
            if (DesktopAcrylicController.IsSupported())
            {
                _wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                _wsdqHelper.EnsureWindowsSystemDispatcherQueueController();
                _configurationSource = new SystemBackdropConfiguration();

                this.Activated += Window_Activated;
                this.Closed += Window_Closed;

                _configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                acrylicController = new DesktopAcrylicController();

                acrylicController.TintColor = Color.FromArgb(255, 32, 32, 32);
                acrylicController.TintOpacity = 0;
                acrylicController.LuminosityOpacity = 0.95f;

                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                acrylicController.SetSystemBackdropConfiguration(_configurationSource);

                return true;
            }

            return false;
        }

        private void Window_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
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
            _configurationSource = null;

            _envService.SettingsInstances = 0;
            if (_pluginManager.IsUnloadRestartPending)
            {
                _internalLauncher.LaunchNewShellInstance();
            }
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark: 
                    _configurationSource.Theme = SystemBackdropTheme.Dark; 
                    if (acrylicController != null) 
                    { 
                        acrylicController.TintColor = Color.FromArgb(255, 0, 0, 0); 
                    } 
                    break;
                case ElementTheme.Light: 
                    _configurationSource.Theme = SystemBackdropTheme.Light; 
                    if (acrylicController != null) 
                    { 
                        acrylicController.TintColor = Color.FromArgb(255, 255, 255, 255); 
                    } 
                    break;
                case ElementTheme.Default: 
                    _configurationSource.Theme = SystemBackdropTheme.Default; 
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
                        contentFrame.Navigate(typeof(PluginsSettingView));
                        break;
                    case "AboutPage":
                        contentFrame.Navigate(typeof(AboutSettingView));
                        break;
                }
            }
        }
    }
}
