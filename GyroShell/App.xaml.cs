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

using GyroShell.Controls;
using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace GyroShell
{
    public partial class App : Application
    {
        internal static StartupWindow startupScreen;
        private Window m_window;
        private IntPtr handle;

        public App()
        {
            this.InitializeComponent();
        }

        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ConfigureServices();
            PreloadServices();

            await LoadStartupScreenContentAsync();
            m_window = new MainWindow();
            m_window.Activate();
        }

        private async Task LoadStartupScreenContentAsync()
        {
            startupScreen = new StartupWindow();
            startupScreen.Activate();
            await Task.Delay(200);
        }
    }
}
