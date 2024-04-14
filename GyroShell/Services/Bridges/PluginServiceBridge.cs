#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using GyroShell.Library.Enums;
using GyroShell.Library.Services.Bridges;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GyroShell.Services.Bridges
{
    public class PluginServiceBridge : IPluginServiceBridge
    {
        public PluginServiceBridge()
        {

        }

        public IServiceProvider CreatePluginServiceProvider(ServiceType[] requestedServices)
        {
            ServiceCollection serviceCollection = new();
            foreach (var serviceType in requestedServices)
            {
                switch (serviceType)
                {
                    case ServiceType.InternalLauncher:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<IInternalLauncher>());
                        break;
                    case ServiceType.SettingsService:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<ISettingsService>());
                        break;
                    case ServiceType.ShellHookService:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<IShellHookService>());
                        break;
                    case ServiceType.TimeService:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<ITimeService>());
                        break;
                    case ServiceType.BatteryService:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<IBatteryService>());
                        break;
                    case ServiceType.NetworkService:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<INetworkService>());
                        break;
                    case ServiceType.SoundService:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<ISoundService>());
                        break;
                    case ServiceType.AppHelperService:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<IAppHelperService>());
                        break;
                    case ServiceType.BitmapService:
                        serviceCollection.AddTransient((Type)App.ServiceProvider.GetRequiredService<IBitmapHelperService>());
                        break;
                    case ServiceType.IconHelperService:
                        serviceCollection.AddTransient((Type)App.ServiceProvider.GetRequiredService<IIconHelperService>());
                        break;
                    case ServiceType.NotificationManager:
                        serviceCollection.AddSingleton(App.ServiceProvider.GetRequiredService<INotificationManager>());
                        break;
                    default:
                        throw new ArgumentException($"Unknown service type: {serviceType}");
                }
            }

            return serviceCollection.BuildServiceProvider();
        }
    }
}