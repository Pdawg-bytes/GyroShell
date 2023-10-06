using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
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
                .AddSingleton<IAppHelperService, AppHelperService>()
                .AddSingleton<IEnvironmentInfoService, EnvironmentInfoService>()
                .AddTransient<ISettingsService, SettingsService>()
                .AddSingleton<INetworkService, NetworkService>()
                .AddSingleton<IBatteryService, BatteryService>()
                .AddTransient<ITaskbarManagerService, TaskbarManagerService>();

            m_serviceProvider = collection.BuildServiceProvider(true);
        }

        private void PreloadServices()
        {
            _ = m_serviceProvider.GetRequiredService<IEnvironmentInfoService>();
            _ = m_serviceProvider.GetRequiredService<IAppHelperService>();
        }
    }
}
