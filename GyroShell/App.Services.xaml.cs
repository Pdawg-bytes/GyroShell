using GyroShell.Library.Services;
using GyroShell.Services;
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
                .AddSingleton<IEnvironmentService, EnvironmentService>();

            m_serviceProvider = collection.BuildServiceProvider(true);
        }

        private void PreloadServices()
        {
            _ = m_serviceProvider.GetRequiredService<IEnvironmentService>();
        }
    }
}
