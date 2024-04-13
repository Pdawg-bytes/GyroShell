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

using GyroShell.Library.Services.Bridges;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using GyroShell.Library.ViewModels;
using GyroShell.Services.Bridges;
using GyroShell.Services.Environment;
using GyroShell.Services.Hardware;
using GyroShell.Services.Helpers;
using GyroShell.Services.Managers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GyroShell
{
    partial class App
    {
        private IServiceProvider m_serviceProvider;

        public static IServiceProvider ServiceProvider
        {
            get
            {
                IServiceProvider serviceProvider = (Current as App).m_serviceProvider ??
                    throw new InvalidOperationException("Service provider was not initialized before accessing.");

                return serviceProvider;
            }
        }

        private void ConfigureServices()
        {
            IServiceCollection collection = new ServiceCollection()
                .AddTransient<IBitmapHelperService, BitmapHelperService>()
                .AddTransient<IIconHelperService, IconHelperService>()
                .AddTransient<IPluginServiceBridge, PluginServiceBridge>()
                .AddSingleton<IAppHelperService, AppHelperService>()
                .AddSingleton<IEnvironmentInfoService, EnvironmentInfoService>()
                .AddSingleton<IShellHookService, ShellHookService>()
                .AddSingleton<ISettingsService, SettingsService>()
                .AddSingleton<INetworkService, NetworkService>()
                .AddSingleton<IBatteryService, BatteryService>()
                .AddSingleton<ISoundService, SoundService>()
                .AddSingleton<IExplorerManagerService, ExplorerManagerService>()
                .AddSingleton<IPluginManager, PluginManager>()
                .AddSingleton<IInternalLauncher, InternalLauncher>()
                .AddSingleton<IDispatcherService, DispatcherService>()
                .AddSingleton<INotificationManager, NotificationManager>()
                .AddSingleton<ITimeService, TimeService>()
                .AddTransient<StartupScreenViewModel>()
                .AddTransient<AboutSettingViewModel>()
                .AddTransient<PluginSettingViewModel>()
                .AddTransient<SettingsWindowViewModel>()
                .AddTransient<DefaultTaskbarViewModel>()
                .AddTransient<CustomizationSettingViewModel>();

            m_serviceProvider = collection.BuildServiceProvider(true);
        }

        private void PreloadServices()
        {
            _ = m_serviceProvider.GetRequiredService<IEnvironmentInfoService>();
            _ = m_serviceProvider.GetRequiredService<IAppHelperService>();
            _ = m_serviceProvider.GetRequiredService<IBitmapHelperService>();
            _ = m_serviceProvider.GetRequiredService<IDispatcherService>();
            _ = m_serviceProvider.GetRequiredService<IPluginManager>();
        }
    }
}