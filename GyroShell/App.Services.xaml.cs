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

        private IServiceProvider ConfigureServices()
        {
            IServiceCollection collection = new ServiceCollection();

            IServiceProvider provider = collection.BuildServiceProvider(true);

            return provider;
        }

        private void PreloadServices()
        {
        }
    }
}
