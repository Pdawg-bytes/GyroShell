using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Helpers;
using GyroShell.Library.Services.Managers;
using GyroShell.Library.ViewModels;
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
        }
    }
}
