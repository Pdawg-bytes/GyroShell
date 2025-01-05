#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Services.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using GyroShell.Library.ViewModels;
using GyroShell.Library.Controls;

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