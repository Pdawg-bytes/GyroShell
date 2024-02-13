using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using GyroShell.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

using static GyroShell.Library.Helpers.Win32.Win32Interop;
using static GyroShell.Library.Helpers.Win32.WindowChecks;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.ViewModels;
using System.Reflection.Metadata;

namespace GyroShell.Controls
{
    public sealed partial class DefaultTaskbar : Page
    {
        private IExplorerManagerService m_explorerManager;

        public DefaultTaskbar()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<DefaultTaskbarViewModel>();

            m_explorerManager = App.ServiceProvider.GetRequiredService<IExplorerManagerService>();

            m_explorerManager.NotifyWinlogonShowShell();
        }

        public DefaultTaskbarViewModel ViewModel => (DefaultTaskbarViewModel)this.DataContext;

        private void StartButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            StartFlyout.ShowAt(StartButton);
        }

        private void MainShellGrid_Loaded(object sender, RoutedEventArgs e)
        {
            App.startupScreen.Close();
        }
    }
}