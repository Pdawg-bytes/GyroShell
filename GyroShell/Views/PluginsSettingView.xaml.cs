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

using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;

using static GyroShell.Services.Managers.PluginManager;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.ViewModels;

namespace GyroShell.Views
{
    public sealed partial class PluginsSettingView : Page
    {
        public PluginsSettingView()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<PluginSettingViewModel>();
        }

        public PluginSettingViewModel ViewModel => (PluginSettingViewModel)this.DataContext;
    }
}
