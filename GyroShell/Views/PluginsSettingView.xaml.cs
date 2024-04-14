#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
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
