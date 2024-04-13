#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using GyroShell.Library.Services.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using GyroShell.Library.ViewModels;

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