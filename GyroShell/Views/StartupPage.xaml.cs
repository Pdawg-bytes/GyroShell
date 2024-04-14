#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace GyroShell.Views
{
    public sealed partial class StartupPage : Page
    {
        public StartupPage()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<StartupScreenViewModel>();
        }

        public StartupScreenViewModel ViewModel => (StartupScreenViewModel)this.DataContext;
    }
}
